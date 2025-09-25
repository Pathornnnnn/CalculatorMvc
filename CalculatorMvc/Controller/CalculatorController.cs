using Microsoft.AspNetCore.Mvc;
using System.Data;
using System.Text.RegularExpressions;

public class CalculatorController : Controller
{
    [HttpPost]
    public IActionResult EvaluateAjax([FromForm] string expression)
    {
        return Json(new { result = EvaluateExpression(expression) });
    }

    [HttpPost]
    public IActionResult EvaluateFetch([FromForm] string expression)
    {
        return Json(new { result = EvaluateExpression(expression) });
    }

    private string EvaluateExpression(string expr)
    {
        try
        {
            expr = ProcessSpecialFunctions(expr);
            double result = EvaluatePower(expr); // คำนวณ ^
            return result.ToString();
        }
        catch
        {
            return "Error";
        }
    }

    private string ProcessSpecialFunctions(string expr)
    {
        // เปอร์เซ็นต์ 10% → 0.1
        expr = Regex.Replace(expr, @"(\d+(\.\d+)?)%", m =>
        {
            double val = double.Parse(m.Groups[1].Value) / 100.0;
            return val.ToString();
        });

        return expr;
    }

    // ฟังก์ชันคำนวณ ^ (ยกกำลัง)
    private double EvaluatePower(string expr)
    {
        // ถ้าไม่มี ^ ใช้ DataTable.Compute ปกติ
        if (!expr.Contains("^"))
        {
            var dt = new DataTable();
            return Convert.ToDouble(dt.Compute(expr, ""));
        }

        // ถ้ามี ^ ให้แยก
        while (expr.Contains("^"))
        {
            // หา pattern "base^exp"
            var match = Regex.Match(expr, @"(\d+(\.\d+)?)\^(\d+(\.\d+)?)");
            if (!match.Success) break;

            double b = double.Parse(match.Groups[1].Value);
            double e = double.Parse(match.Groups[3].Value);
            double pow = Math.Pow(b, e);

            // แทนที่ base^exp ด้วยผลลัพธ์
            expr = expr.Replace(match.Value, pow.ToString());
        }

        var dtFinal = new DataTable();
        return Convert.ToDouble(dtFinal.Compute(expr, ""));
    }

}

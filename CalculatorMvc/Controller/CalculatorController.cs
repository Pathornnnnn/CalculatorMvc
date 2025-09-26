using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Text.RegularExpressions;

public class CalculatorController : Controller
{
    private static bool UseDegree = true;

    [HttpPost]
    public IActionResult EvaluateAjax([FromForm] string expression, [FromForm] string mode)
    {
        UpdateMode(mode);
        return Json(new { result = EvaluateExpression(expression) });
    }

    [HttpPost]
    public IActionResult EvaluateFetch([FromForm] string expression, [FromForm] string mode)
    {
        UpdateMode(mode);
        return Json(new { result = EvaluateExpression(expression) });
    }

    private void UpdateMode(string mode)
    {
        if (!string.IsNullOrEmpty(mode))
        {
            UseDegree = mode.Equals("deg", StringComparison.OrdinalIgnoreCase);
        }
    }

    private string EvaluateExpression(string expr)
    {
        try
        {
            expr = ProcessSpecialFunctions(expr);
            expr = ReplaceMathFunctions(expr);
            double result = EvaluatePower(expr);

            // ปัดผลลัพธ์ให้สวย (สูงสุด 10 ตำแหน่ง)
            double rounded = Math.Round(result, 10);
            return rounded.ToString("0.##########");
        }
        catch
        {
            return "Error";
        }
    }

    private string ProcessSpecialFunctions(string expr)
    {
        expr = Regex.Replace(expr, @"(\d+(\.\d+)?)%", m =>
        {
            double val = double.Parse(m.Groups[1].Value) / 100.0;
            return val.ToString();
        });
        return expr;
    }

    private string ReplaceMathFunctions(string expr)
    {
        expr = Regex.Replace(expr, @"√\(([^)]+)\)", m =>
        {
            double val = EvaluatePower(m.Groups[1].Value);
            return Math.Sqrt(val).ToString();
        });

        expr = Regex.Replace(expr, @"sin\(([^)]+)\)", m =>
        {
            double val = EvaluatePower(m.Groups[1].Value);
            if (UseDegree) val = val * Math.PI / 180;
            return Math.Sin(val).ToString();
        });

        expr = Regex.Replace(expr, @"cos\(([^)]+)\)", m =>
        {
            double val = EvaluatePower(m.Groups[1].Value);
            if (UseDegree) val = val * Math.PI / 180;
            return Math.Cos(val).ToString();
        });

        expr = Regex.Replace(expr, @"tan\(([^)]+)\)", m =>
        {
            double val = EvaluatePower(m.Groups[1].Value);
            if (UseDegree) val = val * Math.PI / 180;
            return Math.Tan(val).ToString();
        });

        expr = Regex.Replace(expr, @"log\(([^)]+)\)", m =>
        {
            double val = EvaluatePower(m.Groups[1].Value);
            return Math.Log10(val).ToString();
        });

        return expr;
    }

    private double EvaluatePower(string expr)
    {
        if (!expr.Contains("^"))
        {
            var dt = new DataTable();
            return Convert.ToDouble(dt.Compute(expr, ""));
        }

        while (expr.Contains("^"))
        {
            var match = Regex.Match(expr, @"(\d+(\.\d+)?)\^(\d+(\.\d+)?)");
            if (!match.Success) break;

            double b = double.Parse(match.Groups[1].Value);
            double e = double.Parse(match.Groups[3].Value);
            double pow = Math.Pow(b, e);

            expr = expr.Replace(match.Value, pow.ToString());
        }

        var dtFinal = new DataTable();
        return Convert.ToDouble(dtFinal.Compute(expr, ""));
    }
}

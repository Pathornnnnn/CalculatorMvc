using Microsoft.AspNetCore.Mvc;
using System;
using System.Data;
using System.Text.RegularExpressions;

public class CalculatorController : Controller
{
    [HttpPost]
    public IActionResult EvaluateAjax([FromForm] string expression, [FromForm] string mode)
    {
        return Json(new { result = EvaluateExpression(expression, mode) });
    }

    [HttpPost]
    public IActionResult EvaluateFetch([FromForm] string expression, [FromForm] string mode)
    {
        return Json(new { result = EvaluateExpression(expression, mode) });
    }

    private string EvaluateExpression(string expr, string mode)
    {
        try
        {
            bool useDegree = string.IsNullOrEmpty(mode) || mode.Equals("deg", StringComparison.OrdinalIgnoreCase);

            expr = ProcessSpecialFunctions(expr);
            expr = ReplaceMathFunctions(expr, useDegree);
            double result = EvaluatePower(expr);

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

    private string ReplaceMathFunctions(string expr, bool useDegree)
    {
        expr = Regex.Replace(expr, @"√\(([^)]+)\)", m =>
        {
            double val = EvaluatePower(m.Groups[1].Value);
            return Math.Sqrt(val).ToString();
        });

        expr = Regex.Replace(expr, @"sin\(([^)]+)\)", m =>
        {
            double val = EvaluatePower(m.Groups[1].Value);
            if (useDegree) val = val * Math.PI / 180;
            return Math.Sin(val).ToString();
        });

        expr = Regex.Replace(expr, @"cos\(([^)]+)\)", m =>
        {
            double val = EvaluatePower(m.Groups[1].Value);
            if (useDegree) val = val * Math.PI / 180;
            return Math.Cos(val).ToString();
        });

        expr = Regex.Replace(expr, @"tan\(([^)]+)\)", m =>
        {
            double val = EvaluatePower(m.Groups[1].Value);
            if (useDegree) val = val * Math.PI / 180;
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

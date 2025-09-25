namespace CalculatorMvc.Models
{
    public class Calculator
    {
        public double Calculate(double num1, double num2, string op)
        {
            return op switch
            {
                "+" => num1 + num2,
                "-" => num1 - num2,
                "*" => num1 * num2,
                "/" => num2 != 0 ? num1 / num2 : double.NaN,
                _ => double.NaN
            };
        }
    }
}

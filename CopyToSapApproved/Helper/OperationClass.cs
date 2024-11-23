using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CopyToSapApproved.Helper;
/// <summary>
/// Class to represent a single operation in the calculator.
/// </summary>
public class OperationClass
{
    public double Operand1 { get; set; }
    public double Operand2 { get; set; }
    public string Operator { get; set; }

    public double Execute()
    {
        return Operator switch
        {
            "+" => Operand1 + Operand2,
            "-" => Operand1 - Operand2,
            "×" => Operand1 * Operand2,
            "÷" => Operand2 != 0 ? Operand1 / Operand2 : throw new DivideByZeroException(),
            _ => throw new InvalidOperationException("Invalid operator"),
        };
    }
}
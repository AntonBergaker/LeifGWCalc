using System;
using System.Collections.Generic;
using System.Linq;

namespace LeifGWCalc
{
    public static class Calculator
    {
        public static string Calculate(string input, bool degrees)
        {
            input = input.ToLower().Replace(" ","");

            ValueSequence numbers = new ValueSequence(input);

            if (numbers.state == ValueSequenceState.Error)
            { return "Error"; }

            numbers = RunOperators(numbers,degrees);

            if (numbers.values.Count == 1 && numbers.values[0].type == ValueTypes.Value)
            { return numbers.ToString();}
            else
            { return "Error"; }
            
        }

        private static ValueSequence RunOperators(ValueSequence numbers, bool useDegrees)
        {
            List<Value> values = numbers.values;

            values = ExecuteParenthesis(values, useDegrees);
            values = ReplaceConstants(values);
            values = ExecuteRightOperators(values, new string[] { "sin", "cos", "tan", "root", "√" }, useDegrees);
            values = ExecuteBothOperators( values, new string[] { "^"});
            values = ExecuteBothOperators( values, new string[] { "*", "x", "/" });
            values = ExecuteBothOperators( values, new string[] { "+", "-"});
            values = MultiplyAllValues(values);

            return numbers;
        }

        private static List<Value> ReplaceConstants(List<Value> values)
        {
            Dictionary<string, double> Constants = new Dictionary<string, double>();
            Constants.Add("pi", Math.PI);
            Constants.Add("π", Math.PI);
            Constants.Add("e", Math.E);

            foreach (Value v in values)
            {
                if (v.type == ValueTypes.Operator)
                { if (Constants.ContainsKey(v.operation))
                    {
                        v.type = ValueTypes.Value;
                        v.value = Constants[v.operation];
                    }
                }
            }

            return values;
        }


        /// <summary>
        /// Multiplies all real values until it hits an operation
        /// </summary>
        private static List<Value> MultiplyAllValues(List<Value> values)
        {
            while (values.Count > 1)
            {
                if (values[0].type == ValueTypes.Value && values[1].type == ValueTypes.Value)
                {
                    values[1].value *= values[0].value;
                    values.RemoveAt(0);
                }
                else
                { break; }
            }
            return values;
        }
        private static List<Value> ExecuteParenthesis(List<Value> values, bool useDegrees)
        {
            for (int i = 0; i < values.Count; i++)
            {
                if (values[i].type == ValueTypes.ValueSequence)
                {
                    ValueSequence newSeq = RunOperators(values[i].sequence, useDegrees);
                    if (newSeq.values.Count == 1)
                    {
                        Value newVal = newSeq.values[0];
                        if (newVal.type == ValueTypes.Value)
                        {
                            values[i] = newVal;
                        }
                    }
                }
            }
            return values;
        }

        private static List<Value> ExecuteBothOperators(List<Value> values, string[] operators)
        {
            for (int i = 1; i < values.Count - 1; i++)
            {
                if (values[i].type == ValueTypes.Operator)
                {
                    if (operators.Contains(values[i].operation))
                    {
                        Value val1 = values[i - 1];
                        Value val2 = values[i + 1];
                        if (val1.type == ValueTypes.Value && val2.type == ValueTypes.Value)
                        {
                            Value newVal = ExecuteMath(val1, values[i], val2);
                            values.RemoveRange(i-1, 3);
                            values.Insert(i-1, newVal);
                            i -= 2;
                        }
                    }
                }
            }
            return values;
        }

        private static List<Value> ExecuteRightOperators(List<Value> values, string[] operators, bool useDegrees = true)
        {
            for (int i=0;i<values.Count-1;i++)
            {
                if (values[i].type == ValueTypes.Operator)
                {
                    if (operators.Contains(values[i].operation))
                    {
                        Value val2 = values[i + 1];
                        if (val2.type == ValueTypes.Value)
                        {
                            Value newVal = ExecuteMath(null, values[i], val2, useDegrees);
                            values.RemoveRange(i,2);
                            values.Insert(i,newVal);
                            i -= 1;
                        }
                    }
                }
            }
            return values;
        }

        private static Value ExecuteMath(Value number1, Value operation, Value number2, bool useDegrees = true)
        {
            double vf = 0d;
            double v1 = 0d;
            double v2 = 0d;
            if (number1 != null)
            { v1 = number1.value; }
            if (number2 != null)
            { v2 = number2.value; }
            switch (operation.operation)
            {
                case "+":
                    vf = v1 + v2;
                    break;
                case "-":
                    vf = v1 - v2;
                    break;
                case "/":
                    vf = v1 / v2;
                    break;
                case "*":
                case "x":
                    vf = v1 * v2;
                    break;
                case "^":
                    vf = Math.Pow(v1,v2);
                    break;
                case "sin":
                    if (useDegrees)
                    { v2 = Math.PI * v2 / 180.0; }
                    vf = Math.Sin(v2);
                    break;
                case "cos":
                    if (useDegrees)
                    { v2 = Math.PI * v2 / 180.0; }
                    vf = Math.Cos(v2);
                    break;
                case "tan":
                    if (useDegrees)
                    { v2 = Math.PI * v2 / 180.0; }
                    vf = Math.Tan(v2);
                    break;
                case "root":
                case "√":
                    vf = Math.Sqrt(v2);
                    break;
            }

            return new Value(vf);
        }
    }
}

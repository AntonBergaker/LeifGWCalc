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
            //Some ugly cleanup to prevent things that contains eachother in the name to fire twice
            input = input.
                Replace("arcsin", "arcs").
                Replace("arccos", "arcc").
                Replace("arctan", "arct").
                Replace(">=", "gorthan").
                Replace("<=", "lorthan").
                Replace("!=", "notqual").
                Replace("true", "tru").
                Replace("false", "fals");
            //Probably would've been easier to just replace e with euler

            ValueSequence numbers = new ValueSequence(input);

            if (numbers.state == ValueSequenceState.Error)
            { return "Error"; }

            numbers = RunOperators(numbers,degrees);

            if (numbers.values.Count == 1)
            {
                if (numbers.values[0].type == ValueTypes.Value)
                { return numbers.ToString(); }
                else if (numbers.values[0].type == ValueTypes.Bool)
                { return numbers.ToString(); }
            }
            
            return "Error";
            
        }

        private static ValueSequence RunOperators(ValueSequence numbers, bool useDegrees)
        {
            List<Value> values = numbers.values;

            values = ExecuteParenthesis(values, useDegrees);
            values = ReplaceConstants(values);
            values = ExecuteLeftOperators( values, new string[] { "!" });
            values = ExecuteRightOperators(values, new string[] { "sin", "cos", "tan", "arcs", "arcc", "arct", "root", "√", "log", "lg", "ln", "round", "floor", "ceil" }, useDegrees);
            values = ExecuteBothOperators( values, new string[] { "mod" });
            values = ExecuteBothOperators( values, new string[] { "^"});
            values = ExecuteBothOperators( values, new string[] { "*", "x", "/" });
            values = MultiplyAllValues(values);
            values = ExecuteBothOperators( values, new string[] { "+", "-" });
            values = ExecuteBothOperators( values, new string[] { "gorthan", ">", "<", "lorthan", "==", "notqual" });
            values = ExecuteBothOperators( values, new string[] { "==", "notqual"}, true);
            values = ExecuteBothOperators( values, new string[] { "&&" }, true);
            values = ExecuteBothOperators( values, new string[] { "||" }, true);
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

            Dictionary<string,bool> BoolConstants = new Dictionary<string, bool>();
            BoolConstants.Add("tru", true);
            BoolConstants.Add("fals", false);

            foreach (Value v in values)
            {
                if (v.type == ValueTypes.Operator)
                {
                    if (BoolConstants.ContainsKey(v.operation))
                    {
                        v.type = ValueTypes.Bool;
                        v.boolean = BoolConstants[v.operation];
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
            for (int i = 0; i < values.Count - 1; i++)
            {
                if (values[i + 0].type == ValueTypes.Value && values[i + 1].type == ValueTypes.Value)
                {
                    values[i + 1].value *= values[i + 0].value;
                    values.RemoveAt(i);
                    i--;
                }
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
                        if (newVal.type == ValueTypes.Value || newVal.type == ValueTypes.Bool)
                        {
                            values[i] = newVal;
                        }
                    }
                }
            }
            return values;
        }

        private static List<Value> ExecuteBothOperators(List<Value> values, string[] operators, bool boolMath = false)
        {
            for (int i = 1; i < values.Count - 1; i++)
            {
                if (values[i].type == ValueTypes.Operator)
                {
                    if (operators.Contains(values[i].operation))
                    {
                        Value val1 = values[i - 1];
                        Value val2 = values[i + 1];
                        ValueTypes checkType = boolMath ? ValueTypes.Bool : ValueTypes.Value;

                        if (val1.type == checkType && val2.type == checkType)
                        {
                            Value newVal;
                            if (boolMath)
                            { newVal = ExecuteBoolMath(val1, values[i], val2); }
                            else
                            { newVal = ExecuteMath(val1, values[i], val2); }
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

        private static List<Value> ExecuteLeftOperators(List<Value> values, string[] operators)
        {
            for (int i = 1; i < values.Count; i++)
            {
                if (values[i].type == ValueTypes.Operator)
                {
                    if (operators.Contains(values[i].operation))
                    {
                        Value val1 = values[i - 1];
                        if (val1.type == ValueTypes.Value)
                        {
                            Value newVal = ExecuteMath(val1, values[i], null);
                            values.RemoveRange(i-1, 2);
                            values.Insert(i-1, newVal);
                            i -= 1;
                        }
                    }
                }
            }
            return values;
        }

        private static Value ExecuteBoolMath(Value number1, Value operation, Value number2, bool useDegrees = true)
        {
            bool bf = false;
            bool b1 = false;
            bool b2 = false;
            if (number1 != null)
            { b1 = number1.boolean; }
            if (number2 != null)
            { b2 = number2.boolean; }

            switch (operation.operation)
            {
                case "&&":
                    bf = b1 && b2;
                    break;
                case "||":
                    bf = b1 || b2;
                    break;
                case "==":
                    bf = b1 == b2;
                    break;
                case "notqual":
                    bf = b1 != b2;
                    break;
            }

            return new Value(bf);
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

            bool? returnBool = null;

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
                case "arcs":
                    vf = Math.Asin(v2);
                    if (useDegrees)
                    { vf = vf * 180.0 / Math.PI; }
                    break;
                case "arcc":
                    vf = Math.Acos(v2);
                    if (useDegrees)
                    { vf = vf * 180.0 / Math.PI; }
                    break;
                case "arct":
                    vf = Math.Atan(v2);
                    if (useDegrees)
                    { vf = vf * 180.0 / Math.PI; }
                    break;
                case "root":
                case "√":
                    vf = Math.Sqrt(v2);
                    break;
                case "log":
                case "lg":
                    vf = Math.Log10(v2);
                    break;
                case "ln":
                case "logn":
                    vf = Math.Log(v2);
                    break;
                case "!":
                    //TODO decimal factorial
                    vf = 1;
                    while (v1>1)
                    { vf *= v1--; }
                    break;
                case "round":
                    vf = Math.Round(v2);
                    break;
                case "ceil":
                    vf = Math.Ceiling(v2);
                    break;
                case "floor":
                    vf = Math.Floor(v2);
                    break;
                case "mod":
                    vf = v1 % v2;
                    break;
                case "gorthan":
                    returnBool = v1 >= v2;
                    break;
                case ">":
                    returnBool = v1 > v2;
                    break;
                case "<":
                    returnBool = v1 < v2;
                    break;
                case "lorthan":
                    returnBool = v1 <= v2;
                    break;
                case "==":
                    returnBool = v1 == v2;
                    break;
                case "notqual":
                    returnBool = v1 != v2;
                    break;
            }

            if (returnBool == null)
            { return new Value(vf); }
            else
            { return new Value((bool)returnBool); }
        }
    }
}

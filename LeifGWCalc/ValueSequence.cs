using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LeifGWCalc
{
    enum ValueSequenceState { Ok, Error }
    class ValueSequence
    {
        public List<Value> values;
        public readonly ValueSequenceState state;

        public ValueSequence(List<Value> values)
        {
            bool error = false;
            this.values = ValueSequenceRecursiveFromValueSequenceSimple(values, out error);

            if (error)
            { state = ValueSequenceState.Error; }
            else
            { state = ValueSequenceState.Ok;}
        }

        public ValueSequence(string input)
        {
            state = ValueSequenceState.Ok;
            string[] operators = new string[] {"arcs", "arcc", "arct", "sin", "cos", "tan",  "root", "+", "-", "^", "/", "*", "x", "√", "(", ")", "pi", "e", "π", "ln", "logn", "log", "lg", "!" , "round", "floor", "ceil", "mod", "gorthan", ">", "<", "lorthan", "==", "notqual", "||", "&&", "tru", "fals"};

            values = new List<Value>();
            string[] seperated = SplitKeepDelimiters(input, operators);

            bool error;
            values = ValueSequenceFromStringArray(seperated, operators, out error);

            if (error)
            { state = ValueSequenceState.Error; }

        }

        /// <summary>
        /// Returns a recursive ValueSequence, where parenthesis are stored as values.
        /// </summary>
        private List<Value> ValueSequenceRecursiveFromValueSequenceSimple(List<Value> values, out bool error)
        {
            error = false;
            int paraDeep = 0;
            int startP = 0;
            int endP;
            for (int i = 0; i <values.Count; i++)
            {
                Value v = values[i];
                if (v.type == ValueTypes.Operator)
                {
                    if (v.operation == "(")
                    {
                        if (paraDeep == 0)
                        { startP = i; }
                        paraDeep++;
                    }
                    if (v.operation == ")")
                    {
                        if (paraDeep == 1)
                        {
                            endP = i;
                            int length = endP-startP;

                            Value newVal = ValueFromValueSequence(values,startP+1, endP);
                            values.RemoveRange(startP, length+1);
                            values.Insert(startP,newVal);
                            i -= length;
                            
                        }
                        paraDeep--;
                    }
                }
            }

            if (error)
            { return values; }

            values = ValueSequenceFixNegatives(values);

            return values;
        }

       /// <summary>
       /// Turns "-" operators into negative numbers where applicable
       /// </summary>
       private List<Value> ValueSequenceFixNegatives(List<Value> values)
        {
            //1 less than count because there needs to be a value after the "-" if it's applicable.
            for (int i=0;i<values.Count-1;i++)
            {
                Value v = values[i];
                if (v.type == ValueTypes.Operator && v.operation == "-")
                {
                    if (i == 0 || values[i-1].type == ValueTypes.Operator)
                    {
                        if (values[i+1].type == ValueTypes.Value)
                        {
                            values[i + 1].value *= -1;
                            values.RemoveAt(i);
                            i--;
                        }
                    }
                }
            }

            return values;
        }

        /// <summary>
        /// Returns a Value from positions inside a list of values
        /// </summary>
        private Value ValueFromValueSequence(List<Value> values, int start, int end)
        {
            List<Value> newValues = new List<Value>();
            for (int i=start;i<end;i++)
            { newValues.Add(values[i]); }

            ValueSequence newValueSequence = new ValueSequence(newValues);

            return new Value(newValueSequence);
        } 

        /// <summary>
        /// Returns a ValueSequence from a string array where the values are seperated.
        /// </summary>
        private List<Value> ValueSequenceFromStringArray(string[] array, string[] operators, out bool error)
        {
            error = false;
            List<Value> values = new List<Value>();
            foreach (string s in array)
            {
                //if the value is an operator
                if (operators.Contains(s))
                {
                    values.Add(new Value(s));
                }
                else //else try to parse it as a number
                {
                    double d;
                    bool success = Double.TryParse(s, out d);
                    if (success)
                    { values.Add(new Value(d)); }
                    else
                    {
                        error = true;
                        break;
                    }
                }
            }

            if (error)
                { return values; }

            values = ValueSequenceRecursiveFromValueSequenceSimple(values, out error);

            return values;
        }

        private string[] SplitKeepDelimiters(string input, string[] delimiters)
        {
            foreach (string s in delimiters)
            { input = input.Replace(s, "b|vs|d" + s + "b|vs|d"); }
            return input.Split(new string[] { "b|vs|d" },StringSplitOptions.RemoveEmptyEntries);
        }

        public override string ToString()
        {
            StringBuilder s = new StringBuilder();
            foreach (Value v in values)
            {
                s.Append( v.ToString());
            }
            return s.ToString();
        }
    }
}

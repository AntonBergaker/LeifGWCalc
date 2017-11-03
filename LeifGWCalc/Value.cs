using System;

namespace LeifGWCalc
{
    enum ValueTypes { Operator, Value, ValueSequence, Bool}
    class Value
    {
        public ValueTypes type;
        public double value;
        public bool boolean;
        public string operation;
        public bool invert = false;
        public ValueSequence sequence;

        public Value(double value)
        {
            this.value = value;
            type = ValueTypes.Value;
        }
        public Value(string operation)
        {
            this.operation = operation;
            type = ValueTypes.Operator;
        }
        public Value(ValueSequence sequence)
        {
            this.sequence = sequence;
            type = ValueTypes.ValueSequence;
        }
        public Value(bool boolean)
        {
            this.boolean = boolean;
            type = ValueTypes.Bool;
        }

        public override string ToString()
        {
            switch (type)
            {
                case ValueTypes.Operator:
                    return operation;
                case ValueTypes.ValueSequence:
                    return "("+sequence.ToString()+")";
                case ValueTypes.Bool:
                    return boolean ? "True" : "False";
                default:
                    return Convert.ToString(value);
            }
        }
    }
}

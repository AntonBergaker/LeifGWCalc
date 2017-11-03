using System;

namespace LeifGWCalc
{
    enum ValueTypes { Operator, Value, ValueSequence}
    class Value
    {
        public ValueTypes type;
        public double value;
        public string operation;
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

        public override string ToString()
        {
            switch (type)
            {
                case ValueTypes.Operator:
                    return operation;
                case ValueTypes.ValueSequence:
                    return "("+sequence.ToString()+")";
                default:
                    return Convert.ToString(value);
            }
        }
    }
}

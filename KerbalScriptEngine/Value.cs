﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using KerboScriptEngine.InternalTypes;

namespace KerboScriptEngine
{
    public struct Value
    {
        public bool IsNull { get; private set; }

        string s_value;
        int i_value;
        double f_value;
        bool b_value;
        OrderedPair op_value;

        public enum ValueTypes : int
        {
            String,
            Integer,
            Float,
            Boolean,
            OrderedPair,
            Pointer
        }

        public static Value NullValue
        {
            get
            {
                Value v = new Value();
                v.IsNull = true;
                return v;
            }
        }

        public ValueTypes Type { get; private set; }

        public string PointerValue
        {
            get
            {
                if (!IsNull)
                    return s_value;
                else
                    return null;
            }
            set
            {
                SetValue(value);
            }
        }

        public string StringValue
        {
            get 
            {
                if (!IsNull)
                    return s_value;
                else
                    return null;
            }
            set 
            { 
                SetValue(value);
            }
        }

        public int IntegerValue
        {
            get 
            {
                if (!IsNull)
                    return i_value;
                else
                    return 0;
            }
            set 
            { 
                SetValue(value);
            }
        }

        public double FloatValue
        {
            get 
            {
                if (!IsNull)
                    return f_value;
                else
                    return double.NaN;
            }
            set 
            { 
                SetValue(value);
            }
        }

        public bool BooleanValue
        {
            get 
            {
                if (!IsNull)
                    return b_value;
                else
                    return false;
            }
            set
            {
                SetValue(value);
            }
        }

        public OrderedPair OrderedPairValue
        {
            get
            {
                if (!IsNull)
                    return op_value;
                else
                    return new OrderedPair();
            }
        }

        public Value(string value)
            : this()
        {
            SetValue(value);
        }

        public Value(int value)
            : this()
        {
            SetValue(value);
        }

        public Value(double value)
            : this()
        {
            SetValue(value);
        }

        public Value(bool value)
            : this()
        {
            SetValue(value);
        }

        public Value(OrderedPair value)
            : this()
        {
            SetValue(value);
        }

        public void SetValue(string value, bool isPointer = false)
        {
            IsNull = false;
            Type = isPointer ? ValueTypes.Pointer : ValueTypes.String;
            s_value = value;
            if (!int.TryParse(value, out i_value)) i_value = 0;
            if (!double.TryParse(value, out f_value)) f_value = double.NaN;
            if (!bool.TryParse(value, out b_value)) b_value = false;
            if (!OrderedPair.TryParse(value, out op_value)) op_value = new OrderedPair();
        }

        public void SetValue(int value)
        {
            IsNull = false;
            Type = ValueTypes.Integer;
            i_value = value;
            s_value = value.ToString();
            f_value = (double)i_value;
            b_value = i_value != 0;
            op_value = new OrderedPair(value, 0);
        }

        public void SetValue(double value)
        {
            IsNull = false;
            Type = ValueTypes.Float;
            f_value = value;
            i_value = (int)value;
            b_value = i_value != 0;
            s_value = value.ToString();
            op_value = new OrderedPair(i_value, 0);
        }

        public void SetValue(bool value)
        {
            IsNull = false;
            Type = ValueTypes.Boolean;
            b_value = value;
            s_value = value.ToString();
            i_value = value ? 1 : 0;
            f_value = (double)i_value;
            op_value = new OrderedPair(i_value, 0);
        }

        public void SetValue(OrderedPair value)
        {
            IsNull = false;
            Type = ValueTypes.OrderedPair;
            op_value = value;
            i_value = value.X;
            f_value = (double)value.X;
            b_value = value.X == 0 ? false : true;
            s_value = value.ToString();
        }

        public void CastToType(ValueTypes t)
        {
            switch (Type)
            {
                case ValueTypes.Boolean:
                    SetValue(BooleanValue);
                    break;

                case ValueTypes.Float:
                    SetValue(FloatValue);
                    break;

                case ValueTypes.Integer:
                    SetValue(IntegerValue);
                    break;
                    
                case ValueTypes.String:
                    SetValue(StringValue);
                    break;

                case ValueTypes.OrderedPair:
                    SetValue(OrderedPairValue);
                    break;
            }
        }

        #region Binary Operations
        public static Value operator ==(Value a, Value b)
        {
            if (a.IsNull | b.IsNull) throw new NullReferenceException();

            if ((a.Type == ValueTypes.String) | (b.Type == ValueTypes.String))
                return new Value(a.StringValue == b.StringValue);
            else if ((a.Type == ValueTypes.Float) | (b.Type == ValueTypes.Float))
                return new Value(Math.Abs(a.FloatValue - b.FloatValue) < double.Epsilon * 10);
            else if ((a.Type == ValueTypes.Integer) | (b.Type == ValueTypes.Integer))
                return new Value(a.IntegerValue == b.IntegerValue);
            else if ((a.Type == ValueTypes.Boolean) | (b.Type == ValueTypes.Boolean))
                return new Value(a.BooleanValue == b.BooleanValue);
            else if ((a.Type == ValueTypes.OrderedPair) | (b.Type == ValueTypes.OrderedPair))
                return new Value((a.OrderedPairValue.X == b.OrderedPairValue.X) & (a.OrderedPairValue.Y == b.OrderedPairValue.Y));  
            else
                return new Value();
        }

        public static Value operator !=(Value a, Value b)
        {
            if (a.IsNull | b.IsNull) throw new NullReferenceException();

            return !(a == b);
        }

        public static Value operator +(Value a, Value b)
        {
            if (a.IsNull | b.IsNull) throw new NullReferenceException();

            if ((a.Type == ValueTypes.String) | (b.Type == ValueTypes.String))
                return new Value(a.StringValue + b.StringValue);
            else if ((a.Type == ValueTypes.Float) | (b.Type == ValueTypes.Float))
                return new Value(a.FloatValue + b.FloatValue);
            else if ((a.Type == ValueTypes.Integer) | (b.Type == ValueTypes.Integer))
                return new Value(a.IntegerValue + b.IntegerValue);
            else if ((a.Type == ValueTypes.Boolean) | (b.Type == ValueTypes.Boolean))
                return new Value((a.IntegerValue + b.IntegerValue) != 0 ? true : false);
            else if ((a.Type == ValueTypes.OrderedPair) | (b.Type == ValueTypes.OrderedPair))
                return new Value(new OrderedPair(a.OrderedPairValue.X + b.OrderedPairValue.X, a.OrderedPairValue.Y + b.OrderedPairValue.Y));
            else
                return new Value();
        }

        public static Value operator -(Value a, Value b)
        {
            if (a.IsNull | b.IsNull) throw new NullReferenceException();

            if ((a.Type == ValueTypes.String) | (b.Type == ValueTypes.String))
                throw new InvalidOperationException("Cannot perform subtraction with a string value.");
            else if ((a.Type == ValueTypes.Float) | (b.Type == ValueTypes.Float))
                return new Value(a.FloatValue - b.FloatValue);
            else if ((a.Type == ValueTypes.Integer) | (b.Type == ValueTypes.Integer))
                return new Value(a.IntegerValue - b.IntegerValue);
            else if ((a.Type == ValueTypes.Boolean) | (b.Type == ValueTypes.Boolean))
                return new Value((a.IntegerValue - b.IntegerValue) != 0 ? true : false);
            else if ((a.Type == ValueTypes.OrderedPair) | (b.Type == ValueTypes.OrderedPair))
                return new Value(new OrderedPair(a.OrderedPairValue.X - b.OrderedPairValue.X, a.OrderedPairValue.Y - b.OrderedPairValue.Y));
            else
                return new Value();
        }

        public static Value operator *(Value a, Value b)
        {
            if (a.IsNull | b.IsNull) throw new NullReferenceException();

            if ((a.Type == ValueTypes.String) | (b.Type == ValueTypes.String))
                throw new InvalidOperationException("Cannot perform multiplication with a string value.");
            else if ((a.Type == ValueTypes.Float) | (b.Type == ValueTypes.Float))
                return new Value(a.FloatValue * b.FloatValue);
            else if ((a.Type == ValueTypes.Integer) | (b.Type == ValueTypes.Integer))
                return new Value(a.IntegerValue * b.IntegerValue);
            else if ((a.Type == ValueTypes.Boolean) | (b.Type == ValueTypes.Boolean))
                return new Value((a.IntegerValue * b.IntegerValue) != 0 ? true : false);
            else if ((a.Type == ValueTypes.OrderedPair) | (b.Type == ValueTypes.OrderedPair))
                return new Value(new OrderedPair(a.OrderedPairValue.X * b.OrderedPairValue.X, a.OrderedPairValue.Y * b.OrderedPairValue.Y));
            else
                return new Value();
        }

        public static Value operator /(Value a, Value b)
        {
            if (a.IsNull | b.IsNull) throw new NullReferenceException();

            if ((a.Type == ValueTypes.String) | (b.Type == ValueTypes.String))
                throw new InvalidOperationException("Cannot perform division with a string value.");
            else if ((a.Type == ValueTypes.Float) | (b.Type == ValueTypes.Float))
            {
                if (b.FloatValue.CompareTo(0f) != 0)
                    throw new DivideByZeroException();
                return new Value(a.FloatValue / b.FloatValue);
            }
            else if ((a.Type == ValueTypes.Integer) | (b.Type == ValueTypes.Integer))
            {
                if (b.IntegerValue == 0)
                    throw new DivideByZeroException();
                return new Value(a.IntegerValue / b.IntegerValue);
            }
            else if ((a.Type == ValueTypes.Boolean) | (b.Type == ValueTypes.Boolean))
            {
                if (b.IntegerValue == 0)
                    throw new DivideByZeroException();
                return new Value((a.IntegerValue / b.IntegerValue) != 0 ? true : false);
            }
            else if ((a.Type == ValueTypes.OrderedPair) | (b.Type == ValueTypes.OrderedPair))
            {
                if ((b.OrderedPairValue.Y == 0) | (b.OrderedPairValue.X == 0))
                    throw new DivideByZeroException();
                return new Value(new OrderedPair(a.OrderedPairValue.X / b.OrderedPairValue.X, a.OrderedPairValue.Y / b.OrderedPairValue.Y));
            }
            else
                return new Value();
        }

        public static Value operator %(Value a, Value b)
        {
            if (a.IsNull | b.IsNull) throw new NullReferenceException();

            if ((a.Type == ValueTypes.String) | (b.Type == ValueTypes.String))
                throw new InvalidOperationException("Cannot perform remainder division with a string value.");
            else if ((a.Type == ValueTypes.Float) | (b.Type == ValueTypes.Float))
            {
                if (b.FloatValue.CompareTo(0f) != 0)
                    throw new DivideByZeroException();
                return new Value(a.FloatValue % b.FloatValue);
            }
            else if ((a.Type == ValueTypes.Integer) | (b.Type == ValueTypes.Integer))
            {
                if (b.IntegerValue == 0)
                    throw new DivideByZeroException();
                return new Value(a.IntegerValue % b.IntegerValue);
            }
            else if ((a.Type == ValueTypes.Boolean) | (b.Type == ValueTypes.Boolean))
            {
                if (b.IntegerValue == 0)
                    throw new DivideByZeroException();
                return new Value((a.IntegerValue % b.IntegerValue) != 0 ? true : false);
            }
            else if ((a.Type == ValueTypes.OrderedPair) | (b.Type == ValueTypes.OrderedPair))
            {
                if ((b.OrderedPairValue.X == 0) | (b.OrderedPairValue.Y == 0))
                    throw new DivideByZeroException();
                return new Value(new OrderedPair(a.OrderedPairValue.X % b.OrderedPairValue.X, a.OrderedPairValue.Y % b.OrderedPairValue.Y));
            }
            else
                return new Value();
        }

        public static Value RaiseToPower(Value x, Value power)
        {
            if (x.IsNull | power.IsNull) throw new NullReferenceException();

            if ((x.Type == ValueTypes.String) | (power.Type == ValueTypes.String))
                throw new InvalidOperationException("Cannot perform multiplication with a string value.");
            else if ((x.Type == ValueTypes.Float) | (power.Type == ValueTypes.Float))
                return new Value(Math.Pow(x.FloatValue, power.FloatValue));
            else if ((x.Type == ValueTypes.Integer) | (power.Type == ValueTypes.Integer))
                return new Value((int)Math.Pow(x.FloatValue, power.FloatValue));
            else if ((x.Type == ValueTypes.Boolean) | (power.Type == ValueTypes.Boolean))
                return new Value(((int)Math.Pow(x.FloatValue, power.FloatValue)) != 0 ? true : false);
            else if ((x.Type == ValueTypes.OrderedPair) | (power.Type == ValueTypes.OrderedPair))
                return new Value(new OrderedPair((int)Math.Pow(x.OrderedPairValue.X, power.OrderedPairValue.X),
                    (int)Math.Pow(x.OrderedPairValue.Y, power.OrderedPairValue.Y)));
            else
                return new Value();
        }

        public static Value operator &(Value a, Value b)
        {
            if (a.IsNull | b.IsNull) throw new NullReferenceException();

            if ((a.Type == ValueTypes.String) | (b.Type == ValueTypes.String))
                throw new InvalidOperationException("Cannot perform bitwise or logical AND with a string value.");
            else if ((a.Type == ValueTypes.Integer) | (b.Type == ValueTypes.Integer))
                return new Value(a.IntegerValue & b.IntegerValue);
            else if ((a.Type == ValueTypes.Boolean) | (b.Type == ValueTypes.Boolean))
                return new Value(a.BooleanValue & b.BooleanValue);
            else if ((a.Type == ValueTypes.Float) | (b.Type == ValueTypes.Float))
                throw new InvalidOperationException("Cannot perform bitwise or logical AND with a floating-point value.");
            else if ((a.Type == ValueTypes.OrderedPair) | (b.Type == ValueTypes.OrderedPair))
                return new Value(new OrderedPair(a.OrderedPairValue.X & b.OrderedPairValue.X, a.OrderedPairValue.Y & b.OrderedPairValue.Y));
            else
                return new Value();
        }

        public static Value operator |(Value a, Value b)
        {
            if (a.IsNull | b.IsNull) throw new NullReferenceException();

            if ((a.Type == ValueTypes.String) | (b.Type == ValueTypes.String))
                throw new InvalidOperationException("Cannot perform bitwise or logical AND with a string value.");
            else if ((a.Type == ValueTypes.Integer) | (b.Type == ValueTypes.Integer))
                return new Value(a.IntegerValue | b.IntegerValue);
            else if ((a.Type == ValueTypes.Boolean) | (b.Type == ValueTypes.Boolean))
                return new Value(a.BooleanValue | b.BooleanValue);
            else if ((a.Type == ValueTypes.Float) | (b.Type == ValueTypes.Float))
                throw new InvalidOperationException("Cannot perform bitwise or logical AND with a floating-point value.");
            else if ((a.Type == ValueTypes.OrderedPair) | (b.Type == ValueTypes.OrderedPair))
                return new Value(new OrderedPair(a.OrderedPairValue.X | b.OrderedPairValue.X, a.OrderedPairValue.Y | b.OrderedPairValue.Y));
            else
                return new Value();
        }

        #endregion

        #region Unary Operations

        public static Value operator +(Value x)
        {
            if (x.IsNull) throw new NullReferenceException();
            return x;
        }

        public static Value operator -(Value x)
        {
            if (x.IsNull) throw new NullReferenceException();

            if (x.Type == ValueTypes.String)
                throw new InvalidOperationException("Cannot perform mathematical negation on a string value.");
            else if (x.Type == ValueTypes.Float)
                return new Value(x.FloatValue * -1);
            else if (x.Type == ValueTypes.Integer)
                return new Value(x.IntegerValue * -1);
            else if (x.Type == ValueTypes.Boolean)
                return new Value((x.IntegerValue * -1) != 0 ? true : false);
            else if (x.Type == ValueTypes.OrderedPair)
                return new Value(new OrderedPair(x.OrderedPairValue.X * -1, x.OrderedPairValue.Y * -1));
            else
                return new Value();
        }

        public static Value operator ~(Value x)
        {
            if (x.IsNull) throw new NullReferenceException();

            if (x.Type == ValueTypes.String)
                throw new InvalidOperationException("Cannot perform binary negation on a string value.");
            else if (x.Type == ValueTypes.Float)
                throw new InvalidOperationException("Cannot perform binary negation on a double-precision floating point value.");
            else if (x.Type == ValueTypes.Integer)
                return new Value(~x.IntegerValue);
            else if (x.Type == ValueTypes.Boolean)
                return new Value((~x.IntegerValue) != 0 ? true : false);
            else if (x.Type == ValueTypes.OrderedPair)
                return new Value(new OrderedPair(~x.OrderedPairValue.X, ~x.OrderedPairValue.Y));
            else
                return new Value();
        }

        public static Value operator !(Value x)
        {
            if (x.IsNull) throw new NullReferenceException();

            if (x.Type == ValueTypes.Boolean)
            {
                return new Value(!x.BooleanValue);
            }
            else
            {
                throw new InvalidOperationException("Cannot perform logical-NOT on non-boolean values.");
            }
        }

        public static Value operator ++(Value x)
        {
            if (x.IsNull) throw new NullReferenceException();

            if (x.Type == ValueTypes.String)
                throw new InvalidOperationException("Cannot perform incrementation with a string value.");
            else if (x.Type == ValueTypes.Float)
                return new Value(x.FloatValue + 1);
            else if (x.Type == ValueTypes.Integer)
                return new Value(x.IntegerValue + 1);
            else if (x.Type == ValueTypes.Boolean)
                return new Value((x.IntegerValue + 1) != 0 ? true : false);
            else if (x.Type == ValueTypes.OrderedPair)
                return new Value(new OrderedPair(x.OrderedPairValue.X + 1, x.OrderedPairValue.Y + 1));
            else
                return new Value();
        }

        public static Value operator --(Value x)
        {
            if (x.IsNull) throw new NullReferenceException();

            if (x.Type == ValueTypes.String)
                throw new InvalidOperationException("Cannot perform decrementation with a string value.");
            else if (x.Type == ValueTypes.Float)
                return new Value(x.FloatValue - 1);
            else if (x.Type == ValueTypes.Integer)
                return new Value(x.IntegerValue - 1);
            else if (x.Type == ValueTypes.Boolean)
                return new Value((x.IntegerValue - 1) != 0 ? true : false);
            else if (x.Type == ValueTypes.OrderedPair)
                return new Value(new OrderedPair(x.OrderedPairValue.X - 1, x.OrderedPairValue.Y - 1));
            else
                return new Value();
        }

        #endregion

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }
    }
}

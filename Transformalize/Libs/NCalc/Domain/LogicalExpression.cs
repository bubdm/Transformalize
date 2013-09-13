#region License

// /*
// Transformalize - Replicate, Transform, and Denormalize Your Data...
// Copyright (C) 2013 Dale Newman
// 
// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.
// 
// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.
// 
// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <http://www.gnu.org/licenses/>.
// */

#endregion

using System;
using System.Text;

namespace Transformalize.Libs.NCalc.Domain
{
    public abstract class LogicalExpression
    {
        private const char BS = '\\';

        private static string extractString(string text)
        {
            var sb = new StringBuilder(text);
            var startIndex = 1; // Skip initial quote
            var slashIndex = -1;

            while ((slashIndex = sb.ToString().IndexOf(BS, startIndex)) != -1)
            {
                var escapeType = sb[slashIndex + 1];
                switch (escapeType)
                {
                    case 'u':
                        var hcode = String.Concat(sb[slashIndex + 4], sb[slashIndex + 5]);
                        var lcode = String.Concat(sb[slashIndex + 2], sb[slashIndex + 3]);
                        var unicodeChar = Encoding.Unicode.GetChars(new[] {Convert.ToByte(hcode, 16), Convert.ToByte(lcode, 16)})[0];
                        sb.Remove(slashIndex, 6).Insert(slashIndex, unicodeChar);
                        break;
                    case 'n':
                        sb.Remove(slashIndex, 2).Insert(slashIndex, '\n');
                        break;
                    case 'r':
                        sb.Remove(slashIndex, 2).Insert(slashIndex, '\r');
                        break;
                    case 't':
                        sb.Remove(slashIndex, 2).Insert(slashIndex, '\t');
                        break;
                    case '\'':
                        sb.Remove(slashIndex, 2).Insert(slashIndex, '\'');
                        break;
                    case '\\':
                        sb.Remove(slashIndex, 2).Insert(slashIndex, '\\');
                        break;
                    default:
                        throw new ApplicationException("Unvalid escape sequence: \\" + escapeType);
                }

                startIndex = slashIndex + 1;
            }

            sb.Remove(0, 1);
            sb.Remove(sb.Length - 1, 1);

            return sb.ToString();
        }

        public BinaryExpression And(LogicalExpression operand)
        {
            return new BinaryExpression(BinaryExpressionType.And, this, operand);
        }

        public BinaryExpression And(object operand)
        {
            return new BinaryExpression(BinaryExpressionType.And, this, new ValueExpression(operand));
        }

        public BinaryExpression DividedBy(LogicalExpression operand)
        {
            return new BinaryExpression(BinaryExpressionType.Div, this, operand);
        }

        public BinaryExpression DividedBy(object operand)
        {
            return new BinaryExpression(BinaryExpressionType.Div, this, new ValueExpression(operand));
        }

        public BinaryExpression EqualsTo(LogicalExpression operand)
        {
            return new BinaryExpression(BinaryExpressionType.Equal, this, operand);
        }

        public BinaryExpression EqualsTo(object operand)
        {
            return new BinaryExpression(BinaryExpressionType.Equal, this, new ValueExpression(operand));
        }

        public BinaryExpression GreaterThan(LogicalExpression operand)
        {
            return new BinaryExpression(BinaryExpressionType.Greater, this, operand);
        }

        public BinaryExpression GreaterThan(object operand)
        {
            return new BinaryExpression(BinaryExpressionType.Greater, this, new ValueExpression(operand));
        }

        public BinaryExpression GreaterOrEqualThan(LogicalExpression operand)
        {
            return new BinaryExpression(BinaryExpressionType.GreaterOrEqual, this, operand);
        }

        public BinaryExpression GreaterOrEqualThan(object operand)
        {
            return new BinaryExpression(BinaryExpressionType.GreaterOrEqual, this, new ValueExpression(operand));
        }

        public BinaryExpression LesserThan(LogicalExpression operand)
        {
            return new BinaryExpression(BinaryExpressionType.Lesser, this, operand);
        }

        public BinaryExpression LesserThan(object operand)
        {
            return new BinaryExpression(BinaryExpressionType.Lesser, this, new ValueExpression(operand));
        }

        public BinaryExpression LesserOrEqualThan(LogicalExpression operand)
        {
            return new BinaryExpression(BinaryExpressionType.LesserOrEqual, this, operand);
        }

        public BinaryExpression LesserOrEqualThan(object operand)
        {
            return new BinaryExpression(BinaryExpressionType.LesserOrEqual, this, new ValueExpression(operand));
        }

        public BinaryExpression Minus(LogicalExpression operand)
        {
            return new BinaryExpression(BinaryExpressionType.Minus, this, operand);
        }

        public BinaryExpression Minus(object operand)
        {
            return new BinaryExpression(BinaryExpressionType.Minus, this, new ValueExpression(operand));
        }

        public BinaryExpression Modulo(LogicalExpression operand)
        {
            return new BinaryExpression(BinaryExpressionType.Modulo, this, operand);
        }

        public BinaryExpression Modulo(object operand)
        {
            return new BinaryExpression(BinaryExpressionType.Modulo, this, new ValueExpression(operand));
        }

        public BinaryExpression NotEqual(LogicalExpression operand)
        {
            return new BinaryExpression(BinaryExpressionType.NotEqual, this, operand);
        }

        public BinaryExpression NotEqual(object operand)
        {
            return new BinaryExpression(BinaryExpressionType.NotEqual, this, new ValueExpression(operand));
        }

        public BinaryExpression Or(LogicalExpression operand)
        {
            return new BinaryExpression(BinaryExpressionType.Or, this, operand);
        }

        public BinaryExpression Or(object operand)
        {
            return new BinaryExpression(BinaryExpressionType.Or, this, new ValueExpression(operand));
        }

        public BinaryExpression Plus(LogicalExpression operand)
        {
            return new BinaryExpression(BinaryExpressionType.Plus, this, operand);
        }

        public BinaryExpression Plus(object operand)
        {
            return new BinaryExpression(BinaryExpressionType.Plus, this, new ValueExpression(operand));
        }

        public BinaryExpression Mult(LogicalExpression operand)
        {
            return new BinaryExpression(BinaryExpressionType.Times, this, operand);
        }

        public BinaryExpression Mult(object operand)
        {
            return new BinaryExpression(BinaryExpressionType.Times, this, new ValueExpression(operand));
        }

        public BinaryExpression BitwiseOr(LogicalExpression operand)
        {
            return new BinaryExpression(BinaryExpressionType.BitwiseOr, this, operand);
        }

        public BinaryExpression BitwiseOr(object operand)
        {
            return new BinaryExpression(BinaryExpressionType.BitwiseOr, this, new ValueExpression(operand));
        }

        public BinaryExpression BitwiseAnd(LogicalExpression operand)
        {
            return new BinaryExpression(BinaryExpressionType.BitwiseAnd, this, operand);
        }

        public BinaryExpression BitwiseAnd(object operand)
        {
            return new BinaryExpression(BinaryExpressionType.BitwiseAnd, this, new ValueExpression(operand));
        }

        public BinaryExpression BitwiseXOr(LogicalExpression operand)
        {
            return new BinaryExpression(BinaryExpressionType.BitwiseXOr, this, operand);
        }

        public BinaryExpression BitwiseXOr(object operand)
        {
            return new BinaryExpression(BinaryExpressionType.BitwiseXOr, this, new ValueExpression(operand));
        }

        public BinaryExpression LeftShift(LogicalExpression operand)
        {
            return new BinaryExpression(BinaryExpressionType.LeftShift, this, operand);
        }

        public BinaryExpression LeftShift(object operand)
        {
            return new BinaryExpression(BinaryExpressionType.LeftShift, this, new ValueExpression(operand));
        }

        public BinaryExpression RightShift(LogicalExpression operand)
        {
            return new BinaryExpression(BinaryExpressionType.RightShift, this, operand);
        }

        public BinaryExpression RightShift(object operand)
        {
            return new BinaryExpression(BinaryExpressionType.RightShift, this, new ValueExpression(operand));
        }

        public override string ToString()
        {
            var serializer = new SerializationVisitor();
            Accept(serializer);

            return serializer.Result.ToString().TrimEnd(' ');
        }

        public virtual void Accept(LogicalExpressionVisitor visitor)
        {
            visitor.Visit(this);
        }
    }
}
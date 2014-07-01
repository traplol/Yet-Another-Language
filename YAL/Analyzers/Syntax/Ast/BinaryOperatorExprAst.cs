using System;

namespace YAL.Analyzers.Syntax.Ast
{
    class BinaryOperatorExprAst : ExprAstBase
    {
        public string Operator { get; set; }
        public IExprAst LeftSide { get; set; }
        public IExprAst RightSide { get; set; }

        public BinaryOperatorExprAst(string @operator, IExprAst leftSide, IExprAst rightSide)
        {
            Type = ExprValueType.BinOp;
            Operator = @operator;
            LeftSide = leftSide;
            LeftSide.Parent = this;
            RightSide = rightSide;
            RightSide.Parent = this;
        }

        public override object Execute(Context<string, object> context)
        {
            if (Operator == "=" && LeftSide.Type == ExprValueType.Identifier)
            {
                return ((IdentifierExprAst) LeftSide).Assign(context, RightSide.Execute(context));
            }
            var left = LeftSide.Execute(context);
            var right = RightSide.Execute(context);

            if (left is bool && right is bool)
            {
                return Booleans((bool)left, (bool)right);
            }
            if (left is double && right is double)
            {
                return Doubles((double)left, (double)right);
            }
            if (left is int && right is int)
            {
                return Integers((int) left, (int) right);
            }
            if (left == null)
                left = "null";
            if (right == null)
                right = "null";
            return Strings(left.ToString(), right.ToString());
        }

        private object Booleans(bool left, bool right)
        {
            switch (Operator)
            {
                default:
                    return false;
                case "==":
                    return left == right;
                case "!=":
                    return left != right;
                case "&&":
                    return left && right;
                case "||":
                    return left || right;
            }
        }

        private object Integers(int left, int right)
        {
            switch (Operator)
            {
                default:
                    return double.NaN;
                case "*":
                    return left * right;
                case "/":
                    return left / right;
                case "%":
                    return left % right;
                case "+":
                    return left + right;
                case "-":
                    return left - right;
                case "==":
                    return left == right;
                case "<":
                    return left < right;
                case ">":
                    return left > right;
                case "<=":
                    return left <= right;
                case ">=":
                    return left >= right;
                case "!=":
                    return left != right;
            }
        }
        private object Doubles(double left, double right)
        {
            //return left + right;
            switch (Operator)
            {
                default:
                    return double.NaN;
                case "*":
                    return left * right;
                case "/":
                    return left / right;
                case "%":
                    return left % right;
                case "+":
                    return left + right;
                case "-":
                    return left - right;
                case "==":
                    return left == right;
                case "<":
                    return left < right;
                case ">":
                    return left > right;
                case "<=":
                    return left <= right;
                case ">=":
                    return left >= right;
                case "!=":
                    return left != right;
            }
        }

        private object Strings(string left, string right)
        {
            switch (Operator)
            {
                default:
                    return double.NaN;

                case "+":
                    return left + right;
                case "==":
                    return left == right;
                case "!=":
                    return left != right;

                case "<":
                    return left.Length < right.Length;
                case ">":
                    return left.Length > right.Length;
                case "<=":
                    return left.Length <= right.Length;
                case ">=":
                    return left.Length >= right.Length;
            }
        }

     
    }
}

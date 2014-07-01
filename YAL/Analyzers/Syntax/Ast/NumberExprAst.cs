
namespace YAL.Analyzers.Syntax.Ast
{
    class NumberExprAst : ExprAstBase
    {
        public object Value { get; set; }


        public NumberExprAst(int value)
        {
            Type = ExprValueType.Number;
            Value = value;
        }

        public NumberExprAst(double value)
        {
            Type = ExprValueType.Number;
            Value = value;
        }

        public override object Execute(Context<string, object> context)
        {
            return Value;
        }
    }
}

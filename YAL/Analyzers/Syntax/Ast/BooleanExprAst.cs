
namespace YAL.Analyzers.Syntax.Ast
{
    class BooleanExprAst : ExprAstBase
    {
        public bool Value { get; set; }

        public BooleanExprAst(bool value)
        {
            Type = ExprValueType.Boolean;
            Value = value;
        }

        public override object Execute(Context<string, object> context)
        {
            return Value;
        }
    }
}

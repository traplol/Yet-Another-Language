
namespace YAL.Analyzers.Syntax.Ast
{
    class StringExprAst : ExprAstBase
    {
        public string Value { get; set; }

        public StringExprAst(string value)
        {
            Type = ExprValueType.String;
            Value = value;
        }

        public override object Execute(Context<string, object> context)
        {
            return Value;
        }
    }
}

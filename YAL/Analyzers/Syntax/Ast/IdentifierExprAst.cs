using System;
using System.Collections.Generic;

namespace YAL.Analyzers.Syntax.Ast
{
    class IdentifierExprAst : ExprAstBase
    {
        public string Name { get; set; }

        public IdentifierExprAst(string name)
        {
            Type = ExprValueType.Identifier;
            Name = name;
        }

        public override object Execute(Context<string, object> context)
        {
            return context[Name].Value;
        }

        public object Assign(Context<string, object> context, object value )
        {
            return context[Name] = new KeyValuePair<string, object>(Name, value);
        }
    }
}

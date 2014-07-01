using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAL.Analyzers.Syntax.Ast
{
    class ArrayExprAst : ExprAstBase
    {
        public string Name { get; set; }
        public object[] Array { get; set; }
        public int Size { get; set; }

        public ArrayExprAst(string name)
        {
            Name = name;
            Type = ExprValueType.Array;
            Size = 12;
            Array = new object[Size];
        }

        public override object Execute(Context<string, object> context)
        {
            return Array;
        }
    }
}

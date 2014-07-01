using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace YAL.Analyzers.Syntax.Ast
{
    class ArrayAccessExprAst : ExprAstBase
    {

        public IExprAst AccessIndex { get; set; }
        public string Name { get; set; }

        public ArrayAccessExprAst(IExprAst accessIndex, string name)
        {
            AccessIndex = accessIndex;
            Name = name;
            Type = ExprValueType.ArrayAccess;
        }

        public override object Execute(Context<string, object> context)
        {
            return context[Name].Value;
        }

        //public object Access(int index)
        //{
        //    if (index < 0)
        //        index = Array.Length + index; // starts accessing the opposite end of the array
        //    if (index < 0 || index >= Array.Length)
        //        return null;
        //    return Array[index];
        //}

        //public void Assign(int index, object value)
        //{
        //    if (index < 0)
        //        index = Array.Length + index; // starts accessing the opposite end of the array
        //    if (index < 0) // negative number here means ouside of array bounds still
        //        return;
        //    if (index >= Array.Length)
        //        Resize(index - Array.Length + 1);
        //    Array[index] = value;
        //}

        //public void Resize(int num = 12)
        //{
        //    var tmp = Array;
        //    Array = new object[Size += num];
        //    tmp.CopyTo(Array, 0);
        //}
    }
}

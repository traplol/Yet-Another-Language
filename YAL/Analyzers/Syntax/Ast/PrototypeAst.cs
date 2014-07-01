using System.Collections.Generic;

namespace YAL.Analyzers.Syntax.Ast
{
    class PrototypeAst
    {
        public string Name { get; set; }
        public List<string> Args { get; set; }

        public PrototypeAst(string name, List<string> args)
        {
            Name = name;
            Args = args;
        }


    }
}

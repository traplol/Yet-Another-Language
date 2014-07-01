using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using YAL.Analyzers.Lexical;
using YAL.Analyzers.Syntax;

namespace YAL
{
    class Program
    {
        static void Main(string[] args)
        {
            if (!args.Any())
            {
                Console.WriteLine("No input file!");
                Console.WriteLine("Press any key to exit...");
                Console.ReadKey();
            }
            Lexer lexer;
            //while (!args.Any())
            //{
            //    Console.Write("\r\n\r\n>  ");
            //    var inp = Console.ReadLine();
            //    lexer = new Lexer(inp);
            //    new AstBuilder(lexer).Build();
            //}

            lexer = new Lexer(File.ReadAllText(args[0]));
            new AstBuilder(lexer).Build();
            Console.ReadKey();
        }
    }
}

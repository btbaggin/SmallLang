using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using SmallLang;

namespace SmallLangTest
{
    static class Program
    {
        static void Main(string[] args)
        {
            var c = new Compiler();
            var o = new CompilationOptions("stdlib", @"C:\Users\ajensen\source\repos\SmallLang\SmallLangTest\libs", "stdlib.dll", CompilationOutputType.Dll);
            //c.Run(@"C:\Users\ajensen\source\repos\SmallLang\SmallLangTest\libs\stdlib.sml", o);

            //o = new CompilationOptions("graphics", @"C:\Users\ajensen\source\repos\SmallLang\SmallLangTest\libs", "graphics.dll", CompilationOutputType.Dll);
            //c.Run(@"C:\Users\ajensen\source\repos\SmallLang\SmallLangTest\libs\graphics.sml", o);

            //o = new CompilationOptions("input", @"C:\Users\ajensen\source\repos\SmallLang\SmallLangTest\libs", "input.dll", CompilationOutputType.Dll);
            //c.Run(@"C:\Users\ajensen\source\repos\SmallLang\SmallLangTest\libs\input.sml", o);

            //o = new CompilationOptions("math", @"C:\Users\ajensen\source\repos\SmallLang\SmallLangTest\libs", "math.dll", CompilationOutputType.Dll);
            //c.Run(@"C:\Users\ajensen\source\repos\SmallLang\SmallLangTest\libs\math.sml", o);

            //o = new CompilationOptions("Maze", @"C:\Users\ajensen\source\repos\SmallLang\SmallLangTest\Maze", "Maze.exe", CompilationOutputType.Exe);
            //c.Run(@"C:\Users\ajensen\source\repos\SmallLang\SmallLangTest\Maze\simple.sml", o);

            o = new CompilationOptions("EmitTest", @"C:\Users\ajensen\source\repos\SmallLang\SmallLangTest", "emittest.exe", CompilationOutputType.Exe);
            c.Run(@"C:\Users\ajensen\source\repos\SmallLang\SmallLangTest\emittest.sml", o);
            System.Console.WriteLine("Finished");
            System.Console.Read();
        }
    }
}

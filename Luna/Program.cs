using System;
using Luna.DataClasses;

namespace Luna
{
    class Program
    {
        [STAThread]
        static void Main(string[] args)
        {
            using var game = new Luna.Game1();
            game.Run();
        }
    }
}
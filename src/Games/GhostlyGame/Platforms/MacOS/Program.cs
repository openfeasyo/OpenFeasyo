using System;

namespace GhostlyGame
{
    class MainClass
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            using (var game = new GhostlyLib.GhostlyGame())
                game.Run();
        }
    }
}

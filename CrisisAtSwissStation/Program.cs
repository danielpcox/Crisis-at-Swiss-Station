using System;

namespace CrisisAtSwissStation
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main(string[] args)
        {
            GameEngine.Instance.Run();
        }
    }
}


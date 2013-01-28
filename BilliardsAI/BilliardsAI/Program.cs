using System;

namespace BilliardsAI
{
#if WINDOWS || XBOX
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            using (Billiards game = new Billiards())
            {
                game.Run();
            }
        }
    }
#endif
}


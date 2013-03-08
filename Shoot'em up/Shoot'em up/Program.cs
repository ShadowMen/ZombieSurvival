using System;

namespace Shoot_em_up
{
#if WINDOWS || XBOX
    static class Program
    {
        const string MutexID = "Zombie Survive";
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        static void Main(string[] args)
        {
            System.Threading.Mutex mutexInstance;

            try
            {
                //Finde heraus ob eine Instanz schon geöffnet ist
                mutexInstance = System.Threading.Mutex.OpenExisting(MutexID);
                //Existiert schon eine, wird dieses Programm beendet
                Environment.Exit(0);
            }
            catch
            {
                //Wenn nicht erstell eine
                mutexInstance = new System.Threading.Mutex(true, MutexID);
            }

            using (Game1 game = new Game1())
            {
                game.Run();
            }

            mutexInstance.ReleaseMutex();
        }
    }
#endif
}


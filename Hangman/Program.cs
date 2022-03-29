using System;

namespace Hangman
{
    class Program
    {
        static void Main(string[] args)
        {

            Game.Instance.Start();

            /*for (int i = 0; i <= 10; i++)
            {
                Console.WriteLine(10-i);
                AsciiDrawer.DrawHangmanedProgress(10-i);
            }*/

            Console.WriteLine("End of program reached, press any key to close");
            Console.ReadKey();
        }
    }
}

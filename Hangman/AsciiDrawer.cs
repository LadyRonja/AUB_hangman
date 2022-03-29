﻿using System;
using System.Collections.Generic;
using System.Text;

namespace Hangman
{
    class AsciiDrawer
    {
        #region ASCII_Art
        private static string[] artGallery = {
        @"  






            __________",//0
        @"  






            =========",//1
        @"  

                  |
                  |
                  |
                  |
                  |
            =========",//2
        @"     

                 \|
                  |
                  |
                  |
                  |
            =========",//3
        @"  
              +---+
                 \|
                  |
                  |
                  |
                  |
            =========",//4
        @"
              +---+
              |  \|
              O   |
                  |
                  |
                  |
            =========",//5
        @"
              +---+
              |  \|
              O   |
              |   |
                  |
                  |
            =========",//6
        @"
              +---+
              |  \|
              O   |
             /|   |
                  |
                  |
            =========",//7
        @"
              +---+
              |  \|
              O   |
             /|\  |
                  |
                  |
            =========",//8
        @"
              +---+
              |  \|
              O   |
             /|\  |
             /    |
                  |
            =========",//9
        @"
              +---+
              |  \|
              O   |
             /|\  |
             / \  |
                  |
            =========",//10

        };
        #endregion

        public static void DrawHangmanedProgress(int guessesLeft)
        {
            //Lazy protection vs invalid input
            try
            {
                //draw ascii art within the span of 0-10
                Console.WriteLine(artGallery[guessesLeft]);
            }
            catch (Exception)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine("ERROR IN DRAWING ASCII, PLEASE CONTACT DEVELOPER");
                Console.ResetColor();
                throw;
            }
            
        }
    }
}

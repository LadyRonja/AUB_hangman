using System;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Reflection;
using System.Linq;

namespace Hangman
{
    sealed class Game
    {
        #region Singleton
        private static readonly Game instance = new Game();

        // Explicit static constructor to tell C# compiler not to mark type as beforefieldinit
        static Game() { }
        private Game() { }

        public static Game Instance
        {
            get
            {
                return instance;
            }
        }
        #endregion

        private const int MAX_GUESSES = 10;
        private Random rand = new Random();
        private string executionFolder = Directory.GetParent(Assembly.GetExecutingAssembly().Location).FullName;
        private string wordListPath = "UNINITIALIZED";

        private bool shouldRun = false;
        private int guessesLeft = MAX_GUESSES;

        private string[] backupWords = {"Missing", "Something", "Within", "Execution", "Folder", "Backup", "Words", "Engaged" };
        private StringBuilder guessedLetters = new StringBuilder();
        private char[] correctGuesses = new char[0];
        private string currentWord = "";

        public void Start()
        {
            shouldRun = true;
            wordListPath = Path.Combine(executionFolder, "wordlist.txt");


            PrepareNewGame();
            Run();
        }

        private void PrepareNewGame()
        {
            //Get a word to be guessed
            //if the .txt can't be found use the backup words.
            if (File.Exists(wordListPath))
            {
                string[] lines = File.ReadAllLines(wordListPath);
                int _rand = rand.Next(0, lines.Length - 1);
                currentWord = lines[_rand];
            }
            else
            {
                int _rand = rand.Next(0, backupWords.Length - 1);
                currentWord = backupWords[_rand];
            }

            //Fill the correctGuesses array with underdashes
            correctGuesses = new char[currentWord.Length];
            for (int i = 0; i < correctGuesses.Length; i++)  { correctGuesses[i] = '_'; }

            //Empty previous guesses
            guessedLetters = new StringBuilder();

            //Reset guess count
            guessesLeft = MAX_GUESSES;
        }

        private void Run()
        {
            PrintGameState();

            while (shouldRun)
            {
                //If the player is out of guesses, end the game in a loss
                //Otherwise let them take another guess
                if (guessesLeft == 0)
                {
                    EndGame(false); 
                }
                else 
                { 
                    TakeGuess(); 
                }
            }
        }

        private void PrintGameState()
        {
            Console.Clear();
            //Console.WriteLine(currentWord); //Debug

            //Print the ASCII art
            AsciiDrawer.DrawHangmanedProgress(MAX_GUESSES-guessesLeft);
            
            //Print the correctly guessed letters
            for (int i = 0; i < correctGuesses.Length; i++)
            {
                Console.Write(correctGuesses[i] + " ");
            }
            Console.WriteLine();
           
            //Print amount of guesses left
            Console.WriteLine($"You have {guessesLeft} guesses left.");

            //Print already guessed letters
            Console.Write("You know the word doesn't contain these letters: ");
            for (int i = 0; i < guessedLetters.Length; i++)
            {
                Console.Write(guessedLetters[i] + " ");
            }
            Console.WriteLine();
        }

        private void TakeGuess()
        {
            Console.WriteLine("Please enter a guess, a single letter or the full word");
            string userInput = "";

            //Keep taking input until user enters a legal guess
            while (LegalGuess(Console.ReadLine(), out userInput) == false) 
            {
                //User keeps guessing
            }

            HandleGuess(userInput);
        }

        /// <summary>
        /// Ensure input is only letters, equal length to currentWord or only one letter.
        /// </summary>
        /// <param name="input">String to validate</param>
        /// <param name="validOutput">input if valid, otherwise L</param>
        /// <returns></returns>
        private bool LegalGuess(string input, out string validOutput)
        {
            bool noErrorFound = true;

            //Does input only consist of letters?
            for (int i = 0; i < input.Length; i++)
            {
                if (Char.IsLetter(input[i]) == false)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Guess must only contain letters");
                    Console.ResetColor();

                    noErrorFound = false;
                    break;
                    
                }
            }

            //Is input either length 1 or the length of the current word?
            if (input.Length != 1)
            {
                if (input.Length != currentWord.Length)
                {
                    
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("Guess must be either one letter or of equal length to the word");
                    Console.ResetColor();

                    noErrorFound = false;
                }
            }

            //Has a single letter already been guessed
            if (input.Length == 1)
            {
                if (guessedLetters.ToString().Contains(input[0]) || correctGuesses.Contains(input[0]))
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine("You've already guessed that letter before");
                    Console.ResetColor();

                    noErrorFound = false;
                }

            }



            validOutput = (noErrorFound) ? input : "L";

            //Debug close program
            if (input == "-1")
            {
                shouldRun = false;
                return true;
            }

            return noErrorFound;
        }

        private void HandleGuess(string guess)
        {
            //Guesses are not case sensitive
            guess = guess.ToLower();
            char charGuess = guess[0];

            //Check full guesses
            if (guess.Length != 1)
            {
                if (Equals(guess, currentWord)) //Correctly guessed the word
                {
                    //End the game in the players win
                    EndGame(true);
                }
                else
                {
                    //Subtract a guess, inform the player that they were close
                    guessesLeft--;

                    PrintGameState();
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    Console.WriteLine("Unfortunately, that is not the correct word");
                    Console.ResetColor();
                }

                return; //If it's a full word then don't check single letter guesses
            }

            //Check single char guess


            //is the letter a correct guess?
            if (currentWord.Contains(charGuess))
            {
                //upate the char array that dispalys the correct guesses in all places the guess matches
                for (int i = 0; i < currentWord.Length; i++)
                {
                    if (currentWord[i] == charGuess)
                    {
                        correctGuesses[i] = charGuess;
                    }
                }

            }
            else // if the guess is incorrect, remove a guess and add it to the list
            {
                guessedLetters.Append(charGuess);
                guessesLeft--;
            }
            PrintGameState();
        }

        private void EndGame(bool playerWon)
        {
            //If the palyer won, fill out the correct guesses fully and congratulate the player
            //If the player lost inform them what the correct word was and inform them that they've lost
            //Either way ask if they want to play again

            if (playerWon)
            {
                for (int i = 0; i < currentWord.Length; i++)
                {
                    correctGuesses[i] = currentWord[i];
                }

                PrintGameState();

                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine("You guessed correctly! Well played!");
                Console.ResetColor();
            }
            else
            {
                PrintGameState();

                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine("Unfortunatly, you've lost the game!");
                Console.WriteLine($"The word was {currentWord}");
                Console.ResetColor();
            }

            Console.WriteLine("\n Do you want to play again? (Y)es/(N)o?");
            char confirmationResponse = '_';
            while (ConfirmationValidated(Console.ReadLine(), out confirmationResponse) == false) //Keep taking input until user enters a legal confirmation
            {
                //User must enter input that begins with a Y or a N (not case sensitive)
            }

            if (confirmationResponse == 'Y')//Start a new game
            {
                PrepareNewGame();

                Console.WriteLine("New game ready, press any key to begin");
                Console.ReadKey();
                PrintGameState();
            }
            else if (confirmationResponse == 'N')//exit application
            {
                shouldRun = false;
            }

        }

        private bool ConfirmationValidated(string input, out char confirmation)
        {
            
            //if there is no input return false
            if (input.Length == 0)
            {
                confirmation = 'N';
                return false;
            }

            //Is the first char Y or N?
            char charInput = input.ToUpper()[0];
            if (Equals(charInput, 'Y') || Equals(charInput, 'N'))
            {
                confirmation = charInput;
                return true;
            }
            else
            {
                confirmation = 'N';
                return false;
            }

        }
    }
}

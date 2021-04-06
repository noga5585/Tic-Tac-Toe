using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tic_Tac_Toe
{
    class BestComputerMove
    {
        public static bool secondTime = true; 
        
        // check how many options there are ( free cells ). 
        public static int Select_A_Cell(ButtonMark[] array, bool isPlayerXTurn)
        {           
            List<int> tempList = new List<int>();
            for(int i = 0; i < array.Length; i++)
            {
                if(array[i] == ButtonMark.Free)
                {
                    tempList.Add(i); 
                }
            }
            int[] options = tempList.ToArray();            
            return Get_Max_Option(options, array, isPlayerXTurn); // return the best option 
        }

        // check how is the best option.
        private static int Get_Max_Option(int[] options, ButtonMark[] array, bool isPlayerXTurn)
        {
            ButtonMark[] copyArray = new ButtonMark[array.Length];
            secondTime = !secondTime; 
            array.CopyTo(copyArray, 0);
            List<int> maxOptions = new List<int>();
            // Initializes the best option to be the first to have
            maxOptions.Add(options[0]);
            int maxGrade = GetGrade(options[0], copyArray, isPlayerXTurn);           
            for (int i = 1; i < options.Length; i++) // go over the all options
            {
                array.CopyTo(copyArray,0);
                int grade = GetGrade(options[i], copyArray, isPlayerXTurn);  
                // best option for X move
                if (maxGrade == grade) // if there is option this the same grade 
                {
                    maxOptions.Add(options[i]); // add this option to the list
                }
                else if(isPlayerXTurn && maxGrade < grade) // if there is option with beter grade
                {
                    maxOptions.Clear();
                    maxGrade = grade;
                    maxOptions.Add(options[i]);
                }
                else if (!isPlayerXTurn && maxGrade > grade)
                {

                    maxOptions.Clear();
                    maxOptions.Add(options[i]);
                    maxGrade = grade;
                }
            }
            // Choose a random selection from all the best options available at this moment of the game.
            Random rnd = new Random();
            int index = rnd.Next(0, maxOptions.Count);
            return maxOptions[index]; 
        }

        // Gets the score of the move.
        private static int GetGrade(int option, ButtonMark[] array, bool isPlayerXTurn)
        {
            if(BoardGame.winTheComputer || (!BoardGame.btn1.IsEnabled && secondTime))
            {
                return -1; 
            }
            int moreCellFree = 0; 
            // mark the option 
            if (isPlayerXTurn)
            {
                array[option] = ButtonMark.Noutht;
            }
            else
                array[option] = ButtonMark.Cross;

            // check how many cells are free 
            for (int i = 0; i < array.Length; i++)
            {
                if (array[i] == ButtonMark.Free)
                {
                    moreCellFree += 1; 
                }
            }
            // if after this move the is win.
            GameResult gameResult = CheckForWinner.Chack_For_Winner(array);
            // Play until there are no more free cells or a result
            while (moreCellFree != 0 && gameResult == GameResult.Nothing)
            {
                isPlayerXTurn = !isPlayerXTurn;
                int choice = Select_A_Cell(array, isPlayerXTurn);
                if (isPlayerXTurn)
                {
                    array[choice] = ButtonMark.Noutht;
                }
                else
                    array[choice] = ButtonMark.Cross;
                moreCellFree -= 1;
                gameResult = CheckForWinner.Chack_For_Winner(array);
            }
            //Checking the end result of a game
            if (gameResult == GameResult.Xwin)
            {
                return 1;
            }
            if (gameResult == GameResult.Owin)
            {
                return -1;
            }
            return 0; // for tie
        }
    }
}

using System.Media;
using static System.Net.Mime.MediaTypeNames;

namespace Tic_Tac_Toe
{
    class CheckForWinner : BoardGame
    {
        // Checks if there is a win - all the options for a win
        public static GameResult Chack_For_Winner(ButtonMark[] buttonMarks)
        {
            ButtonMark howWin = ButtonMark.Free;
            thisGameResult = GameResult.Nothing;
            bool tie = true;
            // check row
            if (buttonMarks[0] != ButtonMark.Free && (buttonMarks[0] & buttonMarks[1] & buttonMarks[2]) == buttonMarks[0])
            {
                howWin = buttonMarks[0];
            }
            else if (buttonMarks[3] != ButtonMark.Free && (buttonMarks[3] & buttonMarks[4] & buttonMarks[5]) == buttonMarks[3])
            {
                howWin = buttonMarks[3];
            }
            else if (buttonMarks[6] != ButtonMark.Free && (buttonMarks[6] & buttonMarks[7] & buttonMarks[8]) == buttonMarks[6])
            {
                howWin = buttonMarks[6];
            }

            // check column 
            else if (buttonMarks[0] != ButtonMark.Free && (buttonMarks[0] & buttonMarks[3] & buttonMarks[6]) == buttonMarks[0])
            {
                howWin = buttonMarks[0];
            }
            else if (buttonMarks[1] != ButtonMark.Free && (buttonMarks[1] & buttonMarks[4] & buttonMarks[7]) == buttonMarks[1])
            {
                howWin = buttonMarks[1];
            }
            else if (buttonMarks[2] != ButtonMark.Free && (buttonMarks[2] & buttonMarks[5] & buttonMarks[8]) == buttonMarks[2])
            {
                howWin = buttonMarks[2];
            }
            // check slant
            else if (buttonMarks[0] != ButtonMark.Free && (buttonMarks[0] & buttonMarks[4] & buttonMarks[8]) == buttonMarks[0])
            {
                howWin = buttonMarks[0];
            }
            else if (buttonMarks[2] != ButtonMark.Free && (buttonMarks[2] & buttonMarks[4] & buttonMarks[6]) == buttonMarks[2])
            {
                howWin = buttonMarks[2];
            }
            // if O won
            if (howWin == ButtonMark.Cross)
            {
                thisGameResult = GameResult.Owin;
                return thisGameResult;
            }
            // if X won
            if (howWin == ButtonMark.Noutht)
            {
                thisGameResult = GameResult.Xwin;
                return thisGameResult;
            }
            // check if there are empty cells
            for (int i = 0; i < buttonMarks.Length; i++)
            {
                if (buttonMarks[i] == ButtonMark.Free)
                {
                    tie = false;
                    break;
                }
            }
            // if the not a emply cell and the is not winner this tie.
            if (tie)
            {
                thisGameResult = GameResult.Tie;
                return thisGameResult;
            }
            // there is not result. 
            return thisGameResult;
        }
    }

}


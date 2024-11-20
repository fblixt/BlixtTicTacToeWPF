using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace BlixtTicTacToe
{
    class GameLogic
    {
        private string[,] board = new string[3, 3];
        private bool xIsUp = true;

        public string CurrentPlayer => xIsUp ? "x" : "o";

        public bool SetMark(int row, int col)
        {
            if (board[row, col] == null)
            {
                board[row, col] = CurrentPlayer;
                return true;
            }
            return false;
        }

        public string? CheckWinner()
        {
            for (int i = 0; i < 3; i++)
            {
                if (board[i, 0] != null && board[i, 0] == board[i, 1] && board[i, 1] == board[i, 2])
                    return board[i, 0];

                if (board[0, i] != null && board[0, i] == board[1, i] && board[1, i] == board[2, i])
                    return board[0, i];
            }

            if (board[0, 0] != null && board[0, 0] == board[1, 1] && board[1, 1] == board[2, 2])
                return board[0, 0];

            if (board[0, 2] != null && board[0, 2] == board[1, 1] && board[1, 1] == board[2, 0])
                return board[0, 2];

            xIsUp = !xIsUp;
            return null;
        }
        public void NewGame()
        {
            board = new string[3, 3];
            xIsUp = true;
        }

        public bool IsDraw()
        {
            foreach (var button in board)
            {
                if (button == null) return false;
            }
            return true;
        }
    }

}

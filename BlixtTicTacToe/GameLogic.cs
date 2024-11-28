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
        private string?[,] board = new string[3, 3];
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

        public (int row, int col) GetEasyComMark()
        {
            var emptyButtons = new List<(int row, int col)>();

            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    if (board[row, col] == null)
                    {
                        emptyButtons.Add((row, col));
                    }
                }
            }

            Random random = new();
            int index = random.Next(emptyButtons.Count);
            return emptyButtons[index];
        }

        public (int row, int col) GetImpossibleComMark()
        {
            if (board[1, 1] == null)
                return (1, 1);

            string comMark = xIsUp ? "x" : "o";
            string playerMark = xIsUp ? "o" : "x";

            var winningMove = FindWinningMove(comMark);
            if (winningMove != null)
                return winningMove.Value;

            var blockingMove = FindWinningMove(playerMark);
            if (blockingMove != null)
                return blockingMove.Value;

            var doubleThreat = FindDoubleThreat(comMark);
            if (doubleThreat != null)
                return doubleThreat.Value;

            var cornerCloseMove = FindCornerNextToOpponentMark(playerMark);
            if (cornerCloseMove != null)
                return cornerCloseMove.Value;

            var cornerMove = FindCornerMove();
            if (cornerMove != null)
                return cornerMove.Value;

            return GetEasyComMark();
        }

        private (int row, int col)? FindWinningMove(string mark)
        {
            for (int row = 0; row < 3; row++)
            {
                int count = 0;
                (int row, int col)? emptyButton = null;

                for (int col = 0; col < 3; col++)
                {
                    if (board[row, col] == mark)
                        count++;
                    else if (board[row, col] == null)
                        emptyButton = (row, col);
                }

                if (count == 2 && emptyButton != null)
                    return emptyButton.Value;
            }

            for (int col = 0; col < 3; col++)
            {
                int count = 0;
                (int row, int col)? emptyButton = null;

                for (int row = 0; row < 3; row++)
                {
                    if (board[row, col] == mark)
                        count++;
                    else if (board[row, col] == null)
                        emptyButton = (row, col);
                }

                if (count == 2 && emptyButton != null)
                    return emptyButton.Value;
            }

            var diagonals = new (int, int)[]
            {
                (0, 0), (1, 1), (2, 2),
                (0, 2), (1, 1), (2, 0)
            };

            for (int i = 0; i < diagonals.Length; i += 3)
            {
                int count = 0;
                (int row, int col)? emptySpot = null;

                for (int j = 0; j < 3; j++)
                {
                    var (row, col) = diagonals[i + j];
                    if (board[row, col] == mark)
                        count++;
                    else if (board[row, col] == null)
                        emptySpot = (row, col);
                }

                if (count == 2 && emptySpot != null)
                    return emptySpot.Value;
            }

            return null;
        }

        private (int row, int col)? FindDoubleThreat(string player)
        {
            for (int row = 0; row < 3; row++)
            {
                for (int col = 0; col < 3; col++)
                {
                    if (board[row, col] == null)
                    {
                        board[row, col] = player;
                        int winningLines = CountWinningLines(player);
                        board[row, col] = null;

                        if (winningLines >= 2)
                        {
                            return (row, col);
                        }
                    }
                }
            }

            return null;
        }

        private int CountWinningLines(string player)
        {
            int count = 0;
            
            for (int i = 0; i < 3; i++)
            {
                if (CanWinLine(player, board[i, 0], board[i, 1], board[i, 2]))
                    count++;
                if (CanWinLine(player, board[0, i], board[1, i], board[2, i]))
                    count++;
            }

            if (CanWinLine(player, board[0, 0], board[1, 1], board[2, 2]))
                count++;
            if (CanWinLine(player, board[0, 2], board[1, 1], board[2, 0]))
                count++;

            return count;
        }

        private bool CanWinLine(string player, string? a, string? b, string? c)
        {
            return (a == player && b == player && c == null) ||
                   (a == player && c == player && b == null) ||
                   (b == player && c == player && a == null);
        }

        private (int row, int col)? FindCornerNextToOpponentMark(string opponentMark)
        {
            var corners = new (int row, int col)[] { (0, 0), (0, 2), (2, 0), (2, 2) };
            foreach (var corner in corners)
            {
                if (board[corner.row, corner.col] != null)
                    continue;

                var adjacentPositions = GetAdjacentPositions(corner);
                if (adjacentPositions.Any(pos =>
                    pos.row >= 0 && pos.row < 3 && pos.col >= 0 && pos.col < 3 && board[pos.row, pos.col] == opponentMark))
                {
                    return corner;
                }
            }

            return null;
        }

        private (int row, int col)? FindCornerMove()
        {
            var corners = new List<(int row, int col)>
            {
                (0, 0), (0, 2), (2, 0), (2, 2)
            };

            foreach (var (row, col) in corners)
            {
                if (board[row, col] == null)
                {
                    if (row == 0 && col == 0 && board[2, 2] == CurrentPlayer) continue;
                    if (row == 0 && col == 2 && board[2, 0] == CurrentPlayer) continue;
                    if (row == 2 && col == 0 && board[0, 2] == CurrentPlayer) continue;
                    if (row == 2 && col == 2 && board[0, 0] == CurrentPlayer) continue;

                    return (row, col);
                }
            }

            return null;
        }

        private List<(int row, int col)> GetAdjacentPositions((int row, int col) corner)
        {
            return corner switch
            {
                (0, 0) => new List<(int row, int col)> { (0, 1), (1, 0) },
                (0, 2) => new List<(int row, int col)> { (0, 1), (1, 2) },
                (2, 0) => new List<(int row, int col)> { (1, 0), (2, 1) },
                (2, 2) => new List<(int row, int col)> { (1, 2), (2, 1) },
                _ => new List<(int row, int col)>()
            };
        }

        public int CountPlayerMarks(string player)
        {
            int count = 0;
            foreach (var position in board)
            {
                if (position == player)
                {
                    count++;
                }
            }
            return count;
        }

        public void MoveMark(int rowFrom, int colFrom, int rowTo, int colTo)
        {
            if (board[rowFrom, colFrom] != CurrentPlayer) return;

            board[rowTo, colTo] = CurrentPlayer;
            board[rowFrom, colFrom] = null;
        }
        public string? GetPlayerAtPosition(int row, int col)
        {
            return board[row, col];
        }

    }
}

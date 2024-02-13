using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Mankalah
{
    public class jy49Player : Player
    {
        public jy49Player(Position pos, int maxTimePerMove) 
            : base(pos, "jy49Player", maxTimePerMove) 
        {
        }

        public override string getImage()
        {
            return "thepicture.png";
        }
       
        
        public override string gloat()
        {
            return "Caught in a whirlwind of victory laps, I gloat in my Mankalah conquest, with every stone moved, another chapter of triumph written.";
        }

        public override int evaluate(Board b)
        {
            int score = 0;

            // Calculates the difference between the numbers of scores at position 13 and 6 and adds it to the board
            score += b.stonesAt(13) - b.stonesAt(6);

            // Trackers of the amount of stones
            int topStones = 0;
            int bottomStones = 0;
            
            // Stones on the top row
            for (int i = 7; i <= 12; i++)
            {
                topStones += b.stonesAt(i);
            }

            // Stones on the bottom row 
            for (int i = 0; i <= 5; i++)
            {
                bottomStones += b.stonesAt(i);
            }
            score += topStones - bottomStones;

            // Go-again situations
            int topGoAgain = 0;
            int bottomGoAgain = 0;

            // Counts how many times the top and bottom row has to go again
            for (int i =7; i <= 12; i++)
            {
                if (b.stonesAt(i) == (13-i))
                {
                    topGoAgain++;
                }
            }
            score += topGoAgain - bottomGoAgain;

            // Keeps in track of the potential captures 
            int totalCaptures = 0;
            for (int i = 7; i <= 12; i++)
            {
                int target = i + b.stonesAt(i);
                if (target < 13)
                {
                    int targetStones = b.stonesAt(target);

                    // I gained help from ChatGPT to come up with this line of code
                    // 
                    if (b.whoseMove() == Position.Top && targetStones == 0 && b.stonesAt(12 - target) != 0)
                    {
                        totalCaptures += b.stonesAt(12 - target);
                    }
                }
            }
            score += totalCaptures;
            return score;
        }


        // Starts the watch and sets the time and uses staged DFS
        public override int chooseMove(Board b)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            int depth = 1;
            Result move = new Result(0, 0);

            while (watch.ElapsedMilliseconds < getTimePerMove())
            {
                move = minimax(b, depth++, watch, int.MinValue, int.MaxValue);
                //Console.WriteLine("Current best move: " + move.getMove() + " and value " + move.getScore());
            }
            return move.getMove();
        }


        // The powerpoint slide was really helpful
        private Result minimax(Board b, int d, Stopwatch w, int alpha, int beta)
        {
            int bestMove = 0;
            int bestValue;

            if (b.gameOver() || d == 0)
            {
                return new Result(0, evaluate(b));
            }

            // Checks through all the moves that are on top 
            if (b.whoseMove() == Position.Top)
            {
                bestValue = int.MinValue;
                for (int move = 7; move <= 12; move++)
                {
                    if (b.legalMove(move))
                    {
                        Board b1 = new Board(b);
                        b1.makeMove(move, false);
                        Result val = minimax(b1, d - 1, w, alpha, beta);
                        if (val.getScore() > bestValue)
                        {
                            bestValue = val.getScore();
                            bestMove = move;
                        }

                        // Alpha-beta pruning 
                        alpha = Math.Max(alpha, bestValue);
                        if (beta <= alpha)
                        {
                            break;
                        }
                    }
                }
            }

            else
            {
                // Checks all the moves that are on the bottom 
                bestValue = int.MaxValue;
                for (int move = 0; move <= 5; move++)
                {
                    if (b.legalMove(move))
                    {
                        Board b1 = new Board(b);
                        b1.makeMove(move, false);
                        Result val = minimax(b1, d - 1, w, alpha, beta);
                        if (val.getScore() < bestValue)
                        {
                            bestValue = val.getScore();
                            bestMove = move;
                        }

                        // Alpha-beta pruning 
                        beta = Math.Min(beta, bestValue);
                        if (beta <= alpha)
                        {
                            break;
                        }
                    }
                }
            }
            return new Result(bestMove, bestValue);
        }

       


        class Result
        {
            private int move;
            private int score;

            public Result(int m, int s)
            {
                move = m;
                score = s;
            }

            public int getMove()
            {
                return move;
            }

            public int getScore()
            {
                return score;
            }
        }
        // adapt all code from your player class into this

    }
}
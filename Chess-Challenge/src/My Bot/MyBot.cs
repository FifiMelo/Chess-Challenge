using ChessChallenge.API;
using System;
using System.Diagnostics;
using System.Collections.Generic;


public class MyBot : IChessBot
{
    public
    Move the_best_move;
    int fixed_depth = 5;
    int[] material = new int[7] { 0, 1, 3, 3, 5, 9, 0 };


    float StaticEvaluation(Board board)
    {
        Move[] moves = board.GetLegalMoves();
        foreach(Move move in moves)
        {

        }
        return 2;
    }

    float material_eval(Board board, bool all_positive = false)
    {

        float e = 0;
        foreach(PieceList list in board.GetAllPieceLists())
        {
            foreach(Piece piece in list)
            {
                e += material[(int)piece.PieceType] * (piece.IsWhite ? 1 : all_positive ? 1 : -1);
            }

        }
        return e;
    }
    int compare_moves(Move move1, Move move2)
    {
        int result = 0;
        if (move1.IsCapture)
        {
            result--;
            if (move1.MovePieceType == PieceType.Pawn)
                result--;
        }

        if (move2.IsCapture)
        {
            result++;
            if (move2.MovePieceType == PieceType.Pawn)
                result++;
        }

        return result > 0 ? 1 : (result < 0 ? -1 : 0);
    }

    float Minimax(Board board, int depth, float alpha, float beta)
    {


        // check for mate, draw other

        if (board.IsDraw())
        {
            //Console.WriteLine("Found draw");
            return 0;
        }

        if (board.IsInCheckmate())
        {
            Console.WriteLine("Found result");
            if (!board.IsWhiteToMove)
            {
                return float.MinValue + fixed_depth - depth;
            }
            return float.MaxValue - fixed_depth + depth;
        }



        // if in leaf
        if (depth == 0)
            return StaticEvaluation(board);

        Move[] moves = board.GetLegalMoves();
        Array.Sort(moves, compare_moves);
        //Array.Sort(available_moves, compare_moves);
        if (board.IsWhiteToMove)
        {
            float max_eval = float.MinValue;

            foreach (Move move in moves)
            {
                board.MakeMove(move);
                float eval = Minimax(board, depth - 1, alpha, beta);
                /*
                Console.WriteLine("Pozycje: " + board.GetFenString());
                Console.WriteLine("Na glebokosci: " + depth);
                Console.WriteLine("Oceniam na: " + eval);
                */
                if (eval > max_eval)
                {
                    max_eval = eval;
                    if (depth == fixed_depth)
                        the_best_move = move;
                }
                board.UndoMove(move);
                alpha = Math.Max(alpha, eval);

                if (beta <= alpha)
                {
                    break;
                }

            }
            return max_eval;
        }

        float min_eval = float.MaxValue;
        foreach (Move move in moves)
        {
            board.MakeMove(move);
            float eval = Minimax(board, depth - 1, alpha, beta);
            /*
            Console.WriteLine("Pozycje: " + board.GetFenString());
            Console.WriteLine("Na glebokosci: " + depth);
            Console.WriteLine("Oceniam na: " + eval);
            */
            if (eval < min_eval)
            {
                min_eval = eval;
                if (depth == fixed_depth)
                    the_best_move = move;
            }
            board.UndoMove(move);

            beta = Math.Min(beta, eval);
            if (beta <= alpha)
            {
                break;
            }

        }

        return min_eval;
    }


    public Move Think(Board board, Timer timer)
    {
        Minimax(board, fixed_depth, float.MinValue, float.MaxValue);


        return the_best_move;

    }
}

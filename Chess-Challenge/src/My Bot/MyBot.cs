using ChessChallenge.API;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;

public class MyBot : IChessBot
{
    public Move the_best_move;
    public int fixed_depth = 6;

    public float StaticEvaluation(Board board)
    {
        int e = 0;
        e += board.GetPieceList(PieceType.Rook, true).Count * 5;
        e += board.GetPieceList(PieceType.Queen, true).Count * 9;
        e += board.GetPieceList(PieceType.Bishop, true).Count * 3;
        e += board.GetPieceList(PieceType.Knight, true).Count * 3;
        e += board.GetPieceList(PieceType.Pawn, true).Count * 1;

        e += board.GetPieceList(PieceType.Rook, false).Count * -5;
        e += board.GetPieceList(PieceType.Queen, false).Count * -9;
        e += board.GetPieceList(PieceType.Bishop, false).Count * -3;
        e += board.GetPieceList(PieceType.Knight, false).Count * -3;
        e += board.GetPieceList(PieceType.Pawn, false).Count * -1;
        
        return e;
    }
    public int compare_moves(Move move1, Move move2)
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
    public float Minimax(Board board, int depth, bool maximizingPlayer, float alpha, float beta)
    {
        // check for mate, draw other
        
        if (board.IsDraw())
        {
            //Console.WriteLine("Found draw");
            return 0;
        }
            
        if (board.IsInCheckmate())
        {
            //Console.WriteLine("Found result");
            if(!board.IsWhiteToMove)
            {
                return float.MinValue + fixed_depth - depth;
            }
            return float.MaxValue - fixed_depth + depth;
        }
        
        

        // if in leaf
        if (depth == 0)
            return StaticEvaluation(board);
        Move[] available_moves = board.GetLegalMoves();
        Array.Sort(available_moves, compare_moves);
        if (maximizingPlayer)
        {
            float max_eval = float.MinValue;
            
            
            foreach (Move move in available_moves)
            {
                board.MakeMove(move);
                float eval = Minimax(board, depth - 1, false, alpha, beta);
                if(eval > max_eval)
                {
                    max_eval = eval;
                    if(depth == fixed_depth)
                        the_best_move = move;
                }
                board.UndoMove(move);
                alpha = Math.Max(alpha, eval);
                
                if(beta <= alpha)
                {
                    break;
                }
                

            }
            return max_eval;
        }

        float min_eval = float.MaxValue;
        foreach (Move move in available_moves)
        {
            board.MakeMove(move);
            float eval = Minimax(board, depth - 1, true, alpha, beta);
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
        Console.WriteLine(Minimax(board, fixed_depth, board.IsWhiteToMove, float.MinValue, float.MaxValue));
        return the_best_move;
        
    }

}
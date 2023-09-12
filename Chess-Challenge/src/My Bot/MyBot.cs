using ChessChallenge.API;
using System;
using System.Diagnostics;
using System.Collections.Generic;


public class MyBot : IChessBot
{
    
    Move the_best_move;
    int fixed_depth = 5;
    int[] material = new int[7] { 0, 1, 3, 3, 5, 9, 0 };
    float[] control_piece_weights = new float[7] { 0, 6, 3, 3, 1, 0, -1F };
    float[] control_square_weight = new float[64];

    public MyBot()
    {
        for(int i = 0; i < 64;i++)
            control_square_weight[i] = 0.01F;
        control_square_weight[27] = 0.1F;
        control_square_weight[28] = 0.1F;
        control_square_weight[35] = 0.1F;
        control_square_weight[36] = 0.1F;

        control_square_weight[0] = -0.4F;
        control_square_weight[1] = -0.4F;
        control_square_weight[6] = -0.4F;
        control_square_weight[7] = -0.4F;
        control_square_weight[63] = -0.4F;
        control_square_weight[62] = -0.4F;
        control_square_weight[56] = -0.4F;
        control_square_weight[57] = -0.4F;
    }
    
    
    float SpaceControlEval(Board board)
    {
        float e = 0;
        float temp = 0;
        foreach (Move move in board.GetLegalMoves())
        {
            temp = control_piece_weights[(int)move.MovePieceType] *
                control_square_weight[move.TargetSquare.Index] *
                (board.IsWhiteToMove ? 1 : -1);
            Console.WriteLine(move.MovePieceType);
            Console.WriteLine(temp);
            e += temp;
        }
        board.ForceSkipTurn();
        foreach (Move move in board.GetLegalMoves())
        {
            temp = control_piece_weights[(int)move.MovePieceType] *
               control_square_weight[move.TargetSquare.Index] *
               (board.IsWhiteToMove ? 1 : -1);
            Console.WriteLine(move.MovePieceType);
            Console.WriteLine(temp);
            e += temp;
        }
        board.UndoSkipTurn();
        return e;

    }



    float StaticEvaluation(Board board)
    {
        return MaterialEval(board) + SpaceControlEval(board);
    }

    float MaterialEval(Board board, bool all_positive = false)
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
            return board.IsWhiteToMove ? float.MinValue : float.MaxValue;
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
        //Console.WriteLine(Minimax(board, fixed_depth, float.MinValue, float.MaxValue));
        //SpaceControlEval(board);
        //return the_best_move;

        analize_position("4r1k1/1q3ppp/1pp5/4p3/6P1/5P1P/PPPQ4/1KR5 b - - 0 1");
        return board.GetLegalMoves()[1];

    }

    void analize_position(string fen)
    {
        Board board = Board.CreateBoardFromFEN(fen);
        SpaceControlEval(board);
        
    }

}

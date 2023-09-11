using ChessChallenge.API;
using System;
using System.Diagnostics;
using System.Collections.Generic;


public class MyBot : IChessBot
{
    public Move the_best_move;
    public int fixed_depth = 5;
    public int[,] white_pawn_movement = new int[2, 2]
{
        { 7, 1},
        { 9, 1},
};
    public int[,] black_pawn_movement = new int[2, 2]
    {
        { -7, -1},
        { -9, -1},
    };
    public int[,] knight_movement = new int[8, 2]
    {
        {15, 2},
        {17, 2},
        {10, 1},
        {-6, -1},
        {-15, -2},
        {-17, -2},
        {-10, -1},
        {6, 1},
    };
    public int[,] king_movement = new int[8, 2]
    {
        {7 , 1},
        {8 , 1},
        {9 , 1},
        {1 , 0},
        {-7 , -1},
        {-8 , -1},
        {-9 , -1},
        {-1 , 0},
    };



    public float StaticEvaluation(Board board)
    {
        int[] control = new int[64];
        int eval = 0;



        foreach (PieceList piecelist in board.GetAllPieceLists())
        {
            foreach (Piece piece in piecelist)
            {

                int square = piece.Square.Index;
                if (piece.IsKnight)
                {

                    for (int i = 0; i < 8; i++)
                    {
                        Console.WriteLine(square / 8 + knight_movement[i, 1]);
                        Console.WriteLine((square + knight_movement[i, 0]) / 8);
                        if (square / 8 + knight_movement[i, 1] == (square + knight_movement[i, 0]) / 8 &&
                            0 <= square + knight_movement[i, 0] &&
                            square + knight_movement[i, 0] < 64)
                        {
                            control[square + knight_movement[i, 0]] += piece.IsWhite ? 1 : -1;
                            
                        }
                        
                    }
                    eval += piece.IsWhite ? 3 : -3;
                }

                if (piece.IsPawn && piece.IsWhite)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        if (square / 8 + white_pawn_movement[i, 1] == (square + white_pawn_movement[i, 0]) / 8)
                        {
                            control[square + white_pawn_movement[i, 0]]++;
                            
                        }
                    }
                    eval++;
                }

                if (piece.IsPawn && !piece.IsWhite)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        if (square / 8 + black_pawn_movement[i, 1] == (square + black_pawn_movement[i, 0]) / 8)
                        {
                            control[square + black_pawn_movement[i, 0]]--;
                            
                        }
                    }
                    eval--;
                }

                if (piece.IsKing)
                {
                    for (int i = 0; i < 8; i++)
                    {
                        if (square / 8 + king_movement[i, 1] == (square + king_movement[i, 0]) / 8 &&
                            0 <= square + king_movement[i, 0] &&
                            square + king_movement[i, 0] < 64)
                        {
                            control[square + king_movement[i, 0]] += piece.IsWhite ? 1 : -1;
                        }
                    }
                }

            }
        }




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

    public float Minimax(Board board, int depth, float alpha, float beta)
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

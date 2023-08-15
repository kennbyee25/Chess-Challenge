using ChessChallenge.API;
using System;
using System.Threading.Tasks.Sources;

public class MyBot : IChessBot
{
    // Piece values: null, pawn, knight, bishop, rook, queen, king
    int[] pieceValues = { 0, 100, 300, 300, 500, 900, 10000 };
    int searchDepth = 3;

    public Move Think(Board board, Timer timer)
    {
        MinimaxAlphaBeta(board, searchDepth, out Move moveToPlay, timer);
        return moveToPlay;
    }

    int EvaluateBoard(Board board)
    {
        int neg = board.IsWhiteToMove ? 1 : -1;

        if (board.IsInCheckmate())
        {
            return -10000;
        }

        PieceList[] pieces = board.GetAllPieceLists();

        int score = 0;
        // white
        for (int i = 0; i < 6; i++)
        {
            score += pieceValues[i] * pieces[i].Count;
        }
        // black
        for (int i = 6; i < 12; i++)
        {
            score -= pieceValues[i - 6] * pieces[i].Count;
        }

        return neg * score;
    }

    int MinimaxAlphaBeta(Board board, int depth, out Move bestMove, Timer timer)
    {
        Move[] moves = board.GetLegalMoves();
        bestMove = new Move();
        if (moves.Length == 0 || depth == 0 || timer.MillisecondsElapsedThisTurn > 5000)
        {
            return EvaluateBoard(board);
        }

        int bestScore = int.MinValue;

        Random rng = new();
        int[] randomIndices = new int[moves.Length];
        for (int i = 0; i < moves.Length; i++)
        {
            randomIndices[i] = i;
        }
        for (int i = 0; i < moves.Length; i++)
        {
            int j = rng.Next(i, moves.Length);
            int temp = randomIndices[i];
            randomIndices[i] = randomIndices[j];
            randomIndices[j] = temp;
        }
        for (int i = 0; i < moves.Length; i++)
        {
            Move move = moves[randomIndices[i]];
            board.MakeMove(move);
            int score = -MinimaxAlphaBeta(board, depth - 1, out _, timer);
            board.UndoMove(move);

            if (score > bestScore)
            {
                bestScore = score;
                bestMove = move;
            }
        }
        
        return bestScore;
    }
}
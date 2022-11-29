using ChessLoggerWPF;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ChessLoggerWPF
{

    public class Game
    {
        public Piece[,] board = new Piece[8, 8];
        public bool currentlyWhite = true;
        public List<Position> WhiteAttackPositions = new List<Position>();
        public List<Position> BlackAttackPositions = new List<Position>();

        public Game()
        {
            for (int i = 0; i < 8; i++)
            {
                board[1, i] = new Pawn(true, new Position(1, i));
                board[6, i] = new Pawn(false, new Position(6, i));
            }
            board[0, 0] = new Rook(true, new Position(0, 0));
            board[0, 7] = new Rook(true, new Position(0, 7));
            board[7, 0] = new Rook(false, new Position(7, 0));
            board[7, 7] = new Rook(false, new Position(7, 7));

            board[0, 1] = new Horse(true, new Position(0, 1));
            board[0, 6] = new Horse(true, new Position(0, 6));
            board[7, 1] = new Horse(true, new Position(7, 1));
            board[7, 6] = new Horse(true, new Position(7, 6));

            board[0, 2] = new Bishop(true, new Position(0, 2));
            board[0, 5] = new Bishop(true, new Position(0, 5));
            board[7, 2] = new Bishop(false, new Position(7, 2));
            board[7, 5] = new Bishop(false, new Position(7, 5));

            board[0, 3] = new Queen(true, new Position(0, 3));
            board[7, 4] = new Queen(false, new Position(7, 4));

            board[0, 4] = new King(true, new Position(0, 4));
            board[7, 3] = new King(false, new Position(7, 3));



        }

        public string createStringOfGame()
        {
            string currentGameState = "";
            for (int i = 7; i > -1; i--)
            {
                for (int j = 0; j < 8; j++)
                {
                    currentGameState += (board[i, j] == null ? "NULL" : board[i, j].toString()) + "|";
                }
                currentGameState += "\n";
            }

            return currentGameState;
        }

        private void createAttackingZones()
        {
            this.WhiteAttackPositions.Clear();
            this.BlackAttackPositions.Clear();
            King BlackKing = null;
            King WhiteKing = null;
            for (int i = 0; i < 8; i++)
            {
                for (int j = 0; j < 8; j++)
                {
                    Piece p = board[i, j];
                    if (p is King)
                    {
                        if ((p as King).isWhite)
                        {
                            WhiteKing = (King)p;
                        }
                        else
                        {
                            BlackKing = (King)p;
                        }
                    }
                    if (p != null)
                    {
                        if (p.isWhite)
                            this.WhiteAttackPositions.AddRange(p.createMoveList(board));
                        else
                            this.BlackAttackPositions.AddRange(p.createMoveList(board));
                    }                    
                }
            }
            WhiteKing.canMoveList.RemoveAll(s => BlackAttackPositions.Contains(s));
            BlackKing.canMoveList.RemoveAll(s => WhiteAttackPositions.Contains(s));

            if (WhiteKing.canMoveList.Count == 0 && BlackAttackPositions.Contains(WhiteKing.Position))
                throw new Exception("SAKK MATT FEHÉR KIKAPOTT");
            if (BlackKing.canMoveList.Count == 0 && WhiteAttackPositions.Contains(BlackKing.Position))
                throw new Exception("SAKK MATT FEKETE KIKAPOTT");
        }

        /// <summary>
        /// THis method gives back the new position of a piece and updates the game status
        /// </summary>
        /// <param name="impactedPosition1"></param>
        /// <param name="impactedPosition2"></param>
        /// <returns>Either the new position as a String or "ERROR"</returns>
        public String CalculateNextStep(Position impactedPosition1, Position impactedPosition2)
        {
            this.createAttackingZones();
            string step = "Error";
            Piece p1 = board[impactedPosition1.x, impactedPosition1.y];
            if (p1 != null && currentlyWhite == p1.isWhite && p1.MoveTo(impactedPosition2, board))
            {
                step = board[impactedPosition2.x, impactedPosition2.y].toString();
                currentlyWhite = !currentlyWhite;
                return step;
            }

            Piece p2 = board[impactedPosition2.x, impactedPosition2.y];
            if (p2 != null && currentlyWhite == p2.isWhite && p2.MoveTo(impactedPosition1, board))
            {
                step = board[impactedPosition1.x, impactedPosition1.y].toString();
                currentlyWhite = !currentlyWhite;
                return step;
            }
            this.createAttackingZones();
            return step;
        }

    }

    public class Pawn : Piece
    {
        public Pawn(bool isWhite, Position position) : base(isWhite, position)
        {
        }

        public override List<Position> createMoveList(Piece[,] board)
        {
            this.canMoveList = new List<Position>();
            if (this.isWhite)
            {
                if (this.Position.x < 6 && board[this.Position.x + 1, this.Position.y] == null)
                    this.canMoveList.Add(new Position(Position.x + 1, Position.y));
                if (Position.x < 7 && Position.y < 7 && board[Position.x + 1, Position.y + 1] != null
                    && !board[Position.x + 1, Position.y + 1].isWhite)
                    this.canMoveList.Add(new Position(Position.x + 1, Position.y + 1));
                if (Position.x < 7 && Position.y > 0 && board[Position.x + 1, Position.y - 1] != null
                    && !board[Position.x + 1, Position.y - 1].isWhite)
                    this.canMoveList.Add(new Position(Position.x + 1, Position.y - 1));
                if (Position.x == 1 && board[Position.x + 1, Position.y] == null
                    && board[Position.x + 2, Position.y] == null)
                    this.canMoveList.Add(new Position(Position.x + 2, Position.y));
            }
            else
            {
                if (this.Position.x > 0 && board[this.Position.x - 1, this.Position.y] == null)
                    this.canMoveList.Add(new Position(Position.x - 1, Position.y));
                if (Position.x > 0 && Position.y < 7 && board[Position.x - 1, Position.y + 1] != null
                    && !board[Position.x - 1, Position.y + 1].isWhite)
                    this.canMoveList.Add(new Position(Position.x + 1, Position.y + 1));
                if (Position.x > 0 && Position.y > 0 && board[Position.x - 1, Position.y - 1] != null
                    && !board[Position.x - 1, Position.y - 1].isWhite)
                    this.canMoveList.Add(new Position(Position.x + 1, Position.y - 1));
                if (Position.x == 6 && board[Position.x - 1, Position.y] == null
                   && board[Position.x - 2, Position.y] == null)
                    this.canMoveList.Add(new Position(Position.x - 2, Position.y));
            }
            return this.canMoveList;
        }

        public override string toString()
        {
            return (this.isWhite ? "W" : "B") + "P" + Position.toStringMethod();
        }

    }

    public class Bishop : Piece
    {
        public Bishop(bool isWhite, Position position) : base(isWhite, position)
        {
        }

        public override List<Position> createMoveList(Piece[,] board)
        {
            this.canMoveList = new List<Position>();
            int posX = Position.x;
            int posY = Position.y;
            int n = 1;
            //Up-Right side moving
            while (posX + n < 8 && posY + n < 8)
            {
                if (board[posX + n, posY + n] == null)
                {
                    canMoveList.Add(new Position(posX + n, posY + n));
                    n++;
                    continue;
                }
                if (board[posX + n, posY + n] != null && board[posX + n, posY + n].isWhite == this.isWhite)
                {
                    break;
                }
                else
                {
                    canMoveList.Add(new Position(posX + n, posY + n));
                    break;
                }
            }
            n = 1;
            //Up-Left side moving
            while (posX + n < 8 && posY - n > -1)
            {
                if (board[posX + n, posY - n] == null)
                {
                    canMoveList.Add(new Position(posX + n, posY - n));
                    n++;
                    continue;
                }
                if (board[posX + n, posY - n] != null && board[posX + n, posY - n].isWhite == this.isWhite)
                {
                    break;
                }
                else
                {
                    canMoveList.Add(new Position(posX + n, posY - n));
                    break;
                }
            }
            n = 1;
            //Down-Right side moving
            while (posX - n > -1 && posY + n < 8)
            {
                if (board[posX - n, posY + n] == null)
                {
                    canMoveList.Add(new Position(posX - n, posY + n));
                    n++;
                    continue;
                }
                if (board[posX - n, posY + n] != null && board[posX - n, posY + n].isWhite == this.isWhite)
                {
                    break;
                }
                else
                {
                    canMoveList.Add(new Position(posX - n, posY + n));
                    break;
                }
            }
            n = 1;
            //Down-Left side moving
            while (posX - n > -1 && posY - n > -1)
            {
                if (board[posX - n, posY - n] == null)
                {
                    canMoveList.Add(new Position(posX - n, posY - n));
                    n++;
                    continue;
                }
                if (board[posX - n, posY - n] != null && board[posX - n, posY - n].isWhite == this.isWhite)
                {
                    break;
                }
                else
                {
                    canMoveList.Add(new Position(posX - n, posY - n));
                    break;
                }
            }
            return this.canMoveList;

        }

        public override string toString()
        {
            return (this.isWhite ? "W" : "B") + "B" + Position.toStringMethod();
        }
    }

    public class Rook : Piece
    {
        public Rook(bool isWhite, Position position) : base(isWhite, position)
        {
        }

        public override List<Position> createMoveList(Piece[,] board)
        {
            this.canMoveList = new List<Position>();
            //Right Add if null; break if samecolour; add if enemy break
            int posX = Position.x;
            int posY = Position.y;
            while (posY < 7)
            {
                if (board[posX, posY + 1] == null)
                {
                    canMoveList.Add(new Position(posX, posY + 1));
                    posY++;
                    continue;
                }
                if (board[posX, posY + 1] != null && board[posX, posY + 1].isWhite == this.isWhite)
                {
                    break;
                }
                else
                {
                    canMoveList.Add(new Position(posX, posY + 1));
                    break;
                }
            }
            posX = Position.x;
            posY = Position.y;

            while (posY > 0)
            {
                if (board[posX, posY - 1] == null)
                {
                    canMoveList.Add(new Position(posX, posY - 1));
                    posY--;
                    continue;
                }
                if (board[posX, posY - 1] != null && board[posX, posY - 1].isWhite == this.isWhite)
                {
                    break;
                }
                else
                {
                    canMoveList.Add(new Position(posX, posY - 1));
                    break;
                }
            }
            posX = Position.x;
            posY = Position.y;
            while (posX < 7)
            {
                if (board[posX + 1, posY] == null)
                {
                    canMoveList.Add(new Position(posX + 1, posY));
                    posX++;
                    continue;
                }
                if (board[posX + 1, posY] != null && board[posX + 1, posY].isWhite == this.isWhite)
                {
                    break;
                }
                else
                {
                    canMoveList.Add(new Position(posX + 1, posY));
                    break;
                }
            }
            posX = Position.x;
            posY = Position.y;

            while (posX > 0)
            {
                if (board[posX - 1, posY] == null)
                {
                    canMoveList.Add(new Position(posX - 1, posY));
                    posX--;
                    continue;
                }
                if (board[posX - 1, posY] != null && board[posX - 1, posY].isWhite == this.isWhite)
                {
                    break;
                }
                else
                {
                    canMoveList.Add(new Position(posX - 1, posY));
                    break;
                }
            }



            return this.canMoveList;
        }

        public override string toString()
        {
            return (this.isWhite ? "W" : "B") + "R" + Position.toStringMethod();
        }
    }

    public class Queen : Piece
    {
        public Queen(bool isWhite, Position position) : base(isWhite, position)
        {
        }

        public override List<Position> createMoveList(Piece[,] board)
        {
            this.canMoveList = new List<Position>();
            int posX = Position.x;
            int posY = Position.y;
            while (posY < 7)
            {
                if (board[posX, posY + 1] == null)
                {
                    canMoveList.Add(new Position(posX, posY + 1));
                    posY++;
                    continue;
                }
                if (board[posX, posY + 1] != null && board[posX, posY + 1].isWhite == this.isWhite)
                {
                    break;
                }
                else
                {
                    canMoveList.Add(new Position(posX, posY + 1));
                    break;
                }
            }
            posX = Position.x;
            posY = Position.y;

            while (posY > 0)
            {
                if (board[posX, posY - 1] == null)
                {
                    canMoveList.Add(new Position(posX, posY - 1));
                    posY--;
                    continue;
                }
                if (board[posX, posY - 1] != null && board[posX, posY - 1].isWhite == this.isWhite)
                {
                    break;
                }
                else
                {
                    canMoveList.Add(new Position(posX, posY - 1));
                    break;
                }
            }
            posX = Position.x;
            posY = Position.y;
            while (posX < 7)
            {
                if (board[posX + 1, posY] == null)
                {
                    canMoveList.Add(new Position(posX + 1, posY));
                    posX++;
                    continue;
                }
                if (board[posX + 1, posY] != null && board[posX + 1, posY].isWhite == this.isWhite)
                {
                    break;
                }
                else
                {
                    canMoveList.Add(new Position(posX + 1, posY));
                    break;
                }
            }
            posX = Position.x;
            posY = Position.y;

            while (posX > 0)
            {
                if (board[posX - 1, posY] == null)
                {
                    canMoveList.Add(new Position(posX - 1, posY));
                    posX--;
                    continue;
                }
                if (board[posX - 1, posY] != null && board[posX - 1, posY].isWhite == this.isWhite)
                {
                    break;
                }
                else
                {
                    canMoveList.Add(new Position(posX - 1, posY));
                    break;
                }
            }
            posX = Position.x;
            posY = Position.y;
            int n = 1;
            //Up-Right side moving
            while (posX + n < 8 && posY + n < 8)
            {
                if (board[posX + n, posY + n] == null)
                {
                    canMoveList.Add(new Position(posX + n, posY + n));
                    n++;
                    continue;
                }
                if (board[posX + n, posY + n] != null && board[posX + n, posY + n].isWhite == this.isWhite)
                {
                    break;
                }
                else
                {
                    canMoveList.Add(new Position(posX + n, posY + n));
                    break;
                }
            }
            n = 1;
            //Up-Left side moving
            while (posX + n < 8 && posY - n > -1)
            {
                if (board[posX + n, posY - n] == null)
                {
                    canMoveList.Add(new Position(posX + n, posY - n));
                    n++;
                    continue;
                }
                if (board[posX + n, posY - n] != null && board[posX + n, posY - n].isWhite == this.isWhite)
                {
                    break;
                }
                else
                {
                    canMoveList.Add(new Position(posX + n, posY - n));
                    break;
                }
            }
            n = 1;
            //Down-Right side moving
            while (posX - n > -1 && posY + n < 8)
            {
                if (board[posX - n, posY + n] == null)
                {
                    canMoveList.Add(new Position(posX - n, posY + n));
                    n++;
                    continue;
                }
                if (board[posX - n, posY + n] != null && board[posX - n, posY + n].isWhite == this.isWhite)
                {
                    break;
                }
                else
                {
                    canMoveList.Add(new Position(posX - n, posY + n));
                    break;
                }
            }
            n = 1;
            //Down-Left side moving
            while (posX - n > -1 && posY - n > -1)
            {
                if (board[posX - n, posY - n] == null)
                {
                    canMoveList.Add(new Position(posX - n, posY - n));
                    n++;
                    continue;
                }
                if (board[posX - n, posY - n] != null && board[posX - n, posY - n].isWhite == this.isWhite)
                {
                    break;
                }
                else
                {
                    canMoveList.Add(new Position(posX - n, posY - n));
                    break;
                }
            }
            return this.canMoveList;

        }

        public override string toString()
        {
            return (this.isWhite ? "W" : "B") + "Q" + Position.toStringMethod();
        }
    }

    public class Horse : Piece
    {
        public Horse(bool isWhite, Position position) : base(isWhite, position)
        {
        }

        public override List<Position> createMoveList(Piece[,] board)
        {
            this.canMoveList = new List<Position>();
            //Upper side
            if (Position.x + 1 < 8)
            {
                //Left-Side
                if (Position.y - 2 > -1 && (board[Position.x + 1, Position.y - 2] == null || board[Position.x + 1, Position.y - 2].isWhite != isWhite))
                {
                    canMoveList.Add(new Position(Position.x + 1, Position.y - 2));
                }
                //Right-Side
                if (Position.y + 2 < 8 && (board[Position.x + 1, Position.y + 2] == null || board[Position.x + 1, Position.y + 2].isWhite != isWhite))
                {
                    canMoveList.Add(new Position(Position.x + 1, Position.y + 2));
                }
                //Far Upper
                if (Position.x + 2 < 8)
                {
                    //Left Side
                    if (Position.y - 1 > -1 && (board[Position.x + 2, Position.y - 1] == null || board[Position.x + 2, Position.y - 1].isWhite != isWhite))
                    {
                        canMoveList.Add(new Position(Position.x + 2, Position.y - 1));
                    }
                    //Right Side
                    if (Position.y + 1 < 8 && (board[Position.x + 2, Position.y + 1] == null || board[Position.x + 2, Position.y + 1].isWhite != isWhite))
                    {
                        canMoveList.Add(new Position(Position.x + 2, Position.y + 1));
                    }
                }
            }
            //Lower side
            if (Position.x - 1 > -1)
            {
                //Left-Side
                if (Position.y - 2 > -1 && (board[Position.x - 1, Position.y - 2] == null || board[Position.x - 1, Position.y - 2].isWhite != isWhite))
                {
                    canMoveList.Add(new Position(Position.x - 1, Position.y - 2));
                }
                //Right-Side
                if (Position.y + 2 < 8 && (board[Position.x - 1, Position.y + 2] == null || board[Position.x - 1, Position.y + 2].isWhite != isWhite))
                {
                    canMoveList.Add(new Position(Position.x - 1, Position.y + 2));
                }
                //Far Lower
                if (Position.x - 2 > -1)
                {
                    //Left Side
                    if (Position.y - 1 > -1 && (board[Position.x - 2, Position.y - 1] == null || board[Position.x - 2, Position.y - 1].isWhite != isWhite))
                    {
                        canMoveList.Add(new Position(Position.x - 2, Position.y - 1));
                    }
                    //Right Side
                    if (Position.y + 1 < 8 && (board[Position.x - 2, Position.y + 1] == null || board[Position.x - 2, Position.y + 1].isWhite != isWhite))
                    {
                        canMoveList.Add(new Position(Position.x - 2, Position.y + 1));
                    }
                }
            }


            return this.canMoveList;
        }

        public override string toString()
        {
            return (this.isWhite ? "W" : "B") + "H" + Position.toStringMethod();
        }
    }

    public class King : Piece
    {
        public King(bool isWhite, Position position) : base(isWhite, position)
        {
        }

        public override List<Position> createMoveList(Piece[,] board)
        {
            canMoveList = new List<Position>();
            //Lower Row
            if (Position.x - 1 > -1)
            {
                if (Position.y - 1 > -1 && board[Position.x - 1, Position.y - 1] == null)
                    this.canMoveList.Add(new Position(Position.x - 1, Position.y - 1));
                if (board[Position.x - 1, Position.y] == null)
                    this.canMoveList.Add(new Position(Position.x - 1, Position.y));
                if (Position.y + 1 < 8 && board[Position.x - 1, Position.y + 1] == null)
                    this.canMoveList.Add(new Position(Position.x - 1, Position.y - 1));
            }
            //His Own Row
            if (Position.y - 1 > -1 && board[Position.x, Position.y - 1] == null)
                this.canMoveList.Add(new Position(Position.x, Position.y - 1));
            if (Position.y + 1 < 8 && board[Position.x, Position.y + 1] == null)
                this.canMoveList.Add(new Position(Position.x, Position.y + 1));
            //Upper Row
            if (Position.x + 1 < 8)
            {
                if (Position.y - 1 > -1 && board[Position.x + 1, Position.y - 1] == null)
                    this.canMoveList.Add(new Position(Position.x + 1, Position.y - 1));
                if (board[Position.x + 1, Position.y] == null)
                    this.canMoveList.Add(new Position(Position.x + 1, Position.y));
                if (Position.y + 1 < 8 && board[Position.x + 1, Position.y + 1] == null)
                    this.canMoveList.Add(new Position(Position.x + 1, Position.y - 1));
            }
            return this.canMoveList;
        }

        public override string toString()
        {
            return (this.isWhite ? "W" : "B") + "K" + Position.toStringMethod();
        }
    }



    public abstract class Piece
    {
        public List<Position> canMoveList = new List<Position>();
        public Position Position { get; set; }
        public bool isWhite { get; set; }
        public Piece(bool isWhite, Position position)
        {
            Position = position;
            this.isWhite = isWhite;
        }

        public bool MoveTo(Position newPostition, Piece[,] board)
        {
            if (this.canMoveList.Contains(newPostition))
            {
                board[newPostition.x, newPostition.y] = this;
                board[Position.x, Position.y] = null;
                this.Position = newPostition;
                return true;
            }
            return false;
        }

        public abstract string toString();

        public static Piece fromString(String inputString)
        {
            //WP67  <-- White Pawn 6,7
            try
            {
                bool isWhite = inputString[0] == 'W';
                Position position = new Position(inputString[2], inputString[3]);
                switch (inputString[1])
                {
                    case 'P':
                        return new Pawn(isWhite, position);
                    case 'R':
                        return new Rook(isWhite, position);
                    case 'B':
                        return new Bishop(isWhite, position);
                    case 'H':
                        return null;
                    case 'Q':
                        return new Queen(isWhite, position);
                    case 'K':
                        return null;
                }
                return null;
            }
            catch
            {
                return null;
            }

        }

        public abstract List<Position> createMoveList(Piece[,] board);
    }

    public class Position
    {
        static string[] letters = { "A", "B", "C", "D", "E", "F", "G", "H" };
        public int x, y;

        public Position(int x, int y)
        {
            this.x = x;
            this.y = y;
        }

        public Position(String inputString)
        {
            this.x = Int32.Parse(inputString.Trim().Split(",")[0].Remove(0, 1));
            this.y = Int32.Parse(inputString.Trim().Split(",")[1].Remove(2, 1));
        }


        public String toStringMethod()
        {
            return (letters[y]).ToString() + (x + 1).ToString();
        }

        public override bool Equals(object? obj)
        {
            return obj is Position position &&
                   x == position.x &&
                   y == position.y;
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(x, y);
        }
    }
}




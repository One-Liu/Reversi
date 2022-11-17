using Godot;
using System;
using System.Collections.Generic;

namespace ReversiFEI.Matches
{
    public class BoardContainer : GridContainer
    {
        public Texture PlayerTexture { get; set; }
        public Texture OpponentTexture { get; set; }
        private Texture ValidTexture;
        private Texture InvalidTexture;
        
        public int PlayerPiece { get; set;}
        public int OpponentPiece { get; set;}
        private readonly int EMPTY_CELL = 0;
        
        private int[,] board;
        
        [Export]
        private int boardSize = 10;
        
        public override void _Ready()
        {
            PlayerTexture = (Texture) ResourceLoader.Load("res://resources/square_set1piece1.png");
            OpponentTexture = (Texture) ResourceLoader.Load("res://resources/square_set1piece2.png");
            ValidTexture = (Texture) ResourceLoader.Load("res://resources/square_valid.png");
            InvalidTexture = (Texture) ResourceLoader.Load("res://resources/square_invalid.png");
            
            Columns = boardSize;
            PlayerPiece = 1; //Change to choose depending on turn order
                             //ej. if player 1 then PlayerPiece = 1
                             //else PlayerPiece = -1

            if(PlayerPiece == 1)
                OpponentPiece = -1;
            else
                OpponentPiece = 1;
            
            board = CreateBoard(boardSize);
            Populate(boardSize);
            GameStartState();
        }
        
        private int[,] CreateBoard(int size)
        {
            int[,] newBoard = new int[size, size];
            
            for(int x = 0; x < size; x++)
            {
                for(int y = 0; y < size; y++)
                {
                    newBoard[x,y] = 0;
                } 
            }
            
            return newBoard;
        }
        
        private void GameStartState()
        {
            if(boardSize % 2 == 0 || boardSize >= 6)
            {
                int coordinate1 = (int) Mathf.Floor(boardSize / 2) - 1;
                int coordinate2 = (int) Mathf.Floor(boardSize / 2);
                board[coordinate1,coordinate1] = 1;
                board[coordinate2,coordinate2] = 1;
                board[coordinate2,coordinate1] = -1;
                board[coordinate1,coordinate2] = -1;
                
                UpdateGameState(board);
            }
            else
            {
                GD.Print("Invalid board size!");
            }
        }
        
        public void ChangeTileState(int xPosition, int yPosition, int newState)
        {
            if(board[xPosition,yPosition] == EMPTY_CELL)
            {
                board[xPosition,yPosition] = newState;
                
                FlipPieces(PlayerPiece,1,0);    //Horizontal search
                FlipPieces(PlayerPiece,-1,0);
                FlipPieces(PlayerPiece,0,1);    //Vertical search
                FlipPieces(PlayerPiece,0,-1);
                FlipPieces(PlayerPiece,1,1);    //Diagonal search
                FlipPieces(PlayerPiece,-1,-1);
                
                FlipPieces(OpponentPiece,1,0);
                FlipPieces(OpponentPiece,-1,0);
                FlipPieces(OpponentPiece,0,1);
                FlipPieces(OpponentPiece,0,-1);
                FlipPieces(OpponentPiece,1,1);
                FlipPieces(OpponentPiece,-1,-1);
                
                UpdateGameState(board);
            }
        }
        
        private void Populate(int boardSize)
        {
            var tile = GD.Load<PackedScene>("res://src/scene/userInterface/Tile.tscn");
            
            for(int x = 0; x < boardSize; x++)
            {
                for(int y = 0; y < boardSize; y++)
                {
                    var tileInstance = tile.Instance() as Tile;
                    tileInstance.XPosition = x;
                    tileInstance.YPosition = y;
                    AddChild(tileInstance);
                }
            }
        }
        
        private bool IsInsideBoard(int coordinate)
        {
            bool insideBoard = false;
            
            if(coordinate < Columns && coordinate >= 0)
                insideBoard = true;
            
            return insideBoard;
        }
        
        private bool IsLegalMove(int xCoordinate, int yCoordinate)
        {
            bool legalMove = false;
            
            if(IsInsideBoard(xCoordinate + 1))
            {
                if(board[xCoordinate + 1, yCoordinate] == PlayerPiece)
                    legalMove = true;
            }
            
            if(IsInsideBoard(xCoordinate - 1))
            {
                if(board[xCoordinate - 1, yCoordinate] == PlayerPiece)
                    legalMove = true;
            }
            
            if(IsInsideBoard(yCoordinate + 1))
            {
                if(board[xCoordinate, yCoordinate + 1] == PlayerPiece)
                    legalMove = true;
            }
            
            if(IsInsideBoard(yCoordinate - 1))
            {
                if(board[xCoordinate, yCoordinate - 1] == PlayerPiece)
                    legalMove = true;
            }

            return legalMove;
        }
        
        private void FlipPieces(int piece, int xDirection, int yDirection)
        {
            int otherPiece;
            
            if(piece == 1)
                otherPiece = -1;
            else
                otherPiece = 1;
            
            var tilesToFlip = new List<(int, int)>();
            var tileBuffer = new List<(int, int)>();
            
            for(int x = 0; x < boardSize; x++)
            {
                for(int y = 0; y < boardSize; y++)
                {
                    if(board[x,y] == piece)
                    {
                        if(IsInsideBoard(x + xDirection) && IsInsideBoard(y + yDirection))
                        {
                            if(board[x + xDirection, y + yDirection] == otherPiece)
                            {
                                while(board[x + xDirection, y + yDirection] == otherPiece)
                                {   
                                    tileBuffer.Add((x + xDirection, y + yDirection));
                                    xDirection = xDirection + xDirection;
                                    yDirection = yDirection + yDirection;
                                    
                                    if(!IsInsideBoard(x + xDirection) && !IsInsideBoard(y + yDirection))
                                    {
                                        tileBuffer.Clear();
                                        break;
                                    }
                                    
                                    if(board[x + xDirection, y + yDirection] == piece)
                                    {
                                        tilesToFlip.AddRange(tileBuffer);
                                        tileBuffer.Clear();
                                        break;
                                    }
                                }
                            }
                        }
                    }
                }
            }
            
            foreach(var (x,y) in tilesToFlip)
            {
                board[x,y] = piece;
            }
        }
        
        private void UpdateGameState(int[,] board)
        {
            foreach(Tile t in GetChildren())
            {
                for(int x = 0; x < board.GetLength(0); x++)
                {
                    for(int y = 0; y < board.GetLength(1); y++)
                    {
                        if(board[x,y] == EMPTY_CELL)
                        {
                            if(IsLegalMove(x,y))
                            {
                                if(t.XPosition == x && t.YPosition == y)
                                {
                                    t.TextureHover = ValidTexture;
                                    t.Disabled = false;
                                }
                            }
                            else
                            {
                                if(t.XPosition == x && t.YPosition == y)
                                {
                                    t.Disabled = true;
                                }
                            }
                        }
                        else if(board[x,y] == PlayerPiece)
                        {
                            if(t.XPosition == x && t.YPosition == y)
                            {
                                t.TextureDisabled = PlayerTexture;
                                t.Disabled = true;
                            }
                        }
                        else if(board[x,y] == OpponentPiece)
                        {
                                if(t.XPosition.Equals(x) & t.YPosition.Equals(y))
                            {
                                t.TextureDisabled = OpponentTexture;
                                t.Disabled = true;
                            }
                        }
                    }
                }
            }
        }
    }
}

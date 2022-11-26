using Godot;
using System;
using System.Collections.Generic;
using ReversiFEI.Network;
using ReversiFEI.Controller;

namespace ReversiFEI.Matches
{
    public class BoardContainer : GridContainer
    {
        private readonly int EMPTY_CELL = 0;
        
        [Export]
        private int boardSize = 10;
        
        public Texture PlayerTexture { get; set; }
        public Texture OpponentTexture { get; set; }
        private Texture ValidTexture;
        
        public int PlayerPiece { get; set;}
        public int OpponentPiece { get; set;}
        
        private int[,] board;
        private int myPieces = 0;
        private int opponentPieces = 0;
        
        private NetworkUtilities networkUtilities;
        private Controls controls;
        
        public override void _Ready()
        {
            networkUtilities = GetNode("/root/NetworkUtilities") as NetworkUtilities;
            controls = GetNode("/root/Controls") as Controls;
            
            networkUtilities.Connect("PiecePlaced",this,nameof(ReceiveOpponentMove));
            networkUtilities.Connect("OpponentTurnSkipped",this,nameof(OpponentSkippedTurn));
            networkUtilities.Connect("MatchEnded",this,nameof(Results));

            Columns = boardSize;
            PlayerPiece = networkUtilities.MyPiece;

            if(PlayerPiece == 1)
            {
                OpponentPiece = -1;
                PlayerTexture = (Texture) ResourceLoader.Load("res://resources/square_set1piece1.png");
                OpponentTexture = (Texture) ResourceLoader.Load("res://resources/square_set1piece2.png");
            }
            else
            {
                OpponentPiece = 1;
                PlayerTexture = (Texture) ResourceLoader.Load("res://resources/square_set1piece2.png");
                OpponentTexture = (Texture) ResourceLoader.Load("res://resources/square_set1piece1.png");
            }
            ValidTexture = (Texture) ResourceLoader.Load("res://resources/square_valid.png");
            
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
                int coordinate1 = (int) Mathf.Floor((float)boardSize / 2.0F) - 1;
                int coordinate2 = (int) Mathf.Floor((float)boardSize / 2.0F);
                board[coordinate1,coordinate1] = 1;
                board[coordinate2,coordinate2] = 1;
                board[coordinate2,coordinate1] = -1;
                board[coordinate1,coordinate2] = -1;
                
                UpdateGameState(board);
                
                if(!networkUtilities.MyTurn)
                {
                    GD.Print("Not my turn");
                    LockTiles();
                }
                else
                {
                    UnlockTiles();
                }
            }
            else
            {
                GD.Print("Invalid board size!");
            }
        }
        
        private void ChangeTileState(int xPosition, int yPosition, int newState)
        {
            if(board[xPosition,yPosition] == EMPTY_CELL)
            {
                board[xPosition,yPosition] = newState;
            }
        }
        
        public void MakeMove(int xPosition, int yPosition, int newState)
        {
            int opponentId = networkUtilities.OpponentId;
            networkUtilities.SendMove(xPosition,yPosition,newState);
            ChangeTileState(xPosition,yPosition,newState);
            CheckBoard(PlayerPiece);
            UpdateGameState(board);
            LockTiles();
        }
        
        private void ReceiveOpponentMove(int xPosition, int yPosition, int newState)
        {
            ChangeTileState(xPosition,yPosition,newState);
            CheckBoard(OpponentPiece);
            UpdateGameState(board);
            if(MovesAvailable())
            {
                UnlockTiles();
            }
            else
                GD.Print("No moves are available.");
                networkUtilities.SkipTurn(false);
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
            
            try{               
                if(IsInsideBoard(xCoordinate + 1) && board[xCoordinate + 1, yCoordinate] == PlayerPiece)
                {
                    legalMove = true;
                }
                
                if(IsInsideBoard(xCoordinate - 1) && board[xCoordinate - 1, yCoordinate] == PlayerPiece)
                {
                    legalMove = true;
                }
                
                if(IsInsideBoard(yCoordinate + 1) && board[xCoordinate, yCoordinate + 1] == PlayerPiece)
                {
                    legalMove = true;
                }
                
                if(IsInsideBoard(yCoordinate - 1) && board[xCoordinate, yCoordinate - 1] == PlayerPiece)
                {
                    legalMove = true;
                }
            }
            catch(IndexOutOfRangeException)
            {
                legalMove = false;
            }
            
            return legalMove;
        }
        
        private bool MovesAvailable()
        {
            bool availableMoves = false;
            for(int i = 0; i < boardSize; i++)
            {
                for(int j = 0; j < boardSize; j++)
                {
                    if(IsLegalMove(i,j))
                    {
                        if(board[i,j] != PlayerPiece && board[i,j] != OpponentPiece)
                        availableMoves = true;
                    }
                }
            }
            return availableMoves;
        }
        
        private void OpponentSkippedTurn()
        {
            if(MovesAvailable())
                UnlockTiles();
            else
                networkUtilities.SkipTurn(true);
        }
        
        private void LockTiles()
        {
            foreach(Tile t in GetChildren())
            {
                t.Disabled = true;
            }
        }
        
        private void UnlockTiles()
        {
            foreach(Tile t in GetChildren())
            {
                if(IsLegalMove(t.XPosition,t.YPosition) 
                && board[t.XPosition,t.YPosition] == EMPTY_CELL)
                {
                    t.Disabled = false;
                    t.TextureHover = ValidTexture;
                }
            }
        }
        
        private void CheckBoard(int piece)
        {
            ClockwiseScan(piece);
            CounterclockwiseScan(piece);
        }
        
        private void ClockwiseScan(int piece)
        {
            FlipPieces(piece,1,0);
            FlipPieces(piece,1,1);
            FlipPieces(piece,0,1);
            FlipPieces(piece,-1,1);
            FlipPieces(piece,-1,0);
            FlipPieces(piece,-1,-1);
            FlipPieces(piece,0,-1);
            FlipPieces(piece,1,-1);
            FlipPieces(piece,0,-1);
        }
        
        private void CounterclockwiseScan(int piece)
        {
            FlipPieces(piece,0,-1);
            FlipPieces(piece,-1,-1);
            FlipPieces(piece,-1,0);
            FlipPieces(piece,-1,1);
            FlipPieces(piece,0,1);
            FlipPieces(piece,1,1);
            FlipPieces(piece,1,0);
        }
        
        private void FlipPieces(int piece, int xDirection, int yDirection)
        {
            int otherPiece;
            
            if(piece == PlayerPiece)
                otherPiece = OpponentPiece;
            else
                otherPiece = PlayerPiece;
            
            var tilesToFlip = new List<(int, int)>();
            var tileBuffer = new List<(int, int)>();
            
            for(int x = 0; x < boardSize; x++)
            {
                for(int y = 0; y < boardSize; y++)
                {
                    if(board[x,y] == piece &&
                        IsInsideBoard(x + xDirection) && IsInsideBoard(y + yDirection) 
                        && board[x + xDirection, y + yDirection] == otherPiece)
                    {
                        while(board[x + xDirection, y + yDirection] == otherPiece)
                        {
                            tileBuffer.Add((x + xDirection, y + yDirection));
                            xDirection = xDirection + xDirection;
                            yDirection = yDirection + yDirection;
                            
                            try{
                                if(board[x + xDirection, y + yDirection] == EMPTY_CELL)
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
                            catch(IndexOutOfRangeException)
                            {
                                tileBuffer.Clear();
                                break;
                            }
                        }
                    }
                }
                
                myPieces = CountPieces(PlayerPiece);
                opponentPieces = CountPieces(OpponentPiece);
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
                        if(board[x,y] == PlayerPiece && t.XPosition == x && t.YPosition == y)
                        {
                            t.TextureDisabled = PlayerTexture;
                            t.Disabled = true;
                        }
                        else if(board[x,y] == OpponentPiece && t.XPosition.Equals(x) && t.YPosition.Equals(y))
                        {
                            t.TextureDisabled = OpponentTexture;
                            t.Disabled = true;
                        }
                    }
                }
            }
        }
        
        private int CountPieces(int piece)
        {
            int count = 0;
            
            for(int x = 0; x < board.GetLength(0); x++)
            {
                for(int y = 0; y < board.GetLength(1); y++)
                {
                    if(board[x,y] == piece)
                        count++;
                }
            }
            
            return count;
        }
        
        private void Results()
        {
            if(myPieces > opponentPieces)
                GD.Print("You won!");
            else if(myPieces < opponentPieces)
                GD.Print("You lost...");
            else
                GD.Print("Tie!?");
                
            controls.GoToLobby();
        }
    }
}

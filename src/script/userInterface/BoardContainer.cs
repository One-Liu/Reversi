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
        
        public int[,] board;
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
            GetTree().Connect("network_peer_disconnected", this, nameof(OpponentDisconnected));

            Columns = boardSize;
            PlayerPiece = networkUtilities.MyPiece;
            if(PlayerPiece == 1)
                OpponentPiece = -1;
            else
                OpponentPiece = 1;
            
            ValidTexture = (Texture) ResourceLoader.Load("res://resources/square_valid.png");
            
            SetPlayerSprites();
            
            board = CreateBoard(boardSize);
            Populate(boardSize);
            GameStartState();
        }
        
        public int[,] CreateBoard(int size)
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
        
        private void SetPlayerSprites()
        {
            if(PlayerPiece == 1)
            {
                switch(networkUtilities.PlayerSet)
                {
                    case 1:
                        PlayerTexture = (Texture) ResourceLoader.Load("res://resources/square_set1piece1.png");
                        break;
                    case 2:
                        PlayerTexture = (Texture) ResourceLoader.Load("res://resources/square_set2piece1.png");
                        break;
                    case 3:
                        PlayerTexture = (Texture) ResourceLoader.Load("res://resources/square_set3piece1.png");
                        break;
                    case 4:
                        PlayerTexture = (Texture) ResourceLoader.Load("res://resources/square_set4piece1.png");
                        break;
                    default:
                        PlayerTexture = (Texture) ResourceLoader.Load("res://resources/square_set1piece1.png");
                        break;
                }
                
                switch(networkUtilities.OpponentSet)
                {
                    case 1:
                        OpponentTexture = (Texture) ResourceLoader.Load("res://resources/square_set1piece2.png");
                        break;
                    case 2:
                        OpponentTexture = (Texture) ResourceLoader.Load("res://resources/square_set2piece2.png");
                        break;
                    case 3:
                        OpponentTexture = (Texture) ResourceLoader.Load("res://resources/square_set3piece2.png");
                        break;
                    case 4:
                        OpponentTexture = (Texture) ResourceLoader.Load("res://resources/square_set4piece2.png");
                        break;
                    default:
                        OpponentTexture = (Texture) ResourceLoader.Load("res://resources/square_set1piece2.png");
                        break;
                }
            }
            else
            {
                switch(networkUtilities.PlayerSet)
                {
                    case 1:
                        PlayerTexture = (Texture) ResourceLoader.Load("res://resources/square_set1piece2.png");
                        break;
                    case 2:
                        PlayerTexture = (Texture) ResourceLoader.Load("res://resources/square_set2piece2.png");
                        break;
                    case 3:
                        PlayerTexture = (Texture) ResourceLoader.Load("res://resources/square_set3piece2.png");
                        break;
                    case 4:
                        PlayerTexture = (Texture) ResourceLoader.Load("res://resources/square_set4piece2.png");
                        break;
                    default:
                        PlayerTexture = (Texture) ResourceLoader.Load("res://resources/square_set1piece2.png");
                        break;
                }
                
                switch(networkUtilities.OpponentSet)
                {
                    case 1:
                        OpponentTexture = (Texture) ResourceLoader.Load("res://resources/square_set1piece1.png");
                        break;
                    case 2:
                        OpponentTexture = (Texture) ResourceLoader.Load("res://resources/square_set2piece1.png");
                        break;
                    case 3:
                        OpponentTexture = (Texture) ResourceLoader.Load("res://resources/square_set3piece1.png");
                        break;
                    case 4:
                        OpponentTexture = (Texture) ResourceLoader.Load("res://resources/square_set4piece1.png");
                        break;
                    default:
                        OpponentTexture = (Texture) ResourceLoader.Load("res://resources/square_set1piece1.png");
                        break;
                }
            }
        }
        
        public void GameStartState()
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
                
                if(networkUtilities.MyTurn)
                {
                    UnlockTiles();
                }
                else
                {
                    LockTiles();
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
            CheckBoard(newState, xPosition, yPosition);
            UpdateGameState(board);
            
            var match = GetParent() as Match;
            match.SetScores(myPieces, opponentPieces);
        }
        
        public void MakeMove(int xPosition, int yPosition, int newState)
        {
            networkUtilities.SendMove(xPosition,yPosition,newState);
            ChangeTileState(xPosition,yPosition,newState);
            LockTiles();
        }
        
        private void ReceiveOpponentMove(int xPosition, int yPosition, int newState)
        {
            ChangeTileState(xPosition,yPosition,newState);
            if(MovesAvailable())
            {
                UnlockTiles();
            }
            else
            {
                GD.Print("No moves are available.");
                networkUtilities.SkipTurn(false);
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
        
        public bool IsInsideBoard(int coordinate)
        {
            bool insideBoard = false;
            
            if(coordinate < Columns && coordinate >= 0)
                insideBoard = true;
            
            return insideBoard;
        }
        
        public bool IsLegalMove(int xCoordinate, int yCoordinate)
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
        
        public bool MovesAvailable()
        {
            bool availableMoves = false;
            foreach(Tile t in GetChildren())
            {
                if(IsLegalMove(t.XPosition,t.YPosition) 
                && board[t.XPosition,t.YPosition] == EMPTY_CELL)
                {
                    availableMoves = true;
                    break;
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
                t.TextureHover = null;
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
        
        private void CheckBoard(int piece, int row, int col)
        {
            for(int i = -1; i <= 1; i++)
            {
                for(int j = -1; j <= 1; j++)
                {
                    FlipPieces(piece, row, col, i, j);
                }
            }
        }
        
        private void FlipPieces(int piece, int row, int col, int xDirection, int yDirection)
        {
            int otherPiece;
            
            if(piece == PlayerPiece)
                otherPiece = OpponentPiece;
            else
                otherPiece = PlayerPiece;
            
            var tilesToFlip = new List<(int, int)>();
            
            int rowToCheck = row + xDirection;
            int colToCheck = col + yDirection;
            
            while(rowToCheck >= 0 && rowToCheck < board.GetLength(0) && colToCheck >= 0 &&
            colToCheck < board.GetLength(1) && board[rowToCheck, colToCheck] == otherPiece)
            {
                tilesToFlip.Add((rowToCheck,colToCheck));
                rowToCheck += xDirection;
                colToCheck += yDirection;
            }
            
            if (rowToCheck < 0 || rowToCheck > board.GetLength(0) - 1 || colToCheck < 0 ||
                colToCheck > board.GetLength(1) - 1 || (rowToCheck - xDirection == row && colToCheck - yDirection == col) ||
                board[rowToCheck, colToCheck] != piece)
            {
                tilesToFlip.Clear();
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
            
            myPieces = CountPieces(PlayerPiece);
            opponentPieces = CountPieces(OpponentPiece);
        }
        
        public int CountPieces(int piece)
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
        
        private void OpponentDisconnected(int peerId)
        {
            if(peerId == networkUtilities.OpponentId)
            {
                RegisterVictory(true);
            }
        }
        
        private void Results()
        {
            myPieces = CountPieces(PlayerPiece);
            opponentPieces = CountPieces(OpponentPiece);
            
            if(myPieces > opponentPieces)
            {
                GD.Print("You won!");
                RegisterVictory(true);
                return;
            }
            else if(myPieces < opponentPieces)
            {
                GD.Print("You lost...");
            }
            else
                GD.Print("Tie!?");
            
            RegisterVictory(false);
        }
        
        private void RegisterVictory(bool won)
        {
            if(won)
                networkUtilities.RequestVictoryRegistration();
            
            controls.GoToLobby();
        }
    }
}

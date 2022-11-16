using Godot;
using System;

public class Match : Control
{
    [Export]
    private int boardSize = 10;
    private int[,] board;
    private BoardContainer _BoardContainer;
    public int PlayerPiece { get; set;}

    public override void _Ready()
    {
        CreateBoard();
        
        PlayerPiece = 1; //Change to choose depending on turn order
                         //ej. if player 1 then PlayerPiece = 1
                         //else PlayerPiece = -1
        
        _BoardContainer = GetNode<BoardContainer>("BoardContainer");
        _BoardContainer.Columns = boardSize;
        _BoardContainer.Populate(boardSize);
        
        GameStartState();
    }

    public void CreateBoard()
    {
        board = new int[boardSize, boardSize];
    }

    public void GameStartState()
    {
        if(boardSize % 2 == 0 | boardSize >= 6)
        {
            int coordinate1 = (int) Mathf.Floor(boardSize / 2) - 1;
            int coordinate2 = (int) Mathf.Floor(boardSize / 2);
            board[coordinate1,coordinate1] = 1;
            board[coordinate2,coordinate2] = 1;
            board[coordinate2,coordinate1] = -1;
            board[coordinate1,coordinate2] = -1;
            
            _BoardContainer.UpdateGameState(board);
        }
        else
        {
            GD.Print("Invalid board size!");
        }
    }
    
    public void ChangeTileState(int xPosition, int yPosition, int newState)
    {
        board[xPosition,yPosition] = newState;
        _BoardContainer.UpdateGameState(board);
    }
}

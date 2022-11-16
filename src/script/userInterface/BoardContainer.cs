using Godot;
using System;

public class BoardContainer : GridContainer
{
    public Texture playerTexture { get; set; }
    public Texture opponentTexture { get; set; }
    
    public void Populate(int boardSize)
    {
        var tile = GD.Load<PackedScene>("res://src/scene/userInterface/Tile.tscn");
        
        for(int i = 0; i < boardSize; i++)
        {
            for(int j = 0; j < boardSize; j++)
            {
                Tile tileInstance = (Tile) tile.Instance();
                tileInstance.XPosition = i;
                tileInstance.YPosition = j;
                AddChild(tileInstance);
            }
        }
    }
    
    public void UpdateGameState(int[,] board)
    {
        foreach(Tile t in GetChildren())
        {
            for(int i = 0; i < board.GetLength(0); i++)
            {
                for(int j = 0; j < board.GetLength(1); j++)
                {
                    switch(board[i,j])
                    {
                        case 1:
                            if(t.XPosition.Equals(i) & t.YPosition.Equals(j))
                            {
                                t.TextureNormal = playerTexture;
                            }
                            break;
                        case -1:
                            if(t.XPosition.Equals(i) & t.YPosition.Equals(j))
                            {
                                t.TextureNormal = opponentTexture;
                            }
                            break;
                        case 0:
                            break;
                        default:
                            GD.Print($"Undefined state behaviour at coordinates {i},{j}");
                    }
                }
            }
        }
    }
}

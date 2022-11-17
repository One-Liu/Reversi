using Godot;
using System;

namespace ReversiFEI.Matches
{
    public class BoardContainer : GridContainer
    {
        public Texture PlayerTexture { get; set; }
        public Texture OpponentTexture { get; set; }
        
        
        public void Populate(int boardSize)
        {
            var tile = GD.Load<PackedScene>("res://src/scene/userInterface/Tile.tscn");
            
            for(int i = 0; i < boardSize; i++)
            {
                for(int j = 0; j < boardSize; j++)
                {
                    var tileInstance = tile.Instance() as Tile;
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
                                    t.TextureNormal = PlayerTexture;
                                }
                                break;
                            case -1:
                                if(t.XPosition.Equals(i) & t.YPosition.Equals(j))
                                {
                                    t.TextureNormal = OpponentTexture;
                                }
                                break;
                            case 0:
                                break;
                            default:
                                GD.Print($"Undefined state behaviour at coordinates {i},{j}");
                                break;
                        }
                    }
                }
            }
        }
    }
}

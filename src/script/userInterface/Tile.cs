using Godot;
using System;

public class Tile : TextureButton
{
    public int XPosition { get; set; }
    public int YPosition { get; set; }
    private Match _Match;
    
    public override void _Ready()
    {
        _Match = GetNode<Match>("/root/Match");
    }

    private void _on_Tile_pressed()
    {
        _Match.ChangeTileState(XPosition,YPosition,_Match.PlayerPiece);
    }
}

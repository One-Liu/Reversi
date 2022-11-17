using Godot;
using System;

namespace ReversiFEI.Matches
{
    public class Tile : TextureButton
    {
        public int XPosition { get; set; }
        public int YPosition { get; set; }
        private BoardContainer _BoardContainer;
        
        public override void _Ready()
        {
            _BoardContainer = GetNode<BoardContainer>("/root/Match/BoardContainer");
        }

        private void _on_Tile_pressed()
        {
            _BoardContainer.ChangeTileState(XPosition,YPosition,_BoardContainer.PlayerPiece);
        }
    }
}

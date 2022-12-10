using Godot;
using System;

namespace ReversiFEI.Matches
{
    public class Tile : TextureButton
    {
        public int XPosition { get; set; }
        public int YPosition { get; set; }
        private BoardContainer boardContainer;
        
        public override void _Ready()
        {
            boardContainer = GetNode<BoardContainer>("/root/Match/BoardContainer");
        }

        private void _on_Tile_pressed()
        {
            boardContainer.MakeMove(XPosition,YPosition,boardContainer.PlayerPiece);
        }
    }
}

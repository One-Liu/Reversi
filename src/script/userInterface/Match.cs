using Godot;
using System;

namespace ReversiFEI.Matches
{
    public class Match : Control
    {
        private BoardContainer _BoardContainer;

        public override void _Ready()
        {
            _BoardContainer.PlayerTexture = (Texture) ResourceLoader.Load("res://resources/square_set1piece1.png");
            _BoardContainer.OpponentTexture = (Texture) ResourceLoader.Load("res://resources/square_set1piece2.png");
        }
    }
}

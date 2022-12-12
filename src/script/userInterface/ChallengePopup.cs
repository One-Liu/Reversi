using Godot;
using System;

namespace ReversiFEI.Controller
{
    public class ChallengePopup : Popup
    {
        private Button kickButton;
        
        public override void _Ready()
        {
            kickButton = GetNode<Button>("KickButton");
            if(OS.HasFeature("Server"))
            {
                kickButton.Disabled = false;
            }
        }
    }
}

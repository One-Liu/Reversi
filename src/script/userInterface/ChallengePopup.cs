using Godot;
using System;
using ReversiFEI.Network;

namespace ReversiFEI.Controller
{
    public class ChallengePopup : Popup
    {
        private NetworkUtilities networkUtilities;
        private Button kickButton;
        private Button addFriendButton;
        
        
        public override void _Ready()
        {
            networkUtilities = GetNode<NetworkUtilities>("/root/NetworkUtilities");
            kickButton = GetNode<Button>("KickButton");
            addFriendButton = GetNode<Button>("AddFriendButton");
            
            if(OS.HasFeature("Server"))
            {
                kickButton.Disabled = false;
            }
            
            if(networkUtilities.IsGuest)
                addFriendButton.Disabled = true;
        }
    }

}

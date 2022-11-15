using Godot;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ReversiFEI.Network
{
    public class FriendsList : Control
    {

        private NetworkUtilities networkUtilities;

        public override void _Ready()
        {
            networkUtilities = GetNode("/root/NetworkUtilities") as NetworkUtilities;
            SetFriends();
        }
        
        private async Task SetFriends()
        {
            var friendsList = GetNode<ItemList>("FriendsList");
            friendsList.Clear();
                
            networkUtilities.JoinGame();
            await ToSignal(GetTree(), "connected_to_server");
            
            if(GetTree().NetworkPeer == null)
            {
                GD.Print("Sign up failed.");    
            }
            else
            {
                networkUtilities.UpdateFriends();
                networkUtilities.LeaveGame();
                foreach(string friend in networkUtilities.Friends)
                {
                    if(friend != null)
                        friendsList.AddItem(friend);
                }
                
                friendsList.SortItemsByText();
            }
        }
    }
}

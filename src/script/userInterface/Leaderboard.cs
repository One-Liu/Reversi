using Godot;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace ReversiFEI.Network
{
    public class Leaderboard : Control
    {
        private NetworkUtilities networkUtilities;

        public override void _Ready()
        {
            networkUtilities = GetNode("/root/NetworkUtilities") as NetworkUtilities;
            SetLeaderboard();
        }
        
        private async Task SetLeaderboard()
        {
            var leaderboard = GetNode<ItemList>("TopPlayersItemList");
            leaderboard.Clear();
                
            networkUtilities.JoinGame();
            await ToSignal(GetTree(), "connected_to_server");
            
            if(GetTree().NetworkPeer == null)
            {
                GD.Print("Log in failed.");    
            }
            else
            {
                networkUtilities.UpdateLeaderboard();
                networkUtilities.LeaveGame();
                
                int position = 0; 
                
                foreach(string player in networkUtilities.Leaderboard)
                {
                    if(player != null)
                    {
                        position += 1;
                        leaderboard.AddItem($"{position}) {player}");
                    }
                }
            }
        }
    }
} 

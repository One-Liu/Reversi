using Godot;
using System;
using ReversiFEI.Network;

namespace ReversiFEI.Matches
{
    public class Match : Control
    {
        private NetworkUtilities networkUtilities;
        private Label playerNickname;
        private Sprite playerAvatar;
        private Label playerTotalPoints;
        private Label opponentNickname;
        private Sprite opponentAvatar;
        private Label opponentTotalPoints;
        
        public override void _Ready()
        {
            networkUtilities = GetNode("/root/NetworkUtilities") as NetworkUtilities;
            playerNickname = GetNode<Label>("PlayerHBoxContainer/PlayerVBoxContainer/PlayersNickname");
            playerAvatar = GetNode<Sprite>("PlayerHBoxContainer/PlayerVBoxContainer/PlayerAvatar");
            playerTotalPoints = GetNode<Label>("PlayerHBoxContainer/PlayerTotalPoints");
            opponentNickname = GetNode<Label>("OpponentHBoxContainer/OpponentVBoxContainer/OpponentNickname");
            opponentAvatar = GetNode<Sprite>("OpponentHBoxContainer/OpponentVBoxContainer/OpponentAvatar");
            opponentTotalPoints = GetNode<Label>("OpponentHBoxContainer/OpponentTotalPoints");
            SetPlayersProfile();
        }
        
        private void SetPlayersProfile()
        {
            var avatar1 = (Texture)GD.Load("res://resources/Avatar1.png");
            var avatar2 = (Texture)GD.Load("res://resources/Avatar2.png");
            var avatar3 = (Texture)GD.Load("res://resources/Avatar3.png");
            var avatar4 = (Texture)GD.Load("res://resources/Avatar4.png");
            
            playerNickname.Text = networkUtilities.Playername;
            
            switch(networkUtilities.PlayerAvatar)
            {
                case 1:
                    playerAvatar.Texture = avatar1;
                    break;
                case 2:
                    playerAvatar.Texture = avatar2;
                    break;
                case 3:
                    playerAvatar.Texture = avatar3;
                    break;
                case 4:
                    playerAvatar.Texture = avatar4;
                    break;
            }
            
            opponentNickname.Text = networkUtilities.Players[networkUtilities.OpponentId];
        }
        
        public void SetScores(int playerScore, int opponentScore)
        {
            playerTotalPoints.Text = GD.Var2Str(playerScore);
            opponentTotalPoints.Text = GD.Var2Str(opponentScore);
        }
    }
}

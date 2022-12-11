using Godot;
using System;
using ReversiFEI.Network;

namespace ReversiFEI.Matches
{
    public class Match : Control
    {
        private NetworkUtilities networkUtilities;
        private Label playersNickname;
        private Label playerTotalPoints;
        private Label opponentNickname;
        private Label opponentTotalPoints;
        
        public override void _Ready()
        {
            networkUtilities = GetNode("/root/NetworkUtilities") as NetworkUtilities;
            playersNickname = GetNode<Label>("PlayerHBoxContainer/PlayerVBoxContainer/PlayersNickname");
            playerTotalPoints = GetNode<Label>("PlayerHBoxContainer/PlayerTotalPoints");
            opponentNickname = GetNode<Label>("OpponentHBoxContainer/OpponentVBoxContainer/OpponentNickname");
            opponentTotalPoints = GetNode<Label>("OpponentHBoxContainer/OpponentTotalPoints");
            SetPlayerNames();
            SetScores(2,2);
        }
        
        private void SetPlayerNames()
        {
            playersNickname.Text = networkUtilities.Playername;
            opponentNickname.Text = networkUtilities.Players[networkUtilities.OpponentId];
        }
        
        public void SetScores(int playerScore, int opponentScore)
        {
            playerTotalPoints.Text = GD.Var2Str(playerScore);
            opponentTotalPoints.Text = GD.Var2Str(opponentScore);
        }
    }
}

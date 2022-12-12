using Godot;
using System;
using ReversiFEI.Network;
using ReversiFEI.Controller;

namespace ReversiFEI.Matches
{
    public class Match : Control
    {
        private NetworkUtilities networkUtilities;
        private Controls controls;
        private Label playersNickname;
        private Label playerTotalPoints;
        private Label opponentNickname;
        private Label opponentTotalPoints;
        
        public override void _Ready()
        {
            controls = GetNode("/root/Controls") as Controls;
            networkUtilities = GetNode("/root/NetworkUtilities") as NetworkUtilities;
            playersNickname = GetNode<Label>("PlayerHBoxContainer/PlayerVBoxContainer/PlayersNickname");
            playerTotalPoints = GetNode<Label>("PlayerHBoxContainer/PlayerTotalPoints");
            opponentNickname = GetNode<Label>("OpponentHBoxContainer/OpponentVBoxContainer/OpponentNickname");
            opponentTotalPoints = GetNode<Label>("OpponentHBoxContainer/OpponentTotalPoints");
            SetPlayerNames();
            GD.Print(networkUtilities.soundEnabled);
            PlayMusic(networkUtilities.soundEnabled);
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
        
        public void PlayMusic(bool music)
        {
            if(music==true)
                GetNode<AudioStreamPlayer>("Music").Play(); 
            else
                GetNode<AudioStreamPlayer>("Music").Stop(); 
        }
        
    }
}

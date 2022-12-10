using Godot;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using ReversiFEI.Controller;
using ReversiFEI.Network;

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
            networkUtilities.Connect("MessageReceivedMatch",this,nameof(ReceiveMessages));
            
            networkUtilities = GetNode("/root/NetworkUtilities") as NetworkUtilities;
            playersNickname = GetNode<Label>("PlayerHBoxContainer/PlayerVBoxContainer/PlayersNickname");
            playerTotalPoints = GetNode<Label>("PlayerHBoxContainer/PlayerTotalPoints");
            opponentNickname = GetNode<Label>("OpponentHBoxContainer/OpponentVBoxContainer/OpponentNickname");
            opponentTotalPoints = GetNode<Label>("OpponentHBoxContainer/OpponentTotalPoints");
            SetPlayerNames();
        }
    
    
        public override void _Input(InputEvent inputEvent)
        {
            if (inputEvent.IsActionPressed("lobby_SendMessage"))
            {
                SendMessage();
            }
        }
      
        private void SendMessage()
        {
            var message = GetNode<LineEdit>("Panel/ChatLineEdit").Text;
            networkUtilities.SendMessageMatch(message);
            GetNode<LineEdit>("Panel/ChatLineEdit").Clear();
        }
      
        private void ReceiveMessages()
        {
         GetNode("Panel").GetNode<TextEdit>("Chat").Text += networkUtilities.MessagesMatch.Last();
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

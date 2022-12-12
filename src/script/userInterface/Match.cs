using Godot;
using System;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using ReversiFEI.Controller;
using ReversiFEI.Network;
using ReversiFEI.Controller;

namespace ReversiFEI.Matches
{
    public class Match : Control
    {
        private NetworkUtilities networkUtilities;
        private Controls controls;
        private Label playerNickname;
        private Sprite playerAvatar;

        private Label playerTotalPoints;
        private Label opponentNickname;
        private Sprite opponentAvatar;
        private Label opponentTotalPoints;
        
        public override void _Ready()
        {
            controls = GetNode("/root/Controls") as Controls;
            networkUtilities = GetNode("/root/NetworkUtilities") as NetworkUtilities;
            networkUtilities.Connect("MessageReceivedMatch",this,nameof(ReceiveMessages));
            playerNickname = GetNode<Label>("PlayerHBoxContainer/PlayerVBoxContainer/PlayersNickname");
            playerAvatar = GetNode<Sprite>("PlayerAvatar");
            playerTotalPoints = GetNode<Label>("PlayerHBoxContainer/PlayerTotalPoints");
            opponentNickname = GetNode<Label>("OpponentHBoxContainer/OpponentVBoxContainer/OpponentNickname");
            opponentAvatar = GetNode<Sprite>("OpponentAvatar");
            opponentTotalPoints = GetNode<Label>("OpponentHBoxContainer/OpponentTotalPoints");
            SetPlayersProfile();
            SetScores(2,2);
        }

        public override void _Input(InputEvent inputEvent)
        {
            if (inputEvent.IsActionPressed("EnterSendMessage"))
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
                default:
                    playerAvatar.Texture = avatar1;
                    break;
            }
            
            opponentNickname.Text = networkUtilities.Players[networkUtilities.OpponentId];
            
            switch(networkUtilities.OpponentAvatar)
            {
                case 1:
                    opponentAvatar.Texture = avatar1;
                    break;
                case 2:
                    opponentAvatar.Texture = avatar2;
                    break;
                case 3:
                    opponentAvatar.Texture = avatar3;
                    break;
                case 4:
                    opponentAvatar.Texture = avatar4;
                    break;
                default:
                    opponentAvatar.Texture = avatar1;
                    break;
            }
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


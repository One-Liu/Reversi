using Godot;
using System;
using System.Net.Mail;
using EmailValidation;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Security.Cryptography;
using MySql.Data.MySqlClient;
using ReversiFEI.Network;
using System.Text;

namespace ReversiFEI.Controller
{
    public class Controls : Node
    {
        private NetworkUtilities networkUtilities;
        
        public override void _Ready()
        {
            networkUtilities = GetNode("/root/NetworkUtilities") as NetworkUtilities;
            GD.Randomize();
        }

        private void GoToLogIn()
        {
            GetTree().ChangeScene("res://src/scene/userInterface/LogIn.tscn");
        }
        
        private void GoToSignUp()
        {
            GetTree().ChangeScene("res://src/scene/userInterface/SignUp.tscn");
        }
        
        private void GoToMainMenu()
        {
            GetTree().ChangeScene("res://src/scene/userInterface/MainMenu.tscn");
        }
        
        private void GoToMainMenuAsGuest()
        {
            networkUtilities.Playername = "guest#" + GD.Randi() % 99999999998 + 1;
            GetTree().ChangeScene("res://src/scene/userInterface/MainMenu.tscn");
        }
        
        private void ExitLobby()
        {
            networkUtilities.LeaveGame();
            GetTree().ChangeScene("res://src/scene/userInterface/MainMenu.tscn");
        }
        
        private void GoToReversiMenu()
        {
            GetTree().ChangeScene("res://src/scene/userInterface/ReversiMenu.tscn");
        }
        
        public void GoToLobby()
        {
            GetTree().ChangeScene("res://src/scene/userInterface/Lobby.tscn");
        }

        public void GoToMatch()
        {
            GetTree().ChangeScene("res://src/scene/userInterface/Match.tscn");
        }

        private void GoToHowTo()
        {
            GetTree().ChangeScene("res://src/scene/userInterface/HowTo.tscn");
        }


        private void GoToFriendsList()
        {
            GetTree().ChangeScene("res://src/scene/userInterface/FriendsList.tscn");
        }


        private void GoToLeaderboard()
        {
            GetTree().ChangeScene("res://src/scene/userInterface/Leaderboard.tscn");
        }
        
        private void GoToCustomizeProfile()
        {
            GetTree().ChangeScene("res://src/scene/userInterface/CustomizeProfile.tscn");
        }


        private void GoToSound()
        {
            GetTree().ChangeScene("res://src/scene/userInterface/Sound.tscn");
        }

        private void GoToLanguage()
        {
            GetTree().ChangeScene("res://src/scene/userInterface/Language.tscn");
        }
        
        private void Exit()
        {
            GetTree().Quit();
        }
        
        private void ChangeLanguage()
        {
            switch(TranslationServer.GetLocale())
            {
                case "en":
                    TranslationServer.SetLocale("es");
                    break;
                case "es":
                    TranslationServer.SetLocale("en");
                    break;
                default:
                    TranslationServer.SetLocale("en");
                    break;
            }
        }
        
        private bool ValidateEmail(String email) {
            var validEmail = true;
            
            if(String.IsNullOrEmpty(email))
            {
                validEmail = false;
            }
            else if(!EmailValidator.Validate(email))
            {
                validEmail = false;
            }
            return validEmail;
        }
        
        private bool ValidatePassword(String password) {        
            return !String.IsNullOrEmpty(password);
        }

        private async Task SignUp()
            Random generator = new Random();
            string email = GetNode<LineEdit>("EmailLineEdit").Text;
            string username = GetNode<LineEdit>("UsernameLineEdit").Text;
            string password = GetNode<LineEdit>("PasswordLineEdit").Text;
            string confirmPassword = GetNode<LineEdit>("ConfirmPasswordLineEdit").Text;
            int verificationCode = generator.Next(100000, 1000000);
                
                SmtpClient client = new SmtpClient("smtp-mail.outlook.com");
                        client.Port = 587;
                        client.DeliveryMethod = SmtpDeliveryMethod.Network;
                        client.UseDefaultCredentials = false;
                        System.Net.NetworkCredential credential =
                            new System.Net.NetworkCredential("reversifei@outlook.com", "reversi123");
                        client.EnableSsl = true;
                        client.Credentials = credential;

                        MailMessage message = new MailMessage("reversifei@outlook.com", "reversifei@gmail.com");
                        message.Subject = "Test mail";
                        message.Body = "This is a test";
                        client.Send(message);
                
            if(ValidateEmail(email) && password.Equals(confirmPassword))
            {
                try
                    {
                        
                        //int verification =  int.Parse(GetNode<LineEdit>("Verification/TextEdit").Text);
                       GetNode<WindowDialog>("Verification").Visible = true;
                       // VerificationButtonPressed(email,username,password,verificationCode,verification);
                    }
                    catch (Exception)
                    {
                        throw;
                    }
            }
        }

        private async Task LogIn()
        {
            string email = GetNode<LineEdit>("EmailLineEdit").Text;
            string password = GetNode<LineEdit>("PasswordLineEdit").Text;
            
            if(ValidateEmail(email) && ValidatePassword(password)) 
            {
                email = String.Concat(email.Where(c => !Char.IsWhiteSpace(c)));
                
                networkUtilities.JoinGame();
                await ToSignal(GetTree(), "connected_to_server");
                if(GetTree().NetworkPeer == null)
                    GD.Print("Log in failed.");
                else
                    networkUtilities.LogIn(email, password);
                    
                await ToSignal(networkUtilities, "LoggedIn");
                ShowNicknameUpdatedPopUp();
                GoToMainMenu();
            } else {
                GD.Print("Invalid email or password");
            }   
        }
        
        private async Task VerificationButtonPressed(string email, string username, string password, int verificationCode,int codeEntered)
        {
            if(verificationCode==codeEntered)
            {
                networkUtilities.JoinGame();
                await ToSignal(GetTree(), "connected_to_server");
                if(GetTree().NetworkPeer == null)
                    GD.Print("Sign up failed.");
                else
                {
                    networkUtilities.SignUp(email, username, password);
                }
            }else
            {
                GetNode<AcceptDialog>("IncorrectDialog").Visible = true;
                string verification = GetNode<LineEdit>("Verification/TextEdit").Text;
            }
        }
        
        private void ShowConnectionFailedPopUp()
        {
            GetNode<WindowDialog>("PopUp/ConnectionFailed").Visible = true;
        }
        
        private void HideConnectionFailedPopUp()
        {
            GetNode<WindowDialog>("ConnectionFailed").Visible = false;
        }
        
        private void ShowChallengeDeclinedPopUp()
        {
            GetNode<WindowDialog>("PopUp/ChallengeDeclined").Visible = true;
        }
        
        private void HideChallengeDeclinedPopUp()
        {
            GetNode<WindowDialog>("ChallengeDeclined").Visible = false;
        }
        
        private void ShowFriendAddedPopUp()
        {
            GetNode<WindowDialog>("PopUp/FriendAdded").Visible = true;
        }
        
        private void HideFriendAddedPopUp()
        {
            GetNode<WindowDialog>("FriendAdded").Visible = false;
        }
        
        private void ShowFriendDeletedPopUp()
        {
            GetNode<WindowDialog>("PopUp/FriendDeleted").Visible = true;
        }
        
        private void HideFriendDeletedPopUp()
        {
            GetNode<WindowDialog>("FriendDeleted").Visible = false;
        }
        
        private void ShowSetOfPiecesUpdatedPopUp()
        {
            GetNode<WindowDialog>("PopUp/SetOfPiecesUpdated").Visible = true;
        }
        
        private void HideSetOfPiecesUpdatedPopUp()
        {
            GetNode<WindowDialog>("SetOfPiecesUpdated").Visible = false;
        }
        
        private void ShowPasswordUpdatedPopUp()
        {
            GetNode<WindowDialog>("PopUp/PasswordUpdated").Visible = true;
        }
        
        private void HidePasswordUpdatedPopUp()
        {
            GetNode<WindowDialog>("PasswordUpdated").Visible = false;
        }
        
        private void ShowNicknameUpdatedPopUp()
        {
            GetNode<WindowDialog>("PopUp/NicknameUpdated").Visible = true;
        }
        
        private void HideNicknameUpdatedPopUp()
        {
            GetNode<WindowDialog>("NicknameUpdated").Visible = false;
        }
        
        private void ShowNewChallengePopUp()
        {
            GetNode<WindowDialog>("PopUp/NewChallenge").Visible = true;
        }
        
        private void HideNewChallengePopUp()
        {
            GetNode<WindowDialog>("NewChallenge").Visible = false;
        }
        
        private void ShowChangeNicknamePopUp()
        {
            GetNode<WindowDialog>("PopUp/ChangeNickname").Visible = true;
        }
        
        private void HideChangeNicknamePopUp()
        {
            GetNode<WindowDialog>("ChangeNickname").Visible = false;
        }
        
        private void ShowChangePasswordPopUp()
        {
            GetNode<WindowDialog>("PopUp/ChangePassword").Visible = true;
        }
        
        private void HideChangePasswordPopUp()
        {
            GetNode<WindowDialog>("ChangePassword").Visible = false;
        }
        
        private void ShowChangeSetOfPiecesPopUp()
        {
            GetNode<WindowDialog>("PopUp/ChangeSetOfPieces").Visible = true;
        }
        
        private void HideChangeSetOfPiecesPopUp()
        {
            GetNode<WindowDialog>("ChangeSetOfPieces").Visible = false;
        }
        
        private void ShowChangeAvatarPopUp()
        {
            GetNode<WindowDialog>("PopUp/ChangeAvatar").Visible = true;
        }

        private void HidePlayerOptionsPopUp()
        {
            GetNode<WindowDialog>("PlayerOptions").Visible = false;
        }
        
        private void ChangeSoundState()
        {
            if(GetNode<CheckButton>("SoundCheckButton").Pressed)
            {
                GD.Print("On");
            }
            else
            {
                GD.Print("Off");
            }
        }
        
        private void ChangeNickname()
        {
            var nickname = GetNode<LineEdit>("ChangeNickname/NewNicknameLineEdit").Text;
            var userNickname = networkUtilities.Playername;
            
            var newNickname = String.Concat(nickname.Where(c => !Char.IsWhiteSpace(c)));
            
            if(userNickname != newNickname)
            {
                var nicknameUpdated = networkUtilities.ChangeNickname(newNickname);
            
                if(nicknameUpdated)
                {
                    GetParent().GetNode<Label>("UserNicknameTitle").Text = newNickname;
                    GetNode<WindowDialog>("NicknameUpdated").Visible = true;
                }
            }
            
            GetNode<WindowDialog>("ChangeNickname").Visible = false;
        }
        
        private void _on_Panel_ready()
        {
            var userNickname = networkUtilities.Playername;
            GetNode<Label>("UserNicknameTitle").Text = userNickname;
        }
    }
}

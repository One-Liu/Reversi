using Godot;
using System;
using System.Net.Mail;
using EmailValidation;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using ReversiFEI.Network;
using System.Net;
using System.Text.Json;
using ReversiFEI.Email;
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
            if(networkUtilities.IsHosting())
                networkUtilities.LeaveGame();
            GetTree().ChangeScene("res://src/scene/userInterface/MainMenu.tscn");
        }
        
        private void GoToMainMenuAsGuest()
        {
            networkUtilities.Playername = "guest#" + GD.Randi() % 99999999998 + 1;
            var rng = new RandomNumberGenerator();
            rng.Randomize();
            networkUtilities.PlayerSet = rng.RandiRange(1,4);
            networkUtilities.IsGuest = true;
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
            if(networkUtilities.IsGuest)
                GoToSound();
            else
                GetTree().ChangeScene("res://src/scene/userInterface/CustomizeProfile.tscn");
        }

        private void GoToSound()
        {
            networkUtilities.LeaveGame();
            GetTree().ChangeScene("res://src/scene/userInterface/Sound.tscn");
        }

        private void GoToLanguage()
        {
            networkUtilities.LeaveGame();
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
        
        private bool ValidateEmail(string email) {
            var validEmail = true;
            
            if(string.IsNullOrEmpty(email))
            {
                validEmail = false;
            }
            else if(!EmailValidator.Validate(email))
            {
                validEmail = false;
            }
            
            return validEmail;
        }
        
        private bool ValidatePassword(string password) {
            var validPassword = true;
            
            if(string.IsNullOrEmpty(password))
            {
                validPassword = false;
            }
            else if(password.Length <= 8)
            {
                validPassword = false;
            }
            else if(!password.All(char.IsLetterOrDigit))
            {
                validPassword = false;
            }
            
            return validPassword;
        }

        private async Task SignUp()
        {
            Random generator = new Random();
            string email = GetNode<LineEdit>("EmailLineEdit").Text;
            email = String.Concat(email.Where(c => !Char.IsWhiteSpace(c)));
            string username = GetNode<LineEdit>("UsernameLineEdit").Text;
            username = String.Concat(username.Where(c => !Char.IsWhiteSpace(c)));
            string password = GetNode<LineEdit>("PasswordLineEdit").Text;
            string confirmPassword = GetNode<LineEdit>("ConfirmPasswordLineEdit").Text;
            var emptyFields = GetNode<Label>("EmptyFields");
            var invalidEmailOrPassword = GetNode<Label>("InvalidEmailOrPassword");
            var differentPasswords = GetNode<Label>("DifferentPasswords");
        
            if(string.IsNullOrEmpty(username) || string.IsNullOrEmpty(confirmPassword))
            {
                emptyFields.Visible = true;
                invalidEmailOrPassword.Visible = false;
                differentPasswords.Visible = false;
            }
            else if(!ValidateEmail(email) || !ValidatePassword(password) || !username.All(char.IsLetterOrDigit))
            {
                emptyFields.Visible = false;
                invalidEmailOrPassword.Visible = true;
                differentPasswords.Visible = false;
            }
            else if(!password.Equals(confirmPassword))
            {
                emptyFields.Visible = false;
                invalidEmailOrPassword.Visible = false;
                differentPasswords.Visible = true;
            }
            else
            {
                emptyFields.Visible = false;
                invalidEmailOrPassword.Visible = false;
                differentPasswords.Visible = false;
                
                networkUtilities.JoinGame();
                await ToSignal(GetTree(), "connected_to_server");
                if(GetTree().NetworkPeer == null)
                    GD.Print("Sign up failed.");
                else
                    networkUtilities.SignUp(email, username, password);
            }
        }
        
        private void LogIn()
        {
            
            string email = GetNode<LineEdit>("EmailLineEdit").Text;
            string password = GetNode<LineEdit>("PasswordLineEdit").Text;
            
          //  if(!ValidateEmail(email) || !ValidatePassword(password)) 
           // {
             //   GetNode<Label>("InvalidEmailOrPassword").Visible = true;
         //   } else 
            {
                
                email = String.Concat(email.Where(c => !Char.IsWhiteSpace(c)));
                LogIn(email,password);
            }   
        }
        
        private async Task LogIn(string email, string password)
        {
            networkUtilities.JoinGame();
            await ToSignal(GetTree(), "connected_to_server");
            if(GetTree().NetworkPeer == null)
                GD.Print("Log in failed.");
            else
                networkUtilities.LogIn(email, password);
                
            await ToSignal(networkUtilities, "LoggedIn");
            networkUtilities.IsGuest = false;
            GoToMainMenu();
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
            GetNode<WindowDialog>("SetOfPiecesUpdated").Visible = true;
        }
        
        private void HideSetOfPiecesUpdatedPopUp()
        {
            GetNode<WindowDialog>("SetOfPiecesUpdated").Visible = false;
        }
        
        private void ShowAvatarUpdatedPopUp()
        {
            GetNode<WindowDialog>("AvatarUpdated").Visible = true;
        }
        
        private void HideAvatarUpdatedPopUp()
        {
            GetNode<WindowDialog>("AvatarUpdated").Visible = false;
        }
        
        private void ShowPasswordUpdatedPopUp()
        {
            GetNode<WindowDialog>("PasswordUpdated").Visible = true;
        }
        
        private void HidePasswordUpdatedPopUp()
        {
            GetNode<WindowDialog>("PasswordUpdated").Visible = false;
        }
        
        private void ShowNicknameUpdatedPopUp()
        {
            GetNode<WindowDialog>("NicknameUpdated").Visible = true;
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

        private void HideChangeAvatarPopUp()
        {
            GetNode<WindowDialog>("ChangeAvatar").Visible = false;
        }
        
        private void ChangeSoundState()
        {
            if(GetNode<CheckButton>("SoundCheckButton").Pressed)
            {
                networkUtilities.soundEnabled=true;
            }
            else
            {
                networkUtilities.soundEnabled=false;
            }
        }
        
        private void ChangeNickname()
        {
            var userNickname = networkUtilities.Playername;
            var nickname = GetNode<LineEdit>("ChangeNickname/NewNicknameLineEdit").Text;
            var newNickname = String.Concat(nickname.Where(c => !Char.IsWhiteSpace(c)));
            
            if(ValidNickname(userNickname, newNickname))
            {
                networkUtilities.ChangeNickname(newNickname);
                GetParent().GetNode<Label>("UserNicknameTitle").Text = newNickname;
                ShowNicknameUpdatedPopUp();
            }
            
            GetNode<WindowDialog>("ChangeNickname").Visible = false;
        }
        
        private bool ValidNickname(string oldNickname, string newNickname)
        {
            var validNickname = false;
            
            if(!string.IsNullOrEmpty(newNickname) && !string.Equals(newNickname, oldNickname, StringComparison.Ordinal) && newNickname.All(char.IsLetterOrDigit))
                validNickname = true;
            
            return validNickname;
        }
        
        private void CustomizeProfileOnReady()
        {
            networkUtilities.JoinGame();
            var userNickname = networkUtilities.Playername;
            GetNode<Label>("UserNicknameTitle").Text = userNickname;
        }
        
        private void ChangePassword()
        {   
            var password = GetNode<LineEdit>("ChangePassword/NewPasswordLineEdit").Text;
            var newPassword = String.Concat(password.Where(c => !Char.IsWhiteSpace(c)));
            
            if(ValidatePassword(newPassword))
            {
                networkUtilities.ChangePassword(newPassword);
                ShowPasswordUpdatedPopUp();
            }
            
            HideChangePasswordPopUp();
        }
        
        private void SelectSetOfPieces1()
        {
            var set2 = GetNode<Button>("ChangeSetOfPieces/SetsOfPiecesVBoxContainer/HBoxContainer1/SetOfPieces2Button");
            var set3 = GetNode<Button>("ChangeSetOfPieces/SetsOfPiecesVBoxContainer/HBoxContainer2/SetOfPieces3Button");
            var set4 = GetNode<Button>("ChangeSetOfPieces/SetsOfPiecesVBoxContainer/HBoxContainer2/SetOfPieces4Button");
            
            set2.Pressed = false;
            set3.Pressed = false;
            set4.Pressed = false;
        }
        
        private void SelectSetOfPieces2()
        {
            var set1 = GetNode<Button>("ChangeSetOfPieces/SetsOfPiecesVBoxContainer/HBoxContainer1/SetOfPieces1Button");
            var set3 = GetNode<Button>("ChangeSetOfPieces/SetsOfPiecesVBoxContainer/HBoxContainer2/SetOfPieces3Button");
            var set4 = GetNode<Button>("ChangeSetOfPieces/SetsOfPiecesVBoxContainer/HBoxContainer2/SetOfPieces4Button");
            
            set1.Pressed = false;
            set3.Pressed = false;
            set4.Pressed = false;
        }
        
        private void SelectSetOfPieces3()
        {
            var set1 = GetNode<Button>("ChangeSetOfPieces/SetsOfPiecesVBoxContainer/HBoxContainer1/SetOfPieces1Button");
            var set2 = GetNode<Button>("ChangeSetOfPieces/SetsOfPiecesVBoxContainer/HBoxContainer1/SetOfPieces2Button");
            var set4 = GetNode<Button>("ChangeSetOfPieces/SetsOfPiecesVBoxContainer/HBoxContainer2/SetOfPieces4Button");
            
            set1.Pressed = false;
            set2.Pressed = false;
            set4.Pressed = false;
        }
        
        private void SelectSetOfPieces4()
        {
            var set1 = GetNode<Button>("ChangeSetOfPieces/SetsOfPiecesVBoxContainer/HBoxContainer1/SetOfPieces1Button");
            var set2 = GetNode<Button>("ChangeSetOfPieces/SetsOfPiecesVBoxContainer/HBoxContainer1/SetOfPieces2Button");
            var set3 = GetNode<Button>("ChangeSetOfPieces/SetsOfPiecesVBoxContainer/HBoxContainer2/SetOfPieces3Button");
            
            set1.Pressed = false;
            set2.Pressed = false;
            set3.Pressed = false;
        }
        
        private void ChangeSetOfPieces()
        {
            var set1 = GetNode<Button>("ChangeSetOfPieces/SetsOfPiecesVBoxContainer/HBoxContainer1/SetOfPieces1Button");
            var set2 = GetNode<Button>("ChangeSetOfPieces/SetsOfPiecesVBoxContainer/HBoxContainer1/SetOfPieces2Button");
            var set3 = GetNode<Button>("ChangeSetOfPieces/SetsOfPiecesVBoxContainer/HBoxContainer2/SetOfPieces3Button");
            var set4 = GetNode<Button>("ChangeSetOfPieces/SetsOfPiecesVBoxContainer/HBoxContainer2/SetOfPieces4Button");
            
            if(set1.Pressed)
            {
                networkUtilities.ChangeSetOfPieces(1);
            }
            else if(set2.Pressed)
            {
                networkUtilities.ChangeSetOfPieces(2);
            }
            else if(set3.Pressed)
            {
                networkUtilities.ChangeSetOfPieces(3);
            }
            else if(set4.Pressed)
            {
                networkUtilities.ChangeSetOfPieces(4);
            }
            
            ShowSetOfPiecesUpdatedPopUp();
            
            HideChangeSetOfPiecesPopUp();
        }
        
        private void SelectAvatar1()
        {
            var avatar2 = GetNode<Button>("ChangeAvatar/AvatarsVBoxContainer/HBoxContainer1/Avatar2Button");
            var avatar3 = GetNode<Button>("ChangeAvatar/AvatarsVBoxContainer/HBoxContainer2/Avatar3Button");
            var avatar4 = GetNode<Button>("ChangeAvatar/AvatarsVBoxContainer/HBoxContainer2/Avatar4Button");
            
            avatar2.Pressed = false;
            avatar3.Pressed = false;
            avatar4.Pressed = false;
        }
        
        private void SelectAvatar2()
        {
            var avatar1 = GetNode<Button>("ChangeAvatar/AvatarsVBoxContainer/HBoxContainer1/Avatar1Button");
            var avatar3 = GetNode<Button>("ChangeAvatar/AvatarsVBoxContainer/HBoxContainer2/Avatar3Button");
            var avatar4 = GetNode<Button>("ChangeAvatar/AvatarsVBoxContainer/HBoxContainer2/Avatar4Button");
            
            avatar1.Pressed = false;
            avatar3.Pressed = false;
            avatar4.Pressed = false;
        }
        
        private void SelectAvatar3()
        {
            var avatar1 = GetNode<Button>("ChangeAvatar/AvatarsVBoxContainer/HBoxContainer1/Avatar1Button");
            var avatar2 = GetNode<Button>("ChangeAvatar/AvatarsVBoxContainer/HBoxContainer1/Avatar2Button");
            var avatar4 = GetNode<Button>("ChangeAvatar/AvatarsVBoxContainer/HBoxContainer2/Avatar4Button");
            
            avatar1.Pressed = false;
            avatar2.Pressed = false;
            avatar4.Pressed = false;
        }
        
        private void SelectAvatar4()
        {
            var avatar1 = GetNode<Button>("ChangeAvatar/AvatarsVBoxContainer/HBoxContainer1/Avatar1Button");
            var avatar2 = GetNode<Button>("ChangeAvatar/AvatarsVBoxContainer/HBoxContainer1/Avatar2Button");
            var avatar3 = GetNode<Button>("ChangeAvatar/AvatarsVBoxContainer/HBoxContainer2/Avatar3Button");
            
            avatar1.Pressed = false;
            avatar2.Pressed = false;
            avatar3.Pressed = false;
        }
        
        private void ChangeAvatar()
        {
            
            var avatar1 = GetNode<Button>("ChangeAvatar/AvatarsVBoxContainer/HBoxContainer1/Avatar1Button");
            var avatar2 = GetNode<Button>("ChangeAvatar/AvatarsVBoxContainer/HBoxContainer1/Avatar2Button");
            var avatar3 = GetNode<Button>("ChangeAvatar/AvatarsVBoxContainer/HBoxContainer2/Avatar3Button");
            var avatar4 = GetNode<Button>("ChangeAvatar/AvatarsVBoxContainer/HBoxContainer2/Avatar4Button");
            
            if(avatar1.Pressed)
            {
                networkUtilities.ChangeAvatar(1);
            }
            else if(avatar2.Pressed)
            {
                networkUtilities.ChangeAvatar(2);
            }
            else if(avatar3.Pressed)
            {
                networkUtilities.ChangeAvatar(3);
            }
            else if(avatar4.Pressed)
            {
                networkUtilities.ChangeAvatar(4);
            }
            
            
            ShowAvatarUpdatedPopUp();
            
            HideChangeAvatarPopUp();
        }
    }
}

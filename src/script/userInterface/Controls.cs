using Godot;
using System;

public class Controls : Node
{
    public override void _Ready()
    {
        
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
    
    private void GoToReversiMenu()
    {
        GetTree().ChangeScene("res://src/scene/userInterface/ReversiMenu.tscn");
    }
    
    private void GoToLobby()
    {
        GetTree().ChangeScene("res://src/scene/userInterface/ClientLobby.tscn");
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
    
    private void LogIn()
    {
        ShowNicknameUpdatedPopUp();
        //GoToMainMenu();
    }
    
    private void Register()
    {
        // Replace with function body.
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
    
    private void HideChangeAvatarPopUp()
    {
        GetNode<WindowDialog>("ChangeAvatar").Visible = false;
    }
    
    private void ShowPlayerOptionsPopUp()
    {
        GetNode<WindowDialog>("PopUp/PlayerOptions").Visible = true;
    }
    
    private void HidePlayerOptionsPopUp()
    {
        GetNode<WindowDialog>("PlayerOptions").Visible = false;
    }
}

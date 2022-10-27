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
        GoToMainMenu();
    }
    
    private void Register()
    {
        // Replace with function body.
    }
}

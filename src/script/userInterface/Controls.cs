using Godot;
using System;

using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using MySql.Data.MySqlClient;
using ReversiFEI;
using EmailValidation;

public class Controls : Node
{
    
    private NetworkUtilities networkUtilities;

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
    private void ExitLobby()
    {
        networkUtilities = GetNode("/root/NetworkUtilities") as NetworkUtilities;
        networkUtilities.LeaveGame();
        GetTree().ChangeScene("res://src/scene/userInterface/MainMenu.tscn");
    }
    private void GoToReversiMenu()
    {
        GetTree().ChangeScene("res://src/scene/userInterface/ReversiMenu.tscn");
    }
    
    private void GoToLobby()
    {
        GetTree().ChangeScene("res://src/scene/userInterface/Lobby.tscn");
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
    
    private async void LogIn()
    {
        networkUtilities = GetNode("/root/NetworkUtilities") as NetworkUtilities;
        
        string email = GetParent().GetNode<LineEdit>("EmailLineEdit").Text;
        string password = GetParent().GetNode<LineEdit>("PasswordLineEdit").Text;
        
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
            GoToMainMenu();
            
        } else {
            GD.Print("Invalid email or password");
        }   
    }
    
    private async void SignUp()
    {
        networkUtilities = GetNode("/root/NetworkUtilities") as NetworkUtilities;
        
        string email = GetParent().GetNode<LineEdit>("EmailLineEdit").Text;
        string username = GetParent().GetNode<LineEdit>("UsernameLineEdit").Text;
        string password = GetParent().GetNode<LineEdit>("PasswordLineEdit").Text;
        string confirmPassword = GetParent().GetNode<LineEdit>("ConfirmPasswordLineEdit").Text;
       
        if(ValidateEmail(email))
        {
            if(password.Equals(confirmPassword))
            {
                networkUtilities.JoinGame();
                await ToSignal(GetTree(), "connected_to_server");
                if(GetTree().NetworkPeer == null)
                    GD.Print("Sign up failed.");
                else
                    networkUtilities.SignUp(email, username, password);  
            }
        }
    }
}

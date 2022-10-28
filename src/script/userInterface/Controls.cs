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
    private void ExitLobby()
    {
        var networkUtilities = GetNode("/root/NetworkUtilities") as NetworkUtilities;
        networkUtilities.LeaveGame();
        GetTree().ChangeScene("res://src/scene/userInterface/MainMenu.tscn");
    }
    private void GoToReversiMenu()
    {
        GetTree().ChangeScene("res://src/scene/userInterface/ReversiMenu.tscn");
    }
    
    private void GoToLobby()
    {
        var networkUtilities = GetNode("/root/NetworkUtilities") as NetworkUtilities;
        networkUtilities.JoinGame();
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
    
    private void _on_LogInButton_pressed()
    {
        
        
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
    
    private void LogIn()
    {
       string email = GetParent().GetNode<LineEdit>("EmailLineEdit").Text;
        string password = GetParent().GetNode<LineEdit>("PasswordLineEdit").Text;
        
        if(ValidateEmail(email) && ValidatePassword(password)) {
            email = String.Concat(email.Where(c => !Char.IsWhiteSpace(c)));
            
            using (var db = new PlayerContext())
            {
                try
                {
                    var player = db.Player
                        .SingleOrDefault(b => b.Email == email);
                    
                    byte[] salt = player.Salt;
                    byte[] key = player.Password;
            
                    using (var deriveBytes = new Rfc2898DeriveBytes(password, salt))
                    {
                        byte[] newKey = deriveBytes.GetBytes(64);
                        
                        if(newKey.SequenceEqual(key))
                        {
                            GetTree().ChangeScene("res://src/scene/userInterface/MainMenu.tscn");
                            GD.Print("Log in succesful.");
                        } 
                        else
                        {
                            GD.Print("Log in failed.");
                        }
                    }
                }
                catch(MySqlException e)
                {
                    GD.Print(e.Message);
                }
                catch(NullReferenceException)
                {
                    GD.Print("Log in failed.");
                }
            }
        } else {
            GD.Print("Invalid email or password");
        }
    
                
    }
    
   
    
    /*private void Register()
    {
        GetNode<RichTextLabel>("../EmptyFields").Visible = true;
        //GetNode<RichTextLabel>("../DifferentPasswords").Visible = true;
    }*/
}



using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using MySql.Data.MySqlClient;
using ReversiFEI;
using EmailValidation;

public class LogInButton : Button
{
    public override void _Ready()
    {
        
    }
    
    private void _on_LogInButton_pressed()
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
                            //GetTree().ChangeScene("res://src/scene/userInterface/MainMenu.tscn");
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
                catch(NullReferenceException e)
                {
                    GD.Print("Log in failed.");
                }
            }
        } else {
            GD.Print("Invalid email or password");
        }
    
        
    }
    
    private bool ValidateEmail(String email) {
        var validEmail = true;
        
        if(String.IsNullOrEmpty(email))
        {
            validEmail = false;
        }
        else if(EmailValidator.Validate(email))
        {
            validEmail = false;
        }
        
        return validEmail;
    }
    
    private bool ValidatePassword(String password) {        
        return !String.IsNullOrEmpty(password);
    }
}

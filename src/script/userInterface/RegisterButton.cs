using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using MySql.Data.MySqlClient;
using ReversiFEI;

public class RegisterButton : Button
{
    public override void _Ready()
    {
        
    }
    
    private void _on_RegisterButton_pressed()
    {
        
        string email = GetParent().GetNode<LineEdit>("EmailLineEdit").Text;
        string username = GetParent().GetNode<LineEdit>("UsernameLineEdit").Text;
        string password = GetParent().GetNode<LineEdit>("PasswordLineEdit").Text;
        string confirmPassword = GetParent().GetNode<LineEdit>("ConfirmPasswordLineEdit").Text;
        
        if(password.Equals(confirmPassword))
        {
            
            byte[] salt;
            byte[] passwordBytes;
            
            using (var deriveBytes = new Rfc2898DeriveBytes(password, 16))
            {
                salt = deriveBytes.Salt;
                passwordBytes = deriveBytes.GetBytes(64);
            }
            
            Player playerRegistration = new Player();
            playerRegistration.Email = email;
            playerRegistration.Nickname = username;
            playerRegistration.Password = passwordBytes;
            playerRegistration.Salt = salt;
            
            using (var db = new PlayerContext())
            {
                try
                {
                    db.Player.Add(playerRegistration);
                    
                    if(db.SaveChanges() == 1)
                    {
                        GD.Print("Registration succesful");
                        GetTree().ChangeScene("res://src/scene/userInterface/LogIn.tscn");
                    }
                    else
                    {
                        GD.Print("Registration failes");
                    }
                }
                catch (MySqlException e)
                {
                    GD.Print(e.Message);
                }
            }
        }
    }
}

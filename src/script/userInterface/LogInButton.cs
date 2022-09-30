using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using MySql.Data.MySqlClient;
using ReversiFEI;

public class LogInButton : Button
{
    public override void _Ready()
    {
        
    }
    
    private void _on_LogInButton_pressed()
    {
        
        string email = GetParent().GetNode<LineEdit>("EmailLineEdit").Text;
        string password = GetParent().GetNode<LineEdit>("PasswordLineEdit").Text;
    
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
    }
}

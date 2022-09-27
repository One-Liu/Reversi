using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using MySql.Data.MySqlClient;
using ReversiFEI;

namespace ReversiFEI{
    
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
                        .Where(b => b.Email == email && b.Password == password)
                        .ToList();
                    
                    if(player.Any())
                    {
                        //GetTree().ChangeScene("res://src/scene/userInterface/MainMenu.tscn");
                        GD.Print("Log in succesful.");
                    } 
                    else
                    {
                        GD.Print("Log in failed.");
                    }
                }
                catch (MySqlException e)
                {
                    GD.Print(e.Message);
                }
                catch (KeyNotFoundException e)
                {
                    GD.Print("Log in failed.");
                }
            }
        }
    }
}

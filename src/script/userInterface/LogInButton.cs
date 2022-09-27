using Godot;
using System;
using System.Linq;
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
            LineEdit EmailLineEdit = GetParent().GetNode<LineEdit>("EmailLineEdit");
            LineEdit PasswordLineEdit = GetParent().GetNode<LineEdit>("PasswordLineEdit");
            
            if(EmailLineEdit.Text != "" || PasswordLineEdit.Text != "")
            {
                string email = EmailLineEdit.Text;
                string password = PasswordLineEdit.Text;
            
                using (var db = new PlayerContext())
                {
                    try
                    {
                        var player = db.Player
                            .Where(b => b.email == email && b.password == password)
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
                }
            }
            else
            {
                GD.Print("Fields missing.");
            }
        }
    }
}

using Godot;
using System;
using System.Linq;
using ReversiFEI;

namespace ReversiFEI{
    
    public class LogInButton2 : Button
    {
        public override void _Ready()
        {
            
        }
        
        private void _on_LogInButton_pressed()
        {
            string email = GetNode<LineEdit>("../EmailLineEdit").GetText();
            string password = GetNode<LineEdit>("../PasswordLineEdit").GetText();
        
            using (var db = new PlayerContext())
            {
                var player = db.Players
                    .Single(b => b.email == email && b.password == password);
                
                if(player != null)
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
    }
}

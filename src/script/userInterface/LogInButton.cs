using Godot;
using System;
using System.Linq;
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
        
        if(ValidateEmail(email) && ValidatePassword(password)) {
            email = String.Concat(email.Where(c => !Char.IsWhiteSpace(c)));
            
            try
            {
                if(UserUtilities.LogIn(email,password))
                {
                    GD.Print("log in succesful.");
                } 
                else
                {
                    GD.Print("log in failed.");
                }
            }
            catch (MySqlException e)
            {
                GD.Print(e.Message);
                GD.Print("log in failed.");
            }
            
        } else {
            GD.Print("Invalid email or password");
        }
    
        
    }
    
    private bool ValidateEmail(String email) {
        return !String.IsNullOrEmpty(email);
    }
    
    private bool ValidatePassword(String password) {        
        return !String.IsNullOrEmpty(password);
    }
}

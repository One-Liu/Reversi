using Godot;
using System;
using System.Linq;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Configuration;
using System.Net.Configuration;
using System.Net;
using EmailValidation;
using ReversiFEI;

public class RegisterButton : Button
{
    
    public override void _Ready()
    {
        
    }
    

    /*private bool _on_RegisterButton_pressed()
    {
        string email = GetParent().GetNode<LineEdit>("EmailLineEdit").Text;
        string username = GetParent().GetNode<LineEdit>("UsernameLineEdit").Text;
        string password = GetParent().GetNode<LineEdit>("PasswordLineEdit").Text;
        string confirmPassword = GetParent().GetNode<LineEdit>("ConfirmPasswordLineEdit").Text;
       
        if(ValidateEmail(email))
        {
            email = String.Concat(email.Where(c => !Char.IsWhiteSpace(c)));
            
            if(password.Equals(confirmPassword))
            {
               
                if( string.IsNullOrEmpty(email))
                {
                    GD.Print("Email is empty");
                    return false;
                }
                else
                {
                    if(string.IsNullOrEmpty(password))
                    {
                        GD.Print("Password is empty");
                        return false;
                    }
                if (password.Length < 8 || password.Length > 16)
                {
                    GD.Print("Password must have between 8 y 16 characters");
                    return false;
                }
                else
                {
                    if (!password.Any(char.IsLower) && (!password.Any(char.IsUpper)))
                    {
                        GD.Print("Password must have one lower and one upper letter");
                        return false;
                    }
                        else
                        {
                            if (password.Contains(" "))
                            {
                            GD.Print("Password must not have spaces");
                            return false;
                            }
                            else
                                return true; 
                              }
                        }
                }
            }
            else
            {
                GD.Print("Passwords must be the same");
                return false;
           }
        }
        return false;
    }*/
    
    private void _on_RegisterButton_pressed()
    {
        string email = GetParent().GetNode<LineEdit>("EmailLineEdit").Text;
        string username = GetParent().GetNode<LineEdit>("UsernameLineEdit").Text;
        string password = GetParent().GetNode<LineEdit>("PasswordLineEdit").Text;
        string confirmPassword = GetParent().GetNode<LineEdit>("ConfirmPasswordLineEdit").Text;
       
        if(ValidateEmail(email))
        {
            if(password.Equals(confirmPassword))
            {
                AwaitSignUp(email, username, password);
            }
        }
    }
    
    private bool ValidateEmail(String email) 
    {
        var validEmail = false;        
        if(String.IsNullOrEmpty(email))
        {
            GD.Print("Invalid email");
        }
        else 
        {
            if(EmailValidator.Validate(email))
            {
                validEmail = true;
            }
            else
            {
                GD.Print("Invalid email");
            }
        }
        return validEmail;
    }
    
    private async void AwaitSignUp(string email, string username, string password)
    {
        var networkUtilities = GetNode("/root/NetworkUtilities") as NetworkUtilities;
            
        networkUtilities.JoinGame();
        await ToSignal(GetTree(), "connected_to_server");
        if(GetTree().NetworkPeer == null)
            GD.Print("Sign up failed.");
        else
            networkUtilities.SignUp(email, username, password);  
    }
}

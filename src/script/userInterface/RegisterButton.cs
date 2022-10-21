using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using MySql.Data.MySqlClient;
using System.ComponentModel.DataAnnotations;
using System.Net.Mail;
using System.Configuration;
using System.Net.Configuration;
using System.Net;
using System.Security.Cryptography.X509Certificates;
using System.Net.Security;
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
            
            if(ValidateEmail(email) && ValidateUsername(username) && ValidateConfirmPassword(confirmPassword))
            {
                using (var db = new PlayerContext())
            {
                
                        if(new EmailAddressAttribute().IsValid(email))
                        {
                             GD.Print("Email is valid");
                        }
                             else
                        {
                             GD.Print("Email is invalid");
                        }
            bool flag=false;
            if( string.IsNullOrEmpty(password))
            {
                GD.Print("Password is empty");
                flag=false;
            }
            if (password.Length < 8 || password.Length > 16)
            {
                GD.Print("Password must have between 8 y 16 characters");
                flag=false;
            }
                else
                {
                    if (!password.Any(char.IsLower) && (!password.Any(char.IsUpper)))
                    {	
                        GD.Print("Password must have one lower and one upper letter");
                        flag=false;
                    }
                        else
                        {
                            if (password.Contains(" "))
                        {
                            GD.Print("Password must not have spaces");
                            flag=false;
                        }
                            else
                            {
                                string specialCh = @"%!@#$%^&*()?/>.<,:;'\|}]{[_~`+=-" + "\"";
                                char[] specialChArray = specialCh.ToCharArray();
                                foreach (char ch in specialChArray) 
                                {
                                    if(password.Contains(ch))
                                        playerRegistration.Password = passwordBytes;
                                        else
                                        {
                                            flag=false;
                                            GD.Print("Password must have at least one special character");
                                        }
                                }
                            }
                        }
                    }
                if( string.IsNullOrEmpty(confirmPassword))
                {
                    GD.Print("TextBox is empty");
                }
            }
            }
        }
    }
    
    private bool ValidateUsername(String username)
    {
        return !String.IsNullOrEmpty(username);    
    }
    
    private bool ValidateEmail(String email) 
    {
        var validEmail = true;        
        if(String.IsNullOrEmpty(email))
        {
            validEmail = false;
        }
        else if(!new EmailAddressAttribute().IsValid(email))
        {
            validEmail = false;
        }
        return validEmail;
    }
    
    
    private bool ValidateConfirmPassword(String confirmPassword)
    {        
        return !String.IsNullOrEmpty(confirmPassword);
    }
    
}



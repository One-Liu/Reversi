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
        string email = GetParent().GetNode<LineEdit>("EmailLineEdit").Text;
        string username = GetParent().GetNode<LineEdit>("UsernameLineEdit").Text;
        string password = GetParent().GetNode<LineEdit>("PasswordLineEdit").Text;
        string confirmPassword = GetParent().GetNode<LineEdit>("ConfirmPasswordLineEdit").Text;
        
        if(ValidateEmail(email) && ValidateUsername(username) && ValidateConfirmPassword(confirmPassword) &&  ValidatePassword(password))
        {
            email = String.Concat(email.Where(c => !Char.IsWhiteSpace(c)));
            
            if(password.Equals(confirmPassword))
            {
               
            if( string.IsNullOrEmpty(email))
            {
                GD.Print("Email is empty");
            }
            
            Player playerRegistration = new Player();
            playerRegistration.Email = email;
            playerRegistration.Nickname = username;
            playerRegistration.Password = passwordBytes;
            playerRegistration.Salt = salt;
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
            else
            {
                if(new EmailAddressAttribute().IsValid(email))
                {
                    GD.Print("Email is valid");
                }
                else
                {
                    GD.Print("Email is invalid");
                }
            }
                
            if( string.IsNullOrEmpty(username))
            {
                GD.Print("Usmername is empty");
            }
            if( string.IsNullOrEmpty(password))
            {
                GD.Print("Password is empty");
            }
            else
            {
                if (password.Length < 8 || password.Length > 16)
                GD.Print("La contrasenia debe tener entre 8 y 16 caracteres");
                else
                {
                    if (!password.Any(char.IsLower) && (!password.Any(char.IsUpper)))	
                    GD.Print("La contrasenia debe tener al menos una minuscula y una mayuscula");
                    else
                    {
                        if (password.Contains(" "))
                            GD.Print("La contrasenia no debe tener espacios");
                        else
                        {
                            string specialCh = @"%!@#$%^&*()?/>.<,:;'\|}]{[_~`+=-" + "\"";
                            char[] specialChArray = specialCh.ToCharArray();
                            foreach (char ch in specialChArray) 
                            {
                                    if (!password.Contains(ch))
                                    //playerRegistration.Password = passwordBytes;
                                    GD.Print("La contrasenia debe tener al menos un caracter especial");
                            }
                        }
                    }
                }
                
            if( string.IsNullOrEmpty(confirmPassword))
            {
                GD.Print("TextBox is empty");
            }
            
            try
            { 
                if(UserUtilities.SignUp(email, username, password))
                {
                    GD.Print("Sign up succesful.");
                }
                else
                {
                    GD.Print("Sign up failed.");
                }
            } 
            catch (MySqlException e)
            {
                GD.Print(e.Message);
                GD.Print("Sign up failed.");
            }
           }
        }
    }
    
    private void _on_RegisterButton_pressed()
    {
       
    }
    
    private bool ValidateUsername(String username)
    {
        GD.Print("Invalid email or password");
        return !String.IsNullOrEmpty(username);    
    }
    
    private bool ValidateEmail(String email) 
    {
        var validEmail = true;        
        if(String.IsNullOrEmpty(email))
        {
            GD.Print("Invalid email or password");
            validEmail = false;
        }
        else if(!new EmailAddressAttribute().IsValid(email))
        {
            validEmail = false;
        }
        return validEmail;
    }
    
    private bool ValidatePassword(String password)
    {        
        GD.Print("Invalid email or password");
        return !String.IsNullOrEmpty(password);
    }
    
    private bool ValidateConfirmPassword(String confirmPassword)
    {        
        GD.Print("Invalid email or password");
        return !String.IsNullOrEmpty(confirmPassword);
    }
    
  }
}



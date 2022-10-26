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
using EmailValidation;
using ReversiFEI;

public class RegisterButton : Button
{
    public override void _Ready()
    {
        
    }
    

    private bool _on_RegisterButton_pressed()
    {
        string email = GetParent().GetNode<LineEdit>("EmailLineEdit").Text;
        string username = GetParent().GetNode<LineEdit>("UsernameLineEdit").Text;
        string password = GetParent().GetNode<LineEdit>("PasswordLineEdit").Text;
        string confirmPassword = GetParent().GetNode<LineEdit>("ConfirmPasswordLineEdit").Text;
       
        if(ValidateEmail(email)  )
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
                playerRegistration.GamesWon=0;
                playerRegistration.PiecesSet=1;
                using (var db = new PlayerContext())
                {
           
                    if( string.IsNullOrEmpty(password))
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
                            {
                                try
                                {
                                    playerRegistration.Password = passwordBytes;
                                    db.Player.Add(playerRegistration);
                                    if(db.SaveChanges() == 1)
                                    {
                                        GD.Print("Succesfully registered");
                                        return true;
                                    }
                                    else
                                    {
                                        return false;
                                    }
                                }
                                catch (MySqlException e)
                                {
                                    throw e;
                                }
                              }
                        }
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
        else
        {
            return false;
        }
        return false;
    }
    public bool ValidateEmail(String email) 
    {
        var validEmail = true;        
        if(String.IsNullOrEmpty(email))
        {
            GD.Print("Invalid email ");
            validEmail = false;
        }
        else if(!EmailValidator.Validate(email))
        {
            validEmail = false;
        }
        return validEmail;
    }

}

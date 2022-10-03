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
            
            using (var db = new PlayerContext())
            {
                if( string.IsNullOrEmpty(email))
                {
                    GD.Print("Email is empty");
                }
                    else
                    {
                        if(email == null){
                             GD.Print("Email is null");
                        }
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
                                      if (password.Contains(ch))
                                        playerRegistration.Password = passwordBytes;
                                    else
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
}


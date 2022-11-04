using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using MySql.Data.MySqlClient;
using ReversiFEI;

namespace ReversiFEI
{
    public static class UserUtilities
    {
        public static string LogIn(string email, string password)
        {
            using (var db = new PlayerContext())
                {
                    bool logInUser;
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
                                return player.Nickname;
                            } 
                            else
                            {
                                return null;
                            }
                        }
                    }
                    catch(MySqlException e)
                    {
                        throw e;
                    }
                    catch(NullReferenceException e)
                    {
                        return null;
                    }
                    
                    return logInUser;
                }
        }
        
        public static bool SignUp(string email, string username, string password)
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
                bool userRegistered;
                try
                {
                    db.Player.Add(playerRegistration);
                    
                    if(db.SaveChanges() == 1)
                    {
                        userRegistered = true;
                    }
                    else
                    {
                        userRegistered = false;
                    }
                }
                catch (MySqlException e)
                {
                    throw e;
                }
                
                return userRegistered;
            }
        }
    }
}

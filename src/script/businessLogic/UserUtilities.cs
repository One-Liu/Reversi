using Godot;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Security.Cryptography;
using MySql.Data.MySqlClient;
using ReversiFEI.DatabaseContext;

namespace ReversiFEI.UserTools
{
    public static class UserUtilities
    {
        public static string LogIn(string email, string password)
        {
            using (var db = new PlayerContext())
                {
                    try
                    {
                        var player = db.Player
                            .SingleOrDefault(b => b.Email == email) 
                            ?? new Player();
                        
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
                        GD.PushError(e.Message);
                        throw;
                    }
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
                    GD.PushError(e.Message);
                    throw;
                }
                return userRegistered;
            }
        }
        
         public static bool AddFriend(string playerOne, string playerTwo)
        {
            using (var db = new PlayerContext())
            {
                Friends friendRegistration= new Friends();
                int playerId1 = GetPlayerId(playerOne);
                int playerId2 = GetPlayerId(playerTwo);
                friendRegistration.Player1Id=GetPlayerId(playerOne);;
                friendRegistration.Player2Id=GetPlayerId(playerTwo);
                GD.Print(playerId1+ "   "+playerId2);
                bool friendRegistered;
                try
                {
                    db.Friends.Add(friendRegistration);
                    
                    if(db.SaveChanges() == 1)
                    {
                        friendRegistered = true;
                    }
                    else
                    {
                        friendRegistered = false;
                    }
                }
                catch (MySqlException e)
                {
                    GD.PushError(e.Message);
                    throw;
                }
                return friendRegistered;
            }
        }
         private static int GetPlayerId(string nickname)
        {
            using (var db = new PlayerContext())
            {
                try
                {                
                    var player = 
                        db.Player
                        .SingleOrDefault(b => b.Nickname == nickname)
                        ?? new Player(0);
                    return player.PlayerId;
                }
                catch(MySqlException e)
                {
                    GD.PushError(e.Message);
                    throw;
                }
            }
        }
        
        
    }
}

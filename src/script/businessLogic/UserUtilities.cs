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
        
        public static List<string> GetFriends(string playerName)
        {
            var playerId = GetPlayerId(playerName);
            
            using (var db = new PlayerContext())
            {
                try
                {
                    //Divided in two consults because linq doesn't support conditions in joins                    
                    var friendsList = 
                        (from friend in
                            (from player in db.Player
                            join playerFriends in db.Friends on playerId equals playerFriends.Player1Id
                            select player)
                            .Union
                            (from player in db.Player
                            join playerFriends in db.Friends on playerId equals playerFriends.Player2Id
                            select player)
                            where friend.PlayerId != playerId
                            select friend.Nickname
                        ).ToList();
                    
                    return friendsList;
                }
                catch(MySqlException e)
                {
                    GD.PushError(e.Message);
                    throw;
                }
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
        
        public static bool ChangeNickname(string nickname, string newNickname)
        {
            var nicknameUpdated = false;
            
            using (var db = new PlayerContext())
            {
                try
                {
                    var user = 
                        (from player in db.Player
                        where player.Nickname == nickname
                        select player).FirstOrDefault();
                    
                    var nicknameAlreadyRegistered = 
                        (from player in db.Player
                        where player.Nickname == newNickname
                        select player).FirstOrDefault();
                    
                    if(nicknameAlreadyRegistered == null)
                    {
                        user.Nickname = newNickname;
                        db.SaveChanges();
                        nicknameUpdated = true;
                    }
                }
                catch(MySqlException e)
                {
                    GD.PushError(e.Message);
                    throw;    
                }
            }
            
            return nicknameUpdated;
        }
        
        public static bool ChangePassword(string nickname, string password)
        {
            var passwordUpdated = false;
            
            using (var db = new PlayerContext())
            {
                try
                {
                    var player = 
                        (from player in db.Player
                        where player.Nickname == nickname
                        select player).FirstOrDefault();
                    
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
                    
                    player.Password = password;
                    db.SaveChanges();
                    passwordUpdated = true;
                }
                catch(MySqlException e)
                {
                    GD.PushError(e.Message);
                    throw;    
                }
            }
            
            return passwordUpdated;
        }
    }
}

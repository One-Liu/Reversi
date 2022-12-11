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
            string playerNickname = null;
            
            using (var db = new PlayerContext())
            {
                try
                {
                    var player = db.Player
                        .SingleOrDefault(b => b.Email == email);
                    
                    if(player != null)
                    {
                        byte[] salt = player.Salt;
                        byte[] key = player.Password;
                        
                        using (var deriveBytes = new Rfc2898DeriveBytes(password, salt))
                        {
                            byte[] newKey = deriveBytes.GetBytes(64);
                            
                            if(newKey.SequenceEqual(key))
                            {
                                playerNickname = player.Nickname;
                            }
                        }
                    }
                }
                catch(MySqlException e)
                {
                    GD.PushError(e.Message);
                    throw;
                }
                
                return playerNickname;
            }
        }
        
        public static bool SignUp(string email, string username, string password)
        {
            bool userRegistered;
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
            playerRegistration.PiecesSet = 1;
            playerRegistration.Salt = salt;
            
            using (var db = new PlayerContext())
            {
                try
                {
                    var emailAlreadyRegistered = db.Player
                        .SingleOrDefault(b => b.Email == email);
                    var nicknameAlreadyRegistered = db.Player
                        .SingleOrDefault(b => b.Nickname == username);
                    
                    if(emailAlreadyRegistered != null)
                    {
                        userRegistered = false;
                    }
                    else if(nicknameAlreadyRegistered != null)
                    {
                        userRegistered = false;
                    }
                    else
                    {
                        db.Player.Add(playerRegistration);
                    
                        if(db.SaveChanges() == 1)
                            userRegistered = true;
                        else
                            userRegistered = false;
                    }
                }
                catch (MySqlException e)
                {
                    GD.PushError(e.Message);
                    throw;
                }
            }
            return userRegistered;
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
                            (from friends in db.Friends
                            join player in db.Player on friends.Player1Id equals player.PlayerId
                            where friends.Player2Id == playerId
                            select player)
                            .Union
                            (from friends in db.Friends
                            join player in db.Player on friends.Player2Id equals player.PlayerId
                            where friends.Player1Id == playerId
                            select player)
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
        
        public static int GetPlayerPieceSet(string nickname)
        {
            int setOfPieces = 1;
            
            using (var db = new PlayerContext())
            {
                try
                {
                    var player = db.Player
                                 .SingleOrDefault(b => b.Nickname == nickname)
                                 ?? new Player(0);
                    
                    setOfPieces = player.PiecesSet;
                }
                catch(MySqlException e)
                {
                    GD.PushError(e.Message);
                    throw;
                }
            }
            
            if(setOfPieces <= 0)
                setOfPieces = 1;
            
            return setOfPieces;
        }
        
        public static bool ChangeNickname(string nickname, string newNickname)
        {
            var nicknameUpdated = false;
            
            using (var db = new PlayerContext())
            {
                try
                {
                    var nicknameAlreadyRegistered = 
                        (from player in db.Player
                        where player.Nickname == newNickname
                        select player).FirstOrDefault();
                    
                    if(nicknameAlreadyRegistered == null)
                    {
                        var user = db.Player
                            .SingleOrDefault(b => b.Nickname == nickname)
                            ?? new Player();
                            
                        user.Nickname = newNickname;
                        
                        if(db.SaveChanges() == 1)
                        {
                            nicknameUpdated = true;
                        }
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
            byte[] salt;
            byte[] passwordBytes;
                
            using (var deriveBytes = new Rfc2898DeriveBytes(password, 16))
            {
                salt = deriveBytes.Salt;
                passwordBytes = deriveBytes.GetBytes(64);
            }
            
            using (var db = new PlayerContext())
            {
                try
                {
                    var player = db.Player
                        .SingleOrDefault(b => b.Nickname == nickname) 
                        ?? new Player();
                    
                    player.Salt = salt;
                    player.Password = passwordBytes;
                    
                    if(db.SaveChanges() == 1)
                    {
                        passwordUpdated = true;
                    }
                }
                catch(MySqlException e)
                {
                    GD.PushError(e.Message);
                    throw;    
                }
            }
            
            return passwordUpdated;
        }
        
        public static bool AddVictory(string nickname)
        {
            var victoryAdded = false;
            using (var db = new PlayerContext())
            {
                try
                {
                    var user = db.Player
                        .SingleOrDefault(b => b.Nickname == nickname)
                        ?? new Player();
                            
                    user.GamesWon += 1;
                    
                    if(db.SaveChanges() == 1)
                    {
                        victoryAdded = true;
                    }
                }
                catch(MySqlException e)
                {
                    GD.PushError(e.Message);
                    throw;    
                }
            }
            
            return victoryAdded;
        }

        public static bool ChangeSetOfPieces(string nickname, int setOfPieces)
        {
            var setOfPiecesUpdated = false;
            
            using (var db = new PlayerContext())
            {
                try
                {
                    var player = db.Player
                        .SingleOrDefault(b => b.Nickname == nickname) 
                        ?? new Player();
                    
                    player.PiecesSet = setOfPieces;
                    
                    if(db.SaveChanges() == 1)
                    {
                        setOfPiecesUpdated = true;
                    }
                }
                catch(MySqlException e)
                {
                    GD.PushError(e.Message);
                    throw;
                }
            }
            
            return setOfPiecesUpdated;
        }
        
        public static List<string> GetLeaderboard()
        {
            using (var db = new PlayerContext())
            {
                try
                {                 
                    var leaderboard = 
                        (from player in db.Player
                        orderby player.GamesWon descending
                        select player.Nickname
                        ).Take(15).ToList();

                    return leaderboard;
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

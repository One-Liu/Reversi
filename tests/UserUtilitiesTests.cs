using Godot;
using System;
using ReversiFEI.UserTools;
using System.Collections.Generic;
using System.Linq;

namespace Tests.Reversi 
{
    [Title("UserUtilities tests")]
    
    [Pre(nameof(RunBeforeTestMethod))]
    
    public class UserUtilitiesTests : WAT.Test
    {   
        string playerNickname;
        string email;
        string password;
        string newPlayerNickname;
        string newPlayerEmail;
        string newPlayerPassword;
        List<string> friendsList = new List<string>();
        int newPlayerSetOfPieces;
        
        public void RunBeforeTestMethod()
        {
            playerNickname = "pepe";
            email = "lester@gmail.com";
            password = "luisito123";
            newPlayerNickname = "punisher";
            newPlayerEmail = "max@gmail.com";
            newPlayerPassword = "pepe1234";
            friendsList.Add("luisito");
            newPlayerSetOfPieces = 1;
        }
        
        [Test]
        public void LogInTest()
        {
            Assert.IsTrue(UserUtilities.LogIn(email, password) != null);
        }
        
        [Test]
        public void SignUpTest()
        {
            Assert.IsTrue(UserUtilities.SignUp(newPlayerEmail,newPlayerNickname,newPlayerPassword));
        }
        
        [Test]
        public void GetFriendsTest()
        {
            var retrievedFriendsList = UserUtilities.GetFriends(playerNickname);
            Assert.IsTrue(friendsList.All(retrievedFriendsList.Contains) && retrievedFriendsList.All(friendsList.Contains));
        }
        
        [Test]
        public void ChangeNicknameTest()
        {
            Assert.IsTrue(UserUtilities.ChangeNickname(playerNickname, newPlayerNickname));
        }
        
        [Test]
        public void ChangePasswordTest()
        {
            Assert.IsTrue(UserUtilities.ChangePassword(playerNickname,newPlayerPassword));
        }
        
        [Test]
        public void ChangeSetOfPiecesTest()
        {
            Assert.IsTrue(UserUtilities.ChangeSetOfPieces(playerNickname,newPlayerSetOfPieces));
        }
    }
}

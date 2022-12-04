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
        string newNickname;
        string email;
        string password;
        string newPlayerNickname;
        string newPlayerEmail;
        string newPlayerPassword;
        List<string> friendsList = new List<string>();
        
        public void RunBeforeTestMethod()
        {
            playerNickname = "Lester";
            newNickname = "xXLesterXx";
            email = "lester@gmail.com";
            password = "lester";
            newPlayerNickname = "luisito";
            newPlayerEmail = "luisito@gmail.com";
            newPlayerPassword = "luis01";
            friendsList.Add("luisito");
        }
        
        [Test]
        public void LogInTest()
        {
            Assert.IsTrue(playerNickname == UserUtilities.LogIn(email, password));
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
            Assert.IsTrue(UserUtilities.ChangeNickname(playerNickname, newNickname));
        }
        
        [Test]
        public void ChangePasswordTest()
        {
            Assert.IsTrue(UserUtilities.ChangePassword(playerNickname,password));
        }
    }
}

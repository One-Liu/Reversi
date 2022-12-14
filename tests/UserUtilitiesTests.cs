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
        int newPlayerSetOfPieces;
        
        public void RunBeforeTestMethod()
        {
            playerNickname = "lester";
            email = "lester@gmail.com";
            password = "lester123";
            newPlayerNickname = "denji";
            newPlayerEmail = "denji@gmail.com";
            newPlayerPassword = "denji1234";
            newPlayerSetOfPieces = 1;
        }
        
        [Test]
        public void LogInTest()
        {
            Assert.IsTrue(UserUtilities.LogIn(email, password) != null);
        }
        
        [Test]
        public void LogInFailedTest()
        {
            Assert.IsFalse(UserUtilities.LogIn(email, "123456789") != null);
        }
        
        [Test]
        public void SignUpTest()
        {
            Assert.IsTrue(UserUtilities.SignUp(newPlayerEmail,newPlayerNickname,newPlayerPassword));
        }
        
        [Test]
        public void SignUpFailedEmailAlreadyRegisteredTest()
        {
            Assert.IsFalse(UserUtilities.SignUp("lester@gmail.com",newPlayerNickname,newPlayerPassword));
        }
        
        [Test]
        public void SignUpFailedNicknameAlreadyRegisteredTest()
        {
            Assert.IsFalse(UserUtilities.SignUp(newPlayerEmail,"lester",newPlayerPassword));
        }
        
        [Test]
        public void AddFriendTest()
        {
            Assert.IsTrue(UserUtilities.AddFriend("Mark","SickBoy"));
        }
        
        [Test]
        public void DeleteFriendTest()
        {
            Assert.IsTrue(UserUtilities.DeleteFriend("Mark","SickBoy"));
        }
        
        [Test]
        public void GetFriendsTest()
        {
            Assert.IsTrue(!UserUtilities.GetFriends("tnwlalo").Any());
        }
        
        [Test]
        public void GetPlayerPieceSetTest()
        {
            Assert.IsTrue(UserUtilities.GetPlayerPieceSet("bryanbroos") == 1);
        }
        
        [Test]
        public void GetPlayerPieceSetFailedTest()
        {
            Assert.IsFalse(UserUtilities.GetPlayerPieceSet("bryanbroos") == 2);
        }
        
        [Test]
        public void ChangeNicknameTest()
        {
            Assert.IsTrue(UserUtilities.ChangeNickname("littledemoon", "littledemon"));
        }
        
        [Test]
        public void ChangeNicknameFailedNicknameAlreadyRegisteredTest()
        {
            Assert.IsFalse(UserUtilities.ChangeNickname("bryanbroos", "wildwolf"));
        }
        
        [Test]
        public void ChangePasswordTest()
        {
            Assert.IsTrue(UserUtilities.ChangePassword("Mark","mark12345"));
        }
        
        [Test]
        public void AddVictoryTest()
        {
            Assert.IsTrue(UserUtilities.AddVictory("Mark"));
        }
        
        [Test]
        public void ChangeSetOfPiecesTest()
        {
            Assert.IsTrue(UserUtilities.ChangeSetOfPieces("Mark",4));
        }
        
        [Test]
        public void GetLeaderboardTest()
        {
            Assert.IsTrue(UserUtilities.GetLeaderboard().Any());
        }
    }
}

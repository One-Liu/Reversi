using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using ReversiFEI.Matches;

namespace Tests.Reversi 
{
    [Title("BoardContainer tests")]
    
    [Pre(nameof(RunBeforeTestMethod))]
    
    public class BoardContainerTests : WAT.Test
    {   
        int size;
        int[,] newBoard;
        
        public void RunBeforeTestMethod()
        {
            size = 9;
            newBoard = new int[,]
            {
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,1,-1,0,0,0,0},
                {0,0,0,0,-1,1,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0},
                {0,0,0,0,0,0,0,0,0,0}
            };
        }
        
        [Test]
        public void CreateBoardTest()
        {
            BoardContainer boardContainer = new BoardContainer();
            int[,] generatedBoard = boardContainer.CreateBoard(size);
            boardContainer.Board = generatedBoard;
            
            bool validBoard = true;
            
            for (int row = 0; row < size; row++) 
            {
                for (int column = 0; column < size; column++) 
                {
                    if(generatedBoard[row,column] != newBoard[row,column])
                    {
                        validBoard = false;
                    }
                }
            }
            
            Assert.IsTrue(validBoard);
        }
        
        [Test]
        public void IsInsideBoardTest()
        {
            BoardContainer boardContainer = new BoardContainer();
            boardContainer.Board = newBoard;
            boardContainer.PlayerPiece = 1;
            
            Assert.IsTrue(boardContainer.IsLegalMove(4,3));
        }
        
        [Test]
        public void IsLegalMoveTest()
        {
            BoardContainer boardContainer = new BoardContainer();
            boardContainer.Board = newBoard;
            boardContainer.PlayerPiece = 1;
            
            Assert.IsTrue(boardContainer.IsLegalMove(4,3));
        }
        
        [Test]
        public void MovesAvailableTest()
        {
            BoardContainer boardContainer = new BoardContainer();
            boardContainer.Board = newBoard;
            boardContainer.PlayerPiece = 1;
            
            Assert.IsTrue(boardContainer.MovesAvailable());
        }
        
        [Test]
        public void CountPiecesTest()
        {
            BoardContainer boardContainer = new BoardContainer();
            boardContainer.Board = newBoard;
            boardContainer.PlayerPiece = 1;
            
            Assert.IsTrue(boardContainer.CountPieces(boardContainer.PlayerPiece) == 2);
        }
    }
}

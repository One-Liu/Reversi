using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ReversiFEI{

    public class PlayerContext : DbContext
    {
        public DbSet<Player> Player { get; set; }
        public DbSet<Friends> Friends { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            //for local testing
            optionsBuilder.UseMySQL("server=localhost;database=Reversi;user=reversi;password=Z4Sj(Ba#%3JY=8X");
            
            //for production deployment through dotnet user secrets:
            //optionsBuilder.UseMySQL(Configuration.GetConnectionString("ReversiDatabase"));    
        }
    }
    
    public class Player
    {
        public int PlayerId { get; set; }
        
        public string Nickname { get; set; }
        public string Email { get; set; }
        public byte[] Password { get; set; }
        public byte[] Salt { get; set; }
        public int GamesWon { get; set; }
        public int PiecesSet { get; set; }
    }
    
    public class Friends
    {
        public int FriendsId { get; set; }
        
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
    }
}

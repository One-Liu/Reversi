using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ReversiFEI{

    public class PlayerContext : DbContext
    {
        public DbSet<Player> Player { get; set; }
        //public DbSet<Friends> Friends { get; set; }

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
        
        public string nickname { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public int wonGames { get; set; }
        public int setOfPieces { get; set; }
    }
    
    public class Friends
    {
        public int FriendsId { get; set; }
        
        public Player Player1 { get; set; }
        public Player Player2 { get; set; }
    }
}

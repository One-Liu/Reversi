using System.Collections.Generic;
using MySql.Data.MySqlClient;
using Microsoft.EntityFrameworkCore;

namespace ReversiFEI.DatabaseContext
{

    public class PlayerContext : DbContext
    {
        public DbSet<Player> Player { get; set; }
        public DbSet<Friends> Friends { get; set; }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            var connstring = new MySqlConnectionStringBuilder
                              {
                                  Server = "192.168.82.76",
                                  UserID = "reversi",
                                  Password = "Z4Sj(Ba#%3JY=8X",
                                  Database = "Reversi"
                              };
            
            optionsBuilder.UseMySQL(connstring.ToString());
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
        
        public Player()
        {
        }
        
        public Player(int id)
        {
            PlayerId = id;
        }
    }
    
    public class Friends
    {
        public int FriendsId { get; set; }
        
        public int Player1Id { get; set; }
        public int Player2Id { get; set; }
    }
}

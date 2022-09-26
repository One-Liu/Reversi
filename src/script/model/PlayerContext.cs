using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace ReversiFEI{

    public class PlayerContext : DbContext
    {
        public DbSet<Player> Players { get; set; }
        public DbSet<PlayerToken> PlayerTokens { get; set; }
        public DbSet<Token> Tokens { get; set; }
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            
            //for local testing
            optionsBuilder.UseMySQL("server=localhost;database=reversi;user=reversi;password=password");
            
            //for production deployment through dotnet user secrets:
            //optionsBuilder.UseMySQL(Configuration.GetConnectionString("ReversiDatabase"));    
        }
    }

    public class Player
    {
        public int playerId { get; set; }
        public string username { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public PlayerToken playerToken { get; set; }
    }

    public class PlayerToken
    {
        public int playerTokenID { get; set; }
        
        public int playerId { get; set; }
        public Player player { get; set; }
        public int tokenId { get; set; }
        public Token token { get; set; }
    }

    public class Token
    {
        public int tokenId { get; set; }
        public string design { get; set; }
    }
}

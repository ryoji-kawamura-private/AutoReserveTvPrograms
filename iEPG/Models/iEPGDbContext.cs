using System.Data.Entity;
using iEPG;
using iEPG.Entity;

namespace iEPG.Models
{
    public class iEPGDbContext : DbContext
    {
        public iEPGDbContext() : base("iEPG") { }
        public DbSet<TvProgram> TvPrograms { get; set; }
        public DbSet<PastTvProgram> PastTvPrograms { get; set; }
        public DbSet<SearchCondition> SearchCondition { get; set; }
        public DbSet<FreeWordConditon> FreeWordConditons { get; set; }
    }    
}

using System.Data.Entity;

namespace test2MVC.Models
{
    public class TimingContext : DbContext
    {
        public TimingContext() : base("TimingConnection")
        {
        }
        public DbSet<Timing>Timings { get; set; }

        
    }
}
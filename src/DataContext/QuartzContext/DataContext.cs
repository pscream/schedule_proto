using Microsoft.EntityFrameworkCore;

namespace QuartzContext
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options)
            : base(options)
        {

        }
    }
}

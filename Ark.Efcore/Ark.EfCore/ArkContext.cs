using Microsoft.EntityFrameworkCore;

namespace Ark.EfCore
{
    public class ArkContext : DbContext
    {
        public ArkContext(DbContextOptions options)
        : base(options)
        {
        }
    }
}
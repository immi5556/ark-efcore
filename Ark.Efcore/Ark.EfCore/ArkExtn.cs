using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ark.EfCore
{
    public static class ArkExtn
    {
        static ArkEfcoreSetting LoadConfig(this IConfiguration configuration)
        {
            var conf = configuration.GetSection("ark_efcore").Get<ArkEfcoreSetting>(opt => { });
            if (conf == null) throw new ApplicationException("config section missing.");
            if (string.IsNullOrEmpty(conf.provider)) throw new ApplicationException("provider missing.");
            conf.provider = conf.provider.ToLower();
            if (!new string[] { "sql", "mysql", "postgres", "sqlite" }.Contains(conf.provider)) throw new ApplicationException("invalid provider.");
            return conf;
        }
        public static void AddArkContext<T>(this IServiceCollection services, IConfiguration configuration) where T : ArkContext
        {
            var config = LoadConfig(configuration);
            services.AddDbContext<T>(options =>
            {
                if (config.provider.ToLower() == "sqlite")
                {
                    options.UseSqlite(config.connection_string);
                }
                else if (config.provider.ToLower() == "postgres")
                {
                    options.UseNpgsql(config.connection_string);
                }
                else if (config.provider.ToLower() == "sql")
                {
                    options.UseSqlServer(config.connection_string);
                }
                else if (config.provider.ToLower() == "mysql")
                {
                    options.UseMySQL(config.connection_string);
                }
            });
        }
    }
}

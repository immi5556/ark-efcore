using Microsoft.AspNetCore.Razor.TagHelpers;
using System.Reflection;

namespace Ark.View
{
    public static class TagExtn
    {
        public static Func<string>  RandomStr = () => Guid.NewGuid().ToString("n").Substring(0, 8);
        static void AddArkView(this IMvcBuilder builder)
        {
            builder
                .AddApplicationPart(Assembly.GetExecutingAssembly())
                .AddControllersAsServices()
                .AddTagHelpersAsServices();
        }
        public static void AddArkView(this IServiceCollection services)
        {
            services
                .AddControllers()
                .AddArkView();
        }
    }
}

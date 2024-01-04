using Microsoft.EntityFrameworkCore;
using Todo.Commons.App;
using Todo.Core;

namespace Todo.WebAPI.App_Start
{
    public static class EFCoreSetup
    {
        public static void AddEFCoreSetup(this IServiceCollection services)
        {
            if (services == null) throw new ArgumentNullException(nameof(services));

            services.AddDbContext<DataContext>(options => {
                options.UseSqlServer(Appsettings.Configuration.GetConnectionString("DefaultConnection"));
            });
        }
    }
}

using DotNetCore.CAP.Messages;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Sample.RabbitMQ.MySql
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddDbContext<AppDbContext>();

            services.AddCap(x =>
            {
                x.UseEntityFramework<AppDbContext>();
                x.UseRabbitMQ(o =>
                {
                    o.UserName = "admin";
                    o.Password = "bing2019.00";
                    o.HostName = "10.186.132.60";
                });
                x.UseDashboard();
                x.FailedRetryCount = 5;
                x.FailedThresholdCallback = failed =>
                {
                    var logger = failed.ServiceProvider.GetService<ILogger<Startup>>();
                    logger.LogError($@"A message of type {failed.MessageType} failed after executing {x.FailedRetryCount} several times, 
                        requiring manual troubleshooting. Message name: {failed.Message.GetName()}");
                };
            });

            services.AddControllers();
        }

        public void Configure(IApplicationBuilder app)
        {
            app.UseRouting();
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

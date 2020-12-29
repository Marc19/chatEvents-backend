using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using PowerDiary.ChatEvents.Core.Persistence;
using PowerDiary.ChatEvents.Core.Repositories;
using PowerDiary.ChatEvents.Services.Interfaces;
using PowerDiary.ChatEvents.Services.Services;
using PowerDiary.ChatEvents.API.CustomJSONSerializer;
using PowerDiary.ChatEvents.Core.Observers;

namespace PowerDiary.ChatEvents.API
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddTransient<IChatEventRepository, ChatEventRepository>();
            services.AddTransient<IChatEventService, ChatEventService>();
            services.AddTransient<IChatEventObserverRepository, ChatEventObserverRepository>();
            services.AddSingleton<IChatRoomObserver, ChatRoomEventObserver>();
            services.AddSingleton<IInMemoryDB, InMemoryDB>();

            services.AddControllers().AddJsonOptions(options => {
                options.JsonSerializerOptions.Converters.Add(new EventViewModelJsonConverter());
            }); ;

            services.AddCors(o => o.AddPolicy("MyPolicy", builder =>
            {
                builder.AllowAnyOrigin()
                       .AllowAnyMethod()
                       .AllowAnyHeader();
            }));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseCors("MyPolicy");

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

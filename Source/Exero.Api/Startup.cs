using Exero.Api.Repositories;
using Exero.Api.Repositories.Memory;
using Exero.Api.Repositories.Neo4j;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json.Serialization;

namespace Exero.Api
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
            // Add framework services.
            services.AddMvc(options =>
                {
                    options.Filters.Add(new RequireHttpsAttribute());
                })
                .AddJsonOptions(options => options.SerializerSettings.ContractResolver = 
                    new DefaultContractResolver());

            // DI
            services.Configure<ExeroSettings>(Configuration.GetSection("ExeroSettings"));

            services.AddSingleton<IGraphRepository, GraphRepository>();
            services.AddSingleton<ICategoryRepository, CategoryRepository>();
            services.AddSingleton<IUserRepository, UserMemoryRepository>();
            services.AddSingleton<IExerciseGroupRepository, ExerciseGroupRepository>();
            services.AddSingleton<IExerciseRepository, ExerciseRepository>();
            services.AddSingleton<IWorkoutSessionRepository, WorkoutSessionRepository>();
            services.AddSingleton<IExerciseSessionRepository, ExerciseSessionRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(
            IApplicationBuilder app, 
            IHostingEnvironment env,
            ILoggerFactory loggerFactory)
        {
            loggerFactory.AddConsole();

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRewriter(new RewriteOptions().AddRedirectToHttps(301, 44343));

            app.UseMvc();
        }
    }
}

using FluentValidation.AspNetCore;
using GradProjectServer.DTO;
using GradProjectServer.Services.EntityFramework;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.Linq;
using System.Reflection;

namespace GradProjectServer
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

            services.AddControllers()
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>())
                .AddNewtonsoftJson(op =>
                {
                    op.SerializerSettings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects;
                    op.SerializerSettings.SerializationBinder = new KnownTypesBinder();
                }).ConfigureApiBehaviorOptions(op => {
                    op.InvalidModelStateResponseFactory = ctx =>
                    {
                        var errors = ctx.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .ToDictionary(e => e.Key, e => (object)e.Value.Errors.Select(e => e.ErrorMessage).ToArray());

                        var error = new ErrorDTO
                        {
                            Description = "Invalid model with the following validation errors.",
                            Data = errors
                        };
                        ObjectResult result = new(error)
                        {
                            StatusCode = StatusCodes.Status422UnprocessableEntity,
                            DeclaredType = typeof(ErrorDTO),
                        };
                        return result;
                    };
                });

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "GradProjectServer", Version = "v1" });
            });

            services.AddAutoMapper(Assembly.GetExecutingAssembly());

            services.AddEntityFrameworkNpgsql();
            var connString = Configuration.GetConnectionString("DefaultConnection");
            services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(connString),
                contextLifetime: ServiceLifetime.Scoped,
                optionsLifetime: ServiceLifetime.Singleton);
            services.AddDbContextFactory<AppDbContext>(opt => opt.UseNpgsql(connString));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GradProjectServer v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

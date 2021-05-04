using FluentValidation.AspNetCore;
using GradProjectServer.DTO;
using GradProjectServer.Services.EntityFramework;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using System;
using System.IO;
using System.Linq;
using System.Reflection;
using GradProjectServer.Controllers;
using GradProjectServer.Services.Exams.Entities;
using GradProjectServer.Services.FilesManagers;
using GradProjectServer.Services.UserSystem;
using SkiaSharp;

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
            //todo: please see https://github.com/micro-elements/MicroElements.Swashbuckle.FluentValidation
            services.AddCors();
            services.AddScoped<DbManager>();
            services.AddScoped<UserManager>();
            services.AddScoped<BlankSubQuestionFileManager>()
                .AddScoped<ProgrammingSubQuestionAnswerFileManager>()
                .AddScoped<ProgrammingSubQuestionFileManager>()
                .AddScoped<ResourceFileManager>()
                .AddScoped<UserFileManager>();
            services.AddScoped<UserManager>();
            services.AddHttpContextAccessor();
            services.AddControllers()
                .AddFluentValidation(fv => fv.RegisterValidatorsFromAssemblyContaining<Startup>())
                .AddNewtonsoftJson(op =>
                {
                    op.SerializerSettings.TypeNameHandling = Newtonsoft.Json.TypeNameHandling.Objects;
                    op.SerializerSettings.SerializationBinder = new KnownTypesBinder();
                }).ConfigureApiBehaviorOptions(op =>
                {
                    op.InvalidModelStateResponseFactory = ctx =>
                    {
                        var errors = ctx.ModelState
                            .Where(e => e.Value.Errors.Count > 0)
                            .ToDictionary(e => e.Key,
                                e => (object) e.Value.Errors.Select(e => e.ErrorMessage).ToArray());

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
                c.SwaggerDoc("v1", new OpenApiInfo {Title = "GradProjectServer", Version = "v1"});
                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            });

            services.AddAutoMapper(Assembly.GetExecutingAssembly());
            var appOptions = Configuration
                .GetSection(AppOptions.SectionName)
                .Get<AppOptions>();

            services.AddDbContext<AppDbContext>(opt => opt.UseNpgsql(appOptions.BuildAppConnectionString()),
                contextLifetime: ServiceLifetime.Scoped,
                optionsLifetime: ServiceLifetime.Singleton);
            services.AddDbContextFactory<AppDbContext>(opt => opt.UseNpgsql(appOptions.BuildAppConnectionString()));
            services.Configure<AppOptions>(Configuration.GetSection(AppOptions.SectionName));
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, DbManager dbManager,
            IServiceProvider serviceProvider)
        {
            UserManager.Init(serviceProvider);
            UserFileManager.Init(serviceProvider);
            ProgrammingSubQuestionFileManager.Init(serviceProvider);
            BlankSubQuestionFileManager.Init(serviceProvider);
            ProgrammingSubQuestionAnswerFileManager.Init(serviceProvider);
            ResourceFileManager.Init(serviceProvider);

            dbManager.EnsureDb().Wait();


            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "GradProjectServer v1"));
            }

            app.UseCors(op => op
                .AllowAnyHeader()
                .AllowAnyMethod()
                .AllowAnyOrigin());
            //app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}
using HotelListing.Configurations;
using HotelListing.Data;
using HotelListing.IRepository;
using HotelListing.Repository;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Design;
using HotelListing.Services;
using AspNetCoreRateLimit;

namespace HotelListing
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

            /*Adding Database Context for connecting to the SQL Server*/
            services.AddDbContext<DatabaseContext>(options =>
                options.UseSqlServer(Configuration.GetConnectionString("sqlConnection"))
            );

            // for rate limiting ann cache throttling 
            services.AddMemoryCache();

            // for rate limiting and cache throttlling 
            services.ConfigureRateLimiting();
            services.AddHttpContextAccessor();  // gives acces to actual controllers and inner workings 

            //services.AddResponseCaching();  // ===> moving this inside of the ConfigureHttpCacheHeaders instead
            services.ConfigureHttpCacheHeaders(); 

            /* CUSTOM Services for authenticating users and securing API to the ServicesExtensions*/
            services.AddAuthentication();
            services.ConfigureIdentity();
            services.ConfigureJWT(Configuration);


            /*OMERSON*/
            services.AddCors(o => {
                o.AddPolicy("CORS Policy", builder =>
                    builder.AllowAnyOrigin()
                    .AllowAnyMethod()
                    .AllowAnyHeader()
                    );
            });

            services.AddAutoMapper(typeof(MapperInitializer));

            // At AddTransient will only create an instance(fresh copy) when a new request comes from client 
            // Other Options
            //  AddSingleton
            //  AddScope
            
            services.AddTransient<IUnitOfWork, UnitOfWork>();

            // Adding the Auth Manager for the "AuthManager.cs"  after the JSON Web Token was defined
            services.AddScoped<IAuthManager, AuthManager>();
            
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "HotelListing", Version = "v1" });
            });



            /*OMERSON - As a best practice we put the Adding of Controller Service at the last
             * of Configuring Services 
             */
            services.AddControllers(config =>
            {
                config.CacheProfiles.Add("120SecondsDuration", new CacheProfile
                {
                    Duration = 120

                });
            })              
            .AddNewtonsoftJson(op => 
                op.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

            services.ConfigureVersioning();


        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            /*OMERSON - from dev env taking out Swagger to the Production env*/
            app.UseSwagger();
            //app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "HotelListing v1"));

            // chaing the UseSwagger statement for local or cloud deployment
            app.UseSwaggerUI(c => {
                string swaggerJsonBasePath = string.IsNullOrWhiteSpace(c.RoutePrefix) ? "." : "..";
                c.SwaggerEndpoint($"{swaggerJsonBasePath}/swagger/v1/swagger.json", "HotelListing Listing API");
            }) ;


            // Now no need to have "Try catch block in each of the API" REST methods
            app.ConfigureExceptionHandler();

            app.UseHttpsRedirection();

            /*app.UseCors("CORS Policy");*/
            app.UseCors("AllowAll");

            /* Caching should be just above the app.UseRouting */
            app.UseResponseCaching();
            app.UseHttpCacheHeaders(); // For Cachin using the Marvin Caching Header library 

            // put rate limiting and throttling just beneath the Caching service
            app.UseIpRateLimiting();

            app.UseRouting();



            // app.UseAuthentication SHOULD ALWAYS COME BEFORE THE app.UseAuthorization
            app.UseAuthentication();
            // after getting the JSON Web token we need to use the method below to allow the JSON Token acquired
            // by the client to get access to secured ENDPOINT of API
            app.UseAuthorization();


            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}

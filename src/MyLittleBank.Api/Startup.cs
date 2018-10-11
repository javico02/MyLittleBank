using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyLittleBank.Api;
using MyLittleBank.Dal;
using MyLittleBank.Entities;
using Swashbuckle.AspNetCore.Swagger;
using Threenine.Data.DependencyInjection;

namespace MyLittleBank
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
            // Configuring database context and migrations assembly
            var dbConnString = Configuration.GetConnectionString(LittleBankApiWellKnownConstants.DB_CONNECTIONSTRING_KEY);
            services
                .AddDbContext<MyLittleBankDbContext>(options => options.UseSqlServer(dbConnString, builder => builder.MigrationsAssembly(typeof(Startup).Assembly.FullName)))
                .AddUnitOfWork<MyLittleBankDbContext>();

            // Configuring Swagger
            var swaggerVersion = Configuration[LittleBankApiWellKnownConstants.SWAGGER_VERSION_KEY];
            var swaggerTitle = Configuration[LittleBankApiWellKnownConstants.SWAGGER_TITLE_KEY];
            var swaggerDescription = Configuration[LittleBankApiWellKnownConstants.SWAGGER_DESCRIPTION_KEY];
            var swaggerTermsOfService = Configuration[LittleBankApiWellKnownConstants.SWAGGER_TERMSOFSERVICE_KEY];
            services
                .AddSwaggerGen(options =>
                {
                    options.DescribeAllEnumsAsStrings();
                    options.SwaggerDoc(swaggerVersion, new Info
                    {
                        Title = swaggerTitle,
                        Version = swaggerVersion,
                        Description = swaggerDescription,
                        TermsOfService = swaggerTermsOfService
                    });
                });

            // Configuring Mvc and Fluent Validator
            services
                .AddMvc()
                .SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
                .AddFluentValidation(fvc =>
                {
                    fvc.RegisterValidatorsFromAssemblyContaining<BankAccountValidator>();
                    fvc.RunDefaultMvcValidationAfterFluentValidationExecutes = false;
                });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env)
        {
            // Creating and applying migrations to the database
            using (var serviceScope = app.ApplicationServices.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                serviceScope.ServiceProvider.GetService<MyLittleBankDbContext>().Database.Migrate();
            }

            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseSwagger();
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint(Configuration[LittleBankApiWellKnownConstants.SWAGGER_ENDPOINT_KEY], Configuration[LittleBankApiWellKnownConstants.SWAGGER_NAME_KEY]);
            });

            app.UseMvc();
        }
    }
}

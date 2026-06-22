using System.Text;
using DevExpress.ExpressApp.ApplicationBuilder;
using DevExpress.ExpressApp.Blazor.ApplicationBuilder;
using DevExpress.ExpressApp.Blazor.Services;
using DevExpress.ExpressApp.Security;
using DevExpress.ExpressApp.Security.Authentication;
using DevExpress.ExpressApp.Security.Authentication.ClientServer;
using DevExpress.ExpressApp.WebApi.Services;
using DevExpress.Persistent.Base;
using DevExpress.Persistent.BaseImpl.EF.PermissionPolicy;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Microsoft.AspNetCore.OData;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ModernSchoolDiary.Blazor.Server.Services;
using ModernSchoolDiary.WebApi.JWT;

namespace ModernSchoolDiary.Blazor.Server
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            var supportedCultures = new[] { new System.Globalization.CultureInfo("ru-RU") };
            services.Configure<RequestLocalizationOptions>(options =>
            {
                options.DefaultRequestCulture = new Microsoft.AspNetCore.Localization.RequestCulture("ru-RU");
                options.SupportedCultures = supportedCultures;
                options.SupportedUICultures = supportedCultures;
            });
            services.AddLocalization();
            services.AddSingleton(typeof(Microsoft.AspNetCore.SignalR.HubConnectionHandler<>), typeof(ProxyHubConnectionHandler<>));
            services.AddRazorPages();
            services.AddServerSideBlazor();
            services.AddHttpContextAccessor();
            services.AddScoped<IAuthenticationTokenProvider, JwtTokenProviderService>();
            services.AddScoped<CircuitHandler, CircuitHandlerProxy>();
            services.AddXaf(Configuration, builder =>
            {
                builder.UseApplication<ModernSchoolDiaryBlazorApplication>();

                builder.AddXafWebApi(webApiBuilder =>
                {
                    webApiBuilder.ConfigureOptions(options =>
                    {
                    });
                });

                builder.Modules
                    .AddAuditTrailEFCore()
                    .AddConditionalAppearance()
                    .AddDashboards(options =>
                    {
                        options.DashboardDataType = typeof(DevExpress.Persistent.BaseImpl.EF.DashboardData);
                    })
                    .AddFileAttachments()
                    .AddNotifications()
                    .AddOffice()
                    .AddReports(options =>
                    {
                        options.EnableInplaceReports = true;
                        options.ReportDataType = typeof(DevExpress.Persistent.BaseImpl.EF.ReportDataV2);
                        options.ReportStoreMode = DevExpress.ExpressApp.ReportsV2.ReportStoreModes.XML;
                    })
                    .AddStateMachine(options =>
                    {
                        options.StateMachineStorageType = typeof(DevExpress.Persistent.BaseImpl.EF.StateMachine.StateMachine);
                    })
                    .AddValidation(options =>
                    {
                        options.AllowValidationDetailsAccess = false;
                    })
                    .AddViewVariants()
                    .Add<ModernSchoolDiary.Module.ModernSchoolDiaryModule>()
                    .Add<ModernSchoolDiaryBlazorModule>();
                builder.ObjectSpaceProviders
                    .AddSecuredEFCore(options =>
                    {
                        options.PreFetchReferenceProperties();
                    })
                    .WithAuditedDbContext(contexts =>
                    {
                        contexts.Configure<ModernSchoolDiary.Module.BusinessObjects.ModernSchoolDiaryEFCoreDbContext, ModernSchoolDiary.Module.BusinessObjects.ModernSchoolDiaryAuditingDbContext>(
                            (serviceProvider, businessObjectDbContextOptions) =>
                            {
                                string connectionString = null;
                                if (Configuration.GetConnectionString("ConnectionString") != null)
                                {
                                    connectionString = Configuration.GetConnectionString("ConnectionString");
                                }
#if EASYTEST
                                if(Configuration.GetConnectionString("EasyTestConnectionString") != null) {
                                    connectionString = Configuration.GetConnectionString("EasyTestConnectionString");
                                }
#endif
                                ArgumentNullException.ThrowIfNull(connectionString);
                                businessObjectDbContextOptions.UseConnectionString(connectionString);
                            },
                            (serviceProvider, auditHistoryDbContextOptions) =>
                            {
                                string connectionString = null;
                                if (Configuration.GetConnectionString("ConnectionString") != null)
                                {
                                    connectionString = Configuration.GetConnectionString("ConnectionString");
                                }
#if EASYTEST
                                if(Configuration.GetConnectionString("EasyTestConnectionString") != null) {
                                    connectionString = Configuration.GetConnectionString("EasyTestConnectionString");
                                }
#endif
                                ArgumentNullException.ThrowIfNull(connectionString);
                                auditHistoryDbContextOptions.UseConnectionString(connectionString);
                            });
                    })
                    .AddNonPersistent();
                builder.Security
                    .UseIntegratedMode(options =>
                    {
                        options.Lockout.Enabled = true;

                        options.RoleType = typeof(PermissionPolicyRole);
                        options.UserType = typeof(ModernSchoolDiary.Module.BusinessObjects.ApplicationUser);
                        options.UserLoginInfoType = typeof(ModernSchoolDiary.Module.BusinessObjects.ApplicationUserLoginInfo);
                        options.Events.OnSecurityStrategyCreated += securityStrategy =>
                        {
                            ((SecurityStrategy)securityStrategy).PermissionsReloadMode = PermissionsReloadMode.NoCache;
                        };
                    })
                    .AddPasswordAuthentication(options =>
                    {
                        options.IsSupportChangePassword = true;
                    });
            });
            var authentication = services.AddAuthentication(options =>
            {
                options.DefaultScheme = CookieAuthenticationDefaults.AuthenticationScheme;
            });
            authentication.AddCookie(options =>
            {
                options.LoginPath = "/LoginPage";
            });
            authentication.AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters()
                {
                    ValidateIssuerSigningKey = true,
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration["Authentication:Jwt:IssuerSigningKey"])),
                    AuthenticationType = JwtBearerDefaults.AuthenticationScheme
                };
            });
            services.AddAuthorization(options =>
            {
                options.DefaultPolicy = new AuthorizationPolicyBuilder(
                    JwtBearerDefaults.AuthenticationScheme)
                        .RequireAuthenticatedUser()
                        .RequireXafAuthentication()
                        .Build();
            });

            services
                .AddControllers()
                .AddOData((options, serviceProvider) =>
                {
                    options
                        .AddRouteComponents("api/odata", new EdmModelBuilder(serviceProvider).GetEdmModel(), Microsoft.OData.ODataVersion.V401, _routeServices =>
                        {
                            _routeServices.ConfigureXafWebApiServices();
                        })
                        .EnableQueryFeatures(100);
                });

            services.AddSwaggerGen(c =>
            {
                c.EnableAnnotations();
                c.SwaggerDoc("v1", new OpenApiInfo
                {
                    Title = "ModernSchoolDiary API",
                    Version = "v1",
                    Description = @"Use AddXafWebApi(options) in the ModernSchoolDiary.Blazor.Server\Startup.cs file to make Business Objects available in the Web API."
                });
                c.AddSecurityDefinition("JWT", new OpenApiSecurityScheme()
                {
                    Type = SecuritySchemeType.Http,
                    Name = "Bearer",
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    In = ParameterLocation.Header
                });
                c.AddSecurityRequirement(new OpenApiSecurityRequirement() {
                    {
                        new OpenApiSecurityScheme() {
                            Reference = new OpenApiReference() {
                                Type = Microsoft.OpenApi.Models.ReferenceType.SecurityScheme,
                                Id = "JWT"
                            }
                        },
                        new string[0]
                    },
                });
            });

            services.Configure<Microsoft.AspNetCore.Mvc.JsonOptions>(o =>
            {
                o.JsonSerializerOptions.PropertyNamingPolicy = null;
            });
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "ModernSchoolDiary WebApi v1");
                });
            }
            else
            {
                app.UseExceptionHandler("/Error");
                app.UseHsts();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseRouting();

            var localizationOptions = app.ApplicationServices
                .GetRequiredService<Microsoft.Extensions.Options.IOptions<RequestLocalizationOptions>>().Value;
            app.UseRequestLocalization(localizationOptions);

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseAntiforgery();
            app.UseXaf();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapXafEndpoints();
                endpoints.MapBlazorHub();
                endpoints.MapFallbackToPage("/_Host");
                endpoints.MapControllers();
            });
        }
    }
}
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.OpenApi.Models;
using Persistencia;
using WebAPI.Controllers;
using Swashbuckle.AspNetCore.SwaggerUI;
using MediatR;
using Aplicacion.CursoApp;
using FluentValidation.AspNetCore;
using WebAPI.Middleware;
using Dominio;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authentication;
using System.Diagnostics;
using Aplicacion.Contratos;
using Seguridad.TokenSeguridad;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Authorization;
using Persistencia.DapperConexion;
using Persistencia.DapperConexion.Instructor;

public class CustomTimeProvider : System.TimeProvider
{
    private readonly TimeSpan offset;

    public CustomTimeProvider(TimeSpan offset)
    {
        this.offset = offset;
    }

    public override DateTimeOffset GetUtcNow()
    {
        return DateTimeOffset.UtcNow + offset;
    }

}

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);


        builder.Services.AddDbContext<CursosOnlineContext>(options =>
        {
            //options.UseMySql(builder.Configuration.GetConnectionString("DefaultConnection"),
            //                 ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("DefaultConnection")));
            options.UseMySql(builder.Configuration.GetConnectionString("MigrationTest"),
                            ServerVersion.AutoDetect(builder.Configuration.GetConnectionString("MigrationTest")));
        });

        builder.Services.AddOptions();
        builder.Services.Configure<ConexionConfiguracion>(builder.Configuration.GetSection("ConnectionStrings"));
        
        builder.Services.AddMediatR(typeof(Consulta.Handler).Assembly);
        builder.Services.AddControllers(opt => { 
                                                    // Obliga a los controllers a requerir autorizacion antes de procesar la peticion de un cliente 
                                                    var policy = new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build();
                                                    opt.Filters.Add(new AuthorizeFilter(policy));
                                               }
                                        ).AddFluentValidation(cfg => cfg.RegisterValidatorsFromAssemblyContaining<Nuevo>());

        var aux = builder.Services.AddIdentityCore<Usuario>();
        var identityBuilder = new IdentityBuilder(aux.UserType, aux.Services);
        identityBuilder.AddEntityFrameworkStores<CursosOnlineContext>();
        identityBuilder.AddSignInManager<SignInManager<Usuario>>();

        // Alterneativa a : TryAddSingleton<ISystemClock, SystemClock>();
        var offset = TimeSpan.FromMicroseconds(1);
        builder.Services.AddSingleton<System.TimeProvider, CustomTimeProvider>(sp => new CustomTimeProvider(offset));

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("unaClaveMuyLargaParaEjemploHMACSHA512ConAlMenos512BitsDeLongitudParaQueElAlgoritmoFuncioneCorrectamente"));

        // Instalamos el paquete Microsoft.AspNetCore.Authentication.JwtBearer en WebAPI
        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
                                                                                                        {
                                                                                                            opt.TokenValidationParameters = new TokenValidationParameters
                                                                                                            { 
                                                                                                               ValidateIssuerSigningKey = true,
                                                                                                               IssuerSigningKey = key,
                                                                                                               ValidateAudience = false,
                                                                                                               ValidateIssuer = false
                                                                                                            };
                                                                                                         }
                                                                                               );

        // Inyectamos como servicio el JwtGenerador
        builder.Services.AddScoped<IJwtGenerador, JwtGenerador>();

        builder.Services.AddScoped<IUsuarioSesion, UsuarioSesion>();

        // Inyectamos el automaper 
        builder.Services.AddAutoMapper(typeof(Consulta.Handler));

        // Inyectamos Drapper 
        builder.Services.AddTransient<IFactoryConnection, FactoryConnection>();
        builder.Services.AddScoped<IInstructor, InstructorRepositorio>();

        var app = builder.Build();
        using (var ambiente = app.Services.CreateScope())
        {
            var services = ambiente.ServiceProvider;
            try
            {
                var userManager = services.GetRequiredService<UserManager<Usuario>>();
                var context = services.GetRequiredService<CursosOnlineContext>();
                context.Database.Migrate();
                await DataPrueba.InsertarData(context, userManager);
            }
            catch (Exception ex) 
            { 
                var logging = services.GetRequiredService<ILogger<Program>>();
                logging.LogError(ex, "Error al migrar");
            }
        }
        // Incluimos nuestro manejador 
        app.UseMiddleware<HandlerErrorMiddleware>();
        if (app.Environment.IsDevelopment())
        {
            // Con esto podemos ver las excepciones crudas sin procesar
            //app.UseDeveloperExceptionPage();
        }

        app.UseHttpsRedirection();

        app.UseAuthentication();

        app.UseRouting();

        app.UseAuthorization();

        app.MapGet("/", () => "Welcome to WebAPI");

        app.MapControllers();

        app.Run();
    }
}

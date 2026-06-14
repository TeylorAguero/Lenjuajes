using ApiWebSecurityCloud.Models;
using ApiWebSecurityCloud.Services;
using Microsoft.EntityFrameworkCore;
using System.Text;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

//Configurar servicio del ORM
builder.Services.AddDbContext<DbContextSecurity>(options =>
options.UseSqlServer(builder.Configuration.GetConnectionString("StringCloud")));

//Se configura el servicio de JWT
//Se agrega la interfaz y el object que la implementa
builder.Services.AddScoped<IAuthorizationServices, AuthorizationServices>();

//Se toma la llave definida para el token
var key = builder.Configuration.GetValue<string>("JwtSettings:Key");
//se transforma la key en un bytes array
var keyBytes = Encoding.ASCII.GetBytes(key);

//se configura la autenticación
builder.Services.AddAuthentication(config => {
    config.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    config.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(config => {
    config.RequireHttpsMetadata = false; //No se requiere metadata
    config.SaveToken = true;//Se debe almacenar el token
    //Configuración de validaciones a realizar el token
    config.TokenValidationParameters = new Microsoft.IdentityModel.Tokens.TokenValidationParameters()
    {
        ValidateIssuerSigningKey = true,//Validad la key de inicio sesión
        IssuerSigningKey = new SymmetricSecurityKey(keyBytes),//se asigna el valor de la key a verificar
        ValidateIssuer = false,//No se valida el emisor
        ValidateAudience = false,//No se valida la audiencia
        ValidateLifetime = true,//Se valida el tiempo de vida al token
        ClockSkew = TimeSpan.Zero//No debe existir diferencia desviación para el tiempo del reloj del token
    };
});
var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//Configuración de autenticación
app.UseAuthentication();

//Configuración de autorización
app.UseAuthorization();

app.MapControllers();

app.Run();

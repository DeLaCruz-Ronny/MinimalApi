using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.OutputCaching;
using MinimalApiPeliculas.Enpoints;
using MinimalApiPeliculas.Entidades;
using MinimalApiPeliculas.Repositorios;

var builder = WebApplication.CreateBuilder(args);
//Inicio del area de loa servicios

//---Habilitando el CORS
var origenesPermitidos = builder.Configuration.GetValue<string>("origenesPermitidos")!;
builder.Services.AddCors(opciones =>
{
    opciones.AddDefaultPolicy(configuration =>
    {
        configuration.WithOrigins(origenesPermitidos).AllowAnyHeader().AllowAnyMethod();
    });

    opciones.AddPolicy("libre", configuration =>
    {
        configuration.AllowAnyOrigin().AllowAnyHeader().AllowAnyMethod();
    });

});

builder.Services.AddOutputCache();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IRepositorioGeneros, RepositorioGeneros>();
builder.Services.AddAutoMapper(typeof(Program));

//Fin del area de los servicios
var app = builder.Build();

app.UseSwagger();
app.UseSwaggerUI();

app.UseCors();

app.UseOutputCache();

app.MapGet("/", () => "Hello World!");

app.MapGroup("/generos").MapGeneros();

app.Run();


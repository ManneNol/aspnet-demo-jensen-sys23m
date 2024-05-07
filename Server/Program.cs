// x Koppla in en databas
// x Göra Post
// x Kunna få in data via vår path
// x Bryta ut funktioner och klasser till andra filer
// x error handling & status codes
// -- session hantering / auth --
// x login
// x skydda routes
// x veta vem som gör en request
using System.Security.Claims;
using Npgsql;
using Server;

string connection = "Host=localhost;Username=sys23m;Password=postgres;Database=aspnet_demo";
await using var db = NpgsqlDataSource.Create(connection);

State state = new(db);

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddAuthentication().AddCookie("sys23m.teachers.aspnetdemo");
builder.Services.AddAuthorizationBuilder().AddPolicy("admin", policy => policy.RequireRole("admin"));
builder.Services.AddSingleton(state);
var app = builder.Build();

app.MapPost("/login", Auth.Login);
app.MapGet("/login", (ClaimsPrincipal user) => user.FindFirstValue(ClaimTypes.NameIdentifier));

app.MapGet("/users", Users.All);
app.MapGet("/users/{id}", Users.Single);
app.MapPost("/users", Users.Post).RequireAuthorization("admin");

app.Run("http://localhost:3000");

public record State(NpgsqlDataSource DB);


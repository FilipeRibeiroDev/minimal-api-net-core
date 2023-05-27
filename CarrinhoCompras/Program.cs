using Microsoft.Extensions.Caching.Distributed;
using System.Text.Json;
using WebHost.Customization;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddStackExchangeRedisCache(options =>
{
    options.Configuration = builder.Configuration.GetValue<string>("CacheSettings:ConnectionString");
});
builder.Services.AddSwaggerGen();

builder.Services.AddServiceSdk(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapPost("/carrinhos", async (Carrinho carrinho, IDistributedCache redis) =>
{
    await redis.SetStringAsync(carrinho.UsuarioId, JsonSerializer.Serialize(carrinho));
    return true;
}).RequireAuthorization("Cliente");

app.MapGet("/carrinhos/{usuarioId}", async (string usuarioId, IDistributedCache redis) =>
{
    var data = await redis.GetStringAsync(usuarioId);

    if (string.IsNullOrEmpty(data)) return null;

    var carrinho = JsonSerializer.Deserialize<Carrinho>(data, new JsonSerializerOptions
    {
        PropertyNameCaseInsensitive = false
    });

    return carrinho;
}).RequireAuthorization("Cliente");

app.Run();


record Carrinho (string UsuarioId, List<Produto> Produtos);

record Produto (string Nome, int Quantidade, decimal PrecoUnitario);
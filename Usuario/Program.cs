using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.MapPost("connect/token", (Autenticacao autenticacao, IConfiguration configuration) =>
{
    if(autenticacao.Usuario == "Admin" && autenticacao.Senha == "123")
        return Token.Create(configuration, "Admin");
    
    return "Usuário não encontrado";
});

app.UseHttpsRedirection();
app.Run();

record Autenticacao(string Usuario, string Senha);

public static class Token
{
    public static object Create(IConfiguration configuration, string role)
    {
        var key = Encoding.ASCII.GetBytes(configuration["Autenticacao:Key"]);

        var tokenConfig = new SecurityTokenDescriptor
        {
            Subject = new System.Security.Claims.ClaimsIdentity(new Claim[]
            {
                new Claim(ClaimTypes.Role, role)
            }),
            Expires = DateTime.UtcNow.AddHours(3),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var tokenHandler = new JwtSecurityTokenHandler();
        var token = tokenHandler.CreateToken(tokenConfig);
        var tokenString = tokenHandler.WriteToken(token);

        return new
        {
            token = tokenString,
        };
    }
}
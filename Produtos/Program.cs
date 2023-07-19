using Azure.Storage.Blobs;
using Estoque;
using FluentValidation;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;
using System.Text.RegularExpressions;
using WebHost.Customization;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddServiceSdk(builder.Configuration);
builder.Services.AddScoped<IValidator<Produtos>, ProdutosValidator>();
builder.Services.AddDbContext<ProdutosDbContext>(options =>
{
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection"));
});
builder.Services.AddTransient<IBlob, Blob>();
var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

Routes.Map(app);

app.Run();

public interface IBlob
{
    Task<string> Upload(IFormFile file);
}

public class Blob : IBlob
{
    private readonly IConfiguration _configuration;

    public Blob(IConfiguration configuration)
    {
        _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
    }

    public async Task<string> Upload(IFormFile file)
    {
        using var stream = new MemoryStream();
        file.CopyTo(stream);
        stream.Position = 0;

        var container = new BlobContainerClient(_configuration["Blob:ConnectionString"], _configuration["Blob:ContainerName"]);
        await container.UploadBlobAsync(file.FileName, stream);
        return container.Uri.AbsoluteUri + "/" + file.FileName;
    }
}

public class Produtos
{
    [Key]
    public int Id { get; set; }
    public string Nome { get; set; }
    public string Foto { get; set; }
}

public class ProdutosDbContext : DbContext
{
    public DbSet<Produtos> Produtos { get;set; }

    public ProdutosDbContext(DbContextOptions options) : base(options) { }
}

public class ProdutosValidator : AbstractValidator<Produtos>
{
    public ProdutosValidator()
    {
        RuleFor(x => x.Nome).NotEmpty().WithMessage("O campo nome é obrigatório.")
            .MaximumLength(10).MinimumLength(5).WithMessage("O número de caracteres é inválido.")
            .Must(ValidarSeContemNumero).WithMessage("O nome não deve conter números.");

        RuleFor(x => x.Foto).NotEmpty().WithMessage("O campo foto é obrigatório");
    }

    private bool ValidarSeContemNumero(string nome)
    {
        if (Regex.IsMatch(nome, "[0-9]")) return false;
        return true;
    }
}



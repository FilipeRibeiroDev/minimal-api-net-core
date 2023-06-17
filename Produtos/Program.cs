using Azure.Storage.Blobs;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
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

app.MapPost("/produtos", (Produtos produto, ProdutosDbContext db) =>
{
    db.Produtos.Add(produto);
    db.SaveChanges();
});

app.MapPost("/produtos/foto", async (IFormFile file, IBlob blob) =>
{
    var url = await blob.Upload(file);
    return new { url };
});

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



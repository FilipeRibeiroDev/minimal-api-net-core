using FluentValidation;
using System.ComponentModel.DataAnnotations;

namespace Estoque
{
    public static class RouteExtension
    {
        public static RouteGroupBuilder MapGroupPublic(this RouteGroupBuilder group)
        {
            group.MapGet("/", async (ProdutosDbContext db) =>
            {
                var produtos = db.Produtos.ToList();
                return Results.Ok(produtos);
            });

            group.MapGet("/{id:int}", async (int id, ProdutosDbContext db) =>
            {
                var produto = db.Produtos.Find(id);
                return Results.Ok(produto);
            });

            group.MapGet("/{name}", async (string name, ProdutosDbContext db) =>
            {
                var produtos = db.Produtos.Where(x => x.Nome.Contains(name));
                return Results.Ok(produtos);
            });

            group.MapGet("/{pageNumber}/{pageQuantity}", async (int pageNumber, int pageQuantity, ProdutosDbContext db) =>
            {
                var produtos = db.Produtos.Skip((pageNumber * pageQuantity) - pageQuantity).Take(pageQuantity).ToList();
                return Results.Ok(produtos);
            });

            return group;
        }

        public static RouteGroupBuilder MapGroupPrivate(this RouteGroupBuilder group)
        {
            group.MapPost("/produtos", AddProductAsync).AddEndpointFilter<ProdutosValidFilter>();

            group.MapPost("/produtos/batch", async (List<Produtos> produtos, IValidator<Produtos> validator, ProdutosDbContext db) =>
            {
                foreach (var produto in produtos)
                {
                    var validation = await validator.ValidateAsync(produto);
                    if (!validation.IsValid) return Results.ValidationProblem(validation.ToDictionary());
                }


                var watchRange = System.Diagnostics.Stopwatch.StartNew();
                db.AddRange(produtos);
                db.SaveChanges();
                watchRange.Stop();

                var elapsedRangeMls = watchRange.ElapsedMilliseconds;
                Console.WriteLine($"\n AddRange: {elapsedRangeMls} \n");


                var watchBulk = System.Diagnostics.Stopwatch.StartNew();
                db.BulkInsert(produtos);
                watchBulk.Stop();

                var elapsedBulkMls = watchBulk.ElapsedMilliseconds;
                Console.WriteLine($"\n BulkInsert: {elapsedBulkMls} \n");


                return Results.Ok();
            });

            group.MapPost("/produtos/foto", async (IFormFile file, IBlob blob) =>
            {
                var url = await blob.Upload(file);
                return new { url };
            });

            group.MapDelete("/{id:int}", async (int id, ProdutosDbContext db) =>
            {
                var produto = db.Produtos.Find(id);
                db.Remove(produto);
                db.SaveChanges();
                return Results.Ok(true);
            });

            return group;
        }

        private static async Task<IResult> AddProductAsync(Produtos produto, IValidator<Produtos> validator, ProdutosDbContext db)
        {
            db.Produtos.Add(produto);
            db.SaveChanges();
            return Results.Ok();
        }
    }
}

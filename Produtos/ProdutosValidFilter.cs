using FluentValidation;

namespace Estoque
{
    public class ProdutosValidFilter : IEndpointFilter
    {
        public async ValueTask<object?> InvokeAsync(EndpointFilterInvocationContext context, EndpointFilterDelegate next)
        {
            var validator = context.HttpContext.RequestServices.GetService<IValidator<Produtos>>();
            var produto = context.GetArgument<Produtos>(0);

            var validation = await validator.ValidateAsync(produto);
            if (!validation.IsValid) return Results.ValidationProblem(validation.ToDictionary());

            return await next(context);
            
        }
    }
}

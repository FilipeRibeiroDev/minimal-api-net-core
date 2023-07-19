using FluentValidation;

namespace Estoque
{
    public static class Routes
    {
        public static void Map(WebApplication app)
        {
            app.MapGroup("/public/produtos").MapGroupPublic().WithTags("Produtos");
            app.MapGroup("/private/produtos").MapGroupPrivate().WithTags("Produtos").RequireAuthorization("Admin");
        }
    }
}

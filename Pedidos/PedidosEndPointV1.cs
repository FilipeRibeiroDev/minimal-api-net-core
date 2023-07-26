using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.HttpResults;
using Pedidos.Model;

namespace Pedidos
{
    public class PedidosEndPointV1
    {
        public static Created<Pedido> AddPedido(Pedido pedido, PedidoDbContext db)
        {
            db.Pedido.Add(pedido);
            db.SaveChanges();

            return TypedResults.Created($"/pedidos/{pedido.Id}", pedido);
        }
    }
}

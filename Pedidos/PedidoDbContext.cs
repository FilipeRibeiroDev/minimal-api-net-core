using Microsoft.EntityFrameworkCore;
using Pedidos.Model;
using System.Collections.Generic;

namespace Pedidos
{
    public class PedidoDbContext: DbContext
    { 
        public DbSet<Pedido> Pedido { get; set; }

        public PedidoDbContext(DbContextOptions options) : base(options) { }
        
    }
}

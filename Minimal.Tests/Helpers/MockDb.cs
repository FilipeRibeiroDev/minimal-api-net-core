using Microsoft.EntityFrameworkCore;
using Pedidos;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Minimal.Tests.Helpers
{
    public class MockDb : IDbContextFactory<PedidoDbContext>
    {
        public PedidoDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<PedidoDbContext>()
                .UseInMemoryDatabase($"InMemoryTestDb-{DateTime.Now.ToFileTimeUtc()}")
                .Options;

            return new PedidoDbContext( options );
        }
    }
}

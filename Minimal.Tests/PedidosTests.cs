using Microsoft.AspNetCore.Http.HttpResults;
using Minimal.Tests.Helpers;
using Pedidos;
using Pedidos.Model;

namespace Minimal.Tests
{
    public class PedidosTests
    {
        [Fact]
        public async void CriarPedidoNoBancoDeDados()
        {
            //Arrange
            var usuarioID = 1;
            var endereco = "Rua Brasil";
            var item = new ItensPedido()
            {
                PedidoID = 1,
                Nome = "Produto 1",
                Preco = "10"
            };

            var pedido = new Pedido(usuarioID, endereco);
            pedido.AddItens(item);

            await using var context = new MockDb().CreateDbContext();


            //Act
            var result = PedidosEndPointV1.AddPedido(pedido, context);


            //Assert
            Assert.IsType<Created<Pedido>>(result);
            Assert.NotNull(pedido);
            Assert.NotNull(pedido.Endereco);
            Assert.NotNull(result);
            Assert.NotNull(result.Location);
            Assert.NotEmpty(context.Pedido);
            Assert.Collection(context.Pedido, p =>
            {
                Assert.Equal(endereco, p.Endereco);
                Assert.Equal(usuarioID, p.UsuarioId);
                Assert.True(p.Itens.Any());
            });
        }
    }
}
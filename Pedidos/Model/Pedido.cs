using System.ComponentModel.DataAnnotations;

namespace Pedidos.Model
{
    public class Pedido
    {
        public Pedido(int usuarioId, string endereco)
        {
            UsuarioId = usuarioId;
            Endereco = endereco;
        }

        [Key]
        public int Id { get; set; }
        public int UsuarioId { get; set; }
        public string Endereco { get; set; }

        public List<ItensPedido> Itens { get; set; } = new();
        public void AddItens(ItensPedido pedido)
        {
            Itens.Add(pedido);
        }
    }
}

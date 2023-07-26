using System.ComponentModel.DataAnnotations;

namespace Pedidos.Model
{
    public class ItensPedido
    {
        [Key]
        public int Id { get; set; }
        public int PedidoID { get; set; }
        public string Nome { get; set; }
        public string Preco { get; set; }
    }
}

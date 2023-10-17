using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ProjetoDrogas.Models
{
    public class Cliente
    {
        public Guid ClienteId { get; set; }
        public string Nome { get; set; }
        [DisplayName("Endereço")]
        public string Endereco { get; set; }
        public string Telefone { get; set; }
        [DisplayName("CPF")]
        public string Cpf { get; set; }
        [DisplayName("E-Mail")]
        public string Email { get; set; }
        [DataType(DataType.Date)]
        [DisplayName("Data de Nascimento")]
        public DateTime Nascimento { get; set; }

    }
}

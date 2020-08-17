using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BroadCast_Servidor.model
{
    public class Usuario
    {
        public Usuario()
        {

        }

        public Usuario(string _nome, DateTime _hora_atual, string _mensagem)
        {
            nome = _nome;
            hora_atual = _hora_atual;
            mensagem = _mensagem;
        }

        public string nome { get; set; }
        public DateTime hora_atual { get; set; }
        public string mensagem { get; set; }
    }
}

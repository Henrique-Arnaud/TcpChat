using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using System.Linq;
using System.Threading;
using BroadCast_Teste.model;
using Newtonsoft.Json;

namespace BroadCast_Teste
{
    class Program
    {
        // fornece o fluxo de dados para o acesso à rede
        static NetworkStream stream;

        //Criação de uma thread para que a função de enviar mensagem rode enquanto o usuário recebe mensagens
        static Thread t1 = new Thread(new ThreadStart(EnviarMensagem));

        static void Main(string[] args)
        {
            TcpClient cliente = new TcpClient();

            int cont = 0;

            //Estabelece conexão com o servidor localizado no ip e na porta abaixo
            cliente.Connect(IPAddress.Parse("192.168.15.7"), 9000);

            Console.WriteLine("Conexão Estabelecida");

            //retorna o NetworkStream usado para enviar e receber dados
            stream = cliente.GetStream();

            //para sempre receber alterações
            while (true)
            {
                //verifica se é a primeira vez rodando o loop
                if (cont == 0)
                {
                    //função criada para que o programa envie uma mensagem em branco pra evitar que de erro na leitura de bytes
                    Lixo();
                }
                while (!stream.DataAvailable) ;

                Byte[] bytes = new byte[256];

                stream.Read(bytes, 0, bytes.Length);

                //traduzo os bytes em string, para depois converter em json
                string data = Encoding.UTF8.GetString(bytes);

                //Transformo os dados recebidos no tipo da classe Usuario
                Usuario msg = JsonConvert.DeserializeObject<Usuario>(data);

                //exibo a mensagem recebida
                Console.WriteLine("\t\t\t"+msg.mensagem+" <= "+msg.nome+" Servidor");

                //Aqui verifico se é a primeira vez rodando esse loop
                if (cont == 0)
                {
                    //caso seja, inicia a thread para escrever e enviar mensagens ao mesmo tempo que recebe
                    t1.Start();
                }

                //valor usado pra saber se é a primeira vez rodando o loop (0 = true)
                cont = 1;
            }
        }

        public static void EnviarMensagem()
        {
            while (true)
            {
                string mensagem = Console.ReadLine();
                DateTime hora_atual = DateTime.Now;
                string nome = Environment.UserName;

                Usuario usuario = new Usuario(nome, hora_atual, mensagem);

                //converte a mensagem convertida em json para bytes
                var enviado = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(usuario));

                //envia a mensagem em bytes 
                stream.Write(enviado, 0, enviado.Length);

                //libera os dados do fluxo
                stream.Flush();
            }
        }

        //função criada para que o programa envie uma mensagem em branco pra evitar que de erro na leitura de bytes
        public static void Lixo()
        {
            Usuario usuario = new Usuario("", DateTime.Now, "");
            var enviado = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(usuario));
            stream.Write(enviado, 0, enviado.Length);
            stream.Flush();
        }
    }
}

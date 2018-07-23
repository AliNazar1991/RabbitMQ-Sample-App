using Common;
using RabbitMQ.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StandardQueue
{
    class Program
    {
        private static ConnectionFactory _factory;
        private static IConnection _connection;
        private static IModel _model;

        private const string Queuename = "StandardQueue_ExampleQueue";

        public static void Main(string[] args)
        {
            var payment1 = new Payment { AmountToPay = 25.0m, CardNumber = "123456789", Name = "Ali Nazar1" };
            var payment2 = new Payment { AmountToPay = 26.0m, CardNumber = "123456789", Name = "Ali Nazar2" };
            var payment3 = new Payment { AmountToPay = 27.0m, CardNumber = "123456789", Name = "Ali Nazar3" };
            var payment4 = new Payment { AmountToPay = 28.0m, CardNumber = "123456789", Name = "Ali Nazar4" };
            var payment5 = new Payment { AmountToPay = 29.0m, CardNumber = "123456789", Name = "Ali Nazar5" };
            var payment6 = new Payment { AmountToPay = 21.0m, CardNumber = "123456789", Name = "Ali Nazar6" };
            var payment7 = new Payment { AmountToPay = 22.0m, CardNumber = "123456789", Name = "Ali Nazar7" };
            var payment8 = new Payment { AmountToPay = 23.0m, CardNumber = "123456789", Name = "Ali Nazar8" };
            var payment9 = new Payment { AmountToPay = 24.0m, CardNumber = "123456789", Name = "Ali Nazar9" };
            var payment10 = new Payment { AmountToPay = 30.0m, CardNumber = "123456789", Name = "Ali Nazar10" };

            CreateQueue();

            SendMessage(payment1);
            SendMessage(payment2);
            SendMessage(payment3);
            SendMessage(payment4);
            SendMessage(payment5);
            SendMessage(payment6);
            SendMessage(payment7);
            SendMessage(payment8);
            SendMessage(payment9);
            SendMessage(payment10);

            Recieve();
        }

        private static void CreateQueue()
        {
            _factory = new ConnectionFactory { HostName = "localhost", UserName = "guest", Password = "guest" };
            _connection = _factory.CreateConnection();
            _model = _connection.CreateModel();

            _model.QueueDeclare(Queuename, true, false, false, null);
        }
        private static void SendMessage(Payment message)
        {
            _model.BasicPublish("", Queuename, null, message.Serialize());

            Console.WriteLine("[x] payment message sent : {0} : {1} : {2}", message.CardNumber, message.AmountToPay, message.Name);
        }

        private static void Recieve()
        {
            var consumer = new QueueingBasicConsumer(_model);
            var msgCount = GetMessageCount(_model,Queuename);

            _model.BasicConsume(Queuename, true, consumer);

            var count = 0;

            while(count < msgCount)
            {
                var message = (Payment)consumer.Queue.Dequeue().Body.DeSerialize(typeof(Payment));

                Console.WriteLine("------- Recieved {0} : {1} : {2}", message.CardNumber, message.AmountToPay, message.Name);
                count++;
            }
        }


        public static uint GetMessageCount(IModel channel, string queuename)
        {
            var result = channel.QueueDeclare(queuename, true, false, false, null);
            return result.MessageCount;
        }
    }
}

using System;
using System.Text;
using IQBusinessSubscriberConsole.Configurations;
using RabbitMQ.Client;
using RabbitMQ.Client.Events;

namespace IQBusinessConsumerConsole
{
    class Program
    {
        static void Main(string[] args)
        {
            try {
                var factory = new ConnectionFactory() { HostName = BusConstants.HostName };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: BusConstants.Queue, durable: false, exclusive: false, autoDelete: false, arguments: null);

                    Console.WriteLine(" [*] Waiting for messages.");

                    var consumer = new EventingBasicConsumer(channel);
                    consumer.Received += (model, ea) =>
                    {
                        var body = ea.Body.ToArray();
                        var message = Encoding.UTF8.GetString(body);
                        string name = message.Substring(message.LastIndexOf(',') + 1).Trim();
                        if (string.IsNullOrEmpty(name))
                        {
                            Console.WriteLine("Cannot respond to empty name {0}", name);
                        }
                        else
                        {
                            Console.WriteLine("Hello {0}, I am your father!", name.ToUpper());
                        }

                    };
                    channel.BasicConsume(queue: BusConstants.Queue, autoAck: true, consumer: consumer);

                    Console.WriteLine(" Press [enter] to exit.");
                    Console.ReadLine();
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message.ToString());
            }
         }   
    }
}

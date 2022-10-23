using RabbitMQ.Client;
using RabbitMQ.Client.Events;
using System;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RabbitMqProducer
{
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
            ConsumerMessages();
        }
        private async Task ConsumerMessages()
        {
            await Task.Run(async () =>
            {
                try
                {
                    var factory = new ConnectionFactory() { HostName = "localhost" };
                    using (var connection = factory.CreateConnection())
                    using (var channel = connection.CreateModel())
                    {

                        channel.QueueDeclare(queue: "Client3",
                                           durable: false,
                                           exclusive: false,
                                           autoDelete: false,
                                           arguments: null);

                        var consumer = new EventingBasicConsumer(channel);

                        consumer.Received += (model, ea) =>
                        {
                            var body = ea.Body.ToArray();
                            var message = Encoding.UTF8.GetString(body);

                            try
                            {
                                richTextBox1.Invoke((MethodInvoker)(() => richTextBox2.Text = message));
                            }
                            catch (Exception ex)
                            {

                            }

                        };
                        channel.BasicConsume(queue: "Client3",
                                             autoAck: true,
                                             consumer: consumer);
                        while (true) { }
                    }
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex.ToString());
                }
            });
        }

        private void button1_Click(object sender, EventArgs e)
        {
            try
            {
                var factory = new ConnectionFactory() { HostName = "localhost" };
                using (var connection = factory.CreateConnection())
                using (var channel = connection.CreateModel())
                {
                    channel.QueueDeclare(queue: "Client4",
                                  durable: false,
                                  exclusive: false,
                                  autoDelete: false,
                                  arguments: null);

                    string message = richTextBox1.Text;
                    var body = Encoding.UTF8.GetBytes(message);

                    channel.BasicPublish(exchange: "",
                                     routingKey: "Client4",
                                     basicProperties: null,
                                     body: body);

                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
           
        }
    }
}

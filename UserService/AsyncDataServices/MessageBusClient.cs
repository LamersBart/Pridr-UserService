using System.Text;
using System.Text.Json;
using RabbitMQ.Client;
using UserService.DTOs;

namespace UserService.AsyncDataServices;

public class MessageBusClient : IMessageBusClient
{
    private readonly IConfiguration _configuration;
    private readonly IConnection? _connection;
    private readonly IModel? _channel;

    public MessageBusClient(IConfiguration configuration)
    {
        _configuration = configuration;
        var factory = new ConnectionFactory()
        {
            HostName = _configuration["RabbitMQHost"],
            Port = int.Parse(_configuration["RabbitMQPort"]!),
            ClientProvidedName = "UserService",
        };
        try
        {
            _connection = factory.CreateConnection();
            _channel = _connection.CreateModel();
            _connection.ConnectionShutdown += RabbitMQ_ConnectionShutdown!;
            Console.WriteLine($"Connected to Message Bus: {_channel.QueueDeclare().QueueName}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"--> Could not connect to the message bus {ex.Message}");
        }
    }
    
    public void ProfileUserNameUpdate(ProfileUserNameDto profileUserNameDto)
    {
        var message = JsonSerializer.Serialize(profileUserNameDto);
        if (_channel!.IsOpen) {
            Console.WriteLine("--> RabbitMQ connection open");
            Console.WriteLine($"--> Publishing message: {message}");
            SendMessage(message);
        } else {
            Console.WriteLine("--> RabbitMQ connection is closed, not sending message");
        }
    }
    
    private void SendMessage(string message)
    {
        var body = Encoding.UTF8.GetBytes(message);
        _channel.BasicPublish(
            exchange: "amq.topic",
            routingKey: "USERNAME_UPDATE",
            basicProperties: null,
            body: body);
        Console.WriteLine($"--> We have send {message}");
    }

    public void Dispose()
    {
        Console.WriteLine("--> Message bus disposed");
        if (_channel.IsOpen)
        {
            _channel.Close();
            _connection.Close();
        }
    }

    private void RabbitMQ_ConnectionShutdown(object sender, ShutdownEventArgs e)
    {
        Console.WriteLine($"--> RabbitMQ Connection Shutdown: {e.ReplyText}");
    }
}
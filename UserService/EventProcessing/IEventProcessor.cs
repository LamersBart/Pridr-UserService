namespace UserService.EventProcessing;

public interface IEventProcessor
{
    Task ProcessEvent(string message);
}
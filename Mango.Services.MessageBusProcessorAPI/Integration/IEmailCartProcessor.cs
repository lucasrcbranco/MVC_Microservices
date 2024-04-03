namespace EmailAPI.Integration;

public interface IEmailCartProcessor
{
    Task ListenAsync();
    Task StopListeningAsync();
}
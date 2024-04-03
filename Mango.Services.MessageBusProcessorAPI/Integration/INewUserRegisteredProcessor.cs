namespace EmailAPI.Integration;

public interface INewUserRegisteredProcessor
{
    Task ListenAsync();
    Task StopListeningAsync();
}
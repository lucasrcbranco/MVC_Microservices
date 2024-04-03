using EmailAPI.Integration;

namespace Mango.Services.EmailAPI.Extensions;

public static class ApplicationBuilderExtensions
{
    private static IEmailCartProcessor? EmailAPI { get; set; }
    private static INewUserRegisteredProcessor? NewUserRegisteredProcessor { get; set; }

    public static IApplicationBuilder ConfigureServiceBusConsumer(this IApplicationBuilder applicationBuilder)
    {
        EmailAPI = applicationBuilder.ApplicationServices.GetService<IEmailCartProcessor>();
        NewUserRegisteredProcessor = applicationBuilder.ApplicationServices.GetService<INewUserRegisteredProcessor>();
        var hostApplicationLife = applicationBuilder.ApplicationServices.GetService<IHostApplicationLifetime>();

        if (hostApplicationLife != null)
        {
            hostApplicationLife.ApplicationStarted.Register(OnStart);
            hostApplicationLife.ApplicationStopping.Register(OnStop);
        }

        return applicationBuilder;
    }

    private static async void OnStop()
    {
        if (EmailAPI is null) throw new ArgumentException("Could not create an instance of message bus service");
        if (NewUserRegisteredProcessor is null) throw new ArgumentException("Could not create an instance of message bus service");
        await EmailAPI.StopListeningAsync();
        await NewUserRegisteredProcessor.StopListeningAsync();
    }

    private static async void OnStart()
    {
        if (EmailAPI is null) throw new ArgumentException("Could not create an instance of message bus service");
        if (NewUserRegisteredProcessor is null) throw new ArgumentException("Could not create an instance of message bus service");
        await EmailAPI.ListenAsync();
        await NewUserRegisteredProcessor.ListenAsync();
    }
}

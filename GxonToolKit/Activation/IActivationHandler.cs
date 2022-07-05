using System.Threading.Tasks;

namespace GxonToolKit.Activation;

public interface IActivationHandler
{
    bool CanHandle(object args);

    Task HandleAsync(object args);
}

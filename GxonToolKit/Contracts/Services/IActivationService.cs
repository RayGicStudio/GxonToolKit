using System.Threading.Tasks;

namespace GxonToolKit.Contracts.Services;

public interface IActivationService
{
    Task ActivateAsync(object activationArgs);
}

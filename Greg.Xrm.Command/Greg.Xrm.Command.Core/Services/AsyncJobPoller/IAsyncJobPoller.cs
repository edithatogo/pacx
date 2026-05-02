using Greg.Xrm.Command.Services.Output;

namespace Greg.Xrm.Command.Services.AsyncJobPoller
{
    public interface IAsyncJobPoller
    {
        Task<bool> WaitForOperationAsync(string operationLocation, TimeSpan timeout, TimeSpan pollingInterval, IOutput output, CancellationToken ct);
        Task<bool> WaitForEnvironmentProvisioningAsync(string environmentId, TimeSpan timeout, TimeSpan pollingInterval, IOutput output, CancellationToken ct);
    }
}

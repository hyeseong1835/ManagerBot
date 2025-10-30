using System.Threading.Tasks;

namespace ManagerBot.Features;

public interface IFeature
{
    IFeatureInfo Info { get; set; }
    IHostContext HostCtx { get; set; }
    IFeatureLogger Logger { get; set; }

    ValueTask InitializeAsync();
    ValueTask DisposeAsync();
}

using ReactiveUI;

namespace StreamerUpdate.OBSInterop
{
  public interface IObsRunner : IReactiveObject
  {
    void Go();

    void Stop();

    bool HasExited { get; set; }
  }
}

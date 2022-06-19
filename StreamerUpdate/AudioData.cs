using NAudio.Wave;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;

namespace StreamerUpdate
{
  public class AudioData : ReactiveObject
  {
    [Reactive]
    public int Value { get; set; }
    [Reactive]
    public string Name { get; set; }

    public WaveInEvent WavIn { get; set; }
  }
}
using DynamicData;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Reactive.Linq;
using System.Windows;

namespace StreamerUpdate
{


  public class AudioInputMonitor : IDisposable
  {
    private IDisposable _disposableTimer;
    public ObservableCollection<MMDevice> Devices { get; } = new ObservableCollection<MMDevice>();
    public ObservableCollection<AudioData> AudioInfos { get; }


    public AudioInputMonitor()
    {
      var enumerator = new MMDeviceEnumerator();
      var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);
      Devices.AddRange(devices);
      AudioInfos = new ObservableCollection<AudioData>(new AudioData[Devices.Count]);
      _disposableTimer = Observable.Interval(TimeSpan.FromMilliseconds(100)).Subscribe(_ => UpdateValues());
    }

    public void UpdateValues()
    {
      for (var index = 0; index < Devices.Count; index++)
      {
        if (Application.Current == null)
        {
          AudioInfos[index].WavIn.StopRecording();
          return;
        }

        Application.Current.Dispatcher.Invoke(() =>
        {
          var mmDevice = Devices[index];
          if (AudioInfos[index] == null)
          {
            AudioInfos[index] = new AudioData();
            try
            {
              AudioInfos[index].WavIn = new WaveInEvent();
              AudioInfos[index].WavIn.DeviceNumber = index;
              AudioInfos[index].WavIn.WaveFormat = new WaveFormat(44100, 1);
              AudioInfos[index].WavIn.StartRecording();
            }
            catch (Exception ex)
            {
              Debug.WriteLine(ex.Message);
            }
          }

          AudioInfos[index].Name = mmDevice.FriendlyName;
          AudioInfos[index].Value = (int)(mmDevice.AudioMeterInformation.MasterPeakValue * 100);
        });
      }
    }

    public void Dispose()
    {
      _disposableTimer?.Dispose();
      foreach (var audioInfo in AudioInfos)
      {
        audioInfo.WavIn.StopRecording();
      }
    }
  }
}

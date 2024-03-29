﻿using DynamicData;
using NAudio.CoreAudioApi;
using NAudio.Wave;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Threading;
using System.Windows;

namespace StreamerUpdate
{
  public class AudioInputMonitor : ReactiveObject, IDisposable
  {
    private Timer _disposableTimer;

    public ObservableCollection<MMDevice> Devices { get; } = new ObservableCollection<MMDevice>();
    public ObservableCollection<AudioData> AudioInfos { get; set; }

    public void Dispose()
    {
      _disposableTimer?.Dispose();
      foreach (var audioInfo in AudioInfos) audioInfo.WavIn.StopRecording();
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

    [Reactive]
    public bool HasAudio { get; set; }

    public void BeNoisy()
    {
      if (AudioInfos == null)
      {
        var enumerator = new MMDeviceEnumerator();
        var devices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);
        Devices.AddRange(devices);
        AudioInfos = new ObservableCollection<AudioData>(new AudioData[Devices.Count]);
        if (Devices.Count > 0)
        {
          HasAudio = true;
        }
      }

      _disposableTimer = new Timer(state => UpdateValues(), null, TimeSpan.Zero, TimeSpan.FromMilliseconds(100));
    }

    public void BeQuiet()
    {
      for (var index = 0; index < Devices.Count; index++)
      {
        AudioInfos[index].Value = 0;
      }
      _disposableTimer.Dispose();
    }
  }
}
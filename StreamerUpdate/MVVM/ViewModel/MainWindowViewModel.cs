using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StreamerUpdate.AppContainer;
using StreamerUpdate.MVVM.Model;
using System;
using System.Reactive.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace StreamerUpdate
{
  public class MainWindowViewModel : ReactiveObject
  {
    private readonly Calendar _calendar;
    private AppControl _appControlItem;
    private int _deviceIdx = -1;
    private IDisposable _timerDisposable;
    private BitmapImage _capturedImage;
    private CaptureDevice _captureDevice;
    private IDisposable _fastCaptureDisposable;
    public AudioInputMonitor InputMonitor { get; }

    public MainWindowViewModel(MainWindowModel model)
    {
      Model = model;
      Model.YoutubeHandler.WhenPropertyChanged(v => v.Authenticated).Subscribe(v => Authenticated = v.Value);
      StartStreamingCommand = ReactiveCommand.Create(OpenOBS);
      _captureDevice = new CaptureDevice();
      InputMonitor = new AudioInputMonitor();
      Model.WhenPropertyChanged(m => m.ServiceName).Subscribe((t) => ServiceName = t.Value);
      InputMonitor.WhenPropertyChanged(mo => mo.HasAudio).Subscribe(_ => CheckCanStream());
    }

    private void CheckCanStream()
    {
      CanStream = CameraGood && InputMonitor.HasAudio;
    }

    public void Init()
    {
      ConnectDevice();
      if (CameraBad)
        StartInterval();
      CheckCanStream();
    }

    private void ConnectDevice()
    {
      var devices = _captureDevice.GetDevices();

      if (devices.Count == 0)
      {
        CameraGood = false;
        CameraBad = true;
        return;
      }
      for (var index = 0; index < devices.Count; index++)
      {
        var img = _captureDevice.Capture(index);
        if (img == null)
        {
          continue;
        }
        _timerDisposable?.Dispose();
        _deviceIdx = index;
        CapturedImage = img;
        CameraGood = true;
        CameraBad = false;
        StartFastCapture();
        return;
      }
      CameraGood = false;
      CameraBad = true;
      CheckCanStream();
    }

    private void StartFastCapture()
    {
      bool busy = false;
      _fastCaptureDisposable = Observable.Interval(TimeSpan.FromMilliseconds(500)).Subscribe(
        t =>
        {
          if (busy)
            return;
          busy = true;
          var imag = _captureDevice.Capture(_deviceIdx);
          if (imag == null)
          {
            CameraGood = false;
            CameraBad = true;
            StartInterval();
            _fastCaptureDisposable.Dispose();
            busy = false;
            return;
          }

          CapturedImage = imag;
          busy = false;
        });
    }

    private void StartInterval()
    {
      _timerDisposable = Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(t => ConnectDevice());
    }

    private void OpenOBS()
    {
      AppControlItem.ExeName = @"C:\Program Files\obs-studio\bin\64bit\obs64.exe";
      AppControlItem.Go();
    }

    [Reactive]
    public bool CameraBad { get; set; }

    [Reactive]
    public bool CameraGood { get; set; }

    public ICommand StartStreamingCommand { get; set; }

    public AppControl AppControlItem
    {
      get => _appControlItem;
      set => _appControlItem = value;
    }

    public BitmapImage CapturedImage
    {
      get => _capturedImage;
      set => this.RaiseAndSetIfChanged(ref _capturedImage, value);
    }

    [Reactive]
    public string ServiceName { get; set; }

    [Reactive] public bool CanStream { get; set; }


    [Reactive] 
    public bool Authenticated { get; set; }
    private MainWindowModel Model { get; set; }
  }
}

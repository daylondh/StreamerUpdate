using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StreamerUpdate.MVVM.Model;
using StreamerUpdate.OBSInterop;
using System;
using System.Reactive.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace StreamerUpdate
{
  public class MainWindowViewModel : ReactiveObject
  {
    private readonly Calendar _calendar;
    private int _deviceIdx = -1;
    private IDisposable _timerDisposable;
    private BitmapImage _capturedImage;
    private CaptureDevice _captureDevice;
    private IDisposable _fastCaptureDisposable;
    private IObsRunner _obsRunner;
    public AudioInputMonitor InputMonitor { get; }

    public MainWindowViewModel(MainWindowModel model, IObsRunner obsRunner)
    {
      Model = model;
      _obsRunner = obsRunner;
      StartStreamingCommand = ReactiveCommand.Create(OpenOBS);
      _captureDevice = new CaptureDevice();
      InputMonitor = new AudioInputMonitor();
      Model.WhenPropertyChanged(m => m.ServiceName).Subscribe((t) => ServiceName = t.Value);
      InputMonitor.WhenAnyValue(mo => mo.HasAudio).Subscribe(_ => CheckCanStream());
      _obsRunner.WhenAnyValue(r => r.HasExited).Subscribe(_ => CheckCanStream());
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
      CanStream = false;
      _obsRunner.Go();
    }

    [Reactive]
    public bool CameraBad { get; set; }

    [Reactive]
    public bool CameraGood { get; set; }

    public ICommand StartStreamingCommand { get; set; }


    public BitmapImage CapturedImage
    {
      get => _capturedImage;
      set => this.RaiseAndSetIfChanged(ref _capturedImage, value);
    }

    [Reactive]
    public string ServiceName { get; set; }

    [Reactive] public bool CanStream { get; set; }


    private MainWindowModel Model { get; set; }
  }
}

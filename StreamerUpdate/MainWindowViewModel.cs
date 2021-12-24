using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StreamerUpdate.AppContainer;
using System;
using System.Reactive.Linq;
using System.Windows.Input;
using System.Windows.Media.Imaging;

namespace StreamerUpdate
{
  public class MainWindowViewModel : ReactiveObject
  {
    private AppControl _appControlItem;
    private int _deviceIdx = -1;
    private IDisposable _timerDisposable;
    private BitmapImage _capturedImage;
    private CaptureDevice _captureDevice;
    private IDisposable _fastCaptureDisposable;

    public MainWindowViewModel()
    {
      ClickMeCommand = ReactiveCommand.Create(DoSomething);
      _captureDevice = new CaptureDevice();
      ConnectDevice();
      if (CameraBad)
        StartInterval();
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

        _deviceIdx = index;
        CapturedImage = img;
        _timerDisposable?.Dispose();
        CameraGood = true;
        CameraBad = false;
        StartFastCapture();
        return;
      }

      CameraGood = false;
      CameraBad = true;
    }

    private void StartFastCapture()
    {
      _fastCaptureDisposable = Observable.Interval(TimeSpan.FromMilliseconds(500)).SubscribeOn(RxApp.MainThreadScheduler).ObserveOn(RxApp.MainThreadScheduler).Subscribe(
        t =>
        {
          var imag = _captureDevice.Capture(_deviceIdx);
          if (imag == null)
          {
            CameraGood = false;
            CameraBad = true;
            _fastCaptureDisposable.Dispose();
            StartInterval();
            return;
          }
          CapturedImage = imag;
        });
    }

    private void StartInterval()
    {
      _timerDisposable = Observable.Interval(TimeSpan.FromSeconds(1)).SubscribeOn(RxApp.MainThreadScheduler).ObserveOn(RxApp.MainThreadScheduler).Subscribe(t => ConnectDevice());
    }

    [Reactive]
    public bool CameraBad { get; set; }

    [Reactive]
    public bool CameraGood { get; set; }

    private void DoSomething()
    {
      AppControlItem.ExeName = @"C:\Program Files\obs-studio\bin\64bit\obs64.exe";
      AppControlItem.Go();
    }

    public ICommand ClickMeCommand { get; set; }

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
  }
}

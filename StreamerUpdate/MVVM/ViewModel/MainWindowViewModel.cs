using DynamicData.Binding;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using StreamerUpdate.MVVM.Model;
using StreamerUpdate.OBSInterop;
using System;
using System.Reactive.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace StreamerUpdate
{
  public class MainWindowViewModel : ReactiveObject
  {
    [System.Runtime.InteropServices.DllImport("gdi32.dll")]
    public static extern bool DeleteObject(IntPtr hObject);
    private int _deviceIdx = -1;
    private IDisposable _timerDisposable;
    private ImageSource _capturedImage;
    private readonly CaptureDevice _captureDevice;
    private readonly IObsRunner _obsRunner;
    private bool _captureThreadShouldRun = true;
    public AudioInputMonitor InputMonitor { get; }

    public MainWindowViewModel(MainWindowModel model, IObsRunner obsRunner)
    {
      Model = model;
      _obsRunner = obsRunner;
      StartStreamingCommand = ReactiveCommand.Create(OpenOBS);
      StopStreamingCommand = ReactiveCommand.Create(StopStream);
      _captureDevice = new CaptureDevice();
      InputMonitor = new AudioInputMonitor();
      Model.WhenPropertyChanged(m => m.ServiceName).Subscribe((t) => ServiceName = t.Value);
      InputMonitor.WhenAnyValue(mo => mo.HasAudio).Subscribe(_ => CheckCanStream());
      _obsRunner.WhenAnyValue(r => r.HasExited).Subscribe(_ => ReInit());
    }

    private void StopStream()
    {
      _obsRunner.Stop();
      ReInit();
    }

    private void ReInit()
    {
      IsStreaming = false;
      CheckCanStream();
      if (CanStream)
        StartFastCapture();
      InputMonitor.BeNoisy();

    }

    private void CheckCanStream()
    {
      CanStream = CameraGood && InputMonitor.HasAudio;
    }

    public void Init()
    {
      ConnectDevice();
      InputMonitor.BeNoisy();
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
      var img = _captureDevice.Capture(0);
      if (img == null)
      {
        return;
      }
      _timerDisposable?.Dispose();
      _deviceIdx = 0;
      BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
        img.GetHbitmap(),
        IntPtr.Zero,
        Int32Rect.Empty,
        BitmapSizeOptions.FromEmptyOptions());
      bs.Freeze();
      CapturedImage = bs;
      CameraGood = true;
      CameraBad = false;
      StartFastCapture();
    }

    private void StartFastCapture()
    {
      _captureThreadShouldRun = true;
      new Thread(() =>
      {
        while (_captureThreadShouldRun)
        {
          Thread.Sleep(42);
          var imag = _captureDevice.Capture(_deviceIdx);
          var bmp = imag.GetHbitmap();
          BitmapSource bs = System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
            bmp,
            IntPtr.Zero,
            Int32Rect.Empty,
            BitmapSizeOptions.FromEmptyOptions());
          bs.Freeze();
          CapturedImage = bs;
          DeleteObject(bmp);
        }
        CapturedImage = null;
      }).Start();
    }

    private void StartInterval()
    {
      _timerDisposable = Observable.Interval(TimeSpan.FromSeconds(1)).Subscribe(t =>
      {
        ConnectDevice();
      });
    }

    private void OpenOBS()
    {
      IsStreaming = true;
      CanStream = false;
      _captureThreadShouldRun = false;
      InputMonitor.BeQuiet();
      _obsRunner.Go();
    }

    [Reactive]
    public bool CameraBad { get; set; }

    [Reactive]
    public bool CameraGood { get; set; }

    public ICommand StartStreamingCommand { get; set; }

    public ICommand StopStreamingCommand { get; set; }

    public ImageSource CapturedImage
    {
      get => _capturedImage;
      set
      {
        _capturedImage = value;
        this.RaisePropertyChanged();
      }
    }

    [Reactive]
    public string ServiceName { get; set; }

    [Reactive] public bool CanStream { get; set; }

    [Reactive]
    public bool IsStreaming { get; set; }


    private MainWindowModel Model { get; set; }

    public void Cleanup()
    {
      _captureThreadShouldRun = false;
    }
  }
}

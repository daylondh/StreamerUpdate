using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System.Diagnostics;
using System.Threading.Tasks;

namespace StreamerUpdate.OBSInterop
{
  public class ObsRunner : ReactiveObject, IObsRunner
  {
    private Process _obsProcess = null;

    public void Go()
    {
      if (_obsProcess != null)
      {
        // I dunno.....
      }

      Task.Run(() =>
      {
        var psi = new ProcessStartInfo(@"C:\Program Files\obs-studio\bin\64bit\obs64.exe")
        {
          WorkingDirectory = @"C:\Program Files\obs-studio\bin\64bit\"
        };
        _obsProcess = new Process {StartInfo = psi};
        _obsProcess.Start();
        _obsProcess.Exited += ProcessExited;
        _obsProcess.EnableRaisingEvents = true;
        _obsProcess.WaitForExit();
      });
    }

    private void ProcessExited(object sender, System.EventArgs e)
    {
      HasExited = true;
    }

    public void Stop()
    {
      _obsProcess.CloseMainWindow();
      HasExited = true;
    }

    [Reactive]
    public bool HasExited { get; set; }
  }
}

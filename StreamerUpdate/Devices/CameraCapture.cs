using DirectShowLib;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;

namespace StreamerUpdate
{
  public class CaptureDevice
  {

    private VideoCapture _openedCapture;

    public List<DsDevice> GetDevices()
    {
      var videoDevices = new List<DsDevice>(DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice));
      return videoDevices.Where(d => !d.Name.Equals("OBS Virtual Camera")).ToList();
    }

    public Bitmap Capture(int index)
    {

      if (_openedCapture == null)
        OpenCapture(index);
      try
      {
        var frame = new Mat();
        var read = _openedCapture.Read(frame);
        if (!read)
          return null;
        var bmp = frame.ToBitmap();
        return bmp;
      }
      catch (Exception)
      {
        return null;
      }
    }

    private void OpenCapture(in int index)
    {
      var capture = new VideoCapture(index);
      capture.Open(index);
      if (capture.IsOpened())
        _openedCapture = capture;
    }
  }
}
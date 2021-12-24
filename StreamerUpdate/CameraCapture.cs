using DirectShowLib;
using OpenCvSharp;
using OpenCvSharp.Extensions;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Windows.Media.Imaging;

namespace StreamerUpdate
{

  public class CaptureDevice
  {
    public List<DsDevice> GetDevices()
    {
      var videoDevices = new List<DsDevice>(DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice));
      return videoDevices;
    }

    public BitmapImage Capture(int index)
    {
      var capture = new VideoCapture(index);
      capture.Open(index);
      if (!capture.IsOpened())
      {
        return null;
      }
      try
      {
        var frame = new Mat();
        var read = capture.Read(frame);
        if (!read)
          return null;
        var bmp = BitmapConverter.ToBitmap(frame);
        return Convert(bmp);
      }
      catch (Exception)
      {
        return null;
      }
    }

    private BitmapImage Convert(Bitmap bitmap)
    {
      using (MemoryStream memory = new MemoryStream())
      {
        bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
        memory.Position = 0;
        BitmapImage bitmapimage = new BitmapImage();
        bitmapimage.BeginInit();
        bitmapimage.StreamSource = memory;
        bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
        bitmapimage.EndInit();

        return bitmapimage;
      }
    }
  }
}

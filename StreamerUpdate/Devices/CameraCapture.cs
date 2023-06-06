using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Windows.Media.Imaging;
using DirectShowLib;
using OpenCvSharp;
using OpenCvSharp.Extensions;

namespace StreamerUpdate
{
    public class CaptureDevice
    {
        public List<DsDevice> GetDevices()
        {
            var videoDevices = new List<DsDevice>(DsDevice.GetDevicesOfCat(FilterCategory.VideoInputDevice));
            return videoDevices.Where(d => !d.Name.Equals("OBS Virtual Camera")).ToList();
        }

        public BitmapImage Capture(int index)
        {
            var capture = new VideoCapture(index);
            capture.Open(index);
            if (!capture.IsOpened()) return null;
            try
            {
                var frame = new Mat();
                var read = capture.Read(frame);
                if (!read)
                    return null;
                var bmp = frame.ToBitmap();
                return Convert(bmp);
            }
            catch (Exception)
            {
                return null;
            }
        }

        private BitmapImage Convert(Bitmap bitmap)
        {
            using (var memory = new MemoryStream())
            {
                bitmap.Save(memory, ImageFormat.Bmp);
                memory.Position = 0;
                var bitmapimage = new BitmapImage();
                bitmapimage.BeginInit();
                bitmapimage.StreamSource = memory;
                bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapimage.EndInit();
                bitmapimage.Freeze();

                return bitmapimage;
            }
        }
    }
}
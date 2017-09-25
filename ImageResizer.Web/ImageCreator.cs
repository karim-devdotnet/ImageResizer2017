using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web;

namespace ImageResizer.Web
{
    public class ImageCreator
    {

        public static byte[] ScaleImage(byte[] image, int width,int height, ImageFormat format)
        {
            Image result = null;
            try
            {
                MemoryStream imageStream = new MemoryStream(image);
                result = Image.FromStream(imageStream);
            }
            catch
            {
                //result = GetNoImage();
            }

            if (result == null) return image;

            var newImage = ScaleImage(result, width, height);
            using (MemoryStream ms = new MemoryStream())
            {
                newImage.Save(ms, format);
                return ms.GetBuffer();
            }
        }

        public static Image ScaleImage(Image image, int width, int height)
        {
            var ratioX = (double)width / image.Width;
            var ratioY = (double)height / image.Height;
            var ratio = Math.Min(ratioX, ratioY);

            var newWidth = (int)(image.Width * ratio);
            var newHeight = (int)(image.Height * ratio);
            var newImage = new Bitmap(newWidth, newHeight);
            using (var graphics = Graphics.FromImage(newImage))
            {
                graphics.DrawImage(image, 0, 0, newWidth, newHeight);
            }
            return newImage;
        }

        public static byte[] ResizeImage(byte[] image, int width, int height)
        {
            Image result = null;
            try
            {
                MemoryStream imageStream = new MemoryStream(image);
                result = Image.FromStream(imageStream);
            }
            catch
            {
                //result = GetNoImage();
            }

            int newWidth = result.Width;
            int newHeight = result.Height;
            if (width == 0) width = newWidth;
            if (height == 0) height = newHeight;

            if (newWidth > width || newHeight > width)
            {
                float ratioWidth = (float)width / result.Width;
                float ratioHeight = (float)height / result.Height;
                if (ratioWidth > ratioHeight)
                {
                    newWidth = width;
                    newHeight = Convert.ToInt32(Math.Floor(newHeight * ratioWidth));
                }
                else
                {
                    newHeight = width;
                    newWidth = Convert.ToInt32(Math.Floor(newWidth * ratioHeight));
                }
            }

            using (var bmp = new Bitmap(width, newHeight))
            {
                using (Graphics graphics = Graphics.FromImage(bmp))
                {
                    graphics.FillRectangle(Brushes.White, 0, 0, width, newHeight);
                    graphics.DrawImage(result, (float)(width - newWidth) / 2, (float)(0), newWidth, newHeight);
                }
                return GetBufferFromImage(bmp, ImageFormat.Jpeg, 80);
            }
        }

        private static byte[] GetBufferFromImage(Image image, ImageFormat format, long quality)
        {
            var ms = new MemoryStream();
            if (format == ImageFormat.Jpeg)
                image.Save(ms, GetEncoder(format), GetJpegEncoderParams(quality));
            else
                image.Save(ms, format);

            return ms.GetBuffer();
        }

        private static ImageCodecInfo GetEncoder(ImageFormat format)
        {
            var codecs = ImageCodecInfo.GetImageDecoders();
            return codecs.Where(x => x.FormatID == format.Guid).FirstOrDefault();
        }

        private static EncoderParameters GetJpegEncoderParams(long quality)
        {
            var encoderParams = new EncoderParameters(1);
            encoderParams.Param[0] = new EncoderParameter(Encoder.Quality, quality);
            return encoderParams;
        }
    }
}
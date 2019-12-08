using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;
using System.Drawing;

namespace UtilitiesLib {

    /// <summary>
    /// Helper functions
    /// </summary>
    public static class HelperMethods {

        /// <summary>
        /// Find the file or folder name from a full path
        /// </summary>
        /// <param name="path">The full path</param>
        /// <returns></returns>
        public static string GetFolderName(string path) {
            // If we have no path, return empty
            if (string.IsNullOrEmpty(path)) {
                return string.Empty;
            }

            // Make all slashes back slashes
            string normalizedPath = path.Replace('/', '\\');

            // Find the last backslash in the path
            int lastIndex = normalizedPath.LastIndexOf('\\');

            // If we don't find a backslash, return the path itself
            if (lastIndex <= 0) {
                return path;
            }

            // Return the name after the last back slash
            return path.Substring(lastIndex + 1);
        }

        /// <summary>
        /// Convert a bitmap image to bitmapimage.
        /// The first is the resource file and the latter is as display in wpf
        /// </summary>
        /// <param name="bitmap"></param>
        /// <returns></returns>
        public static BitmapImage BitmapToBitmapImage(Bitmap bitmap) {
            BitmapImage bitmapImage;
            // Save image in memory and convert it to bitmap image...
            using (MemoryStream memory = new MemoryStream()) {
                bitmap.Save(memory, System.Drawing.Imaging.ImageFormat.Bmp);
                memory.Position = 0;
                bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }

        public static BitmapImage CreateThumbNail(string filePathName, int width, int height, int maxWidth, int maxHeight) {
            
            // Create source
            BitmapImage bitmapImage = new BitmapImage();

            // BitmapImage.UriSource must be in a BeginInit/EndInit block
            bitmapImage.BeginInit();
            bitmapImage.UriSource = new Uri(filePathName, UriKind.Absolute);

            if (maxWidth / width > maxHeight / height) {
                bitmapImage.DecodePixelWidth = maxWidth;
            } else {
                bitmapImage.DecodePixelHeight = maxHeight;

            }
            bitmapImage.EndInit();

            return bitmapImage;
        }

        /// <summary>
        /// Convert bitmapimage to byte to store in database
        /// </summary>
        /// <param name="bitmapImage"></param>
        /// <returns></returns>
        public static byte[] BitmapImageToByte(BitmapImage bitmapImage) {
            byte[] data;
            JpegBitmapEncoder encoder = new JpegBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
            using (MemoryStream ms = new MemoryStream()) {
                encoder.Save(ms);
                data = ms.ToArray();
            }
            return data;
        }

        public static BitmapImage BitmapImageFromByte(byte[] data) {
            BitmapImage bitmapImage;
            // Save image in memory and convert it to bitmap image...
            using (MemoryStream memory = new MemoryStream(data)) {
                bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memory;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }
            return bitmapImage;
        }

    }

}

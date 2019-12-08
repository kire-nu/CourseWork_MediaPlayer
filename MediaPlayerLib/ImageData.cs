using System.Windows.Media.Imaging;
using UtilitiesLib;
using MediaPlayerDataAccess;
using System.Drawing;

namespace MediaPlayerLib {
    /// <summary>
    /// Image data
    /// Has further information of image
    /// </summary>
    public class ImageData : MediaData {

        private int height;
        private int width;
        private int duration = 10;
        private bool thumbNailImageLoaded = false;
        private byte[] thumbNailImage;

        /// <summary>
        /// For images in slideshow
        /// </summary>
        /// <param name="filePathName"></param>
        /// <param name="height"></param>
        /// <param name="width"></param>
        /// <param name="duration"></param>
        /// <param name="order"></param>
        /// <param name="inSlideShow"></param>
        public ImageData(string filePathName, int height, int width, int duration, int order, bool inSlideShow) : base(filePathName) {
            this.thumbNailImage = HelperMethods.BitmapImageToByte(HelperMethods.BitmapToBitmapImage(MediaPlayerLib.Properties.Resources.ImageThumbNail));
            this.height = height;
            this.width = width;
            this.duration = duration;
            base.order = order;
            if (inSlideShow) {
                //Update database
                MediaDataMapper mediaDataDataContent = new MediaDataMapper() {
                    Duration = this.Duration,
                    Description = this.Description,
                    Order = this.Order,
                    FilePathName = this.FilePathName,
                    MediaDataType = MediaDataType.Image,
                    ThumbNailImage = this.ThumbNailImage
                };
                this.id = MediaDataQuery.NewMediaData(mediaDataDataContent);
            } else {
                this.id = -1;
            }
        }

        /// <summary>
        /// For images in folder
        /// </summary>
        /// <param name="filePathName"></param>
        /// <param name="inSlideShow"></param>
        public ImageData(string filePathName, bool inSlideShow) : base(filePathName) {
            this.thumbNailImage = HelperMethods.BitmapImageToByte(HelperMethods.BitmapToBitmapImage(MediaPlayerLib.Properties.Resources.ImageThumbNail));
            this.id = -1;

        }

        /// <summary>
        /// Constructor used to restore from database
        /// </summary>
        /// <param name="id"></param>
        public ImageData(int id) {
            MediaDataMapper mediaDataDataContent = MediaDataQuery.GetMediaData(id);
            base.id = id;
            this.filePathName = mediaDataDataContent.FilePathName;
            this.order = mediaDataDataContent.Order;
            this.description = mediaDataDataContent.Description;
            this.thumbNailImage = mediaDataDataContent.ThumbNailImage;
            this.duration = mediaDataDataContent.Duration;
            this.thumbNailImageLoaded = true;
            try {
                Image image = Image.FromFile(this.filePathName);
                this.width = image.Width;
                this.height = image.Height;
            } catch {
                this.width = 0;
                this.height = 0;
            }
        }

        public int Height { get => height; set => height = value; }
        public int Width { get => width; set => width = value; }
        public int Duration { get => duration; set => duration = value; }
        public string Dimensions { get { return string.Concat(width.ToString(), "x", height.ToString()); } }

        public bool ThumbNailImageLoaded { get => thumbNailImageLoaded; }

        /// <summary>
        /// Set thumbnail, replaces the temporary after intial loading
        /// </summary>
        public byte[] ThumbNailImage {
            get => thumbNailImage;
            set {
                thumbNailImage = value;
                thumbNailImageLoaded = true;
                if (this.id >= 0) {
                    //Update database
                    MediaDataQuery.UpdateThumbNail(this.id, this.thumbNailImage);
                }
            }
        }
    }
}

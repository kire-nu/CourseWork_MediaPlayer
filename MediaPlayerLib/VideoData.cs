using System.Windows.Media.Imaging;
using UtilitiesLib;
using MediaPlayerDataAccess;

namespace MediaPlayerLib {
    /// <summary>
    /// Video data
    /// Uses a generic thumbnail image and has no dimensions be default
    /// </summary>
    public class VideoData : MediaData {

        private byte[] thumbNailImage;

        /// <summary>
        /// For video in slideshow
        /// </summary>
        /// <param name="filePathName"></param>
        /// <param name="order"></param>
        /// <param name="inSlideShow"></param>
        public VideoData(string filePathName, int order, bool inSlideShow) : base(filePathName) {
            this.thumbNailImage = HelperMethods.BitmapImageToByte(HelperMethods.BitmapToBitmapImage(MediaPlayerLib.Properties.Resources.VideoThumbNail));
            base.order = order;
            if (inSlideShow) {
                //Update database
                MediaDataMapper mediaDataDataContent = new MediaDataMapper() {
                    Duration = -1,
                    Description = this.Description,
                    Order = this.Order,
                    FilePathName = this.FilePathName,
                    MediaDataType = MediaDataType.Video,
                    ThumbNailImage = this.ThumbNailImage
                };
                this.id = MediaDataQuery.NewMediaData(mediaDataDataContent);
            } else {
                this.id = -1;
            }
        }

        /// <summary>
        /// For video in folder
        /// </summary>
        /// <param name="filePathName"></param>
        /// <param name="inSlideShow"></param>
        public VideoData(string filePathName, bool inSlideShow) : base(filePathName) {
            this.thumbNailImage = HelperMethods.BitmapImageToByte(HelperMethods.BitmapToBitmapImage(MediaPlayerLib.Properties.Resources.VideoThumbNail));
            this.id = -1;
        }

        /// <summary>
        /// Constructor used to restore from database
        /// </summary>
        /// <param name="id"></param>
        public VideoData(int id) {
            MediaDataMapper mediaDataDataContent = MediaDataQuery.GetMediaData(id);
            this.id = id;
            this.filePathName = mediaDataDataContent.FilePathName;
            this.order = mediaDataDataContent.Order;
            this.description = mediaDataDataContent.Description;
            this.thumbNailImage = mediaDataDataContent.ThumbNailImage;
        }


        public byte[] ThumbNailImage {
            get => thumbNailImage;
            set {
                thumbNailImage = value;
            }
        }

        public string Dimensions {
            get { return "Video"; }
        }
    }
}

using System;
using System.IO;
using MediaPlayerDataAccess;

namespace MediaPlayerLib {
    /// <summary>
    /// MediaData, base class for ImageData and VideoData
    /// As general attributes such as ID, file, order and description
    /// </summary>
    [Serializable]
    public abstract class MediaData : IMediaData {

        protected int id;
        protected string filePathName;
        protected int order;
        protected string description = string.Empty;

        public MediaData(string filePathName) {
            this.filePathName = filePathName;
        }

        protected MediaData() { }

        protected MediaData(int id, string filePathName, int order, string description) {
            this.id = id;
            this.filePathName = filePathName;
            this.order = order;
            this.description = description;

        }

        public int Id { get => id; }
        public string FilePathName { get => filePathName; }
        public string FileName { get => Path.GetFileName(filePathName); }
        public int Order {
            get { return order; }
            set {
                order = value;
                if (this.id >= 0) {
                    //Update database
                    MediaDataQuery.UpdateOrder(this.id, value);
                }
            }
        }
        public string Description {
            get { return description; }
            set {
                description = value;
                if (this.id >= 0) {
                    //Update database
                    MediaDataQuery.UpdateDescription(this.id, value);
                }
            }
        }
    }
}

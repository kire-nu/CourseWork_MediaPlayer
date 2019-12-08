using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using MediaPlayerDataAccess;

namespace MediaPlayerLib {
    /// <summary>
    /// Slideshow, which has a collection of mediaData (either image or video)
    /// </summary>
    public class SlideShow {

        private int id;
        private String name;
        private ObservableCollection<MediaData> mediaDataCollection;

        public SlideShow(string name) {
            this.name = name;
            this.id = SlideShowQuery.NewSlideShow(name);
            mediaDataCollection = new ObservableCollection<MediaData>();
        }

        /// <summary>
        /// Constructor used to restore from database
        /// </summary>
        /// <param name="id"></param>
        public SlideShow(int id) {
            this.id = id;
            SlideShowMapper slideShowDataContent = SlideShowQuery.GetSlideShow(id);
            this.name = slideShowDataContent.Name;
            // Get mediadata and restore those
            ObservableCollection<MediaData> unsortedMediaDataCollection = new ObservableCollection<MediaData>();
            Dictionary<int, MediaDataType> mediaDatas = MediaDataQuery.GetMediaDatas(this.id);
            foreach (var item in mediaDatas) {
                MediaData mediaData = null;
                if (item.Value == MediaDataType.Image) {
                    mediaData = new ImageData(item.Key);
                }
                if (item.Value == MediaDataType.Video) {
                    mediaData = new VideoData(item.Key);
                }
                if (mediaData != null) {
                    unsortedMediaDataCollection.Add(mediaData);
                }
            }
            if (unsortedMediaDataCollection.Count > 0) {
                var sortedMediaDataCollection = unsortedMediaDataCollection.OrderBy(x => x.Order);
                mediaDataCollection = new ObservableCollection<MediaData>(sortedMediaDataCollection);
            } else {
                mediaDataCollection = new ObservableCollection<MediaData>();
            }

        }

        public string Name {
            get { return name; }
            set { name = value;
                SlideShowQuery.UpdateName(id, value);
            }
        }
        public int Id { get => id; }
        public ObservableCollection<MediaData> MediaData { get => mediaDataCollection; set => mediaDataCollection = value; }
        public int Count { get => mediaDataCollection.Count; }

        public virtual Album Album { get; set; }

        public void Add(MediaData mediaData) {
            this.mediaDataCollection.Add(mediaData);
            //Update database
            SlideShowQuery.AddMediaData(this.id, mediaData.Id);
        }

        public void Insert(int index, MediaData mediaData) {
            if (ValidateIndex(index)) {
                mediaDataCollection.Insert(index, mediaData);
                //Update database
                SlideShowQuery.AddMediaData(this.id, mediaData.Id);
            }
        }

        public void RemoveAt(int index) {
            if (ValidateIndex(index)) {
                //Remove from database first before removing item
                MediaData mediaData = mediaDataCollection[index];
                SlideShowQuery.RemoveMediaData(this.id, mediaData.Id);
                MediaDataQuery.RemoveMediaData(mediaData.Id);
                mediaDataCollection.RemoveAt(index);
            }
        }

        public void Move(int oldIndex, int newIndex) {
            if ((ValidateIndex(oldIndex)) && ValidateIndex(newIndex)) {
                mediaDataCollection.Move(oldIndex, newIndex);
            }
        }

        public void ReOrder() {
            for (int i = 0; i < mediaDataCollection.Count; i++) {
                mediaDataCollection.ElementAt(i).Order = i;
            }
        }

        public MediaData GetAt(int index) {
            if (ValidateIndex(index)) {
                return mediaDataCollection[index];
            } else {
                return null;
            }
        }

        public int GetDuration(int index) {
            if (ValidateIndex(index)) {
                if (mediaDataCollection[index] is ImageData) {
                    return ((ImageData)mediaDataCollection[index]).Duration;
                }
            }
            return -1;
        }

        public string GetDescription(int index) {
            if (ValidateIndex(index)) {
                return mediaDataCollection[index].Description;
            } else {
                return string.Empty;
            }
        }

        public void SetDuration(int index, int duration) {
            if (ValidateIndex(index)) {
                if (mediaDataCollection[index] is ImageData) {
                    ((ImageData)mediaDataCollection[index]).Duration = duration;
                }
            }
        }

        public void SetDescription(int index, string description) {
            if (ValidateIndex(index)) {
                mediaDataCollection[index].Description = description;
            } 
        }

        private bool ValidateIndex(int index) {
            if ((index>=0) & (index<mediaDataCollection.Count)) {
                return true;
            }
            return false;
        }

        public List<MediaData> GetMediaDataInOrder() {
            MediaData[] sortedMediaData = new MediaData[mediaDataCollection.Count];
            for (int i = 0; i < mediaDataCollection.Count; i++) {
                sortedMediaData[mediaDataCollection[i].Order] = mediaDataCollection[i];
            }
            return sortedMediaData.ToList();
        }
    }
}

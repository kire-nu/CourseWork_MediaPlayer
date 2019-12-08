using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using MediaPlayerDataAccess;

namespace MediaPlayerLib {
    /// <summary>
    /// Album, which has a collection of slideshows
    /// </summary>
    public class Album {

        private int id;
        private String title;
        private DateTime lastUsed;
        private ObservableCollection<SlideShow> slideShows;

        public Album() : this("Untitled") { }

        public Album(string title) {
            this.title = title;
            this.slideShows = new ObservableCollection<SlideShow>();
            this.id = AlbumQuery.NewAlbum(title, out lastUsed);

        }

        /// <summary>
        /// Constructor used to restore from database
        /// </summary>
        /// <param name="id"></param>
        public Album(int id) {
            AlbumMapper albumDataContent = AlbumQuery.GetAlbum(id);
            this.id = id;
            this.title = albumDataContent.Name;
            this.lastUsed = albumDataContent.LastUsed;
            this.slideShows = new ObservableCollection<SlideShow>();
            //Get slideshows and restrore those
            List<int> slideShowIds = SlideShowQuery.GetSlideShows(id);
            foreach (int slideShowId in slideShowIds) {
                SlideShow slideShow = new SlideShow(slideShowId);
                this.slideShows.Add(slideShow);
            }
        }

        public int Id { get => id; }
        public string Title {
            get { return title; }
            set {
                title = value;
                //Update database
                AlbumQuery.UpdateName(id, title);

            }
        }
        public ObservableCollection<SlideShow> SlideShows { get => slideShows; }
        public DateTime LastUsed { get { return AlbumQuery.GetLastUsedTime(id); } }
        public int Count { get => slideShows.Count; }

        public void Add(SlideShow slideShow) {
            this.slideShows.Add(slideShow);
            //Update database
            AlbumQuery.AddSlideShow(id, slideShow.Id);
            
        }

        public void RemoveAt(int index) {
            if (ValidateIndex(index)) {
                int slideShowId = slideShows[index].Id;
                //Remove from database first, before removing item
                AlbumQuery.RemoveSlideShow(this.id, slideShowId);
                SlideShowQuery.RemoveSlideShow(slideShowId);
                slideShows.RemoveAt(index);
            }
        }
        
        public SlideShow GetAt(int index) {
            if (ValidateIndex(index)) {
                return slideShows[index];
            } else {
                return null;
            }
        }

        private bool ValidateIndex(int index) {
            if ((index >= 0) & (index < slideShows.Count)) {
                return true;
            }
            return false;
        }

    }
}

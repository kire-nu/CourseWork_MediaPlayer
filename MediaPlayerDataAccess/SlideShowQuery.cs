using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayerDataAccess {
    /// <summary>
    /// Queries to retive data from/for slideshow class
    /// </summary>
    public static class SlideShowQuery {

        /// <summary>
        /// Add slideshow to table
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int NewSlideShow(string name) {
            SlideShowMapper slideShowMapper = new SlideShowMapper(name);
            int id = -1;
            try {
                using (var db = new MediaPlayerContext()) {
                    // Get id (max+1) and add to table
                    id = db.SlideShowMapper.Select(x => x.Id).DefaultIfEmpty(0).Max() + 1;
                    slideShowMapper.Id = id;
                    db.SlideShowMapper.Add(slideShowMapper);
                    db.SaveChanges();
                }
            } catch {

            }
            return id;
        }

        /// <summary>
        /// Remove slideshow
        /// </summary>
        /// <param name="id"></param>
        public static void RemoveSlideShow(int id) {
            try {
                using (var db = new MediaPlayerContext()) {
                    SlideShowMapper slideShowMapper = db.SlideShowMapper.Single(x => x.Id == id);
                    db.SlideShowMapper.Remove(slideShowMapper);
                    db.SaveChanges();
                }
            } catch {

            }
        }

        /// <summary>
        /// Update name of slideshow
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public static void UpdateName(int id, string name) {
            try {
                SlideShowMapper slideShowMapper;
                using (var db = new MediaPlayerContext()) {
                    // Get data and update field
                    slideShowMapper = db.SlideShowMapper.Single(x => x.Id == id);
                    slideShowMapper.Name = name;
                    db.SaveChanges();
                }
                AlbumQuery.UpdateLastUsed(slideShowMapper.AlbumMapper.Id);
            } catch {

            }
        }

        /// <summary>
        /// Retrive slideshow data
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static SlideShowMapper GetSlideShow(int id) {
            SlideShowMapper slideShowMapper;
            try {
                using (var db = new MediaPlayerContext()) {
                    slideShowMapper = db.SlideShowMapper.Single(x => x.Id == id);
                }
                return slideShowMapper;
            } catch {
                return null;
            }
        }

        /// <summary>
        /// Retrive all slideshow ids for a album
        /// </summary>
        /// <param name="albumId"></param>
        /// <returns></returns>
        public static List<int> GetSlideShows(int albumId) {
            List<int> ints = new List<int>();
            try {
                using (var db = new MediaPlayerContext()) {
                    // Get list from album
                    AlbumMapper albumMapper = db.AlbumMapper.Single(x => x.Id == albumId);
                    List<SlideShowMapper> slideShowMappers = db.SlideShowMapper.Where(x => x.AlbumMapper.Id == albumId).ToList();
                    // Return as ids
                    if (slideShowMappers.Count > 0) {
                        foreach (SlideShowMapper slideShowMapper in slideShowMappers) {
                            ints.Add(slideShowMapper.Id);
                        }
                    }
                }
            } catch {

            }
            return ints;
        }

        /// <summary>
        /// Add mediadata
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mediaDataId"></param>
        public static void AddMediaData(int id, int mediaDataId) {
            int albumId = 0;
            try {
                using (var db = new MediaPlayerContext()) {
                    // Get data and update field
                    MediaDataMapper mediaDataMapper = db.MediaDataMapper.Single(x => x.Id == mediaDataId);
                    SlideShowMapper slideShowMapper = db.SlideShowMapper.Single(x => x.Id == id);
                    slideShowMapper.MediaDataMappers.Add(mediaDataMapper);
                    albumId = slideShowMapper.AlbumMapper.Id;
                    db.SaveChanges();
                }
            } catch {

            }
            AlbumQuery.UpdateLastUsed(albumId);
        }

        /// <summary>
        /// Remove mediadata
        /// </summary>
        /// <param name="id"></param>
        /// <param name="mediaDataId"></param>
        public static void RemoveMediaData(int id, int mediaDataId) {
            try {
                int albumId = 0;
                using (var db = new MediaPlayerContext()) {
                    // Get data and update field
                    MediaDataMapper mediaDataMapper = db.MediaDataMapper.Single(x => x.Id == mediaDataId);
                    SlideShowMapper slideShowMapper = db.SlideShowMapper.Single(x => x.Id == id);
                    slideShowMapper.MediaDataMappers.Remove(mediaDataMapper);
                    albumId = slideShowMapper.AlbumMapper.Id;
                    db.SaveChanges();
                }
                AlbumQuery.UpdateLastUsed(albumId);
            } catch {

            }
        }
    }
}

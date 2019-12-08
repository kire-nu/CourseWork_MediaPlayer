using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity.Migrations;

namespace MediaPlayerDataAccess {
    /// <summary>
    /// Queries to retive data from/for album class
    /// </summary>
    public class AlbumQuery {

        /// <summary>
        /// Return last used time of album
        /// Allow sorting albums by last used
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static DateTime GetLastUsedTime(int id) {
            DateTime lastUsed;
            try {
                using (var db = new MediaPlayerContext()) {
                    AlbumMapper albumMapper = db.AlbumMapper.Single(x => x.Id == id);
                    lastUsed = albumMapper.LastUsed;
                }
            } catch {
            }
            return DateTime.MinValue;
        }

        /// <summary>
        /// Returns the key information of the last used album
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public static void GetLastUsed(ref int id, ref string name) {
            AlbumMapper albumMapper;
            try {
                using (var db = new MediaPlayerContext()) {
                    // Order by last used, and set to current time (as it is being used now)
                    albumMapper = db.AlbumMapper.OrderBy(x => x.LastUsed).FirstOrDefault();
                    albumMapper.LastUsed = DateTime.Now;
                    db.SaveChanges();
                }
                if (albumMapper != null) {
                    id = albumMapper.Id;
                    name = albumMapper.Name;
                    return;
                }
            } catch {

            }
            id = -1;
            name = string.Empty;
        }


        /// <summary>
        /// Add album to table
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public static int NewAlbum(string name, out DateTime lastUsed) {
            // create new entity
            AlbumMapper albumMapper = new AlbumMapper(name);
            int id = 0;
            try {
                using (var db = new MediaPlayerContext()) {
                    // Get new id (max)+1 and set parameteres
                    id = db.AlbumMapper.Select(x => x.Id).DefaultIfEmpty(0).Max() + 1;
                    albumMapper.Id = id;
                    lastUsed = DateTime.Now;
                    albumMapper.LastUsed = lastUsed;
                    db.AlbumMapper.Add(albumMapper);
                    db.SaveChanges();
                }
            } catch {
                lastUsed = DateTime.MinValue;
            }
            return id;
        }

        /// <summary>
        /// Remove album from table
        /// </summary>
        /// <param name="id"></param>
        public static void RemoveAlbum(int id) {
            try {
                using (var db = new MediaPlayerContext()) {
                    AlbumMapper albumMapper = db.AlbumMapper.Single(x => x.Id == id);
                    db.AlbumMapper.Remove(albumMapper);
                    db.SaveChanges();
                }
            } catch {

            }
        }

        /// <summary>
        /// Update name of album
        /// </summary>
        /// <param name="id"></param>
        /// <param name="name"></param>
        public static void UpdateName(int id, string name) {
            try {
                using (var db = new MediaPlayerContext()) {
                    // Get data and update field
                    AlbumMapper albumMapper = db.AlbumMapper.Single(x => x.Id == id);
                    albumMapper.Name = name;
                    albumMapper.LastUsed = DateTime.Now;
                    db.SaveChanges();
                }
            } catch {

            }
        }

        /// <summary>
        /// Retrive album data
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static AlbumMapper GetAlbum(int id) {
            AlbumMapper albumMapper;
            try {
                using (var db = new MediaPlayerContext()) {
                    albumMapper = db.AlbumMapper.Single(x => x.Id == id);
                }
                return albumMapper;
            } catch {
                return null;
            }
        }

        /// <summary>
        /// Get all albums ids
        /// </summary>
        /// <returns></returns>
        public static List<int> GetAlbums() {
            List<int> ints = new List<int>();
            try {
                using (var db = new MediaPlayerContext()) {
                    List<AlbumMapper> albumMappers = db.AlbumMapper.ToList();
                    if (albumMappers.Count > 0) {
                        for (int i = 0; i < albumMappers.Count; i++) {
                            ints.Add(albumMappers[i].Id);
                        }
                    }
                }
            } catch {

            }
            return ints;
        }

        /// <summary>
        /// Add slideshow to album
        /// </summary>
        /// <param name="id"></param>
        /// <param name="slideShowId"></param>
        public static void AddSlideShow(int id, int slideShowId) {
            try {
                using (var db = new MediaPlayerContext()) {
                    // Get data and update field
                    SlideShowMapper slideShowMapper = db.SlideShowMapper.Single(x => x.Id == slideShowId);
                    AlbumMapper albumMapper = db.AlbumMapper.Single(x => x.Id == id);
                    albumMapper.SlideShowMappers.Add(slideShowMapper);
                    albumMapper.LastUsed = DateTime.Now;
                    db.SaveChanges();
                }
            } catch {

            }
        }

        /// <summary>
        /// Remove slideshow from album
        /// </summary>
        /// <param name="id"></param>
        /// <param name="slideShowId"></param>
        public static void RemoveSlideShow(int id, int slideShowId) {
            try {
                using (var db = new MediaPlayerContext()) {
                    // Get data and update field
                    SlideShowMapper slideShowMapper = db.SlideShowMapper.Single(x => x.Id == slideShowId);
                    AlbumMapper albumMapper = db.AlbumMapper.Single(x => x.Id == id);
                    albumMapper.SlideShowMappers.Remove(slideShowMapper);
                    albumMapper.LastUsed = DateTime.Now;
                    db.SaveChanges();
                }
            } catch {

            }
        }

        /// <summary>
        /// Update last used time
        /// </summary>
        /// <param name="id"></param>
        public static void UpdateLastUsed(int id) {
            try {
                using (var db = new MediaPlayerContext()) {
                    AlbumMapper albumMapper = db.AlbumMapper.Single(x => x.Id == id);
                    albumMapper.LastUsed = DateTime.Now;
                    db.SaveChanges();
                }
            } catch {

            }
        }

    }
}

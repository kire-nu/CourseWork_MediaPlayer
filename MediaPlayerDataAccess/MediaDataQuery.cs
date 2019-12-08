using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayerDataAccess {
    /// <summary>
    /// Queries to retive data from/for mediadata class
    /// </summary>
    public class MediaDataQuery {

        /// <summary>
        /// Retrive data
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public static MediaDataMapper GetMediaData(int id) {
            MediaDataMapper mediaDataMapper;
            try {
                using (var db = new MediaPlayerContext()) {
                    mediaDataMapper = db.MediaDataMapper.Single(x => x.Id == id);
                }
                return mediaDataMapper;
            } catch {
                return null;
            }
        }

        /// <summary>
        /// Retrive all ids and media data type
        /// </summary>
        /// <param name="slideShowId"></param>
        /// <returns></returns>
        public static Dictionary<int, MediaDataType> GetMediaDatas(int slideShowId) {
            // Create dictonary with id and corresponding mediadata type
            Dictionary<int, MediaDataType> mediaData = new Dictionary<int, MediaDataType>();
            try {
                using (var db = new MediaPlayerContext()) {
                    List<MediaDataMapper> mediaDataMappers = db.MediaDataMapper.Where(x => x.SlideShowMapper.Id == slideShowId).ToList();
                    foreach (MediaDataMapper mediaDataMapper in mediaDataMappers) {
                        mediaData.Add(mediaDataMapper.Id, mediaDataMapper.MediaDataType);
                    }
                }
            } catch {

            }
            return mediaData;
        }

        /// <summary>
        /// Add new entry
        /// </summary>
        /// <param name="mediaDataMapper"></param>
        /// <returns></returns>
        public static int NewMediaData(MediaDataMapper mediaDataMapper) {
            int id = -1;
            try {
                using (var db = new MediaPlayerContext()) {
                    // Get id (max+1) and add to table
                    id = db.MediaDataMapper.Select(x => x.Id).DefaultIfEmpty(0).Max() + 1;
                    mediaDataMapper.Id = id;
                    db.MediaDataMapper.Add(mediaDataMapper);
                    db.SaveChanges();
                }
            } catch {

            }
            return id;
        }

        /// <summary>
        /// Remove entry
        /// </summary>
        /// <param name="id"></param>
        public static void RemoveMediaData(int id) {
            try {
                using (var db = new MediaPlayerContext()) {
                    MediaDataMapper mediaDataMapper = db.MediaDataMapper.Single(x => x.Id == id);
                    db.MediaDataMapper.Remove(mediaDataMapper);
                    db.SaveChanges();
                }
            } catch {

            }
        }

        /// <summary>
        /// Change order field
        /// </summary>
        /// <param name="id"></param>
        /// <param name="order"></param>
        public static void UpdateOrder(int id, int order) {
            int albumId = 0;
            try {
                using (var db = new MediaPlayerContext()) {
                    // Get data and update field
                    MediaDataMapper mediaDataMapper = db.MediaDataMapper.Single(x => x.Id == id);
                    mediaDataMapper.Order = order;
                    SlideShowMapper slideShowMapper = mediaDataMapper.SlideShowMapper;
                    if (slideShowMapper != null) {
                        albumId = slideShowMapper.AlbumMapper.Id;
                    }
                    db.SaveChanges();
                }
                if (albumId >= 0) {
                    AlbumQuery.UpdateLastUsed(albumId);
                }
            } catch {

            }
        }

        /// <summary>
        /// Update thumbnail image
        /// </summary>
        /// <param name="id"></param>
        /// <param name="thumbNailImage"></param>
        public static void UpdateThumbNail(int id, byte[] thumbNailImage) {
            int albumId = -1;
            try {
                using (var db = new MediaPlayerContext()) {
                    // Get data and update field
                    MediaDataMapper mediaDataMapper = db.MediaDataMapper.Single(x => x.Id == id);
                    mediaDataMapper.ThumbNailImage = thumbNailImage;
                    SlideShowMapper slideShowMapper = mediaDataMapper.SlideShowMapper;
                    if (slideShowMapper != null) {
                        albumId = slideShowMapper.AlbumMapper.Id;
                    }
                    db.SaveChanges();
                }
                if (albumId >= 0) {
                    AlbumQuery.UpdateLastUsed(albumId);
                }
            } catch {

            }
        }

        /// <summary>
        /// Change description field
        /// </summary>
        /// <param name="id"></param>
        /// <param name="description"></param>
        public static void UpdateDescription(int id, string description) {
            int albumId = 0;
            try {
                using (var db = new MediaPlayerContext()) {
                    // Get data and update field
                    MediaDataMapper mediaDataMapper = db.MediaDataMapper.Single(x => x.Id == id);
                    mediaDataMapper.Description = description;
                    SlideShowMapper slideShowMapper = mediaDataMapper.SlideShowMapper;
                    if (slideShowMapper != null) {
                        albumId = slideShowMapper.AlbumMapper.Id;
                    }
                    db.SaveChanges();
                }
                if (albumId >= 0) {
                    AlbumQuery.UpdateLastUsed(albumId);
                }
            } catch {

            }
        }

    }
}

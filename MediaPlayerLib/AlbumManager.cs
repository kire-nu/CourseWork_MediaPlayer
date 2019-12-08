using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaPlayerDataAccess;

namespace MediaPlayerLib {
    /// <summary>
    /// Contains information about all albums
    /// </summary>
    public class AlbumManager {

        private ObservableCollection<Album> albums;

        public AlbumManager() {
            albums = new ObservableCollection<Album>();

            //Retrive albums from database
            List<int> ints = AlbumQuery.GetAlbums();
            if (ints.Count>0) {
                for (int i = 0; i < ints.Count; i++) {
                    // Re-create albums and add to list
                    Album album = new Album(ints[i]);
                    albums.Add(album);
                }
            }
        }

        /// <summary>
        /// Get last used album (when starting program)
        /// </summary>
        /// <returns></returns>
        public Album GetLastAlbum() {
            if (albums.Count>0) {
                Album album = albums.OrderByDescending(x => x.LastUsed).FirstOrDefault();
                return album;
            }
            return null;
        }

        /// <summary>
        /// Get list of albums ordered by last used
        /// </summary>
        /// <returns></returns>
        public ObservableCollection<Album> GetAlbums() {
            if (albums.Count > 0) { 
                var result = albums.OrderByDescending(x => x.LastUsed);
                return new ObservableCollection<Album>(result);
            }
            return null;
        }

        /// <summary>
        /// Retrive album
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public Album GetAlbum(int id) {
            foreach (Album album in albums) {
                if (album.Id == id) {
                    return album;
                }
            }
            return null;
        }

        /// <summary>
        /// Add album
        /// </summary>
        /// <param name="album"></param>
        public void Add(Album album) {
            albums.Add(album);
        }

        /// <summary>
        /// Remove album
        /// </summary>
        /// <param name="id"></param>
        public void RemoveAlbum(int id) {
            for (int i = 0; i < albums.Count; i++) { 
                if (albums[i].Id == id) {
                    // Remove from database first, then from list
                    AlbumQuery.RemoveAlbum(albums[i].Id);
                    albums.RemoveAt(i);
                }
            }
        }
    }
}

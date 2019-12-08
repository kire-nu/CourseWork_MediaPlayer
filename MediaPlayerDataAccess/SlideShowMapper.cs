using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayerDataAccess {
    /// <summary>
    /// Slideshow class to store data in database
    /// </summary>
    public class SlideShowMapper {
        
        public int Id { get; set; }
        public string Name { get; set; }
        public virtual List<MediaDataMapper> MediaDataMappers { get; set; }
        public virtual AlbumMapper AlbumMapper { get; set; }

        public SlideShowMapper(string name) {
            Name = name;
        }
        public SlideShowMapper() { }
    }
}

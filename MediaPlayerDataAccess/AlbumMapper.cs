using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayerDataAccess {
    /// <summary>
    /// Album class to store data in database
    /// </summary>
    public class AlbumMapper {

        public int Id { get; set; }
        public string Name { get; set; }
        public DateTime LastUsed { get; set; }

        public virtual List<SlideShowMapper> SlideShowMappers { get; set; }

        public AlbumMapper(string name) {
            Name = name;
        }
        public AlbumMapper() { }
    }
}

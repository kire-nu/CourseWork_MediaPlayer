using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MediaPlayerDataAccess {
    /// <summary>
    /// MediaData class to store data in database
    /// </summary>
    public class MediaDataMapper {

        public int Id { get; set; }
        public string FilePathName { get; set; }
        public int Order { get; set; }
        public string Description { get; set; }
        public int Duration { get; set; }
        public byte[] ThumbNailImage { get; set; }
        public MediaDataType MediaDataType { get; set; }

        public virtual SlideShowMapper SlideShowMapper { get; set; }
    }
}

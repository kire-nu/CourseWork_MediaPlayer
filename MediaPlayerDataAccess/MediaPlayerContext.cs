using System.Data.Entity;

namespace MediaPlayerDataAccess {
    class MediaPlayerContext : DbContext {


        public DbSet<AlbumMapper> AlbumMapper { get; set; }
        public DbSet<SlideShowMapper> SlideShowMapper { get; set; }
        public DbSet<MediaDataMapper> MediaDataMapper { get; set; }

        public MediaPlayerContext() : base("name=MediaPlayerContext") { }
        //public MediaPlayerContext() { }

    }
}

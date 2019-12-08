
namespace MediaPlayerLib {
    interface IMediaData {


        int Id {
            get;
        }
        string FilePathName {
            get;
        }
        string FileName {
            get;
        }
        int Order {
            get; set;
        }
        string Description {
            get; set;
        }



    }
}

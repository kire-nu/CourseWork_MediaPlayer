using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MediaPlayerLib;

namespace MediaPlayer {
    /// <summary>
    /// Interaction logic for Window1.xaml
    /// </summary>
    public partial class RestoreAlbum : Window {

        private int albumId = -1;

        public RestoreAlbum(ObservableCollection<Album> albums) {
            InitializeComponent();
            listBoxAlbums.ItemsSource = albums;
        }

        public int AlbumId { get { return albumId; } }

        private void listBox_MouseDoubleClick(object sender, MouseButtonEventArgs e) {
            Album album = (Album)listBoxAlbums.SelectedItem;
            this.albumId = album.Id;
            this.DialogResult = true;
        }
    }
}

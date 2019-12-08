using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;
using MediaPlayerLib;

namespace MediaPlayer {
    /// <summary>
    /// Interaction logic for FullScreen.xaml
    /// </summary>
    public partial class FullScreen : Window {

        // List of variables
        List<MediaData> mediaData;
        int index = 0;
        Timer timer;

        public FullScreen(List<MediaData> sortedMediaData) {
            InitializeComponent();
            // Set list of images
            mediaData = sortedMediaData;
            // Listen for key presses
            KeyDown += new KeyEventHandler(FullScreen_KeyDown);
            // Run slideshow
            Run();

        }

        /// <summary>
        /// Listen for key presses
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FullScreen_KeyDown(object sender, KeyEventArgs e) {
            switch(e.Key) {
                case Key.Escape:
                    if (timer != null) {
                        timer.Stop();
                    }
                    mediaElement.Stop();
                    this.Close();
                    break;
                case Key.Right:
                    if (timer != null) {
                        timer.Stop();
                    }
                    mediaElement.Stop();
                    NextImage();
                    break;
                case Key.Left:
                    if (timer != null) {
                        timer.Stop();
                    }
                    mediaElement.Stop();
                    PreviousImage();
                    break;
            }
        }

        /// <summary>
        /// Go to next image
        /// </summary>
        private void NextImage() {
            if (index == mediaData.Count - 1) {
                index = 0;
            } else {
                index++;
            }
            Run();
        }

        /// <summary>
        /// Go to previous image
        /// </summary>
        private void PreviousImage() {
            if (index == 0) {
                index = mediaData.Count - 1;
            } else {
                index--;
            }
            Run();
        }


        /// <summary>
        /// Play slideshow, starting at current image
        /// </summary>
        private void Run() {
            // If image
            if (mediaData[index] is ImageData) {
                BitmapImage bitmapImage = new BitmapImage();
                try {
                    bitmapImage.BeginInit();
                    bitmapImage.UriSource = new Uri(mediaData[index].FilePathName, UriKind.Absolute);
                    bitmapImage.EndInit();
                    bitmapImage.Freeze();
                } catch {
                    MessageBox.Show(string.Format("Image {0} cannot be loaded", mediaData[index].FileName), "Error");
                    this.Close();
                }
                // Update the gui
                Dispatcher.Invoke(delegate { image.Source = bitmapImage; });
                Dispatcher.Invoke(delegate { image.IsEnabled = true; });
                Dispatcher.Invoke(delegate { image.Visibility = System.Windows.Visibility.Visible; });
                Dispatcher.Invoke(delegate { mediaElement.IsEnabled = false; });
                Dispatcher.Invoke(delegate { mediaElement.Visibility = System.Windows.Visibility.Hidden; });
                // Set description
                if (!string.IsNullOrEmpty(mediaData[index].Description)) {
                    Dispatcher.Invoke(delegate { label.Visibility = System.Windows.Visibility.Visible; });
                    Dispatcher.Invoke(delegate { label.Content = mediaData[index].Description; });
                } else {
                    Dispatcher.Invoke(delegate { label.Visibility = System.Windows.Visibility.Hidden; });
                    Dispatcher.Invoke(delegate { label.Content = string.Empty; });
                }                
                // Set timer to next time
                timer = new System.Timers.Timer();
                timer.Elapsed += new ElapsedEventHandler(OnTimedEvent);
                timer.Interval = ((ImageData)mediaData[index]).Duration * 1000;
                timer.Enabled = true;
            } 
            if (mediaData[index] is VideoData) {
                // Update the gui
                if (File.Exists(mediaData[index].FilePathName)) {
                    Dispatcher.Invoke(delegate { image.IsEnabled = false; });
                    Dispatcher.Invoke(delegate { image.Visibility = System.Windows.Visibility.Hidden; });
                    Dispatcher.Invoke(delegate { mediaElement.Source = new Uri(mediaData[index].FilePathName, UriKind.Absolute); });
                    Dispatcher.Invoke(delegate { mediaElement.IsEnabled = true; });
                    Dispatcher.Invoke(delegate { mediaElement.Visibility = System.Windows.Visibility.Visible; });
                    // Set description
                    if (!string.IsNullOrEmpty(mediaData[index].Description)) {
                        Dispatcher.Invoke(delegate { label.Visibility = System.Windows.Visibility.Visible; });
                        Dispatcher.Invoke(delegate { label.Content = mediaData[index].Description; });
                    } else {
                        Dispatcher.Invoke(delegate { label.Visibility = System.Windows.Visibility.Hidden; });
                        Dispatcher.Invoke(delegate { label.Content = string.Empty; });
                    }
                    Dispatcher.Invoke(delegate { mediaElement.Play(); });
                } else {
                    MessageBox.Show(string.Format("Video {0} cannot be loaded", mediaData[index].FileName), "Error");
                    this.Close();
                }
            }

        }

        /// <summary>
        /// duration has passed, move to next image
        /// </summary>
        /// <param name="source"></param>
        /// <param name="e"></param>
        private void OnTimedEvent(object source, ElapsedEventArgs e) {
            timer.Stop();
            NextImage();
        }

        private void mediaElement_MediaEnded(object sender, RoutedEventArgs e) {
            NextImage();
        }
    }
}

using System;
using System.Collections.Generic;
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

namespace MediaPlayer {
    /// <summary>
    /// Interaction logic for MediaFiles.xaml
    /// </summary>
    public partial class FileTypesShowDialog : Window {

        private List<string> imageFiles;
        private List<string> videoFiles;

        public FileTypesShowDialog(List<string> imageFiles, List<string> videoFiles) {
            InitializeComponent();

            Owner = Application.Current.MainWindow;

            this.imageFiles = imageFiles.ToList();
            this.videoFiles = videoFiles.ToList();

            // Set checkboxes
            if (imageFiles.Contains(checkBoxJPG.Content.ToString())) {
                checkBoxJPG.IsChecked = true;
            }
            if (imageFiles.Contains(checkBoxJPEG.Content.ToString())) {
                checkBoxJPEG.IsChecked = true;
            }
            if (imageFiles.Contains(checkBoxPNG.Content.ToString())) {
                checkBoxPNG.IsChecked = true;
            }
            if (imageFiles.Contains(checkBoxBMP.Content.ToString())) {
                checkBoxBMP.IsChecked = true;
            }
            if (videoFiles.Contains(checkBoxAVI.Content.ToString())) {
                checkBoxAVI.IsChecked = true;
            }
            if (videoFiles.Contains(checkBoxMPG.Content.ToString())) {
                checkBoxMPG.IsChecked = true;
            }
            if (videoFiles.Contains(checkBoxMP4.Content.ToString())) {
                checkBoxMP4.IsChecked = true;
            }
        }

        public List<string> ImageFiles { get { return imageFiles; } }
        public List<string> VideoFiles { get { return videoFiles; } }

        /// <summary>
        /// Add item to list of images supported
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxImages_Checked(object sender, RoutedEventArgs e) {
            string content = ((CheckBox)sender).Content.ToString();
            if (!imageFiles.Contains(content)) {
                imageFiles.Add(content);
            }
        }

        /// <summary>
        /// Remove item from list of images supported
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxImages_Unchecked(object sender, RoutedEventArgs e) {
            string content = ((CheckBox)sender).Content.ToString();
            if (imageFiles.Contains(content)) {
                for (int i = 0; i < imageFiles.Count; i++) {
                    if (string.Compare(imageFiles[i], content) == 0) {
                        imageFiles.RemoveAt(i);
                    }
                }
            }
        }

        /// <summary>
        /// Add item to list of videos supported
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxVideos_Checked(object sender, RoutedEventArgs e) {
            string content = ((CheckBox)sender).Content.ToString();
            if (!videoFiles.Contains(content)) {
                videoFiles.Add(content);
            }
        }

        /// <summary>
        /// Remove item from list of videos supported
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void checkBoxVideos_Unchecked(object sender, RoutedEventArgs e) {
            string content = ((CheckBox)sender).Content.ToString();
            if (videoFiles.Contains(content)) {
                for (int i = 0; i < videoFiles.Count; i++) {
                    if (string.Compare(videoFiles[i], content) == 0) {
                        videoFiles.RemoveAt(i);
                    }
                }
            }
        }

        /// <summary>
        /// Apply changes
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonApplyChanges_Click(object sender, RoutedEventArgs e) {
            this.DialogResult = true;
        }
    }
}

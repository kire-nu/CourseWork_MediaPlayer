using System;
using System.Linq;
using System.IO;
using System.Security;
using System.Windows;
using System.Windows.Input;
using MediaPlayerLib;
using UtilitiesLib;
using System.Windows.Controls;
using System.Collections.Generic;
using System.Windows.Media.Imaging;
using System.Collections.ObjectModel;
using Image = System.Drawing.Image;
using System.ComponentModel;

namespace MediaPlayer {
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window {

        //Set variables
        private AlbumManager albumManager;
        private Album album;
        ObservableCollection<MediaData> folderImages = new ObservableCollection<MediaData>();
        ObservableCollection<MediaData> slideshowImages = new ObservableCollection<MediaData>();
        ObservableCollection<SlideShow> slideShows = new ObservableCollection<SlideShow>();
        SlideShow selectedSlideShow;
        List<string> supportedFileExtensions = new List<string> { ".bmp", ".jpeg", ".jpg", ".png", ".avi" };
        List<string> supportedFileExtensionsImage = new List<string> { ".bmp", ".jpeg", ".jpg", ".png"};
        List<string> supportedFileExtensionsVideo = new List<string> { ".avi" };
        System.Windows.Point mouseMoveStartingPoint;


        public MainWindow() {
            InitializeComponent();
            FolderViewLoadFoldersAndDrives();
            albumManager = new AlbumManager();
            album = albumManager.GetLastAlbum();
            if (album == null) {
                album = new Album();
                albumManager.Add(album);
            }
            UpdateGUI();
        }      

        private void UpdateGUI() {
            this.Title = string.Concat("MediaPlayer - ", album.Title);
            slideShows = album.SlideShows;
            if (slideShows.Count > 0) {
                listBoxSlideShows.ItemsSource = album.SlideShows;
                selectedSlideShow = album.SlideShows[0];
                slideshowImages = selectedSlideShow.MediaData;
                slideshowImageList.IsEnabled = true;
                slideshowImageList.ItemsSource = selectedSlideShow.MediaData;
                listBoxSlideShows.Items.Refresh();
            } else {
                slideshowImageList.IsEnabled = false;
            }
            listBoxSlideShows.Items.Refresh();
        }



        #region Folder View

        /// <summary>
        /// Load the my... folders and drives to the folder tree view
        /// </summary>
        private void FolderViewLoadFoldersAndDrives() {
            string path = string.Empty;
            // Get My Documents and add to tree view
            path = System.Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (!string.IsNullOrEmpty(path)) {
                FolderViewAddFolder(path);
            }
            // Get My Pictures and add to tree view
            path = System.Environment.GetFolderPath(Environment.SpecialFolder.MyPictures);
            if (!string.IsNullOrEmpty(path)) {
                FolderViewAddFolder(path);
            }
            // Get My Videos and add to tree view
            path = System.Environment.GetFolderPath(Environment.SpecialFolder.MyVideos);
            if (!string.IsNullOrEmpty(path)) {
                FolderViewAddFolder(path);
            }
            // Add drives to folder view
            FolderViewAddDrives();

        }

        /// <summary>
        /// Add a folder to the folder tree view
        /// </summary>
        /// <param name="folderPath"></param>
        private void FolderViewAddFolder(string folderPath) {
            // Create new item
            TreeViewItem newItem = new TreeViewItem() {
                // Set header and full path
                Header = HelperMethods.GetFolderName(folderPath),
                Tag = folderPath
            };
            // Add a dummy item so it can be expanded, but without loading subfolder
            newItem.Items.Add(null);
            // Listen out for item being expanded
            newItem.Expanded += Folder_Expanded;
            // Add it to the main tree view
            folderView.Items.Add(newItem);
        }

        /// <summary>
        /// Add drives to folder view
        /// </summary>
        public void FolderViewAddDrives() {
            foreach (string drive in Directory.GetLogicalDrives()) {
                // Create new item
                TreeViewItem treeViewItem = new TreeViewItem() {
                    // Set header and full path
                    Header = drive,
                    Tag = drive
                };
                // Add a dummy item so it can be expanded, but without loading subfolder
                treeViewItem.Items.Add(null);
                // Listen out for item being expanded
                treeViewItem.Expanded += Folder_Expanded;
                // Add it to the main tree-view
                folderView.Items.Add(treeViewItem);
            }
        }

        /// <summary>
        /// When a folder is expanded, find the sub folders/files
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private static void Folder_Expanded(object sender, RoutedEventArgs e) {
            // Get item being expanded
            TreeViewItem item = (TreeViewItem)sender;
            // If the item only contains the dummy data
            if (item.Items.Count != 1 || item.Items[0] != null) {
                return;
            }
            // Clear dummy data
            item.Items.Clear();
            // Get full path
            string fullPath = item.Tag.ToString();

            // Create a blank list for directories
            List<string> directories = new List<string>();

            // Try and get directories from the folder
            try {
                string[] dirs = Directory.GetDirectories(fullPath);

                if (dirs.Length > 0)
                    directories.AddRange(dirs);
            } catch {
                MessageBox.Show(string.Format("Error reading {0}", fullPath), "Error");
                return;
            }
            // For each directory...
            foreach(string directoryPath in directories) {
                // Create item
                TreeViewItem treeViewItem = new TreeViewItem() {
                    Header = HelperMethods.GetFolderName(directoryPath),
                    Tag = directoryPath
                };
                // Add dummy item so we can expand folder
                treeViewItem.Items.Add(null);
                // Handle expanding
                treeViewItem.Expanded += Folder_Expanded;
                // Add this item to the parent
                item.Items.Add(treeViewItem);
            }
        }


        #endregion



        #region Display Images in Folder

        /// <summary>
        /// On Select folder, call method to display images
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TreeViewItem_OnItemSelected(object sender, RoutedEventArgs e) {
            TreeViewItem treeViewItem = (TreeViewItem)folderView.SelectedItem;
            string path = treeViewItem.Tag.ToString();
            DisplayImagesInFolder(path);
        }

        /// <summary>
        /// Display images in selected folder
        /// </summary>
        /// <param name="path"></param>
        private void DisplayImagesInFolder(string path) {

            // Clear current show list
            folderImages.Clear();

            // Get all files with the extention that is supported
            List<string> files = Directory.GetFiles(path, "*.*").Where(s => supportedFileExtensions.Contains(Path.GetExtension(s).ToLower())).ToList();

            // For each file
            for (int i = 0; i<files.Count(); i++) {
                //Get extention
                string fileExtention = Path.GetExtension(files[i]).ToLower();
                MediaData mediaData = null;
                // If image
                if (supportedFileExtensionsImage.Contains(fileExtention)) {
                    mediaData = new ImageData(files[i], false);
                    //mediaData.ID = i;

                    // Load images and other data in the background to speed up process 
                    BackgroundWorker imageLoader = new BackgroundWorker();
                    imageLoader.WorkerReportsProgress = true;
                    imageLoader.DoWork += imageLoader_DoWork;
                    imageLoader.RunWorkerCompleted += imageLoader_RunWorkerCompleted;
                    imageLoader.RunWorkerAsync(mediaData);
                }
                // If video
                if (supportedFileExtensionsVideo.Contains(fileExtention)) {
                    mediaData = new VideoData(files[i], false);
                    //mediaData.ID = i;
                }
                folderImages.Add(mediaData);
            }
            // Update list of files
            folderImageList.ItemsSource = folderImages;

        }
                
        /// <summary>
        /// Load images
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imageLoader_DoWork(object sender, DoWorkEventArgs e) {
            // Get entry
            MediaData mediaData = (ImageData)e.Argument;
            if (mediaData is ImageData) {
                try {
                    // Read image to get width and height
                    Image image = Image.FromFile(mediaData.FilePathName);
                    ((ImageData)mediaData).Width = image.Width;
                    ((ImageData)mediaData).Height = image.Height;
                    // Create thumbnail
                    BitmapImage thumbNailImage = HelperMethods.CreateThumbNail(mediaData.FilePathName, ((ImageData)mediaData).Width, ((ImageData)mediaData).Height, 100, 100);
                    ((ImageData)mediaData).ThumbNailImage = HelperMethods.BitmapImageToByte(thumbNailImage);
                    //((ImageData)mediaData).ThumbNailImage.Freeze(); // Needs doing otherwise errors
                } catch {
                    MessageBox.Show(string.Format("Error creating thumbnail for {0}", mediaData.FileName), "Error");
                }
            }
        }

        /// <summary>
        /// Image has loaded, update list
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void imageLoader_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e) {
            Dispatcher.BeginInvoke((Action)(() => {
                folderImageList.Items.Refresh();
            }));
        }
        #endregion



        #region Drag and Drop

        /// <summary>
        /// Get start location of drag and drop if it is selected from folder view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FolderImage_LeftButton(object sender, MouseButtonEventArgs e) {
            mouseMoveStartingPoint = e.GetPosition(null);
        }

        /// <summary>
        /// Move a folder image to slide show view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void FolderImage_Move(object sender, MouseEventArgs e) {
            // Get position of mouse
            System.Windows.Point point = e.GetPosition(null);
            // Calculate distance
            Vector diff = mouseMoveStartingPoint - point;
            // If draged distance greater than minumum distance
            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)) {
                // Get source id
                Border border = (Border)sender;
                MediaData mediaData = (MediaData)border.DataContext;
                // Create new temporary media data entry
                bool thumbNailLoaded = true;
                if (mediaData is ImageData) {
                    // If image has loaded
                    thumbNailLoaded = ((ImageData)mediaData).ThumbNailImageLoaded;
                }
                if (thumbNailLoaded) {
                    //MediaData mediaData = NewMediaData(folderImages[id], -1, -1, false);
                    if (mediaData != null) {
                        DragDrop.DoDragDrop((DependencyObject)sender, mediaData, DragDropEffects.Move);
                    }
                }
            }
        }

        /// <summary>
        /// Get start location of drag and drop if it is selected from slide show
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SlideshowImage_LeftButton(object sender, MouseButtonEventArgs e) {
            mouseMoveStartingPoint = e.GetPosition(null);
            //Border border = (Border)sender;
            //int id = -1;
            //int.TryParse(border.Tag.ToString(), out id);
        }


        /// <summary>
        /// Move a slide show image to slide show view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SlideshowImage_Move(object sender, MouseEventArgs e) {
            // Get position of mouse
            System.Windows.Point point = e.GetPosition(null);
            // Calculate distance
            Vector diff = mouseMoveStartingPoint - point;
            // If draged distance greater than minumum distance
            if (e.LeftButton == MouseButtonState.Pressed &&
                (Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                    Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance)) {
                // Get source
                Border border = (Border)sender;
                MediaData mediaData = (MediaData)border.DataContext;
                // Drag and drop it
                DragDrop.DoDragDrop((DependencyObject)sender, mediaData, DragDropEffects.Move);
            }
        }


        /// <summary>
        /// Drop in slide show
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void SlideshowImageList_Drop(object sender, DragEventArgs e) {
            if (slideshowImageList.IsEnabled) {
                //Get data that's being draged
                MediaData dragedMediaData;
                dragedMediaData = (ImageData)e.Data.GetData(typeof(ImageData));
                if (dragedMediaData == null) {
                    dragedMediaData = (VideoData)e.Data.GetData(typeof(VideoData));
                }
                // Get X location of mouse (only moving in X direction, not Y)            
                double mouseX = e.GetPosition(this).X;
                // Set other x values
                double dragedX_Left = double.NaN;
                double dragedX_Right = double.NaN;
                // Set values
                bool insertedBefore = false;
                bool move = true;


                // If the draged card is in the slideshow, get location
                if (dragedMediaData.Id >= 0) {
                    UIElement uiElement =
                        (UIElement)slideshowImageList.ItemContainerGenerator.ContainerFromIndex(dragedMediaData.Order);
                    dragedX_Left = uiElement.TranslatePoint(new System.Windows.Point(0, 0), this).X;
                    dragedX_Right = dragedX_Left + uiElement.RenderSize.Width;
                    // If mnote draged outsided extent of selected item, it's not being moved
                    if (mouseX > dragedX_Left && mouseX < dragedX_Right) {
                        move = false;
                    }
                }

                // If being mode, we're going to find the location to drop in
                if (move) {
                    // Loop over cards
                    for (int i = 0; i < slideshowImages.Count; i++) {
                        int thisID = slideshowImages[i].Id;
                        if (thisID != dragedMediaData.Id) {
                            // Extent of 
                            double extentLeft = double.NaN;
                            double extentRight = double.NaN;
                            UIElement uiElement = (UIElement)slideshowImageList.ItemContainerGenerator.ContainerFromIndex(slideshowImages[i].Order);
                            // If previous or next card was the draged imaged, use all extent
                            if ((slideshowImages[i].Order == dragedMediaData.Order + 1) || (slideshowImages[i].Order == dragedMediaData.Order - 1)) {
                                extentLeft = uiElement.TranslatePoint(new System.Windows.Point(0, 0), this).X;
                                extentRight = extentLeft + uiElement.RenderSize.Width;
                            } else {
                                // Otherwise use mid to mid
                                if (double.IsNaN(extentRight)) {
                                    extentLeft = uiElement.TranslatePoint(new System.Windows.Point(0, 0), this).X;
                                } else {
                                    extentLeft = extentRight;
                                }
                                extentRight = uiElement.TranslatePoint(new System.Windows.Point(0, 0), this).X + +uiElement.RenderSize.Width / 2;
                            }
                            // If drop location is within extent, insert it here
                            if (mouseX > extentLeft && mouseX < extentRight) {
                                insertedBefore = true;
                                // If draged from folder view, create
                                if (dragedMediaData.Id == -1) {
                                    MediaData mediaData = NewMediaData(dragedMediaData, slideshowImages.Count, i, true);
                                    selectedSlideShow.Insert(i, mediaData);
                                } else {
                                    // Otherwise move current
                                    selectedSlideShow.Move(dragedMediaData.Order, i);
                                }
                                // Image has been inserted, break loop
                                i = slideshowImages.Count;
                            }
                        }
                    }
                    // If we move it to the end
                    if (!insertedBefore) {
                        // If draged from folder view, create
                        if (dragedMediaData.Id == -1) {
                            MediaData mediaData = NewMediaData(dragedMediaData, slideshowImages.Count, slideshowImages.Count, true);
                            selectedSlideShow.Add(mediaData);
                        } else {
                            // Otherwise move current
                            selectedSlideShow.Move(dragedMediaData.Order, slideshowImages.Count - 1);
                        }
                    }
                }
                // Reset order for each image
                selectedSlideShow.ReOrder();
                slideshowImages = selectedSlideShow.MediaData;
                slideshowImageList.ItemsSource = slideshowImages;
                slideshowImageList.Items.Refresh();
            } else {
                MessageBox.Show("Slideshow needs to be created first", "Error");
            }
        }

        /// <summary>
        /// Create new media data entry
        /// </summary>
        /// <param name="source"></param>
        /// <param name="id"></param>
        /// <param name="order"></param>
        /// <returns></returns>
        private MediaData NewMediaData(MediaData source, int id, int order, bool inSlideShow) {
            MediaData mediaData = null;
            if (source is ImageData) {
                mediaData = new ImageData(source.FilePathName, ((ImageData)source).Height, ((ImageData)source).Width, 10, order, inSlideShow) {
                    ThumbNailImage = ((ImageData)source).ThumbNailImage.ToArray()
                };
            }
            if (source is VideoData) {
                mediaData = new VideoData(source.FilePathName, order, inSlideShow);
            }
            return mediaData;
        }

        #endregion



        #region Menu Bar Actions

        /// <summary>
        /// Exit program
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MenuItemClose_Click(object sender, RoutedEventArgs e) {
            Environment.Exit(0);
        }

        /// <summary>
        /// Open file, not implemented
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemOpen_Click(object sender, RoutedEventArgs e) {
            ObservableCollection<Album> albums = albumManager.GetAlbums();
            RestoreAlbum restoreAlbum = new RestoreAlbum(albums);
            if (restoreAlbum.ShowDialog() == true) {
                this.album = albumManager.GetAlbum(restoreAlbum.AlbumId);
                UpdateGUI();
            }
        }

        /// <summary>
        /// Rename Current album
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemRename_Click(object sender, RoutedEventArgs e) {
            // Open dialog to rename with current name
            RenameDialog renameDialog = new RenameDialog(album.Title, "Name", "Album");
            // If renamed
            if (renameDialog.ShowDialog() == true) {
                // Set new name and update list
                album.Title = renameDialog.Name;
                UpdateGUI();
            }
        }

        /// <summary>
        /// New album
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemNew_Click(object sender, RoutedEventArgs e) {
            // Open dialog to rename with current name
            RenameDialog renameDialog = new RenameDialog(string.Empty, "Rename", "Album");
            // If renamed
            if (renameDialog.ShowDialog() == true) {
                // Set new name and update list
                album = new Album(renameDialog.Name);
                albumManager.Add(album);
                UpdateGUI();
            }
        }

        /// <summary>
        /// Delete album
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemDelete_Click(object sender, RoutedEventArgs e) {
            MessageBoxResult messageBoxResult = MessageBox.Show(string.Format("Do you want to delete album {0}", album.Title), "Warning", MessageBoxButton.YesNo);
            if (messageBoxResult == MessageBoxResult.Yes) {
                // Remove for list and update list
                albumManager.RemoveAlbum(album.Id);
                album = albumManager.GetLastAlbum();
                if (album == null) {
                    album = new Album();
                }

                UpdateGUI();
            }

        }


        /// <summary>
        /// Change file extentions
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuItemFileTypes_Click(object sender, RoutedEventArgs e) {
            FileTypesShowDialog fileTypesShowDialog = new FileTypesShowDialog(supportedFileExtensionsImage, supportedFileExtensionsVideo);
            if (fileTypesShowDialog.ShowDialog() == true) {
                supportedFileExtensionsImage = fileTypesShowDialog.ImageFiles;
                supportedFileExtensionsVideo = fileTypesShowDialog.VideoFiles;
                supportedFileExtensions = new List<string>(supportedFileExtensionsImage);
                supportedFileExtensions.AddRange(supportedFileExtensionsVideo);
            }
        }

        #endregion



        #region Slideshow menu

        /// <summary>
        /// Create Slideshow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonAddSlideShow_Click(object sender, RoutedEventArgs e) {
            // Open dialog to enter name
            RenameDialog renameDialog = new RenameDialog(string.Empty, "Name", "SlideShow");
            if (renameDialog.ShowDialog() == true) {
                // Get name and create new slideshow
                string slideShowName = renameDialog.Name;
                SlideShow slideShow = new SlideShow(slideShowName);
                album.Add(slideShow);
                selectedSlideShow = slideShow;
            }
            UpdateGUI();
        }

        /// <summary>
        /// Remove slide show
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRemoveSlideShow_Click(object sender, RoutedEventArgs e) {
            // Slide show must be selected
            if (listBoxSlideShows.SelectedItem != null) {
                int index = listBoxSlideShows.SelectedIndex;
                // Display warning
                MessageBoxResult messageBoxResult = MessageBox.Show(string.Format("Do you want to remove slideshow {0}", album.GetAt(index).Name), "Warning", MessageBoxButton.YesNo);
                if (messageBoxResult == MessageBoxResult.Yes) {
                    // Remove for list and update list
                    album.RemoveAt(index);
                    UpdateGUI();
                }
            }
        }

        /// <summary>
        /// Rename Slide Show
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRenameSlideShow_Click(object sender, RoutedEventArgs e) {
            // Slide show must be selected
            if (listBoxSlideShows.SelectedItem != null) {
                int index = listBoxSlideShows.SelectedIndex;
                // Open dialog to rename with current name
                RenameDialog renameDialog = new RenameDialog(album.GetAt(index).Name, "Rename", "SlideShow");
                // If renamed
                if (renameDialog.ShowDialog() == true) {
                    UpdateGUI();
                }
            }
        }

        /// <summary>
        /// Select slide show
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void listBoxSlideShows_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            // Check selection
            int index = listBoxSlideShows.SelectedIndex;
            if (index >= 0) {
                selectedSlideShow = album.GetAt(index);
                // Check length of list
                if (album.GetAt(index).MediaData.Count > 0) {
                    slideshowImages = new ObservableCollection<MediaData>(album.GetAt(index).MediaData);
                } else {
                    slideshowImages = new ObservableCollection<MediaData>();
                }
            } else {
                slideshowImages = null;
            }
            // Update list
            //listBoxSlideShows.ItemsSource = album.SlideShows;
            slideshowImageList.ItemsSource = slideshowImages;
            slideshowImageList.Items.Refresh();
        }

        #endregion



        #region Slideshow settings

        /// <summary>
        /// Play slideshow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonPlay_Click(object sender, RoutedEventArgs e) {
            // Check that slideshow is not empty
            if (selectedSlideShow != null) {
                if (selectedSlideShow.Count > 0) {
                    // Check that all files exist
                    bool filesExist = true;
                    for (int i = 0; i < selectedSlideShow.Count; i++) {
                        MediaData mediaData = selectedSlideShow.GetAt(i);
                        if (mediaData != null) {
                            if (!File.Exists(mediaData.FilePathName)) {
                                filesExist = false;
                            }
                        }
                    }
                    // If files exist, play slide show
                    if (filesExist) {
                        List<MediaData> imageCards = selectedSlideShow.GetMediaDataInOrder();
                        FullScreen fullScreen = new FullScreen(imageCards);
                        fullScreen.Show();
                    } else {
                        MessageBox.Show("All files does no longer exist", "Error");
                    }
                } else {
                    MessageBox.Show("Slideshow is empty", "Error");
                }
            } else {
                MessageBox.Show("Slideshow does not exist", "Error");
            }
        }

        /// <summary>
        /// When image is selected enable/disable setting
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void selectedImageList_SelectionChanged(object sender, SelectionChangedEventArgs e) {
            if (slideshowImageList.SelectedItem != null) {
                int index = slideshowImageList.SelectedIndex;
                buttonRemoveImage.IsEnabled = true;
                textBoxImageText.IsEnabled = true;
                textBoxImageText.Text = selectedSlideShow.GetDescription(index);
                if (selectedSlideShow.GetAt(index) is ImageData) { 
                    textBoxDuration.IsEnabled = true;
                    textBoxDuration.Text = selectedSlideShow.GetDuration(index).ToString();
                }
            } else {
                buttonRemoveImage.IsEnabled = false;
                textBoxImageText.Text = string.Empty;
                textBoxImageText.IsEnabled = false;
                textBoxDuration.Text = string.Empty;
                textBoxDuration.IsEnabled = false;
            }
        }

        /// <summary>
        /// Save duration when moving away from textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxDuration_LostFocus(object sender, RoutedEventArgs e) {
            if (slideshowImageList.SelectedItem != null) {
                int index = slideshowImageList.SelectedIndex;
                int duration = -1;
                if (int.TryParse(textBoxDuration.Text, out duration)) {
                    selectedSlideShow.SetDuration(index, duration);
                }
            }
        }

        /// <summary>
        /// Save text when moving away from textbox
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void textBoxImageText_LostFocus(object sender, RoutedEventArgs e) {
            if (slideshowImageList.SelectedItem != null) {
                int index = slideshowImageList.SelectedIndex;
                selectedSlideShow.SetDescription(index, textBoxImageText.Text);
            }
        }

        /// <summary>
        /// Remove image from slideshow
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void buttonRemoveImage_Click(object sender, RoutedEventArgs e) {
            if (slideshowImageList.SelectedItem != null) {
                int index = slideshowImageList.SelectedIndex;
                slideshowImages.RemoveAt(index);
                selectedSlideShow.ReOrder();
                slideshowImageList.ItemsSource = slideshowImages;
                slideshowImageList.Items.Refresh();
            }
        }

        #endregion

    }
}

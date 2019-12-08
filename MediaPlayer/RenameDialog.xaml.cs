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
    /// Interaction logic for NewAlbum.xaml
    /// </summary>
    public partial class RenameDialog : Window {

        private string task;
        private string item;
           

        public RenameDialog(string name, string task, string item) {
            InitializeComponent();
            this.task = task;
            this.item = item;
            textboxName.Text = name;
            Owner = Application.Current.MainWindow;
            this.Title = string.Concat(task," ",item);
            this.labelEnterName.Content = string.Concat("Enter name of ", item);
        }
        public string Name { get { return textboxName.Text; } }

        private void bottonDialogOk_Click(object sender, RoutedEventArgs e) {
            if (textboxName.Text.Length > 0) {
                this.DialogResult = true;
            } else {
                MessageBox.Show(string.Concat("Error: ",item," name cannot be blank"), "Error");
            }
        }
    }
}

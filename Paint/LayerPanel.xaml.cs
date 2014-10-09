using System;
using System.Collections.Generic;
using System.IO.Packaging;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Paint
{
    /// <summary>
    /// Interaction logic for LayerPanel.xaml
    /// </summary>
    public partial class LayerPanel : UserControl
    {

        #region Events
        public event LayerNameChanged NameChanged;
        public delegate void LayerNameChanged(object sender, NameArgs args);

        public event ActivePropertyChanged ActiveChanged;
        public delegate void ActivePropertyChanged(object sender, EventArgs e);

        public event VisibilityPropertyChanged VisibilityChanged;
        public delegate void VisibilityPropertyChanged(object sender, EventArgs e);

        public event DeleteThisLayer DeleteLayerEvent;
        public delegate void DeleteThisLayer(object sender, EventArgs e);
        #endregion
        public bool Active = false;
        private bool Visible = true;
        public string Name;
        
        public LayerPanel(string name)
        {
            InitializeComponent();
            NameTextBlock.Text = name;
            Name = name;
        }

        //public LayerPanel(string name, bool bydefault)
        //{
        //    InitializeComponent();
        //    NameTextBlock.Text = name;
        //    Name = name;
        //    Active = true;
        //    PanelBorder.Background = Brushes.CadetBlue;
        //}

        private void EditLayerName(object sender, RoutedEventArgs e)
        {
            NameTextBlock.Visibility = Visibility.Collapsed;
            EditNameTextBox.Visibility = Visibility.Visible;
            EditNameTextBox.Focus();
        }

        private void EditName_LostFocus(object sender, RoutedEventArgs e)
        {
            var prevName = NameTextBlock.Text;
            NameTextBlock.Text = EditNameTextBox.Text;
            Name = EditNameTextBox.Text;
            EditNameTextBox.Visibility = Visibility.Collapsed;
            NameTextBlock.Visibility = Visibility.Visible;
            NameChanged(this, new NameArgs(prevName, Name));
        }

        private void EditName_EnterPressed(object sender, KeyEventArgs e)
        {
            if (e.Key != Key.Enter) return;
            var prevName = NameTextBlock.Text;
            NameTextBlock.Text = EditNameTextBox.Text;
            Name = EditNameTextBox.Text;
            EditNameTextBox.Visibility = Visibility.Collapsed;
            NameTextBlock.Visibility = Visibility.Visible;

            NameChanged(this, new NameArgs(prevName, Name));
        }

        private void SetPanelActive(object sender, MouseButtonEventArgs e)
        {
            if (Active) return;
            if (e.ClickCount != 2) return;
            Active = true;
            ActiveChanged(this, new EventArgs());
            PanelBorder.Background = Brushes.CadetBlue;
        }

        public void DeactivatePanel()
        {
            Active = false;
            PanelBorder.Background = Brushes.White;
        }

        public void ActivatePanel()
        {
            Active = false;
            PanelBorder.Background = Brushes.CadetBlue;
            ActiveChanged(this, new EventArgs());
        }

        private void VisibilityButton_OnClick(object sender, RoutedEventArgs e)
        {
            if (Visible)
            {
                VisibilityImage.Source = new BitmapImage(new Uri("pack://application:,,,/Icons/blocked.png"));
                Visible = false;
            }
            else
            {
                VisibilityImage.Source = new BitmapImage(new Uri("pack://application:,,,/Icons/Eye_blue.png"));
                Visible = true;
            }
            VisibilityChanged(this, new EventArgs());
        }

        private void DeleteLayerButton_OnClick(object sender, RoutedEventArgs e)
        {
            DeleteLayerEvent(this, new EventArgs());
        }
    }

    public class NameArgs : EventArgs
    {
        private string _prevName;
        private string _newName;

        public string PrevName
        {
            get { return _prevName; }
        }

        public string NewName
        {
            get { return _newName; }
        }

        public NameArgs(string previous, string current)
        {
            _prevName = previous;
            _newName = current;
        }
    }


    
}

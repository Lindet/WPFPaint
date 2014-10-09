using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Paint
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        private Point PrevPosition;
        private bool HasPreviosPoint = false;
        private Brush currentColor = Brushes.Black;
        private readonly ObservableCollection<string> layersList = new ObservableCollection<string>();
        private Canvas currentCanvas;
        private LayerPanel currentPanel;
        private int currentNumber = 0;
       
        
        public MainWindow()
        {
            DataContext = this;
            InitializeComponent();
            CreateNewLayer("DefaultLayer");
        }

        private void CreateNewLayer()
        {
            var newLayer = new Canvas() { Style = (Style)FindResource("PaintCanvaStyle"), Name = "NewLayer" + currentNumber, Width = 1500, Height = 1500};
            currentNumber++;
            LayoutGrid.Children.Add(newLayer);
            currentCanvas = newLayer;
            layersList.Add(newLayer.Name);
            var temp = new LayerPanel(newLayer.Name);
            temp.VisibilityChanged += VisibilityChanged;
            temp.DeleteLayerEvent += DeleteLayer;
            temp.NameChanged += LayerNameChanged;
            temp.ActiveChanged += ActiveChanged;
            LayerStackPanel.Children.Add(temp);

            LayerListExpander.IsExpanded = true;
            temp.ActivatePanel();
        }

        private void CreateNewLayer(string LayerName)
        {
            var newLayer = new Canvas() { Style = (Style)FindResource("PaintCanvaStyle"), Width = 1500, Height = 1500};
            LayoutGrid.Children.Add(newLayer);
            currentCanvas = newLayer;
            layersList.Add(newLayer.Name);
            var temp = new LayerPanel(newLayer.Name);
            temp.VisibilityChanged += VisibilityChanged;
            temp.DeleteLayerEvent += DeleteLayer;
            temp.ActiveChanged += ActiveChanged;
            temp.NameChanged += LayerNameChanged;
            LayerStackPanel.Children.Add(temp);

            temp.ActivatePanel();
            LayerListExpander.IsExpanded = true;
        }

        

        #region Events

        private void ActiveChanged(object sender, EventArgs eventArgs)
        {
            var item = sender as LayerPanel;
            if (currentPanel != null) currentPanel.DeactivatePanel();
            currentPanel = item;
            currentCanvas = LayoutGrid.Children.OfType<Canvas>().FirstOrDefault(z => z.Name == currentPanel.Name);
        }

        private void DeleteLayer(object sender, EventArgs e)
        {
            var item = sender as LayerPanel;
            LayoutGrid.Children.Remove(LayoutGrid.Children.OfType<Canvas>().FirstOrDefault(z => z.Name == item.Name));
            LayerStackPanel.Children.Remove(item);
        }

        private void VisibilityChanged(object sender, EventArgs e)
        {
            var item = sender as LayerPanel;
            LayoutGrid.Children.OfType<Canvas>().FirstOrDefault(z => z.Name == item.Name).Visibility =
                LayoutGrid.Children.OfType<Canvas>().FirstOrDefault(z => z.Name == item.Name).Visibility ==
                Visibility.Visible
                    ? Visibility.Collapsed
                    : Visibility.Visible;
        }

        private void Canvas_MouseMove(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;
            if (!HasPreviosPoint)
            {
                PrevPosition = e.GetPosition(sender as Canvas);
                HasPreviosPoint = true;
            }
            else
            {
                if (currentCanvas.Visibility != Visibility.Visible) return;
                var point = e.GetPosition(sender as Canvas);
                var line = new Line() { Stroke = currentColor, X1 = PrevPosition.X, Y1 = PrevPosition.Y, X2 = point.X, Y2 = point.Y, StrokeThickness = 2};
                currentCanvas.Children.Add(line);
                PrevPosition = point;
            }
        }

        private void LayerNameChanged(object sender, NameArgs e)
        {
            for (var i = 0; i < layersList.Count; i++)
            {
                if (layersList[i] == e.PrevName) layersList[i] = e.NewName;
            }

            LayoutGrid.Children.OfType<Canvas>().FirstOrDefault(z => z.Name == e.PrevName).Name = e.NewName;
        }

        private void Canvas_MouseUp(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;
            HasPreviosPoint = false;
        }

        private void Button_OnClick(object sender, RoutedEventArgs e)
        {
            var temp = sender as Button;
            switch (temp.Name)
            {
                case "NewLayerButton":
                    CreateNewLayer();
                    break;
            }
        }

        private void ColorButton_OnClick(object sender, RoutedEventArgs e)
        {
            currentColor = (sender as Button).Background;
        }

        private void ClearAllCanvas(object sender, RoutedEventArgs e)
        {
            foreach (var item in LayoutGrid.Children)
            {
                if(item is Canvas)
                    (item as Canvas).Children.Clear();
            }
        }

        private void LeavePaintArea(object sender, MouseEventArgs e)
        {
            HasPreviosPoint = false;
        }

        //private void CurrentLayerChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    var selectedName = (sender as ListBox).SelectedItem.ToString();

        //    foreach (var item in Grid.Children)
        //    {
        //        if (!(item is Canvas)) continue;
        //        if ((item as Canvas).Name == selectedName)
        //            currentControl = item as Canvas;
        //    }

        //}

        #endregion


        #region MenuEvents
            private void AppMenu_ExitButtonClick(object sender, MouseButtonEventArgs e)
            {
                App.Current.Shutdown();
            }

        private void AppMenu_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;


            if (e.ClickCount == 2)
                //Двойное нажатие на верхнюю панель максимизирует окно или возвращает его к исходным размерам.
            {
                Window window = Window.GetWindow((Border) sender);
                window.WindowState = window.WindowState == WindowState.Maximized
                    ? WindowState.Normal
                    : WindowState.Maximized;

                LayerListExpander.MaxHeight = SecondRowDefinition.ActualHeight-160;

            }
            else 
                //А если нажатие было одно, то позволяет перетаскивать окно.
            {
                this.DragMove();
            }


        }

        #endregion

        private void Default_OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            HasPreviosPoint = true;
            PrevPosition = e.GetPosition(sender as Image);
        }
    }
}

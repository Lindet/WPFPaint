using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
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
        private Canvas currentControl; 
        public MainWindow()
        {
            InitializeComponent();
            currentControl = FirstLayer;
        }

        private void Canvas_MouseDown(object sender, MouseEventArgs e)
        {
            if (e.LeftButton != MouseButtonState.Pressed) return;
            if (!HasPreviosPoint)
            {
                PrevPosition = e.GetPosition(sender as Canvas);
                HasPreviosPoint = true;
            }
            else
            {
                Brush brush = Brushes.Transparent;
                switch (currentControl.Name)
                {
                    case "FirstLayer":
                        brush = Brushes.Red;
                        break;
                    case "SecondLayer":
                        brush = Brushes.Green;
                        break;
                    case "ThirdLayer":
                        brush = Brushes.Blue;
                        break;

                }
                var point = e.GetPosition(sender as Canvas);
                var line = new Line() {Stroke = brush, X1 = PrevPosition.X, Y1 = PrevPosition.Y, X2 = point.X, Y2 = point.Y, StrokeThickness = 2};
                currentControl.Children.Add(line);
                UpdateLayout();
                PrevPosition = point;
            }
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
                case "FirstButton":
                    currentControl = FirstLayer;
                    break;
                case "SecondButton":
                    currentControl = SecondLayer;
                    break;
                case "ThirdButton":
                    currentControl = ThirdLayer;
                    break;
                  
            }
            currentControlTextBlock.Text = "Current Control : " + currentControl.Name;
        }

        private void ClearAllCanvas(object sender, RoutedEventArgs e)
        {
            foreach (var item in grid.Children)
            {
                if(item is Canvas)
                    (item as Canvas).Children.Clear();
            }
        }

        private void LeavePaintArea(object sender, MouseEventArgs e)
        {
            HasPreviosPoint = false;
        }
    }
}

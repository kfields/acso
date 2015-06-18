using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

using System.Xml;
using System.Windows.Markup;

namespace ACSO
{
    public interface ICitiesReport
    {
        void CitiesModified();
    }
    /// <summary>
    /// Interaction logic for CitiesControl.xaml
    /// </summary>
    public partial class CitiesControl : UserControl
    {
        public ICitiesReport IReport;
        //
        public Canvas cityCanvas;
        public Canvas tourCanvas = new Canvas();
        public Canvas adornCanvas = new Canvas();
        //
        public int CityCount
        {
            get { return cityCanvas.Children.Count; }
        }
        //
        private Tool tool = null;
        private Dictionary<string, Tool> tools = new Dictionary<string, Tool>();
        //
        private const int cityWidth = 10;
        private const int cityHeight = 10;
        private SolidColorBrush cityFillBrush = new SolidColorBrush(Colors.Chartreuse);
        private SolidColorBrush cityStrokeBrush = new SolidColorBrush(Colors.Black);
        //
        public CitiesControl()
        {
            InitializeComponent();
            tool = new SelectorTool(this);
            tool.Activate();
            tools.Add("select", tool);
            tools.Add("create", new CreatorTool(this));
        }
        public void UseTool(string toolName)
        {
            Tool useTool = tools[toolName];
            if (tool == useTool)
                return;
            //else
            tool.Deactivate();
            tool = useTool;
            tool.Activate();
        }
        public void Load(string filename)
        {
            if (filename == "")
                return;
            XmlReader xmlReader = XmlReader.Create(filename);
            cityCanvas = (Canvas)XamlReader.Load(xmlReader);
            cityCanvas.IsHitTestVisible = false;
            double cityCanvasWidth = cityCanvas.Width;
            double cityCanvasHeight = cityCanvas.Height;
            //
            Width = cityCanvasWidth;
            Height = cityCanvasHeight;
            //
            tourCanvas.IsHitTestVisible = false;
            tourCanvas.Children.Clear();
            tourCanvas.Width = cityCanvasWidth;
            tourCanvas.Height = cityCanvasHeight;
            //
            adornCanvas.IsHitTestVisible = false;
            adornCanvas.Width = cityCanvasWidth;
            adornCanvas.Height = cityCanvasHeight;
            //
            //mainCanvas.IsHitTestVisible = false;
            mainCanvas.Background = Brushes.LightSteelBlue;
            mainCanvas.Width = cityCanvasWidth;
            mainCanvas.Height = cityCanvasHeight;
            //
            mainCanvas.Children.Clear();
            mainCanvas.Children.Add(tourCanvas);
            mainCanvas.Children.Add(cityCanvas);
            mainCanvas.Children.Add(adornCanvas);
            //
            IReport.CitiesModified();
        }
        public void CreateCity(Point point)
        {
            Ellipse ellipse = new Ellipse();
            ellipse.Width = cityWidth;
            ellipse.Height = cityHeight;
            ellipse.Fill = cityFillBrush;
            ellipse.Stroke = cityStrokeBrush;
            ellipse.SetValue(Canvas.LeftProperty, point.X - cityWidth/2);
            ellipse.SetValue(Canvas.TopProperty, point.Y - cityHeight/2);
            cityCanvas.Children.Add(ellipse);
            //
            IReport.CitiesModified();
        }
    }
}

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

namespace ACSO
{
    class Tool
    {
        protected CitiesControl control;
        //
        protected Tool(CitiesControl ctrl)
        {
            control = ctrl;
        }
        public virtual void Activate()
        {
        }
        public virtual void Deactivate()
        {
        }
    }
    //
    struct Selectee
    {
        public Shape shape;
        public Shape adorner;
        //
        public Selectee(Shape s, Shape a)
        {
            shape = s;
            adorner = a;
        }
    }
    class Selection
    {
        private List<Selectee> collection = new List<Selectee>();
        public int Count
        {
            get { return collection.Count; }
        }
        private SolidColorBrush rectangleBrush = new SolidColorBrush(Colors.Black);
        private DoubleCollection rectangleDashArray = new DoubleCollection(4);
        private Canvas adornerLayer;
        //
        public bool Empty
        {
            get { return collection.Count == 0; }
        }
        public bool Contains(Shape shape)
        {
            foreach (Selectee selectee in collection)
            {
                if (selectee.shape == shape)
                    return true;
            }
            return false;
        }
        //
        public Selection(Canvas layer)
        {
            adornerLayer = layer;
        }
        //
        public void Add(Shape shape)
        {
            Shape adorner = CreateAdorner(shape);
            collection.Add(new Selectee(shape, adorner));
            adornerLayer.Children.Add(adorner);
        }
        private Shape CreateAdorner(Shape shape)
        {
            Vector vector = VisualTreeHelper.GetOffset(shape);
            //
            Rectangle adorner = new Rectangle();
            adorner.Width = shape.Width;
            adorner.Height = shape.Height;
            adorner.Stroke = rectangleBrush;
            adorner.StrokeThickness = 1;
            rectangleDashArray.Add(5);
            rectangleDashArray.Add(3);
            adorner.StrokeDashArray = rectangleDashArray;
            adorner.StrokeDashOffset = 0;
            adorner.SetValue(Canvas.LeftProperty, vector.X);
            adorner.SetValue(Canvas.TopProperty, vector.Y);
            //
            return adorner;
        }
        public void Clear()
        {
            if (collection.Count == 0)
                return;
            //else
            foreach (Selectee selectee in collection)
            {
                adornerLayer.Children.Remove(selectee.adorner);
            }
            collection.Clear();
        }
        public void DeltaMove(Vector delta)
        {
            foreach (Selectee selectee in collection)
            {
                DeltaMove(selectee.shape, delta);
                DeltaMove(selectee.adorner, delta);
            }
        }
        private void DeltaMove(Shape shape, Vector delta)
        {
            Vector vector = VisualTreeHelper.GetOffset(shape);
            Vector position = vector + delta;
            shape.SetValue(Canvas.LeftProperty, position.X);
            shape.SetValue(Canvas.TopProperty, position.Y);
        }
    }
    class SelectorTool : Tool
    {
        enum Mode {
            Single,
            Move,
            Multi
        }
        Mode mode = Mode.Single;
        Shape selectee = null;
        Selection selection;
        private Point dragOrigin;
        private Point moveOrigin;
        private Rectangle rectangle = new Rectangle();
        private SolidColorBrush rectangleBrush = new SolidColorBrush(Colors.Black);
        private DoubleCollection rectangleDashArray = new DoubleCollection(4);
        //
        private const int moveToleranceX = 5;
        private const int moveToleranceY = 5;
        //
        public SelectorTool(CitiesControl ctrl) : base(ctrl)
        {
            selection = new Selection(ctrl.adornCanvas);
            //
            rectangle.Stroke = rectangleBrush;
            rectangle.StrokeThickness = 1;
            rectangleDashArray.Add(5);
            rectangleDashArray.Add(3);
            rectangle.StrokeDashArray = rectangleDashArray;
            rectangle.StrokeDashOffset = 0;
            control.adornCanvas.Children.Add(rectangle);
        }
        public override void Activate()
        {
            control.MouseUp += OnMouseUp;
            control.MouseDown += OnMouseDown;
            control.MouseMove += OnMouseMove;
        }
        public override void Deactivate()
        {
            control.MouseUp -= OnMouseUp;
            control.MouseDown -= OnMouseDown;
            control.MouseMove -= OnMouseMove;
        }
        private void ShowRectangle()
        {
            if (rectangle.IsVisible)
                return;
            //control.adornCanvas.Children.Add(rectangle);
            rectangle.Visibility = Visibility.Visible;
        }
        private void HideRectangle()
        {
            if (!rectangle.IsVisible)
                return;
            //control.adornCanvas.Children.Remove(rectangle);
            rectangle.Visibility = Visibility.Hidden;
        }
        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (!control.IsMouseCaptured)
            {
                //!!!:What a pain to figure out.  This must be called before capture.
                SingleSelect(sender, e);
                //!!!
                control.CaptureMouse();
                control.Cursor = Cursors.Cross;
            }
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            //Release the mouse
            if (control.IsMouseCaptured)
            {
                //TODO:Check for Alt Key to enable adding selections
                if (mode != Mode.Move)
                    selection.Clear();
                //
                switch (mode)
                {
                    case Mode.Single:
                        {
                            if (selectee != null)
                            {
                                selection.Add(selectee);
                            }
                        }
                        break;
                    case Mode.Move:
                        break;
                    case Mode.Multi:
                        {
                            // Expand the hit test area by creating a geometry centered on the hit test point.
                            Vector vector = VisualTreeHelper.GetOffset(rectangle);
                            Rect rect = new Rect(new Point(vector.X, vector.Y), new Size(rectangle.Width, rectangle.Height));
                            RectangleGeometry expandedHitTestArea = new RectangleGeometry(rect);

                            // Set up a callback to receive the hit test result enumeration.
                            VisualTreeHelper.HitTest(control, null,
                                new HitTestResultCallback(MyHitTestResultCallback),
                                new GeometryHitTestParameters(expandedHitTestArea));
                        }
                        break;
                }
                control.ReleaseMouseCapture();
                control.Cursor = Cursors.Arrow;
                mode = Mode.Single;
                selectee = null;
                HideRectangle();
            }
        }
        private HitTestResultBehavior MyHitTestResultCallback(HitTestResult result)
        {
            // Add the hit test result to the list that will be processed after the enumeration.
            if (result.VisualHit is Shape)
            {
                Shape shape = (Shape)result.VisualHit;
                //TODO:Maybe I should be using a filter.
                if(shape is Ellipse)
                    selection.Add(shape);
            }
            // Set the behavior to return visuals at all z-order levels.
            return HitTestResultBehavior.Continue;
        }
        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            if (control.IsMouseCaptured)
            {
                switch (mode)
                {
                    case Mode.Single:
                        SingleSelect(sender, e);
                        break;
                    case Mode.Move:
                        MoveSelect(sender, e);
                        break;
                    case Mode.Multi:
                        MultiSelect(sender, e);
                        break;
                }
            }
            else
                dragOrigin = e.GetPosition(control); ;
        }
        private void SingleSelect(object sender, MouseEventArgs e)
        {
            Point pt = e.GetPosition(control);

            if(selectee != null)
            {
                if (Math.Abs(pt.X - dragOrigin.X) > moveToleranceX ||
                    Math.Abs(pt.Y - dragOrigin.Y) > moveToleranceY)
                {
                    if (!selection.Contains(selectee))
                    {
                        selection.Clear();
                        selection.Add(selectee);
                    }
                    mode = Mode.Move;
                    moveOrigin = pt;
                }
                return;
            }
            HitTestResult result = VisualTreeHelper.HitTest(control, pt);
            if (result != null && result.VisualHit is Ellipse)
            {
                Shape shape = (Shape)result.VisualHit;
                if (selectee != null || selectee == shape)
                    return;
                //else
                selectee = shape;
                return;
            }
            selection.Clear();
            mode = Mode.Multi;
        }
        private void MoveSelect(object sender, MouseEventArgs e)
        {
            Point p1 = e.GetPosition(control);
            selection.DeltaMove(p1 - moveOrigin);
            moveOrigin = p1;
        }
        private void MultiSelect(object sender, MouseEventArgs e)
        {
            Point p1 = e.GetPosition(control);
            ShowRectangle();
            //Calculate the top left corner of the rectangle 
            //regardless of drag direction
            double x = dragOrigin.X < p1.X ? dragOrigin.X : p1.X;
            double y = dragOrigin.Y < p1.Y ? dragOrigin.Y : p1.Y;
            //
            rectangle.SetValue(Canvas.LeftProperty, x);
            rectangle.SetValue(Canvas.TopProperty, y);
            //
            rectangle.Width = Math.Abs(e.GetPosition(control).X - dragOrigin.X);
            rectangle.Height = Math.Abs(e.GetPosition(control).Y - dragOrigin.Y);
        }
    }
    class CreatorTool : Tool
    {
        public CreatorTool(CitiesControl ctrl)
            : base(ctrl)
        {
        }
        public override void Activate()
        {
            control.MouseDown += OnMouseDown;
            control.Cursor = Cursors.Pen;
        }
        public override void Deactivate()
        {
            control.MouseDown -= OnMouseDown;
            control.Cursor = Cursors.Arrow;
        }

        private void OnMouseDown(object sender, MouseButtonEventArgs e)
        {
            control.CreateCity(e.GetPosition(control));
        }
    }
}

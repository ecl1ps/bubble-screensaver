using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;

namespace Bubbles
{
    public class VisualArea : FrameworkElement
    {
        private readonly VisualCollection children;
        private readonly ExtraDrawingVisual drawingVisual;

        public VisualArea()
        {
            children = new VisualCollection(this);
            drawingVisual = new ExtraDrawingVisual();
            children.Add(drawingVisual);
        }

        protected override void OnRender(DrawingContext dc)
        {
            foreach (Drawing drawing in drawingVisual.DrawingChildren)
                dc.DrawDrawing(drawing);
        }

        protected override int VisualChildrenCount
        {
            get { return children.Count; }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index < 0 || index >= children.Count)
            {
                throw new ArgumentOutOfRangeException();
            }

            return children[index];
        }


        public void Add(Drawing elem)
        {
            drawingVisual.DrawingChildren.Add(elem);
        }


        public void Clear()
        {
            drawingVisual.DrawingChildren.Clear();
        }

        public void RunRender()
        {
            Dispatcher.Invoke(InvalidateVisual, DispatcherPriority.Send);
        }
    }
    public class ExtraDrawingVisual : DrawingVisual
    {
        private readonly DrawingCollection dc = new DrawingCollection();
        public DrawingCollection DrawingChildren { get { return dc; } }
    }
}

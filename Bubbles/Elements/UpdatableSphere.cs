using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Shapes;

namespace Bubbles.Elements
{
    public class UpdatableSphere : IUpdatable
    {
        private readonly DrawingGroup geometryElement;
        private Vector position;
        private Vector direction;
        private readonly double radius;
        private readonly double speed;
        private double BoxMult = 1.25;
        private bool FreeRange = false;
        private Random escapeChance = new Random();
        private int outsideFence = 800;
        private int CapturePct = 96;

        private double saveX;
        private double saveY;
        private double bigWidth;
        private double bigHeight;
        private double minLeft;
        private double maxRight;
        private double minTop;
        private double maxBottom;
        private double fenceLeft;
        private double fenceRight;
        private double fenceTop;
        private double fenceBottom;

        public UpdatableSphere(Size bounds, DrawingGroup ellipse, Vector position, Vector direction, double speed, double radius)
        {
            this.geometryElement = ellipse;
            this.position = position;
            this.direction = direction;
            this.direction.Normalize();
            this.speed = speed;
            this.radius = radius;

            bigWidth = bounds.Width * BoxMult;
            bigHeight = bounds.Height * BoxMult;
            minLeft = 0 - (bigWidth - bounds.Width) / 2;
            maxRight = bounds.Width + (bigWidth - bounds.Width) / 2;
            minTop = 0 - (bigHeight - bounds.Height) / 2;
            maxBottom = bounds.Height + (bigHeight - bounds.Height) / 2;

            fenceLeft = minLeft - outsideFence;
            fenceRight = maxRight + outsideFence;
            fenceTop = minTop - outsideFence;
            fenceBottom = maxBottom + outsideFence;
        }

        public void Update(Size bounds, float tpf)
        {
            if (FreeRange) { checkForRecapture(bounds); }
            CheckOutOfBounds(bounds);
            position += direction * speed * tpf;
        }

        private void checkForRecapture(Size bounds)
        {
            var pos = GetPosition();

            bool recaptured = true;
            if (pos.X - radius < 0) { recaptured = false; }
            if (pos.X + radius > bounds.Width) { recaptured = false; }
            if (pos.Y - radius < 0) { recaptured = false; }
            if (pos.Y + radius > bounds.Height) { recaptured = false; }

            if (recaptured) { FreeRange = false; }
        }

        private void CheckOutOfBounds(Size bounds)
        {
            if (FreeRange) 
            { 
                CheckDistantBounds(bounds);
                return;
            }
            var pos = GetPosition();

            saveX = direction.X;
            saveY = direction.Y;
            if (pos.X - radius <= minLeft && direction.X < 0) { direction.X *= -1; }
            if (pos.X + radius >= maxRight && direction.X > 0) { direction.X *= -1; }

            if (pos.Y - radius <= minTop && direction.Y < 0) { direction.Y *= -1; }
            if (pos.Y + radius >= maxBottom && direction.Y > 0) { direction.Y *= -1; }

            if (saveX != direction.X || saveY != direction.Y)
            {
                if (escapeChance.Next(100) > CapturePct) { FreeRange = true; }
            }
        }

        private void CheckDistantBounds(Size bounds)
        {
            var pos = GetPosition();

            if (pos.X - radius <= fenceLeft && direction.X < 0) { direction.X *= -1; }
            if (pos.X + radius >= fenceRight && direction.X > 0) { direction.X *= -1; }

            if (pos.Y - radius <= fenceTop && direction.Y < 0) { direction.Y *= -1; }
            if (pos.Y + radius >= fenceBottom && direction.Y > 0) { direction.Y *= -1; }
        }

        public void UpdateGeometric()
        {
            ((TransformGroup) geometryElement.Transform).Children.Clear();
            ((TransformGroup) geometryElement.Transform).Children.Add(new TranslateTransform(position.X, position.Y));
        }

        public Vector GetPosition()
        {
            return new Vector(position.X + radius, position.Y + radius);
        }

        public void SetPosition(Vector newPosition)
        {
            position = new Vector(newPosition.X - radius, newPosition.Y - radius);
        }
    }
}

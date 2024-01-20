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

        public UpdatableSphere(Size bounds, DrawingGroup ellipse, Vector position, Vector direction, double speed, double radius)
        {
            this.geometryElement = ellipse;
            this.position = position;
            this.direction = direction;
            this.direction.Normalize();
            this.speed = speed;
            this.radius = radius;
        }

        public void Update(Size bounds, float tpf)
        {
            CheckOutOfBounds(bounds);
            position += direction * speed * tpf;
        }

        private void CheckOutOfBounds(Size bounds)
        {
            if (FreeRange) 
            { 
                CheckDistantBounds(bounds);
                return;
            }
            var pos = GetPosition();
            double saveX = direction.X;
            double saveY = direction.Y;

            double bigWidth = bounds.Width * BoxMult;
            double bigHeight = bounds.Height * BoxMult;
            double minLeft = 0 - (bigWidth - bounds.Width) / 2;
            double maxRight = bounds.Width + (bigWidth - bounds.Width) / 2;
            double minTop = 0 - (bigHeight - bounds.Height) / 2;
            double maxBottom = bounds.Height + (bigHeight - bounds.Height) / 2;

            if (pos.X - radius <= minLeft && direction.X < 0) { direction.X *= -1; }
            if (pos.X + radius >= maxRight && direction.X > 0) { direction.X *= -1; }

            if (pos.Y - radius <= minTop && direction.Y < 0) { direction.Y *= -1; }
            if (pos.Y + radius >= maxBottom && direction.Y > 0) { direction.Y *= -1; }

            if (saveX != direction.X || saveY != direction.Y)
            {
                if (escapeChance.Next(100) > 95) { FreeRange = true; }
            }
        }

        private void CheckDistantBounds(Size bounds)
        {
            return;
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

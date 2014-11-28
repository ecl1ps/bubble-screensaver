using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Shapes;

namespace Bubbles.Elements
{
    public class MovableSphere : IMovable
    {
        private Ellipse ellipse;
        private Vector position;
        private Vector direction;
        private double radius;
        private double speed;
        private bool collidedInLastUpdate;

        public MovableSphere(Size bounds, Ellipse ellipse, Vector position, Vector direction, double speed)
        {
            this.ellipse = ellipse;
            this.position = position;
            this.direction = direction;
            this.direction.Normalize();
            this.speed = speed;
            radius = ellipse.Width / 2;
            collidedInLastUpdate = false;

            UpdatePosition();
        }

        public void Update(Size bounds)
        {
            CheckOutOfBounds(bounds);
            position += direction * speed;
            Application.Current.Dispatcher.BeginInvoke(new Action(UpdatePosition));
        }

        private void CheckOutOfBounds(Size bounds)
        {
            var pos = GetPosition();

            var collides = false;

            if (pos.X - radius <= 0 || pos.X + radius >= bounds.Width)
            {
                if (!collidedInLastUpdate)
                    direction.X *= -1;
                collides = true;
            }

            if (pos.Y - radius <= 0 || pos.Y + radius >= bounds.Height)
            {
                if (!collidedInLastUpdate)
                    direction.Y *= -1;
                collides = true;
            }

            collidedInLastUpdate = collides;
        }

        public void UpdatePosition()
        {
            Canvas.SetLeft(ellipse, position.X);
            Canvas.SetTop(ellipse, position.Y);
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

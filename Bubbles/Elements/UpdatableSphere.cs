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
        private bool collidedInLastUpdate;

        public UpdatableSphere(Size bounds, DrawingGroup ellipse, Vector position, Vector direction, double speed, double radius)
        {
            this.geometryElement = ellipse;
            this.position = position;
            this.direction = direction;
            this.direction.Normalize();
            this.speed = speed;
            this.radius = radius;
            collidedInLastUpdate = false;
        }

        public void Update(Size bounds, float tpf)
        {
            CheckOutOfBounds(bounds);
            position += direction * speed * tpf;
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

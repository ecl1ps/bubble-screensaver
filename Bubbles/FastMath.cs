namespace Bubbles
{
    public class FastMath
    {
        public static float LinearInterpolate(float from, float to, float percentage)
        {
            return from + (to - from) * percentage;
        }
    }
}

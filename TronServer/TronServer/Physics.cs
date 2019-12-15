using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Physics
{
    public static class Physics
    {
        public static bool intersect()
        {
            return true;
        }

        public static float Distance(Vector2 first, Vector2 second)
        {
            return (float)Math.Sqrt(Math.Pow(second.x - first.x, 2) + Math.Pow(second.y - first.y, 2));
        }
    }

    public struct Vector2
    {
        public float x { get; set; }
        public float y { get; set; }
    }

    public struct Line
    {
        public Vector2 start { 
            get {
                return start;
            } set {
                start = value;
                mag = Physics.Distance(value, end);
            }
        }
        public Vector2 end {
            get {
                return end;
            } set {
                end = value;
                mag = Physics.Distance(start, value);
            }
        }
        public float magnitude {
            get {
                return mag;
            }
        }
        private float mag { get; set; }
    }

    public struct Circle
    {
        public Vector2 center { get; set; }
        public float radius { get; set; }
        public float diameter { get { return radius * 2; } set { diameter = value; radius = value / 2; } }
        public float area { get { return area; } set { area = value; } }
    }

    public struct Rectangle
    {

    }
}

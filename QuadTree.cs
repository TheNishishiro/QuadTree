using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace QuadTree
{
    class QuadRectangle<T>
    {
        public float X, Y, Width, Height;

        public QuadRectangle(float x, float y, float width, float height)
        {
            this.X = x;
            this.Y = y;
            this.Width = width;
            this.Height = height;
        }

        public bool BoundryContains(QuadTreeElement<T> p)
        {
            return
                p.X >= X &&
                p.X <= X + Width &&
                p.Y >= Y &&
                p.Y <= Y + Height;
        }

        public bool Intersects(QuadRectangle<T> rect)
        {
            return !(
                rect.X - rect.Width > X + Width ||
                rect.X + rect.Width < X - Width ||
                rect.Y - rect.Height > Y + Height ||
                rect.Y + rect.Height < Y - Height
                );
        }
    }

    class QuadTreeElement<T>
    {
        public int X, Y;
        public T userData;

        public QuadTreeElement(int x, int y, T userData)
        {
            this.X = x;
            this.Y = y;
            this.userData = userData;
        }
    }


    class QuadTree<T>
    {
        QuadTree<T> topLeft = null, topRight = null, bottomLeft = null, bottomRight = null;
        QuadRectangle<T> boundry;
        int capacity;
        bool subdivided;
        List<QuadTreeElement<T>> points;

        public QuadTree(QuadRectangle<T> boundry, int capacity)
        {
            this.capacity = capacity;
            this.boundry = boundry;
            points = new List<QuadTreeElement<T>>();
            subdivided = false;
        }

        public void Subdivide()
        {
            topLeft = new QuadTree<T>(new QuadRectangle<T>(
                boundry.X, 
                boundry.Y, 
                boundry.Width/2, 
                boundry.Height/2), capacity);
            topRight = new QuadTree<T>(new QuadRectangle<T>(
                boundry.X + (boundry.Width/2), 
                boundry.Y, 
                boundry.Width / 2, 
                boundry.Height / 2), capacity);
            bottomLeft = new QuadTree<T>(new QuadRectangle<T>(
                boundry.X, 
                boundry.Y + (boundry.Height/2), 
                boundry.Width / 2, 
                boundry.Height / 2), capacity);
            bottomRight = new QuadTree<T>(new QuadRectangle<T>(
                boundry.X + (boundry.Width/2), 
                boundry.Y + (boundry.Height/2),  
                boundry.Width / 2, 
                boundry.Height / 2), capacity);
            subdivided = true;
        }

        public void Draw(PaintEventArgs e)
        {
            e.Graphics.DrawRectangle(
                new Pen(Color.White, 1), new Rectangle((int)boundry.X, (int)boundry.Y, (int)boundry.Width, (int)boundry.Height));

            foreach (QuadTreeElement<T> p in points)
                e.Graphics.DrawEllipse(
                    new Pen(Color.White, 1),
                    p.X-1, p.Y-1, 2, 2
                    );

            if (subdivided)
            {
                topLeft.Draw(e);
                topRight.Draw(e);
                bottomLeft.Draw(e);
                bottomRight.Draw(e);
            }
        }

        public List<QuadTreeElement<T>> Query(QuadRectangle<T> range, List<QuadTreeElement<T>> result = null)
        {
            if (result == null)
                result = new List<QuadTreeElement<T>>();

            if (!boundry.Intersects(range))
                return null;

            foreach (QuadTreeElement<T> point in points)
            {
                if(range.BoundryContains(point))
                    result.Add(point);
            }

            if (subdivided)
            {
                topLeft.Query(range, result);
                topRight.Query(range, result);
                bottomLeft.Query(range, result);
                bottomRight.Query(range, result);
            }

            return result;
        }

        public bool Insert(QuadTreeElement<T> p)
        {
            if (!boundry.BoundryContains(p))
                return false;

            if(points.Count < capacity)
            {
                points.Add(p);
                return true;
            }
            else
            {
                if(!subdivided)
                    Subdivide();

                if (topLeft.Insert(p))
                    return true;
                else if (topRight.Insert(p))
                    return true;
                else if (bottomLeft.Insert(p))
                    return true;
                else if (bottomRight.Insert(p))
                    return true;

                return false;
            }
        }
    }
}

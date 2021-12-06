using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class Point
{
    public int x;
    public int y;

    public Point(int x, int y)
    {
        this.x = x;
        this.y = y;
    }

    public void Multi(int m)
    {
        x *= m;
        y *= m;
    }
    public void Add(Point p)
    {
        x += p.x;
        y += p.y;
    }
    public Vector2 toVector()
    {
        return new Vector2(x, y);
    }

    public bool Equals(Point p)
    {
        return (x == p.x && y == p.y);
    }
    public static Point fromVector(Vector2 v)
    {
        return new Point((int)v.x, (int)v.y);
    }
    public static Point fromVector(Vector3 v)
    {
        return new Point((int)v.x, (int)v.y);
    }
    public static Point Multi(Point p, int m)
    {
        return new Point(p.x * m, p.y * m);
    }
    public static Point Add(Point p, Point o)
    {
        return new Point(p.x + o.x, p.y + o.y);
    }
    public static Point Clone(Point p)
    {
        return new Point(p.x, p.y);
    }
    public static Point Zero
    {
        get { return new Point(0, 0); }
    }
    public static Point Up
    {
        get { return new Point(0, 1); }
    }
    public static Point Down
    {
        get { return new Point(0, -1); }
    }
    public static Point Left
    {
        get { return new Point(-1, 0); }
    }
    public static Point Right
    {
        get { return new Point(1, 0); }
    }
    public static Point One
    {
        get { return new Point(1, 1); }
    }
}

public class EqualityComparer : IEqualityComparer<Point>
{
    public bool Equals(Point a, Point b)
    {
        return ((a.x.Equals(b.x)) && (a.y.Equals(b.y)));
    }
    public int GetHashCode(Point x)
    {
        return x.x.GetHashCode() ^ x.y.GetHashCode();
    }
}

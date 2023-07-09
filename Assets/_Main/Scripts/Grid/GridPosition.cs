using System;

public struct GridPosition : IEquatable<GridPosition>
{
    public int X;
    public int Z;

    public GridPosition(int x, int z)
    {
        X = x;
        Z = z;
    }

    public override bool Equals(object obj)
    {
        return obj is GridPosition position &&
               X == position.X &&
               Z == position.Z;
    }

    public bool Equals(GridPosition other)
    {
        return this == other;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(X, Z);
    }

    public override string ToString()
    {
        return $"x : {X}; z: {Z}";
    }

    public static bool operator ==(GridPosition left, GridPosition right)
    {
        return left.X == right.X && left.Z == right.Z;
    }

    public static bool operator !=(GridPosition left, GridPosition right)
    {
        return !(left == right);
    }
}

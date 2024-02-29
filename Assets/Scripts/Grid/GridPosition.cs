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

    public override bool Equals(object obj) => obj is GridPosition position && X == position.X && Z == position.Z;

    public bool Equals(GridPosition other) => this == other;

    public override int GetHashCode() => HashCode.Combine(X, Z);

    public override string ToString() => $"x: {X}; z: {Z}";

    public static bool operator ==(GridPosition left, GridPosition right) => left.X == right.X && left.Z == right.Z;
    public static bool operator !=(GridPosition left, GridPosition right) => !(left == right);

    public static GridPosition operator +(GridPosition left, GridPosition right) => new GridPosition(left.X + right.X, left.Z + right.Z);
    public static GridPosition operator -(GridPosition left, GridPosition right) => new GridPosition(left.X - right.X, left.Z - right.Z);
}

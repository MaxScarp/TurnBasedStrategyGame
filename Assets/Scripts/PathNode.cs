public class PathNode
{
    public GridPosition GridPosition { get; private set; }
    public int GCost { get; set; }
    public int HCost { get; set; }
    public int FCost { get => GCost + HCost; }
    public PathNode CameFromPathNode { get; set; }
    public bool IsWalkable { get; set; }

    public PathNode(GridPosition gridPosition)
    {
        GridPosition = gridPosition;
        IsWalkable = true;
    }

    public override string ToString() => GridPosition.ToString();
}

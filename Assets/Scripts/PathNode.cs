public class PathNode
{
    public GridPosition GridPosition { get; private set; }
    public int GCost { get; set; }
    public int HCost { get; set; }
    public int FCost { get => GCost + HCost; }
    public PathNode CameFromPathNode { get; set; }

    public PathNode(GridPosition gridPosition)
    {
        GridPosition = gridPosition;
    }

    public override string ToString() => GridPosition.ToString();
}

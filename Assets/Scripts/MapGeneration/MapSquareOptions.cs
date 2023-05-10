using System.Linq;
using System.Collections.Generic;
using System;

public class MapSquareOptions
{
    private List<int> options;
    public int X { get; private set; }
    public int Y { get; private set; }

    public bool IsCollapsed { get { return options.Count == 1; }}

    public int TileId { get { return options.First(); }}

    public int NumberOfPossibilities { get { return options.Count; }}

    public IReadOnlyList<int> Options { get { return options; }}

    public MapSquareOptions(int x, int y, List<int> options)
    {
        this.options = options;
        this.X = x;
        this.Y = y;
    }

    public void Choose(int option)
    {
        options = new List<int> { option };
    }

    public bool Restrict(IEnumerable<int> allowed)
    {
        int cnt = options.Count;
        options = options.Intersect(allowed).ToList();

        if(options.Count == 0)
            throw new Exception("Wave function collapse failed - impossible tile combination found");

        return cnt != options.Count;
    }
}

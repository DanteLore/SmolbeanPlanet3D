using System.Linq;
using System.Collections.Generic;
using System;

public class MapSquareOptions
{
    private List<string> options;
    public int X { get; private set; }
    public int Y { get; private set; }

    public bool IsCollapsed { get { return options.Count == 1; }}

    public String TileName { get { return options.First(); }}

    public int NumberOfPossibilities { get { return options.Count; }}

    public IReadOnlyList<String> Options { get { return options; }}

    public MapSquareOptions(int x, int y, List<String> options)
    {
        this.options = options;
        this.X = x;
        this.Y = y;
    }

    public void Choose(String option)
    {
        options = new List<string> { option };
    }

    public bool Restrict(IEnumerable<String> allowed)
    {
        int cnt = options.Count;
        options = options.Intersect(allowed).ToList();

        if(options.Count == 0)
            throw new Exception("Wave function collapse failed - impossible tile combination found");

        return cnt != options.Count;
    }
}

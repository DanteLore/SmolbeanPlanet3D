using System.Linq;
using System.Collections.Generic;
using System;
using UnityEngine;

public class MapSquareOptions
{
    private int[] options;
    public int X { get; private set; }
    public int Y { get; private set; }

    public bool IsCollapsed { get { return options.Length == 1; }}

    public int TileId { get { return options.First(); }}

    public int NumberOfPossibilities { get { return options.Length; }}

    public IReadOnlyList<int> Options { get { return options; }}

    public MapSquareOptions(int x, int y, int[] options)
    {
        this.options = options;
        this.X = x;
        this.Y = y;
    }

    public void Choose(int option)
    {
        options = new int[] { option };
    }

    public bool Restrict(IEnumerable<int> allowed)
    {
        int cnt = options.Length;
        var newOptions = options.Intersect(allowed).ToArray();

        if(options.Length == 0)
        {
            Debug.Log($"Options[{options.Count()}] {string.Join(", ", options)}");
            Debug.Log($"Allowed[{allowed.Count()}] {string.Join(", ", allowed)}");
            throw new Exception("Wave function collapse failed - impossible tile combination found");
        }

        options = newOptions;
        return cnt != options.Length;
    }
}

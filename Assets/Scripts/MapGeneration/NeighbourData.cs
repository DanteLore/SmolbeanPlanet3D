using System;
using System.Collections.Generic;

[Serializable]
public class NeighbourData
{
    public int id;
    public string name;
    public List<int> leftMatches;
    public List<int> rightMatches;
    public List<int> frontMatches;
    public List<int> backMatches;
    public int backLeftLevel;
    public int backRightLevel;
    public int frontleftLevel;
    public int frontRightLevel;
}


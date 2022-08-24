namespace HoloBrawl.Terrain;

/// <summary>
/// Used to store Terrain data, the big stuff is in Floor
/// </summary>
public sealed class Terrain
{
    public string Name { get; }
    public Floor[] Floors { get; }
    
    public Terrain(string name, Floor[] floors)
    {
        Name = name;
        Floors = floors;
    }
}
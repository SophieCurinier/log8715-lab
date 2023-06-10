using System.Collections.Generic;

public struct Entity 
{
    public uint Id { get;  set; }
    public List<IComponent> components { get; set; }
}
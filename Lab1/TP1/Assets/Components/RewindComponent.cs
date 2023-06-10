using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;

public struct RewindArchetype
{
    public PositionComponent PositionComponent;
    public ColorComponent ColorComponent;
    public SizeComponent SizeComponent;
}

public struct RewindComponent : IComponent 
{
    public Dictionary<float, RewindArchetype> RecordedData;
}
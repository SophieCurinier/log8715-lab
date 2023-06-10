using UnityEngine;
using System.Collections;

public class ColorSystem : ISystem
{
    public ColorSystem() { Name = "Color"; }

    public string Name { get; }

    public void UpdateSystem()
    {
        var manager = ECSManager.Instance;
        int explosionSize = manager.Config.explosionSize;
        ColorComponent colorComponent ;
        colorComponent.Color = Color.white;
        
        World.ForEach<PositionComponent>((uint entityId, PositionComponent positionComponent) =>
        {
            if (World.IsEntityTagged<IsStaticTag>(entityId))
            {
                colorComponent.Color = Color.red;
            }
            else 
            {
                bool hasProtectionComponent = World.IsEntityTagged<ProtectionComponent>(entityId);
                if (hasProtectionComponent)
                {
                    if (World.GetComponent<ProtectionComponent>(entityId).ProtectionDuration > 0)
                    {
                        colorComponent.Color = Color.green;
                    }
                    else if ((World.GetComponent<ProtectionComponent>(entityId).ProtectionDuration == 0) && (World.GetComponent<ProtectionComponent>(entityId).ProtectionCooldown > 0))
                    {
                        colorComponent.Color = new Color(218f/255f, 247f/255f, 166f/255f);
                    }
                }
                else
                {
                    SizeComponent sizeComponent = World.GetComponent<SizeComponent>(entityId);
                    if (sizeComponent.Size + 1 == explosionSize)
                    {
                        colorComponent.Color = new Color(242f/255f, 140f/255f, 40f/255f);
                    }
                    else if (World.IsEntityTagged<IsClickedTag>(entityId))
                    {
                        colorComponent.Color = Color.magenta;
                    }
                    else
                    {
                        colorComponent.Color = Color.blue;
                    }
                }
            }
            manager.UpdateShapeColor(entityId, colorComponent.Color);
            World.SetComponentData<ColorComponent>(entityId, colorComponent);
        });
    }
}
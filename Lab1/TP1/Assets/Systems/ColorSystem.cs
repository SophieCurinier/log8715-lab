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
        Color entityColor ;
        
        World.ForEach<PositionComponent>((uint entityId, PositionComponent positionComponent) =>
        {
            if (World.IsEntityTagged<IsStaticTag>(entityId))
            {
                entityColor = Color.red;
                manager.UpdateShapeColor(entityId, entityColor);
            }
            else 
            {
                bool hasProtectionComponent = World.IsEntityTagged<ProtectionComponent>(entityId);
                if (hasProtectionComponent)
                {
                    if (World.GetComponent<ProtectionComponent>(entityId).ProtectionDuration > 0)
                    {
                        entityColor = Color.green;
                        manager.UpdateShapeColor(entityId, entityColor);
                    }
                    else if ((World.GetComponent<ProtectionComponent>(entityId).ProtectionDuration == 0) && (World.GetComponent<ProtectionComponent>(entityId).ProtectionCooldown > 0))
                    {
                        entityColor = new Color(218f/255f, 247f/255f, 166f/255f);
                        manager.UpdateShapeColor(entityId, entityColor);
                    }
                }
                else
                {
                    SizeComponent sizeComponent = World.GetComponent<SizeComponent>(entityId);
                    if (sizeComponent.Size + 1 == explosionSize)
                    {
                        entityColor = new Color(242f/255f, 140f/255f, 40f/255f);
                        manager.UpdateShapeColor(entityId, entityColor);
                    }
                    else
                    {
                        entityColor = Color.blue;
                        manager.UpdateShapeColor(entityId, entityColor);
                    }
                }
            }
        });
    }
}
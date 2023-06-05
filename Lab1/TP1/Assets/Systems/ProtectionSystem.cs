using UnityEngine;
using System.Collections;

public class ProtectionSystem : ISystem
{
    public ProtectionSystem() { Name = "Protection"; }

    public string Name { get; }

    private void becomeProtect(int protectionSize, uint entityId, int entitySize, float protectionProbability, float protectionDuration, float protectionCooldown)
    {
        if ((entitySize <= protectionSize))
        {
            float random = Random.Range(0f, 1f);
            if (random <= protectionProbability)
            {
                ProtectionComponent protectionComponent;
                protectionComponent.ProtectionDuration = protectionDuration;
                protectionComponent.ProtectionCooldown = protectionCooldown;
                World.AddComponent<ProtectionComponent>(entityId, protectionComponent);
            }
        }
    }

    private void hasProtectionComponent(uint entityId)
    {
        ProtectionComponent protectionComponent = World.GetComponent<ProtectionComponent>(entityId);

        if (protectionComponent.ProtectionDuration > 0)
        {
            protectionComponent.ProtectionDuration = Mathf.Max(protectionComponent.ProtectionDuration - Time.deltaTime, 0);
        } 
        else if ((protectionComponent.ProtectionDuration == 0) && (protectionComponent.ProtectionCooldown > 0))
        {
            protectionComponent.ProtectionCooldown = Mathf.Max(protectionComponent.ProtectionCooldown - Time.deltaTime, 0);
            
            if (protectionComponent.ProtectionCooldown == 0)
            {
                World.DeleteComponent<ProtectionComponent>(entityId);
            }
        }
    }

    public void UpdateSystem()
    {
        var manager = ECSManager.Instance;

        int protectionSize  = manager.Config.protectionSize;
        float protectionProbability = manager.Config.protectionProbability;
        float protectionDuration = manager.Config.protectionDuration;
        float protectionCooldown = manager.Config.protectionCooldown;
        
        World.ForEach<SizeComponent>((uint entityId, SizeComponent sizeComponent) => 
        {
            if (!(World.IsEntityTagged<ProtectionComponent>(entityId)) && !(World.IsEntityTagged<IsStaticTag>(entityId)))
            {
                becomeProtect(protectionSize, entityId, sizeComponent.Size, protectionProbability, protectionDuration, protectionCooldown);
            }
            else 
            {
                hasProtectionComponent(entityId);
            }
        });
    }
}
using UnityEngine;
using System.Collections;

public class ExplosionSystem : ISystem
{
    public ExplosionSystem() { Name = "Explosion"; }

    public string Name { get; }

    private uint createSon(int size, Vector2 position, Vector2 velocity, float angle, ECSManager manager)
    {
        uint entitySon = World.CreateEntity();

        SizeComponent sizeSonComponent;
        sizeSonComponent.Size = size / 2;
        World.AddComponent<SizeComponent>(entitySon, sizeSonComponent);

        PositionComponent positionSonComponent;
        positionSonComponent.Position.x = position.x + (int)Mathf.Pow(-1, entitySon)*size/2*Mathf.Cos(angle);
        positionSonComponent.Position.y = position.y + size/2*Mathf.Sin(angle);
        World.AddComponent<PositionComponent>(entitySon, positionSonComponent);

        VelocityComponent velocitySonComponent;
        velocitySonComponent.Velocity = (int)Mathf.Pow(-1, entitySon)*velocity;
        World.AddComponent<VelocityComponent>(entitySon, velocitySonComponent);

        manager.CreateShape(entitySon, sizeSonComponent.Size);
        manager.UpdateShapePosition(entitySon, positionSonComponent.Position);

        return entitySon;
    }

    public void UpdateSystem()
    {
        var manager = ECSManager.Instance;
        int explosionSize  = manager.Config.explosionSize;
        
        World.ForEach<SizeComponent>((uint entityId, SizeComponent sizeComponent) => 
        {
            if (sizeComponent.Size >= explosionSize)
            {
                PositionComponent positionComponent = World.GetComponent<PositionComponent>(entityId);
                VelocityComponent velocityComponent = World.GetComponent<VelocityComponent>(entityId);
                
                // Calcul entityId circle velocity orientation
                float angle = Mathf.Atan2(velocityComponent.Velocity.y, velocityComponent.Velocity.x);
                
                uint entitySon1 = createSon(sizeComponent.Size, positionComponent.Position, velocityComponent.Velocity, angle, manager);
                
                uint entitySon2 = createSon(sizeComponent.Size, positionComponent.Position, velocityComponent.Velocity, angle, manager);
                
                // Destroy Father
                manager.DestroyShape(entityId);
                World.DeleteEntity(entityId);
            }
        });
    }
}
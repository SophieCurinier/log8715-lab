using UnityEngine;
using System.Collections;

public class ExplosionSystem : ISystem
{
    public ExplosionSystem() { Name = "Explosion"; }

    public string Name { get; }

    public void UpdateSystem()
    {
        var manager = ECSManager.Instance;
        int explosionSize  = manager.Config.explosionSize;
        
        World.ForEach<SizeComponent>((uint entityId, SizeComponent sizeComponent) => 
        {
            if (sizeComponent.Size == explosionSize)
            {
                PositionComponent positionComponent = World.GetComponent<PositionComponent>(entityId);
                VelocityComponent velocityComponent = World.GetComponent<VelocityComponent>(entityId);
                
                // Calcul entityId circle velocity orientation
                float angle = Mathf.Atan2(velocityComponent.Velocity.y, velocityComponent.Velocity.x);
                
                // Add Son 1
                uint entitySon1 = World.CreateEntity();

                SizeComponent sizeSonComponent;
                sizeSonComponent.Size = sizeComponent.Size / 2;
                World.AddComponent<SizeComponent>(entitySon1, sizeSonComponent);

                PositionComponent positionSonComponent1;
                positionSonComponent1.Position.x = positionComponent.Position.x + sizeComponent.Size/2*Mathf.Cos(angle);
                positionSonComponent1.Position.y = positionComponent.Position.y + sizeComponent.Size/2*Mathf.Sin(angle);
                World.AddComponent<PositionComponent>(entitySon1, positionSonComponent1);

                VelocityComponent velocitySonComponent1;
                velocitySonComponent1.Velocity = velocityComponent.Velocity;
                World.AddComponent<VelocityComponent>(entitySon1, velocitySonComponent1);

                manager.CreateShape(entitySon1, sizeSonComponent.Size);
                manager.UpdateShapePosition(entitySon1, positionSonComponent1.Position);

                // Add Son 2
                uint entitySon2 = World.CreateEntity();

                World.AddComponent<SizeComponent>(entitySon2, sizeSonComponent);

                PositionComponent positionSonComponent2;
                positionSonComponent2.Position.x = positionComponent.Position.x - sizeComponent.Size/2*Mathf.Cos(angle);
                positionSonComponent2.Position.y = positionComponent.Position.y - sizeComponent.Size/2*Mathf.Sin(angle);
                World.AddComponent<PositionComponent>(entitySon2, positionSonComponent2);

                VelocityComponent velocitySonComponent2;
                velocitySonComponent2.Velocity = velocityComponent.Velocity * -1;
                World.AddComponent<VelocityComponent>(entitySon2, velocitySonComponent2);

                manager.CreateShape(entitySon2, sizeSonComponent.Size);
                manager.UpdateShapePosition(entitySon2, positionSonComponent2.Position);
                
                // Destroy Father
                manager.DestroyShape(entityId);
                World.DeleteEntity(entityId);
            }
        });
    }
}
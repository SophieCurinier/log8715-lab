using UnityEngine;
using System;

public class MovementSystem : ISystem
{
    public MovementSystem() { Name = "Movement"; }

    public string Name { get; }

    public void UpdateSystem()
    {
        var manager = ECSManager.Instance;

        World.ForEach<VelocityComponent>((uint entityId, VelocityComponent velocityComponent) =>
        {
            if (!World.IsEntityTagged<IsStaticTag>(entityId))
            {
                PositionComponent positionComponent = World.GetComponent<PositionComponent>(entityId);
                positionComponent.Position.x += velocityComponent.Velocity.x * Time.deltaTime;
                positionComponent.Position.y += velocityComponent.Velocity.y * Time.deltaTime;
                World.SetComponentData<PositionComponent>(entityId, positionComponent);
                manager.UpdateShapePosition(entityId, positionComponent.Position);
            }
        });
    }
}
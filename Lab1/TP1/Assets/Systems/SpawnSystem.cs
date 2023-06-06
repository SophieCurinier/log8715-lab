using UnityEngine;

public class SpawnSystem : ISystem
{
    public SpawnSystem() { Name = "Spawn"; }

    public string Name { get; }

    private bool _initialized = false;

    private void HandleStatic(uint entityId, Vector2 velocity)
    {
        if ((velocity.x == 0) && (velocity.y == 0))
        {
            IsStaticTag isStaticTag ;
            World.AddComponent<IsStaticTag>(entityId, isStaticTag);
        }

        return;
    }

    public void UpdateSystem()
    {
        if (_initialized)
        {
            return;
        }

        var manager = ECSManager.Instance;
        var instancesToSpawn = manager.Config.circleInstancesToSpawn;

        foreach (var instance in instancesToSpawn)
        {
            uint entity = World.CreateEntity();

            SizeComponent sizeComponent;
            sizeComponent.Size = instance.initialSize;
            World.AddComponent<SizeComponent>(entity, sizeComponent);

            PositionComponent positionComponent;
            positionComponent.Position = instance.initialPosition;
            World.AddComponent<PositionComponent>(entity, positionComponent);

            VelocityComponent velocityComponent;
            velocityComponent.Velocity = instance.initialVelocity;
            World.AddComponent<VelocityComponent>(entity, velocityComponent);

            HandleStatic(entity, velocityComponent.Velocity);

            manager.CreateShape(entity, sizeComponent.Size);
            manager.UpdateShapePosition(entity, positionComponent.Position);
        }

        _initialized = true;
    }
}
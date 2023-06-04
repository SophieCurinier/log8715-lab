using UnityEngine;

public class SpawnSystem : ISystem
{
    public SpawnSystem() { Name = "Spawn"; }

    public string Name { get; }

    private bool _initialized = false;

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

            manager.CreateShape(entity, sizeComponent.Size);
            manager.UpdateShapePosition(entity, positionComponent.Position);
        }

        _initialized = true;
    }
}
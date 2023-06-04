using UnityEngine;

public class BoundaryCollision : ISystem
{
    public BoundaryCollision() { Name = "BoundaryCollision"; }

    public string Name { get; }

    public void UpdateSystem()
    {
        var manager = ECSManager.Instance;

        Camera mainCamera = Camera.main;
        float screenHeight = mainCamera.orthographicSize * 2f;
        float screenWidth = screenHeight * mainCamera.aspect;

        World.ForEach<VelocityComponent>((uint entityId, VelocityComponent velocityComponent) =>
        {
            if (!World.IsEntityTagged<IsStaticTag>(entityId))
            {
                PositionComponent positionComponent = World.GetComponent<PositionComponent>(entityId);
                SizeComponent sizeComponent = World.GetComponent<SizeComponent>(entityId);

                bool outOfX = (Mathf.Abs((positionComponent.Position.x)) >= Mathf.Abs(screenWidth-sizeComponent.Size)*0.5);
                bool outOfY = (Mathf.Abs((positionComponent.Position.y)) >= Mathf.Abs(screenHeight-sizeComponent.Size)*0.5);

                if (outOfX)
                {
                    velocityComponent.Velocity.x *= -1;
                    World.SetComponentData<VelocityComponent>(entityId, velocityComponent);

                }
                
                if (outOfY)
                {
                    velocityComponent.Velocity.y *= -1;
                    World.SetComponentData<VelocityComponent>(entityId, velocityComponent);
                }
            }
        });
    }
}
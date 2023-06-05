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

                bool outOfX = ((positionComponent.Position.x-(float)sizeComponent.Size/2 < -screenWidth/2f) || (positionComponent.Position.x+(float)sizeComponent.Size/2 > screenWidth/2f));
                bool outOfY = ((positionComponent.Position.y-(float)sizeComponent.Size/2 < -screenHeight/2f) || (positionComponent.Position.y+(float)sizeComponent.Size/2 > screenHeight/2f));

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
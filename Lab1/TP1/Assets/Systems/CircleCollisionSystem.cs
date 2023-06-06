using UnityEngine;

public class CircleCollisionSystem : ISystem
{
    public CircleCollisionSystem() { Name = "CircleCollision"; }

    public string Name { get; }

    private CollisionResult DetectCollision(Vector2 position1, Vector2 velocity1, int size1, Vector2 position2, Vector2 velocity2, int size2, CollisionResult collisionResult)
    {
        collisionResult = CollisionUtility.CalculateCollision(position1, velocity1, size1, position2, velocity2, size2);

        return collisionResult;
    }

    private void UpdatePositionAndVelocity(uint entityId, PositionComponent positionComponent, VelocityComponent velocityComponent, ECSManager manager)
    {
        World.SetComponentData<PositionComponent>(entityId, positionComponent);
        World.SetComponentData<VelocityComponent>(entityId, velocityComponent);
        manager.UpdateShapePosition(entityId, positionComponent.Position);
    }

    private void UpdateSize(uint entityId, SizeComponent sizeComponent, ECSManager manager)
    {
        if (sizeComponent.Size == 0)
        {
            World.DeleteEntity(entityId);
            manager.DestroyShape(entityId);
        }
        else 
        {
            World.SetComponentData<SizeComponent>(entityId, sizeComponent);
            manager.UpdateShapeSize(entityId, sizeComponent.Size);
        }
    }

    private float getVelocityOrientation(uint entityId)
    {
        VelocityComponent velocityComponent = World.GetComponent<VelocityComponent>(entityId);

        float angle = Mathf.Atan2(velocityComponent.Velocity.y, velocityComponent.Velocity.x);
        
        return angle;
    }

    private void HandleSize(uint entityId1, SizeComponent sizeComponent1, uint entityId2, SizeComponent sizeComponent2, ECSManager manager)
    {
        bool hasDifferentSize = sizeComponent1.Size != sizeComponent2.Size;
        bool isStatic1 = World.IsEntityTagged<IsStaticTag>(entityId1);
        bool isStatic2 = World.IsEntityTagged<IsStaticTag>(entityId2);

        if (hasDifferentSize && (!isStatic1) && (!isStatic2))
        {
            uint entityLarger, entitySmaller;
            SizeComponent sizeLarger, sizeSmaller;

            if (sizeComponent1.Size > sizeComponent2.Size)
            {
                entityLarger = entityId1;
                sizeLarger = sizeComponent1;

                entitySmaller = entityId2;
                sizeSmaller = sizeComponent2;
            }
            else
            {
                entityLarger = entityId2;
                sizeLarger = sizeComponent2;

                entitySmaller = entityId1;
                sizeSmaller = sizeComponent1;
            }

            bool hasProtectionComponentLarger = World.IsEntityTagged<ProtectionComponent>(entityLarger);
            bool isProtectedLarger = hasProtectionComponentLarger && (World.GetComponent<ProtectionComponent>(entityLarger).ProtectionDuration > 0);

            bool hasProtectionComponentSmaller = World.IsEntityTagged<ProtectionComponent>(entitySmaller);
            bool isProtectedSmaller = hasProtectionComponentSmaller && (World.GetComponent<ProtectionComponent>(entitySmaller).ProtectionDuration > 0);

            if (isProtectedLarger)
            {
                return;
            }
            else if (isProtectedSmaller)
            {
                float angle = getVelocityOrientation(entityLarger);

                PositionComponent positionComponentLarger = World.GetComponent<PositionComponent>(entityLarger);
                positionComponentLarger.Position.x -= Mathf.Cos(angle);
                positionComponentLarger.Position.y -= Mathf.Sin(angle);

                World.SetComponentData<PositionComponent>(entityLarger, positionComponentLarger);
                manager.UpdateShapePosition(entityLarger, positionComponentLarger.Position);
                
                sizeLarger.Size ++;
            } 
            else
            {
                sizeLarger.Size ++;
                sizeSmaller.Size --;
            }

            UpdateSize(entityLarger, sizeLarger, manager);
            UpdateSize(entitySmaller, sizeSmaller, manager);
        }
    }

    public void UpdateSystem()
    {
        var manager = ECSManager.Instance;

        World.ForEach<PositionComponent>((uint entityId1, PositionComponent positionComponent1) =>
        {
            World.ForEach<PositionComponent>((uint entityId2, PositionComponent positionComponent2) =>
            {
                VelocityComponent velocityComponent1 = World.GetComponent<VelocityComponent>(entityId1);
                SizeComponent sizeComponent1 = World.GetComponent<SizeComponent>(entityId1);

                VelocityComponent velocityComponent2 = World.GetComponent<VelocityComponent>(entityId2);
                SizeComponent sizeComponent2 = World.GetComponent<SizeComponent>(entityId2);

                if (entityId1 < entityId2)
                { 
                    CollisionResult collisionResult = CollisionUtility.CalculateCollision(positionComponent1.Position, velocityComponent1.Velocity, sizeComponent1.Size, positionComponent2.Position, velocityComponent2.Velocity, sizeComponent2.Size);
                    bool isInCollision = (collisionResult != null);
                    bool isNotAlreadyInCollision = (!(World.IsEntityTagged<IsInCollisionTag>(entityId1)) && !(World.IsEntityTagged<IsInCollisionTag>(entityId2)));

                    if (isInCollision)
                    {
                        IsInCollisionTag isInCollisionTag ;
                        World.AddComponent<IsInCollisionTag>(entityId1, isInCollisionTag);
                        World.AddComponent<IsInCollisionTag>(entityId2, isInCollisionTag);

                        positionComponent1.Position = collisionResult.position1;
                        velocityComponent1.Velocity = collisionResult.velocity1;
                        UpdatePositionAndVelocity(entityId1, positionComponent1, velocityComponent1, manager);

                        positionComponent2.Position = collisionResult.position2;
                        velocityComponent2.Velocity = collisionResult.velocity2;
                        UpdatePositionAndVelocity(entityId1, positionComponent1, velocityComponent1, manager);

                        if (isNotAlreadyInCollision)
                        {
                            HandleSize(entityId1, sizeComponent1, entityId2, sizeComponent2, manager);
                        }
                    }  
                    else if (!isInCollision && (World.IsEntityTagged<IsInCollisionTag>(entityId1)) && (World.IsEntityTagged<IsInCollisionTag>(entityId2)))
                    {
                        World.DeleteComponent<IsInCollisionTag>(entityId1);
                        World.DeleteComponent<IsInCollisionTag>(entityId2);
                    }
                }
            });
        });
    }
}
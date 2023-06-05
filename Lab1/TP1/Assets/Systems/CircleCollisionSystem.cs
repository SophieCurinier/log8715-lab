using UnityEngine;

public class CircleCollisionSystem : ISystem
{
    public CircleCollisionSystem() { Name = "CircleCollision"; }

    public string Name { get; }

    public void UpdateSystem()
    {
        var manager = ECSManager.Instance;

        World.ForEach<PositionComponent>((uint entityId1, PositionComponent positionComponent1) =>
        {
            VelocityComponent velocityComponent1 = World.GetComponent<VelocityComponent>(entityId1);
            SizeComponent sizeComponent1 = World.GetComponent<SizeComponent>(entityId1);

            World.ForEach<PositionComponent>((uint entityId2, PositionComponent positionComponent2) =>
            {
                VelocityComponent velocityComponent2 = World.GetComponent<VelocityComponent>(entityId2);
                SizeComponent sizeComponent2 = World.GetComponent<SizeComponent>(entityId2);
                if (entityId1 != entityId2)
                {
                    CollisionResult collisionResult = CollisionUtility.CalculateCollision(positionComponent1.Position, velocityComponent1.Velocity, sizeComponent1.Size, positionComponent2.Position, velocityComponent2.Velocity, sizeComponent2.Size);
                    
                    bool isInCollision = (collisionResult != null);
                    
                    if (isInCollision && !(World.IsEntityTagged<IsInCollisionTag>(entityId1)) && !(World.IsEntityTagged<IsInCollisionTag>(entityId2)))
                    {
                        IsInCollisionTag isInCollisionTag ;
                        World.AddComponent<IsInCollisionTag>(entityId1, isInCollisionTag);
                        World.AddComponent<IsInCollisionTag>(entityId2, isInCollisionTag);

                        if (!(World.IsEntityTagged<IsStaticTag>(entityId1)) && !(World.IsEntityTagged<IsStaticTag>(entityId2)))
                        {
                            if (sizeComponent2.Size != sizeComponent1.Size)
                            {
                                if (sizeComponent2.Size < sizeComponent1.Size)
                                {
                                    sizeComponent1.Size ++ ;
                                    sizeComponent2.Size -- ;
                                } 
                                else
                                {
                                    sizeComponent1.Size -- ;
                                    sizeComponent2.Size ++ ;
                                }

                                World.SetComponentData<SizeComponent>(entityId1, sizeComponent1);
                                World.SetComponentData<SizeComponent>(entityId2, sizeComponent2);

                                manager.UpdateShapeSize(entityId1, sizeComponent1.Size);
                                manager.UpdateShapeSize(entityId2, sizeComponent2.Size);
                            }
                        }
                        
                        positionComponent1.Position = collisionResult.position1;
                        velocityComponent1.Velocity = collisionResult.velocity1;
                        positionComponent2.Position = collisionResult.position2;
                        velocityComponent2.Velocity = collisionResult.velocity2;

                        World.SetComponentData<PositionComponent>(entityId1, positionComponent1);
                        World.SetComponentData<VelocityComponent>(entityId1, velocityComponent1);
                        World.SetComponentData<PositionComponent>(entityId2, positionComponent2);
                        World.SetComponentData<VelocityComponent>(entityId2, velocityComponent2);

                        manager.UpdateShapePosition(entityId1, positionComponent1.Position);
                        manager.UpdateShapePosition(entityId2, positionComponent2.Position);
                    }
                    else 
                    {
                        World.DeleteComponent<IsInCollisionTag>(entityId1);
                        World.DeleteComponent<IsInCollisionTag>(entityId2);
                    }
                }
            });
        });
    }
}
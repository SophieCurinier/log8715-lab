using UnityEngine;

public class CircleCollisionSystem : ISystem
{
    public CircleCollisionSystem() { Name = "CircleCollision"; }

    public string Name { get; }

    private void modifyPositionAndVelocity(uint entityId, PositionComponent positionComponent, VelocityComponent velocityComponent, ECSManager manager)
    {
        World.SetComponentData<PositionComponent>(entityId, positionComponent);
        World.SetComponentData<VelocityComponent>(entityId, velocityComponent);

        manager.UpdateShapePosition(entityId, positionComponent.Position);
    }

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
                    
                    bool hasCollision = (collisionResult != null);
                    
                    if (hasCollision && !(World.IsEntityTagged<IsInCollisionTag>(entityId1)) && !(World.IsEntityTagged<IsInCollisionTag>(entityId2)))
                    {
                        IsInCollisionTag isInCollisionTag ;
                        World.AddComponent<IsInCollisionTag>(entityId1, isInCollisionTag);
                        World.AddComponent<IsInCollisionTag>(entityId2, isInCollisionTag);

                        positionComponent1.Position = collisionResult.position1;
                        velocityComponent1.Velocity = collisionResult.velocity1;
                        positionComponent2.Position = collisionResult.position2;
                        velocityComponent2.Velocity = collisionResult.velocity2;

                        bool entity1HasProtection = World.IsEntityTagged<ProtectionComponent>(entityId1);
                        bool entity1IsProtected = (entity1HasProtection && (World.GetComponent<ProtectionComponent>(entityId1).ProtectionDuration > 0));
                        
                        bool entity2HasProtection = World.IsEntityTagged<ProtectionComponent>(entityId2);
                        bool entity2IsProtected = (entity2HasProtection && (World.GetComponent<ProtectionComponent>(entityId2).ProtectionDuration > 0));

                        if (!(World.IsEntityTagged<IsStaticTag>(entityId1)) && !(World.IsEntityTagged<IsStaticTag>(entityId2)))
                        {
                            if (sizeComponent1.Size != sizeComponent2.Size)
                            {
                                if ((sizeComponent1.Size > sizeComponent2.Size))
                                {
                                    if (!entity2IsProtected)
                                    {
                                        sizeComponent2.Size -- ;
                                    }
                                    else
                                    {
                                        float angle = Mathf.Atan2(velocityComponent1.Velocity.y, velocityComponent1.Velocity.x);
                                        positionComponent1.Position.x -= Mathf.Cos(angle);
                                        positionComponent1.Position.y -= Mathf.Sin(angle); 
                                    }
                                    sizeComponent1.Size ++ ;
                                } 
                                else
                                {
                                    if (!entity1IsProtected)
                                    {
                                        sizeComponent1.Size -- ;
                                    }
                                    else
                                    {
                                        float angle = Mathf.Atan2(velocityComponent2.Velocity.y, velocityComponent2.Velocity.x);
                                        positionComponent2.Position.x -= Mathf.Cos(angle);
                                        positionComponent2.Position.y -= Mathf.Sin(angle);
                                    }
                                    sizeComponent2.Size ++ ;
                                }

                                World.SetComponentData<SizeComponent>(entityId1, sizeComponent1);
                                World.SetComponentData<SizeComponent>(entityId2, sizeComponent2);
                            }
                            
                            modifyPositionAndVelocity(entityId1, positionComponent1, velocityComponent1, manager);
                            modifyPositionAndVelocity(entityId2, positionComponent2, velocityComponent2, manager);

                            manager.UpdateShapeSize(entityId1, sizeComponent1.Size);
                            manager.UpdateShapeSize(entityId2, sizeComponent2.Size);
                        }

                        if (World.GetComponent<SizeComponent>(entityId1).Size == 0)
                        {
                            manager.DestroyShape(entityId1);
                            World.DeleteEntity(entityId1);
                        }
                        else if (World.GetComponent<SizeComponent>(entityId2).Size == 0)
                        {
                            manager.DestroyShape(entityId2);
                            World.DeleteEntity(entityId2);
                        }
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
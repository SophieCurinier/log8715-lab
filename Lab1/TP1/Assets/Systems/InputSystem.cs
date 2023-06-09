using UnityEngine;
using System.Collections.Generic;

public class InputSystem : ISystem
{
    public InputSystem() { Name = "Input"; }

    public string Name { get; }

    public void UpdateSystem()
    {
        var manager = ECSManager.Instance;

        if (Input.GetMouseButtonDown(0))
        {
            Vector2 clickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            World.ForEach<PositionComponent>((uint entityId, PositionComponent positionComponent) =>
            {
                SizeComponent sizeComponent = World.GetComponent<SizeComponent>(entityId);

                bool isInCircle = (Mathf.Pow((clickPosition.x - positionComponent.Position.x),2) + Mathf.Pow(clickPosition.y - positionComponent.Position.y,2) <= Mathf.Pow(sizeComponent.Size/2,2));
                bool isDynamic = !(World.IsEntityTagged<IsStaticTag>(entityId));

                if (isInCircle && isDynamic)
                {
                    IsClickedTag isClickedTag;
                    World.AddComponent<IsClickedTag>(entityId, isClickedTag);
                }
            });
        }

        if (Input.GetKeyDown("space"))
        {
            List<IsRewindComponent> isRewindComponents = World.GetComponents<IsRewindComponent>();
            
            if (isRewindComponents.Count == 0)
            {
                uint rewindEntity = World.CreateEntity();
                
                IsRewindComponent isRewindComponent;
                isRewindComponent.RewindDuration = 3f;

                World.AddComponent<IsRewindComponent>(rewindEntity, isRewindComponent);
            }
            else if (isRewindComponents.Count == 1)
            {
                World.ForEach<IsRewindComponent>((uint entityId, IsRewindComponent isRewindComponent) =>
                {
                    Debug.Log("Rewind cooldown : " + isRewindComponent.RewindDuration);
                    isRewindComponent.RewindDuration = 0f;
                    World.SetComponentData<IsRewindComponent>(entityId, isRewindComponent);
                });
            }
        }
    }
}
using UnityEngine;

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

                if (isInCircle)
                {
                    IsClickedTag isClickedTag;
                    World.AddComponent<IsClickedTag>(entityId, isClickedTag);
                }
            });
        }
    }
}
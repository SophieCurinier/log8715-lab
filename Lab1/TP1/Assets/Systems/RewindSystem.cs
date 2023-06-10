using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

public class RewindSystem : ISystem
{
    public RewindSystem() { Name = "Rewind"; }

    public string Name { get; }

    private bool activateRewind(ECSManager manager)
    {
        bool isReplayActive = false;

        World.ForEach<IsRewindComponent>((uint entityId, IsRewindComponent isRewindComponent) =>
        {
            if (isRewindComponent.RewindDuration == 3f)
            {
                List<ISystem> allSystems = RegisterSystems.GetListOfSystems();

                foreach (var system in allSystems)
                {
                    if ((system.Name != "Rewind") && (system.Name != "Input"))
                    {
                        manager.Config.SystemsEnabled[system.Name] = false;
                    }
                }

                isRewindComponent.RewindDuration = Mathf.Max(isRewindComponent.RewindDuration-Time.deltaTime,0);
                World.SetComponentData<IsRewindComponent>(entityId, isRewindComponent);

                isReplayActive = true;
            }
            else if (isRewindComponent.RewindDuration == 0f)
            {
                List<ISystem> allSystems = RegisterSystems.GetListOfSystems();

                foreach (var system in allSystems)
                {
                    if ((system.Name != "Rewind") && (system.Name != "Input"))
                    {
                        manager.Config.SystemsEnabled[system.Name] = true;
                    }
                }

                World.DeleteEntity(entityId);
            }
            else if (isRewindComponent.RewindDuration > 0f)
            {
                isRewindComponent.RewindDuration = Mathf.Max(isRewindComponent.RewindDuration-Time.deltaTime,0);
                World.SetComponentData<IsRewindComponent>(entityId, isRewindComponent);

                isReplayActive = true;
            }
        });

        return isReplayActive;
    }

    private void recordData()
    {
        float currentTime = Time.time;
        
        World.ForEach<RewindComponent>((uint entityId, RewindComponent rewindComponent) =>
        {
            // Remove old recorded data
            float thresholdTime = currentTime - 3;

            List<float> keysToRemove = rewindComponent.RecordedData.Keys
                .Where(time => time < thresholdTime)
                .ToList();

            foreach (float key in keysToRemove)
            {
                rewindComponent.RecordedData.Remove(key);
            }

            // Add new rewindArchetype
            PositionComponent positionComponent = World.GetComponent<PositionComponent>(entityId);
            SizeComponent sizeComponent = World.GetComponent<SizeComponent>(entityId);
            ColorComponent colorComponent = World.GetComponent<ColorComponent>(entityId);

            RewindArchetype newRewindArchetype ;
            newRewindArchetype.PositionComponent = positionComponent;
            newRewindArchetype.SizeComponent = sizeComponent;
            newRewindArchetype.ColorComponent = colorComponent;

            rewindComponent.RecordedData[currentTime] = newRewindArchetype;

            // Set RewindComponentData
            World.SetComponentData<RewindComponent>(entityId, rewindComponent);
        });
    }

    private void playRewind(bool isReaplyActive, ECSManager manager)
    {
        float currentTime = Time.time;
        float targetTime = currentTime - 3;

        if (isReaplyActive)
        {
            World.ForEach<RewindComponent>((uint entityId, RewindComponent rewindComponent) =>
            {
                float closestKey = rewindComponent.RecordedData.Keys.OrderBy(key => Mathf.Abs(key - targetTime)).FirstOrDefault();
                
                RewindArchetype rewindArchetype = rewindComponent.RecordedData[closestKey];

                manager.UpdateShapePosition(entityId, rewindArchetype.PositionComponent.Position);
                manager.UpdateShapeSize(entityId, rewindArchetype.SizeComponent.Size);
                manager.UpdateShapeColor(entityId, rewindArchetype.ColorComponent.Color);

            });
        }
    }

    public void UpdateSystem()
    {
        var manager = ECSManager.Instance;
        bool isReplayActive ;

        isReplayActive = activateRewind(manager);

        recordData();

        playRewind(isReplayActive, manager);

    }
}
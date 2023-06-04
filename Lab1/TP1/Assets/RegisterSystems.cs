using System.Collections.Generic;

public class RegisterSystems
{
    public static List<ISystem> GetListOfSystems()
    {
        // determine order of systems to add
        var toRegister = new List<ISystem>();
        
        // Add your systems here
        var spawnSystem = new SpawnSystem();
        var movementSystem = new MovementSystem();

        toRegister.Add(spawnSystem);
        toRegister.Add(movementSystem);

        return toRegister;
    }
}
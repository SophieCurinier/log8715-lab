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
        var boundaryCollisionSystem = new BoundaryCollisionSystem();
        var circleCollisionSystem = new CircleCollisionSystem();
        var explosionSystem = new ExplosionSystem();
        var ProtectionSystem = new ProtectionSystem();
        var ColorSystem = new ColorSystem();

        toRegister.Add(spawnSystem);
        toRegister.Add(movementSystem);
        toRegister.Add(boundaryCollisionSystem);
        toRegister.Add(circleCollisionSystem);
        toRegister.Add(explosionSystem);
        toRegister.Add(ProtectionSystem);
        toRegister.Add(ColorSystem);

        return toRegister;
    }
}
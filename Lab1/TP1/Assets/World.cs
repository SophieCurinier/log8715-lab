using System;
using System.Collections.Generic;
using System.Linq;

public static class World
{
    private static uint _nextEntityId = 0;
    private static Dictionary<Type, Dictionary<uint, IComponent>> _componentsByType = new Dictionary<Type, Dictionary<uint, IComponent>>();

    public static uint CreateEntity()
    {
        uint id = _nextEntityId++ ;
        return id;
    }

    public static void DeleteEntity(uint entityId)
    {
        foreach (var componentDictionnary in _componentsByType)
        {
            if (componentDictionnary.Value.ContainsKey(entityId))
            {
                componentDictionnary.Value.Remove(entityId);
            }
        }
    }

    public static void AddComponent<T>(uint entityId, T component) where T : IComponent
    {
        if (!_componentsByType.ContainsKey(typeof(T)))
        {
            _componentsByType[typeof(T)] = new Dictionary<uint, IComponent>();
        }
        _componentsByType[typeof(T)][entityId] = component;
    }
    
    public static void DeleteComponent<T>(uint entityId) where T : IComponent
    {
        if (_componentsByType.ContainsKey(typeof(T)) &&
            _componentsByType[typeof(T)].ContainsKey(entityId))
        {
            _componentsByType[typeof(T)].Remove(entityId);
        }
    }

    public static List<T> GetComponents<T>() where T : IComponent
    {
        if (!_componentsByType.ContainsKey(typeof(T)))
        {
            return new List<T>();
        }
        return _componentsByType[typeof(T)].Values.Cast<T>().ToList();
    }

    public static T GetComponent<T>(uint entityId) where T : IComponent
    {
        if (!_componentsByType.ContainsKey(typeof(T)) ||
            !_componentsByType[typeof(T)].ContainsKey(entityId))
        {
            return default(T);
        }
        return (T)_componentsByType[typeof(T)][entityId];
    }

    public static void ForEach<T>(Action<uint, T> action) where T : IComponent 
    {
        if (_componentsByType.ContainsKey(typeof(T)))
        {
            foreach(KeyValuePair <uint, IComponent> myValue in _componentsByType[typeof(T)].ToList() ){
                action(myValue.Key, (T)myValue.Value);
            }
        }
    }

    public static void SetComponentData<T>(uint entityId, T component) where T : IComponent
    {
        if (_componentsByType.ContainsKey(typeof(T)) &&
            _componentsByType[typeof(T)].ContainsKey(entityId))
        {
            _componentsByType[typeof(T)][entityId] = component;
        }
        else
        {
            if (!_componentsByType.ContainsKey(typeof(T)))
            {
                _componentsByType[typeof(T)] = new Dictionary<uint, IComponent>();
            }
            _componentsByType[typeof(T)][entityId] = component;
        }       
    }

    public static bool IsEntityTagged<T>(uint entityId) where T : IComponent
    {
        if (_componentsByType.ContainsKey(typeof(T)) &&
            _componentsByType[typeof(T)].ContainsKey(entityId))
        {
            return true;
        }
        return false;
    }
}
using System.Collections.Generic;

namespace Utilities
{
    public class FixedSystemEngine : ISystem
    {
        private readonly Dictionary<SystemTypes, ISystem> _systems = new();
        private readonly List<SystemTypes> _removedSystems = new();

        public void Update(float deltaTime)
        {
            foreach (var removedSystem in _removedSystems)
            {
                _systems.Remove(removedSystem);
            }
            _removedSystems.Clear();

            foreach (var system in _systems.Values)
            {
                system.Update(deltaTime);
            }
        }

        public T Get<T>(SystemTypes systemType) where T : ISystem
        {
            return (T)_systems[systemType];
        }

        public void Add(SystemTypes systemType, ISystem system)
        {
            _systems.Add(systemType, system);
        }
        
        public void Clear()
        {
            _systems.Clear();
        }

        public void Remove(SystemTypes type)
        {
            _removedSystems.Add(type);
        }
    }
}
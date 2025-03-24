using System.Collections.Generic;
using System.Linq;

namespace Global
{
    public class Registry<T> where T: IRegistrableData
    {
        private Dictionary<string, RegistryEntry> _entries;
        
        public void CreateRegistry()
        {
            var entryFields = GetType().GetFields();
            _entries = new Dictionary<string, RegistryEntry>();

            foreach (var entryField in entryFields)
            {
                _entries.Add(entryField.Name, (RegistryEntry)entryField.GetValue(this));
            }
        }

        public RegistryEntry<T> GetByID(string id)
        {
            return _entries[id].Get<T>();
        }

        public RegistryEntry<T>[] GetAll()
        {
            return _entries.Values.Select(entry => entry.Get<T>()).ToArray();
        }
    }
}

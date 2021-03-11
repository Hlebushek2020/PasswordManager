using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PasswordManager.Classes
{
    public class SectionManager
    {
        private Dictionary<string, string> collection = new Dictionary<string, string>();

        public bool InStorage { get; set; } = false;
        public int Count { get => collection.Count; }

        public void AddItem(string param, string value)
        {
            collection.Add(param, value);
            if (InStorage)
                PasswordStorage.IsChanged = true;
        }

        public void AddItemNoChange(string param, string value) =>
            collection.Add(param, value);

        public void UpdateItem(string param, string value)
        {
            collection[param] = value;
            if (InStorage)
                PasswordStorage.IsChanged = true;
        }

        public void DeleteItem(string param)
        {
            collection.Remove(param);
            if (InStorage)
                PasswordStorage.IsChanged = true;
        }

        public bool ContainsItem(string param) =>
            collection.ContainsKey(param);

        public string GetItemValue(string param) =>
            collection[param];

        public IEnumerable<string> GetItemKeys() =>
            collection.Keys;

        public IEnumerable<KeyValuePair<string, string>> GetItems() =>
            collection;
    }
}

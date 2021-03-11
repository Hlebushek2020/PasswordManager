using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace PasswordManager.Classes
{
    public static class PasswordStorage
    {
        private static readonly Dictionary<string, SectionManager> collection = new Dictionary<string, SectionManager>();

        public static bool IsChanged { get; set; } = false;
        public static int Count { get => collection.Count; }

        public static void AddSection(string sectionName, SectionManager section)
        {
            collection.Add(sectionName, section);
            collection[sectionName].InStorage = true;
            IsChanged = true;
        }

        public static void AddSectionNoChange(string sectionName, SectionManager section)
        {
            collection.Add(sectionName, section);
            collection[sectionName].InStorage = true;
        }

        public static void RemoveSection(string sectionName)
        {
            collection[sectionName].InStorage = false;
            collection.Remove(sectionName);
            IsChanged = true;
        }

        public static IEnumerable<string> GetItemKeys() =>
            collection.Keys;

        public static IEnumerable<KeyValuePair<string, SectionManager>> GetItems() =>
            collection;

        public static SectionManager GetItem(string sectionName) =>
            collection[sectionName];

        public static bool Contains(string sectionName) =>
            collection.ContainsKey(sectionName);

        public static void Clear() =>
            collection.Clear();
    }
}
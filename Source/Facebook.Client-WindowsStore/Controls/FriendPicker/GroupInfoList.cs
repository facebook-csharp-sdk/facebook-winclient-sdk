namespace Facebook.Client.Controls
{
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;

    internal class GroupInfoList<T> : IGrouping<string, T>
    {
        private IEnumerable<T> items;

        public GroupInfoList(string key, IEnumerable<T> items)
        {
            this.Key = key;
            this.items = items ?? Enumerable.Empty<T>();
        }

        public string Key { get; private set; }

        public IEnumerator<T> GetEnumerator()
        {
            return items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return items.GetEnumerator();
        }
    }
}

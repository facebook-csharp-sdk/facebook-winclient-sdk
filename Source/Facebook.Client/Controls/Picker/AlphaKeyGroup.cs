namespace Facebook.Client.Controls
{
#if NETFX_CORE
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
#endif
#if WP8
    using System.Collections.Generic;
    using System.Globalization;
    using System.Linq;
    using Microsoft.Phone.Globalization;
#endif
    /// <summary>
    /// Helper class that is used to convert a flat list of data into a grouped list in which the entries are grouped by a key.
    /// </summary>
    /// <see href="http://msdn.microsoft.com/en-us/library/windowsphone/develop/jj244365%28v=vs.105%29.aspx"/>
    /// <typeparam name="T">The data type of the grouped list.</typeparam>
    internal class AlphaKeyGroup<T> : List<T>
    {
        private const string GlobeGroupKey = "\uD83C\uDF10";

        /// <summary>
        /// Initializes a new instance of the AlphaKeyGroup class.
        /// </summary>
        /// <param name="key">The key for this group.</param>
        public AlphaKeyGroup(string key)
        {
            this.Key = key;
        }

        /// <summary>
        /// Initializes a new instance of the AlphaKeyGroup class.
        /// </summary>
        /// <param name="grouping">The grouping object. N.B. this will enumerate all items.</param>
        public AlphaKeyGroup(IGrouping<string, T> grouping)
        {
            this.Key = grouping.Key;
            this.AddRange(grouping);
        }

        /// <summary>
        /// The delegate that will be used to obtain the key information.
        /// </summary>
        /// <param name="item">An object of type T</param>
        /// <returns>The key value to use for this object</returns>
        public delegate string GetKeyDelegate(T item);

        /// <summary>
        /// Gets the Key of this group.
        /// </summary>
        public string Key { get; private set; }

        /// <summary>
        /// Create a list of AlphaGroup&lt;T&gt; with keys set by a SortedLocaleGrouping.
        /// </summary>
        /// <param name="items">The items to place in the groups.</param>
        /// <param name="ci">The CultureInfo to group and sort by.</param>
        /// <param name="getKey">A delegate to get the key from an item.</param>
        /// <param name="sort">Will sort the data if true.</param>
        /// <returns>An items source for a LongListSelector</returns>
        public static List<AlphaKeyGroup<T>> CreateGroups(IEnumerable<T> items, CultureInfo ci, GetKeyDelegate getKey, bool sort)
        {
            SortedLocaleGrouping slg = new SortedLocaleGrouping(ci);
            List<AlphaKeyGroup<T>> list = CreateGroups(slg);

            foreach (T item in items)
            {
                int index = slg.GetGroupIndex(getKey(item));
                if (index >= 0 && index < list.Count)
                {
                    list[index].Add(item);
                }
            }

            if (sort)
            {
                foreach (AlphaKeyGroup<T> group in list)
                {
                    group.Sort((c0, c1) => { return ci.CompareInfo.Compare(getKey(c0), getKey(c1)); });
                }
            }

            return list;
        }

        /// <summary>
        /// Create a list of AlphaGroup&lt;T&gt; with keys set by a SortedLocaleGrouping.
        /// </summary>
        /// <param name="slg">The sorted group headers for the specified locale</param>
        /// <returns>The items source for a LongListSelector</returns>
        private static List<AlphaKeyGroup<T>> CreateGroups(SortedLocaleGrouping slg)
        {
            List<AlphaKeyGroup<T>> list = new List<AlphaKeyGroup<T>>();

            foreach (string key in slg.GroupDisplayNames)
            {
                if (key == "...")
                {
                    list.Add(new AlphaKeyGroup<T>(GlobeGroupKey));
                }
                else
                {
                    list.Add(new AlphaKeyGroup<T>(key));
                }
            }

            return list;
        }
    }
}
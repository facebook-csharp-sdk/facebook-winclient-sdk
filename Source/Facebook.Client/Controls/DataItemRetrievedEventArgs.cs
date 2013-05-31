namespace Facebook.Client.Controls
{
    /// <summary>
    /// Provides data for the DataItemRetrieved event.
    /// </summary>
    public class DataItemRetrievedEventArgs<T>
    {
        /// <summary>
        /// Initializes a new instance of the DataItemRetrievedEventArgs class.
        /// </summary>
        /// <param name="item">The item retrieved.</param>
        public DataItemRetrievedEventArgs(T item)
        {
            this.Item = item;
        }

        /// <summary>
        /// The item retrieved.
        /// </summary>
        public T Item { get; private set; }

        /// <summary>
        /// Controls whether the item is included in the list.
        /// </summary>
        public bool Exclude { get; set; }
    }
}

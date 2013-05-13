namespace Facebook.Client.Controls
{
    using System.Collections.Generic;

    /// <summary>
    /// Provides data for the LoadCompleted event.
    /// </summary>
    public class DataReadyEventArgs<T>
    {
        /// <summary>
        /// Initializes a new instance of the DataReadyEventArgs class.
        /// </summary>
        /// <param name="data">The data that was loaded.</param>
        public DataReadyEventArgs(IEnumerable<T> data)
        {
            this.Data = data;
        }

        /// <summary>
        /// Gets the list of Facebook friends retrieved.
        /// </summary>
        public IEnumerable<T> Data { get; private set; }
    }
}

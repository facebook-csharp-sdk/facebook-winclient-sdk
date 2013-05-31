namespace Facebook.Client
{
    using System.Collections.Generic;

    /// <summary>
    /// Base class for objects returned by the Facebook Graph API that provides access to its properties dynamically.
    /// </summary>
    public class GraphObject
    {
        private IDictionary<string, object> graphObject;

        /// <summary>
        /// Initializes a new instance of the GraphObject class.
        /// </summary>
        public GraphObject()
        {
            this.graphObject = new Dictionary<string, object>();
        }

        /// <summary>
        /// Initializes a new instance of the GraphObject class from a dynamic object returned by the Facebook API.
        /// </summary>
        /// <param name="graphObject">The dynamic object representing the Facebook object.</param>
        public GraphObject(IDictionary<string, object> graphObject)
        {
            this.graphObject = graphObject;
        }

        /// <summary>
        /// Gets the property of the object with the specified key.
        /// </summary>
        /// <param name="key"></param>
        /// <returns>The key of the property to get.</returns>
        public object this[string key]
        {
            get
            {
                return this.graphObject.ContainsKey(key) ? this.graphObject[key] : null;
            }
        }
    }
}

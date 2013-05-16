namespace Facebook.Client.Controls
{
    /// <summary>
    /// Specifies the selection mode of a Picker control.
    /// </summary>
    public enum PickerSelectionMode
    {
        /// <summary>
        /// The user can't select items.
        /// </summary>
        None = 0,

        /// <summary>
        /// The user can select a single item.
        /// </summary>
        Single = 1,

        /// <summary>
        /// The user can select multiple items.
        /// </summary>
        Multiple = 2,

#if NETFX_CORE
        /// <summary>
        /// The user can select multiple items. The selected items don't have to be contiguous.
        /// </summary>
        Extended = 3
#endif
    }
}

namespace Facebook.Client.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using System.Windows;
    using System.Windows.Controls;
    using Microsoft.Phone.Controls;

    /// <summary>
    /// Displays a list of selectable items with optional multi-selection.
    /// </summary>
    [TemplatePart(Name = PartListSelector, Type = typeof(LongListSelector))]
    public abstract class Picker<T> : Control
        where T : class
    {
        #region Part Definitions

        private const string PartListSelector = "PART_ListSelector";

        #endregion Part Definitions

        #region Default Property Values

        private const PickerSelectionMode DefaultSelectionMode = PickerSelectionMode.Multiple;

        #endregion Default Property Values

        #region Member variables

        private LongListSelector longListSelector;

        #endregion Member variables

        /// <summary>
        /// Initializes a new instance of the Picker class.
        /// </summary>
        public Picker()
        {
            this.SetValue(ItemsProperty, new ObservableCollection<T>());
            this.SetValue(SelectedItemsProperty, new ObservableCollection<T>());
        }

        #region Events

        /// <summary>
        /// Occurs when the current selection changes.
        /// </summary>
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        /// <summary>
        /// Occurs whenever a new item is about to be added to the list.
        /// </summary>
        public event EventHandler<DataItemRetrievedEventArgs<T>> DataItemRetrieved;

        /// <summary>
        /// Occurs when items in the the list have finished loading.
        /// </summary>
        public event EventHandler<DataReadyEventArgs<T>> LoadCompleted;

        /// <summary>
        /// Occurs whenever an error occurs while loading data.
        /// </summary>
        public event EventHandler<LoadFailedEventArgs> LoadFailed;

        #endregion Events

        #region Properties

        #region SelectionMode

        /// <summary>
        /// Gets or sets the selection behavior of the control. 
        /// </summary>
        public PickerSelectionMode SelectionMode
        {
            get { return (PickerSelectionMode)GetValue(SelectionModeProperty); }
            set { this.SetValue(SelectionModeProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectionMode dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register("SelectionMode", typeof(PickerSelectionMode), typeof(Picker<T>), new PropertyMetadata(DefaultSelectionMode, OnSelectionModeProperyChanged));

        private static void OnSelectionModeProperyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var picker = (Picker<T>)d;
            picker.ClearSelection();          
        }

        #endregion SelectionMode

        #region Items

        /// <summary>
        /// Gets the list of currently selected items for the FriendPicker control.
        /// </summary>
        public ObservableCollection<T> Items
        {
            get { return (ObservableCollection<T>)this.GetValue(ItemsProperty); }
            private set { this.SetValue(ItemsProperty, value); }
        }

        /// <summary>
        /// Identifies the Items dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(ObservableCollection<T>), typeof(Picker<T>), null);

        #endregion Items

        #region SelectedItem

        /// <summary>
        /// Gets the currently selected item for the Picker control.
        /// </summary>
        public T SelectedItem
        {
            get { return (T)GetValue(SelectedItemProperty); }
            private set { SetValue(SelectedItemProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectedItem dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemProperty =
            DependencyProperty.Register("SelectedItem", typeof(T), typeof(Picker<T>), new PropertyMetadata(null));

        #endregion SelectedItem

        #region SelectedItems

        /// <summary>
        /// Gets the list of currently selected items for the FriendPicker control.
        /// </summary>
        public ObservableCollection<T> SelectedItems
        {
            get { return (ObservableCollection<T>)this.GetValue(SelectedItemsProperty); }
            private set { this.SetValue(SelectedItemsProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectedItems dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(ObservableCollection<T>), typeof(Picker<T>), null);

        #endregion SelectedItems

        #endregion Properties

        #region Implementation

        /// <summary>
        /// Invoked whenever application code or internal processes (such as a rebuilding layout pass) call ApplyTemplate. In simplest 
        /// terms, this means the method is called just before a UI element displays in your app. Override this method to influence the 
        /// default post-template logic of a class. 
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            this.longListSelector = GetTemplateChild(Picker<T>.PartListSelector) as LongListSelector;
            if (this.longListSelector != null)
            {
                this.longListSelector.SelectionChanged += this.OnSelectionChanged;

                if (System.ComponentModel.DesignerProperties.IsInDesignTool)
                {
                    this.SetDataSource(this.GetDesignTimeData());
                }
            }
        }

        protected void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (this.SelectionMode == PickerSelectionMode.None)
            {
                return;
            }

            if (this.longListSelector == null)
            {
                return;
            }

            if (this.longListSelector.SelectedItem == null)
            {
                return;
            }

            IList<object> removedItems;
            IList<object> addedItems;

            if (this.SelectionMode == PickerSelectionMode.Single)
            {
                // Single Selection mode
                removedItems = ((IList<object>)e.RemovedItems)
                                    .Where(item => item != null)
                                    .ToList();
                addedItems = ((IList<object>)e.AddedItems)
                                    .Where(item => item != null)
                                    .ToList();                
            }
            else
            {
                // Multiple selection mode
                var selectedItem = this.longListSelector.SelectedItem as PickerItem<T>;
                addedItems = selectedItem.IsSelected ? new object[0] : new object[] { selectedItem };
                removedItems = selectedItem.IsSelected ? new object[] { selectedItem } : new object[0];

                // Reset selected item to null (no selection)
                this.longListSelector.SelectedItem = null;
            }

            foreach (var item in removedItems)
            {
                var pickerItem = (PickerItem<T>)item;
                pickerItem.IsSelected = false;          
                this.SelectedItems.Remove(pickerItem.Item);
            }

            foreach (var item in addedItems)
            {
                var pickerItem = (PickerItem<T>)item;
                pickerItem.IsSelected = true;
                this.SelectedItems.Add(pickerItem.Item);
            }

            this.SelectedItem = addedItems.Select(p => ((PickerItem<T>)p).Item).FirstOrDefault() 
                                    ?? this.SelectedItems.FirstOrDefault();

            this.SelectionChanged.RaiseEvent(
                this, 
                new SelectionChangedEventArgs(removedItems.Select(item => ((PickerItem<T>)item).Item).ToList(), addedItems.Select(item => ((PickerItem<T>)item).Item).ToList()));
        }

        protected void ClearSelection()
        {
            this.SelectedItems.Clear();

            if (this.longListSelector != null && this.longListSelector.ItemsSource != null)
            {
                var source = this.longListSelector.ItemsSource as IEnumerable<PickerItem<T>>;
                if (source == null)
                {
                    source = (this.longListSelector.ItemsSource as IEnumerable<AlphaKeyGroup<PickerItem<T>>>)
                        .SelectMany(i => i);
                }

                source.Where(f => f.IsSelected)
                    .ToList()
                    .ForEach(i => i.IsSelected = false);

                this.longListSelector.SelectedItem = null;
            }
        }

        protected void SetDataSource(IEnumerable<T> items)
        {
            if (this.longListSelector != null)
            {
                this.longListSelector.ItemsSource = this.GetData(items);
            }
        }

        protected async Task RefreshData()
        {
            this.Items.Clear();
            this.SelectedItems.Clear();
            this.SetValue(SelectedItemProperty, null);

            try
            {
                await LoadData();
            }
            catch (Exception ex)
            {
                // TODO: review the types of exception that should be caught here
                this.LoadFailed.RaiseEvent(this, new LoadFailedEventArgs("Error loading data.", ex.Message));
            }

            this.SetDataSource(this.Items);
            this.LoadCompleted.RaiseEvent(this, new DataReadyEventArgs<T>(this.Items.ToList()));
        }

        protected virtual IEnumerable<T> GetDesignTimeData()
        {
            return null;
        }

        protected void OnLoadCompleted(DataReadyEventArgs<T> args)
        {
            this.LoadCompleted.RaiseEvent(this, args);
        }

        protected void OnLoadFailed(LoadFailedEventArgs args)
        {
            this.LoadFailed.RaiseEvent(this, args);
        }

        protected bool OnDataItemRetrieved(DataItemRetrievedEventArgs<T> args, Func<DataItemRetrievedEventArgs<T>, bool> cancelInvocation)
        {
            return this.DataItemRetrieved.RaiseEvent(this, args, cancelInvocation);
        }

        protected abstract IList GetData(IEnumerable<T> items);

        protected abstract Task LoadData();
        
        #endregion Implementation
    }
}
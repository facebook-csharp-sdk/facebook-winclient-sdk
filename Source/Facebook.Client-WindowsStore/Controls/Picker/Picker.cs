namespace Facebook.Client.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Input;
    using Windows.UI.Xaml.Media;

    /// <summary>
    /// Displays a list of selectable items with optional multi-selection.
    /// </summary>
    [TemplatePart(Name = PartSemanticZoom, Type = typeof(SemanticZoom))]
    public abstract class Picker<T> : Control
        where T : class
    {
        #region Part Definitions

        private const string PartSemanticZoom = "PART_SemanticZoom";

        #endregion Part Definitions

        #region Default Property Values

        private const PickerSelectionMode DefaultSelectionMode = PickerSelectionMode.Multiple;
        private static readonly Brush DefaultGroupHeaderBackground = null;
        private static readonly Brush DefaultGroupHeaderForeground = null;

        #endregion Default Property Values

        #region Member variables

        private SemanticZoom semanticZoom;

        #endregion Member variables

        #region Events

        /// <summary>
        /// Occurs whenever a new item is about to be added to the list.
        /// </summary>
        public event EventHandler<DataItemRetrievedEventArgs<T>> DataItemRetrieved;

        /// <summary>
        /// Occurs when the items in the list have finished loading.
        /// </summary>
        public event EventHandler<DataReadyEventArgs<T>> LoadCompleted;

        /// <summary>
        /// Occurs whenever an error occurs while loading data.
        /// </summary>
        public event EventHandler<LoadFailedEventArgs> LoadFailed;

        #endregion Events

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

        #region GroupHeaderForeground

        /// <summary>
        /// Gets or sets the foreground brush used for the group headers.
        /// </summary>
        public Brush GroupHeaderForeground
        {
            get { return (Brush)GetValue(GroupHeaderForegroundProperty); }
            set { this.SetValue(GroupHeaderForegroundProperty, value); }
        }

        /// <summary>
        /// Identifies the GroupHeaderForeground dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupHeaderForegroundProperty =
            DependencyProperty.Register("GroupHeaderForeground", typeof(Brush), typeof(Picker<T>), new PropertyMetadata(DefaultGroupHeaderForeground));

        #endregion GroupHeaderForeground

        #region GroupHeaderBackground

        /// <summary>
        /// Gets or sets the background brush used for the group headers.
        /// </summary>
        public Brush GroupHeaderBackground
        {
            get { return (Brush)GetValue(GroupHeaderBackgroundProperty); }
            set { this.SetValue(GroupHeaderBackgroundProperty, value); }
        }

        /// <summary>
        /// Identifies the GroupHeaderBackground dependency property.
        /// </summary>
        public static readonly DependencyProperty GroupHeaderBackgroundProperty =
            DependencyProperty.Register("GroupHeaderBackground", typeof(Brush), typeof(Picker<T>), new PropertyMetadata(DefaultGroupHeaderBackground));

        #endregion GroupHeaderBackground

        #region Items

        /// <summary>
        /// Gets the list of currently selected items for the Picker control.
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
        /// Gets the list of currently selected items for the Picker control.
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
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.semanticZoom != null)
            {
                var view = this.semanticZoom.ZoomedInView as ListViewBase;
                if (view != null)
                {
                    view.SelectionChanged -= this.OnSelectionChanged;
                }

                this.semanticZoom.Tapped -= this.OnSemanticZoomTapped;
            }

            this.semanticZoom = this.GetTemplateChild(Picker<T>.PartSemanticZoom) as SemanticZoom;
            if (this.semanticZoom != null)
            {
                var view = this.semanticZoom.ZoomedInView as ListViewBase;
                if (view != null)
                {
                    view.SelectionChanged += this.OnSelectionChanged;
                }

                this.semanticZoom.IsZoomOutButtonEnabled = false;
                this.semanticZoom.Tapped += this.OnSemanticZoomTapped;
                this.semanticZoom.Tag = this;
            }

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                this.SetDataSource(this.GetDesignTimeData());
            }
        }

        protected void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var removedItems = e.RemovedItems
                                .Select(item => (object)((PickerItem<T>)item).Item)
                                .ToList();
            var addedItems = e.AddedItems
                                .Select(item => (object)((PickerItem<T>)item).Item)
                                .ToList();

            foreach (var item in removedItems)
            {
                this.SelectedItems.Remove((T)item);
            }

            foreach (var item in addedItems)
            {
                this.SelectedItems.Add((T)item);
            }

            this.SelectedItem = (T)addedItems.FirstOrDefault() 
                                    ?? this.SelectedItems.FirstOrDefault();

            this.SelectionChanged.RaiseEvent(this, new SelectionChangedEventArgs(removedItems, addedItems));
        }

        private void OnSemanticZoomTapped(object sender, TappedRoutedEventArgs e)
        {
            var group = (e.OriginalSource as FrameworkElement).DataContext as AlphaKeyGroup<PickerItem<T>>;
            if (group != null)
            {
                if (this.semanticZoom.IsZoomedInViewActive)
                {
                    this.semanticZoom.ToggleActiveView();
                    this.semanticZoom.CanChangeViews = false;
                }
                else if (group.Any())
                {
                    this.Focus(Windows.UI.Xaml.FocusState.Programmatic);
                    this.semanticZoom.CanChangeViews = true;
                    this.semanticZoom.ToggleActiveView();
                }
            }
        }

        protected void ClearSelection()
        {
            this.SelectedItems.Clear();
            if (this.semanticZoom != null)
            {
                var listView = this.semanticZoom.ZoomedInView as ListViewBase;
                if (listView != null)
                {
                    listView.SelectedItems.Clear();
                }
            }
        }

        protected void SetDataSource(IEnumerable<T> items)
        {
            if (this.semanticZoom != null)
            {
                var source = this.GetData(items);                
                this.semanticZoom.DataContext = new CollectionViewSource
                {
                    IsSourceGrouped = !(source is IList<PickerItem<T>>),
                    Source = this.GetData(items)
                };
            }
        }

        protected async Task RefreshData()
        {
            this.Items.Clear();
            this.SelectedItems.Clear();
            this.SetValue(SelectedItemProperty, DependencyProperty.UnsetValue);

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
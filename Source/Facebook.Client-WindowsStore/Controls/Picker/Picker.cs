// BUG!!!!!!
// If you are zoomed out in a semantic zoom control, and the last group is empty, if you tap it, 
// you get a catastrophic failure:
// See http://social.msdn.microsoft.com/Forums/en-US/winappswithcsharp/thread/6535656e-3293-4e0d-93b5-453864b95601
// See http://stackoverflow.com/questions/9952574/semantic-zoom-control-throwing-exception-when-groups-are-empty?rq=1
// See http://social.msdn.microsoft.com/Forums/en-US/winappswithcsharp/thread/6535656e-3293-4e0d-93b5-453864b95601
// See http://stackoverflow.com/questions/14423536/semantic-zoom-catastrophic-failure-on-empty-group
namespace Facebook.Client.Controls
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
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

        private bool isZoomedOut = false;
        private SemanticZoom semanticZoom;

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
        protected override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            if (this.semanticZoom != null)
            {
                var view = this.semanticZoom.ZoomedInView as ListViewBase;
                view.SelectionChanged -= this.OnSelectionChanged;
                this.semanticZoom.ViewChangeCompleted -= this.OnSemanticZoomViewChangeCompleted;
                this.semanticZoom.Tapped -= this.OnSemanticZoomTapped;
                (this.semanticZoom.ZoomedOutView as Control).Tapped -= this.OnSemanticZoomOutViewTapped;
            }

            this.semanticZoom = this.GetTemplateChild(Picker<T>.PartSemanticZoom) as SemanticZoom;
            if (this.semanticZoom != null)
            {
                var view = this.semanticZoom.ZoomedInView as ListViewBase;
                view.SelectionChanged += this.OnSelectionChanged;
                this.semanticZoom.IsZoomOutButtonEnabled = false;
                this.semanticZoom.ViewChangeCompleted += this.OnSemanticZoomViewChangeCompleted;
                this.semanticZoom.Tapped += this.OnSemanticZoomTapped;
                this.semanticZoom.Tag = this;
                (this.semanticZoom.ZoomedOutView as Control).Tapped += this.OnSemanticZoomOutViewTapped;
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

            this.SelectionChanged.RaiseEvent(this, new SelectionChangedEventArgs(removedItems, addedItems));
        }

        // TODO: this is a hack to prevent switching views whenever an empty group is clicked. 
        // It is not completely effective. For example, it fails if you double-click a group, or click outside a group's area.
        // Must find a better alternative.
        private void OnSemanticZoomOutViewTapped(object sender, TappedRoutedEventArgs e)
        {
            var group = (e.OriginalSource as FrameworkElement).DataContext as AlphaKeyGroup<PickerItem<T>>;
            if (group != null && !group.Any())
            {
                this.semanticZoom.IsZoomedInViewActive = false;
                e.Handled = true;
            }
        }

        private void OnSemanticZoomTapped(object sender, TappedRoutedEventArgs e)
        {
            if (!this.isZoomedOut && ((e.OriginalSource as FrameworkElement).DataContext is AlphaKeyGroup<PickerItem<T>>))
            {
                this.isZoomedOut = true;
                this.semanticZoom.ToggleActiveView();
                e.Handled = true;
            }
        }

        private void OnSemanticZoomViewChangeCompleted(object sender, SemanticZoomViewChangedEventArgs e)
        {
            if (!e.IsSourceZoomedInView)
            {
                this.isZoomedOut = false;
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

        protected virtual IEnumerable<T> GetDesignTimeData()
        {
            return null;
        }

        protected abstract IList GetData(IEnumerable<T> items);

        #endregion Implementation
    }
}
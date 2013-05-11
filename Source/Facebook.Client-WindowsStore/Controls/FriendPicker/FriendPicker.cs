// BUG!!!!!!
// If you are zoomed out in a semantic zoom control, and the last group is empty, if you tap it, 
// you get a catastrophic failure:
// See http://social.msdn.microsoft.com/Forums/en-US/winappswithcsharp/thread/6535656e-3293-4e0d-93b5-453864b95601
// See http://stackoverflow.com/questions/9952574/semantic-zoom-control-throwing-exception-when-groups-are-empty?rq=1
// See http://social.msdn.microsoft.com/Forums/en-US/winappswithcsharp/thread/6535656e-3293-4e0d-93b5-453864b95601
// See http://stackoverflow.com/questions/14423536/semantic-zoom-catastrophic-failure-on-empty-group
namespace Facebook.Client.Controls
{
    using System.Collections.Generic;
    using System.Linq;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;
    using Windows.UI.Xaml.Data;
    using Windows.UI.Xaml.Input;

    /// <summary>
    /// Shows a user interface that can be used to select Facebook friends.
    /// </summary>
    [TemplatePart(Name = PartSemanticZoom, Type = typeof(SemanticZoom))]
    public class FriendPicker : FriendPickerBase
    {
        #region Part Definitions

        private const string PartSemanticZoom = "PART_SemanticZoom";

        #endregion Part Definitions

        #region Default Property Values

        private const ListViewSelectionMode DefaultSelectionMode = ListViewSelectionMode.Multiple;

        #endregion Default Property Values

        #region Member variables

        private bool isZoomedOut = false;
        private SemanticZoom semanticZoom;

        #endregion Member variables

        #region Properties

        #region SelectionMode

        /// <summary>
        /// Gets or sets the selection behavior of the control. 
        /// </summary>
        public ListViewSelectionMode SelectionMode
        {
            get { return (ListViewSelectionMode)GetValue(SelectionModeProperty); }
            set { SetValue(SelectionModeProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectionMode dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectionModeProperty =
            DependencyProperty.Register("SelectionMode", typeof(ListViewSelectionMode), typeof(FriendPicker), new PropertyMetadata(DefaultSelectionMode));

        #endregion SelectionMode

        #endregion Properties

        /// <summary>
        /// Initializes a new instance of the FriendPicker class.
        /// </summary>
        public FriendPicker()
            :base()
        {
            this.DefaultStyleKey = typeof(FriendPicker);
        }

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
                (this.semanticZoom.ZoomedOutView as Control).Tapped -= OnSemanticZoomOutViewTapped;
            }

            this.semanticZoom = this.GetTemplateChild(FriendPicker.PartSemanticZoom) as SemanticZoom;
            if (this.semanticZoom != null)
            {
                var view = this.semanticZoom.ZoomedInView as ListViewBase;
                view.SelectionChanged += this.OnSelectionChanged;
                this.semanticZoom.IsZoomOutButtonEnabled = false;
                this.semanticZoom.ViewChangeCompleted += this.OnSemanticZoomViewChangeCompleted;
                this.semanticZoom.Tapped += this.OnSemanticZoomTapped;
                this.semanticZoom.Tag = this;
                (this.semanticZoom.ZoomedOutView as Control).Tapped += OnSemanticZoomOutViewTapped;
            }

            if (Windows.ApplicationModel.DesignMode.DesignModeEnabled)
            {
                this.SetDataSource(FriendPickerDesignSupport.DesignData);
            }
        }

        // TODO: this is a hack to prevent switching views whenever an empty group is clicked. 
        // It is not completely effective. For example, it fails if you double-click a group, or click outside a group's area.
        // Must find a better alternative.
        void OnSemanticZoomOutViewTapped(object sender, TappedRoutedEventArgs e)
        {
            var group = (e.OriginalSource as FrameworkElement).DataContext as GroupInfoList<GraphUser>;
            if (group != null && !group.Any())
            {
                this.semanticZoom.IsZoomedInViewActive = false;
                e.Handled = true;
            }
        }

        private void OnSemanticZoomTapped(object sender, TappedRoutedEventArgs e)
        {
            if (!this.isZoomedOut && ((e.OriginalSource as FrameworkElement).DataContext is GroupInfoList<GraphUser>))
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

        protected override void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var item in e.RemovedItems)
            {
                this.SelectedItems.Remove((GraphUser)item);
            }

            foreach (var item in e.AddedItems)
            {
                this.SelectedItems.Add((GraphUser)item);
            }

            base.OnSelectionChanged(sender, e);
        }

        private CollectionViewSource GroupData(IEnumerable<GraphUser> friends)
        {
            var groups = from f in friends
                         group f by f.Name.Substring(0, 1) into grp
                         orderby grp.Key
                         select grp;

            // TODO: this assumes 26 letters only, may vary in other cultures
            var alphabet = Enumerable.Range('A', 26).Select(c => ((char)c).ToString());

            return new CollectionViewSource()
            {
                IsSourceGrouped = true,
                Source = from letter in alphabet
                         join grp in groups on letter equals grp.Key into gj
                         from subfriend in gj.DefaultIfEmpty()
                         select new GroupInfoList<GraphUser>(letter, subfriend)
            };
        }

        protected override void SetDataSource(IEnumerable<GraphUser> friends)
        {
            if (this.semanticZoom != null)
            {
                this.semanticZoom.DataContext = this.GroupData(friends);
            }
        }

        #endregion Implementation
    }
}

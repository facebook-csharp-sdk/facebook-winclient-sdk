namespace Facebook.Client.Controls
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Globalization;
    using System.Linq;
    using System.Threading;
    using System.Threading.Tasks;
    using Windows.Devices.Geolocation;
    using Windows.Foundation;
    using Windows.UI.Core;
    using Windows.UI.Xaml;
    using Windows.UI.Xaml.Controls;

    /// <summary>
    /// Shows a user interface that can be used to select Facebook places.
    /// </summary>
    [TemplatePart(Name = PartListBox, Type = typeof(ListBox))]
    public class PlacePicker : Control
    {
        #region Part Definitions

        private const string PartListBox = "PART_ListBox";

        #endregion Part Definitions

        #region Default Property Values

        private const string DefaultAccessToken = "";
        private const string DefaultDisplayFields = "id,name,location,category,picture,were_here_count";
        private const bool DefaultDisplayProfilePictures = true;
        private static readonly Size DefaultPictureSize = new Size(50, 50);
        private const string DefaultSearchText = "";
        private const int DefaultRadiusInMeters = 1000;
        private const int DefaultResultsLimit = 100;
        private const bool DefaultTrackLocation = true;
        private static readonly LocationCoordinate DefaultLocationCoordinate = new LocationCoordinate(0.0, 0.0);

        #endregion Default Property Values

        #region Member variables

        private Geolocator geoLocator;
        private ListBox listBox;

        #endregion Member variables

        /// <summary>
        /// Initializes a new instance of the PlacePicker class.
        /// </summary>
        public PlacePicker()
        {
            this.DefaultStyleKey = typeof(PlacePicker);
            this.SetValue(ItemsProperty, new ObservableCollection<GraphLocation>());
            this.SetValue(SelectedItemsProperty, new ObservableCollection<GraphLocation>());
        }

        #region Events

        /// <summary>
        /// Occurs whenever a new place is about to be added to the list.
        /// </summary>
        public event EventHandler<DataItemRetrievedEventArgs<GraphLocation>> PlaceRetrieved;

        /// <summary>
        /// Occurs when the list of places has finished loading.
        /// </summary>
        public event EventHandler<DataReadyEventArgs<GraphLocation>> LoadCompleted;

        /// <summary>
        /// Occurs whenever an error occurs while loading data.
        /// </summary>
        public event EventHandler<LoadFailedEventArgs> LoadFailed;

        /// <summary>
        /// Occurs when the current selection changes.
        /// </summary>
        public event EventHandler<SelectionChangedEventArgs> SelectionChanged;

        #endregion Events

        #region Properties

        #region AccessToken

        /// <summary>
        /// Gets or sets the access token returned by the Facebook Login service.
        /// </summary>
        public string AccessToken
        {
            get { return (string)this.GetValue(AccessTokenProperty); }
            set { this.SetValue(AccessTokenProperty, value); }
        }

        /// <summary>
        /// Identifies the AccessToken dependency property.
        /// </summary>
        public static readonly DependencyProperty AccessTokenProperty =
            DependencyProperty.Register("AccessToken", typeof(string), typeof(PlacePicker), new PropertyMetadata(DefaultAccessToken, OnAccessTokenPropertyChanged));

        private async static void OnAccessTokenPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var placePicker = (PlacePicker)d;
            var currentLocation = await placePicker.GetCurrentLocation();
            await placePicker.RefreshData(currentLocation.Latitude, currentLocation.Longitude);
        }

        #endregion AccessToken

        #region Items

        /// <summary>
        /// Gets the list of friends retrieved by the PlacePicker control.
        /// </summary>
        public ObservableCollection<GraphLocation> Items
        {
            get { return (ObservableCollection<GraphLocation>)this.GetValue(ItemsProperty); }
            private set { this.SetValue(ItemsProperty, value); }
        }

        /// <summary>
        /// Identifies the Items dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemsProperty =
            DependencyProperty.Register("Items", typeof(ObservableCollection<GraphLocation>), typeof(PlacePicker), null);

        #endregion Items

        #region SelectedItems

        /// <summary>
        /// Gets the list of currently selected items for the PlacePicker control.
        /// </summary>
        public ObservableCollection<GraphLocation> SelectedItems
        {
            get { return (ObservableCollection<GraphLocation>)this.GetValue(SelectedItemsProperty); }
            private set { this.SetValue(SelectedItemsProperty, value); }
        }

        /// <summary>
        /// Identifies the SelectedItems dependency property.
        /// </summary>
        public static readonly DependencyProperty SelectedItemsProperty =
            DependencyProperty.Register("SelectedItems", typeof(ObservableCollection<GraphLocation>), typeof(PlacePicker), null);

        #endregion SelectedItems

        #region DisplayFields

        /// <summary>
        /// Gets or sets additional fields to fetch when requesting place data.
        /// </summary>
        /// <remarks>
        /// By default, the following data is retrieved for each place: TODO: to be completed.
        /// </remarks>
        public string DisplayFields
        {
            get { return (string)GetValue(DisplayFieldsProperty); }
            set { SetValue(DisplayFieldsProperty, value); }
        }

        /// <summary>
        /// Identifies the DisplayFields dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayFieldsProperty =
            DependencyProperty.Register("DisplayFields", typeof(string), typeof(PlacePicker), new PropertyMetadata(DefaultDisplayFields));

        #endregion DisplayFields

        #region DisplayProfilePictures

        /// <summary>
        /// Specifies whether profile pictures are displayed.
        /// </summary>
        public bool DisplayProfilePictures
        {
            get { return (bool)GetValue(DisplayProfilePicturesProperty); }
            set { SetValue(DisplayProfilePicturesProperty, value); }
        }

        /// <summary>
        /// Identifies the DisplayProfilePictures dependency property.
        /// </summary>
        public static readonly DependencyProperty DisplayProfilePicturesProperty =
            DependencyProperty.Register("DisplayProfilePictures", typeof(bool), typeof(PlacePicker), new PropertyMetadata(DefaultDisplayProfilePictures));

        #endregion DisplayProfilePictures

        #region PictureSize
        /// <summary>
        /// Gets or sets the size of the profile pictures displayed.
        /// </summary>
        public Size PictureSize
        {
            get { return (Size)GetValue(PictureSizeProperty); }
            set { SetValue(PictureSizeProperty, value); }
        }

        /// <summary>
        /// Identifies the PictureSize dependency property.
        /// </summary>
        public static readonly DependencyProperty PictureSizeProperty =
            DependencyProperty.Register("PictureSize", typeof(Size), typeof(PlacePicker), new PropertyMetadata(DefaultPictureSize));

        #endregion PictureSize

        #region SearchText

        public string SearchText
        {
            get { return (string)GetValue(SearchTextProperty); }
            set { SetValue(SearchTextProperty, value); }
        }

        public static readonly DependencyProperty SearchTextProperty =
            DependencyProperty.Register("SearchText", typeof(string), typeof(PlacePicker), new PropertyMetadata(DefaultSearchText));

        #endregion SearchText

        #region RadiusInMeters DependencyProperty

        public int RadiusInMeters
        {
            get { return (int)GetValue(RadiusInMetersProperty); }
            set { SetValue(RadiusInMetersProperty, value); }
        }

        public static readonly DependencyProperty RadiusInMetersProperty =
            DependencyProperty.Register("RadiusInMeters", typeof(int), typeof(PlacePicker), new PropertyMetadata(DefaultRadiusInMeters));

        #endregion RadiusInMeters DependencyProperty

        #region LocationCoordinate DependencyProperty

        public LocationCoordinate LocationCoordinate
        {
            get { return (LocationCoordinate)GetValue(LocationCoordinateProperty); }
            set { SetValue(LocationCoordinateProperty, value); }
        }

        public static readonly DependencyProperty LocationCoordinateProperty =
            DependencyProperty.Register("LocationCoordinate", typeof(LocationCoordinate), typeof(PlacePicker), new PropertyMetadata(DefaultLocationCoordinate, OnLocationCoordinateChanged));

        private async static void OnLocationCoordinateChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var placePicker = (PlacePicker)d;
            var coordinate = (LocationCoordinate)e.NewValue;
            if (coordinate != null)
            {
                await placePicker.RefreshData(coordinate.Latitude, coordinate.Longitude);
            }
        }

        #endregion LocationCoordinate DependencyProperty

        #region TrackLocation

        public bool TrackLocation
        {
            get { return (bool)GetValue(TrackLocationProperty); }
            set { SetValue(TrackLocationProperty, value); }
        }

        public static readonly DependencyProperty TrackLocationProperty =
            DependencyProperty.Register("TrackLocation", typeof(bool), typeof(PlacePicker), new PropertyMetadata(DefaultTrackLocation, OnTrackCurrentLocationPropertyChanged));

        private static void OnTrackCurrentLocationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var placePicker = (PlacePicker)d;
            if ((bool)e.NewValue)
            {
                placePicker.geoLocator = new Geolocator();
                placePicker.geoLocator.PositionChanged += placePicker.OnPositionChanged;
            }
            else if (placePicker.geoLocator != null)
            {
                placePicker.geoLocator.PositionChanged -= placePicker.OnPositionChanged;
                placePicker.geoLocator = null;
            }
        }

        #endregion TrackLocation

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

            this.listBox = this.GetTemplateChild(PartListBox) as ListBox;
            if (this.listBox != null)
            {
                this.listBox.SelectionChanged += OnSelectionChanged;
            }
        }

        private void OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            foreach (var item in e.RemovedItems)
            {
                this.SelectedItems.Remove((GraphLocation)item);
            }

            foreach (var item in e.AddedItems)
            {
                this.SelectedItems.Add((GraphLocation)item);
            }
            
            this.SelectionChanged.RaiseEvent(this, e);
        }

        private void OnPositionChanged(Geolocator sender, PositionChangedEventArgs args)
        {
            this.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, async () =>
            {
                await this.RefreshData(args.Position.Coordinate.Latitude, args.Position.Coordinate.Longitude);
            });
        }

        private async Task RefreshData(double latitude, double longitude)
        {
            this.Items.Clear();
            this.SelectedItems.Clear();

            if (string.IsNullOrEmpty(this.AccessToken))
            {
                return;
            }

            try
            {
                FacebookClient facebookClient = new FacebookClient(this.AccessToken);

                string graphUrl = string.Format(
                                        CultureInfo.InvariantCulture,
                                        "/search?fields={0}",
                                        this.DisplayFields);
                dynamic placesTaskResult = await facebookClient.GetTaskAsync(
                                                graphUrl,
                                                new
                                                {
                                                    type = "place",
                                                    q = this.SearchText,
                                                    center = latitude.ToString() + "," + longitude.ToString(),
                                                    distance = this.RadiusInMeters
                                                });

                var data = (IEnumerable<dynamic>)placesTaskResult.data;
                foreach (var item in data)
                {
                    var place = new GraphLocation(item.location);
                    if (this.PlaceRetrieved.RaiseEvent(this, new DataItemRetrievedEventArgs<GraphLocation>(place), e => e.Exclude))
                    {
                        this.Items.Add(place);
                    }
                }

                this.SetDataSource(this.Items);
                this.LoadCompleted.RaiseEvent(this, new DataReadyEventArgs<GraphLocation>((this.Items.ToList())));
            }
            // TODO: review the types of exception that can be caught here
            catch (Exception ex)
            {
                this.LoadFailed.RaiseEvent(this, new LoadFailedEventArgs("Error loading place data.", ex.Message));
            }
        }

        private async Task<LocationCoordinate> GetCurrentLocation()
        {
            CancellationTokenSource cts = new CancellationTokenSource();
            CancellationToken token = cts.Token;
            try
            {
                // We will wait 100 milliseconds and accept locations up to 48 hours old before we give up
                // TODO: setting a timeout of 100 milliseconds fails with a timeout exception. Find out the the reason. 
                var position = await this.geoLocator.GetGeopositionAsync(new TimeSpan(0, 1, 0), new TimeSpan(0, 0, 0, 1)).AsTask(token);
                return new LocationCoordinate(position.Coordinate.Latitude, position.Coordinate.Longitude);
            }
            catch (System.UnauthorizedAccessException)
            {
                //MessageTextbox.Text = "Location disabled.";

                //LatitudeTextbox.Text = "No data";
                //LongitudeTextbox.Text = "No data";
                //AccuracyTextbox.Text = "No data";
            }
            catch (TaskCanceledException)
            {
                //MessageTextbox.Text = "Operation canceled.";
            }
            catch (Exception)
            {
                // this API can timeout, so no point breaking the code flow. Use
                // default latitutde and longitude and continue on.
            }
            //finally
            //{
            //    _cts = null;
            //}

            // default location
            return new LocationCoordinate(51.494338, -0.176759);
        }

        protected void SetDataSource(IEnumerable<GraphLocation> place)
        {
            //if (this.semanticZoom != null)
            //{
            //    this.semanticZoom.DataContext = this.GroupData(friends);
            //}
        }

        #endregion Implementation
    }
}

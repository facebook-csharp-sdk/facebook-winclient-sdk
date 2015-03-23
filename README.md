# Facebook C# SDK for Windows & Windows Phone
The Facebook C# SDK for Windows & Windows Phone helps .NET developers build Windows Phone and Windows Store applications that integrate with Facebook.

[Like us on Facebook at our official page](http://facebook.com/csharpsdk) or [Follow us on twitter @chsarpsdk](http://twitter.com/csharpsdk).

## NuGet

    Install-Package Facebook.Client

*Binaries for Facebook C# SDK for Windows & Windows Phone are only distributed via nuget. For those using older versions of Visual Studio that does not support NuGet Package Manager, please download the [command line version of NuGet.exe](http://nuget.codeplex.com/releases/view/58939) and run the following
command.*

    nuget install Facebook.Client
    
If you would like to get an older version of the the binaries please use the following command.

    nuget install Facebook.Client -v 0.1.1
    
## Documentation
You can find the documentation for this project at [http://facebooksdk.net](http://facebooksdk.net).

## Running the Samples
The SDK comes with quite a few samples. Specifically, the samples cover Login/Dialogs/Native Controls and Open Graph.

The Samples are in the Samples Folder. However, the way the developers test the samples are by Opening the Solution Files in the Source Directory. Here is How the various Solutions are laid out:

1. Windows Phone 8.1 Universal: Open Source\Facebook.Client-Universal\Facebook.Client-Universal.WindowsPhone\Facebook.Client-Universal.WindowsPhone.sln. This contains all the samples for Windows Phone 8.1 Universal apps hooked up in the same solution as the SDK.

2. Windows Phone 8.0 or Windows Phone 8.1 (Silverlight only): Open Source\Facebook.Client-WP8.sln. This contains all the samples for Windows Phone 8.0 and Windows Phone 8.1 (Silverlight only) hooked up in the same solution as the SDK.

3. Windows 8.1: Open the solution file Source\Facebook.Client-Universal\Facebook.Client-Universal.Windows\Facebook.Client-Universal.Windows.sln. This contains all the samples for Windows 8.1 hooked up in the same solution as the SDK.

4. Windows 8.0: Open the solution file Source\Facebook.Client-WindowsStore.sln. This contains all the samples for Windows 8.0 hooked up in the same solution as the SDK.

## Help and Support
Use [facebook.stackoverflow.com](http://facebook.stackoverflow.com) for help and support. We answer questions there regularly. Use the tags '[facebook-c#-sdk](http://stackoverflow.com/questions/tagged/facebook-c%23-sdk)' and '[facebook](http://stackoverflow.com/questions/tagged/facebook)' plus any other tags that are relevant. If you have a feature request or bug create an issue.

[Facebook Platform Status](https://developers.facebook.com/live_status)

[Facebook Change Log](https://developers.facebook.com/docs/changelog/)

## Features
* NuGet Packages Available ([Facebook.Client](http://nuget.org/packages/Facebook.Client))
* Authentication Helpers

## Supported Platforms
* Windows Store
* Windows Phone 8+
 
## Contribute

Please refer to our official docs on [Contributing to Facebook C# SDK](http://csharpsdk.org/docs/contribute) for more details.

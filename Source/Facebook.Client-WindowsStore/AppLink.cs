using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.WebUI;

namespace Facebook.Client
{

    public class Target
    {     
        // url that we have to launch for the platform
        public Uri Uri { get; set; }

        // app name that we have to show for the back button in the target app
        public string Name { get; set; }
        
        // package family name to redirect to the store
        public string PackageID { get; set; }
    }

    public class AppLink
    {
        // source url
        public Uri SourceUri { get; set; }
        
        // list of targets
        public List<Target> Targets { get; set; } 

        // fallback web url
        public Uri FallbackUri { get; set; }
    }

    public enum NavigationResult
    {
        Failed,
        Web,
        Application
    }



    


    public class Bolts
    {
        // provides version
        public String Version { get; set; }
    }


}

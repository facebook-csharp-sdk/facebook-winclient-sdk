using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Client
{
    public class FacebookDialogResult
    {
        public object ResponseData { get; private set; }

        public FacebookWebDialogResponseStatus ResponseStatus { get; private set; }

        public uint ResponseErrorDetail { get; private set; }

        public FacebookDialogResult(object data, FacebookWebDialogResponseStatus status, uint error)
        {
            ResponseData = data;
            ResponseStatus = status;
            ResponseErrorDetail = error;
        }
    }
}

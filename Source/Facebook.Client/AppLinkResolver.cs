using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Client
{
    public interface AppLinkResolver
    {
        Task<AppLink> GetAppLinkAsync(string accessToken, string sourceUrl);
    }
}

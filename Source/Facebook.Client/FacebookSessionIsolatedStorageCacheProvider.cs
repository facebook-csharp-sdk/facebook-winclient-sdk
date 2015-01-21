//-----------------------------------------------------------------------
// <copyright file="FacebookSessionIsolatedStorageCacheProvider.cs" company="The Outercurve Foundation">
//    Copyright (c) 2011, The Outercurve Foundation. 
//
//    Licensed under the Apache License, Version 2.0 (the "License");
//    you may not use this file except in compliance with the License.
//    You may obtain a copy of the License at
//      http://www.apache.org/licenses/LICENSE-2.0
//
//    Unless required by applicable law or agreed to in writing, software
//    distributed under the License is distributed on an "AS IS" BASIS,
//    WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
//    See the License for the specific language governing permissions and
//    limitations under the License.
// </copyright>
// <author>Nathan Totten (ntotten.com) and Prabir Shrestha (prabir.me)</author>
// <website>https://github.com/facebook-csharp-sdk/facbook-winclient-sdk</website>
//-----------------------------------------------------------------------

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.IO.IsolatedStorage;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Facebook.Client
{
    public class AccessTokenDataIsolatedStorageCacheProvider : AccessTokenDataCacheProvider
    {
        private const string fileName = "FACEBOOK_SESSION";
        private static readonly object _fileLock = new object();

        // all access to the file should be via a lock to avoid race conditions and 
        // data corruption
        public override AccessTokenData GetSessionData()
        {
            lock (_fileLock)
            {
                if (DesignerProperties.IsInDesignTool)
                {
                    // return dummy data for the design view
                    return new AccessTokenData() { AccessToken = "", FacebookId = "" };
                }

                var store = IsolatedStorageFile.GetUserStoreForApplication();
                if (!store.FileExists(fileName))
                {
                    return null;
                }

                AccessTokenData data;
                var serializer = new XmlSerializer(typeof (AccessTokenData));
                using (var stream = store.OpenFile(fileName, System.IO.FileMode.Open))
                {
                    data = serializer.Deserialize(stream) as AccessTokenData;
                }

                return data;
            }
        }

        public override void SaveSessionData(AccessTokenData data)
        {
            lock (_fileLock)
            {
                var serializer = new XmlSerializer(typeof(AccessTokenData));
                var store = IsolatedStorageFile.GetUserStoreForApplication();
                using (var stream = store.OpenFile(fileName, FileMode.Create))
                {
                    serializer.Serialize(stream, data);
                }
            }
        }

        public override void DeleteSessionData()
        {
            lock (_fileLock)
            {
                var store = IsolatedStorageFile.GetUserStoreForApplication();
                if (store.FileExists(fileName))
                {
                    store.DeleteFile(fileName);
                }
            }
        }
    }
}

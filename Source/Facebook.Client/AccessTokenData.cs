//-----------------------------------------------------------------------
// <copyright file="AccessTokenData.cs" company="The Outercurve Foundation">
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
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Client
{
    public class AccessTokenData
    {

        public AccessTokenData()
        {
            this.CurrentPermissions = new List<string>();
        }

        // NOTE: If you add properties to this object, you must update the 
        // cache provider implimentations to ensure all values get persisted.
        public String AppId;
        public string AccessToken { get; set; }
        public DateTime Expires { get; set; }
        public DateTime Issued { get; set; }
        public string FacebookId { get; set; }
        public List<string> CurrentPermissions { get; set; }
        public string State { get; set; }
    }
}

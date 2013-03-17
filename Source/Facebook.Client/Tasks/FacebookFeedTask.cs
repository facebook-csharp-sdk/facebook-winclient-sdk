using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Facebook.Client.Tasks
{
    public class FacebookFeedTask : FacebookTask
    {

        /// <summary>	
        /// The ID or username of the user posting the message. If this is unspecified, it defaults to the current user. If specified, it must be the ID of the user or of a page that the user administers.
        /// </summary>
        [DataMember(Name = "from")]
        public string From { get; set; }

        /// <summary>	
        /// The ID or username of the profile that this story will be published to. If this is unspecified, it defaults to the value of from.
        /// </summary>
        [DataMember(Name = "to")]
        public string To { get; set; }

        /// <summary>
        /// The link attached to this post
        /// </summary>
        [DataMember(Name = "link")]
        public Uri Link { get; set; }

        /// <summary>	
        /// The URL of a picture attached to this post. The picture must be at least 50px by 50px (though minimum 200px by 200px is preferred) and have a maximum aspect ratio of 3:1
        /// </summary>
        [DataMember(Name = "picture")]
        public Uri Picture { get; set; }

        /// <summary>	
        /// The URL of a media file (either SWF or MP3) attached to this post. If both source and picture are specified, only source is used.
        /// </summary>
        [DataMember(Name = "source")]
        public Uri Source { get; set; }

        /// <summary>	
        /// The name of the link attachment.
        /// </summary>
        [DataMember(Name = "name")]
        public string Name { get; set; }

        /// <summary>	
        /// The caption of the link (appears beneath the link name). If not specified, this field is automatically populated with the URL of the link.
        /// </summary>
        [DataMember(Name = "caption")]
        public string Caption { get; set; }

        /// <summary>	
        /// The description of the link (appears beneath the link caption). If not specified, this field is automatically populated by information scraped from the link, typically the title of the page.
        /// </summary>
        [DataMember(Name = "description")]
        public string Description { get; set; }

        /// <summary>	
        /// A JSON object of key/value pairs which will appear in the stream attachment beneath the description, with each property on its own line. Keys must be strings, and values can be either strings or JSON objects with the keys text and href.
        /// </summary>
        [DataMember(Name = "properties")]
        public IDictionary<string, object> Properties { get; set; }

        /// <summary>	
        ///A JSON array containing a single object describing the action link which will appear next to the "Comment" and "Like" link under posts. The contained object must have the keys name and link.
        /// </summary>
        [DataMember(Name = "actions")]
        public object[] Actions { get; set; }

        /// <summary>	
        /// A text reference for the category of feed post. This category is used in Facebook Insights to help you measure the performance of different types of post
        /// </summary>
        [DataMember(Name = "ref")]
        public string Ref { get; set; }

        public async Task<FacebookFeedTaskResult> Show()
        {
            if (string.IsNullOrEmpty(this.AppId))
                throw new ArgumentNullException("AppId");

            var parameters = BuildParameters();
            var result = await FacebookWebDialog.ShowDialogAsync("feed", parameters) as IDictionary<string, object>;
            var taskResult = new FacebookFeedTaskResult();
            taskResult.PostId = (string)result["post_id"];
            return taskResult;
        }

    }
}

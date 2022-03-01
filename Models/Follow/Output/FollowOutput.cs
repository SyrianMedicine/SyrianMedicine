using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Models.Follow.Output
{
    /// <summary>
    /// users following
    /// </summary>
    public class FollowOutput
    {

        /// <summary>
        /// this user follow (FollowedUserName)
        /// </summary>
        /// <value></value>
        public string UserName { get; set; }

        /// <summary> 
        /// this user followed by (UserName)
        /// </summary>
        public string FollowedUserName { get; set; }
    }
}
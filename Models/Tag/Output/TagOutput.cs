using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Models.Tag.Output
{
    public class TagOutput
    {
        /// <summary>
        /// tag id
        /// </summary> 
        [Required]
        public int Id { get; set; }
        /// <summary>
        /// tag name  
        /// </summary>
        /// <value>example (قلبية , عصبية ,,,,)</value>
        public String Name { get; set; }
    }
}
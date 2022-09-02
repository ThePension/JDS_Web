using JDSCommon.Database.DataContract;

#nullable disable

namespace JDSWeb.Models
{
    public class EventViewModel
    {
        /* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * *\
		|*                             PROPERTIES                            *|
		\* * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * * */

        public Event[] Events { get; set; }

		public Event Event => Events[0];

        public string SearchPattern { get; set; }
    }
}

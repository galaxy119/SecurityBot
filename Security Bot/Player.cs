using System.Collections.Generic;

namespace Security_Bot
{
	public class Player
	{
		public string steamid { get; set; }
		public int communityvisibilitystate { get; set; }
		public int profilestate { get; set; }
		public string personaname { get; set; }
		public int lastlogoff { get; set; }
		public string profileurl { get; set; }
		public string avatar { get; set; }
		public string avatarmedium { get; set; }
		public string avatarfull { get; set; }
		public int personastate { get; set; }
		public string realname { get; set; }
		public string primaryclanid { get; set; }
		public int timecreated { get; set; }
		public int personastateflags { get; set; }
	}

	public class Response
	{
		public List<Player> players { get; set; }
	}

	public class RootObject
	{
		public Response response { get; set; }
	}
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class BaseMessage
{
	public enum MsgType
	{
		RoomJoin = 0,
		RoomLeave,
		MatchFound,
		MatchMaking,
		playerdetails,
		MatchNotFound,
		GameDetails

	}


	public MsgType Type { get; set; }
}


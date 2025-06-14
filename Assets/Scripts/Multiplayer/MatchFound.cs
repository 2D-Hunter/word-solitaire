using System.Collections.Generic;

public class Player
{
    public string sessionId { get; set; }
    public object joinedAt { get; set; }
}

public class MatchFound
{
    public string id { get; set; }
    public List<Player> players { get; set; }
    public bool full { get; set; }
}
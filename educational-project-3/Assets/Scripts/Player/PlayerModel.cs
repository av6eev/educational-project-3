namespace Player
{
    public class PlayerModel
    {
        public string Id { get; }
        public string TeamName { get; }
        
        public PlayerModel(string id, string teamName)
        {
            Id = id;
            TeamName = teamName;
        }
    }
}
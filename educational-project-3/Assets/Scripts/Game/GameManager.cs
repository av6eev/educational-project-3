using Bot;
using Floor;
using Player;
using Start;
using Utilities;

namespace Game
{
    public class GameManager
    {
        public StartView StartView;
        
        public GameView GameView;
        public GameDescriptions GameDescriptions;
        
        public GameModel GameModel;
        public BotModel BotModel;
        public PlayerModel PlayerModel;
        public FloorModel FloorModel;

        public FixedSystemEngine FixedSystemEngine;
        public SystemEngine SystemEngine;
    }
}
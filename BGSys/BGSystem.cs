using hoshi_lib.Thread;
using hoshi_lib.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGS {
    public class BGSystem {
        static public HTimer MainTimer { get { return mainTimer; } }
        static HTimer mainTimer = new HTimer(100);
        static public BGSystem Instance {
            get {
                if (instance == null) instance = new BGSystem();
                return instance; 
            }
        }
        static BGSystem instance;

        public int PlayerAmount {
            get { return playerAmount; }
        }
        public IList<Player> Players {
            get {
                return players;
            }
        }
        public ProcessSet Processes { get; protected set; }
        public Player CurrentPlayer {
            get { return playersrator.Current; }
        }
        public AProcess CurrentProcess {
            get { return Processes.Current; }
        }
        #region private
        int playerAmount = 0;
        List<Player> players = new List<Player>(8);
        IEnumerator<Player> playersrator;
        #endregion

        protected BGSystem() {
            Processes = new ProcessSet();
        }
        
        public void Start() {
            MainTimer.Start();
            playersrator = Players.GetEnumerator();
            NextPlayer();
            Processes.Next();
            if (CurrentProcess != null) CurrentProcess.Do();
        }
        public void End() {
            MainTimer.Stop();
        }
        public void NextPlayer() {
            if (!playersrator.MoveNext()) {
                playersrator.Reset();
                playersrator.MoveNext();
            }
        }
    
        internal bool NextProcess() {
            return Processes.Next();
        }

    }

}

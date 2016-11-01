using hoshi_lib.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGS {
    public class BGMessageView : HControl {
        public BGSystem BGSys { get; protected set; }

        public BGMessageView(BGSystem bgs) {
            BGSys = bgs;
            Background = null;
            Size = new hoshi_lib.Size(300, 50);
            BGSystem.MainTimer.Tick += (s, e) => this.Update();
        }

        public void Update() {
            if (BGSys == null) return;
            Text = BGSys.CurrentProcess.Message;
        }
    }
}

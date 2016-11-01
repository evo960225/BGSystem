using hoshi_lib.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;

namespace BGS {

    public class StringItem : BGO {
        public string Text { get; set; }
    }

    public class BGItemView : BGView<StringItem> {

        public override StringItem BGData {get;set;}
        IBGO bgo;
        public BGItemView(IBGO bgo) {
            this.bgo = bgo;
        }

        public override void SystemUpdate() {
            if (bgo.IsSelectable && !this.IsSelectable) {
                this.Selectable();
            } 
            if(!bgo.IsSelectable && this.IsSelectable){
                this.Unselectable();
            }
        }

       
    }

    public class BGIfItemList : StackControl {

        public IEnumerable<BGView<StringItem>> Buttons {
            get { return buttons; }
        }
        public List<BGView<StringItem>> buttons = new List<BGView<StringItem>>(6);
         
        public BGIfItemList() {

            Background = null;
            this.Orientation = System.Windows.Controls.Orientation.Horizontal;
            HorizontalAlignment = System.Windows.HorizontalAlignment.Left;
            VerticalAlignment = System.Windows.VerticalAlignment.Top;
            Size = new hoshi_lib.Size(500, 50);

            BGSystem.MainTimer.Tick += (s, e) => this.Update();
        }

        ProcessIf preP;
        public void Update() {
            var sys = BGSystem.Instance;
            if (sys == null) return;
            if (!(sys.CurrentProcess is ProcessIf)) preP = null;
            else {
                if (preP == sys.CurrentProcess) return;
                var p = sys.CurrentProcess as ProcessIf;
                preP = p;
                this.Children.Clear();
                buttons.Clear();

                var able = p.Selectable();
                foreach (var it in able) {
                    var button = createButton(it);
                    buttons.Add(button);
                    this.Children.Add(button);
                }
            }
        }
        BGView<StringItem> createButton(hoshi_lib.Pair<string,IBGO> data) {
            var sys = BGSystem.Instance;
            BGItemView button = new BGItemView(data.B);
            //button.BGData = data.A;
            button.MouseDown += (s, e) => data.B.Select();
            
            initBottonView(button);
            //Set Text and Size
            button.Text = button.BGData.Text;
            button.Margin = new System.Windows.Thickness(5, 0, 5, 0);
            var width = 40 + button.BGData.Text.Count() * button.FontSize / 2;
            button.Size = new hoshi_lib.Size(width, 40);

            return button;
        }
        void initBottonView(BGItemView button) {
            button.BorderWidth = 2;
            button.Background = new SolidColorBrush(Color.FromRgb(252, 128, 128));
            button.FontColor = new SolidColorBrush(Color.FromRgb(255, 255, 255));
            button.FontSize = 14;
        }
       
    }
}

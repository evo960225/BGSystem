using hoshi_lib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BGS {

    public abstract class AProcess {

        static public IEnumerable<IBGO> GlobalSelected {
            get { return globalSelected; }
        }
        static protected List<IBGO> globalSelected;

        public AProcessSet Parent { get; internal set; }
        public AProcess Current {
            get {
                if (Root == this) return currentP;
                else return Root.Current;
            }
            protected set {
                if (Root == this) currentP = value;
                else Root.Current = value;
            }
        }
        public string Name { get; set; }
        public string Message { get; set; }
        public AProcess Root {
            get {
                if (Parent == null) return this;
                else return Parent.Root;
            }
        }
        AProcess currentP;

        abstract public void Do();
        abstract public ConditionCheck Listen(IBGO send);
        abstract public bool Next();
        abstract public void Reset();
    }

    public class Process : AProcess {

        public IEnumerable<IBGO> Selected {
            get { return selected; }
        }
        public Func<IEnumerable<IBGO>> Selectable { get; set; }
        public Func<IBGO, ConditionCheck> LoopCondition { get; set; }
        public Action<IEnumerable<IBGO>> Func { get; set; }
        protected List<IBGO> selected = new List<IBGO>(5);
        private IEnumerable<IBGO> beAbled;

        public override void Do() {
            if (Selectable != null) {
                beAbled = Selectable();
                if (beAbled == null || beAbled.Count() == 0) {
                    if (Func != null) Func(selected);
                    Root.Next();
                    if (Current != null) Current.Do();
                    return;
                }
                beAbled = beAbled.ToList();
                foreach (var it in beAbled) {
                    it.Selectable();
                }
            } else {
                if (Func != null) Func(selected);
                if (!Root.Next()) return;
                if (Current != null) Current.Do();
            }
        }

        public override ConditionCheck Listen(IBGO send) {
            if (LoopCondition != null) {
                var condition = LoopCondition(send);
                if (condition == ConditionCheck.Continue) {
                    selected.Add(send);
                    return ConditionCheck.Continue;
                } else if (condition == ConditionCheck.NotOK) {
                    return ConditionCheck.NotOK;
                }
            }
            selected.Add(send);
            if (Func != null) {
                Func(selected);
            }

            return ConditionCheck.OK;
        }
        public override bool Next() {
            Reset();
            return false;
        }
        public override void Reset() {
            if (beAbled == null) return;
            if (selected != null) {
                foreach (var it in beAbled) {
                    it.Unselectable();
                }
            }
            selected.Clear();
        }

        public bool SelectedRemove(IBGO send) {
            send.Unselect();
            return selected.Remove(send);
        }

    }

    public abstract class AProcessSet : AProcess {

        public IList<AProcess> Processes { get { return processes; } }
        public int ProcessesIndex { get; protected set; }
        #region protected
        protected List<AProcess> processes = new List<AProcess>(10);
        #endregion

        public void AddProcess(AProcess process) {
            process.Parent = this;
            processes.Add(process);
        }
        public void AddProcess(params AProcess[] processes) {
            foreach (var it in processes) {
                it.Parent = this;
                this.processes.Add(it);
            }
        }

    }
    
    public class ProcessSet : AProcessSet {
        public bool IsEnd {
            get { return ProcessesIndex >= Processes.Count() - 1; }
        }
        public Func<bool> WhileLoop { get; set; }

        public ProcessSet() {
            Reset();
        }
        public ProcessSet(params AProcess[] processes) {
            this.processes = new List<AProcess>(processes);
            Reset();
        }

        public override void Do() {
            //Reset();
            Next();
            if (processes != null && ProcessesIndex >= 0 && ProcessesIndex < processes.Count)
                processes[ProcessesIndex].Do();
        }
        public override ConditionCheck Listen(IBGO send) {
            Reset();
            return ConditionCheck.OK;
        }
        public override bool Next() {
            while (ProcessesIndex < 0 || !processes[ProcessesIndex].Next()) {
                if (IsEnd) {
                    if (processes.Count == 0) return false;
                    if (WhileLoop != null && WhileLoop()) {
                        Reset();
                    } else {
                        return false;
                    }
                } else {
                    Current = processes[++ProcessesIndex];
                    return true;
                }
            }
            return true;
        }
        public override void Reset() {
            ProcessesIndex = -1;
        }

    }

    public class ProcessIf : AProcessSet {

        public Func<IEnumerable<Pair<string,IBGO>>> Selectable { get; set; }
        bool selected = false;
        protected List<IBGO> allableList;

        public override void Do() {
            allableList = Selectable().Select((x)=>x.B).ToList();
            if (allableList.Count() == 0) {
                Root.Next();
                if (Current != null) Current.Do();
                return;
            }

            foreach (var it in allableList) {
                it.Selectable();
            }
        }
        public override ConditionCheck Listen(IBGO send) {
            ProcessesIndex = allableList.FindIndex((x) => x == send);
            if (ProcessesIndex < 0) return ConditionCheck.NotOK;
            return ConditionCheck.OK;
        }
        public override bool Next() {
            if (selected) {
                return processes[ProcessesIndex].Next();
            }
            if (ProcessesIndex >= 0 && ProcessesIndex < processes.Count) {
                Current = processes[ProcessesIndex];
                selected = true;
                Reset();
                return true;
            }
            return false;
        }
        public override void Reset() {
            if (allableList != null) {
                foreach (var it in allableList) {
                    it.Unselectable();
                }
            }
            ProcessesIndex = -1;
            selected = false;
        }

    }

    public class ProcessList : AProcessSet {

        public bool IsEnd {
            get { return canSelect.Count == 0; }
        }
        public Func<bool> WhileLoop { get; set; }
        public IEnumerable<string> Item {
            get { return items; }
        }
        List<string> items = new List<string>(5);

        protected List<IBGO> canSelect;
        protected List<IBGO> allableList;
        public Func<IEnumerable<IBGO>> Selectable;

        public ProcessList() {
            Reset();
        }
        public override void Do() {
            if (canSelect == null) {
                if (Selectable != null) {
                    canSelect = Selectable().ToList();
                    allableList = canSelect.ToList();
                }
            }
            if (canSelect != null) {
                foreach (var it in canSelect) {
                    it.Selectable();
                }
            }

        }
        public override ConditionCheck Listen(IBGO send) {
            ProcessesIndex = allableList.FindIndex((x) => x == send);
            canSelect.Remove(send);
            return ConditionCheck.OK;
        }

        bool curIsPSet = false;
        public override bool Next() {
            if (ProcessesIndex >= 0) {
                if (!processes[ProcessesIndex].Next()) {
                    if (!curIsPSet) {
                        Current = processes[ProcessesIndex];
                        ProcessesIndex = -1;
                        return true;
                    }
                    ProcessesIndex = -1;
                    curIsPSet = false;
                } else {
                    Reset();
                    curIsPSet = true;
                    return true;
                }
            }

            if (ProcessesIndex == -1) {
                Current = this;
            }
            if (Current == this && IsEnd) {
                return false;
            }
            return true;
        }
        public override void Reset() {
            foreach (var it in allableList) {
                it.Unselectable();
            }
            ProcessesIndex = -1;
            curIsPSet = false;
            canSelect = null;
        }

    }
}

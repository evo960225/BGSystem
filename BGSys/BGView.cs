using hoshi_lib;
using hoshi_lib.View;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace BGS {

    public interface IBGO {
        BGO BGObject { get; }
        bool IsSelected { get; }
        bool IsSelectable { get; }
        void Select();
        void Unselect();
        void Selectable();
        void Unselectable();
        void SystemUpdate();
    }

    public interface IBGO<T> : IBGO {
        T BGData { get; set; }
    }

    abstract public class BGView<DataT> : HContainer,IHControl, IBGO<DataT> where DataT:BGO {
        #region private
        bool __isSelectable = false;
        bool __isSelected = false;
        Brush _selectableBorderColor = Brushes.YellowGreen;
        #endregion
        #region Event
        public event SelectEvent SelectedEvent {
            add { selectedEvent += value; }
            remove { selectedEvent -= value; }
        }
        protected SelectEvent selectedEvent;
        public event SelectEvent UnselectEvent {
            add { unselectEvent += value; }
            remove { unselectEvent -= value; }
        }
        protected SelectEvent unselectEvent;
        public event SelectEvent SelectableEvent {
            add { selectableEvent += value; }
            remove { selectableEvent -= value; }
        }
        protected SelectEvent selectableEvent;
        public event SelectEvent UnselectableEvent {
            add { unselectableEvent += value; }
            remove { unselectableEvent -= value; }
        }
        protected SelectEvent unselectableEvent;
        public event UpdateEvent UpdatedEvent {
            add { updatedEvent += value; }
            remove { updatedEvent -= value; }
        }
        protected UpdateEvent updatedEvent;
        #endregion
        #region hcontrol
        HControl _borderControl = new HControl();
        public Brush BorderBrush {
            get { return _borderControl.BorderBrush; }
            set { _borderControl.BorderBrush = value; }
        }
        public double? BorderWidth {
            get { return _borderControl.BorderWidth; }
            set { _borderControl.BorderWidth = value; }
        }
        public new System.Windows.Controls.UserControl This {
            get { return _borderControl.This; }
        }
        public FontFamily Font {
            get { return _borderControl.Font; }
            set { _borderControl.Font = value; }
        }
        public Brush FontColor {
            get { return _borderControl.BorderBrush; }
            set { _borderControl.BorderBrush = value; }
        }
        public double FontSize {
            get { return _borderControl.FontSize; }
            set { _borderControl.FontSize = value; }
        }
        public string Text {
            get { return _borderControl.Text; }
            set { _borderControl.Text = value; }
        }
        #endregion

        public bool IsSelected {
            get { return __isSelected; }
            protected set {
                var tmp = __isSelected;
                __isSelected = value;
                if (tmp != __isSelected) {
                    if (__isSelected) {
                        if (selectedEvent != null) selectedEvent();
                    } else {
                        if (unselectEvent != null) unselectEvent();
                    }
                }
            }
        }
        public bool IsSelectable {
            get {
                return __isSelectable;
            }
        }
        bool _isSelectable {
            get { return __isSelectable; }
            set { __isSelectable = value; }
        }

        public Brush SelectableBorderColor {
            get { return _selectableBorderColor; }
            set { _selectableBorderColor = value; }
        }

        public BGO BGObject {
            get { return BGData; }
        }
        abstract public DataT BGData {
            get;
            set;
        }
       
        public BGView() {
            this.AddElement(_borderControl);
            _borderControl.BorderWidth = 2;
            _borderControl.Background = null;
            Binding height =new Binding();
            height.Source=this;
            height.Path =new System.Windows.PropertyPath(BGView<DataT>.HeightProperty);
            _borderControl.SetBinding(HControl.HeightProperty,height);
            Binding width = new Binding();
            width.Source = this;
            width.Path = new System.Windows.PropertyPath(BGView<DataT>.WidthProperty);
            _borderControl.SetBinding(HControl.WidthProperty, width);

            
            AddMouseEvent(MouseButtonEvent.LeftButtonDown, (s, e) =>{
                if (IsSelectable) {
                    Select();
                    e.Handled = true;
                }
            });
            
            //Test:BGTimer
            BGSystem.MainTimer.Tick += (s, e) => this.SystemUpdate();
        }

        public void Select() {
            if (IsSelectable) {
                IsSelected = this.SendMsg();
            }
        }
        public void Unselect() {
            if (IsSelectable) {
                this.SendMsg();
            }
        }
        public void Selectable() {
            _isSelectable = true;
            BorderBrush = SelectableBorderColor;
            if (selectableEvent != null) selectableEvent();
        }
        public void Unselectable() {
            _isSelectable = false;
            __isSelected = false;
            this.BorderBrush = null;
            if (unselectableEvent != null) unselectableEvent();
        }
        public void EventClear() {
            selectedEvent = null;
            unselectEvent = null;
            selectableEvent = null;
            unselectableEvent = null;
            updatedEvent = null;
        }
        bool SendMsg() {
            var sys = BGSystem.Instance;
            if (sys.CurrentProcess == null) return false;

            var check = sys.CurrentProcess.Listen(this);
            if (check == ConditionCheck.OK) {
                IsSelected = true;
                if (sys.NextProcess()) {
                    sys.CurrentProcess.Do();
                }
            } else if (check == ConditionCheck.NotOK) {
                return false;
            }

            return true;
        }

        public abstract void SystemUpdate();


      
    }

    abstract public class BGDeckView<CardT> : BGView<BGOList<CardT>> where CardT:BGO {

        public BGDeckView() {
            //Test:BGTimer
            BGSystem.MainTimer.Tick += (s, e) => this.SystemUpdate();
        }


        public override void SystemUpdate() {
            throw new NotImplementedException();
        }
    }

    abstract public class BGContainer<BgT> : HContainer, IBGO<BgT> where BgT : BGO {

        #region private
        bool _isSelectable = false;
        bool __isSelected = false;
        Brush _selectableBorderColor = Brushes.YellowGreen;
        #endregion
        #region Event
        public event SelectEvent SelectedEvent {
            add { selectedEvent += value; }
            remove { selectedEvent -= value; }
        }
        protected SelectEvent selectedEvent;
        public event SelectEvent UnselectEvent {
            add { unselectEvent += value; }
            remove { unselectEvent -= value; }
        }
        protected SelectEvent unselectEvent;
        public event SelectEvent SelectableEvent {
            add { selectableEvent += value; }
            remove { selectableEvent -= value; }
        }
        protected SelectEvent selectableEvent;
        public event SelectEvent UnselectableEvent {
            add { unselectableEvent += value; }
            remove { unselectableEvent -= value; }
        }
        protected SelectEvent unselectableEvent;
        public event UpdateEvent UpdatedEvent {
            add { updatedEvent += value; }
            remove { updatedEvent -= value; }
        }
        protected UpdateEvent updatedEvent;
        #endregion

        public BGO BGObject {
            get { return BGData; }
        }
        abstract public BgT BGData {
            get;
            set;
        }

        public bool IsSelected {
            get { return __isSelected; }
            protected set {
                var tmp = __isSelected;
                __isSelected = value;
                if (tmp != __isSelected) {
                    if (__isSelected) {
                        if (selectedEvent != null) selectedEvent();
                    } else {
                        if (unselectEvent != null) unselectEvent();
                    }
                }
            }
        }
        public bool IsSelectable {
            get {
                return _isSelectable;
            }
        }

        public BGContainer() {
            //Test:BGTimer
            BGSystem.MainTimer.Tick += (s, e) => this.SystemUpdate();
        }

        abstract public void SystemUpdate();


        public void Select() {
            if (IsSelectable) {
                IsSelected = this.SendMsg();
            }
        }
        public void Unselect() {
            if (IsSelectable) {
                this.SendMsg();
            }
        }
        public void Selectable() {
            _isSelectable = true;
            if (selectableEvent != null) selectableEvent();
        }
        public void Unselectable() {
            _isSelectable = false;
            __isSelected = false;
            if (unselectableEvent != null) unselectableEvent();
        }
        public void EventClear() {
            selectedEvent = null;
            unselectEvent = null;
            selectableEvent = null;
            unselectableEvent = null;
            updatedEvent = null;
        }
        bool SendMsg() {
            var sys = BGSystem.Instance;
            if (sys.CurrentProcess == null) return false;

            var check = sys.CurrentProcess.Listen(this);
            if (check == ConditionCheck.OK) {
                IsSelected = true;
                if (sys.NextProcess()) {
                    sys.CurrentProcess.Do();
                }
            } else if (check == ConditionCheck.NotOK) {
                return false;
            }

            return true;
        }
    }


    abstract public class BGListView<BgV, BgT> : MatrixControl<BgV>, IBGO<BGOList<BgT>>
        where BgV : BGView<BgT>, new()
        where BgT : BGO {

        #region private
        bool _isSelectable = false;
        bool __isSelected = false;
        Brush _selectableBorderColor = Brushes.YellowGreen;
        #endregion
        #region Event
        public event SelectEvent SelectedEvent {
            add { selectedEvent += value; }
            remove { selectedEvent -= value; }
        }
        protected SelectEvent selectedEvent;
        public event SelectEvent UnselectEvent {
            add { unselectEvent += value; }
            remove { unselectEvent -= value; }
        }
        protected SelectEvent unselectEvent;
        public event SelectEvent SelectableEvent {
            add { selectableEvent += value; }
            remove { selectableEvent -= value; }
        }
        protected SelectEvent selectableEvent;
        public event SelectEvent UnselectableEvent {
            add { unselectableEvent += value; }
            remove { unselectableEvent -= value; }
        }
        protected SelectEvent unselectableEvent;
        public event UpdateEvent UpdatedEvent {
            add { updatedEvent += value; }
            remove { updatedEvent -= value; }
        }
        protected UpdateEvent updatedEvent;
        #endregion

        public BGO BGObject {
            get { return BGData; }
        }
        abstract public BGOList<BgT> BGData {
            get;
            set;
        }

        int _bgDataMax = -1;
        public int BGDataMax {
            get { return _bgDataMax; }
            set { _bgDataMax = value; }
        }
        
        public bool IsSelected {
            get { return __isSelected; }
            protected set {
                var tmp = __isSelected;
                __isSelected = value;
                if (tmp != __isSelected) {
                    if (__isSelected) {
                        if (selectedEvent != null) selectedEvent();
                    } else {
                        if (unselectEvent != null) unselectEvent();
                    }
                }
            }
        }
        public bool IsSelectable {
            get {
                return _isSelectable;
            }
        }

        public BGListView(MatrixSize size)
            : base(size, new Size(50, 50), 5, 5) {
                //Test:BGTimer
                BGSystem.MainTimer.Tick += (s, e) => this.SystemUpdate();
        }
   
        abstract public void SystemUpdate();


        public void Select() {
            if (IsSelectable) {
                IsSelected = this.SendMsg();
            }
        }
        public void Unselect() {
            if (IsSelectable) {
                this.SendMsg();
            }
        }
        public void Selectable() {
            _isSelectable = true;
            if (selectableEvent != null) selectableEvent();
        }
        public void Unselectable() {
            _isSelectable = false;
            __isSelected = false;
            if (unselectableEvent != null) unselectableEvent();
        }
        public void EventClear() {
            selectedEvent = null;
            unselectEvent = null;
            selectableEvent = null;
            unselectableEvent = null;
            updatedEvent = null;
        }
        bool SendMsg() {
            var sys = BGSystem.Instance;
            if (sys.CurrentProcess == null) return false;

            var check = sys.CurrentProcess.Listen(this);
            if (check == ConditionCheck.OK) {
                IsSelected = true;
                if (sys.NextProcess()) {
                    sys.CurrentProcess.Do();
                }
            } else if (check == ConditionCheck.NotOK) {
                return false;
            }

            return true;
        }


     
    }

  

}

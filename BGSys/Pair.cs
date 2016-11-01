using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace hoshi_lib {
    public class Pair<T1,T2> {
        T1 a;
        T2 b;
 
        public T1 A {
            get { return a; }
            set { a = value; }
        }
        public T2 B {
            get { return b; }
            set { b = value; }
        }
        public Pair() {

        }
        public Pair(T1 a, T2 b) {
            this.a = a;
            this.b = b;
        }
    }
}

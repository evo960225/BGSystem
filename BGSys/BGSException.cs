using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace BGS {
    public class BGSException : Exception {
        public BGSException()
            : base() {

        }
        public BGSException(string message)
            : base(message) {

        }
        public BGSException(string message, Exception innerException)
            : base(message, innerException) {

        }
        protected BGSException(SerializationInfo info, StreamingContext context)
            : base(info, context) {

        }
    }
}

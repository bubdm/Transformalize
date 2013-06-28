using System.Collections.Generic;
using System.Text;
using Transformalize.Model;

namespace Transformalize.Transforms {
    public class LeftTransform : ITransform {
        private readonly int _length;
        private readonly Dictionary<string, IField> _parameters;
        private readonly Dictionary<string, IField> _results;

        public LeftTransform(int length) {
            _length = length;
        }

        public LeftTransform(int length, Dictionary<string, IField> parameters, Dictionary<string, IField> results) {
            _length = length;
            _parameters = parameters;
            _results = results;
            HasParameters = parameters != null && parameters.Count > 0;
            HasResults = results != null && results.Count > 0;
        }

        public void Transform(StringBuilder sb) {
            sb.Left(_length);
        }

        public object Transform(object value) {
            return value.ToString().Left(_length);
        }

        public bool HasParameters { get; private set; }
        public bool HasResults { get; private set; }

        public void Dispose() { }
    }
}
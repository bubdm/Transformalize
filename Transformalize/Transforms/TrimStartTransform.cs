using System.Collections.Generic;
using System.Text;
using Transformalize.Model;

namespace Transformalize.Transforms
{
    public class TrimStartTransform : ITransform {
        private readonly string _trimChars;
        private readonly Dictionary<string, IField> _parameters;
        private readonly Dictionary<string, IField> _results;
        private readonly char[] _trimCharArray;

        public TrimStartTransform(string trimChars) {
            _trimChars = trimChars;
            _trimCharArray = trimChars.ToCharArray();
        }

        public TrimStartTransform(string trimChars, Dictionary<string, IField> parameters, Dictionary<string, IField> results) {
            _trimChars = trimChars;
            _parameters = parameters;
            _results = results;
            _trimCharArray = trimChars.ToCharArray();
            HasParameters = parameters != null && parameters.Count > 0;
            HasResults = results != null && results.Count > 0;
        }

        public void Transform(StringBuilder sb) {
            sb.TrimStart(_trimChars);
        }

        public object Transform(object value)
        {
            return value.ToString().TrimStart(_trimCharArray);
        }

        public bool HasParameters { get; private set; }
        public bool HasResults { get; private set; }

        public void Dispose() { }
    }
}
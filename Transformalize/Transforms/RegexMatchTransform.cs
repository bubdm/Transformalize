#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2017 Dale Newman
//  
// Licensed under the Apache License, Version 2.0 (the "License");
// you may not use this file except in compliance with the License.
// You may obtain a copy of the License at
//   
//       http://www.apache.org/licenses/LICENSE-2.0
//   
// Unless required by applicable law or agreed to in writing, software
// distributed under the License is distributed on an "AS IS" BASIS,
// WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
// See the License for the specific language governing permissions and
// limitations under the License.
#endregion

using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using Transformalize.Configuration;
using Transformalize.Contracts;

namespace Transformalize.Transforms {

    /// <summary>
    /// Pulls just the matched part out of any of the time (first hit)
    /// </summary>
    public class RegexMatchTransform : StringTransform {
        private readonly Field[] _input;
        private readonly Regex _regex;

        public RegexMatchTransform(IContext context = null) : base(context, "string") {
            if (IsMissingContext()) {
                return;
            }

            if (IsNotReceiving("string")) {
                return;
            }

            if (IsMissing(Context.Operation.Pattern)) {
                return;
            }

            _input = MultipleInput();

#if NETS10
            _regex = new Regex(Context.Operation.Pattern);
#else
            _regex = new Regex(Context.Operation.Pattern, RegexOptions.Compiled);
#endif

        }

        public override IRow Operate(IRow row) {
            foreach (var field in _input) {
                var match = _regex.Match(GetString(row, field));
                if (!match.Success)
                    continue;
                row[Context.Field] = match.Value;
                return row;
            }

            row[Context.Field] = Context.Field.Convert(Context.Field.Default);
            return row;
        }

        public override IEnumerable<OperationSignature> GetSignatures() {
            return new[] {
                new OperationSignature("match") {
                    Parameters =  new List<OperationParameter> {
                        new OperationParameter("pattern")
                    }
                }
            };
        }
    }
}
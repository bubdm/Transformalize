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
using Transformalize.Configuration;
using Transformalize.Contracts;
using Transformalize.Transforms;

namespace Transformalize.Validators {

    public class InValidator : BaseValidate {

        private readonly Field _input;
        private readonly HashSet<object> _set = new HashSet<object>();
        private readonly string _readableDomain;
        private readonly BetterFormat _betterFormat;

        public InValidator(IContext context) : base(context) {

            if (!Run)
                return;

            if (IsMissing(context.Operation.Domain)) {
                return;
            }

            _input = SingleInput();
            var items = Utility.Split(Context.Operation.Domain, ',');
            foreach (var item in items) {
                try {
                    _set.Add(_input.Convert(item));
                } catch (Exception ex) {
                    context.Warn($"In transform can't convert {item} to {_input.Type} {ex.Message}.");
                }
            }

            _readableDomain = Utility.ReadableDomain(items);
            var help = context.Field.Help;
            if (help == string.Empty) {
                help = $"{context.Field.Label} must be in {_readableDomain}.";
            }
            _betterFormat = new BetterFormat(context, help, context.Entity.GetAllFields);
        }

        public override IRow Operate(IRow row) {
            var valid = _set.Contains(row[_input]);
            row[ValidField] = valid;
            if (!valid) {
                AppendMessage(row, _betterFormat.Format(row));
            }
            Increment();
            return row;
        }
    }
}
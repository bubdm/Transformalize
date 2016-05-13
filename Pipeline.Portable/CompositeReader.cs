﻿#region license
// Transformalize
// A Configurable ETL solution specializing in incremental denormalization.
// Copyright 2013 Dale Newman
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
using System.Collections.Generic;
using System.Linq;
using Cfg.Net.Ext;
using Pipeline.Configuration;
using Pipeline.Contracts;

namespace Pipeline {

    public class CompositeReader : IRead {
        private readonly IEnumerable<IRead> _readers;

        public CompositeReader(params IRead[] readers) {
            _readers = readers;
        }

        public CompositeReader(IEnumerable<IRead> readers) {
            _readers = readers;
        }

        public IEnumerable<IRow> Read() {
            return _readers.SelectMany(reader => reader.Read());
        }
    }
}
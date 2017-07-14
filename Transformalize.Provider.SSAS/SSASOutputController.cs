﻿#region license
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
using Transformalize.Context;
using Transformalize.Contracts;

namespace Transformalize.Provider.SSAS {
    public class SSASOutputController : BaseOutputController {

        public SSASOutputController(
            OutputContext context,
            IAction initializer,
            IVersionDetector inputVersionDetector,
            IVersionDetector outputVersionDetector
            ) : base(context, initializer, inputVersionDetector, outputVersionDetector) { }

        public override void Start() {
            base.Start();

            // we do not actually write to SSAS, it reads directly from the input
            Context.Entity.BatchId = 0;
            Context.Entity.Identity = 0;
            Context.Entity.IsFirstRun = false;
        }
    }
}
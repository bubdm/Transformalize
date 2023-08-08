#region license
// Transformalize
// Configurable Extract, Transform, and Load
// Copyright 2013-2019 Dale Newman
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
using Transformalize.Contracts;

namespace Transformalize.Impl {

   [Obsolete("No longer necessary.  AsParallel() is not applied in DefaultDeleteHandler.")]
   public class ParallelDeleteHandler : IEntityDeleteHandler {
      private readonly IEntityDeleteHandler _deleteHandler;

      public ParallelDeleteHandler(IEntityDeleteHandler deleteHandler) {
         _deleteHandler = deleteHandler;
      }

      public IEnumerable<IRow> DetermineDeletes() {
         return _deleteHandler.DetermineDeletes();
      }

      public void Delete() {
         _deleteHandler.Delete();
      }
   }
}
﻿#region license
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
using System.Linq;
using Autofac;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Transformalize.Configuration;
using Transformalize.Containers.Autofac;
using Transformalize.Contracts;
using Transformalize.Providers.Console;

namespace Tests {

   [TestClass]
   public class TestSignature {

      [TestMethod]
      public void Validator() {
         const string xml = @"
    <add name='TestSignature'>
      <entities>
        <add name='TestData'>
          <rows>
            <add args='10,0' />
          </rows>
          <fields>
            <add name='args' length='128'>
                <transforms>
                    <add method='fromsplit' separator=','>
                        <fields>
                            <add name='TotalWidth' />
                            <add name='PaddingChar' />
                        </fields>
                    </add>
                </transforms>
            </add>
          </fields>
          <calculated-fields>
            <add name='length' type='int' t='copy(args).splitlength(\,)' />
            <add name='TotalWidthCheck' type='bool' t='copy(TotalWidth).is(int)' />
          </calculated-fields>
        </add>
      </entities>
    </add>";

         var logger = new ConsoleLogger(LogLevel.Debug);
         using (var cfgScope = new ConfigurationContainer().CreateScope(xml, logger)) {

            var process = cfgScope.Resolve<Process>();

            using (var scope = new Container().CreateScope(process, logger)) {
               var output = scope.Resolve<IProcessController>().Read().ToArray();


               var field = process.Entities.First().CalculatedFields.First(cf => cf.Name == "length");
               Assert.AreEqual(2, output[0][field]);

               foreach (var row in output) {
                  Console.WriteLine(row);
               }
            }
         }
      }
   }
}

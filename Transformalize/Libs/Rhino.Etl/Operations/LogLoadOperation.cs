#region License
// /*
// See license included in this library folder.
// */
#endregion

using System;
using System.Collections.Generic;
using System.Linq;
using Transformalize.Libs.fastJSON;
using Transformalize.Libs.NLog;
using Transformalize.Main;

namespace Transformalize.Libs.Rhino.Etl.Operations {
    public class LogLoadOperation : AbstractOperation {
        private readonly Logger _log = LogManager.GetLogger("file-output");
        private readonly List<string> _columns = new List<string>();
        private readonly string _name;

        public LogLoadOperation(Entity entity) {
            _name = Common.EntityOutputName(entity, entity.ProcessName);
            _columns.AddRange(new FieldSqlWriter(entity.Fields, entity.CalculatedFields).Output().ToArray().Select(f => f.Alias));
        }

        public override IEnumerable<Row> Execute(IEnumerable<Row> rows) {
            GlobalDiagnosticsContext.Set("output", _name);

            foreach (var row in rows) {
                _log.Info(JSON.Instance.ToJSON(_columns.ToDictionary(alias => alias, alias => row[alias])));
            }
            LogManager.Flush();
            yield break;
        }
    }
}
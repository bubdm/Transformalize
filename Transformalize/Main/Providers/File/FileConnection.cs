using Transformalize.Configuration;

namespace Transformalize.Main.Providers.File {
    public class FileConnection : AbstractConnection {

        public FileConnection(Process process, ConnectionConfigurationElement element, AbstractProvider provider, IConnectionChecker connectionChecker, IScriptRunner scriptRunner, IProviderSupportsModifier providerSupportsModifier, IEntityRecordsExist recordsExist, IEntityDropper dropper)
            : base(element, provider, connectionChecker, scriptRunner, providerSupportsModifier, recordsExist, dropper) {

            TypeAndAssemblyName = process.Providers[element.Provider.ToLower()];

            EntityKeysQueryWriter = new EmptyQueryWriter();
            EntityKeysRangeQueryWriter = new EmptyQueryWriter();
            EntityKeysAllQueryWriter = new EmptyQueryWriter();
            TableQueryWriter = new EmptyTableQueryWriter();
        }
    }
}
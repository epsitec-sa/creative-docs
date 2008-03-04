using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;

namespace Epsitec.ModuleRepository
{
	[ServiceBehavior]
	public class ModuleRepositoryService : IModuleRepositoryService
	{
		#region IModuleRepositoryService Members

		public int GetNewModuleId(string moduleName, string developerName)
		{
			return 1;
		}

		public ModuleDirectory CreateEmptyModule(int moduleId, string moduleLayerPrefix, string sourceNamespace)
		{
			List<ModuleFile> files = new List<ModuleFile> ();

			files.Add (new ModuleFile ()
			{
				Path = "module.info"
			});

			return new ModuleDirectory ()
			{
				Name = "Foo",
				Files = files.ToArray ()
			};
		}

		#endregion
	}
}

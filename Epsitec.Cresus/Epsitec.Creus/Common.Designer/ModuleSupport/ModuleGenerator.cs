//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.CodeGeneration;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Designer.ModuleSupport
{
	public static class ModuleGenerator
	{
		/// <summary>
		/// Creates the live modules.
		/// </summary>
		public static void CreateLiveModules()
		{
			ResourceManagerPool pool = new ResourceManagerPool ("ModuleGenerator");

			pool.SetupDefaultRootPaths ();

			pool.ScanForModules (ResourceManagerPool.SymbolicNames.Library);
			pool.ScanForModules (ResourceManagerPool.SymbolicNames.Patches);

			string liveRoot = pool.GetRootAbsolutePath (ResourceManagerPool.SymbolicNames.Live);

			List<ResourceModuleInfo> modules = ModuleGenerator.GetSourceModules (pool);

			foreach (ResourceModuleInfo module in modules)
			{
				string livePath = System.IO.Path.Combine (liveRoot, module.FullId.Name);
				ModuleMerger.Merge (pool, module, livePath);
			}
		}

		private static List<ResourceModuleInfo> GetSourceModules(ResourceManagerPool pool)
		{
			Dictionary<int, ResourceModuleInfo> moduleDict = new Dictionary<int, ResourceModuleInfo> ();

			foreach (ResourceModuleInfo info in pool.Modules)
			{
				ResourceModuleInfo existing;

				if (moduleDict.TryGetValue (info.FullId.Id, out existing))
				{
					if (info.IsPatchModule)
					{
						System.Diagnostics.Debug.Assert (existing.IsPatchModule == false);
						moduleDict[info.FullId.Id] = info;
					}
				}
				else
				{
					moduleDict[info.FullId.Id] = info;
				}
			}

			return Collection.ToList (moduleDict.Values);
		}
	}
}

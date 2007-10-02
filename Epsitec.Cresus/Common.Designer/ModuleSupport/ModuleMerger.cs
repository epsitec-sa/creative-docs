//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.CodeGeneration;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Designer.ModuleSupport
{
	public static class ModuleMerger
	{
		public static bool Merge(ResourceManagerPool pool, string modulePath, string outputPath)
		{
			ResourceModuleInfo info = pool.GetModuleInfo (modulePath);

			if (info == null)
			{
				System.Diagnostics.Debug.WriteLine ("Failed to locate module " + modulePath);
				return false;
			}
			if (string.IsNullOrEmpty (info.ReferenceModulePath))
			{
				System.Diagnostics.Debug.WriteLine ("Not a patch module : "+ modulePath);
				return false;
			}

			System.IO.Directory.CreateDirectory (outputPath);

			try
			{
				Module.Merge (info.FullId, outputPath);
			}
			catch (System.Exception ex)
			{
				System.Diagnostics.Debug.WriteLine ("Merge failed : " + ex.Message);
				return false;
			}

			return true;
		}
	}
}

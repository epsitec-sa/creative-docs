//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer;

using NUnit.Framework;

using System.Collections.Generic;

namespace Epsitec.Cresus.DataLayer
{
	[TestFixture]
	public class EntityCodeGeneratorTest
	{
		[Test]
		public void Check01CreateSourceDemo5juin()
		{
			ResourceManagerPool pool = new ResourceManagerPool ();
			pool.AddModuleRootPath ("%app%", @"S:\Epsitec.Cresus\Cresus.DataLayer.Tests");
			pool.ScanForAllModules ();
			ResourceManager manager = new ResourceManager (pool);
			manager.DefineDefaultModuleName ("Demo5juin");
			CodeGenerator generator = new CodeGenerator (manager);
			generator.Emit ();
			generator.Formatter.SaveCodeToTextFile (@"..\..\Demo5juinEntities.cs", System.Text.Encoding.UTF8);
		}
	}
}

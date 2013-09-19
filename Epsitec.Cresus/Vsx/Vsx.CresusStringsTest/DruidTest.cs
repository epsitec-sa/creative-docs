using System;
using System.Diagnostics;
using System.Linq;
using Epsitec.Cresus.ResourceManagement;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Epsitec.Cresus.Strings
{
	[TestClass]
	public class DruidTest
	{
		[TestMethod]
		public void Load()
		{
			var module = ResourceModule.Load (TestData.CommonDialogsModuleInfoPath);
			foreach (var bundle in module)
			{
				foreach (var item in bundle.Values)
				{
					//var x = Epsitec.Common.Support.Druid.FromModuleString (item.Id, int.Parse (item.Module.Info.Id));
					//var y = Epsitec.Common.Support.Druid.ToFullString (x);

					Trace.WriteLine (string.Format ("module: {0}, name: {1}, id: {2}, druid: {3}", item.Module.Info.Id, item.Id, item.Name, item.Druid));
				}
			}
		}
	}
}

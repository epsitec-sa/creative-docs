using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Epsitec.Cresus.Strings
{
	public static class TestData
	{
		public const string Directory = @"..\..\TestData\";
		public const string Strings00FileName = "Strings.00.resource";
		public const string StringsDeFileName = "Strings.de.resource";
		public const string Captions00FileName = "Captions.00.resource";
		public const string ModuleInfoFileName = "module.info";

		public static readonly string Strings00Path = TestData.Directory + TestData.Strings00FileName;
		public static readonly string StringsDePath = TestData.Directory + TestData.StringsDeFileName;
		public static readonly string Captions00Path = TestData.Directory + TestData.Captions00FileName;
		public static readonly string ModuleInfoPath = TestData.Directory + TestData.ModuleInfoFileName;

		public static readonly string CresusGraphSolutionPath = @"..\..\..\..\App.CresusGraph\App.CresusGraph.sln";
		public static readonly string CommonDialogsModuleInfoPath = @"..\..\..\..\Common\Resources\Common.Dialogs\module.info";
	}
}

using System;
using System.Globalization;
using System.IO;
using System.Linq;
using Epsitec.Cresus.ResourceManagement;
using Epsitec.Cresus.Strings.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Epsitec.Cresus.Strings
{
	[TestClass]
	public class ResourceModuleTest
	{
		[TestMethod]
		public void Load()
		{
			var module = ResourceModule.Load (TestData.ModuleInfoPath);
			Assert.AreEqual (3, module.Count ());
		}

		[TestMethod]
		public void ByNameFirst()
		{
			var module = ResourceModule.Load (TestData.ModuleInfoPath).ByNameFirst;
			var stringsFr = module["Strings"][French];
			var stringsDe = module["Strings"][German];
			var captionsFr = module["Captions"][French];

			var item1 = module["Strings"][French].ByName["Application.Name"];
			var item2 = module["Strings"][French]["A"];

			Assert.AreEqual (item1, item2);
		}

		[TestMethod]
		public void ByCultureFirst()
		{
			var module = ResourceModule.Load (TestData.ModuleInfoPath).ByCultureFirst;
			var frStrings = module[French]["Strings"];
			var deStrings = module[German]["Strings"];
			var frCaptions = module[French]["Captions"];
		}

		[TestMethod]
		[STAThread]
		public void DisplayMultiCultureHyperlink_MoreThanPiccolo()
		{
			var module = ResourceModule.Load (TestData.ModuleInfoPath);
			var mapper = new ResourceMapper ();
			mapper.VisitModule (module);

			var symbolMap = mapper.MatchItemSymbolTail ("Res.Strings.Message.MoreThanPiccolo");
			var view = MultiCultureResourceItemCollectionView.Create (symbolMap);

			var window = WpfHelper.CreateWindow ();
			window.Content = view;
			window.ShowDialog ();
		}

		[TestMethod]
		[STAThread]
		public void DisplayMultiCulture_DialogOpenFile()
		{
			var module = ResourceModule.Load (@"..\..\..\..\Common\Resources\Common.Dialogs\module.info");
			var mapper = new ResourceMapper ();
			mapper.VisitModule (module);

			var symbolMap = mapper.MatchItemSymbolTail ("Epsitec.Common.Dialogs.Res.Strings.Dialog.Question.Open.File");
			var view = MultiCultureResourceItemCollectionView.Create (symbolMap);

			var window = WpfHelper.CreateWindow ();
			window.Content = view;
			window.ShowDialog ();
		}

		private static readonly CultureInfo French = CultureInfo.CreateSpecificCulture ("fr");
		private static readonly CultureInfo German = CultureInfo.CreateSpecificCulture ("de");
		private static readonly CultureInfo English = CultureInfo.CreateSpecificCulture ("en");
	}
}

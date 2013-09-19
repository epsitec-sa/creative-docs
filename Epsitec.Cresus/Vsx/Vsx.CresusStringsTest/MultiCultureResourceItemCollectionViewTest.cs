using System;
using System.Threading;
using Epsitec.Cresus.ResourceManagement;
using Epsitec.Cresus.Strings.Views;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Roslyn.Services;

namespace Epsitec.Cresus.Strings
{
	[TestClass]
	public class MultiCultureResourceItemCollectionViewTest
	{
		[TestMethod]
		[STAThread]
		public void MultiLineItem()
		{
			var module = ResourceModule.Load (@"..\..\..\..\Common\Resources\Common.Dialogs\module.info");
			var mapper = new ResourceSymbolMapper ();
			mapper.VisitModule (module);

			var resources = mapper.FindTail ("Epsitec.Common.Dialogs.Res.Strings.Dialog.Question.Open.File", CancellationToken.None);
			var view = new MultiCultureResourceItemCollectionView (resources);

			var window = WpfHelper.CreateWindow ();
			window.Content = view;
			window.ShowDialog ();
		}

		[TestMethod]
		[STAThread]
		public void MultiLineItem_NoSymbol()
		{
			var module = ResourceModule.Load (@"..\..\..\..\Common\Resources\Common.Dialogs\module.info");
			var mapper = new ResourceSymbolMapper ();
			mapper.VisitModule (module);

			var resources = mapper.FindTail ("Epsitec.Common.Dialogs.Res.Strings.Dialog.Question.Open.File", CancellationToken.None);
			var view = new MultiCultureResourceItemCollectionView (resources);

			var window = WpfHelper.CreateWindow ();
			window.Content = view;
			window.ShowDialog ();
		}

		[TestMethod]
		[STAThread]
		public void Hyperlink()
		{
			var module = ResourceModule.Load (TestData.ModuleInfoPath);
			var mapper = new ResourceSymbolMapper ();
			mapper.VisitModule (module);

			var resources = mapper.FindTail ("Res.Strings.Message.MoreThanPiccolo", CancellationToken.None);
			var view = new MultiCultureResourceItemCollectionView (resources);

			var window = WpfHelper.CreateWindow ();
			window.Content = view;
			window.ShowDialog ();
		}

		[TestMethod]
		[STAThread]
		public void FirstLevelCollection()
		{
			var module = ResourceModule.Load (TestData.ModuleInfoPath);
			var mapper = new ResourceSymbolMapper ();
			mapper.VisitModule (module);

			var resources = mapper.FindPartial ("Res.Strings.Message", CancellationToken.None);
			var view = new MultiCultureResourceItemCollectionView (resources);

			var window = WpfHelper.CreateWindow ();
			window.Content = view;
			window.ShowDialog ();
		}

		[TestMethod]
		[STAThread]
		public void SecondLevelCollection()
		{
			var module = ResourceModule.Load (TestData.ModuleInfoPath);
			var mapper = new ResourceSymbolMapper ();
			mapper.VisitModule (module);

			var resources = mapper.FindPartial ("Res.Strings", CancellationToken.None);
			var view = new MultiCultureResourceItemCollectionView (resources);

			var window = WpfHelper.CreateWindow ();
			window.Content = view;
			window.ShowDialog ();
		}

		[TestMethod]
		[STAThread]
		public void Solution()
		{
			var workspace = Workspace.LoadSolution (TestData.CresusGraphSolutionPath);
			var solution = new SolutionResource (workspace.CurrentSolution);
			var mapper = new ResourceSymbolMapper ();
			mapper.VisitSolution (solution);

			var resources = mapper.FindPartial ("Res.Strings", CancellationToken.None);
			var view = new MultiCultureResourceItemCollectionView (resources);

			var window = WpfHelper.CreateWindow ();
			window.Content = view;
			window.ShowDialog ();
		}
	}
}

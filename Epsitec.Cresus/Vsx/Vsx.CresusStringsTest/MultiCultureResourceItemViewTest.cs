using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Navigation;
using System.Xml.Linq;
using Epsitec.Cresus.ResourceManagement;
using Epsitec.Cresus.Strings.ViewModels;
using Epsitec.Cresus.Strings.Views;
using Epsitec.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Epsitec.Cresus.Strings
{
	[TestClass]
	public class MultiCultureResourceItemViewTest
	{
		[TestMethod]
		[STAThread]
		public void Hyperlink_SingleCulture()
		{
			var bundle = ResourceBundle.Load (TestData.Strings00Path);
			var mapper = new ResourceSymbolMapper ();
			mapper.VisitBundle (bundle);

			var resources = mapper.FindTail ("Res.Strings.Message.MoreThanPiccolo", CancellationToken.None);
			var view = new MultiCultureResourceItemView (resources.Single ())
			{
				Margin = new Thickness (12)
			};

			var window = WpfHelper.CreateWindow ();
			window.Content = view;
			window.ShowDialog ();
		}

		[TestMethod]
		[STAThread]
		public void XmlView()
		{
			var bundle = ResourceBundle.Load (TestData.Captions00Path);
			var mapper = new ResourceSymbolMapper ();
			mapper.VisitBundle (bundle);

			var resources = mapper.FindTail ("Res.Captions", CancellationToken.None);
			var view = new MultiCultureResourceItemView (resources.Single ())
			{
				Margin = new Thickness (12)
			};

			var window = WpfHelper.CreateWindow ();
			window.Content = view;
			window.ShowDialog ();
		}

		[TestMethod]
		[STAThread]
		public void Hyperlink_MultiCulture()
		{
			var module = ResourceModule.Load (TestData.ModuleInfoPath);
			var mapper = new ResourceSymbolMapper ();
			mapper.VisitModule (module);

			var resources = mapper.FindTail ("Res.Strings.Message.MoreThanPiccolo", CancellationToken.None);
			var view = new MultiCultureResourceItemView (resources.Single ());

			var window = WpfHelper.CreateWindow ();
			window.Content = view;
			window.ShowDialog ();
		}
	}
}

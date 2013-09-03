using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
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
	public class ResourceItemViewTest
	{
		[TestMethod]
		[STAThread]
		public void InlinesSpike()
		{
			// <img src="http://www.epsitec.ch/products/graphe/images/boite-graphe1.gif" />

			var window = new Window ();
			var stackPanel = new StackPanel();
			window.Content = stackPanel;

			var p1 = new TextBlock ();
			stackPanel.Children.Add (p1);
	
			p1.Inlines.Add ("Souhaitez-vous faire plus avec Crésus Graphe ?");
			p1.Inlines.Add (new LineBreak ());
			p1.Inlines.Add ("Découvrez la ");

			var s1 = new Hyperlink ();
			s1.NavigateUri = new Uri ("http://www.epsitec.ch/products/graphe/spec#PRO");
			s1.RequestNavigate += HandleRequestNavigate;
			s1.Inlines.Add ("version ");

			var r1 = new Run ();
			//(r1 as System.Windows.Markup.IAddChild).AddText("PRO");
			r1.Text = "PRO";
			r1.FontWeight = FontWeights.Bold;

			s1.Inlines.Add (r1);

			p1.Inlines.Add (s1);
			p1.Inlines.Add (" !");

			window.ShowDialog ();

			s1.RequestNavigate -= HandleRequestNavigate;
		}

		[TestMethod]
		[STAThread]
		public void DisplayHyperlink_MoreThanPiccolo()
		{
			var bundle = new ResourceBundle (TestData.Strings00Path);
			var item = bundle.ByName["Message.MoreThanPiccolo"];

			var viewModel = new ResourceItemViewModel (item, bundle.Culture);
			viewModel.Value = item.Value;

			var window = WpfHelper.CreateWindow ();
			var view = new ResourceItemView ();
			view.DataContext = viewModel;

			window.Content = view;
			window.ShowDialog ();
		}

		private static void HandleRequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			Process.Start (new ProcessStartInfo (e.Uri.AbsoluteUri));
			e.Handled = true;
		}
	}
}

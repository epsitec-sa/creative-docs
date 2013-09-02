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
using Epsitec.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Epsitec.Cresus.Strings
{
	[TestClass]
	public class ResourceItemViewTest
	{
		[TestMethod]
		[STAThread]
		public void Mvvm()
		{
			var bundle = new ResourceBundle (TestData.Strings00Path);
			var item = bundle.ByName["Message.MoreThanPiccolo"];

			var viewModel = new ResourceItemViewModel ();
			viewModel.Value = item.Value;

			var window = new Window ();
			var view = new ResourceItemView ();
			view.DataContext = viewModel;

			window.Content = view;
			window.ShowDialog ();
		}

		[TestMethod]
		[STAThread]
		public void Inlines_Manual()
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
		public void Content()
		{
			var bundle = new ResourceBundle (TestData.Strings00Path);
			var item = bundle.ByName["Message.MoreThanPiccolo"];

			var window = new Window ();
			var control = new TextBlock ();
			TextLayout.SetHtml(control, item.Value);
			window.Content = control;
			window.ShowDialog ();
		}

		[TestMethod]
		[STAThread]
		public void ContentAndSymbol()
		{
			var bundle = new ResourceBundle (TestData.Strings00Path);
			var item = bundle.ByName["Message.MoreThanPiccolo"];

			var window = new Window ();

			var grid = new Grid ();
			grid.ColumnDefinitions.Add (new ColumnDefinition ()
			{
				Width = GridLength.Auto
			});
			grid.RowDefinitions.Add (new RowDefinition ()
			{
				Height = GridLength.Auto
			});
			grid.RowDefinitions.Add (new RowDefinition ()
			{
				Height = GridLength.Auto
			});

			var border0 = new Border ()
			{
				Padding = new Thickness (6),
				BorderThickness = new Thickness (1.0),
				BorderBrush = Brushes.Black,
				Child = new TextBlock ()
				{
					Text = item.Name
				}
			};

			var control = new TextBlock ();
			TextLayout.SetHtml (control, item.Value);
			var border1 = new Border ()
			{
				Padding = new Thickness (6),
				BorderThickness = new Thickness (1, 0, 1, 1),
				BorderBrush = Brushes.Black,
				Child = control
			};
			Grid.SetRow (border1, 1);

			grid.Children.Add (border0);
			grid.Children.Add (border1);

			window.Content = grid;
			window.ShowDialog ();
		}

		[TestMethod]
		[STAThread]
		public void ContentSymbolAndCulture()
		{
			var bundle = new ResourceBundle (TestData.Strings00Path);
			var item = bundle.ByName["Message.MoreThanPiccolo"];

			var window = new Window ();

			var grid = new Grid ();
			grid.ColumnDefinitions.Add (new ColumnDefinition ()
			{
				Width = GridLength.Auto
			});
			grid.ColumnDefinitions.Add (new ColumnDefinition ()
			{
				Width = GridLength.Auto
			});

			grid.RowDefinitions.Add (new RowDefinition ()
			{
				Height = GridLength.Auto
			});
			grid.RowDefinitions.Add (new RowDefinition ()
			{
				Height = GridLength.Auto
			});
			grid.RowDefinitions.Add (new RowDefinition ()
			{
				Height = GridLength.Auto
			});

			// row 0 : symbol
			var border0 = new Border ()
			{
				Padding = new Thickness (6),
				BorderThickness = new Thickness (1.0),
				BorderBrush = Brushes.Black,
				Child = new TextBlock ()
				{
					Text = item.Name,
					HorizontalAlignment = HorizontalAlignment.Center
				}
			};
			Grid.SetColumnSpan (border0, 2);

			// row 1, col 0
			var border10 = new Border ()
			{
				Padding = new Thickness (6),
				BorderThickness = new Thickness (1, 0, 1, 1),
				BorderBrush = Brushes.Black,
				Child = new TextBlock ()
				{
					Text = bundle.Culture.Parent.DisplayName
				}
			};
			Grid.SetRow (border10, 1);
			Grid.SetColumn (border10, 0);

			// row 1, col 1
			var control = new TextBlock ();
			TextLayout.SetHtml (control, item.Value);
			var border11 = new Border ()
			{
				Padding = new Thickness (6),
				BorderThickness = new Thickness (0, 0, 1, 1),
				BorderBrush = Brushes.Black,
				Child = control
			};
			Grid.SetRow (border11, 1);
			Grid.SetColumn (border11, 1);


			grid.Children.Add (border0);
			grid.Children.Add (border10);
			grid.Children.Add (border11);

			window.Content = grid;
			window.ShowDialog ();
		}

		private static void HandleRequestNavigate(object sender, RequestNavigateEventArgs e)
		{
			Process.Start (new ProcessStartInfo (e.Uri.AbsoluteUri));
			e.Handled = true;
		}
	}
}

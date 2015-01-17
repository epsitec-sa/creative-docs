using System;
using System.Windows.Controls;
using Epsitec.Windows;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Epsitec.Cresus.Strings
{
	[TestClass]
	public class XLayoutTest
	{
		[TestMethod]
		[STAThread]
		public void DecodeHtmlEntities()
		{
			Assert.AreEqual ("<data true=\"zero &lt; one\"/>", XLayout.Builder.UnescapeHtmlEntities ("&lt;data true=\"zero &amp;lt; one\"/&gt;").Value);
		}

		[TestMethod]
		[STAThread]
		public void AppendSimpleMarkup1Single()
		{
			var textBlock = new TextBlock ();
			XLayout.SetText (textBlock, Data.SimpleMarkup1);

			var window = WpfHelper.CreateWindow ();
			window.Content = textBlock;
			window.ShowDialog ();
		}

		[TestMethod]
		[STAThread]
		public void AppendSimpleMarkup2Single()
		{
			var textBlock = new TextBlock ();
			XLayout.SetText (textBlock, Data.SimpleMarkup2);

			var window = WpfHelper.CreateWindow ();
			window.Content = textBlock;
			window.ShowDialog ();
		}

		[TestMethod]
		[STAThread]
		public void AppendComplexMarkup1Single()
		{
			var textBlock = new TextBlock ();
			XLayout.SetText (textBlock, Data.ComplexMarkup1);

			var window = WpfHelper.CreateWindow ();
			window.Content = textBlock;
			window.ShowDialog ();
		}

		[TestMethod]
		[STAThread]
		public void AppendComplexMarkup2Single()
		{
			var textBlock = new TextBlock ();
			XLayout.SetText (textBlock, Data.ComplexMarkup2);

			var window = WpfHelper.CreateWindow ();
			window.Content = textBlock;
			window.ShowDialog ();
		}

		[TestMethod]
		[STAThread]
		public void AppendSimpleMarkup1Multiple()
		{
			var textBlock = new TextBlock ();
			var markup = Data.SimpleMarkup1;
			XLayout.SetText (textBlock, markup + markup);

			var window = WpfHelper.CreateWindow ();
			window.Content = textBlock;
			window.ShowDialog ();
		}


		public static class Data
		{
			public const string EscapedSimpleMarkup1 = 
				"Souhaitez-vous faire plus avec Crésus Graphe ?&lt;br/&gt;Découvrez la &lt;a href=\"http://www.epsitec.ch/products/graphe/spec#PRO\"&gt;version PRO&lt;/a&gt; !";

			public const string EscapedSimpleMarkup2 = 
				"&lt;s:data id=\"_0\"" +
				" f:Name=\"Message.LicenseInvalid.Question\"" +
				" f:Description=\"Pour pouvoir utiliser Crésus Graphe, vous devez soit disposer d&amp;amp;apos;une licence correspondante, soit être au bénéfice d&amp;amp;apos;un logiciel Crésus Comptabilité avec droit aux mises à jour.&amp;lt;br/&amp;gt;Que souhaitez-vous faire ?\"" +
				" f:Labels=\"{Collection {Text 'Pas de licence valide pour Crésus Graphe.'}, {Text 'Licence échue'}}\" /&gt;";

			public const string EscapedComplexMarkup1 = 
				"&lt;data &gt;" +
				"	&lt;data1 &gt;" +
					"	&lt;data11 &gt;" +
					"	&lt;/data11 &gt;" +
				"	&lt;/data1 &gt;" +
				"	Crésus Graphe {0} — version {1}" +
				"&lt;/data&gt;";

			public const string EscapedComplexMarkup2 = 
				"&lt;s:data id=\"_0\"" +
				" f:Name=\"Message.LicenseInvalid.Question\"" +
				" f:Description=\"Pour pouvoir utiliser Crésus Graphe, vous devez soit disposer d&amp;amp;apos;une licence correspondante, soit être au bénéfice d&amp;amp;apos;un logiciel Crésus Comptabilité avec droit aux mises à jour.&amp;lt;br/&amp;gt;Que souhaitez-vous faire ?\"" +
				" f:Labels=\"{Collection {Text 'Pas de licence valide pour Crésus Graphe.'}, {Text 'Licence échue'}}\" &gt;" +
				"	Souhaitez-vous faire plus avec Crésus Graphe ?&lt;br/&gt;Découvrez la &lt;a href=\"http://www.epsitec.ch/products/graphe/spec#PRO\"&gt;version PRO&lt;/a&gt; !" +
				"   &lt;br/&gt;" +
				"   &lt;br/&gt;" +
				"	&lt;i&gt;Note: licence active tant que le droit aux mises à jour pour &lt;b&gt;Crésus Comptabilité&lt;/b&gt; est valable (échéance: {0}).&lt;/i&gt;" +
				"	&lt;s:data id=\"_0\" f:Name=\"DocumentView.Options.AccumulateValues\" f:Labels=\"{Collection {Text 'accumulation des valeurs'}}\" /&gt;" +
				"	Crésus Graphe {0} — version {1}" +
				"&lt;/s:data&gt;";

			public static string SimpleMarkup1
			{
				get
				{
					return XLayout.UnescapeHtmlEntities (Data.EscapedSimpleMarkup1).Value;
				}
			}

			public static string SimpleMarkup2
			{
				get
				{
					return XLayout.UnescapeHtmlEntities (Data.EscapedSimpleMarkup2).Value;
				}
			}

			public static string ComplexMarkup1
			{
				get
				{
					return XLayout.UnescapeHtmlEntities (Data.EscapedComplexMarkup1).Value;
				}
			}

			public static string ComplexMarkup2
			{
				get
				{
					return XLayout.UnescapeHtmlEntities (Data.EscapedComplexMarkup2).Value;
				}
			}
		}
	}
}

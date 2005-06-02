//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Tests
{
	/// <summary>
	/// Vérifie le bon fonctionnement des propriétés.
	/// </summary>
	public sealed class CheckProperties
	{
		public static void RunTests()
		{
			CheckProperties.TestFont ();
			CheckProperties.TestFontSize ();
			CheckProperties.TestMargins ();
		}

		
		private static void TestFont()
		{
			Properties.FontProperty font_a = new Properties.FontProperty ("Futura", "Roman");
			Properties.FontProperty font_b = new Properties.FontProperty (null, "Heavy");
			Properties.FontProperty font_c = new Properties.FontProperty ("Arial", null);
			
			Properties.FontProperty font_ab = font_a.GetCombination (font_b) as Properties.FontProperty;
			Properties.FontProperty font_ac = font_a.GetCombination (font_c) as Properties.FontProperty;
			
			Debug.Assert.IsTrue (font_ab.FaceName == "Futura");
			Debug.Assert.IsTrue (font_ab.StyleName == "Heavy");
			Debug.Assert.IsTrue (font_ac.FaceName == "Arial");
			Debug.Assert.IsTrue (font_ac.StyleName == "Roman");
			
			Debug.Assert.IsTrue (font_a.ToString () == "Futura/Roman/[null]");
			Debug.Assert.IsTrue (font_b.ToString () == "[null]/Heavy/[null]");
			Debug.Assert.IsTrue (font_c.ToString () == "Arial/[null]/[null]");
		}
		
		private static void TestFontSize()
		{
			Properties.FontSizeProperty font_size_a = new Properties.FontSizeProperty (12.0, Properties.SizeUnits.Points);
			Properties.FontSizeProperty font_size_b = new Properties.FontSizeProperty (50.0, Properties.SizeUnits.Percent);
			Properties.FontSizeProperty font_size_c = new Properties.FontSizeProperty (-2.0, Properties.SizeUnits.DeltaPoints);
			Properties.FontSizeProperty font_size_d = new Properties.FontSizeProperty (200.0, Properties.SizeUnits.Percent);
			
			Properties.FontSizeProperty font_size_ab = font_size_a.GetCombination (font_size_b) as Properties.FontSizeProperty;
			Properties.FontSizeProperty font_size_ba = font_size_b.GetCombination (font_size_a) as Properties.FontSizeProperty;
			Properties.FontSizeProperty font_size_ac = font_size_a.GetCombination (font_size_c) as Properties.FontSizeProperty;
			Properties.FontSizeProperty font_size_cb = font_size_c.GetCombination (font_size_b) as Properties.FontSizeProperty;
			Properties.FontSizeProperty font_size_bd = font_size_b.GetCombination (font_size_d) as Properties.FontSizeProperty;
			
			Debug.Assert.IsTrue (font_size_ab.ToString () == "6/Points");
			Debug.Assert.IsTrue (font_size_ba.ToString () == "12/Points");
			Debug.Assert.IsTrue (font_size_ac.ToString () == "10/Points");
			Debug.Assert.IsTrue (font_size_cb.ToString () == "-1/DeltaPoints");
			Debug.Assert.IsTrue (font_size_bd.ToString () == "100/Percent");
			
			Debug.Expect.Exception (new Debug.Method (Ex1), typeof (System.InvalidOperationException));
		}
		
		private static void TestMargins()
		{
			Properties.MarginsProperty margins_a = new Properties.MarginsProperty ();
			Properties.MarginsProperty margins_b = new Properties.MarginsProperty (15.0, 20.0, 0.0, 0.0, Properties.SizeUnits.Points, 0, 0, 0, 0, 0, false);
			
			Properties.MarginsProperty margins_c = margins_a.GetCombination (margins_b) as Properties.MarginsProperty;
			Properties.MarginsProperty margins_d = margins_b.GetCombination (margins_a) as Properties.MarginsProperty;
			
			margins_a.LeftMarginBody  = 10;
			margins_a.RightMarginBody = 10;
			
			Properties.MarginsProperty margins_e = margins_a.GetCombination (margins_b) as Properties.MarginsProperty;
			Properties.MarginsProperty margins_f = margins_b.GetCombination (margins_a) as Properties.MarginsProperty;
			
			Debug.Assert.IsTrue (margins_c.LeftMarginFirstLine == 15.0);
			Debug.Assert.IsTrue (margins_c.LeftMarginBody      == 20.0);
			Debug.Assert.IsTrue (margins_c.RightMarginBody     ==  0.0);
			
			Debug.Assert.IsTrue (margins_d.LeftMarginFirstLine == 15.0);
			Debug.Assert.IsTrue (margins_d.LeftMarginBody      == 20.0);
			Debug.Assert.IsTrue (margins_d.RightMarginBody     ==  0.0);
			
			Debug.Assert.IsTrue (margins_e.LeftMarginFirstLine == 15.0);
			Debug.Assert.IsTrue (margins_e.LeftMarginBody      == 20.0);
			Debug.Assert.IsTrue (margins_e.RightMarginBody     ==  0.0);
			
			Debug.Assert.IsTrue (margins_f.LeftMarginFirstLine == 15.0);
			Debug.Assert.IsTrue (margins_f.LeftMarginBody      == 10.0);
			Debug.Assert.IsTrue (margins_f.RightMarginBody     == 10.0);
			
			Debug.Assert.IsTrue (margins_a.ToString () == "[NaN]/10/[NaN]/10/0/0/0/0/0/[false]");
			Debug.Assert.IsTrue (margins_b.ToString () == "15/20/0/0/0/0/0/0/0/[false]");
		}
		
		
		private static void Ex1()
		{
			Properties.FontSizeProperty a = new Properties.FontSizeProperty (80.0, Properties.SizeUnits.Percent);
			Properties.FontSizeProperty b = new Properties.FontSizeProperty (-5.0, Properties.SizeUnits.DeltaPoints);
			
			a.GetCombination (b);
		}
	}
}

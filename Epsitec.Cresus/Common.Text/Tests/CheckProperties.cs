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
			Properties.FontProperty font_a = new Properties.FontProperty ("Futura", "Roman");
			Properties.FontProperty font_b = new Properties.FontProperty (null, "Heavy");
			Properties.FontProperty font_c = new Properties.FontProperty ("Arial", null);
			
			Properties.FontProperty font_ab = font_a.GetCombination (font_b) as Properties.FontProperty;
			Properties.FontProperty font_ac = font_a.GetCombination (font_c) as Properties.FontProperty;
			
			Debug.Assert.IsTrue (font_ab.FaceName == "Futura");
			Debug.Assert.IsTrue (font_ab.StyleName == "Heavy");
			Debug.Assert.IsTrue (font_ac.FaceName == "Arial");
			Debug.Assert.IsTrue (font_ac.StyleName == "Roman");
			
			Debug.Assert.IsTrue (font_a.ToString () == "Futura/Roman");
			Debug.Assert.IsTrue (font_b.ToString () == "<null>/Heavy");
			Debug.Assert.IsTrue (font_c.ToString () == "Arial/<null>");
			
			Properties.FontSizeProperty font_size_a = new Properties.FontSizeProperty (12.0, Properties.FontSizeUnits.Points);
			Properties.FontSizeProperty font_size_b = new Properties.FontSizeProperty (50.0, Properties.FontSizeUnits.Percent);
			Properties.FontSizeProperty font_size_c = new Properties.FontSizeProperty (-2.0, Properties.FontSizeUnits.DeltaPoints);
			Properties.FontSizeProperty font_size_d = new Properties.FontSizeProperty (200.0, Properties.FontSizeUnits.Percent);
			
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
		
		private static void Ex1()
		{
			Properties.FontSizeProperty a = new Properties.FontSizeProperty (80.0, Properties.FontSizeUnits.Percent);
			Properties.FontSizeProperty b = new Properties.FontSizeProperty (-5.0, Properties.FontSizeUnits.DeltaPoints);
			
			a.GetCombination (b);
		}
	}
}

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
			CheckProperties.TestUnderlines ();
			CheckProperties.TestSerialization ();
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
			
			Debug.Assert.IsTrue (font_size_ab.ToString () == "6/pt");
			Debug.Assert.IsTrue (font_size_ba.ToString () == "12/pt");
			Debug.Assert.IsTrue (font_size_ac.ToString () == "10/pt");
			Debug.Assert.IsTrue (font_size_cb.ToString () == "-1/+pt");
			Debug.Assert.IsTrue (font_size_bd.ToString () == "100/%");
			
			Debug.Expect.Exception (new Debug.Method (Ex1), typeof (System.InvalidOperationException));
		}
		
		private static void TestMargins()
		{
			Properties.MarginsProperty margins_a = new Properties.MarginsProperty ();
			Properties.MarginsProperty margins_b = new Properties.MarginsProperty (15.0, 20.0, 0.0, 0.0, Properties.SizeUnits.Points, 0, 0, 0, 0, 0, Properties.ThreeState.False);
			
			Properties.MarginsProperty margins_c = margins_a.GetCombination (margins_b) as Properties.MarginsProperty;
			Properties.MarginsProperty margins_d = margins_b.GetCombination (margins_a) as Properties.MarginsProperty;
			
			margins_a.Units = Properties.SizeUnits.Points;
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
			
			Debug.Assert.IsTrue (margins_a.ToString () == "[NaN]/10/[NaN]/10/pt/0/0/0/0/0/[?]");
			Debug.Assert.IsTrue (margins_b.ToString () == "15/20/0/0/pt/0/0/0/0/0/[false]");
		}
		
		private static void TestUnderlines()
		{
			TextStory story = new TextStory ();
			
			ulong[] text;
			System.Collections.ArrayList properties = new System.Collections.ArrayList ();
			
			properties.Add (new Properties.FontProperty ("Arial", "Regular"));
			properties.Add (new Properties.FontSizeProperty (12.0, Properties.SizeUnits.Points));
			properties.Add (new Properties.UnderlineProperty (double.NaN, Properties.SizeUnits.None, double.NaN, Properties.SizeUnits.None, "underline", "-"));
			properties.Add (new Properties.UnderlineProperty (double.NaN, Properties.SizeUnits.None, double.NaN, Properties.SizeUnits.None, "underline", "color=red"));
			properties.Add (new Properties.UnderlineProperty (double.NaN, Properties.SizeUnits.None, double.NaN, Properties.SizeUnits.None, "frame", "backcolor=yellow;color=black"));
			properties.Add (new Properties.UnderlineProperty (double.NaN, Properties.SizeUnits.None, 2.0, Properties.SizeUnits.Points, "underline", "color=red"));
			
			story.ConvertToStyledText ("Abc", properties, out text);
			
			System.Collections.ArrayList underlines = new System.Collections.ArrayList ();
			
			story.TextContext.GetUnderlines (text[0], underlines);
			
			Debug.Assert.IsTrue (underlines.Count == 4);
			
			underlines.Sort (Properties.UnderlineProperty.Comparer);
			
			Debug.Assert.IsTrue ((underlines[0] as Properties.UnderlineProperty).LineStyle == "backcolor=yellow;color=black");
			Debug.Assert.IsTrue ((underlines[1] as Properties.UnderlineProperty).LineStyle == "-");
			Debug.Assert.IsTrue ((underlines[2] as Properties.UnderlineProperty).LineStyle == "color=red");
			Debug.Assert.IsTrue ((underlines[3] as Properties.UnderlineProperty).LineStyle == "color=red");
			Debug.Assert.IsTrue ((underlines[2] as Properties.UnderlineProperty).ThicknessUnits == Properties.SizeUnits.None);
			Debug.Assert.IsTrue ((underlines[3] as Properties.UnderlineProperty).ThicknessUnits == Properties.SizeUnits.Points);
		}
		
		private static void TestSerialization()
		{
			Properties.FontProperty     p1 = new Properties.FontProperty ("Futura", "Roman");
			Properties.FontSizeProperty p2 = new Properties.FontSizeProperty (12.0, Properties.SizeUnits.Points);
			Properties.MarginsProperty  p3 = new Properties.MarginsProperty (15.0, 20.0, 0.0, 0.0, Properties.SizeUnits.Points, 0, 0, 0, 0, 0, Properties.ThreeState.False);
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			int end1, end2, end3;
			
			Properties.BaseProperty.SerializeToText (buffer, p1);	end1 = buffer.Length;
			Properties.BaseProperty.SerializeToText (buffer, p2);	end2 = buffer.Length;
			Properties.BaseProperty.SerializeToText (buffer, p3);	end3 = buffer.Length;
			
			Properties.BaseProperty p;
			
			Properties.FontProperty     p1x;
			Properties.FontSizeProperty p2x;
			Properties.MarginsProperty  p3x;
			
			Properties.BaseProperty.DeserializeFromText (null, buffer.ToString (),    0, end1,        out p); p1x = p as Properties.FontProperty;
			Properties.BaseProperty.DeserializeFromText (null, buffer.ToString (), end1, end2 - end1, out p); p2x = p as Properties.FontSizeProperty;
			Properties.BaseProperty.DeserializeFromText (null, buffer.ToString (), end2, end3 - end2, out p); p3x = p as Properties.MarginsProperty;
			
			Debug.Assert.IsNotNull (p1x);
			Debug.Assert.IsNotNull (p2x);
			Debug.Assert.IsNotNull (p3x);
			
			Debug.Assert.IsTrue (p1.FaceName == p1x.FaceName);
			Debug.Assert.IsTrue (p1.StyleName == p1x.StyleName);
			
			Debug.Assert.IsTrue (p2.Size == p2x.Size);
			Debug.Assert.IsTrue (p2.Units == p2x.Units);
			
			Debug.Assert.IsTrue (p3.EnableHyphenation == p3x.EnableHyphenation);
		}
		
		private static void Ex1()
		{
			Properties.FontSizeProperty a = new Properties.FontSizeProperty (80.0, Properties.SizeUnits.Percent);
			Properties.FontSizeProperty b = new Properties.FontSizeProperty (-5.0, Properties.SizeUnits.DeltaPoints);
			
			a.GetCombination (b);
		}
	}
}

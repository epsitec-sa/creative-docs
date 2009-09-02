//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Tests
{
	/// <summary>
	/// Summary description for CheckStretchProfile.
	/// </summary>
	public sealed class CheckStretchProfile
	{
		public static void RunTests()
		{
			Layout.StretchProfile profile = new Layout.StretchProfile ();
			
			double p1;
			double p2;
			double p3;
			double p4;
			double p5;
			double p6;
			
			profile.Add (Unicode.StretchClass.NoStretch, 10.0);
			profile.Add (Unicode.StretchClass.Character, 90.0);
			profile.Add (Unicode.StretchClass.Space,     10.0);
			
			p1 = profile.ComputePenalty (110, 10, 2);
			p2 = profile.ComputePenalty (120, 10, 2);
			p3 = profile.ComputePenalty (150, 10, 2);
			p4 = profile.ComputePenalty (109, 10, 2);
			p5 = profile.ComputePenalty (108, 10, 2);
			p6 = profile.ComputePenalty (107, 10, 2);
			
			profile.Clear ();
			profile.Add (Unicode.StretchClass.Character, 100.0);
			profile.Add (Unicode.StretchClass.Space,      10.0);
			
			p1 = profile.ComputePenalty (110, 10, 2);
			p2 = profile.ComputePenalty (120, 10, 2);
			p3 = profile.ComputePenalty (150, 10, 2);
			p4 = profile.ComputePenalty (109, 10, 2);
			p5 = profile.ComputePenalty (108, 10, 2);
			p6 = profile.ComputePenalty (107, 10, 2);
			
			profile.Clear ();
			profile.Add (Unicode.StretchClass.Character, 90.0);
			profile.Add (Unicode.StretchClass.Space,     10.0);
			profile.Add (Unicode.StretchClass.Kashida,   10.0);
			
			p1 = profile.ComputePenalty (110, 10, 2);
			p2 = profile.ComputePenalty (120, 10, 2);
			p3 = profile.ComputePenalty (150, 10, 2);
			p4 = profile.ComputePenalty (109, 10, 2);
			p5 = profile.ComputePenalty (108, 10, 2);
			p6 = profile.ComputePenalty (107, 10, 2);
			
			double[] widths = new double[] { 10, 20, 30, 20, 10, 5, 5, 2, 8 };
			double[] scales = new double[widths.Length];
			Unicode.StretchClass[] stretch = new Unicode.StretchClass[] { Unicode.StretchClass.Character, Unicode.StretchClass.Character, Unicode.StretchClass.Character, Unicode.StretchClass.Character, Unicode.StretchClass.Character, Unicode.StretchClass.Space, Unicode.StretchClass.Space, Unicode.StretchClass.Kashida, Unicode.StretchClass.Kashida };
			
			Layout.StretchProfile.Scales classScales;
			profile.ComputeScales (120, out classScales);
			
			Debug.Assert.IsTrue (classScales.ScaleCharacter > 1.0009);
			Debug.Assert.IsTrue (classScales.ScaleKashida   > 1.9017);
			Debug.Assert.IsTrue (classScales.ScaleSpace     > 1.0901);
			Debug.Assert.IsTrue (classScales.ScaleCharacter < 1.0010);
			Debug.Assert.IsTrue (classScales.ScaleKashida   < 1.9018);
			Debug.Assert.IsTrue (classScales.ScaleSpace     < 1.0902);
			Debug.Assert.IsTrue (classScales.ScaleNoStretch == 1.0);
			
			profile.ComputeScales (150, out classScales);
			
			Debug.Assert.IsTrue (classScales.ScaleCharacter > 1.0036);
			Debug.Assert.IsTrue (classScales.ScaleKashida   > 4.6068);
			Debug.Assert.IsTrue (classScales.ScaleSpace     > 1.3606);
			Debug.Assert.IsTrue (classScales.ScaleCharacter < 1.0037);
			Debug.Assert.IsTrue (classScales.ScaleKashida   < 4.6069);
			Debug.Assert.IsTrue (classScales.ScaleSpace     < 1.3607);
			Debug.Assert.IsTrue (classScales.ScaleNoStretch == 1.0);
			
			profile.ComputeScales (108, out classScales);
			
			Debug.Assert.IsTrue (classScales.ScaleCharacter > 0.9999);
			Debug.Assert.IsTrue (classScales.ScaleKashida   > 0.9000);
			Debug.Assert.IsTrue (classScales.ScaleSpace     > 0.9000);
			Debug.Assert.IsTrue (classScales.ScaleCharacter < 1.0000);
			Debug.Assert.IsTrue (classScales.ScaleKashida   < 0.9001);
			Debug.Assert.IsTrue (classScales.ScaleSpace     < 0.9001);
			Debug.Assert.IsTrue (classScales.ScaleNoStretch == 1.0);
		}
	}
}

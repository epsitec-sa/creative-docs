//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
			Internal.StretchProfile profile = new Internal.StretchProfile ();
			
			double p1;
			double p2;
			double p3;
			double p4;
			double p5;
			double p6;
			
			profile.Add (Unicode.StretchClass.No,        10.0);
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
			double delta;
			
			delta  = profile.AdjustWidths (120, widths, stretch, scales);
			
			Debug.Assert.IsTrue ((int)(delta*1000+0.5) == 10*1000);
			
			delta  = profile.AdjustWidths (150, widths, stretch, scales);
			
			Debug.Assert.IsTrue ((int)(delta*1000+0.5) == 40*1000);
			
			delta  = profile.AdjustWidths (108, widths, stretch, scales);
			
			Debug.Assert.IsTrue ((int)(delta*1000-0.5) == -2*1000);
		}
	}
}

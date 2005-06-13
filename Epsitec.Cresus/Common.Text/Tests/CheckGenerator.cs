//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Tests
{
	/// <summary>
	/// Summary description for CheckUnicode.
	/// </summary>
	public sealed class CheckGenerator
	{
		public static void RunTests()
		{
			Generator generator = new Generator ();
			
			Generator.Sequence s_num = Generator.CreateSequence (Generator.SequenceType.Numeric);
			Generator.Sequence s_alf = Generator.CreateSequence (Generator.SequenceType.Alphabetic);
			Generator.Sequence s_ALF = Generator.CreateSequence (Generator.SequenceType.Alphabetic);
			
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			s_num.Suffix = ".";
			s_alf.Suffix = ")";
			s_ALF.Prefix = "<";
			s_ALF.Casing = Generator.Casing.Upper;
			s_ALF.Suffix = ">";
			
			s_num.GenerateText (1, System.Globalization.CultureInfo.CurrentCulture, buffer);
			s_num.GenerateText (10, System.Globalization.CultureInfo.CurrentCulture, buffer);
			s_alf.GenerateText (3, System.Globalization.CultureInfo.CurrentCulture, buffer);
			s_ALF.GenerateText (1, System.Globalization.CultureInfo.CurrentCulture, buffer);
			
			Debug.Assert.IsTrue (buffer.ToString () == "1.10.c)<A>");
			
			generator.Add (s_num);
			generator.Add (s_num);
			generator.Add (s_alf);
			generator.Add (s_num);
			
			int[] ranks = new int[] { 1, 10, 3, 2, 3, 4 };
			
			Debug.Assert.IsTrue ("1.10.c)2.3.4." == generator.GenerateText (ranks, System.Globalization.CultureInfo.CurrentCulture));
		}
	}
}

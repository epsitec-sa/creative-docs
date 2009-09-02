//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Internal.Sequences
{
	/// <summary>
	/// La classe Roman produit des numéros i, ii, iii, iv, etc.
	/// </summary>
	public class Roman : Generator.Sequence
	{
		public Roman()
		{
		}
		
		
		public override Generator.SequenceType	WellKnownType
		{
			get
			{
				return Generator.SequenceType.Roman;
			}
		}
		
		
		public override bool ParseText(string text, out int value)
		{
			if ((text == null) ||
				(text.Length == 0))
			{
				value = 0;
				return false;
			}
			
			text = text.ToUpper ();
			
			if (Roman.inverseLookup.Count == 0)
			{
				for (int i = 1; i < 1000; i++)
				{
					Roman.inverseLookup[this.GetRawText (i, System.Globalization.CultureInfo.InvariantCulture)] = i;
				}
			}
			
			if (Roman.inverseLookup.Contains (text))
			{
				value = (int) Roman.inverseLookup[text];
				return true;
			}
			
			value = 0;
			return false;
		}
		
		
		protected override string GetRawText(int rank, System.Globalization.CultureInfo culture)
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			int value = rank;
			
			Roman.Generate (buffer, ref value, 1000, 'M');
			Roman.Generate (buffer, ref value, 500, 'D');
			Roman.Generate (buffer, ref value, 100, 'C');
			Roman.Generate (buffer, ref value, 50, 'L');
			Roman.Generate (buffer, ref value, 10, 'X');
			Roman.Generate (buffer, ref value, 5, 'V');
			Roman.Generate (buffer, ref value, 1, 'I');
			
			string roman = buffer.ToString ();
			
			roman = roman.Replace ("IIII", "IV");
			roman = roman.Replace ("VIV", "IX");
			roman = roman.Replace ("XXXX", "XL");
			roman = roman.Replace ("LXL", "XC");
			roman = roman.Replace ("CCCC", "CD");
			roman = roman.Replace ("DCD", "CM");
			
			return roman;
		}
		
		private static void Generate(System.Text.StringBuilder buffer, ref int value, int magnitude, char letter)
		{
			while (value >= magnitude)
			{
				value -= magnitude;
				buffer.Append (letter);
			}
		}
		
		
		static System.Collections.Hashtable		inverseLookup = new System.Collections.Hashtable ();
	}
}

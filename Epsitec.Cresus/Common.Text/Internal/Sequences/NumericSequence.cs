//	Copyright � 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Internal.Sequences
{
	/// <summary>
	/// La classe Numeric produit des num�ros 1, 2, 3 ...
	/// </summary>
	public class Numeric : Generator.Sequence
	{
		public Numeric()
		{
		}
		
		
		public override Generator.SequenceType	WellKnownType
		{
			get
			{
				return Generator.SequenceType.Numeric;
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
			
			if (Types.InvariantConverter.SafeConvert (text, out value))
			{
				return true;
			}
			
			return false;
		}
		
		
		protected override string GetRawText(int rank, System.Globalization.CultureInfo culture)
		{
			return string.Format (culture, "{0}", rank);
		}
	}
}

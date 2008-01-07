//	Copyright � 2006-2008, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Internal.Sequences
{
	/// <summary>
	/// La classe Empty produit des s�quences vides.
	/// </summary>
	public class Empty : Generator.Sequence
	{
		public Empty()
		{
		}
		
		
		public override Generator.SequenceType	WellKnownType
		{
			get
			{
				return Generator.SequenceType.Empty;
			}
		}
		
		
		public override bool ParseText(string text, out int value)
		{
			value = 1;
			return true;
		}
		
		
		protected override string GetRawText(int rank, System.Globalization.CultureInfo culture)
		{
			return "";
		}
	}
}

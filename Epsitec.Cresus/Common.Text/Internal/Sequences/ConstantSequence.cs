//	Copyright � 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Internal.Sequences
{
	/// <summary>
	/// La classe Constant produit des s�quences de puces pr�d�finies. Chaque
	/// niveau poss�de sa puce.
	/// </summary>
	public class Constant : Generator.Sequence
	{
		public Constant() : this ("\u25CF")
		{
		}
		
		public Constant(string constant)
		{
			this.constant = constant;
		}
		
		
		public override Generator.SequenceType	WellKnownType
		{
			get
			{
				return Generator.SequenceType.Constant;
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
			
			value = this.constant.IndexOf (text[0]) + 1;
			
			return (value > 0);
		}
		
		
		protected override string GetRawText(int rank, System.Globalization.CultureInfo culture)
		{
			rank = System.Math.Min (rank, this.constant.Length);
			
			return this.constant.Substring (rank-1, 1);
		}
		
		protected override string GetSetupArgument()
		{
			return this.constant;
		}
		
		protected override void Setup(string argument)
		{
			this.constant = argument;
		}

		
		private string							constant;
	}
}

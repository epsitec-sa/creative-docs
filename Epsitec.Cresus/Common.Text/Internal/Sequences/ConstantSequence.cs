//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Internal.Sequences
{
	/// <summary>
	/// La classe Constant produit  ...
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

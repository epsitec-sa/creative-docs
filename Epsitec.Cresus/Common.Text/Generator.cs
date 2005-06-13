//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe Generator gère les générateurs de texte automatique.
	/// </summary>
	public class Generator
	{
		public Generator()
		{
		}
		
		
		public abstract class Sequence
		{
			public Sequence()
			{
			}
			
			
			public string						Prefix
			{
				get
				{
					return this.prefix;
				}
				set
				{
					this.prefix = value;
				}
			}
			
			public string						Suffix
			{
				get
				{
					return this.suffix;
				}
				set
				{
					this.suffix = value;
				}
			}
			
			public Casing						Casing
			{
				get
				{
					return this.casing;
				}
				set
				{
					this.casing = value;
				}
			}
			
			
			public string GetText(int rank, System.Globalization.CultureInfo culture)
			{
				System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
				
				if (this.prefix != null)
				{
					buffer.Append (this.prefix);
				}
				
				string text = this.GetRawText (rank, culture);
				
				switch (this.casing)
				{
					case Casing.Default:
						buffer.Append (text);
						break;
					
					case Casing.Lower:
						buffer.Append (text.ToLower (culture));
						break;
					
					case Casing.Upper:
						buffer.Append (text.ToUpper (culture));
						break;
				}
				
				if (this.suffix != null)
				{
					buffer.Append (this.suffix);
				}
				
				return buffer.ToString ();
			}
			
			
			protected abstract string GetRawText(int rank, System.Globalization.CultureInfo culture);
			
			private string						prefix;
			private string						suffix;
			private Casing						casing;
		}
		
		public enum Casing
		{
			Default,
			Lower,
			Upper
		}
	}
}

//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Layout
{
	/// <summary>
	/// La classe StretchProfile permet de compter les caractères selon leur
	/// classe de flexibilité et d'accumuler la place occupée par chaque type
	/// de caractère.
	/// </summary>
	public class StretchProfile
	{
		public StretchProfile()
		{
		}
		
		public StretchProfile(StretchProfile profile)
		{
			this.countNoStretch = profile.countNoStretch;
			this.countCharacter  = profile.countCharacter;
			this.countSpace      = profile.countSpace;
			this.countKashida    = profile.countKashida;
			
			this.widthNoStretch = profile.widthNoStretch;
			this.widthCharacter  = profile.widthCharacter;
			this.widthSpace      = profile.widthSpace;
			this.widthKashida    = profile.widthKashida;
			
			this.countEndSpace   = profile.countEndSpace;
			this.countEndCSpace = profile.countEndCSpace;
			this.widthEndSpace   = profile.widthEndSpace;
			this.widthEndCSpace = profile.widthEndCSpace;
		}
		
		
		public short							CountNoStretch
		{
			get
			{
				return this.countNoStretch;
			}
		}
		
		public short							CountCharacter
		{
			get
			{
				return this.countCharacter;
			}
		}
		
		public short							CountSpace
		{
			get
			{
				return this.countSpace;
			}
		}
		
		public short							CountEndSpace
		{
			get
			{
				return (short)(this.countEndSpace + this.countEndCSpace);
			}
		}
		
		public short							CountKashida
		{
			get
			{
				return this.countKashida;
			}
		}
		
		
		public double							WidthNoStretch
		{
			get
			{
				return this.widthNoStretch;
			}
		}
		
		public double							WidthCharacter
		{
			get
			{
				return this.widthCharacter;
			}
		}
		
		public double							WidthSpace
		{
			get
			{
				return this.widthSpace;
			}
		}
		
		public double							WidthEndSpace
		{
			get
			{
				return this.widthEndSpace + this.widthEndCSpace;
			}
		}
		
		public double							WidthKashida
		{
			get
			{
				return this.widthKashida;
			}
		}
		
		
		public int								TotalCount
		{
			get
			{
				return this.CountNoStretch + this.CountCharacter + this.CountSpace + this.CountKashida;
			}
		}
		
		public double							TotalWidth
		{
			get
			{
				return this.WidthNoStretch + this.WidthCharacter + this.WidthSpace + this.WidthKashida;
			}
		}
		
		
		public void Clear()
		{
			this.countNoStretch = 0;
			this.countCharacter  = 0;
			this.countSpace      = 0;
			this.countKashida    = 0;
			
			this.widthNoStretch = 0;
			this.widthCharacter  = 0;
			this.widthSpace      = 0;
			this.widthKashida    = 0;
			
			this.countEndSpace   = 0;
			this.countEndCSpace = 0;
			this.widthEndSpace   = 0;
			this.widthEndCSpace = 0;
		}
		
		public void IncludeEndSpace()
		{
			this.countSpace += this.countEndSpace;
			this.widthSpace += this.widthEndSpace;
			
			this.countCharacter += this.countEndCSpace;
			this.widthCharacter += this.widthEndCSpace;
			
			this.countEndSpace   = 0;
			this.countEndCSpace = 0;
			this.widthEndSpace   = 0;
			this.widthEndCSpace = 0;
		}
		
		
		public void Add(StretchProfile profile)
		{
			if ((profile.countNoStretch > 0) ||
				(profile.countCharacter > 0) ||
				(profile.countKashida > 0))
			{
				this.countSpace += this.countEndSpace;
				this.widthSpace += this.widthEndSpace;
				
				this.countCharacter += this.countEndCSpace;
				this.widthCharacter += this.widthEndCSpace;
				
				this.countEndSpace   = 0;
				this.countEndCSpace = 0;
				this.widthEndSpace   = 0;
				this.widthEndCSpace = 0;
				
				this.countNoStretch += profile.countNoStretch;
				this.countCharacter  += profile.countCharacter;
				this.countSpace      += profile.countSpace;
				this.countKashida    += profile.countKashida;
				
				this.widthNoStretch += profile.widthNoStretch;
				this.widthCharacter  += profile.widthCharacter;
				this.widthSpace      += profile.widthSpace;
				this.widthKashida    += profile.widthKashida;
			}
			else
			{
				this.countEndSpace   += profile.countSpace;
				this.countEndCSpace += profile.countEndCSpace;
				this.widthEndSpace   += profile.widthSpace;
				this.widthEndCSpace += profile.widthEndCSpace;
			}
		}
		
		public void Add(Unicode.StretchClass stretch, double width)
		{
			if ((stretch != Unicode.StretchClass.Space) &&
				(stretch != Unicode.StretchClass.CharacterSpace) &&
				(this.countEndSpace + this.countEndCSpace > 0))
			{
				this.countSpace     += this.countEndSpace;
				this.countCharacter += this.countEndCSpace;
				this.widthSpace     += this.widthEndSpace;
				this.widthCharacter += this.widthEndCSpace;
				
				this.countEndSpace   = 0;
				this.countEndCSpace = 0;
				this.widthEndSpace   = 0;
				this.widthEndCSpace = 0;
			}
			
			switch (stretch)
			{
				case Unicode.StretchClass.NoStretch:
					this.countNoStretch += 1;
					this.widthNoStretch += width;
					break;
				
				case Unicode.StretchClass.Character:
					this.countCharacter += 1;
					this.widthCharacter += width;
					break;
				
				case Unicode.StretchClass.CharacterSpace:
					this.countEndCSpace += 1;
					this.widthEndCSpace += width;
					break;
				
				case Unicode.StretchClass.Space:
					this.countEndSpace += 1;
					this.widthEndSpace += width;
					break;
				
				case Unicode.StretchClass.Kashida:
					this.countKashida += 1;
					this.widthKashida += width;
					break;
				
				default:
					throw new System.ArgumentOutOfRangeException ("stretch", stretch, "Invalid StretchClass provided.");
			}
		}
		
		public void Add(Layout.Context context, OpenType.Font font, double size, ulong[] text, int offset, int length, double glue)
		{
			ushort[] glyphs;
			byte[]   attributes = new byte[length];
			
			for (int i = 0; i < length; i++)
			{
				attributes[i] = (byte) Unicode.BreakAnalyzer.GetStretchClass (Unicode.Bits.GetCode (text[i]));
			}
			
			if (BaseEngine.GenerateGlyphs (context, font, text, offset, length, out glyphs, ref attributes))
			{
				throw new System.InvalidOperationException (string.Format ("Special glyph required, not in font '{0}'", font.FontIdentity.FullName));
			}
			
			int      count = glyphs.Length;
			double[] xPos = new double[count];
			double   xEnd = font.GetPositions (glyphs, size, 0, xPos);
			
			for (int i = 0; i < count-1; i++)
			{
				if (glyphs[i] != 0xffff)
				{
					this.Add ((Unicode.StretchClass) attributes[i], xPos[i+1] - xPos[i] + glue);
				}
			}
			
			if (glyphs[count-1] != 0xffff)
			{
				this.Add ((Unicode.StretchClass) attributes[count-1], xEnd - xPos[count-1] + glue);
			}
		}
		
		public void Add(OpenType.Font font, double size, ushort[] glyphs, byte[] stretchClassAttributes, double glue)
		{
			int      count = glyphs.Length;
			double[] xPos = new double[count];
			double   xEnd = font.GetPositions (glyphs, size, 0, xPos);
			
			for (int i = 0; i < count-1; i++)
			{
				if (glyphs[i] != 0xffff)
				{
					this.Add ((Unicode.StretchClass) stretchClassAttributes[i], xPos[i+1] - xPos[i] + glue);
				}
			}
			
			if (glyphs[count-1] != 0xffff)
			{
				this.Add ((Unicode.StretchClass) stretchClassAttributes[count-1], xEnd - xPos[count-1] + glue);
			}
		}
		
		
		public double ComputePenalty(double width, double fenceBefore, double fenceAfter)
		{
			//	Calcule la pénalité pour la ligne qui doit "tenir" dans l'espace
			//	disponible (width = largeur disponible, avec les frontières avant
			//	et après pouvant servir à modifier les coefficients).
			
			//	Une pénalité nulle est attribuée à une ligne qui occupe parfaitement
			//	l'espace disponible. Une pénalité maximale est attribuée à une ligne
			//	qui ne "tient" pas dans l'espace donné.
			
			//	NB: le calcul de la pénalité ne tient pas compte des espaces qui
			//		débordent en fin de texte.
			
			double totalWidth = this.TotalWidth;
			
			if (totalWidth == width)
			{
				return 0;
			}
			
			if ((totalWidth == 0) ||
				(totalWidth > width + fenceAfter))
			{
				return StretchProfile.MaxPenalty;
			}
			
			double εNoStretch;
			double εCharacter;
			double εSpace;
			double εKashida;
			
			double deltaWidth;
			
			//	L'extensibilité et la compressibilité des divers caractères utilise
			//	des coefficients différents :
			
			if (totalWidth > width)
			{
				deltaWidth = totalWidth - width;
				
				εNoStretch  = this.widthNoStretch / StretchProfile.κNoStretchCompress;
				εCharacter  = this.widthCharacter  / StretchProfile.κCharacterCompress;
				εSpace      = this.widthSpace      / StretchProfile.κSpaceCompress;
				εKashida    = this.widthKashida    / StretchProfile.κKashidaCompress;
			}
			else
			{
				deltaWidth  = width - totalWidth;
				
				εNoStretch  = this.widthNoStretch / StretchProfile.κNoStretchExtend;
				εCharacter  = this.widthCharacter  / StretchProfile.κCharacterExtend;
				εSpace      = this.widthSpace      / StretchProfile.κSpaceExtend;
				εKashida    = this.widthKashida    / StretchProfile.κKashidaExtend;
			}
			
			double total_ε = εNoStretch + εCharacter + εSpace + εKashida;
			double total_κ = 1 / total_ε;
			
			return System.Math.Min (total_κ * deltaWidth, StretchProfile.MaxPenalty);
		}
		
		public double ComputeScales(double width, out Scales scales)
		{
			//	Détermine l'échelle à utiliser pour ajuster chaque glyphe contenu
			//	dans la ligne afin d'obtenir une justification optimale; se base
			//	sur les classes des caractères.
			
			//	NB: le calcul de l'échelle ne tient pas compte des espaces qui
			//		débordent en fin de texte.
			
			double totalWidth = this.TotalWidth;
			
			scales = new Scales ();
			
			if ((totalWidth == width) ||
				(totalWidth == 0))
			{
				scales.ScaleNoStretch = 1.0;
				scales.ScaleCharacter = 1.0;
				scales.ScaleSpace     = 1.0;
				scales.ScaleKashida   = 1.0;
				
				return 0.0;
			}
			
			double εNoStretch;
			double εCharacter;
			double εSpace;
			double εKashida;
			
			double deltaWidth = width - totalWidth;
			
			//	L'extensibilité et la compressibilité des divers caractères utilise
			//	des coefficients différents :
			
			if (totalWidth > width)
			{
				εNoStretch  = this.widthNoStretch / StretchProfile.κNoStretchCompress;
				εCharacter  = this.widthCharacter  / StretchProfile.κCharacterCompress;
				εSpace      = this.widthSpace      / StretchProfile.κSpaceCompress;
				εKashida    = this.widthKashida    / StretchProfile.κKashidaCompress;
			}
			else
			{
				εNoStretch  = this.widthNoStretch / StretchProfile.κNoStretchExtend;
				εCharacter  = this.widthCharacter  / StretchProfile.κCharacterExtend;
				εSpace      = this.widthSpace      / StretchProfile.κSpaceExtend;
				εKashida    = this.widthKashida    / StretchProfile.κKashidaExtend;
			}
			
			double total_ε = εNoStretch + εCharacter + εSpace + εKashida;
			double total_κ = 1 / total_ε;
			double force   = total_κ * deltaWidth;
			
			scales.ScaleNoStretch = 1 + (this.widthNoStretch == 0 ? 0 : force * εNoStretch / this.widthNoStretch);
			scales.ScaleCharacter = 1 + (this.widthCharacter == 0  ? 0 : force * εCharacter / this.widthCharacter);
			scales.ScaleSpace     = 1 + (this.widthSpace == 0      ? 0 : force * εSpace / this.widthSpace);
			scales.ScaleKashida   = 1 + (this.widthKashida == 0    ? 0 : force * εKashida / this.widthKashida);
			
			return deltaWidth;
		}
		
		
		public override string ToString()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			buffer.Append ("{");
			buffer.Append ("NoStretch=");
			buffer.Append (this.CountNoStretch);
			buffer.Append (":");
			buffer.Append (this.WidthNoStretch);
			buffer.Append ("/Character=");
			buffer.Append (this.CountCharacter);
			buffer.Append (":");
			buffer.Append (this.WidthCharacter);
			buffer.Append ("/Space=");
			buffer.Append (this.CountSpace);
			buffer.Append (":");
			buffer.Append (this.WidthSpace);
			buffer.Append ("/Kashida=");
			buffer.Append (this.CountKashida);
			buffer.Append (":");
			buffer.Append (this.WidthKashida);
			buffer.Append ("/EndSpace=");
			buffer.Append (this.CountEndSpace);
			buffer.Append (":");
			buffer.Append (this.WidthEndSpace);
			buffer.Append ("/Total=");
			buffer.Append (this.TotalCount);
			buffer.Append (":");
			buffer.Append (this.TotalWidth);
			buffer.Append ("}");
			
			return buffer.ToString ();
		}

		
		/// <summary>
		/// La structure Scales permet de définir les échelles pour un 'stretch'
		/// réparti entre les diverses classes de caractères.
		/// </summary>
		public struct Scales
		{
			public double						ScaleNoStretch;
			public double						ScaleCharacter;
			public double						ScaleSpace;
			public double						ScaleKashida;
		}
		
		
		
		private short							countNoStretch;
		private short							countCharacter;
		private short							countSpace;
		private short							countKashida;
		
		private short							countEndSpace;
		private short							countEndCSpace;
		private double							widthEndSpace;
		private double							widthEndCSpace;
		
		private double							widthNoStretch;
		private double							widthCharacter;
		private double							widthSpace;
		private double							widthKashida;
		
		private static double					κNoStretchCompress = 1000000000.0;
		private static double					κCharacterCompress  = 1000000.0;
		private static double					κSpaceCompress	  = 100.0;
		private static double					κKashidaCompress	  = 100.0;
		
		private static double					κNoStretchExtend	= 1000000000.0;
		private static double					κCharacterExtend	= 1000.0;
		private static double					κSpaceExtend		= 10.0;
		private static double					κKashidaExtend	= 1.0;
		
		public static readonly double			MaxPenalty = 1000000.0*1000000.0;
	}
}

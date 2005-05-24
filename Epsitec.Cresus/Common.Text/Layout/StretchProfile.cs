//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
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
			this.count_no_stretch = profile.count_no_stretch;
			this.count_character  = profile.count_character;
			this.count_space      = profile.count_space;
			this.count_kashida    = profile.count_kashida;
			
			this.width_no_stretch = profile.width_no_stretch;
			this.width_character  = profile.width_character;
			this.width_space      = profile.width_space;
			this.width_kashida    = profile.width_kashida;
			
			this.count_end_space  = profile.count_end_space;
			this.width_end_space  = profile.width_end_space;
		}
		
		
		public short							CountNoStretch
		{
			get
			{
				return this.count_no_stretch;
			}
		}
		
		public short							CountCharacter
		{
			get
			{
				return this.count_character;
			}
		}
		
		public short							CountSpace
		{
			get
			{
				return this.count_space;
			}
		}
		
		public short							CountEndSpace
		{
			get
			{
				return this.count_end_space;
			}
		}
		
		public short							CountKashida
		{
			get
			{
				return this.count_kashida;
			}
		}
		
		
		public double							WidthNoStretch
		{
			get
			{
				return this.width_no_stretch;
			}
		}
		
		public double							WidthCharacter
		{
			get
			{
				return this.width_character;
			}
		}
		
		public double							WidthSpace
		{
			get
			{
				return this.width_space;
			}
		}
		
		public double							WidthEndSpace
		{
			get
			{
				return this.width_end_space;
			}
		}
		
		public double							WidthKashida
		{
			get
			{
				return this.width_kashida;
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
			this.count_no_stretch = 0;
			this.count_character  = 0;
			this.count_space      = 0;
			this.count_kashida    = 0;
			
			this.width_no_stretch = 0;
			this.width_character  = 0;
			this.width_space      = 0;
			this.width_kashida    = 0;
			
			this.count_end_space  = 0;
			this.width_end_space  = 0;
		}
		
		public void IncludeEndSpace()
		{
			this.count_space += this.count_end_space;
			this.width_space += this.width_end_space;
			
			this.count_end_space = 0;
			this.width_end_space = 0;
		}
		
		
		public void Add(StretchProfile profile)
		{
			if ((profile.count_no_stretch > 0) ||
				(profile.count_character > 0) ||
				(profile.count_kashida > 0))
			{
				this.count_space += this.count_end_space;
				this.width_space += this.width_end_space;
				
				this.count_end_space = 0;
				this.width_end_space = 0;
				
				this.count_no_stretch += profile.count_no_stretch;
				this.count_character  += profile.count_character;
				this.count_space      += profile.count_space;
				this.count_kashida    += profile.count_kashida;
				
				this.width_no_stretch += profile.width_no_stretch;
				this.width_character  += profile.width_character;
				this.width_space      += profile.width_space;
				this.width_kashida    += profile.width_kashida;
			}
			else
			{
				this.count_end_space += profile.CountSpace;
				this.width_end_space += profile.WidthSpace;
			}
		}
		
		public void Add(Unicode.StretchClass stretch, double width)
		{
			if ((stretch != Unicode.StretchClass.Space) &&
				(this.count_end_space > 0))
			{
				this.count_space += this.count_end_space;
				this.width_space += this.width_end_space;
				
				this.count_end_space = 0;
				this.width_end_space = 0;
			}
			
			switch (stretch)
			{
				case Unicode.StretchClass.NoStretch:
					this.count_no_stretch += 1;
					this.width_no_stretch += width;
					break;
				
				case Unicode.StretchClass.Character:
				case Unicode.StretchClass.CharacterSpace:
					this.count_character += 1;
					this.width_character += width;
					break;
				
				case Unicode.StretchClass.Space:
					this.count_end_space += 1;
					this.width_end_space += width;
					break;
				
				case Unicode.StretchClass.Kashida:
					this.count_kashida += 1;
					this.width_kashida += width;
					break;
				
				default:
					throw new System.ArgumentOutOfRangeException ("stretch", stretch, "Invalid StretchClass provided.");
			}
		}
		
		public void Add(OpenType.Font font, double size, ulong[] text, int offset, int length)
		{
			ushort[] glyphs;
			byte[]   attributes = new byte[length];
			
			for (int i = 0; i < length; i++)
			{
				attributes[i] = (byte) Unicode.BreakAnalyzer.GetStretchClass (Unicode.Bits.GetCode (text[i]));
			}
			
			BaseEngine.GenerateGlyphs (font, text, offset, length, out glyphs, attributes);
			
			int      count = glyphs.Length;
			double[] x_pos = new double[count];
			double   x_end = font.GetPositions (glyphs, size, 0, x_pos);
			
			for (int i = 0; i < count-1; i++)
			{
				if (glyphs[i] != 0xffff)
				{
					this.Add ((Unicode.StretchClass) attributes[i], x_pos[i+1] - x_pos[i]);
				}
			}
			
			if (glyphs[count-1] != 0xffff)
			{
				this.Add ((Unicode.StretchClass) attributes[count-1], x_end - x_pos[count-1]);
			}
		}
		
		public void Add(OpenType.Font font, double size, ushort[] glyphs, byte[] stretch_class_attributes)
		{
			int      count = glyphs.Length;
			double[] x_pos = new double[count];
			double   x_end = font.GetPositions (glyphs, size, 0, x_pos);
			
			for (int i = 0; i < count-1; i++)
			{
				if (glyphs[i] != 0xffff)
				{
					this.Add ((Unicode.StretchClass) stretch_class_attributes[i], x_pos[i+1] - x_pos[i]);
				}
			}
			
			if (glyphs[count-1] != 0xffff)
			{
				this.Add ((Unicode.StretchClass) stretch_class_attributes[count-1], x_end - x_pos[count-1]);
			}
		}
		
		
		public double ComputePenalty(double width, double fence_before, double fence_after)
		{
			//	Calcule la pénalité pour la ligne qui doit "tenir" dans l'espace
			//	disponible (width = largeur disponible, avec les frontières avant
			//	et après pouvant servir à modifier les coefficients).
			
			//	Une pénalité nulle est attribuée à une ligne qui occupe parfaitement
			//	l'espace disponible. Une pénalité maximale est attribuée à une ligne
			//	qui ne "tient" pas dans l'espace donné.
			
			//	NB: le calcul de la pénalité ne tient pas compte des espaces qui
			//		débordent en fin de texte.
			
			double total_width = this.TotalWidth;
			
			if (total_width == width)
			{
				return 0;
			}
			
			if ((total_width == 0) ||
				(total_width > width + fence_after))
			{
				return StretchProfile.MaxPenalty;
			}
			
			double ε_no_stretch;
			double ε_character;
			double ε_space;
			double ε_kashida;
			
			double delta_width;
			
			//	L'extensibilité et la compressibilité des divers caractères utilise
			//	des coefficients différents :
			
			if (total_width > width)
			{
				delta_width = total_width - width;
				
				ε_no_stretch = this.width_no_stretch / StretchProfile.κ_no_stretch_compress;
				ε_character  = this.width_character  / StretchProfile.κ_character_compress;
				ε_space      = this.width_space      / StretchProfile.κ_space_compress;
				ε_kashida    = this.width_kashida    / StretchProfile.κ_kashida_compress;
			}
			else
			{
				delta_width = width - total_width;
				
				ε_no_stretch = this.width_no_stretch / StretchProfile.κ_no_stretch_extend;
				ε_character  = this.width_character  / StretchProfile.κ_character_extend;
				ε_space      = this.width_space      / StretchProfile.κ_space_extend;
				ε_kashida    = this.width_kashida    / StretchProfile.κ_kashida_extend;
			}
			
			double total_ε = ε_no_stretch + ε_character + ε_space + ε_kashida;
			double total_κ = 1 / total_ε;
			
			return System.Math.Min (total_κ * delta_width, StretchProfile.MaxPenalty);
		}
		
		public double ComputeScales(double width, out Scales scales)
		{
			//	Détermine l'échelle à utiliser pour ajuster chaque glyphe contenu
			//	dans la ligne afin d'obtenir une justification optimale; se base
			//	sur les classes des caractères.
			
			//	NB: le calcul de l'échelle ne tient pas compte des espaces qui
			//		débordent en fin de texte.
			
			double total_width = this.TotalWidth;
			
			scales = new Scales ();
			
			if ((total_width == width) ||
				(total_width == 0))
			{
				scales.ScaleNoStretch = 1.0;
				scales.ScaleCharacter = 1.0;
				scales.ScaleSpace     = 1.0;
				scales.ScaleKashida   = 1.0;
				
				return 0.0;
			}
			
			double ε_no_stretch;
			double ε_character;
			double ε_space;
			double ε_kashida;
			
			double delta_width = width - total_width;
			
			//	L'extensibilité et la compressibilité des divers caractères utilise
			//	des coefficients différents :
			
			if (total_width > width)
			{
				ε_no_stretch = this.width_no_stretch / StretchProfile.κ_no_stretch_compress;
				ε_character  = this.width_character  / StretchProfile.κ_character_compress;
				ε_space      = this.width_space      / StretchProfile.κ_space_compress;
				ε_kashida    = this.width_kashida    / StretchProfile.κ_kashida_compress;
			}
			else
			{
				ε_no_stretch = this.width_no_stretch / StretchProfile.κ_no_stretch_extend;
				ε_character  = this.width_character  / StretchProfile.κ_character_extend;
				ε_space      = this.width_space      / StretchProfile.κ_space_extend;
				ε_kashida    = this.width_kashida    / StretchProfile.κ_kashida_extend;
			}
			
			double total_ε = ε_no_stretch + ε_character + ε_space + ε_kashida;
			double total_κ = 1 / total_ε;
			double force   = total_κ * delta_width;
			
			scales.ScaleNoStretch = 1 + (this.width_no_stretch == 0 ? 0 : force * ε_no_stretch / this.width_no_stretch);
			scales.ScaleCharacter = 1 + (this.width_character == 0  ? 0 : force * ε_character / this.width_character);
			scales.ScaleSpace     = 1 + (this.width_space == 0      ? 0 : force * ε_space / this.width_space);
			scales.ScaleKashida   = 1 + (this.width_kashida == 0    ? 0 : force * ε_kashida / this.width_kashida);
			
			return delta_width;
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
		
		
		
		private short							count_no_stretch;
		private short							count_character;
		private short							count_space;
		private short							count_kashida;
		
		private short							count_end_space;
		private double							width_end_space;
		
		private double							width_no_stretch;
		private double							width_character;
		private double							width_space;
		private double							width_kashida;
		
		private static double					κ_no_stretch_compress = 1000000000.0;
		private static double					κ_character_compress  = 1000000.0;
		private static double					κ_space_compress	  = 100.0;
		private static double					κ_kashida_compress	  = 100.0;
		
		private static double					κ_no_stretch_extend	= 1000000000.0;
		private static double					κ_character_extend	= 1000.0;
		private static double					κ_space_extend		= 10.0;
		private static double					κ_kashida_extend	= 1.0;
		
		public static readonly double			MaxPenalty = 1000000.0*1000000.0;
	}
}

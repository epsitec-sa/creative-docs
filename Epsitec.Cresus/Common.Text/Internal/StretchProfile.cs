//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Internal
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
			this.count_no        = profile.count_no;
			this.count_character = profile.count_character;
			this.count_space     = profile.count_space;
			this.count_kashida   = profile.count_kashida;
			
			this.width_no        = profile.width_no;
			this.width_character = profile.width_character;
			this.width_space     = profile.width_space;
			this.width_kashida   = profile.width_kashida;
		}
		
		
		public short							CountNo
		{
			get
			{
				return this.count_no;
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
		
		public short							CountKashida
		{
			get
			{
				return this.count_kashida;
			}
		}
		
		
		public double							WidthNo
		{
			get
			{
				return this.width_no;
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
		
		public double							WidthKashida
		{
			get
			{
				return this.width_kashida;
			}
		}
		
		
		public void Clear()
		{
			this.count_no        = 0;
			this.count_character = 0;
			this.count_space     = 0;
			this.count_kashida   = 0;
			
			this.width_no        = 0;
			this.width_character = 0;
			this.width_space     = 0;
			this.width_kashida   = 0;
		}
		
		public void Add(Unicode.StretchClass stretch, double width)
		{
			switch (stretch)
			{
				case Unicode.StretchClass.No:
					this.count_no += 1;
					this.width_no += width;
					break;
				
				case Unicode.StretchClass.Character:
				case Unicode.StretchClass.CharacterSpace:
					this.count_character += 1;
					this.width_character += width;
					break;
				
				case Unicode.StretchClass.Space:
					this.count_space += 1;
					this.width_space += width;
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
			StretchProfile.Analyse (font, size, text, offset, length, this);
		}
		
		
		public double ComputePenalty(double width, double fence_before, double fence_after)
		{
			double total_width = this.WidthNo + this.WidthCharacter + this.WidthSpace + this.WidthKashida;
			
			if (total_width == width)
			{
				return 0;
			}
			
			if ((total_width == 0) ||
				(total_width > width + fence_after))
			{
				return StretchProfile.MaximumPenalty;
			}
			
			double ε_no;
			double ε_character;
			double ε_space;
			double ε_kashida;
			
			double delta_width;
			
			if (total_width > width)
			{
				delta_width   = total_width - width;
				
				ε_no        = this.width_no        / StretchProfile.κ_no_compress;
				ε_character = this.width_character / StretchProfile.κ_character_compress;
				ε_space     = this.width_space     / StretchProfile.κ_space_compress;
				ε_kashida   = this.width_kashida   / StretchProfile.κ_kashida_compress;
			}
			else
			{
				delta_width   = width - total_width;
				
				ε_no        = this.width_no        / StretchProfile.κ_no_extend;
				ε_character = this.width_character / StretchProfile.κ_character_extend;
				ε_space     = this.width_space     / StretchProfile.κ_space_extend;
				ε_kashida   = this.width_kashida   / StretchProfile.κ_kashida_extend;
			}
			
			double total_ε = ε_no + ε_character + ε_space + ε_kashida;
			double total_κ = 1 / total_ε;
			
			return total_κ * delta_width;
		}
		
		public static void Analyse(OpenType.Font font, double size, ulong[] text, int offset, int length, StretchProfile profile)
		{
			ushort[] glyphs;
			byte[]   attributes = new byte[length];
			
			for (int i = 0; i < length; i++)
			{
				attributes[i] = (byte) Unicode.BreakAnalyzer.GetStretchClass (Unicode.Bits.GetCode (text[i]));
			}
			
			font.GenerateGlyphs (text, offset, length, out glyphs, attributes);
			
			int      count = glyphs.Length;
			double[] x_pos = new double[count];
			double   x     = 0.0;
			
			font.GetPositions (glyphs, size, x, x_pos);
			
			for (int i = 0; i < count; i++)
			{
				profile.Add ((Unicode.StretchClass) attributes[i], x_pos[i] - x);
				
				x = x_pos[i];
			}
		}
		
		
		
		private short							count_no;
		private short							count_character;
		private short							count_space;
		private short							count_kashida;
		
		private double							width_no;
		private double							width_character;
		private double							width_space;
		private double							width_kashida;
		
		private static double					κ_no_compress		= 1000000000.0;
		private static double					κ_character_compress= 1000000.0;
		private static double					κ_space_compress	= 100.0;
		private static double					κ_kashida_compress	= 100.0;
		
		private static double					κ_no_extend			= 1000000000.0;
		private static double					κ_character_extend	= 1000.0;
		private static double					κ_space_extend		= 10.0;
		private static double					κ_kashida_extend	= 1.0;
		
		public static readonly double			MaximumPenalty = 1000000.0*1000000.0;
	}
}

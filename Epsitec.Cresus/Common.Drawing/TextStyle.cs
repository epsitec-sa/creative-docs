//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	public enum TextJustifMode
	{
		None,
		AllButLast,
		All,
	}
	
	/// <summary>
	/// La classe TextStyle définit le style d'un texte (paragraphe).
	/// </summary>
	public class TextStyle
	{
		public TextStyle()
		{
			this.font            = Font.DefaultFont;
			this.size            = Font.DefaultFontSize;
			
			this.color           = new Color (0, 0, 0);
			this.color_anchor    = new Color (0, 0, 1);
			this.color_wave      = new Color (1, 0, 0);
			
			this.alignment       = ContentAlignment.TopLeft;
			this.break_mode      = TextBreakMode.Split;
			this.justif_mode     = TextJustifMode.None;
			this.show_line_break = false;
			
			this.language        = "";
		}
		
		static TextStyle()
		{
			TextStyle.default_style = new TextStyle ();
			
			TextStyle.default_style.is_default_style = true;
		}
		
		
		public bool								IsDefaultStyle
		{
			get
			{
				return this.is_default_style;
			}
		}
		
		
		//	UnderlineThickness
		//	UnderlineOffset
		
		public Font								Font
		{
			get
			{
				return this.font;
			}
			set
			{
				this.CheckForDefaultStyle ();
				
				if (value != this.font)
				{
					this.font = value;
					this.OnChanged ();
				}
			}
		}
		
		public double							Size
		{
			get
			{
				return this.size;
			}
			set
			{
				this.CheckForDefaultStyle ();
				
				if (this.size != value)
				{
					this.size = value;
					this.OnChanged ();
				}
			}
		}
		
		public Color							Color
		{
			get
			{
				return this.color;
			}
			set
			{
				this.CheckForDefaultStyle ();
				
				if (this.color != value)
				{
					this.color = value;
					this.OnChanged ();
				}
			}
		}
		
		public Color							AnchorColor
		{
			get
			{
				return this.color_anchor;
			}
			set
			{
				this.CheckForDefaultStyle ();
				
				if (this.color_anchor != value)
				{
					this.color_anchor = value;
					this.OnChanged ();
				}
			}
		}
		
		public Color							WaveColor
		{
			get
			{
				return this.color_wave;
			}
			set
			{
				this.CheckForDefaultStyle ();
				
				if (this.color_wave != value)
				{
					this.color_wave = value;
					this.OnChanged ();
				}
			}
		}
		
		public ContentAlignment					Alignment
		{
			get
			{
				return this.alignment;
			}
			set
			{
				this.CheckForDefaultStyle ();
				
				if (this.alignment != value)
				{
					this.alignment = value;
					this.OnChanged ();
				}
			}
		}
		
		public TextBreakMode					BreakMode
		{
			get
			{
				return this.break_mode;
			}

			set
			{
				this.CheckForDefaultStyle ();
				
				if (this.break_mode != value)
				{
					this.break_mode = value;
					this.OnChanged ();
				}
			}
		}
		
		public TextJustifMode					JustifMode
		{
			get
			{
				return this.justif_mode;
			}
			set
			{
				this.CheckForDefaultStyle ();
				
				if (this.justif_mode != value)
				{
					this.justif_mode = value;
					this.OnChanged ();
				}
			}
		}
		
		public bool								ShowLineBreak
		{
			get
			{
				return this.show_line_break;
			}
			set
			{
				this.CheckForDefaultStyle ();
				
				if (this.show_line_break != value)
				{
					this.show_line_break = value;
					this.OnChanged ();
				}
			}
		}
		
		public string							Language
		{
			get
			{
				return this.language;
			}
			set
			{
				this.CheckForDefaultStyle ();
				
				if (this.language != value)
				{
					this.OnChanged ();
				}
			}
		}
		
		public IImageProvider					ImageProvider
		{
			get
			{
				if (this.image_provider == null)
				{
					return TextStyle.DefaultImageProvider;
				}
				
				return this.image_provider;
			}
			set
			{
				this.CheckForDefaultStyle ();
				
				if (this.image_provider != value)
				{
					this.image_provider = value;
					this.OnChanged ();
				}
			}
		}

		
		public static TextStyle					Default
		{
			get
			{
				return TextStyle.default_style;
			}
		}
		
		public static IImageProvider			DefaultImageProvider
		{
			get
			{
				return TextStyle.default_image_provider;
			}
			set
			{
				TextStyle.default_image_provider = value;
			}
		}
		
		
		protected void CheckForDefaultStyle ()
		{
			if (this.is_default_style)
			{
				throw new System.InvalidOperationException ("TextStyle.Default cannot be modified.");
			}
		}
				
		
		protected virtual void OnChanged()
		{
			if (this.Changed != null)
			{
				this.Changed (this);
			}
		}
		
		
		public event Support.EventHandler		Changed;
		
		private static TextStyle				default_style;
		private static IImageProvider			default_image_provider;
		
		private bool							is_default_style;
		
		private Font							font;
		private double							size;
		private Color							color;
		private Color							color_anchor;
		private Color							color_wave;
		private ContentAlignment				alignment;
		private TextBreakMode					break_mode;
		private TextJustifMode					justif_mode;
		private bool							show_line_break;
		private string							language;
		private IImageProvider					image_provider;
	}
}

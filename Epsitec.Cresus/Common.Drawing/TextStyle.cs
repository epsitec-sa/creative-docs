//	Copyright © 2004, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Drawing
{
	public enum TextJustifMode
	{
		None,
		
		NoLine,
		AllButLast,
		All,
	}
	
	/// <summary>
	/// La classe TextStyle définit le style d'un texte (paragraphe) qui peut dériver
	/// d'un style par défaut ou de n'importe quel autre style.
	/// </summary>
	public class TextStyle : System.ICloneable
	{
		public TextStyle() : this (null)
		{
		}
		
		public TextStyle(TextStyle parent)
		{
			if (parent == null)
			{
				parent = TextStyle.Default;
			}
			
			this.parent          = parent;
			this.font            = null;
			this.size            = 0;
			
			this.color           = Color.Empty;
			this.color_anchor    = Color.Empty;
			this.color_wave      = Color.Empty;
			
			this.alignment       = ContentAlignment.None;
			this.break_mode      = TextBreakMode.None;
			this.justif_mode     = TextJustifMode.None;
			this.show_line_break = ThreeState.None;
			
			this.language        = null;
		}
		
		private TextStyle(int i)
		{
			this.font            = Font.DefaultFont;
			this.size            = Font.DefaultFontSize;
			
			this.color           = new Color (0, 0, 0);
			this.color_anchor    = new Color (0, 0, 1);
			this.color_wave      = new Color (1, 0, 0);
			
			this.alignment       = ContentAlignment.TopLeft;
			this.break_mode      = TextBreakMode.Ellipsis | TextBreakMode.SingleLine;
			this.justif_mode     = TextJustifMode.NoLine;
			this.show_line_break = ThreeState.False;
			
			this.language        = "";
		}
		
		static TextStyle()
		{
			TextStyle.default_style = new TextStyle (0);
			TextStyle.default_style.is_default_style = true;
		}
		
		
		public bool								IsDefaultStyle
		{
			get
			{
				return this.is_default_style;
			}
		}
		
		public TextStyle						Parent
		{
			get
			{
				return this.parent;
			}
		}
		
		
		//	UnderlineThickness
		//	UnderlineOffset
		
		public Font								Font
		{
			get
			{
				return (this.font == null) ? this.parent.Font : this.font;
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
				return (this.size == 0) ? this.parent.Size : this.size;
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
				return (this.color.IsEmpty) ? this.parent.Color : this.color;
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
				return (this.color_anchor.IsEmpty) ? this.parent.AnchorColor : this.color_anchor;
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
				return (this.color_wave.IsEmpty) ? this.parent.WaveColor : this.color_wave;
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
				return (this.alignment == ContentAlignment.None) ? this.parent.Alignment : this.alignment;
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
				return (this.break_mode == TextBreakMode.None) ? this.parent.BreakMode : this.break_mode;
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
				return (this.justif_mode == TextJustifMode.None) ? this.parent.JustifMode : this.justif_mode;
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
				if (this.show_line_break == ThreeState.None)
				{
					return this.parent.ShowLineBreak;
				}
				
				switch (this.show_line_break)
				{
					case ThreeState.True:	return true;
					case ThreeState.False:	return false;
				}
				
				throw new System.InvalidOperationException ("TextStyle has invalid ShowLineBreak state.");
			}
			set
			{
				this.CheckForDefaultStyle ();
				
				ThreeState test = (value ? ThreeState.True : ThreeState.False);
				
				if (this.show_line_break != test)
				{
					this.show_line_break = test;
					this.OnChanged ();
				}
			}
		}
		
		public string							Language
		{
			get
			{
				return (this.language == null) ? this.parent.Language : this.language;
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
				return (this.image_provider == null) ? this.parent.ImageProvider : this.image_provider;
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
		
		
		#region ICloneable Members
		object System.ICloneable.Clone()
		{
			return this.Clone ();
		}
		#endregion
		
		public TextStyle Clone()
		{
			return this.CloneCopyToNewObject (this.CloneNewObject ()) as TextStyle;
		}
		
		public static void DefineDefaultColor(Drawing.Color color)
		{
			TextStyle.default_style.color = color;
		}
		
		public static void DefineDefaultImageProvider(IImageProvider provider)
		{
			TextStyle.Default.image_provider = provider;
		}
		
		
		protected void CheckForDefaultStyle ()
		{
			if (this.is_default_style)
			{
				throw new System.InvalidOperationException ("TextStyle.Default cannot be modified.");
			}
		}
				
		
		protected virtual object CloneNewObject()
		{
			return new TextStyle ();
		}
		
		protected virtual object CloneCopyToNewObject(object o)
		{
			TextStyle that = o as TextStyle;
			
			//	Copie tous les éléments en utilisant "this" comme modèle. Il y a juste le
			//	parent pour lequel nous devons faire attention.
			
			that.parent          = this.is_default_style ? TextStyle.default_style : this.parent;
			that.font            = this.font;
			that.size            = this.size;
			that.color           = this.color;
			that.color_anchor    = this.color_anchor;
			that.color_wave      = this.color_wave;
			that.alignment       = this.alignment;
			that.break_mode      = this.break_mode;
			that.justif_mode     = this.justif_mode;
			that.show_line_break = this.show_line_break;
			that.language        = this.language;
			that.image_provider  = this.image_provider;
			
			return that;
		}
		
		
		protected virtual void OnChanged()
		{
			if (this.Changed != null)
			{
				this.Changed (this);
			}
		}
		
		
		protected enum ThreeState
		{
			None,								//	état non défini
			False,
			True
		}
	
		
		public event Support.EventHandler		Changed;
		
		private static TextStyle				default_style;
		
		private bool							is_default_style;
		private TextStyle						parent;
		
		private Font							font;
		private double							size;
		private Color							color;
		private Color							color_anchor;
		private Color							color_wave;
		private ContentAlignment				alignment;
		private TextBreakMode					break_mode;
		private TextJustifMode					justif_mode;
		private ThreeState						show_line_break;
		private string							language;
		private IImageProvider					image_provider;
	}
}

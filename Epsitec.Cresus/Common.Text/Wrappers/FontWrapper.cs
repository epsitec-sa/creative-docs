//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Wrappers
{
	/// <summary>
	/// La classe FontWrapper simplifie l'accès aux réglages liés à la fonte
	/// (FontFace, FontStyle, taille, etc.)
	/// </summary>
	public class FontWrapper : AbstractWrapper
	{
		public FontWrapper()
		{
			this.active_state  = new State (this, AccessMode.ReadOnly);
			this.defined_state = new State (this, AccessMode.ReadWrite);
		}
		
		
		public State								Active
		{
			get
			{
				return this.active_state;
			}
		}
		
		public State								Defined
		{
			get
			{
				return this.defined_state;
			}
		}
		
		
		internal override void InternalSynchronise(AbstractState state, StateProperty property)
		{
			if (state == this.defined_state)
			{
				this.SynchroniseFont ();
				this.SynchroniseInvert ();
				
				this.defined_state.ClearValueFlags ();
			}
		}
		
		
		private void SynchroniseFont()
		{
			int defines = 0;
			int changes = 0;
			
			string   font_face     = null;
			string   font_style    = null;
			string[] font_features = new string[0];
			
			double font_size = double.NaN;
			
			Properties.SizeUnits units = Properties.SizeUnits.None;
			
			if (this.defined_state.IsValueFlagged (State.FontFaceProperty))
			{
				changes++;
				
				if (this.defined_state.IsFontFaceDefined)
				{
					font_face = this.defined_state.FontFace;
					defines++;
				}
			}
			
			if (this.defined_state.IsValueFlagged (State.FontStyleProperty))
			{
				changes++;
				
				if (this.defined_state.IsFontStyleDefined)
				{
					font_style = this.defined_state.FontStyle;
					defines++;
				}
			}
			
			if (this.defined_state.IsValueFlagged (State.FontSizeProperty) ||
				this.defined_state.IsValueFlagged (State.UnitsProperty))
			{
				changes++;
				
				if ((this.defined_state.IsFontSizeDefined) &&
					(this.defined_state.IsUnitsDefined))
				{
					font_size = this.defined_state.FontSize;
					units     = this.defined_state.Units;
					defines++;
				}
			}
			
			//	...
			
			if (changes > 0)
			{
				if (defines > 0)
				{
					Property p_font = new Properties.FontProperty (font_face, font_style, font_features);
					Property p_size = new Properties.FontSizeProperty (font_size, units);
					
					this.DefineMetaProperty (FontWrapper.Font, 0, p_font, p_size);
				}
				else
				{
					this.ClearMetaProperty (FontWrapper.Font);
				}
			}
		}
		
		private void SynchroniseInvert()
		{
			if (this.defined_state.IsValueFlagged (State.InvertBoldProperty))
			{
				if (this.defined_state.IsInvertBoldDefined)
				{
					if (this.defined_state.InvertBold)
					{
						Property p_font = new Properties.FontProperty (null, "!Bold", new string[0]);
						this.DefineMetaProperty (FontWrapper.InvertBold, 1, p_font);
					}
					else
					{
						this.ClearMetaProperty (FontWrapper.InvertBold);
					}
				}
			}
			
			if (this.defined_state.IsValueFlagged (State.InvertItalicProperty))
			{
				if (this.defined_state.IsInvertItalicDefined)
				{
					if (this.defined_state.InvertItalic)
					{
						Property p_font = new Properties.FontProperty (null, "!Italic", new string[0]);
						this.DefineMetaProperty (FontWrapper.InvertItalic, 1, p_font);
					}
					else
					{
						this.ClearMetaProperty (FontWrapper.InvertItalic);
					}
				}
			}
		}

		
		internal override void Update(bool active)
		{
			State state = active ? this.Active : this.Defined;
			
			System.Diagnostics.Debug.Assert (state.IsDirty == false);
			
			this.UpdateFont (state, active);
			this.UpdateInvert (state, active);
			
			state.NotifyIfDirty ();
		}
		
		
		private void UpdateFont(State state, bool active)
		{
			Properties.FontProperty     p_font;
			Properties.FontSizeProperty p_size;
			
			if (active)
			{
				p_font = this.ReadAccumulatedProperty (Properties.WellKnownType.Font) as Properties.FontProperty;
				p_size = this.ReadAccumulatedProperty (Properties.WellKnownType.FontSize) as Properties.FontSizeProperty;
			}
			else
			{
				p_font = this.ReadMetaProperty (FontWrapper.Font, Properties.WellKnownType.Font) as Properties.FontProperty;
				p_size = this.ReadMetaProperty (FontWrapper.Font, Properties.WellKnownType.FontSize) as Properties.FontSizeProperty;
			}
			
			if (p_font != null)
			{
				if (p_font.FaceName == null)
				{
					state.DefineValue (State.FontFaceProperty);
				}
				else
				{
					state.DefineValue (State.FontFaceProperty, p_font.FaceName);
				}
				
				if (p_font.StyleName == null)
				{
					state.DefineValue (State.FontStyleProperty);
				}
				else
				{
					string style_name = p_font.StyleName;
					
					System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
					
					foreach (string element in style_name.Split (' '))
					{
						if ((element.StartsWith ("!")) ||
							(element.StartsWith ("-")))
						{
							continue;
						}
						
						if (buffer.Length > 0)
						{
							buffer.Append (" ");
						}
						
						if (element.StartsWith ("+"))
						{
							buffer.Append (element.Substring (1));
						}
						else
						{
							buffer.Append (element);
						}
					}
					
					style_name = buffer.ToString ();
					style_name = OpenType.FontCollection.GetStyleHash (style_name);
					
					state.DefineValue (State.FontStyleProperty, style_name.Length == 0 ? "Regular" : style_name);
				}
			}
			else
			{
				state.DefineValue (State.FontFaceProperty);
				state.DefineValue (State.FontStyleProperty);
			}
			
			if ((p_size != null) &&
				(p_size.Units != Properties.SizeUnits.None))
			{
				state.DefineValue (State.UnitsProperty, p_size.Units);
				
				if (double.IsNaN (p_size.Size))
				{
					state.DefineValue (State.FontSizeProperty);
				}
				else
				{
					state.DefineValue (State.FontSizeProperty, p_size.Size);
				}
			}
			else
			{
				state.DefineValue (State.FontSizeProperty);
				state.DefineValue (State.UnitsProperty);
			}
		}
		
		private void UpdateInvert(State state, bool active)
		{
			if (active)
			{
				state.ClearInvertBold ();
				state.ClearInvertItalic ();
			}
			else
			{
				Properties.FontProperty p_bold   = this.ReadMetaProperty (FontWrapper.InvertBold, Properties.WellKnownType.Font) as Properties.FontProperty;
				Properties.FontProperty p_italic = this.ReadMetaProperty (FontWrapper.InvertItalic, Properties.WellKnownType.Font) as Properties.FontProperty;
				
				if (p_bold == null)
				{
					state.DefineValue (State.InvertBoldProperty, false);
				}
				else
				{
					state.DefineValue (State.InvertBoldProperty, true);
				}
				
				if (p_italic == null)
				{
					state.DefineValue (State.InvertItalicProperty, false);
				}
				else
				{
					state.DefineValue (State.InvertItalicProperty, true);
				}
			}
		}
		
		
		public class State : AbstractState
		{
			internal State(AbstractWrapper wrapper, AccessMode access) : base (wrapper, access)
			{
			}
			
			
			public string							FontFace
			{
				get
				{
					return (string) this.GetValue (State.FontFaceProperty);
				}
				set
				{
					this.SetValue (State.FontFaceProperty, value);
				}
			}
			
			public string							FontStyle
			{
				get
				{
					return (string) this.GetValue (State.FontStyleProperty);
				}
				set
				{
					this.SetValue (State.FontStyleProperty, value);
				}
			}
			
			public double							FontSize
			{
				get
				{
					return (double) this.GetValue (State.FontSizeProperty);
				}
				set
				{
					this.SetValue (State.FontSizeProperty, value);
				}
			}
			
			public Properties.SizeUnits				Units
			{
				get
				{
					return (Properties.SizeUnits) this.GetValue (State.UnitsProperty);
				}
				set
				{
					this.SetValue (State.UnitsProperty, value);
				}
			}
			
			public bool								InvertBold
			{
				get
				{
					return (bool) this.GetValue (State.InvertBoldProperty);
				}
				set
				{
					this.SetValue (State.InvertBoldProperty, value);
				}
			}
			
			public bool								InvertItalic
			{
				get
				{
					return (bool) this.GetValue (State.InvertItalicProperty);
				}
				set
				{
					this.SetValue (State.InvertItalicProperty, value);
				}
			}
			
			
			public bool								IsFontFaceDefined
			{
				get
				{
					return this.IsValueDefined (State.FontFaceProperty);
				}
			}
			
			public bool								IsFontStyleDefined
			{
				get
				{
					return this.IsValueDefined (State.FontStyleProperty);
				}
			}
			
			public bool								IsFontSizeDefined
			{
				get
				{
					return this.IsValueDefined (State.FontSizeProperty);
				}
			}
			
			public bool								IsUnitsDefined
			{
				get
				{
					return this.IsValueDefined (State.UnitsProperty);
				}
			}
			
			public bool								IsInvertBoldDefined
			{
				get
				{
					return this.IsValueDefined (State.InvertBoldProperty);
				}
			}
			
			public bool								IsInvertItalicDefined
			{
				get
				{
					return this.IsValueDefined (State.InvertItalicProperty);
				}
			}
			
			
			public void ClearFontFace()
			{
				this.ClearValue (State.FontFaceProperty);
			}
			
			public void ClearFontStyle()
			{
				this.ClearValue (State.FontStyleProperty);
			}
			
			public void ClearFontSize()
			{
				this.ClearValue (State.FontSizeProperty);
			}
			
			public void ClearUnits()
			{
				this.ClearValue (State.UnitsProperty);
			}
			
			public void ClearInvertBold()
			{
				this.ClearValue (State.InvertBoldProperty);
			}
			
			public void ClearInvertItalic()
			{
				this.ClearValue (State.InvertItalicProperty);
			}
			
			
			#region State Properties
			public static readonly StateProperty	FontFaceProperty = new StateProperty (typeof (State), "FontFace", null);
			public static readonly StateProperty	FontStyleProperty = new StateProperty (typeof (State), "FontStyle", null);
			public static readonly StateProperty	FontSizeProperty = new StateProperty (typeof (State), "FontSize", double.NaN);
			public static readonly StateProperty	UnitsProperty = new StateProperty (typeof (State), "Units", Properties.SizeUnits.None);
			public static readonly StateProperty	InvertBoldProperty = new StateProperty (typeof (State), "InvertBold", false);
			public static readonly StateProperty	InvertItalicProperty = new StateProperty (typeof (State), "InvertItalic", false);
			#endregion
		}
		
		
		private State								active_state;
		private State								defined_state;
		
		private const string						Font = "Font";
		private const string						InvertBold = "X-Bold";
		private const string						InvertItalic = "X-Italic";
	}
}

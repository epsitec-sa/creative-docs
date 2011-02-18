//	Copyright © 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text.Wrappers
{
	/// <summary>
	/// La classe TextWrapper simplifie l'accès aux réglages liés à la fonte
	/// (FontFace, FontStyle, taille, etc.) et au texte en général.
	/// </summary>
	public class TextWrapper : AbstractWrapper
	{
		public TextWrapper()
		{
			this.active_state  = new State (this, AccessMode.ReadOnly);
			this.defined_state = new State (this, AccessMode.ReadWrite);
		}
		
		
		public State								Active
		{
			//	Etat réellement utilisé dans le contexte courant, qui dépend de l'état défini
			//	directement (Defined) et des wrappers sous-jacents.
			//	On peut ainsi avoir un Defined="Arial/Regular/120%" et un Active="Arial/Bold/14.4pt"
			//	par exemple, si le style sous-jacent du paragraphe disait qu'il faut inverser le gras
			//	et que la taille est de 12pt.
			get
			{
				return this.active_state;
			}
		}
		
		public State								Defined
		{
			//	Etat défini directement dans le wrapper courant.
			get
			{
				return this.defined_state;
			}
		}
		
		
		internal override void InternalSynchronize(AbstractState state, StateProperty property)
		{
			if (state == this.defined_state)
			{
				this.SynchronizeFontFace ();
				this.SynchronizeFontFeatures ();
				this.SynchronizeFontSize ();
				this.SynchronizeFontGlue ();
				this.SynchronizeInvert ();
				this.SynchronizeXline ();
				this.SynchronizeXscript ();
				this.SynchronizeColor ();
				this.SynchronizeLanguage ();
				this.SynchronizeLink ();
				this.SynchronizeConditions ();
				this.SynchronizeUserTags ();
				
				this.defined_state.ClearValueFlags ();
			}
		}
		
		
		private void SynchronizeFontFace()
		{
			int defines = 0;
			int changes = 0;
			
			string font_face  = null;
			string font_style = null;
			
			if (this.defined_state.IsValueFlagged (State.FontFaceProperty))
			{
				changes++;
			}
			if (this.defined_state.IsFontFaceDefined)
			{
				font_face = this.defined_state.FontFace;
				defines++;
			}
			
			if (this.defined_state.IsValueFlagged (State.FontStyleProperty))
			{
				changes++;
			}
			if (this.defined_state.IsFontStyleDefined)
			{
				font_style = this.defined_state.FontStyle;
				defines++;
			}
			
			if (changes > 0)
			{
				if (defines > 0)
				{
					Property p_font = new Properties.FontProperty (font_face, font_style, null);
					
					this.DefineMetaProperty (TextWrapper.FontFace, 0, p_font);
				}
				else
				{
					this.ClearMetaProperty (TextWrapper.FontFace);
				}
			}
		}
		
		private void SynchronizeFontFeatures()
		{
			int defines = 0;
			int changes = 0;
			
			string[] font_features = new string[0];
			
			if (this.defined_state.IsValueFlagged (State.FontFeaturesProperty))
			{
				changes++;
			}
			if (this.defined_state.IsFontFeaturesDefined)
			{
				font_features = this.defined_state.FontFeatures;
				defines++;
			}
			
			if (changes > 0)
			{
				if (defines > 0)
				{
					Property p_font = new Properties.FontProperty (null, null, font_features);
					
					this.DefineMetaProperty (TextWrapper.FontFeatures, 0, p_font);
				}
				else
				{
					this.ClearMetaProperty (TextWrapper.FontFeatures);
				}
			}
		}
		
		private void SynchronizeFontSize()
		{
			int defines = 0;
			int changes = 0;
			
			double font_size = double.NaN;
			
			Properties.SizeUnits units = Properties.SizeUnits.None;
			
			if (this.defined_state.IsValueFlagged (State.FontSizeProperty) ||
				this.defined_state.IsValueFlagged (State.UnitsProperty))
			{
				changes++;
			}
			if ((this.defined_state.IsFontSizeDefined) &&
				(this.defined_state.IsUnitsDefined))
			{
				font_size = this.defined_state.FontSize;
				units     = this.defined_state.Units;
				defines++;
			}
			
			if (changes > 0)
			{
				if (defines > 0)
				{
					if (this.IsAttachedToDefaultParagraphStyle)
					{
						System.Diagnostics.Debug.Assert (units != Properties.SizeUnits.Percent);
						System.Diagnostics.Debug.Assert (units != Properties.SizeUnits.DeltaInches);
						System.Diagnostics.Debug.Assert (units != Properties.SizeUnits.DeltaMillimeters);
						System.Diagnostics.Debug.Assert (units != Properties.SizeUnits.DeltaPoints);
					}
					
					Property p_size = new Properties.FontSizeProperty (font_size, units, double.NaN);
					
					this.DefineMetaProperty (TextWrapper.FontSize, 0, p_size);
				}
				else
				{
					this.ClearMetaProperty (TextWrapper.FontSize);
				}
			}
		}
		
		private void SynchronizeFontGlue()
		{
			int defines = 0;
			int changes = 0;
			
			double font_glue = double.NaN;
			
			if (this.defined_state.IsValueFlagged (State.FontGlueProperty))
			{
				changes++;
			}
			if (this.defined_state.IsFontGlueDefined)
			{
				font_glue = this.defined_state.FontGlue / 2;
				defines++;
			}
			
			if (changes > 0)
			{
				if (defines > 0)
				{
					Property p_size = new Properties.FontSizeProperty (double.NaN, Properties.SizeUnits.None, font_glue);
					
					this.DefineMetaProperty (TextWrapper.FontGlue, 0, p_size);
				}
				else
				{
					this.ClearMetaProperty (TextWrapper.FontGlue);
				}
			}
		}
		
		
#if false
		private void SynchronizeFont()
		{
			int defines = 0;
			int changes = 0;
			
			string   font_face     = null;
			string   font_style    = null;
			string[] font_features = new string[0];
			
			double font_size = double.NaN;
			double font_glue = double.NaN;
			
			Properties.SizeUnits units = Properties.SizeUnits.None;
			
			if (this.defined_state.IsValueFlagged (State.FontFaceProperty))
			{
				changes++;
			}
			if (this.defined_state.IsFontFaceDefined)
			{
				font_face = this.defined_state.FontFace;
				defines++;
			}
			
			if (this.defined_state.IsValueFlagged (State.FontFeaturesProperty))
			{
				changes++;
			}
			if (this.defined_state.IsFontFeaturesDefined)
			{
				font_features = this.defined_state.FontFeatures;
				defines++;
			}
			
			if (this.defined_state.IsValueFlagged (State.FontStyleProperty))
			{
				changes++;
			}
			if (this.defined_state.IsFontStyleDefined)
			{
				font_style = this.defined_state.FontStyle;
				defines++;
			}
			
			if (this.defined_state.IsValueFlagged (State.FontSizeProperty) ||
				this.defined_state.IsValueFlagged (State.UnitsProperty) ||
				this.defined_state.IsValueFlagged (State.FontGlueProperty))
			{
				changes++;
			}
			if ((this.defined_state.IsFontSizeDefined) &&
				(this.defined_state.IsUnitsDefined))
			{
				font_size = this.defined_state.FontSize;
				units     = this.defined_state.Units;
				defines++;
			}
			if (this.defined_state.IsFontGlueDefined)
			{
				font_glue = this.defined_state.FontGlue / 2;
				defines++;
			}
			
			//	...
			
			if (changes > 0)
			{
				if (defines > 0)
				{
					Property p_font = new Properties.FontProperty (font_face, font_style, font_features);
					Property p_size = new Properties.FontSizeProperty (font_size, units, font_glue);
					
					this.DefineMetaProperty (TextWrapper.Font, 0, p_font, p_size);
				}
				else
				{
					this.ClearMetaProperty (TextWrapper.Font);
				}
			}
		}
#endif
		
		private void SynchronizeInvert()
		{
			if ((this.Attachment == Attachment.Text) ||
				(this.AttachedTextStyleClass == TextStyleClass.Text))
			{
				if (this.defined_state.IsValueFlagged (State.InvertBoldProperty))
				{
					if (this.defined_state.IsInvertBoldDefined)
					{
						if (this.defined_state.InvertBold)
						{
							Property p_font = new Properties.FontProperty (null, "!Bold", new string[0]);
							this.DefineMetaProperty (TextWrapper.InvertBold, 1, p_font);
						}
						else
						{
							this.ClearMetaProperty (TextWrapper.InvertBold);
						}
					}
					else
					{
						this.ClearMetaProperty (TextWrapper.InvertBold);
					}
				}
				
				if (this.defined_state.IsValueFlagged (State.InvertItalicProperty))
				{
					if (this.defined_state.IsInvertItalicDefined)
					{
						if (this.defined_state.InvertItalic)
						{
							Property p_font = new Properties.FontProperty (null, "!Italic", new string[0]);
							this.DefineMetaProperty (TextWrapper.InvertItalic, 1, p_font);
						}
						else
						{
							this.ClearMetaProperty (TextWrapper.InvertItalic);
						}
					}
					else
					{
						this.ClearMetaProperty (TextWrapper.InvertItalic);
					}
				}
			}
		}

		private void SynchronizeXline()
		{
			this.SynchronizeUnderline ();
			this.SynchronizeOverline ();
			this.SynchronizeStrikeout ();
			this.SynchronizeTextBox ();
			this.SynchronizeTextMarker ();
		}
		
		private void SynchronizeUnderline()
		{
			if (this.defined_state.IsValueFlagged (State.UnderlineProperty))
			{
				if (this.defined_state.IsUnderlineDefined)
				{
					this.DefineMetaProperty (TextWrapper.Underline, 0, this.defined_state.Underline.ToProperty ());
				}
				else
				{
					this.ClearMetaProperty (TextWrapper.Underline);
				}
			}
		}
		
		private void SynchronizeOverline()
		{
			if (this.defined_state.IsValueFlagged (State.OverlineProperty))
			{
				if (this.defined_state.IsOverlineDefined)
				{
					this.DefineMetaProperty (TextWrapper.Overline, 0, this.defined_state.Overline.ToProperty ());
				}
				else
				{
					this.ClearMetaProperty (TextWrapper.Overline);
				}
			}
		}
		
		private void SynchronizeStrikeout()
		{
			if (this.defined_state.IsValueFlagged (State.StrikeoutProperty))
			{
				if (this.defined_state.IsStrikeoutDefined)
				{
					this.DefineMetaProperty (TextWrapper.Strikeout, 0, this.defined_state.Strikeout.ToProperty ());
				}
				else
				{
					this.ClearMetaProperty (TextWrapper.Strikeout);
				}
			}
		}
		
		private void SynchronizeTextBox()
		{
			if (this.defined_state.IsValueFlagged (State.TextBoxProperty))
			{
				if (this.defined_state.IsTextBoxDefined)
				{
					this.DefineMetaProperty (TextWrapper.TextBox, 0, this.defined_state.TextBox.ToProperty ());
				}
				else
				{
					this.ClearMetaProperty (TextWrapper.TextBox);
				}
			}
		}
		
		private void SynchronizeTextMarker()
		{
			if (this.defined_state.IsValueFlagged (State.TextMarkerProperty))
			{
				if (this.defined_state.IsTextMarkerDefined)
				{
					this.DefineMetaProperty (TextWrapper.TextMarker, 0, this.defined_state.TextMarker.ToProperty ());
				}
				else
				{
					this.ClearMetaProperty (TextWrapper.TextMarker);
				}
			}
		}
		
		private void SynchronizeXscript()
		{
			if (this.defined_state.IsValueFlagged (State.XscriptProperty))
			{
				if (this.defined_state.IsXscriptDefined)
				{
					Properties.FontXscriptProperty property = this.defined_state.Xscript.ToProperty ();
					this.DefineMetaProperty (TextWrapper.Xscript, 0, property);
				}
				else
				{
					this.ClearMetaProperty (TextWrapper.Xscript);
				}
			}
		}
		
		private void SynchronizeColor()
		{
			if (this.defined_state.IsValueFlagged (State.ColorProperty))
			{
				if (this.defined_state.IsColorDefined)
				{
					this.DefineMetaProperty (TextWrapper.Color, 0, new Properties.FontColorProperty (this.defined_state.Color));
				}
				else
				{
					this.ClearMetaProperty (TextWrapper.Color);
				}
			}
		}
		
		private void SynchronizeLanguage()
		{
			if ((this.defined_state.IsValueFlagged (State.LanguageLocaleProperty)) ||
				(this.defined_state.IsValueDefined (State.LanguageHyphenationProperty)))
			{
				if ((this.defined_state.IsLanguageLocaleDefined) ||
					(this.defined_state.IsLanguageHyphenationDefined))
				{
					string locale = this.defined_state.IsLanguageLocaleDefined ? this.defined_state.LanguageLocale : null;
					double hyphen = this.defined_state.IsLanguageHyphenationDefined ? this.defined_state.LanguageHyphenation : double.NaN;
					
					this.DefineMetaProperty (TextWrapper.Language, 0, new Properties.LanguageProperty (locale, hyphen));
				}
				else
				{
					this.ClearMetaProperty (TextWrapper.Language);
				}
			}
		}
		
		private void SynchronizeLink()
		{
			if (this.defined_state.IsValueFlagged (State.LinkProperty))
			{
				if (this.defined_state.IsLinkDefined)
				{
					string link = this.defined_state.Link;
					
					this.DefineMetaProperty (TextWrapper.Link, 0, new Properties.LinkProperty (link));
				}
				else
				{
					this.ClearMetaProperty (TextWrapper.Link);
				}
			}
		}
		
		private void SynchronizeConditions()
		{
			if (this.defined_state.IsValueFlagged (State.ConditionsProperty))
			{
				string[] conditions = this.defined_state.Conditions;
				
				if ((this.defined_state.IsConditionsDefined) &&
					(conditions.Length > 0))
				{
					Properties.ConditionalProperty[] properties = new Properties.ConditionalProperty[conditions.Length];
					
					for (int i = 0; i < conditions.Length; i++)
					{
						properties[i] = new Properties.ConditionalProperty (conditions[i]);
					}
					
					this.DefineMetaProperty (TextWrapper.Conditions, 0, properties);
				}
				else
				{
					this.ClearMetaProperty (TextWrapper.Conditions);
				}
			}
		}
		
		private void SynchronizeUserTags()
		{
			if (this.defined_state.IsValueFlagged (State.UserTagsProperty))
			{
				string[] user_tags = this.defined_state.UserTags;
				
				if ((this.defined_state.IsUserTagsDefined) &&
					(user_tags.Length > 0))
				{
					Properties.UserTagProperty[] properties = new Properties.UserTagProperty[user_tags.Length];
					
					for (int i = 0; i < user_tags.Length; i++)
					{
						string tag_type = "?";
						string tag_data = user_tags[i];
						long   id       = 0;
						
						//	TODO: gérer correctement TagType et TagData
						
						properties[i] = new Properties.UserTagProperty (tag_type, tag_data, id);
					}
					
					this.DefineMetaProperty (TextWrapper.UserTags, 0, properties);
				}
				else
				{
					this.ClearMetaProperty (TextWrapper.UserTags);
				}
			}
		}
		
		
		internal override void UpdateState(bool active)
		{
			State state = active ? this.Active : this.Defined;
			
			System.Diagnostics.Debug.Assert (state.IsDirty == false);
			
			this.UpdateFontFace (state, active);
			this.UpdateFontFeatures (state, active);
			this.UpdateFontSize (state, active);
			this.UpdateFontGlue (state, active);
			this.UpdateInvert (state, active);
			this.UpdateXline (state, active);
			this.UpdateXscript (state, active);
			this.UpdateColor (state, active);
			this.UpdateLanguage (state, active);
			this.UpdateLink (state, active);
			this.UpdateConditions (state, active);
			this.UpdateUserTags (state, active);
			
			state.NotifyIfDirty ();
		}
		
		
		private void UpdateFontFace(State state, bool active)
		{
			Properties.FontProperty p_font;
			
			if (active)
			{
				p_font = this.ReadAccumulatedProperty (Properties.WellKnownType.Font) as Properties.FontProperty;
			}
			else
			{
				p_font = this.ReadMetaProperty (TextWrapper.FontFace, Properties.WellKnownType.Font) as Properties.FontProperty;
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
					
					if ((buffer.Length == 0) &&
						(this.AttachedTextStyleClass == TextStyleClass.Text))
					{
						state.DefineValue (State.FontStyleProperty);
					}
					else
					{
						state.DefineValue (State.FontStyleProperty, style_name.Length == 0 ? "Regular" : style_name);
					}
				}
			}
			else
			{
				state.DefineValue (State.FontFaceProperty);
				state.DefineValue (State.FontStyleProperty);
			}
		}
		
		private void UpdateFontFeatures(State state, bool active)
		{
			Properties.FontProperty p_font;
			
			if (active)
			{
				p_font = this.ReadAccumulatedProperty (Properties.WellKnownType.Font) as Properties.FontProperty;
			}
			else
			{
				p_font = this.ReadMetaProperty (TextWrapper.FontFeatures, Properties.WellKnownType.Font) as Properties.FontProperty;
			}
			
			if (p_font != null)
			{
				if ((p_font.Features == null) ||
					(p_font.Features.Length == 0))
				{
					state.DefineValue (State.FontFeaturesProperty);
				}
				else
				{
					state.DefineValue (State.FontFeaturesProperty, p_font.Features);
				}
			}
			else
			{
				state.DefineValue (State.FontFeaturesProperty);
			}
		}
		
		private void UpdateFontSize(State state, bool active)
		{
			Properties.FontSizeProperty p_size;
			
			if (active)
			{
				p_size = this.ReadAccumulatedProperty (Properties.WellKnownType.FontSize) as Properties.FontSizeProperty;
			}
			else
			{
				p_size = this.ReadMetaProperty (TextWrapper.FontSize, Properties.WellKnownType.FontSize) as Properties.FontSizeProperty;
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
		
		private void UpdateFontGlue(State state, bool active)
		{
			Properties.FontSizeProperty p_size;
			
			if (active)
			{
				p_size = this.ReadAccumulatedProperty (Properties.WellKnownType.FontSize) as Properties.FontSizeProperty;
			}
			else
			{
				p_size = this.ReadMetaProperty (TextWrapper.FontGlue, Properties.WellKnownType.FontSize) as Properties.FontSizeProperty;
			}
			
			if ((p_size != null) &&
				(double.IsNaN (p_size.Glue) == false))
			{
				state.DefineValue (State.FontGlueProperty, p_size.Glue * 2);
			}
			else
			{
				state.DefineValue (State.FontGlueProperty);
			}
		}
		
		
#if false
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
				p_font = this.ReadMetaProperty (TextWrapper.Font, Properties.WellKnownType.Font) as Properties.FontProperty;
				p_size = this.ReadMetaProperty (TextWrapper.Font, Properties.WellKnownType.FontSize) as Properties.FontSizeProperty;
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
				
				if ((p_font.Features == null) ||
					(p_font.Features.Length == 0))
				{
					state.DefineValue (State.FontFeaturesProperty);
				}
				else
				{
					state.DefineValue (State.FontFeaturesProperty, p_font.Features);
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
			
			if ((p_size != null) &&
				(double.IsNaN (p_size.Glue) == false))
			{
				state.DefineValue (State.FontGlueProperty, p_size.Glue * 2);
			}
			else
			{
				state.DefineValue (State.FontGlueProperty);
			}
		}
#endif		
		
		private void UpdateInvert(State state, bool active)
		{
			if (active)
			{
				state.DefineValue (State.InvertBoldProperty);
				state.DefineValue (State.InvertItalicProperty);
			}
			else
			{
				Properties.FontProperty p_bold   = this.ReadMetaProperty (TextWrapper.InvertBold, Properties.WellKnownType.Font) as Properties.FontProperty;
				Properties.FontProperty p_italic = this.ReadMetaProperty (TextWrapper.InvertItalic, Properties.WellKnownType.Font) as Properties.FontProperty;
				
				if ((p_bold == null) ||
					(p_bold.StyleName == null) ||
					(p_bold.StyleName.IndexOf ("!Bold") < 0))
				{
					state.DefineValue (State.InvertBoldProperty, false);
				}
				else
				{
					state.DefineValue (State.InvertBoldProperty, true);
				}
				
				if ((p_italic == null) ||
					(p_italic.StyleName == null) ||
					(p_italic.StyleName.IndexOf ("!Italic") < 0))
				{
					state.DefineValue (State.InvertItalicProperty, false);
				}
				else
				{
					state.DefineValue (State.InvertItalicProperty, true);
				}
			}
		}
		
		private void UpdateXline(State state, bool active)
		{
			Properties.UnderlineProperty  p_underline;
			Properties.StrikeoutProperty  p_strikeout;
			Properties.OverlineProperty   p_overline;
			Properties.TextBoxProperty    p_textbox;
			Properties.TextMarkerProperty p_textmarker;
			
			if (active)
			{
				p_underline  = this.ReadAccumulatedProperty (Properties.WellKnownType.Underline)  as Properties.UnderlineProperty;
				p_strikeout  = this.ReadAccumulatedProperty (Properties.WellKnownType.Strikeout)  as Properties.StrikeoutProperty;
				p_overline   = this.ReadAccumulatedProperty (Properties.WellKnownType.Overline)   as Properties.OverlineProperty;
				p_textbox    = this.ReadAccumulatedProperty (Properties.WellKnownType.TextBox)    as Properties.TextBoxProperty;
				p_textmarker = this.ReadAccumulatedProperty (Properties.WellKnownType.TextMarker) as Properties.TextMarkerProperty;
			}
			else
			{
				p_underline  = this.ReadMetaProperty (TextWrapper.Underline,  Properties.WellKnownType.Underline)  as Properties.UnderlineProperty;
				p_strikeout  = this.ReadMetaProperty (TextWrapper.Strikeout,  Properties.WellKnownType.Strikeout)  as Properties.StrikeoutProperty;
				p_overline   = this.ReadMetaProperty (TextWrapper.Overline,   Properties.WellKnownType.Overline)   as Properties.OverlineProperty;
				p_textbox    = this.ReadMetaProperty (TextWrapper.TextBox,    Properties.WellKnownType.TextBox)    as Properties.TextBoxProperty;
				p_textmarker = this.ReadMetaProperty (TextWrapper.TextMarker, Properties.WellKnownType.TextMarker) as Properties.TextMarkerProperty;
			}
			
			if (p_underline == null)
			{
				state.DefineValue (State.UnderlineProperty);
			}
			else
			{
				state.Underline.DefineUsingProperty (p_underline);
			}
			
			if (p_strikeout == null)
			{
				state.DefineValue (State.StrikeoutProperty);
			}
			else
			{
				state.Strikeout.DefineUsingProperty (p_strikeout);
			}
			
			if (p_overline == null)
			{
				state.DefineValue (State.OverlineProperty);
			}
			else
			{
				state.Overline.DefineUsingProperty (p_overline);
			}
			
			if (p_textbox == null)
			{
				state.DefineValue (State.TextBoxProperty);
			}
			else
			{
				state.TextBox.DefineUsingProperty (p_textbox);
			}
			
			if (p_textmarker == null)
			{
				state.DefineValue (State.TextMarkerProperty);
			}
			else
			{
				state.TextMarker.DefineUsingProperty (p_textmarker);
			}
		}
		
		private void UpdateXscript(State state, bool active)
		{
			Properties.FontXscriptProperty p_xscript;
			
			if (active)
			{
				p_xscript = this.ReadAccumulatedProperty (Properties.WellKnownType.FontXscript) as Properties.FontXscriptProperty;
			}
			else
			{
				p_xscript = this.ReadMetaProperty (TextWrapper.Xscript, Properties.WellKnownType.FontXscript) as Properties.FontXscriptProperty;
			}
			
			if (p_xscript == null)
			{
				state.DefineValue (State.XscriptProperty);
			}
			else
			{
				state.Xscript.DefineUsingProperty (p_xscript);
			}
		}
		
		private void UpdateColor(State state, bool active)
		{
			Properties.FontColorProperty p_color;
			
			if (active)
			{
				p_color = this.ReadAccumulatedProperty (Properties.WellKnownType.FontColor) as Properties.FontColorProperty;
			}
			else
			{
				p_color = this.ReadMetaProperty (TextWrapper.Color, Properties.WellKnownType.FontColor) as Properties.FontColorProperty;
			}
			
			if (p_color == null)
			{
				state.DefineValue (State.ColorProperty);
			}
			else
			{
				state.DefineValue (State.ColorProperty, p_color.TextColor);
			}
		}
		
		private void UpdateLanguage(State state, bool active)
		{
			Properties.LanguageProperty p_language;
			
			if (active)
			{
				p_language = this.ReadAccumulatedProperty (Properties.WellKnownType.Language) as Properties.LanguageProperty;
			}
			else
			{
				p_language = this.ReadMetaProperty (TextWrapper.Language, Properties.WellKnownType.Language) as Properties.LanguageProperty;
			}
			
			if (p_language == null)
			{
				state.DefineValue (State.LanguageLocaleProperty);
				state.DefineValue (State.LanguageHyphenationProperty);
			}
			else
			{
				if (p_language.Locale == null)
				{
					state.DefineValue (State.LanguageLocaleProperty);
				}
				else
				{
					state.DefineValue (State.LanguageLocaleProperty, p_language.Locale);
				}
				
				if (double.IsNaN (p_language.Hyphenation))
				{
					state.DefineValue (State.LanguageHyphenationProperty);
				}
				else
				{
					state.DefineValue (State.LanguageHyphenationProperty, p_language.Hyphenation);
				}
			}
		}
		
		private void UpdateLink(State state, bool active)
		{
			Properties.LinkProperty p_link;
			
			if (active)
			{
				p_link = this.ReadAccumulatedProperty (Properties.WellKnownType.Link) as Properties.LinkProperty;
			}
			else
			{
				p_link = this.ReadMetaProperty (TextWrapper.Link, Properties.WellKnownType.Link) as Properties.LinkProperty;
			}
			
			if (p_link == null)
			{
				state.DefineValue (State.LinkProperty);
			}
			else
			{
				state.DefineValue (State.LinkProperty, p_link.Link);
			}
		}
		
		private void UpdateConditions(State state, bool active)
		{
			Property[] p_conditions;
			
			if (active)
			{
				p_conditions = this.ReadAccumulatedProperties (Properties.WellKnownType.Conditional);
			}
			else
			{
				p_conditions = this.ReadMetaProperties (TextWrapper.Conditions, Properties.WellKnownType.Conditional);
			}
			
			if ((p_conditions == null) ||
				(p_conditions.Length == 0))
			{
				state.DefineValue (State.ConditionsProperty);
			}
			else
			{
				string[] conditions = new string[p_conditions.Length];
				
				for (int i = 0; i < p_conditions.Length; i++)
				{
					conditions[i] = (p_conditions[i] as Properties.ConditionalProperty).Condition;
				}
				
				state.DefineValue (State.ConditionsProperty, conditions);
			}
		}
		
		private void UpdateUserTags(State state, bool active)
		{
			Property[] p_user_tags;
			
			if (active)
			{
				p_user_tags = this.ReadAccumulatedProperties (Properties.WellKnownType.UserTag);
			}
			else
			{
				p_user_tags = this.ReadMetaProperties (TextWrapper.UserTags, Properties.WellKnownType.UserTag);
			}
			
			if ((p_user_tags == null) ||
				(p_user_tags.Length == 0))
			{
				state.DefineValue (State.UserTagsProperty);
			}
			else
			{
				string[] user_tags = new string[p_user_tags.Length];
				
				for (int i = 0; i < p_user_tags.Length; i++)
				{
					user_tags[i] = (p_user_tags[i] as Properties.UserTagProperty).TagData;
				}
				
				//	TODO: gérer correctement TagData et TagType
				
				state.DefineValue (State.UserTagsProperty, user_tags);
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
			
			public double							FontGlue
			{
				get
				{
					return (double) this.GetValue (State.FontGlueProperty);
				}
				set
				{
					this.SetValue (State.FontGlueProperty, value);
				}
			}
			
			public string[]							FontFeatures
			{
				get
				{
					return (string[]) this.GetValue (State.FontFeaturesProperty);
				}
				set
				{
					this.SetValue (State.FontFeaturesProperty, value);
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
			
			public string							Color
			{
				get
				{
					return (string) this.GetValue (State.ColorProperty);
				}
				set
				{
					this.SetValue (State.ColorProperty, value);
				}
			}
			
			public string							LanguageLocale
			{
				get
				{
					return (string) this.GetValue (State.LanguageLocaleProperty);
				}
				set
				{
					this.SetValue (State.LanguageLocaleProperty, value);
				}
			}
			
			public double							LanguageHyphenation
			{
				get
				{
					return (double) this.GetValue (State.LanguageHyphenationProperty);
				}
				set
				{
					this.SetValue (State.LanguageHyphenationProperty, value);
				}
			}
			
			public XscriptDefinition				Xscript
			{
				get
				{
					XscriptDefinition value = this.GetValue (State.XscriptProperty) as XscriptDefinition;
					
					return value == null ? new XscriptDefinition (this, State.XscriptProperty) : value;
				}
			}
			
			public XlineDefinition					Underline
			{
				get
				{
					XlineDefinition value = this.GetValue (State.UnderlineProperty) as XlineDefinition;
					
					return value == null ? new XlineDefinition (this, State.UnderlineProperty) : value;
				}
			}
			
			public XlineDefinition					Strikeout
			{
				get
				{
					XlineDefinition value = this.GetValue (State.StrikeoutProperty) as XlineDefinition;
					
					return value == null ? new XlineDefinition (this, State.StrikeoutProperty) : value;
				}
			}
			
			public XlineDefinition					Overline
			{
				get
				{
					XlineDefinition value = this.GetValue (State.OverlineProperty) as XlineDefinition;
					
					return value == null ? new XlineDefinition (this, State.OverlineProperty) : value;
				}
			}
			
			public XlineDefinition					TextBox
			{
				get
				{
					XlineDefinition value = this.GetValue (State.TextBoxProperty) as XlineDefinition;
					
					return value == null ? new XlineDefinition (this, State.TextBoxProperty) : value;
				}
			}
			
			public XlineDefinition					TextMarker
			{
				get
				{
					XlineDefinition value = this.GetValue (State.TextMarkerProperty) as XlineDefinition;
					
					return value == null ? new XlineDefinition (this, State.TextMarkerProperty) : value;
				}
			}
			
			public string							Link
			{
				get
				{
					return (string) this.GetValue (State.LinkProperty);
				}
				set
				{
					this.SetValue (State.LinkProperty, value);
				}
			}
			
			public string[]							Conditions
			{
				get
				{
					return (string[]) this.GetValue (State.ConditionsProperty);
				}
				set
				{
					this.SetValue (State.ConditionsProperty, value);
				}
			}
			
			public string[]							UserTags
			{
				get
				{
					return (string[]) this.GetValue (State.UserTagsProperty);
				}
				set
				{
					this.SetValue (State.UserTagsProperty, value);
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
			
			public bool								IsFontGlueDefined
			{
				get
				{
					return this.IsValueDefined (State.FontGlueProperty);
				}
			}
			
			public bool								IsFontFeaturesDefined
			{
				get
				{
					return this.IsValueDefined (State.FontFeaturesProperty);
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
			
			public bool								IsColorDefined
			{
				get
				{
					return this.IsValueDefined (State.ColorProperty);
				}
			}
			
			public bool								IsLanguageLocaleDefined
			{
				get
				{
					return this.IsValueDefined (State.LanguageLocaleProperty);
				}
			}
			
			public bool								IsLanguageHyphenationDefined
			{
				get
				{
					return this.IsValueDefined (State.LanguageHyphenationProperty);
				}
			}
			
			public bool								IsXscriptDefined
			{
				get
				{
					return this.IsValueDefined (State.XscriptProperty);
				}
			}
			
			public bool								IsUnderlineDefined
			{
				get
				{
					return this.IsValueDefined (State.UnderlineProperty);
				}
			}
			
			public bool								IsStrikeoutDefined
			{
				get
				{
					return this.IsValueDefined (State.StrikeoutProperty);
				}
			}
			
			public bool								IsOverlineDefined
			{
				get
				{
					return this.IsValueDefined (State.OverlineProperty);
				}
			}
			
			public bool								IsTextBoxDefined
			{
				get
				{
					return this.IsValueDefined (State.TextBoxProperty);
				}
			}
			
			public bool								IsTextMarkerDefined
			{
				get
				{
					return this.IsValueDefined (State.TextMarkerProperty);
				}
			}
			
			public bool								IsLinkDefined
			{
				get
				{
					return this.IsValueDefined (State.LinkProperty);
				}
			}
			
			public bool								IsConditionsDefined
			{
				get
				{
					return this.IsValueDefined (State.ConditionsProperty);
				}
			}
			
			public bool								IsUserTagsDefined
			{
				get
				{
					return this.IsValueDefined (State.UserTagsProperty);
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
			
			public void ClearFontGlue()
			{
				this.ClearValue (State.FontGlueProperty);
			}
			
			public void ClearFontFeatures()
			{
				this.ClearValue (State.FontFeaturesProperty);
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
			
			public void ClearColor()
			{
				this.ClearValue (State.ColorProperty);
			}
			
			public void ClearLanguageLocale()
			{
				this.ClearValue (State.LanguageLocaleProperty);
			}
			
			public void ClearLanguageHyphenation()
			{
				this.ClearValue (State.LanguageHyphenationProperty);
			}
			
			public void ClearXscript()
			{
				this.ClearValue (State.XscriptProperty);
			}
			
			public void ClearUnderline()
			{
				this.ClearValue (State.UnderlineProperty);
			}
			
			public void ClearStrikeout()
			{
				this.ClearValue (State.StrikeoutProperty);
			}
			
			public void ClearOverline()
			{
				this.ClearValue (State.OverlineProperty);
			}
			
			public void ClearTextBox()
			{
				this.ClearValue (State.TextBoxProperty);
			}
			
			public void ClearTextMarker()
			{
				this.ClearValue (State.TextMarkerProperty);
			}
			
			public void ClearLink()
			{
				this.ClearValue (State.LinkProperty);
			}
			
			public void ClearConditions()
			{
				this.ClearValue (State.ConditionsProperty);
			}
			
			public void ClearUserTags()
			{
				this.ClearValue (State.UserTagsProperty);
			}
			
			
			#region State Properties
			public static readonly StateProperty	FontFaceProperty = new StateProperty (typeof (State), "FontFace", null);
			public static readonly StateProperty	FontStyleProperty = new StateProperty (typeof (State), "FontStyle", null);
			public static readonly StateProperty	FontSizeProperty = new StateProperty (typeof (State), "FontSize", double.NaN);
			public static readonly StateProperty	FontGlueProperty = new StateProperty (typeof (State), "FontGlue", double.NaN);
			public static readonly StateProperty	FontFeaturesProperty = new StateProperty (typeof (State), "FontFeatures", new string[0]);
			public static readonly StateProperty	UnitsProperty = new StateProperty (typeof (State), "Units", Properties.SizeUnits.None);
			public static readonly StateProperty	InvertBoldProperty = new StateProperty (typeof (State), "InvertBold", false);
			public static readonly StateProperty	InvertItalicProperty = new StateProperty (typeof (State), "InvertItalic", false);
			public static readonly StateProperty	ColorProperty = new StateProperty (typeof (State), "Color", null);
			public static readonly StateProperty	LanguageLocaleProperty = new StateProperty (typeof (State), "LanguageLocale", null);
			public static readonly StateProperty	LanguageHyphenationProperty = new StateProperty (typeof (State), "LanguageHyphenation", 0);
			public static readonly StateProperty	XscriptProperty = new StateProperty (typeof (State), "Xscript", null);
			public static readonly StateProperty	UnderlineProperty = new StateProperty (typeof (State), "Underline", null);
			public static readonly StateProperty	StrikeoutProperty = new StateProperty (typeof (State), "Strikeout", null);
			public static readonly StateProperty	OverlineProperty = new StateProperty (typeof (State), "Overline", null);
			public static readonly StateProperty	TextBoxProperty = new StateProperty (typeof (State), "TextBox", null);
			public static readonly StateProperty	TextMarkerProperty = new StateProperty (typeof (State), "TextMarker", null);
			public static readonly StateProperty	LinkProperty = new StateProperty (typeof (State), "Link", null);
			public static readonly StateProperty	ConditionsProperty = new StateProperty (typeof (State), "Conditions", new string[0]);
			public static readonly StateProperty	UserTagsProperty = new StateProperty (typeof (State), "UserTags", new string[0]);
			#endregion
		}
		
		public class XlineDefinition
		{
			internal XlineDefinition(State host, StateProperty property)
			{
				this.host = host;
				this.property = property;
			}
			
			
			public bool								IsDisabled
			{
				get
				{
					return this.is_disabled;
				}
				set
				{
					if (this.is_disabled != value)
					{
						this.is_disabled = value;
						this.SetState ();
					}
				}
			}
			
			public bool								IsEmpty
			{
				get
				{
					return (this.draw_style == null) && (this.draw_class == null);
				}
			}
			
			
			public double							Position
			{
				get
				{
					return this.position;
				}
				set
				{
					if (this.position != value)
					{
						this.position = value;
						this.SetState ();
					}
				}
			}
			
			public Properties.SizeUnits				PositionUnits
			{
				get
				{
					return this.position_units;
				}
				set
				{
					if (this.position_units != value)
					{
						this.position_units = value;
						this.SetState ();
					}
				}
			}
			
			
			public double							Thickness
			{
				get
				{
					return this.thickness;
				}
				set
				{
					if (this.thickness != value)
					{
						this.thickness = value;
						this.SetState ();
					}
				}
			}
			
			public Properties.SizeUnits				ThicknessUnits
			{
				get
				{
					return this.thickness_units;
				}
				set
				{
					if (this.thickness_units != value)
					{
						this.thickness_units = value;
						this.SetState ();
					}
				}
			}
			
			
			public string							DrawClass
			{
				get
				{
					return this.draw_class;
				}
				set
				{
					if (this.draw_class != value)
					{
						this.draw_class = value;
						this.SetState ();
					}
				}
			}
			
			public string							DrawStyle
			{
				get
				{
					return this.draw_style;
				}
				set
				{
					if (this.draw_style != value)
					{
						this.draw_style = value;
						this.SetState ();
					}
				}
			}
			
			
			public void DefineUsingProperty(Properties.AbstractXlineProperty value)
			{
				if ((this.is_disabled != value.IsDisabled) ||
					(this.position_units != value.PositionUnits) ||
					(this.thickness_units != value.ThicknessUnits) ||
					(! NumberSupport.Equal (this.position, value.Position)) ||
					(! NumberSupport.Equal (this.thickness, value.Thickness)) ||
					(this.draw_class != value.DrawClass) ||
					(this.draw_style != value.DrawStyle))
				{
					this.is_disabled     = value.IsDisabled;
					this.position_units  = value.PositionUnits;
					this.thickness_units = value.ThicknessUnits;
					this.position        = value.Position;
					this.thickness       = value.Thickness;
					this.draw_class      = value.DrawClass;
					this.draw_style      = value.DrawStyle;
					
					this.host.DefineValue (this.property, this, true);
				}
			}
			
			public Properties.AbstractXlineProperty ToProperty()
			{
				if (this.is_disabled)
				{
					switch (this.property.Name)
					{
						case "Underline":	return Properties.UnderlineProperty.DisableOverride;
						case "Strikeout":	return Properties.StrikeoutProperty.DisableOverride;
						case "Overline":	return Properties.OverlineProperty.DisableOverride;
						case "TextBox":		return Properties.TextBoxProperty.DisableOverride;
						case "TextMarker":	return Properties.TextMarkerProperty.DisableOverride;
					}
				}
				else
				{
					switch (this.property.Name)
					{
						case "Underline":	return new Properties.UnderlineProperty (this.position, this.position_units, this.thickness, this.thickness_units, this.draw_class, this.draw_style);
						case "Strikeout":	return new Properties.StrikeoutProperty (this.position, this.position_units, this.thickness, this.thickness_units, this.draw_class, this.draw_style);
						case "Overline":	return new Properties.OverlineProperty (this.position, this.position_units, this.thickness, this.thickness_units, this.draw_class, this.draw_style);
						case "TextBox":		return new Properties.TextBoxProperty (this.position, this.position_units, this.thickness, this.thickness_units, this.draw_class, this.draw_style);
						case "TextMarker":	return new Properties.TextMarkerProperty (this.position, this.position_units, this.thickness, this.thickness_units, this.draw_class, this.draw_style);
					}
				}
				
				throw new System.NotSupportedException (string.Format ("Property {0} not supported", this.property.Name));
			}
			
			
			public bool EqualsIgnoringIsDisabled(object obj)
			{
				XlineDefinition that = obj as XlineDefinition;
				
				if (that == null)
				{
					return false;
				}
				if (this == that)
				{
					return true;
				}
				
				return this.position_units == that.position_units
					&& this.thickness_units == that.thickness_units
					&& NumberSupport.Equal (this.position, that.position)
					&& NumberSupport.Equal (this.thickness, that.thickness)
					&& this.draw_class == that.draw_class
					&& this.draw_style == that.draw_style;
			}
			
			
			public override bool Equals(object obj)
			{
				XlineDefinition that = obj as XlineDefinition;
				
				if (that == null)
				{
					return false;
				}
				if (this == that)
				{
					return true;
				}
				
				return this.is_disabled == that.is_disabled
					&& this.position_units == that.position_units
					&& this.thickness_units == that.thickness_units
					&& NumberSupport.Equal (this.position, that.position)
					&& NumberSupport.Equal (this.thickness, that.thickness)
					&& this.draw_class == that.draw_class
					&& this.draw_style == that.draw_style;
			}
			
			public override int GetHashCode()
			{
				return base.GetHashCode ();
			}

			
			private void SetState()
			{
				this.host.SetValue (this.property, this, true);
			}
			
			
			private State							host;
			private StateProperty					property;
			
			private bool							is_disabled;
			private double							position;
			private double							thickness;
			private Properties.SizeUnits			position_units;
			private Properties.SizeUnits			thickness_units;
			private string							draw_class;
			private string							draw_style;
		}
		
		public class XscriptDefinition
		{
			internal XscriptDefinition(State host, StateProperty property)
			{
				this.host = host;
				this.property = property;
			}
			
			
			public bool								IsDisabled
			{
				get
				{
					return this.is_disabled;
				}
				set
				{
					if (this.is_disabled != value)
					{
						this.is_disabled = value;
						this.SetState ();
					}
				}
			}
			
			public bool								IsEmpty
			{
				get
				{
					return double.IsNaN (this.scale) && double.IsNaN (this.offset);
				}
			}
			
			
			public double							Scale
			{
				get
				{
					return this.scale;
				}
				set
				{
					if (this.scale != value)
					{
						this.scale = value;
						this.SetState ();
					}
				}
			}
			
			public double							Offset
			{
				get
				{
					return this.offset;
				}
				set
				{
					if (this.offset != value)
					{
						this.offset = value;
						this.SetState ();
					}
				}
			}
			
			
			public void DefineUsingProperty(Properties.FontXscriptProperty value)
			{
				if ((this.is_disabled != false) ||
					(! NumberSupport.Equal (this.scale, value.Scale)) ||
					(! NumberSupport.Equal (this.offset, value.Offset)))
				{
					this.is_disabled = false;
					this.scale       = value.Scale;
					this.offset      = value.Offset;
					
					this.host.DefineValue (this.property, this, true);
				}
			}
			
			public Properties.FontXscriptProperty ToProperty()
			{
				if (this.is_disabled)
				{
					switch (this.property.Name)
					{
						case "Xscript": return Properties.FontXscriptProperty.DisableOverride;
					}
				}
				else
				{
					switch (this.property.Name)
					{
						case "Xscript": return new Properties.FontXscriptProperty (this.scale, this.offset);
					}
				}
				
				throw new System.NotSupportedException (string.Format ("Property {0} not supported", this.property.Name));
			}
			
			
			public bool EqualsIgnoringIsDisabled(object obj)
			{
				XscriptDefinition that = obj as XscriptDefinition;
				
				if (that == null)
				{
					return false;
				}
				if (this == that)
				{
					return true;
				}
				
				return NumberSupport.Equal (this.scale, that.scale)
					&& NumberSupport.Equal (this.offset, that.offset);
			}
			
			
			public override bool Equals(object obj)
			{
				XscriptDefinition that = obj as XscriptDefinition;
				
				if (that == null)
				{
					return false;
				}
				if (this == that)
				{
					return true;
				}
				
				return this.is_disabled == that.is_disabled
					&& NumberSupport.Equal (this.scale, that.scale)
					&& NumberSupport.Equal (this.offset, that.offset);
			}
			
			public override int GetHashCode()
			{
				return base.GetHashCode ();
			}

			
			private void SetState()
			{
				this.host.SetValue (this.property, this, true);
			}
			
			
			private State							host;
			private StateProperty					property;
			
			private bool							is_disabled;
			private double							scale;
			private double							offset;
		}
		
		
		private State								active_state;
		private State								defined_state;
		
//		private const string						Font			= "#Tx#Font";
		private const string						FontFace		= "#Tx#Font";
		private const string						FontFeatures	= "#Tx#FntFtr";
		private const string						FontSize		= "#Tx#FntSiz";
		private const string						FontGlue		= "#Tx#FntGlu";
		private const string						Color			= "#Tx#Color";
		private const string						Language		= "#Tx#Lang";
		private const string						Link			= "#Tx#Link";
		private const string						UserTags		= "#Tx#UsrTags";
		private const string						Conditions		= "#Tx#Conds";
		private const string						Underline		= "#Tx#Under";
		private const string						Overline		= "#Tx#Over";
		private const string						Strikeout		= "#Tx#StrOut";
		private const string						TextBox			= "#Tx#TBox";
		private const string						TextMarker		= "#Tx#TMark";
		private const string						Xscript			= "#Tx#Xscript";
		private const string						InvertBold		= "#Tx#X-Bold";
		private const string						InvertItalic	= "#Tx#X-Italic";
	}
}

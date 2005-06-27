//	Copyright © 2005, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe Context décrit un contexte (pour la désérialisation) lié à
	/// un environnement 'texte'.
	/// </summary>
	public class Context
	{
		public Context()
		{
			this.style_list     = new StyleList ();
			this.layout_list    = new LayoutList (this);
			this.generator_list = new GeneratorList (this);
			this.p_manager_list = new ParagraphManagerList (this);
			this.char_marker    = new Internal.CharMarker ();
			this.conditions     = new System.Collections.Hashtable ();
			
			this.char_marker.Add (Context.DefaultMarkers.TagSelected);
			this.char_marker.Add (Context.DefaultMarkers.TagRequiresSpellChecking);
			
			this.markers = new Context.DefaultMarkers (this.char_marker);
			 
			this.font_collection = new OpenType.FontCollection ();
			this.font_collection.Initialize ();
			
			this.font_cache = new System.Collections.Hashtable ();
			
			this.CreateDefaultLayout ();
		}
		
		
		public StyleList						StyleList
		{
			get
			{
				return this.style_list;
			}
		}
		
		public LayoutList						LayoutList
		{
			get
			{
				return this.layout_list;
			}
		}
		
		public GeneratorList					GeneratorList
		{
			get
			{
				return this.generator_list;
			}
		}
		
		public ParagraphManagerList				ParagraphManagerList
		{
			get
			{
				return this.p_manager_list;
			}
		}
		
		public System.Globalization.CultureInfo	Culture
		{
			get
			{
				return System.Globalization.CultureInfo.CurrentCulture;
			}
		}
		
		public TextStyle						DefaultStyle
		{
			get
			{
				return this.default_style;
			}
			set
			{
				this.default_style = value;
			}
		}
		
		
		public Context.DefaultMarkers			Markers
		{
			get
			{
				return this.markers;
			}
		}
		
		internal Internal.CharMarker			CharMarker
		{
			get
			{
				return this.char_marker;
			}
		}
		
		
		public int GetGlyphForSpecialCode(ulong code)
		{
			System.Diagnostics.Debug.Assert (Unicode.Bits.GetSpecialCodeFlag (code));
			
			ulong stripped_code = Internal.CharMarker.ExtractStyleAndSettings (code);
			
			Styles.SimpleStyle   style          = this.style_list[stripped_code];
			Styles.LocalSettings local_settings = style.GetLocalSettings (stripped_code);
			Styles.ExtraSettings extra_settings = style.GetExtraSettings (stripped_code);
			
			int glyph = -1;
			
			if (local_settings != null)
			{
				glyph = local_settings.GetGlyphForSpecialCode (code);
			}
			
			if ((glyph == -1) &&
				(extra_settings != null))
			{
				glyph = extra_settings.GetGlyphForSpecialCode (code);
			}
			
			return glyph;
		}
		
		public void DefineResource(string name, IGlyphRenderer renderer)
		{
			if (this.resources == null)
			{
				this.resources = new System.Collections.Hashtable ();
			}
			
			this.resources[name] = renderer;
		}
		
		public void RemoveResource(string name)
		{
			if ((this.resources != null) &&
				(this.resources.Contains (name)))
			{
				this.resources.Remove (name);
			}
		}
		
		
		public void SetCondition(string name)
		{
			this.conditions[name] = this;
		}
		
		public void ClearCondition(string name)
		{
			this.conditions.Remove (name);
		}
		
		
		public IGlyphRenderer FindResource(string name)
		{
			if (this.resources != null)
			{
				return this.resources[name] as IGlyphRenderer;
			}
			
			return null;
		}
		
		
		public void GetFont(string name, out OpenType.Font font)
		{
			if (this.get_font_cache_name == name)
			{
				font = this.get_font_cache_font;
				return;
			}
			
			OpenType.FontIdentity id = this.font_collection[name];
			
			if (id == null)
			{
				font = null;
			}
			else
			{
				this.CreateOrGetFontFromCache (id.InvariantFaceName, id.InvariantStyleName, out font);
			}
			
			this.get_font_cache_name = name;
			this.get_font_cache_font = font;
		}
		
		public void GetFont(Properties.FontProperty property, out OpenType.Font font)
		{
			this.CreateOrGetFontFromCache (property.FaceName, property.StyleName, out font);
		}
		
		public void GetFont(ulong code, out OpenType.Font font, out double font_size)
		{
			int  current_style_index   = Internal.CharMarker.GetStyleIndex (code);
			long current_style_version = this.style_list.InternalStyleTable.Version;
			
			if ((this.get_font_last_style_version == current_style_version) &&
				(this.get_font_last_style_index   == current_style_index))
			{
				font      = this.get_font_last_font;
				font_size = this.get_font_last_font_size;
				
				return;
			}
			
			Styles.SimpleStyle style = this.style_list.GetStyleFromIndex (current_style_index);
			
			Properties.FontProperty     font_p      = style[Properties.WellKnownType.Font] as Properties.FontProperty;
			Properties.FontSizeProperty font_size_p = style[Properties.WellKnownType.FontSize] as Properties.FontSizeProperty;
			
			font_size = font_size_p.SizeInPoints;
			
			this.CreateOrGetFontFromCache (font_p.FaceName, font_p.StyleName, out font);
			
			if (font_p.Features == null)
			{
				font.SelectFeatures ();
			}
			else
			{
				font.SelectFeatures (font_p.Features);
			}
			
			this.get_font_last_style_version = current_style_version;
			this.get_font_last_style_index   = current_style_index;
			this.get_font_last_font          = font;
			this.get_font_last_font_size     = font_size;
		}
		
		public void GetFontOffsets(ulong code, out double baseline_offset, out double advance_offset)
		{
			code = Internal.CharMarker.ExtractStyleAndSettings (code);
			
			long current_style_version = this.style_list.InternalStyleTable.Version;
			
			if ((this.get_font_offset_last_style_version == current_style_version) &&
				(this.get_font_offset_last_code == code))
			{
				baseline_offset = this.get_font_offset_last_baseline_offset;
				advance_offset  = this.get_font_offset_last_advance_offset;
				
				return;
			}
			
			int current_style_index = Internal.CharMarker.GetStyleIndex (code);
			
			if ((this.get_font_last_style_version != current_style_version) ||
				(this.get_font_last_style_index   != current_style_index))
			{
				//	Rafraîchit les informations sur la fonte utilisée :
				
				OpenType.Font font;
				double        font_size;
				
				this.GetFont (code, out font, out font_size);
			}
			
			Styles.SimpleStyle   style          = this.style_list.GetStyleFromIndex (current_style_index);
			Styles.LocalSettings local_settings = style.GetLocalSettings (code);
			
			advance_offset  = 0;
			baseline_offset = 0;
			
			if (local_settings != null)
			{
				Properties.FontOffsetProperty font_offset_p = local_settings[Properties.WellKnownType.FontOffset] as Properties.FontOffsetProperty;
				Properties.FontKernProperty   font_kern_p   = local_settings[Properties.WellKnownType.FontKern] as Properties.FontKernProperty;
				
				if (font_offset_p != null)
				{
					double ascender = this.get_font_last_font.GetAscender (this.get_font_last_font_size);
					baseline_offset = font_offset_p.GetOffsetInPoints (ascender);
				}
				
				if (font_kern_p != null)
				{
					double em_size = this.get_font_last_font_size;
					advance_offset = font_kern_p.GetOffsetInPoints (em_size);
				}
			}
			
			this.get_font_offset_last_style_version   = current_style_version;
			this.get_font_offset_last_code            = code;
			this.get_font_offset_last_baseline_offset = baseline_offset;
			this.get_font_offset_last_advance_offset  = advance_offset;
		}
		
		public void GetColor(ulong code, out Drawing.Color color)
		{
			code = Internal.CharMarker.ExtractStyleAndSettings (code);
			
			long current_style_version = this.style_list.InternalStyleTable.Version;
			
			if ((this.get_color_last_style_version == current_style_version) &&
				(this.get_color_last_code == code))
			{
				color = this.get_color_last_color;
				
				return;
			}
			
			Styles.SimpleStyle style = this.style_list[code];
			
			Styles.ExtraSettings extra_settings = style.GetExtraSettings (code);
			
			Properties.ColorProperty color_p = extra_settings[Properties.WellKnownType.Color] as Properties.ColorProperty;
			
			if (color_p == null)
			{
				color = Drawing.Color.Empty;
			}
			else
			{
				color = color_p.TextColor;
			}
			
			this.get_color_last_style_version = current_style_version;
			this.get_color_last_code          = code;
			this.get_color_last_color         = color;
		}
		
		public void GetLanguage(ulong code, out Properties.LanguageProperty property)
		{
			code = Internal.CharMarker.ExtractStyleAndSettings (code);
			
			long current_style_version = this.style_list.InternalStyleTable.Version;
			
			if ((this.get_language_last_style_version == current_style_version) &&
				(this.get_language_last_code == code))
			{
				property = this.get_language_last_property;
				
				return;
			}
			
			Styles.SimpleStyle   style          = this.style_list[code];
			Styles.ExtraSettings extra_settings = style.GetExtraSettings (code);
			
			property = extra_settings == null ? null : extra_settings[Properties.WellKnownType.Language] as Properties.LanguageProperty;
			
			this.get_language_last_style_version = current_style_version;
			this.get_language_last_code          = code;
			this.get_language_last_property      = property;
		}
		
		public bool GetAutoText(ulong code, out Properties.AutoTextProperty property)
		{
			code = Internal.CharMarker.ExtractStyleAndSettings (code);
			
			long current_style_version = this.style_list.InternalStyleTable.Version;
			
			if ((this.get_auto_text_last_style_version == current_style_version) &&
				(this.get_auto_text_last_code == code))
			{
				property = this.get_auto_text_last_property;
				
				return property != null;
			}
			
			Styles.SimpleStyle   style          = this.style_list[code];
			Styles.ExtraSettings extra_settings = style.GetExtraSettings (code);
			
			property = extra_settings == null ? null : extra_settings[Properties.WellKnownType.AutoText] as Properties.AutoTextProperty;
			
			this.get_auto_text_last_style_version = current_style_version;
			this.get_auto_text_last_code          = code;
			this.get_auto_text_last_property      = property;
			
			return property != null;
		}
		
		public bool GetGenerator(ulong code, out Properties.GeneratorProperty property)
		{
			code = Internal.CharMarker.ExtractStyleAndSettings (code);
			
			long current_style_version = this.style_list.InternalStyleTable.Version;
			
			if ((this.get_generator_last_style_version == current_style_version) &&
				(this.get_generator_last_code == code))
			{
				property = this.get_generator_last_property;
				
				return property != null;
			}
			
			Styles.SimpleStyle   style          = this.style_list[code];
			Styles.ExtraSettings extra_settings = style.GetExtraSettings (code);
			
			property = extra_settings == null ? null : extra_settings[Properties.WellKnownType.Generator] as Properties.GeneratorProperty;
			
			this.get_generator_last_style_version = current_style_version;
			this.get_generator_last_code          = code;
			this.get_generator_last_property      = property;
			
			return property != null;
		}
		
		public bool TestConditions(ulong code)
		{
			Properties.ConditionalProperty[] properties;
			bool summary;
			
			this.GetConditions (code, out properties, out summary);
			
			return summary;
		}
		
		public void GetConditions(ulong code, out Properties.ConditionalProperty[] properties, out bool summary)
		{
			int  current_style_index   = Internal.CharMarker.GetStyleIndex (code);
			long current_style_version = this.style_list.InternalStyleTable.Version;
			
			if ((this.get_condition_last_style_version != current_style_version) ||
				(this.get_condition_last_style_index   != current_style_index))
			{
				Styles.SimpleStyle style = this.style_list.GetStyleFromIndex (current_style_index);
				
				Property[] props = style.FindProperties (Properties.WellKnownType.Conditional);
				
				this.get_condition_last_style_version = current_style_version;
				this.get_condition_last_style_index   = current_style_index;
				this.get_condition_last_properties    = props;
				
				bool ok = true;
				
				foreach (Properties.ConditionalProperty condition_p in props)
				{
					bool is_true = this.conditions.Contains (condition_p.Condition);
					bool show_if = condition_p.ShowIfTrue;
					
					if ((show_if && !is_true) ||
						(!show_if && is_true))
					{
						ok = false;
						break;
					}
				}
				
				this.get_condition_last_summary = ok;
			}
			
			if (this.get_condition_last_properties.Length > 0)
			{
				summary    = this.get_condition_last_summary;
				properties = new Properties.ConditionalProperty[this.get_condition_last_properties.Length];
				
				this.get_condition_last_properties.CopyTo (properties, 0);
			}
			else
			{
				summary    = true;
				properties = null;
			}
		}
		
		public void GetProperties(ulong code, out Property[] properties)
		{
			int  current_style_index   = Internal.CharMarker.GetStyleIndex (code);
			long current_style_version = this.style_list.InternalStyleTable.Version;
			
			if ((this.get_properties_last_style_version != current_style_version) ||
				(this.get_properties_last_style_index   != current_style_index))
			{
				Styles.SimpleStyle style = this.style_list.GetStyleFromIndex (current_style_index);
				
				Properties.PropertiesProperty props = style == null ? null : style[Properties.WellKnownType.Properties] as Properties.PropertiesProperty;
				
				if (props == null)
				{
					this.get_properties_last_properties = new Property[0];
				}
				else
				{
					string[] serialized = props.SerializedProperties;
					
					if (serialized.Length > 0)
					{
						this.get_properties_last_properties = Properties.PropertiesProperty.DeserializeProperties (this, serialized);
					}
					else
					{
						this.get_properties_last_properties = new Property[0];
					}
				}
				
				this.get_properties_last_style_version = current_style_version;
				this.get_properties_last_style_index   = current_style_index;
			}
			
			properties = new Property[this.get_properties_last_properties.Length];
			this.get_properties_last_properties.CopyTo (properties, 0);
		}
		
		public void GetStyles(ulong code, out TextStyle[] styles)
		{
			int  current_style_index   = Internal.CharMarker.GetStyleIndex (code);
			long current_style_version = this.style_list.InternalStyleTable.Version;
			
			if ((this.get_styles_last_style_version != current_style_version) ||
				(this.get_styles_last_style_index   != current_style_index))
			{
				Styles.SimpleStyle        style = this.style_list.GetStyleFromIndex (current_style_index);
				Properties.StylesProperty props = style == null ? null : style[Properties.WellKnownType.Styles] as Properties.StylesProperty;
				
				this.get_styles_last_styles        = (props == null) ? new TextStyle[0] : props.Styles;
				this.get_styles_last_style_version = current_style_version;
				this.get_styles_last_style_index   = current_style_index;
			}
			
			styles = new TextStyle[this.get_styles_last_styles.Length];
			this.get_styles_last_styles.CopyTo (styles, 0);
		}
		
		public void GetLeading(ulong code, out Properties.LeadingProperty property)
		{
			int  current_style_index   = Internal.CharMarker.GetStyleIndex (code);
			long current_style_version = this.style_list.InternalStyleTable.Version;
			
			if ((this.get_leading_last_style_version == current_style_version) &&
				(this.get_leading_last_style_index   == current_style_index))
			{
				property = this.get_leading_last_property;
				
				return;
			}
			
			Styles.SimpleStyle style = this.style_list.GetStyleFromIndex (current_style_index);
			
			property = style[Properties.WellKnownType.Leading] as Properties.LeadingProperty;
			
			this.get_leading_last_style_version = current_style_version;
			this.get_leading_last_style_index   = current_style_index;
			this.get_leading_last_property      = property;
		}
		
		public void GetKeep(ulong code, out Properties.KeepProperty property)
		{
			int  current_style_index   = Internal.CharMarker.GetStyleIndex (code);
			long current_style_version = this.style_list.InternalStyleTable.Version;
			
			if ((this.get_keep_last_style_version == current_style_version) &&
				(this.get_keep_last_style_index   == current_style_index))
			{
				property = this.get_keep_last_property;
				
				return;
			}
			
			Styles.SimpleStyle style = this.style_list.GetStyleFromIndex (current_style_index);
			
			property = style[Properties.WellKnownType.Keep] as Properties.KeepProperty;
			
			this.get_keep_last_style_version = current_style_version;
			this.get_keep_last_style_index   = current_style_index;
			this.get_keep_last_property      = property;
		}
		
		public void GetUnderlines(ulong code, out Properties.UnderlineProperty[] properties)
		{
			code = Internal.CharMarker.ExtractStyleAndSettings (code);
			
			long current_style_version = this.style_list.InternalStyleTable.Version;
			
			if ((this.get_underlines_last_style_version != current_style_version) ||
				(this.get_underlines_last_code != code))
			{
				Styles.SimpleStyle style = this.style_list[code];
				
				Property[]           base_props     = null;
				Styles.ExtraSettings extra_settings = style.GetExtraSettings (code);
				
				if (extra_settings != null)
				{
					base_props = extra_settings.FindProperties (Properties.WellKnownType.Underline);
					
					System.Array.Sort (base_props, Properties.UnderlineProperty.Comparer);
				}
				
				this.get_underlines_last_style_version = current_style_version;
				this.get_underlines_last_code          = code;
				this.get_underlines_last_properties    = base_props;
			}
			
			if ((this.get_underlines_last_properties != null) &&
				(this.get_underlines_last_properties.Length > 0))
			{
				int count = this.get_underlines_last_properties.Length;
				
				properties = new Properties.UnderlineProperty[count];
				this.get_underlines_last_properties.CopyTo (properties, 0);
			}
			else
			{
				properties = null;
			}
		}
		
		public void GetLinks(ulong code, out Properties.LinkProperty[] properties)
		{
			code = Internal.CharMarker.ExtractStyleAndSettings (code);
			
			long current_style_version = this.style_list.InternalStyleTable.Version;
			
			if ((this.get_links_last_style_version != current_style_version) ||
				(this.get_links_last_code != code))
			{
				Styles.SimpleStyle style = this.style_list[code];
				
				Property[]           base_props     = null;
				Styles.ExtraSettings extra_settings = style.GetExtraSettings (code);
				
				if (extra_settings != null)
				{
					base_props = extra_settings.FindProperties (Properties.WellKnownType.Link);
					
					System.Array.Sort (base_props, Properties.LinkProperty.Comparer);
				}
				
				this.get_links_last_style_version = current_style_version;
				this.get_links_last_code          = code;
				this.get_links_last_properties    = base_props;
			}
			
			if ((this.get_links_last_properties != null) &&
				(this.get_links_last_properties.Length > 0))
			{
				int count = this.get_links_last_properties.Length;
				
				properties = new Properties.LinkProperty[count];
				this.get_links_last_properties.CopyTo (properties, 0);
			}
			else
			{
				properties = null;
			}
		}
		
		public void GetMetas(ulong code, out Properties.MetaProperty[] properties)
		{
			code = Internal.CharMarker.ExtractStyleAndSettings (code);
			
			long current_style_version = this.style_list.InternalStyleTable.Version;
			
			if ((this.get_metas_last_style_version != current_style_version) ||
				(this.get_metas_last_code != code))
			{
				Styles.SimpleStyle style = this.style_list[code];
				
				Property[]           base_props     = null;
				Styles.ExtraSettings extra_settings = style.GetExtraSettings (code);
				
				if (extra_settings != null)
				{
					base_props = extra_settings.FindProperties (Properties.WellKnownType.Meta);
					
					System.Array.Sort (base_props, Properties.MetaProperty.Comparer);
				}
				
				this.get_metas_last_style_version = current_style_version;
				this.get_metas_last_code          = code;
				this.get_metas_last_properties    = base_props;
			}
			
			if ((this.get_metas_last_properties != null) &&
				(this.get_metas_last_properties.Length > 0))
			{
				int count = this.get_metas_last_properties.Length;
				
				properties = new Properties.MetaProperty[count];
				this.get_metas_last_properties.CopyTo (properties, 0);
			}
			else
			{
				properties = null;
			}
		}
		
		public void GetLayoutEngine(ulong code, out Layout.BaseEngine engine, out Properties.LayoutProperty property)
		{
			int  current_style_index   = Internal.CharMarker.GetStyleIndex (code);
			long current_style_version = this.style_list.InternalStyleTable.Version;
			
			if ((this.get_layout_last_style_version == current_style_version) &&
				(this.get_layout_last_style_index   == current_style_index))
			{
				engine   = this.get_layout_last_engine;
				property = this.get_layout_last_property;
				
				return;
			}
			
			Styles.SimpleStyle style = this.style_list.GetStyleFromIndex (current_style_index);
			
			property = style[Properties.WellKnownType.Layout] as Properties.LayoutProperty;
			
			if (property == null)
			{
				engine = null;
			}
			else
			{
				engine = this.layout_list[property.EngineName];
			}
			
			if (engine == null)
			{
				engine = this.layout_list["*"];
			}
			
			this.get_layout_last_style_version = current_style_version;
			this.get_layout_last_style_index   = current_style_index;
			this.get_layout_last_property      = property;
			this.get_layout_last_engine        = engine;
		}
		
		public void GetMargins(ulong code, out Properties.MarginsProperty property)
		{
			code = Internal.CharMarker.ExtractStyleAndSettings (code);
			
			long current_style_version = this.style_list.InternalStyleTable.Version;
			
			if ((this.get_margins_last_style_version == current_style_version) &&
				(this.get_margins_last_code == code))
			{
				property = this.get_margins_last_property;
				
				return;
			}
			
			Styles.SimpleStyle style = this.style_list[code];
			
			Styles.LocalSettings local_settings = style.GetLocalSettings (code);
			Styles.ExtraSettings extra_settings = style.GetExtraSettings (code);
			
			property = style[Properties.WellKnownType.Margins] as Properties.MarginsProperty;
			
			this.get_margins_last_style_version = current_style_version;
			this.get_margins_last_code          = code;
			this.get_margins_last_property      = property;
		}
		
		public void GetTab(ulong code, out Properties.TabProperty property)
		{
			code = Internal.CharMarker.ExtractStyleAndSettings (code);
			
			Styles.SimpleStyle style = this.style_list[code];
			
			Styles.LocalSettings local_settings = style.GetLocalSettings (code);
			Styles.ExtraSettings extra_settings = style.GetExtraSettings (code);
			
			property = extra_settings[Properties.WellKnownType.Tab] as Properties.TabProperty;
		}
		
		public void GetImage(ulong code, out Properties.ImageProperty property)
		{
			ulong stripped_code = Internal.CharMarker.ExtractStyleAndSettings (code);
			
			Styles.SimpleStyle   style          = this.style_list[stripped_code];
			Styles.LocalSettings local_settings = style.GetLocalSettings (stripped_code);
			
			if (local_settings != null)
			{
				property = local_settings[Properties.WellKnownType.Image] as Properties.ImageProperty;
			}
			else
			{
				property = null;
			}
		}
		
		
		public int GetTextStartDistance(TextStory story, ICursor cursor, Property property)
		{
			//	Trouve le début du texte marqué avec la propriété indiquée; retourne
			//	la distance parcourue (-1 en cas d'erreur).
			//
			//	Si le curseur se trouve au début du texte marqué, retourne 0.
			
			if (this.ContainsProperty (story, cursor, 0, property))
			{
				Internal.TextTable text   = story.TextTable;
				TextFinder         finder = new TextFinder (this, property);
				
				int distance = story.GetCursorPosition (cursor);
				int traverse = text.TraverseText (cursor.CursorId, - distance, finder.Callback);
				
				return traverse == -1 ? distance : traverse;
			}
			
			return -1;
		}
		
		public int GetTextEndDistance(TextStory story, ICursor cursor, Property property)
		{
			//	Trouve la fin du texte marqué avec la propriété indiquée; retourne
			//	la distance parcourue (-1 en cas d'erreur).
			
			if (this.ContainsProperty (story, cursor, 0, property))
			{
				Internal.TextTable text   = story.TextTable;
				TextFinder         finder = new TextFinder (this, property);
				
				int distance = story.TextLength - story.GetCursorPosition (cursor);
				int traverse = text.TraverseText (cursor.CursorId, distance, finder.Callback);
				
				return traverse == -1 ? distance : traverse;
			}
			
			return -1;
		}
		
		
		public bool ContainsProperty(TextStory story, ICursor cursor, Property property)
		{
			return this.ContainsProperty (story, cursor, 0, property);
		}
		
		public bool ContainsProperty(TextStory story, ICursor cursor, int offset, Property property)
		{
			ulong code = story.TextTable.ReadChar (cursor.CursorId, offset);
			
			if (code != 0)
			{
				code = Internal.CharMarker.ExtractStyleAndSettings (code);
				
				Styles.SimpleStyle style = this.style_list[code];
				
				if ((style != null) &&
					(style.Contains (code, property)))
				{
					return true;
				}
			}
			
			return false;
		}
		
		public bool ContainsProperty(TextStory story, ICursor cursor, int offset, Properties.WellKnownType well_known_type, Properties.PropertyType property_type)
		{
			ulong code = story.TextTable.ReadChar (cursor.CursorId, offset);
			
			if (code != 0)
			{
				code = Internal.CharMarker.ExtractStyleAndSettings (code);
				
				Styles.SimpleStyle style = this.style_list[code];
				
				if ((style != null) &&
					(style.Contains (code, well_known_type, property_type)))
				{
					return true;
				}
			}
			
			return false;
		}
		
		
		#region TextFinder Class
		private class TextFinder
		{
			public TextFinder(Context context, Property property)
			{
				this.context  = context;
				this.property = property;
				this.code     = code;
			}
			
			
			public TextStory.CodeCallback		Callback
			{
				get
				{
					return new TextStory.CodeCallback (this.Find);
				}
			}
			
			
			public bool Find(ulong code)
			{
				code = Internal.CharMarker.ExtractStyleAndSettings (code);
				
				if (code == this.code)
				{
					return false;
				}
				
				this.code = code;
				
				Styles.SimpleStyle style = this.context.style_list[code];
				
				if (style.Contains (this.code, this.property))
				{
					return false;
				}
				
				return true;
			}
			
			
			private Context						context;
			private Property					property;
			private ulong						code;
		}
		#endregion
		
		#region DefaultMarkers Class
		public class DefaultMarkers
		{
			internal DefaultMarkers(Internal.CharMarker marker)
			{
				this.requires_spell_checking = marker[DefaultMarkers.TagRequiresSpellChecking];
				this.selected                = marker[DefaultMarkers.TagSelected];
			}
			
			
			public ulong						Selected
			{
				get
				{
					return this.selected;
				}
			}
			
			public ulong						RequiresSpellChecking
			{
				get
				{
					return this.requires_spell_checking;
				}
			}
			
			
			private ulong						selected;
			private ulong						requires_spell_checking;
			
			public static string				TagSelected					= "Selected";
			public static string				TagRequiresSpellChecking	= "RequiresSpellChecking";
		}
		#endregion
		
		private void CreateDefaultLayout()
		{
			this.layout_list.NewEngine ("*", typeof (Layout.LineEngine));
		}
		
		private void CreateOrGetFontFromCache(string font_face, string font_style, out OpenType.Font font)
		{
			string font_full = string.Concat (font_face, "/", font_style);
			
			font = this.font_cache[font_full] as OpenType.Font;
			
			if (font == null)
			{
				font = this.font_collection.CreateFont (font_face, font_style);
				this.font_cache[font_full] = font;
			}
		}
		
		
		private StyleList						style_list;
		private LayoutList						layout_list;
		private GeneratorList					generator_list;
		private ParagraphManagerList			p_manager_list;
		private Internal.CharMarker				char_marker;
		private DefaultMarkers					markers;
		private System.Collections.Hashtable	conditions;
		private TextStyle						default_style;
		
		private OpenType.FontCollection			font_collection;
		private System.Collections.Hashtable	font_cache;
		
		private System.Collections.Hashtable	resources;
		
		private long							get_font_last_style_version;
		private int								get_font_last_style_index;
		private OpenType.Font					get_font_last_font;
		private double							get_font_last_font_size;
		
		private string							get_font_cache_name;
		private OpenType.Font					get_font_cache_font;
		
		private long							get_font_offset_last_style_version;
		private ulong							get_font_offset_last_code;
		private double							get_font_offset_last_baseline_offset;
		private double							get_font_offset_last_advance_offset;
		
		private long							get_color_last_style_version;
		private ulong							get_color_last_code;
		private Drawing.Color					get_color_last_color;
		
		private long							get_language_last_style_version;
		private ulong							get_language_last_code;
		private Properties.LanguageProperty		get_language_last_property;
		
		private long							get_auto_text_last_style_version;
		private ulong							get_auto_text_last_code;
		private Properties.AutoTextProperty		get_auto_text_last_property;
		
		private long							get_generator_last_style_version;
		private ulong							get_generator_last_code;
		private Properties.GeneratorProperty	get_generator_last_property;
		
		private long							get_condition_last_style_version;
		private int								get_condition_last_style_index;
		private Property[]						get_condition_last_properties;
		private bool							get_condition_last_summary;
		
		private long							get_properties_last_style_version;
		private int								get_properties_last_style_index;
		private Property[]						get_properties_last_properties;
		
		private long							get_styles_last_style_version;
		private int								get_styles_last_style_index;
		private TextStyle[]						get_styles_last_styles;
		
		private long							get_leading_last_style_version;
		private int								get_leading_last_style_index;
		private Properties.LeadingProperty		get_leading_last_property;
		
		private long							get_keep_last_style_version;
		private int								get_keep_last_style_index;
		private Properties.KeepProperty			get_keep_last_property;
		
		private long							get_underlines_last_style_version;
		private ulong							get_underlines_last_code;
		private Property[]						get_underlines_last_properties;
		
		private long							get_links_last_style_version;
		private ulong							get_links_last_code;
		private Property[]						get_links_last_properties;
		
		private long							get_metas_last_style_version;
		private ulong							get_metas_last_code;
		private Property[]						get_metas_last_properties;
		
		private long							get_layout_last_style_version;
		private int								get_layout_last_style_index;
		private Properties.LayoutProperty		get_layout_last_property;
		private Layout.BaseEngine				get_layout_last_engine;
		
		private long							get_margins_last_style_version;
		private ulong							get_margins_last_code;
		private Properties.MarginsProperty		get_margins_last_property;
	}
}

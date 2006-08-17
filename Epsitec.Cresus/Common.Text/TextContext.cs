//	Copyright © 2005-2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe TextContext décrit un contexte (pour la désérialisation) lié à
	/// un environnement 'texte'.
	/// </summary>
	public class TextContext : ILanguageRecognizer
	{
		public TextContext()
		{
			this.style_list     = new StyleList (this);
			this.tab_list       = new TabList (this);
			this.layout_list    = new LayoutList (this);
			this.generator_list = new GeneratorList (this);
			this.p_manager_list = new ParagraphManagerList (this);
			this.char_marker    = new Internal.CharMarker ();
			this.conditions     = new System.Collections.Hashtable ();
			this.stories        = new System.Collections.ArrayList ();
			
			this.char_marker.Add (TextContext.DefaultMarkers.TagSelected);
			this.char_marker.Add (TextContext.DefaultMarkers.TagRequiresSpellChecking);
			this.char_marker.Add (TextContext.DefaultMarkers.TagSpellCheckingError);
			
			this.markers = new TextContext.DefaultMarkers (this.char_marker);
			
			TextContext.InitializeFontCollection (null);
			
			this.CreateDefaultLayout ();
		}
		
		
		public StyleList						StyleList
		{
			get
			{
				return this.style_list;
			}
		}
		
		public TabList							TabList
		{
			get
			{
				return this.tab_list;
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
		
		public TextStyle						DefaultParagraphStyle
		{
			get
			{
				return this.default_para_style;
			}
			set
			{
				this.default_para_style = value;
			}
		}
		
		public TextStyle						DefaultTextStyle
		{
			get
			{
				return this.default_text_style;
			}
			set
			{
				this.default_text_style = value;
			}
		}
		
		
		public bool								ShowControlCharacters
		{
			get
			{
				return this.show_control_characters;
			}
			set
			{
				this.show_control_characters = value;
			}
		}
		
		public bool								IsDegradedLayoutEnabled
		{
			get
			{
				return this.is_degraded_layout_enabled;
			}
			set
			{
				this.is_degraded_layout_enabled = value;
			}
		}
		
		public bool								IsPropertiesPropertyEnabled
		{
			get
			{
				return this.is_properties_property_enabled;
			}
			set
			{
				this.is_properties_property_enabled = value;
			}
		}
		
		
		public TextContext.DefaultMarkers		Markers
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
		
		
		public long GenerateUniqueId()
		{
			lock (this.unique_id_lock)
			{
				return this.unique_id++;
			}
		}
		
		
		public byte[] Serialize()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			buffer.Append ("TextContext");
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeInt (TextContext.SerializationVersion));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeLong (this.unique_id));
			buffer.Append ("/");
			
			this.style_list.Serialize (buffer);
			buffer.Append ("/");
			
			string default_para_style_name = this.default_para_style == null ? null : this.default_para_style.Name;
			string default_text_style_name = this.default_text_style == null ? null : this.default_text_style.Name;
			
			buffer.Append (SerializerSupport.SerializeString (default_para_style_name));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeString (default_text_style_name));
			buffer.Append ("/");
			
			this.tab_list.Serialize (buffer);
			buffer.Append ("/");
			
			this.generator_list.Serialize (buffer);
			buffer.Append ("/");
			
			this.SerializeConditions (buffer);
			
			buffer.Append ("/");
			buffer.Append ("~");
			
			return System.Text.Encoding.UTF8.GetBytes (buffer.ToString ());
		}
		
		public void Deserialize(byte[] data)
		{
			string source = System.Text.Encoding.UTF8.GetString (data);
			string[] args = source.Split ('/');
			
			int offset = 0;
			
			string magick  = args[offset++];
			int    version = SerializerSupport.DeserializeInt (args[offset++]);
			
			System.Diagnostics.Debug.Assert (magick == "TextContext");
			System.Diagnostics.Debug.Assert (version <= TextContext.SerializationVersion);
			
			this.unique_id = SerializerSupport.DeserializeLong (args[offset++]);
			
			this.style_list.Deserialize (this, version, args, ref offset);
			
			this.default_para_style = null;
			this.default_text_style = null;
			
			if (version >= 2)
			{
				string default_para_style_name = SerializerSupport.DeserializeString (args[offset++]);
				
				System.Diagnostics.Debug.Assert (default_para_style_name != null);
				System.Diagnostics.Debug.Assert (default_para_style_name.Length > 0);
				
				this.default_para_style = this.style_list[default_para_style_name, TextStyleClass.Paragraph];
			}
			
			if (version >= 3)
			{
				string default_text_style_name = SerializerSupport.DeserializeString (args[offset++]);
				
				System.Diagnostics.Debug.Assert (default_text_style_name != null);
				System.Diagnostics.Debug.Assert (default_text_style_name.Length > 0);
				
				this.default_text_style = this.style_list[default_text_style_name, TextStyleClass.Text];
			}
			
			this.tab_list.Deserialize (this, version, args, ref offset);
			this.generator_list.Deserialize (this, version, args, ref offset);
			this.DeserializeConditions (version, args, ref offset);
			
			this.style_list.UpdateTabListUserCount ();
			this.style_list.UpdateGeneratorUserCount ();
			
			System.Diagnostics.Debug.Assert (args[offset] == "~");
			System.Diagnostics.Debug.Assert (args.Length == offset+1);
		}
		
		
		public bool GetGlyphAndFontForSpecialCode(ulong code, out ushort special_glyph, out OpenType.Font special_font)
		{
			System.Diagnostics.Debug.Assert (Unicode.Bits.GetSpecialCodeFlag (code));
			
			ulong stripped_code = Internal.CharMarker.ExtractCoreAndSettings (code);
			
			//	TODO: optimiser l'accès aux core_settings, local_settings et extra_settings dans
			//	tout TextContext en utilisant SettingsTable.GetCoreAndSettings()
			
			Styles.CoreSettings  core_settings  = this.style_list[stripped_code];
			Styles.LocalSettings local_settings = core_settings.GetLocalSettings (stripped_code);
			Styles.ExtraSettings extra_settings = core_settings.GetExtraSettings (stripped_code);
			
			int           glyph = -1;
			OpenType.Font font  = null;
			
			if (local_settings != null)
			{
				glyph = local_settings.GetGlyphForSpecialCode (code);
				font  = local_settings.GetFontForSpecialCode (this, code);
			}
			
			if ((glyph == -1) &&
				(extra_settings != null))
			{
				glyph = extra_settings.GetGlyphForSpecialCode (code);
				font  = extra_settings.GetFontForSpecialCode (this, code);
			}
			
			if (glyph == -1)
			{
				special_glyph = 0xffff;
				special_font  = null;
				
				return false;
			}
			else
			{
				special_glyph = (ushort) glyph;
				special_font  = font;
				
				return true;
			}
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
		
		public bool TestCondition(string name)
		{
			return this.conditions.Contains (name);
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
		
		
		public static void InitializeFontCollection(OpenType.FontIdentityCallback callback)
		{
			if (TextContext.font_collection == null)
			{
				TextContext.font_collection = OpenType.FontCollection.Default;
				TextContext.font_collection.RefreshCache (callback);
				TextContext.font_cache = new System.Collections.Hashtable ();
			}
		}
		
		public static string[] GetAvailableFontFaces()
		{
			TextContext.InitializeFontCollection (null);
			
			System.Collections.Hashtable hash = new System.Collections.Hashtable ();
			
			foreach (OpenType.FontIdentity id in TextContext.font_collection)
			{
				if (hash.Contains (id.InvariantFaceName) == false)
				{
					hash[id.InvariantFaceName] = id;
				}
			}
			
			string[] names = new string[hash.Count];
			hash.Keys.CopyTo (names, 0);
			System.Array.Sort (names);
			return names;
		}
		
		public static OpenType.FontIdentity[] GetAvailableFontIdentities(string face)
		{
			TextContext.InitializeFontCollection (null);
			
			if (TextContext.font_ids.ContainsKey (face))
			{
				List<OpenType.FontIdentity> list = TextContext.font_ids[face];
				return list.ToArray ();
			}
			else
			{
				List<OpenType.FontIdentity> list = new List<Epsitec.Common.OpenType.FontIdentity> ();
				
				foreach (OpenType.FontIdentity id in TextContext.font_collection)
				{
					if (id.InvariantFaceName == face)
					{
						list.Add (id);
					}
				}
				
				if (list.Count > 1)
				{
					list.Sort (OpenType.FontIdentity.Comparer);
				}
				
				TextContext.font_ids[face] = list;
				return list.ToArray ();
			}
		}
		
		
		public static OpenType.Font GetFont(OpenType.FontIdentity id)
		{
			OpenType.Font font = null;
			
			if (id != null)
			{
				TextContext.CreateOrGetFontFromCache (id.InvariantFaceName, id.InvariantStyleName, out font);
			}
			
			return font;
		}
		
		public static OpenType.Font GetFont(string invariant_face_name, string invariant_style_name)
		{
			OpenType.Font font;
			
			TextContext.CreateOrGetFontFromCache (invariant_face_name, invariant_style_name, out font);
			
			return font;
		}
		
		
		public void GetFontAndSize(ulong code, out OpenType.Font font, out double font_size, out double scale)
		{
			double glue;
			
			this.InternalGetFontAndSize (code, out font, out font_size, out scale, out glue);
			
			if (Unicode.Bits.GetSpecialCodeFlag (code))
			{
				//	Ce n'est pas un caractère normal, mais un caractère qui doit
				//	être remplacé par un glyph à la volée. Modifie aussi la fonte
				//	si besoin.
				
				ushort        s_glyph;
				OpenType.Font s_font;
				
				this.GetGlyphAndFontForSpecialCode (code, out s_glyph, out s_font);
				
				if (s_font != null)
				{
					font = s_font;
				}
			}
		}
		
		public void GetFontAndSize(ulong code, out OpenType.Font font, out double font_size, out double scale, out double glue)
		{
			this.InternalGetFontAndSize (code, out font, out font_size, out scale, out glue);
			
			if (Unicode.Bits.GetSpecialCodeFlag (code))
			{
				//	Ce n'est pas un caractère normal, mais un caractère qui doit
				//	être remplacé par un glyph à la volée. Modifie aussi la fonte
				//	si besoin.
				
				ushort        s_glyph;
				OpenType.Font s_font;
				
				this.GetGlyphAndFontForSpecialCode (code, out s_glyph, out s_font);
				
				if (s_font != null)
				{
					font = s_font;
				}
			}
		}
		
		public void GetFont(Properties.FontProperty font_property, out OpenType.Font font)
		{
			if (font_property != null)
			{
				string font_face  = font_property.FaceName;
				string font_style = font_property.StyleName;
				
				TextContext.CreateOrGetFontFromCache (font_face, font_style, out font);
			}
			else
			{
				font = null;
			}
		}
		
		public void GetFont(Property[] properties, out OpenType.Font font)
		{
			Properties.FontProperty        font_property = null;
			Properties.FontXscriptProperty font_xscript_property = null;
			
			for (int i = 0; i < properties.Length; i++)
			{
				if (properties[i] != null)
				{
					switch (properties[i].WellKnownType)
					{
						case Properties.WellKnownType.Font:
							font_property = properties[i] as Properties.FontProperty;
							break;
						
						case Properties.WellKnownType.FontXscript:
							font_xscript_property = properties[i] as Properties.FontXscriptProperty;
							break;
					}
				}
			}
			
			this.GetFont (font_property, out font);
		}
		
		
		public void GetFontSize(ulong code, out double font_size_in_points)
		{
			OpenType.Font font;
			
			double font_size;
			double scale;
			double glue;
			
			this.InternalGetFontAndSize (code, out font, out font_size, out scale, out glue);
			
			font_size_in_points = font_size * scale;
		}
		
		public void GetFontSize(Properties.FontProperty font_property, Properties.FontSizeProperty font_size_property, Properties.FontXscriptProperty font_xscript_property, out double font_size, out double font_scale, out double font_glue)
		{
			font_size  = font_size_property.SizeInPoints;
			font_scale = (font_xscript_property == null) || (font_xscript_property.IsDisabled) ? 1.0 : font_xscript_property.Scale;
			font_glue  = double.IsNaN (font_size_property.Glue) ? 0 : font_size * font_size_property.Glue;
		}
		
		public void GetFontSize(Property[] properties, out double font_size, out double font_scale, out double font_glue)
		{
			Properties.FontProperty        font_property         = null;
			Properties.FontSizeProperty    font_size_property    = null;
			Properties.FontXscriptProperty font_xscript_property = null;
			
			for (int i = 0; i < properties.Length; i++)
			{
				if (properties[i] != null)
				{
					switch (properties[i].WellKnownType)
					{
						case Properties.WellKnownType.Font:
							font_property = properties[i] as Properties.FontProperty;
							break;
						
						case Properties.WellKnownType.FontSize:
							font_size_property = properties[i] as Properties.FontSizeProperty;
							break;
						
						case Properties.WellKnownType.FontXscript:
							font_xscript_property = properties[i] as Properties.FontXscriptProperty;
							break;
					}
				}
			}
			
			this.GetFontSize (font_property, font_size_property, font_xscript_property, out font_size, out font_scale, out font_glue);
		}
		
		
		public void GetFontBaselineOffset(double font_pt_size, Property[] properties, out double offset)
		{
			Properties.FontProperty        font_property         = null;
			Properties.FontOffsetProperty  font_offset_property  = null;
			Properties.FontXscriptProperty font_xscript_property = null;
			
			for (int i = 0; i < properties.Length; i++)
			{
				if (properties[i] != null)
				{
					switch (properties[i].WellKnownType)
					{
						case Properties.WellKnownType.Font:
							font_property = properties[i] as Properties.FontProperty;
							break;
						
						case Properties.WellKnownType.FontOffset:
							font_offset_property = properties[i] as Properties.FontOffsetProperty;
							break;
						
						case Properties.WellKnownType.FontXscript:
							font_xscript_property = properties[i] as Properties.FontXscriptProperty;
							break;
					}
				}
			}
			
			offset = (font_offset_property == null) ? 0 : font_offset_property.GetOffsetInPoints (font_pt_size);
			
			if ((font_xscript_property != null) &&
				(font_xscript_property.IsDisabled == false))
			{
				offset += font_xscript_property.Offset * font_pt_size;
			}
		}
		
		public void GetFontOffsets(ulong code, out double baseline_offset, out double advance_offset)
		{
			code = Internal.CharMarker.ExtractCoreAndSettings (code);
			
			long current_style_version = this.style_list.Version;
			
			if ((this.get_font_offset_last_style_version == current_style_version) &&
				(this.get_font_offset_last_code == code))
			{
				baseline_offset = this.get_font_offset_last_baseline_offset;
				advance_offset  = this.get_font_offset_last_advance_offset;
				
				return;
			}
			
			int current_style_index = Internal.CharMarker.GetCoreIndex (code);
			
			if ((this.get_font_last_style_version != current_style_version) ||
				(this.get_font_last_style_index   != current_style_index))
			{
				//	Rafraîchit les informations sur la fonte utilisée :
				
				OpenType.Font font;
				double        font_size;
				double        font_scale;
				
				this.GetFontAndSize (code, out font, out font_size, out font_scale);
			}
			
			Styles.CoreSettings  core_settings  = this.style_list.GetCoreFromIndex (current_style_index);
			Styles.LocalSettings local_settings = core_settings.GetLocalSettings (code);
			
			advance_offset  = 0;
			baseline_offset = 0;
			
			Properties.FontXscriptProperty font_xscript_p = core_settings[Properties.WellKnownType.FontXscript] as Properties.FontXscriptProperty;
			
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
			
			if ((font_xscript_p != null) &&
				(font_xscript_p.IsDisabled == false))
			{
				baseline_offset += font_xscript_p.Offset * this.get_font_last_font_size;
			}
			
			this.get_font_offset_last_style_version   = current_style_version;
			this.get_font_offset_last_code            = code;
			this.get_font_offset_last_baseline_offset = baseline_offset;
			this.get_font_offset_last_advance_offset  = advance_offset;
		}
		
		
		public void GetColor(ulong code, out string color)
		{
			code = Internal.CharMarker.ExtractCoreAndSettings (code);
			
			long current_style_version = this.style_list.Version;
			
			if ((this.get_color_last_style_version == current_style_version) &&
				(this.get_color_last_code == code))
			{
				color = this.get_color_last_color;
				
				return;
			}
			
			Styles.CoreSettings  core_settings  = this.style_list[code];
			Styles.ExtraSettings extra_settings = core_settings.GetExtraSettings (code);
			
			Properties.FontColorProperty color_p = extra_settings[Properties.WellKnownType.FontColor] as Properties.FontColorProperty;
			
			if (color_p == null)
			{
				color = null;
			}
			else
			{
				color = color_p.TextColor;
			}
			
			this.get_color_last_style_version = current_style_version;
			this.get_color_last_code          = code;
			this.get_color_last_color         = color;
		}
		
		public void GetOpenType(ulong code, out Properties.OpenTypeProperty property)
		{
			code = Internal.CharMarker.ExtractCoreAndSettings (code);
			
			Styles.CoreSettings  core_settings  = this.style_list[code];
			Styles.LocalSettings local_settings = core_settings.GetLocalSettings (code);
			
			if (local_settings == null)
			{
				property = null;
			}
			else
			{
				property = local_settings[Properties.WellKnownType.OpenType] as Properties.OpenTypeProperty;
			}
		}
		
		public void GetLanguage(ulong code, out Properties.LanguageProperty property)
		{
			code = Internal.CharMarker.ExtractCoreAndSettings (code);
			
			long current_style_version = this.style_list.Version;
			
			if ((this.get_language_last_style_version == current_style_version) &&
				(this.get_language_last_code == code))
			{
				property = this.get_language_last_property;
				
				return;
			}
			
			Styles.CoreSettings  core_settings  = this.style_list[code];
			Styles.ExtraSettings extra_settings = core_settings == null ? null : core_settings.GetExtraSettings (code);
			
			property = extra_settings == null ? null : extra_settings[Properties.WellKnownType.Language] as Properties.LanguageProperty;
			
			this.get_language_last_style_version = current_style_version;
			this.get_language_last_code          = code;
			this.get_language_last_property      = property;
		}
		
		
		public IParagraphManager GetParagraphManager(ulong code)
		{
			Properties.ManagedParagraphProperty mpp;
			
			if ((code != 0) &&
				(this.GetManagedParagraph (code, out mpp)))
			{
				return this.p_manager_list[mpp.ManagerName];
			}
			else
			{
				return null;
			}
		}
		
		
		public bool GetManagedParagraph(ulong code, out Properties.ManagedParagraphProperty property)
		{
			int  current_style_index   = Internal.CharMarker.GetCoreIndex (code);
			long current_style_version = this.style_list.Version;
			
			if ((this.get_managed_paragraph_last_style_version == current_style_version) &&
				(this.get_managed_paragraph_last_style_index   == current_style_index))
			{
				property = this.get_managed_paragraph_last_property;
				
				return property != null;
			}
			
			Styles.CoreSettings core_settings  = this.style_list[code];
			
			property = core_settings[Properties.WellKnownType.ManagedParagraph] as Properties.ManagedParagraphProperty;
			
			this.get_managed_paragraph_last_style_version = current_style_version;
			this.get_managed_paragraph_last_style_index   = current_style_index;
			this.get_managed_paragraph_last_property      = property;
			
			return property != null;
		}
		
		public void GetManagedInfo(ulong code, string name, out Properties.ManagedInfoProperty property)
		{
			Styles.CoreSettings  core_settings  = this.style_list[code];
			Styles.ExtraSettings extra_settings = core_settings.GetExtraSettings (code);
			
			if (extra_settings == null)
			{
				property = null;
			}
			else
			{
				property = Properties.ManagedInfoProperty.Find (extra_settings.GetProperties (), name);
			}
		}
		
		
		public bool GetAutoText(ulong code, out Properties.AutoTextProperty property)
		{
			code = Internal.CharMarker.ExtractCoreAndSettings (code);
			
			long current_style_version = this.style_list.Version;
			
			if ((this.get_auto_text_last_style_version == current_style_version) &&
				(this.get_auto_text_last_code == code))
			{
				property = this.get_auto_text_last_property;
				
				return property != null;
			}
			
			Styles.CoreSettings  core_settings  = this.style_list[code];
			Styles.ExtraSettings extra_settings = core_settings.GetExtraSettings (code);
			
			property = extra_settings == null ? null : extra_settings[Properties.WellKnownType.AutoText] as Properties.AutoTextProperty;
			
			this.get_auto_text_last_style_version = current_style_version;
			this.get_auto_text_last_code          = code;
			this.get_auto_text_last_property      = property;
			
			return property != null;
		}
		
		public bool GetGenerator(ulong code, out Properties.GeneratorProperty property)
		{
			code = Internal.CharMarker.ExtractCoreAndSettings (code);
			
			long current_style_version = this.style_list.Version;
			
			if ((this.get_generator_last_style_version == current_style_version) &&
				(this.get_generator_last_code == code))
			{
				property = this.get_generator_last_property;
				
				return property != null;
			}
			
			Styles.CoreSettings  core_settings  = this.style_list[code];
			Styles.ExtraSettings extra_settings = core_settings.GetExtraSettings (code);
			
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
			int  current_style_index   = Internal.CharMarker.GetCoreIndex (code);
			long current_style_version = this.style_list.Version;
			
			if ((this.get_condition_last_style_version != current_style_version) ||
				(this.get_condition_last_style_index   != current_style_index))
			{
				Styles.CoreSettings core_settings = this.style_list.GetCoreFromIndex (current_style_index);
				
				Property[] props = core_settings.FindProperties (Properties.WellKnownType.Conditional);
				
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
			if (this.IsPropertiesPropertyEnabled)
			{
				int  current_style_index   = Internal.CharMarker.GetCoreIndex (code);
				long current_style_version = this.style_list.Version;
				
				if ((this.get_properties_last_style_version != current_style_version) ||
					(this.get_properties_last_style_index   != current_style_index))
				{
					Styles.CoreSettings core_settings = this.style_list.GetCoreFromIndex (current_style_index);
					
					Properties.PropertiesProperty props = core_settings == null ? null : core_settings[Properties.WellKnownType.Properties] as Properties.PropertiesProperty;
					
					if (props == null)
					{
						this.get_properties_last_properties = new Property[0];
					}
					else
					{
						string[] serialized = props.SerializedProperties;
						
						if (serialized.Length > 0)
						{
							this.get_properties_last_properties = Properties.PropertiesProperty.DeserializePropertiesFromStringArray (this, serialized);
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
			else
			{
				TextStyle[] styles;
				this.GetPropertiesQuickAndDirty (code, out styles, out properties);
			}
		}
		
		public void GetStylesAndProperties(ulong code, out TextStyle[] styles, out Property[] properties)
		{
			this.GetPropertiesQuickAndDirty (code, out styles, out properties);
		}
		
		public void GetStyles(ulong code, out TextStyle[] styles)
		{
			code = Internal.CharMarker.ExtractCoreAndExtraSettings (code);
			
			long current_style_version = this.style_list.Version;
			
			if ((this.get_styles_last_style_version != current_style_version) ||
				(this.get_styles_last_style_code    != code))
			{
				Styles.CoreSettings       core_settings = this.style_list[code];
				Properties.StylesProperty props;
				
				if (core_settings == null)
				{
					props = null;
				}
				else
				{
					props = core_settings[Properties.WellKnownType.Styles] as Properties.StylesProperty;
					
					if (props == null)
					{
						Styles.ExtraSettings extra = core_settings.GetExtraSettings (code);
						props = extra[Properties.WellKnownType.Styles] as Properties.StylesProperty;
					}
				}
				
				if (props == null)
				{
					styles = new TextStyle[0];
				}
				else
				{
					styles = props.GetTextStyles (this);
				}
				
				this.get_styles_last_styles        = styles;
				this.get_styles_last_style_version = current_style_version;
				this.get_styles_last_style_code    = code;
			}
			
			styles = new TextStyle[this.get_styles_last_styles.Length];
			this.get_styles_last_styles.CopyTo (styles, 0);
		}
		
		
		public TextStyle[] FilterDefaultTextStyle(TextStyle[] styles)
		{
			int count = styles.Length;
			
			for (int i = 0; i < styles.Length; i++)
			{
				if (styles[i] == this.default_text_style)
				{
					count--;
				}
			}
			
			if (count != styles.Length)
			{
				TextStyle[] copy = new TextStyle[count];
				
				for (int i = 0, j = 0; i < styles.Length && j < count; i++)
				{
					if (styles[i] == this.default_text_style)
					{
						//	Saute...
					}
					else
					{
						copy[j++] = styles[i];
					}
				}
				
				return copy;
			}
			
			return styles;
		}
		
		public TextStyle[] AddDefaultTextStyleIfNeeded(TextStyle[] styles)
		{
			for (int i = 0; i < styles.Length; i++)
			{
				if ((styles[i] == this.default_text_style) ||
					(styles[i].TextStyleClass == TextStyleClass.Text))
				{
					return styles;
				}
			}
			
			TextStyle[] copy = new TextStyle[styles.Length+1];
			styles.CopyTo (copy, 0);
			copy[styles.Length] = this.default_text_style;
			
			return copy;
		}
		
		public TextStyle[] RemoveDeadStyles(TextStyle[] styles)
		{
			int count = styles.Length;
			
			for (int i = 0; i < styles.Length; i++)
			{
				if (styles[i].IsDeleted)
				{
					count--;
				}
			}
			
			if (count != styles.Length)
			{
				TextStyle[] copy = new TextStyle[count];
				
				for (int i = 0, j = 0; i < styles.Length && j < count; i++)
				{
					if (styles[i].IsDeleted)
					{
						//	Saute...
					}
					else
					{
						copy[j++] = styles[i];
					}
				}
				
				return copy;
			}
			
			return styles;
		}
		
		
		#region Internal Property & Style Related Methods
		internal void GetAllProperties(ulong code, out Property[] properties)
		{
			//	Retourne les propriétés associées à un code de caractère donné.
			//	Les propriétés sont brutes, telles que vues par le système de
			//	layout, par exemple.
			
			Internal.SettingsTable settings = this.StyleList.InternalSettingsTable;
			
			Styles.CoreSettings  core  = settings.GetCore (code);
			Styles.LocalSettings local = core == null ? null : core.GetLocalSettings (code);
			Styles.ExtraSettings extra = core == null ? null : core.GetExtraSettings (code);
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			if (core != null)
			{
				list.AddRange (core.GetProperties ());
			}
			if (local != null)
			{
				list.AddRange (local.GetProperties ());
			}
			if (extra != null)
			{
				list.AddRange (extra.GetProperties ());
			}
			
			properties = (Property[]) list.ToArray (typeof (Property));
		}
		
		internal void GetLocalAndExtraSettingsProperties(ulong code, out Property[] properties)
		{
			//	Retourne les propriétés associées à un code de caractère donné.
			//	Les propriétés sont brutes, telles que vues par le système de
			//	layout, par exemple.
			
			Internal.SettingsTable settings = this.StyleList.InternalSettingsTable;
			
			Styles.CoreSettings core = settings.GetCore (code);
			
			if (core == null)
			{
				properties = new Property[0];
				return;
			}
			
			Styles.LocalSettings local = core.GetLocalSettings (code);
			Styles.ExtraSettings extra = core.GetExtraSettings (code);
			
			if ((local == null) &&
				(extra == null))
			{
				properties = new Property[0];
				return;
			}
			
			if (local == null)
			{
				properties = extra.GetProperties ();
				return;
			}
			
			if (extra == null)
			{
				properties = local.GetProperties ();
				return;
			}
			
			//	Il y a à la fois des réglages locaux et extra, il faut donc
			//	accumuler les deux :
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			list.AddRange (local.GetProperties ());
			list.AddRange (extra.GetProperties ());
			
			properties = (Property[]) list.ToArray (typeof (Property));
		}
		
		internal void GetLocalSettingsProperties(ulong code, out Property[] properties)
		{
			//	Retourne les propriétés associées à un code de caractère donné.
			//	Les propriétés sont brutes, telles que vues par le système de
			//	layout, par exemple.
			//	Ne retourne que les propriétés de la catégorie LocalSettings.
			
			Internal.SettingsTable settings = this.StyleList.InternalSettingsTable;
			
			Styles.LocalSettings local = settings.GetLocalSettings (code);
			
			if (local != null)
			{
				properties = local.GetProperties ();
			}
			else
			{
				properties = new Property[0];
			}
		}
		
		internal void GetFlatProperties(System.Collections.ICollection text_styles, out TextStyle[] styles, out Property[] properties)
		{
			//	Retourne les propriétés définies par une collection de styles.
			//	Retourne aussi les styles sous la forme d'un tableau trié.
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			//	Trie les styles selon leur priorité, avant de les convertir en
			//	propriétés :
			
			styles = new TextStyle[text_styles.Count];
			text_styles.CopyTo (styles, 0);
			System.Array.Sort (styles, TextStyle.Comparer);
			
			//	Les diverses propriétés des styles passés en entrée sont
			//	extraites et ajoutées dans la liste complète des propriétés :
			
			foreach (TextStyle style in styles)
			{
				//	Passe en revue toutes les propriétés définies par le style
				//	en cours d'analyse et ajoute celles-ci séquentiellement dans
				//	la liste des propriétés :
				
				list.AddRange (style.GetProperties ());
			}
			
			properties = (Property[]) list.ToArray (typeof (Property));
		}
		
		internal void GetPropertiesQuickAndDirty(ulong code, out TextStyle[] styles, out Property[] properties)
		{
			//	Détermine quels styles et propriétés ont conduit aux propriétés
			//	associées avec le caractère spécifié. Cette méthode se base sur
			//	un certain nombre de simplifications :
			//
			//	- Un style ne fait pas référence à des propriétés LocalSettings.
			//
			//	- Les propriétés PropertyType.CoreSettings et ExtraSettings sont
			//	  toujours enrobées dans une méta-propriété (donc un TextStyle).
			//
			//	- TabsProperty doit être adaptée (catégorie des ExtraSettings)
			//	  pour ne garder que les taquets locaux (TabClass.Auto).
			//
			//	Avec ces données, la liste des propriétés se laisse reconstruire
			//	relativement facilement.
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			Property[] all_properties;
			
#if true
			this.GetLocalAndExtraSettingsProperties (code, out all_properties);
			this.GetStyles (code, out styles);
			
			foreach (Property property in all_properties)
			{
				if (property.PropertyType == Properties.PropertyType.LocalSetting)
				{
					list.Add (property);
				}
				else if (property.WellKnownType == Properties.WellKnownType.Tabs)
				{
					Properties.TabsProperty tabs = property as Properties.TabsProperty;
					
					tabs = TabList.FilterTabs (tabs, TabClass.Auto);
					
					if (tabs != null)
					{
						list.Add (tabs);
					}
				}
				else if ((property.WellKnownType == Properties.WellKnownType.Generator) ||
					/**/ (property.WellKnownType == Properties.WellKnownType.AutoText))
				{
					//	Ces propriétés ne peuvent jamais faire partie d'un style que
					//	l'utilisateur peut appliquer; elles sont générées uniquement
					//	par les "managed paragraphs" :
					
					list.Add (property);
				}
			}
#else
			this.GetAllProperties (code, out all_properties);
			this.GetStyles (code, out styles);
			
			Property[] style_properties;
			
			this.GetFlatProperties (styles, out styles, out style_properties);
			this.AccumulateProperties (style_properties, out style_properties);
			
			foreach (Property pp in all_properties)
			{
				if ((pp.WellKnownType == Properties.WellKnownType.Properties) ||
					(pp.WellKnownType == Properties.WellKnownType.Styles))
				{
					continue;
				}
				
				bool skip = false;
				
				for (int i = 0; i < style_properties.Length; i++)
				{
					Property sp = style_properties[i];
					
					if (sp != null)
					{
						if (sp.WellKnownType == pp.WellKnownType)
						{
							if (sp.GetContentsSignature () == pp.GetContentsSignature ())
							{
								if (Property.CompareEqualContents (pp, sp))
								{
									style_properties[i] = null;
									skip = true;
									break;
								}
							}
						}
					}
				}
				
				if (! skip)
				{
					list.Add (pp);
				}
			}
#endif
			
			properties = (Property[]) list.ToArray (typeof (Property));
		}
		
		internal void AccumulateProperties(System.Collections.ICollection text_properties, out Property[] properties)
		{
			Styles.PropertyContainer.Accumulator accumulator = new Styles.PropertyContainer.Accumulator ();
			
			foreach (Property property in text_properties)
			{
				accumulator.Accumulate (property);
			}
			
			properties = accumulator.AccumulatedProperties;
		}
		#endregion
		
		public void GetLeading(ulong code, out Properties.LeadingProperty property)
		{
			int  current_style_index   = Internal.CharMarker.GetCoreIndex (code);
			long current_style_version = this.style_list.Version;
			
			if ((this.get_leading_last_style_version == current_style_version) &&
				(this.get_leading_last_style_index   == current_style_index))
			{
				property = this.get_leading_last_property;
				
				return;
			}
			
			Styles.CoreSettings core_settings = this.style_list.GetCoreFromIndex (current_style_index);
			
			property = core_settings[Properties.WellKnownType.Leading] as Properties.LeadingProperty;
			
			this.get_leading_last_style_version = current_style_version;
			this.get_leading_last_style_index   = current_style_index;
			this.get_leading_last_property      = property;
		}
		
		public void GetKeep(ulong code, out Properties.KeepProperty property)
		{
			int  current_style_index   = Internal.CharMarker.GetCoreIndex (code);
			long current_style_version = this.style_list.Version;
			
			if ((this.get_keep_last_style_version == current_style_version) &&
				(this.get_keep_last_style_index   == current_style_index))
			{
				property = this.get_keep_last_property;
				
				return;
			}
			
			Styles.CoreSettings core_settings = this.style_list.GetCoreFromIndex (current_style_index);
			
			property = core_settings[Properties.WellKnownType.Keep] as Properties.KeepProperty;
			
			this.get_keep_last_style_version = current_style_version;
			this.get_keep_last_style_index   = current_style_index;
			this.get_keep_last_property      = property;
		}
		
		public void GetXlines(ulong code, out Properties.AbstractXlineProperty[] properties)
		{
			code = Internal.CharMarker.ExtractCoreAndSettings (code);
			
			long current_style_version = this.style_list.Version;
			
			if ((this.get_underlines_last_style_version != current_style_version) ||
				(this.get_underlines_last_code != code))
			{
				Styles.CoreSettings core_settings = this.style_list[code];
				
				Property[]           base_props     = null;
				Styles.ExtraSettings extra_settings = core_settings.GetExtraSettings (code);
				
				if (extra_settings != null)
				{
					base_props = extra_settings.FindProperties (Properties.WellKnownType.Underline, Properties.WellKnownType.Strikeout, Properties.WellKnownType.Overline, Properties.WellKnownType.TextBox, Properties.WellKnownType.TextMarker);
					
					System.Array.Sort (base_props, Properties.AbstractXlineProperty.Comparer);
				}
				
				this.get_underlines_last_style_version = current_style_version;
				this.get_underlines_last_code          = code;
				this.get_underlines_last_properties    = base_props;
			}
			
			if ((this.get_underlines_last_properties != null) &&
				(this.get_underlines_last_properties.Length > 0))
			{
				int count = this.get_underlines_last_properties.Length;
				
				properties = new Properties.AbstractXlineProperty[count];
				this.get_underlines_last_properties.CopyTo (properties, 0);
			}
			else
			{
				properties = null;
			}
		}
		
		public void GetLinks(ulong code, out Properties.LinkProperty[] properties)
		{
			code = Internal.CharMarker.ExtractCoreAndSettings (code);
			
			long current_style_version = this.style_list.Version;
			
			if ((this.get_links_last_style_version != current_style_version) ||
				(this.get_links_last_code != code))
			{
				Styles.CoreSettings core_settings = this.style_list[code];
				
				Property[]           base_props     = null;
				Styles.ExtraSettings extra_settings = core_settings.GetExtraSettings (code);
				
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
		
		public void GetUserTags(ulong code, out Properties.UserTagProperty[] properties)
		{
			code = Internal.CharMarker.ExtractCoreAndSettings (code);
			
			long current_style_version = this.style_list.Version;
			
			if ((this.get_usertags_last_style_version != current_style_version) ||
				(this.get_usertags_last_code != code))
			{
				Styles.CoreSettings core_settings = this.style_list[code];
				
				Property[]           base_props     = null;
				Styles.ExtraSettings extra_settings = core_settings.GetExtraSettings (code);
				
				if (extra_settings != null)
				{
					base_props = extra_settings.FindProperties (Properties.WellKnownType.UserTag);
					
					System.Array.Sort (base_props, Properties.UserTagProperty.Comparer);
				}
				
				this.get_usertags_last_style_version = current_style_version;
				this.get_usertags_last_code          = code;
				this.get_usertags_last_properties    = base_props;
			}
			
			if ((this.get_usertags_last_properties != null) &&
				(this.get_usertags_last_properties.Length > 0))
			{
				int count = this.get_usertags_last_properties.Length;
				
				properties = new Properties.UserTagProperty[count];
				this.get_usertags_last_properties.CopyTo (properties, 0);
			}
			else
			{
				properties = null;
			}
		}
		
		public void GetLayoutEngine(ulong code, out Layout.BaseEngine engine, out Properties.LayoutProperty property)
		{
			int  current_style_index   = Internal.CharMarker.GetCoreIndex (code);
			long current_style_version = this.style_list.Version;
			
			if ((this.get_layout_last_style_version == current_style_version) &&
				(this.get_layout_last_style_index   == current_style_index))
			{
				engine   = this.get_layout_last_engine;
				property = this.get_layout_last_property;
				
				return;
			}
			
			Styles.CoreSettings core_settings = this.style_list.GetCoreFromIndex (current_style_index);
			
			property = core_settings[Properties.WellKnownType.Layout] as Properties.LayoutProperty;
			
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
			code = Internal.CharMarker.ExtractCoreAndSettings (code);
			
			long current_style_version = this.style_list.Version;
			
			if ((this.get_margins_last_style_version == current_style_version) &&
				(this.get_margins_last_code == code))
			{
				property = this.get_margins_last_property;
				
				return;
			}
			
			Styles.CoreSettings core_settings = this.style_list[code];
			
			property = core_settings[Properties.WellKnownType.Margins] as Properties.MarginsProperty;
			
			this.get_margins_last_style_version = current_style_version;
			this.get_margins_last_code          = code;
			this.get_margins_last_property      = property;
		}
		
		public void GetBreak(ulong code, out Properties.BreakProperty property)
		{
			code = Internal.CharMarker.ExtractCoreAndSettings (code);
			
			Styles.CoreSettings  core_settings  = this.style_list[code];
			Styles.LocalSettings local_settings = core_settings.GetLocalSettings (code);
			
			if (local_settings == null)
			{
				property = null;
			}
			else
			{
				property = local_settings[Properties.WellKnownType.Break] as Properties.BreakProperty;
			}
		}
		
		public void GetTab(ulong code, out Properties.TabProperty property)
		{
			code = Internal.CharMarker.ExtractCoreAndSettings (code);
			
			Styles.CoreSettings  core_settings  = this.style_list[code];
			Styles.LocalSettings local_settings = core_settings.GetLocalSettings (code);
			
			if (local_settings == null)
			{
				property = null;
			}
			else
			{
				property = local_settings[Properties.WellKnownType.Tab] as Properties.TabProperty;
			}
		}
		
		public void GetTabs(ulong code, out Properties.TabsProperty property)
		{
			code = Internal.CharMarker.ExtractCoreAndExtraSettings (code);
			
			Styles.CoreSettings  core_settings  = this.style_list[code];
			Styles.ExtraSettings extra_settings = core_settings.GetExtraSettings (code);
			
			if (extra_settings == null)
			{
				property = null;
			}
			else
			{
				property = extra_settings[Properties.WellKnownType.Tabs] as Properties.TabsProperty;
			}
		}
		
		public void GetTabAndTabs(ulong code, out Properties.TabProperty tab_property, out Properties.TabsProperty tabs_property)
		{
			code = Internal.CharMarker.ExtractCoreAndSettings (code);
			
			Styles.CoreSettings  core_settings  = this.style_list[code];
			Styles.LocalSettings local_settings = core_settings.GetLocalSettings (code);
			Styles.ExtraSettings extra_settings = core_settings.GetExtraSettings (code);
			
			if (local_settings == null)
			{
				tab_property  = null;
			}
			else
			{
				tab_property  = local_settings[Properties.WellKnownType.Tab] as Properties.TabProperty;
			}
			
			if (extra_settings == null)
			{
				tabs_property = null;
			}
			else
			{
				tabs_property = extra_settings[Properties.WellKnownType.Tabs] as Properties.TabsProperty;
			}
		}
		
		public void GetImage(ulong code, out Properties.ImageProperty property)
		{
			ulong stripped_code = Internal.CharMarker.ExtractCoreAndSettings (code);
			
			Styles.CoreSettings  core_settings  = this.style_list[stripped_code];
			Styles.LocalSettings local_settings = core_settings.GetLocalSettings (stripped_code);
			
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
			ulong code = story.ReadChar (cursor, offset);
			
			if (code != 0)
			{
				code = Internal.CharMarker.ExtractCoreAndSettings (code);
				
				Styles.CoreSettings core_settings = this.style_list[code];
				
				if ((core_settings != null) &&
					(core_settings.Contains (code, property)))
				{
					return true;
				}
			}
			
			return false;
		}
		
		public bool ContainsProperty(TextStory story, ICursor cursor, int offset, Properties.WellKnownType well_known_type, Properties.PropertyType property_type)
		{
			ulong code = story.ReadChar (cursor, offset);
			
			if (code != 0)
			{
				code = Internal.CharMarker.ExtractCoreAndSettings (code);
				
				Styles.CoreSettings core_settings = this.style_list[code];
				
				if ((core_settings != null) &&
					(core_settings.Contains (code, well_known_type, property_type)))
				{
					return true;
				}
			}
			
			return false;
		}
		
		
		internal void Attach(TextStory story)
		{
			this.stories.Add (story);
		}
		
		internal void Detach(TextStory story)
		{
			this.stories.Remove (story);
		}
		
		internal System.Collections.ICollection GetTextStories()
		{
			return this.stories;
		}


		#region ILanguageRecognizer Members

		bool ILanguageRecognizer.GetLanguage(ulong[] text, int offset, out double hyphenation, out string locale)
		{
			Properties.LanguageProperty property;

			this.GetLanguage (text[offset], out property);

			if (property == null)
			{
				hyphenation = 0;
				locale = null;
				return false;
			}
			else
			{
				hyphenation = property.Hyphenation;
				locale = property.Locale;
				return true;
			}
		}

		#endregion
		
		#region TextFinder Class
		private class TextFinder
		{
			public TextFinder(TextContext context, Property property)
			{
				this.context  = context;
				this.property = property;
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
				code = Internal.CharMarker.ExtractCoreAndSettings (code);
				
				if (code == this.code)
				{
					return false;
				}
				
				this.code = code;
				
				Styles.CoreSettings core_settings = this.context.style_list[code];
				
				if (core_settings.Contains (this.code, this.property))
				{
					return false;
				}
				
				return true;
			}
			
			
			private TextContext					context;
			private Property					property;
			private ulong						code;
		}
		#endregion
		
		#region DefaultMarkers Class
		public class DefaultMarkers
		{
			internal DefaultMarkers(Internal.CharMarker marker)
			{
				this.selected                = marker[DefaultMarkers.TagSelected];
				this.requires_spell_checking = marker[DefaultMarkers.TagRequiresSpellChecking];
				this.spell_checking_error    = marker[DefaultMarkers.TagSpellCheckingError];
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
			
			public ulong						SpellCheckingError
			{
				get
				{
					return this.spell_checking_error;
				}
			}
			
			
			private ulong						selected;
			private ulong						requires_spell_checking;
			private ulong						spell_checking_error;
			
			public const string					TagSelected					= "Selected";
			public const string					TagRequiresSpellChecking	= "RequiresSpellChecking";
			public const string					TagSpellCheckingError		= "SpellCheckingError";
		}
		#endregion
		
		private void SerializeConditions(System.Text.StringBuilder buffer)
		{
			string[] conditions = new string[this.conditions.Count];
			this.conditions.Keys.CopyTo (conditions, 0);
			
			buffer.Append (SerializerSupport.SerializeStringArray (conditions));
		}
		
		private void DeserializeConditions(int version, string[] args, ref int offset)
		{
			string[] conditions = SerializerSupport.DeserializeStringArray (args[offset++]);
			
			this.conditions.Clear ();
			
			foreach (string condition in conditions)
			{
				this.SetCondition (condition);
			}
			
			System.Diagnostics.Debug.Assert (this.conditions.Count == conditions.Length);
		}
		
		
		private void CreateDefaultLayout()
		{
			this.layout_list.NewEngine ("*", typeof (Layout.LineEngine));
		}
		
		private static void CreateOrGetFontFromCache(string font_face, string font_style, out OpenType.Font font)
		{
			TextContext.InitializeFontCollection (null);
			
			string font_full = string.Concat (font_face, "/", font_style);
			
			lock (TextContext.font_cache)
			{
				font = TextContext.font_cache[font_full] as OpenType.Font;
				
				if (font == null)
				{
					font = TextContext.font_collection.CreateFont (font_face, font_style);
					TextContext.font_cache[font_full] = font;
				}
			}
		}
		
		private void InternalGetFontAndSize(ulong code, out OpenType.Font font, out double font_size, out double scale, out double glue)
		{
			int  current_style_index   = Internal.CharMarker.GetCoreIndex (code);
			long current_style_version = this.style_list.Version;
			
			if ((this.get_font_last_style_version == current_style_version) &&
				(this.get_font_last_style_index   == current_style_index))
			{
				font      = this.get_font_last_font;
				font_size = this.get_font_last_font_size;
				scale     = this.get_font_last_scale;
				glue      = this.get_font_last_glue;
				
				return;
			}
			
			Styles.CoreSettings core_settings = this.style_list.GetCoreFromIndex (current_style_index);

			Properties.FontProperty        font_p         = core_settings[Properties.WellKnownType.Font] as Properties.FontProperty;
			Properties.FontSizeProperty    font_size_p    = core_settings[Properties.WellKnownType.FontSize] as Properties.FontSizeProperty;
			Properties.FontXscriptProperty font_xscript_p = core_settings[Properties.WellKnownType.FontXscript] as Properties.FontXscriptProperty;
			
			this.GetFont (font_p, out font);
			this.GetFontSize (font_p, font_size_p, font_xscript_p, out font_size, out scale, out glue);
			
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
			this.get_font_last_scale         = scale;
			this.get_font_last_font_size     = font_size;
			this.get_font_last_glue          = glue;
		}
		
		
		
		private StyleList						style_list;
		private TabList							tab_list;
		private LayoutList						layout_list;
		private GeneratorList					generator_list;
		private ParagraphManagerList			p_manager_list;
		private Internal.CharMarker				char_marker;
		private DefaultMarkers					markers;
		private System.Collections.Hashtable	conditions;
		private System.Collections.ArrayList	stories;
		private TextStyle						default_para_style;
		private TextStyle						default_text_style;
		
		private long							unique_id = 1;
		private object							unique_id_lock = new object ();
		
		private bool							show_control_characters;
		private bool							is_degraded_layout_enabled;
		private bool							is_properties_property_enabled = true;
		
		static OpenType.FontCollection			font_collection;
		static System.Collections.Hashtable		font_cache;
		
		private System.Collections.Hashtable	resources;
		
		private long							get_font_last_style_version;
		private int								get_font_last_style_index;
		private OpenType.Font					get_font_last_font;
		private double							get_font_last_font_size;
		private double							get_font_last_scale;
		private double							get_font_last_glue;
		
		private long							get_font_offset_last_style_version;
		private ulong							get_font_offset_last_code;
		private double							get_font_offset_last_baseline_offset;
		private double							get_font_offset_last_advance_offset;
		
		private long							get_color_last_style_version;
		private ulong							get_color_last_code;
		private string							get_color_last_color;
		
		private long							get_language_last_style_version;
		private ulong							get_language_last_code;
		private Properties.LanguageProperty		get_language_last_property;

		private long							get_managed_paragraph_last_style_version;
		private int								get_managed_paragraph_last_style_index;
		private Properties.ManagedParagraphProperty get_managed_paragraph_last_property;
		
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
		private ulong							get_styles_last_style_code;
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
		
		private long							get_usertags_last_style_version;
		private ulong							get_usertags_last_code;
		private Property[]						get_usertags_last_properties;
		
		private long							get_layout_last_style_version;
		private int								get_layout_last_style_index;
		private Properties.LayoutProperty		get_layout_last_property;
		private Layout.BaseEngine				get_layout_last_engine;
		
		private long							get_margins_last_style_version;
		private ulong							get_margins_last_code;
		private Properties.MarginsProperty		get_margins_last_property;

		static Dictionary<string, List<OpenType.FontIdentity>> font_ids = new Dictionary<string, List<Epsitec.Common.OpenType.FontIdentity>> ();
		
		internal const int						SerializationVersion = 5;
	}
}

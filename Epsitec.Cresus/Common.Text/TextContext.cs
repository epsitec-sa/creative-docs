//	Copyright � 2005-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Text
{
	/// <summary>
	/// La classe TextContext d�crit un contexte (pour la d�s�rialisation) li� �
	/// un environnement 'texte'.
	/// </summary>
	public class TextContext : ILanguageRecognizer
	{
		public TextContext()
		{
			this.styleList     = new StyleList (this);
			this.tabList       = new TabList (this);
			this.layoutList    = new LayoutList (this);
			this.generatorList = new GeneratorList (this);
			this.pManagerList  = new ParagraphManagerList (this);
			this.charMarker    = new Internal.CharMarker ();
			this.conditions     = new System.Collections.Hashtable ();
			this.stories        = new System.Collections.ArrayList ();
			
			this.charMarker.Add (TextContext.DefaultMarkers.TagSelected);
			this.charMarker.Add (TextContext.DefaultMarkers.TagRequiresSpellChecking);
			this.charMarker.Add (TextContext.DefaultMarkers.TagSpellCheckingError);
			
			this.markers = new TextContext.DefaultMarkers (this.charMarker);
			
			TextContext.InitializeFontCollection (null);
			
			this.CreateDefaultLayout ();
		}
		
		
		public StyleList						StyleList
		{
			get
			{
				return this.styleList;
			}
		}
		
		public TabList							TabList
		{
			get
			{
				return this.tabList;
			}
		}
		
		public LayoutList						LayoutList
		{
			get
			{
				return this.layoutList;
			}
		}
		
		public GeneratorList					GeneratorList
		{
			get
			{
				return this.generatorList;
			}
		}
		
		public ParagraphManagerList				ParagraphManagerList
		{
			get
			{
				return this.pManagerList;
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
				return this.defaultParaStyle;
			}
			set
			{
				this.defaultParaStyle = value;
			}
		}
		
		public TextStyle						DefaultTextStyle
		{
			get
			{
				return this.defaultTextStyle;
			}
			set
			{
				this.defaultTextStyle = value;
			}
		}
		
		
		public bool								ShowControlCharacters
		{
			get
			{
				return this.showControlCharacters;
			}
			set
			{
				this.showControlCharacters = value;
			}
		}
		
		public bool								IsDegradedLayoutEnabled
		{
			get
			{
				return this.isDegradedLayoutEnabled;
			}
			set
			{
				this.isDegradedLayoutEnabled = value;
			}
		}
		
		public bool								IsPropertiesPropertyEnabled
		{
			get
			{
				return this.isPropertiesPropertyEnabled;
			}
			set
			{
				this.isPropertiesPropertyEnabled = value;
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
				return this.charMarker;
			}
		}
		
		
		public long GenerateUniqueId()
		{
			lock (this.uniqueIdLock)
			{
				return this.uniqueId++;
			}
		}
		
		
		public byte[] Serialize()
		{
			System.Text.StringBuilder buffer = new System.Text.StringBuilder ();
			
			buffer.Append ("TextContext");
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeInt (TextContext.SerializationVersion));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeLong (this.uniqueId));
			buffer.Append ("/");
			
			this.styleList.Serialize (buffer);
			buffer.Append ("/");
			
			string defaultParaStyleName = this.defaultParaStyle == null ? null : this.defaultParaStyle.Name;
			string defaultTextStyleName = this.defaultTextStyle == null ? null : this.defaultTextStyle.Name;
			
			buffer.Append (SerializerSupport.SerializeString (defaultParaStyleName));
			buffer.Append ("/");
			buffer.Append (SerializerSupport.SerializeString (defaultTextStyleName));
			buffer.Append ("/");
			
			this.tabList.Serialize (buffer);
			buffer.Append ("/");
			
			this.generatorList.Serialize (buffer);
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
			
			this.uniqueId = SerializerSupport.DeserializeLong (args[offset++]);
			
			this.styleList.Deserialize (this, version, args, ref offset);
			
			this.defaultParaStyle = null;
			this.defaultTextStyle = null;
			
			if (version >= 2)
			{
				string defaultParaStyleName = SerializerSupport.DeserializeString (args[offset++]);
				
				System.Diagnostics.Debug.Assert (defaultParaStyleName != null);
				System.Diagnostics.Debug.Assert (defaultParaStyleName.Length > 0);
				
				this.defaultParaStyle = this.styleList[defaultParaStyleName, TextStyleClass.Paragraph];
			}
			
			if (version >= 3)
			{
				string defaultTextStyleName = SerializerSupport.DeserializeString (args[offset++]);
				
				System.Diagnostics.Debug.Assert (defaultTextStyleName != null);
				System.Diagnostics.Debug.Assert (defaultTextStyleName.Length > 0);
				
				this.defaultTextStyle = this.styleList[defaultTextStyleName, TextStyleClass.Text];
			}
			
			this.tabList.Deserialize (this, version, args, ref offset);
			this.generatorList.Deserialize (this, version, args, ref offset);
			this.DeserializeConditions (version, args, ref offset);
			
			this.styleList.UpdateTabListUserCount ();
			this.styleList.UpdateGeneratorUserCount ();
			
			System.Diagnostics.Debug.Assert (args[offset] == "~");
			System.Diagnostics.Debug.Assert (args.Length == offset+1);
		}
		
		
		public bool GetGlyphAndFontForSpecialCode(ulong code, out ushort specialGlyph, out OpenType.Font specialFont)
		{
			System.Diagnostics.Debug.Assert (Unicode.Bits.GetSpecialCodeFlag (code));
			
			ulong strippedCode = Internal.CharMarker.ExtractCoreAndSettings (code);
			
			//	TODO: optimiser l'acc�s aux coreSettings, localSettings et extraSettings dans
			//	tout TextContext en utilisant SettingsTable.GetCoreAndSettings()
			
			Styles.CoreSettings  coreSettings  = this.styleList[strippedCode];
			Styles.LocalSettings localSettings = coreSettings.GetLocalSettings (strippedCode);
			Styles.ExtraSettings extraSettings = coreSettings.GetExtraSettings (strippedCode);
			
			int           glyph = -1;
			OpenType.Font font  = null;
			
			if (localSettings != null)
			{
				glyph = localSettings.GetGlyphForSpecialCode (code);
				font  = localSettings.GetFontForSpecialCode (this, code);
			}
			
			if ((glyph == -1) &&
				(extraSettings != null))
			{
				glyph = extraSettings.GetGlyphForSpecialCode (code);
				font  = extraSettings.GetFontForSpecialCode (this, code);
			}
			
			if (glyph == -1)
			{
				specialGlyph = 0xffff;
				specialFont  = null;
				
				return false;
			}
			else
			{
				specialGlyph = (ushort) glyph;
				specialFont  = font;
				
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


		public static void PostponeFullFontCollectionInitialization()
		{
			OpenType.FontCollection.FontListFilter =
				delegate (string family)
				{
					return family == Drawing.Font.DefaultFontFamily;
				};
		}
		
		public static void InitializeFontCollection(OpenType.FontIdentityCallback callback)
		{
			if (TextContext.fontCollection == null)
			{
				OpenType.FontCollection.FontListFilter = null;
				TextContext.fontCollection = OpenType.FontCollection.Default;
				TextContext.fontCollection.RefreshCache (callback);
				TextContext.fontCache = new Dictionary<string, OpenType.Font> ();
			}
		}
		
		public static string[] GetAvailableFontFaces()
		{
			TextContext.InitializeFontCollection (null);

			Dictionary<string, OpenType.FontIdentity> hash = new Dictionary<string, OpenType.FontIdentity> ();
			
			foreach (OpenType.FontIdentity id in TextContext.fontCollection)
			{
				if (hash.ContainsKey (id.InvariantFaceName) == false)
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
			
			if (TextContext.fontIds.ContainsKey (face))
			{
				return TextContext.fontIds[face].ToArray ();
			}
			else
			{
				List<OpenType.FontIdentity> list = new List<OpenType.FontIdentity> ();
				
				foreach (OpenType.FontIdentity id in TextContext.fontCollection)
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
				
				TextContext.fontIds[face] = list;
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
		
		public static OpenType.Font GetFont(string invariantFaceName, string invariantStyleName)
		{
			OpenType.Font font;
			
			TextContext.CreateOrGetFontFromCache (invariantFaceName, invariantStyleName, out font);
			
			return font;
		}
		
		
		public void GetFontAndSize(ulong code, out OpenType.Font font, out double fontSize, out double scale)
		{
			double glue;
			
			this.InternalGetFontAndSize (code, out font, out fontSize, out scale, out glue);
			
			if (Unicode.Bits.GetSpecialCodeFlag (code))
			{
				//	Ce n'est pas un caract�re normal, mais un caract�re qui doit
				//	�tre remplac� par un glyph � la vol�e. Modifie aussi la fonte
				//	si besoin.
				
				ushort        sGlyph;
				OpenType.Font sFont;
				
				this.GetGlyphAndFontForSpecialCode (code, out sGlyph, out sFont);
				
				if (sFont != null)
				{
					font = sFont;
				}
			}
		}
		
		public void GetFontAndSize(ulong code, out OpenType.Font font, out double fontSize, out double scale, out double glue)
		{
			this.InternalGetFontAndSize (code, out font, out fontSize, out scale, out glue);
			
			if (Unicode.Bits.GetSpecialCodeFlag (code))
			{
				//	Ce n'est pas un caract�re normal, mais un caract�re qui doit
				//	�tre remplac� par un glyph � la vol�e. Modifie aussi la fonte
				//	si besoin.
				
				ushort        sGlyph;
				OpenType.Font sFont;
				
				this.GetGlyphAndFontForSpecialCode (code, out sGlyph, out sFont);
				
				if (sFont != null)
				{
					font = sFont;
				}
			}
		}
		
		public void GetFont(Properties.FontProperty fontProperty, out OpenType.Font font)
		{
			if (fontProperty != null)
			{
				string fontFace  = fontProperty.FaceName;
				string fontStyle = fontProperty.StyleName;
				
				TextContext.CreateOrGetFontFromCache (fontFace, fontStyle, out font);
			}
			else
			{
				font = null;
			}
		}
		
		public void GetFont(Property[] properties, out OpenType.Font font)
		{
			Properties.FontProperty        fontProperty = null;
			Properties.FontXscriptProperty fontXscriptProperty = null;
			
			for (int i = 0; i < properties.Length; i++)
			{
				if (properties[i] != null)
				{
					switch (properties[i].WellKnownType)
					{
						case Properties.WellKnownType.Font:
							fontProperty = properties[i] as Properties.FontProperty;
							break;
						
						case Properties.WellKnownType.FontXscript:
							fontXscriptProperty = properties[i] as Properties.FontXscriptProperty;
							break;
					}
				}
			}
			
			this.GetFont (fontProperty, out font);
		}
		
		
		public void GetFontSize(ulong code, out double fontSizeInPoints)
		{
			OpenType.Font font;
			
			double fontSize;
			double scale;
			double glue;
			
			this.InternalGetFontAndSize (code, out font, out fontSize, out scale, out glue);
			
			fontSizeInPoints = fontSize * scale;
		}
		
		public void GetFontSize(Properties.FontProperty fontProperty, Properties.FontSizeProperty fontSizeProperty, Properties.FontXscriptProperty fontXscriptProperty, out double fontSize, out double fontScale, out double fontGlue)
		{
			fontSize  = fontSizeProperty.SizeInPoints;
			fontScale = (fontXscriptProperty == null) || (fontXscriptProperty.IsDisabled) ? 1.0 : fontXscriptProperty.Scale;
			fontGlue  = double.IsNaN (fontSizeProperty.Glue) ? 0 : fontSize * fontSizeProperty.Glue;
		}
		
		public void GetFontSize(Property[] properties, out double fontSize, out double fontScale, out double fontGlue)
		{
			Properties.FontProperty        fontProperty         = null;
			Properties.FontSizeProperty    fontSizeProperty     = null;
			Properties.FontXscriptProperty fontXscriptProperty  = null;
			
			for (int i = 0; i < properties.Length; i++)
			{
				if (properties[i] != null)
				{
					switch (properties[i].WellKnownType)
					{
						case Properties.WellKnownType.Font:
							fontProperty = properties[i] as Properties.FontProperty;
							break;
						
						case Properties.WellKnownType.FontSize:
							fontSizeProperty = properties[i] as Properties.FontSizeProperty;
							break;
						
						case Properties.WellKnownType.FontXscript:
							fontXscriptProperty = properties[i] as Properties.FontXscriptProperty;
							break;
					}
				}
			}
			
			this.GetFontSize (fontProperty, fontSizeProperty, fontXscriptProperty, out fontSize, out fontScale, out fontGlue);
		}
		
		
		public void GetFontBaselineOffset(double fontPtSize, Property[] properties, out double offset)
		{
			Properties.FontProperty        fontProperty         = null;
			Properties.FontOffsetProperty  fontOffsetProperty   = null;
			Properties.FontXscriptProperty fontXscriptProperty  = null;
			
			for (int i = 0; i < properties.Length; i++)
			{
				if (properties[i] != null)
				{
					switch (properties[i].WellKnownType)
					{
						case Properties.WellKnownType.Font:
							fontProperty = properties[i] as Properties.FontProperty;
							break;
						
						case Properties.WellKnownType.FontOffset:
							fontOffsetProperty = properties[i] as Properties.FontOffsetProperty;
							break;
						
						case Properties.WellKnownType.FontXscript:
							fontXscriptProperty = properties[i] as Properties.FontXscriptProperty;
							break;
					}
				}
			}
			
			offset = (fontOffsetProperty == null) ? 0 : fontOffsetProperty.GetOffsetInPoints (fontPtSize);
			
			if ((fontXscriptProperty != null) &&
				(fontXscriptProperty.IsDisabled == false))
			{
				offset += fontXscriptProperty.Offset * fontPtSize;
			}
		}
		
		public void GetFontOffsets(ulong code, out double baselineOffset, out double advanceOffset)
		{
			code = Internal.CharMarker.ExtractCoreAndSettings (code);
			
			long currentStyleVersion = this.styleList.Version;
			
			if ((this.getFontOffsetLastStyleVersion == currentStyleVersion) &&
				(this.getFontOffsetLastCode == code))
			{
				baselineOffset = this.getFontOffsetLastBaselineOffset;
				advanceOffset  = this.getFontOffsetLastAdvanceOffset;
				
				return;
			}
			
			int currentStyleIndex = Internal.CharMarker.GetCoreIndex (code);
			
			if ((this.getFontLastStyleVersion != currentStyleVersion) ||
				(this.getFontLastStyleIndex   != currentStyleIndex))
			{
				//	Rafra�chit les informations sur la fonte utilis�e :
				
				OpenType.Font font;
				double        fontSize;
				double        fontScale;
				
				this.GetFontAndSize (code, out font, out fontSize, out fontScale);
			}
			
			Styles.CoreSettings  coreSettings  = this.styleList.GetCoreFromIndex (currentStyleIndex);
			Styles.LocalSettings localSettings = coreSettings.GetLocalSettings (code);
			
			advanceOffset  = 0;
			baselineOffset = 0;
			
			Properties.FontXscriptProperty fontXscriptP = coreSettings[Properties.WellKnownType.FontXscript] as Properties.FontXscriptProperty;
			
			if (localSettings != null)
			{
				Properties.FontOffsetProperty fontOffsetP = localSettings[Properties.WellKnownType.FontOffset] as Properties.FontOffsetProperty;
				Properties.FontKernProperty   fontKernP   = localSettings[Properties.WellKnownType.FontKern] as Properties.FontKernProperty;
				
				if (fontOffsetP != null)
				{
					double ascender = this.getFontLastFont.GetAscender (this.getFontLastFontSize);
					baselineOffset = fontOffsetP.GetOffsetInPoints (ascender);
				}
				
				if (fontKernP != null)
				{
					double emSize = this.getFontLastFontSize;
					advanceOffset = fontKernP.GetOffsetInPoints (emSize);
				}
			}
			
			if ((fontXscriptP != null) &&
				(fontXscriptP.IsDisabled == false))
			{
				baselineOffset += fontXscriptP.Offset * this.getFontLastFontSize;
			}
			
			this.getFontOffsetLastStyleVersion   = currentStyleVersion;
			this.getFontOffsetLastCode            = code;
			this.getFontOffsetLastBaselineOffset = baselineOffset;
			this.getFontOffsetLastAdvanceOffset  = advanceOffset;
		}
		
		
		public void GetColor(ulong code, out string color)
		{
			code = Internal.CharMarker.ExtractCoreAndSettings (code);
			
			long currentStyleVersion = this.styleList.Version;
			
			if ((this.getColorLastStyleVersion == currentStyleVersion) &&
				(this.getColorLastCode == code))
			{
				color = this.getColorLastColor;
				
				return;
			}
			
			Styles.CoreSettings  coreSettings  = this.styleList[code];
			Styles.ExtraSettings extraSettings = coreSettings.GetExtraSettings (code);
			
			Properties.FontColorProperty colorP = extraSettings[Properties.WellKnownType.FontColor] as Properties.FontColorProperty;
			
			if (colorP == null)
			{
				color = null;
			}
			else
			{
				color = colorP.TextColor;
			}
			
			this.getColorLastStyleVersion  = currentStyleVersion;
			this.getColorLastCode          = code;
			this.getColorLastColor         = color;
		}
		
		public void GetOpenType(ulong code, out Properties.OpenTypeProperty property)
		{
			code = Internal.CharMarker.ExtractCoreAndSettings (code);
			
			Styles.CoreSettings  coreSettings  = this.styleList[code];
			Styles.LocalSettings localSettings = coreSettings.GetLocalSettings (code);
			
			if (localSettings == null)
			{
				property = null;
			}
			else
			{
				property = localSettings[Properties.WellKnownType.OpenType] as Properties.OpenTypeProperty;
			}
		}
		
		public void GetLanguage(ulong code, out Properties.LanguageProperty property)
		{
			code = Internal.CharMarker.ExtractCoreAndSettings (code);
			
			long currentStyleVersion = this.styleList.Version;
			
			if ((this.getLanguageLastStyleVersion == currentStyleVersion) &&
				(this.getLanguageLastCode == code))
			{
				property = this.getLanguageLastProperty;
				
				return;
			}
			
			Styles.CoreSettings  coreSettings  = this.styleList[code];
			Styles.ExtraSettings extraSettings = coreSettings == null ? null : coreSettings.GetExtraSettings (code);
			
			property = extraSettings == null ? null : extraSettings[Properties.WellKnownType.Language] as Properties.LanguageProperty;
			
			this.getLanguageLastStyleVersion = currentStyleVersion;
			this.getLanguageLastCode          = code;
			this.getLanguageLastProperty      = property;
		}
		
		
		public IParagraphManager GetParagraphManager(ulong code)
		{
			Properties.ManagedParagraphProperty mpp;
			
			if ((code != 0) &&
				(this.GetManagedParagraph (code, out mpp)))
			{
				return this.pManagerList[mpp.ManagerName];
			}
			else
			{
				return null;
			}
		}
		
		
		public bool GetManagedParagraph(ulong code, out Properties.ManagedParagraphProperty property)
		{
			int  currentStyleIndex   = Internal.CharMarker.GetCoreIndex (code);
			long currentStyleVersion = this.styleList.Version;
			
			if ((this.getManagedParagraphLastStyleVersion == currentStyleVersion) &&
				(this.getManagedParagraphLastStyleIndex   == currentStyleIndex))
			{
				property = this.getManagedParagraphLastProperty;
				
				return property != null;
			}
			
			Styles.CoreSettings coreSettings  = this.styleList[code];
			
			property = coreSettings[Properties.WellKnownType.ManagedParagraph] as Properties.ManagedParagraphProperty;
			
			this.getManagedParagraphLastStyleVersion = currentStyleVersion;
			this.getManagedParagraphLastStyleIndex   = currentStyleIndex;
			this.getManagedParagraphLastProperty      = property;
			
			return property != null;
		}
		
		public void GetManagedInfo(ulong code, string name, out Properties.ManagedInfoProperty property)
		{
			Styles.CoreSettings  coreSettings  = this.styleList[code];
			Styles.ExtraSettings extraSettings = coreSettings.GetExtraSettings (code);
			
			if (extraSettings == null)
			{
				property = null;
			}
			else
			{
				property = Properties.ManagedInfoProperty.Find (extraSettings.GetProperties (), name);
			}
		}
		
		
		public bool GetAutoText(ulong code, out Properties.AutoTextProperty property)
		{
			code = Internal.CharMarker.ExtractCoreAndSettings (code);
			
			long currentStyleVersion = this.styleList.Version;
			
			if ((this.getAutoTextLastStyleVersion == currentStyleVersion) &&
				(this.getAutoTextLastCode == code))
			{
				property = this.getAutoTextLastProperty;
				
				return property != null;
			}
			
			Styles.CoreSettings  coreSettings  = this.styleList[code];
			Styles.ExtraSettings extraSettings = coreSettings.GetExtraSettings (code);
			
			property = extraSettings == null ? null : extraSettings[Properties.WellKnownType.AutoText] as Properties.AutoTextProperty;
			
			this.getAutoTextLastStyleVersion = currentStyleVersion;
			this.getAutoTextLastCode          = code;
			this.getAutoTextLastProperty      = property;
			
			return property != null;
		}
		
		public bool GetGenerator(ulong code, out Properties.GeneratorProperty property)
		{
			code = Internal.CharMarker.ExtractCoreAndSettings (code);
			
			long currentStyleVersion = this.styleList.Version;
			
			if ((this.getGeneratorLastStyleVersion == currentStyleVersion) &&
				(this.getGeneratorLastCode == code))
			{
				property = this.getGeneratorLastProperty;
				
				return property != null;
			}
			
			Styles.CoreSettings  coreSettings  = this.styleList[code];
			Styles.ExtraSettings extraSettings = coreSettings.GetExtraSettings (code);
			
			property = extraSettings == null ? null : extraSettings[Properties.WellKnownType.Generator] as Properties.GeneratorProperty;
			
			this.getGeneratorLastStyleVersion  = currentStyleVersion;
			this.getGeneratorLastCode          = code;
			this.getGeneratorLastProperty      = property;
			
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
			long current_style_version = this.styleList.Version;
			
			if ((this.get_condition_last_style_version != current_style_version) ||
				(this.get_condition_last_style_index   != current_style_index))
			{
				Styles.CoreSettings core_settings = this.styleList.GetCoreFromIndex (current_style_index);
				
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
				long current_style_version = this.styleList.Version;
				
				if ((this.get_properties_last_style_version != current_style_version) ||
					(this.get_properties_last_style_index   != current_style_index))
				{
					Styles.CoreSettings core_settings = this.styleList.GetCoreFromIndex (current_style_index);
					
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
			
			long current_style_version = this.styleList.Version;
			
			if ((this.get_styles_last_style_version != current_style_version) ||
				(this.get_styles_last_style_code    != code))
			{
				Styles.CoreSettings       core_settings = this.styleList[code];
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
				if (styles[i] == this.defaultTextStyle)
				{
					count--;
				}
			}
			
			if (count != styles.Length)
			{
				TextStyle[] copy = new TextStyle[count];
				
				for (int i = 0, j = 0; i < styles.Length && j < count; i++)
				{
					if (styles[i] == this.defaultTextStyle)
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
				if ((styles[i] == this.defaultTextStyle) ||
					(styles[i].TextStyleClass == TextStyleClass.Text))
				{
					return styles;
				}
			}
			
			TextStyle[] copy = new TextStyle[styles.Length+1];
			styles.CopyTo (copy, 0);
			copy[styles.Length] = this.defaultTextStyle;
			
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
			//	Retourne les propri�t�s associ�es � un code de caract�re donn�.
			//	Les propri�t�s sont brutes, telles que vues par le syst�me de
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
			//	Retourne les propri�t�s associ�es � un code de caract�re donn�.
			//	Les propri�t�s sont brutes, telles que vues par le syst�me de
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
			
			//	Il y a � la fois des r�glages locaux et extra, il faut donc
			//	accumuler les deux :
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			list.AddRange (local.GetProperties ());
			list.AddRange (extra.GetProperties ());
			
			properties = (Property[]) list.ToArray (typeof (Property));
		}
		
		internal void GetLocalSettingsProperties(ulong code, out Property[] properties)
		{
			//	Retourne les propri�t�s associ�es � un code de caract�re donn�.
			//	Les propri�t�s sont brutes, telles que vues par le syst�me de
			//	layout, par exemple.
			//	Ne retourne que les propri�t�s de la cat�gorie LocalSettings.
			
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
			//	Retourne les propri�t�s d�finies par une collection de styles.
			//	Retourne aussi les styles sous la forme d'un tableau tri�.
			
			System.Collections.ArrayList list = new System.Collections.ArrayList ();
			
			//	Trie les styles selon leur priorit�, avant de les convertir en
			//	propri�t�s :
			
			styles = new TextStyle[text_styles.Count];
			text_styles.CopyTo (styles, 0);
			System.Array.Sort (styles, TextStyle.Comparer);
			
			//	Les diverses propri�t�s des styles pass�s en entr�e sont
			//	extraites et ajout�es dans la liste compl�te des propri�t�s :
			
			foreach (TextStyle style in styles)
			{
				//	Passe en revue toutes les propri�t�s d�finies par le style
				//	en cours d'analyse et ajoute celles-ci s�quentiellement dans
				//	la liste des propri�t�s :
				
				list.AddRange (style.GetProperties ());
			}
			
			properties = (Property[]) list.ToArray (typeof (Property));
		}
		
		internal void GetPropertiesQuickAndDirty(ulong code, out TextStyle[] styles, out Property[] properties)
		{
			//	D�termine quels styles et propri�t�s ont conduit aux propri�t�s
			//	associ�es avec le caract�re sp�cifi�. Cette m�thode se base sur
			//	un certain nombre de simplifications :
			//
			//	- Un style ne fait pas r�f�rence � des propri�t�s LocalSettings.
			//
			//	- Les propri�t�s PropertyType.CoreSettings et ExtraSettings sont
			//	  toujours enrob�es dans une m�ta-propri�t� (donc un TextStyle).
			//
			//	- TabsProperty doit �tre adapt�e (cat�gorie des ExtraSettings)
			//	  pour ne garder que les taquets locaux (TabClass.Auto).
			//
			//	Avec ces donn�es, la liste des propri�t�s se laisse reconstruire
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
					//	Ces propri�t�s ne peuvent jamais faire partie d'un style que
					//	l'utilisateur peut appliquer; elles sont g�n�r�es uniquement
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
			long current_style_version = this.styleList.Version;
			
			if ((this.get_leading_last_style_version == current_style_version) &&
				(this.get_leading_last_style_index   == current_style_index))
			{
				property = this.get_leading_last_property;
				
				return;
			}
			
			Styles.CoreSettings core_settings = this.styleList.GetCoreFromIndex (current_style_index);
			
			property = core_settings[Properties.WellKnownType.Leading] as Properties.LeadingProperty;
			
			this.get_leading_last_style_version = current_style_version;
			this.get_leading_last_style_index   = current_style_index;
			this.get_leading_last_property      = property;
		}
		
		public void GetKeep(ulong code, out Properties.KeepProperty property)
		{
			int  current_style_index   = Internal.CharMarker.GetCoreIndex (code);
			long current_style_version = this.styleList.Version;
			
			if ((this.get_keep_last_style_version == current_style_version) &&
				(this.get_keep_last_style_index   == current_style_index))
			{
				property = this.get_keep_last_property;
				
				return;
			}
			
			Styles.CoreSettings core_settings = this.styleList.GetCoreFromIndex (current_style_index);
			
			property = core_settings[Properties.WellKnownType.Keep] as Properties.KeepProperty;
			
			this.get_keep_last_style_version = current_style_version;
			this.get_keep_last_style_index   = current_style_index;
			this.get_keep_last_property      = property;
		}
		
		public void GetXlines(ulong code, out Properties.AbstractXlineProperty[] properties)
		{
			code = Internal.CharMarker.ExtractCoreAndSettings (code);
			
			long current_style_version = this.styleList.Version;
			
			if ((this.get_underlines_last_style_version != current_style_version) ||
				(this.get_underlines_last_code != code))
			{
				Styles.CoreSettings core_settings = this.styleList[code];
				
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
			
			long current_style_version = this.styleList.Version;
			
			if ((this.get_links_last_style_version != current_style_version) ||
				(this.get_links_last_code != code))
			{
				Styles.CoreSettings core_settings = this.styleList[code];
				
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
			
			long current_style_version = this.styleList.Version;
			
			if ((this.get_usertags_last_style_version != current_style_version) ||
				(this.get_usertags_last_code != code))
			{
				Styles.CoreSettings core_settings = this.styleList[code];
				
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
			long current_style_version = this.styleList.Version;
			
			if ((this.get_layout_last_style_version == current_style_version) &&
				(this.get_layout_last_style_index   == current_style_index))
			{
				engine   = this.get_layout_last_engine;
				property = this.get_layout_last_property;
				
				return;
			}
			
			Styles.CoreSettings core_settings = this.styleList.GetCoreFromIndex (current_style_index);
			
			property = core_settings[Properties.WellKnownType.Layout] as Properties.LayoutProperty;
			
			if (property == null)
			{
				engine = null;
			}
			else
			{
				engine = this.layoutList[property.EngineName];
			}
			
			if (engine == null)
			{
				engine = this.layoutList["*"];
			}
			
			this.get_layout_last_style_version = current_style_version;
			this.get_layout_last_style_index   = current_style_index;
			this.get_layout_last_property      = property;
			this.get_layout_last_engine        = engine;
		}
		
		public void GetMargins(ulong code, out Properties.MarginsProperty property)
		{
			code = Internal.CharMarker.ExtractCoreAndSettings (code);
			
			long current_style_version = this.styleList.Version;
			
			if ((this.get_margins_last_style_version == current_style_version) &&
				(this.get_margins_last_code == code))
			{
				property = this.get_margins_last_property;
				
				return;
			}
			
			Styles.CoreSettings core_settings = this.styleList[code];
			
			property = core_settings[Properties.WellKnownType.Margins] as Properties.MarginsProperty;
			
			this.get_margins_last_style_version = current_style_version;
			this.get_margins_last_code          = code;
			this.get_margins_last_property      = property;
		}
		
		public void GetBreak(ulong code, out Properties.BreakProperty property)
		{
			code = Internal.CharMarker.ExtractCoreAndSettings (code);
			
			Styles.CoreSettings  core_settings  = this.styleList[code];
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
			
			Styles.CoreSettings  core_settings  = this.styleList[code];
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
			
			Styles.CoreSettings  core_settings  = this.styleList[code];
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
			
			Styles.CoreSettings  core_settings  = this.styleList[code];
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
			
			Styles.CoreSettings  core_settings  = this.styleList[stripped_code];
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
			//	Trouve le d�but du texte marqu� avec la propri�t� indiqu�e; retourne
			//	la distance parcourue (-1 en cas d'erreur).
			//
			//	Si le curseur se trouve au d�but du texte marqu�, retourne 0.
			
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
			//	Trouve la fin du texte marqu� avec la propri�t� indiqu�e; retourne
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
				
				Styles.CoreSettings core_settings = this.styleList[code];
				
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
				
				Styles.CoreSettings core_settings = this.styleList[code];
				
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
				
				Styles.CoreSettings core_settings = this.context.styleList[code];
				
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
			this.layoutList.NewEngine ("*", typeof (Layout.LineEngine));
		}
		
		private static void CreateOrGetFontFromCache(string fontFace, string font_style, out OpenType.Font font)
		{
			TextContext.InitializeFontCollection (null);

			string fullName = OpenType.FontName.GetFullName (fontFace, font_style);
			
			lock (TextContext.fontCache)
			{
				if (TextContext.fontCache.TryGetValue (fullName, out font) == false)
				{
					font = TextContext.fontCollection.CreateFont (fontFace, font_style);
					TextContext.fontCache[fullName] = font;
				}
			}
		}
		
		private void InternalGetFontAndSize(ulong code, out OpenType.Font font, out double font_size, out double scale, out double glue)
		{
			int  current_style_index   = Internal.CharMarker.GetCoreIndex (code);
			long current_style_version = this.styleList.Version;
			
			if ((this.getFontLastStyleVersion == current_style_version) &&
				(this.getFontLastStyleIndex   == current_style_index))
			{
				font      = this.getFontLastFont;
				font_size = this.getFontLastFontSize;
				scale     = this.get_font_last_scale;
				glue      = this.get_font_last_glue;
				
				return;
			}
			
			Styles.CoreSettings core_settings = this.styleList.GetCoreFromIndex (current_style_index);

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
			
			this.getFontLastStyleVersion = current_style_version;
			this.getFontLastStyleIndex   = current_style_index;
			this.getFontLastFont          = font;
			this.get_font_last_scale         = scale;
			this.getFontLastFontSize     = font_size;
			this.get_font_last_glue          = glue;
		}
		
		
		
		private StyleList						styleList;
		private TabList							tabList;
		private LayoutList						layoutList;
		private GeneratorList					generatorList;
		private ParagraphManagerList			pManagerList;
		private Internal.CharMarker				charMarker;
		private DefaultMarkers					markers;
		private System.Collections.Hashtable	conditions;
		private System.Collections.ArrayList	stories;
		private TextStyle						defaultParaStyle;
		private TextStyle						defaultTextStyle;
		
		private long							uniqueId = 1;
		private object							uniqueIdLock = new object ();
		
		private bool							showControlCharacters;
		private bool							isDegradedLayoutEnabled;
		private bool							isPropertiesPropertyEnabled = true;
		
		static OpenType.FontCollection			fontCollection;
		static Dictionary<string, OpenType.Font> fontCache;
		
		private System.Collections.Hashtable	resources;
		
		private long							getFontLastStyleVersion;
		private int								getFontLastStyleIndex;
		private OpenType.Font					getFontLastFont;
		private double							getFontLastFontSize;
		private double							get_font_last_scale;
		private double							get_font_last_glue;
		
		private long							getFontOffsetLastStyleVersion;
		private ulong							getFontOffsetLastCode;
		private double							getFontOffsetLastBaselineOffset;
		private double							getFontOffsetLastAdvanceOffset;
		
		private long							getColorLastStyleVersion;
		private ulong							getColorLastCode;
		private string							getColorLastColor;
		
		private long							getLanguageLastStyleVersion;
		private ulong							getLanguageLastCode;
		private Properties.LanguageProperty		getLanguageLastProperty;

		private long							getManagedParagraphLastStyleVersion;
		private int								getManagedParagraphLastStyleIndex;
		private Properties.ManagedParagraphProperty getManagedParagraphLastProperty;
		
		private long							getAutoTextLastStyleVersion;
		private ulong							getAutoTextLastCode;
		private Properties.AutoTextProperty		getAutoTextLastProperty;
		
		private long							getGeneratorLastStyleVersion;
		private ulong							getGeneratorLastCode;
		private Properties.GeneratorProperty	getGeneratorLastProperty;
		
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

		static Dictionary<string, List<OpenType.FontIdentity>> fontIds = new Dictionary<string, List<Epsitec.Common.OpenType.FontIdentity>> ();
		
		internal const int						SerializationVersion = 5;
	}
}

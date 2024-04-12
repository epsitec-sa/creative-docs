//	Copyright © 2005-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.OpenType
{
    /// <summary>
    /// The <c>FontIdentity</c> class provides detailed name information about
    /// a font.
    /// </summary>
    public sealed class FontIdentity
    {
        internal FontIdentity(string fontFilePath, FontName fontName) { 
            this.filePath = fontFilePath;
            this.name = fontName;
        }

        public string Name
        {
            get
            {
                return this.name.FaceName;
            }
        }

        /// <summary>
        /// Gets the font face name for the current locale, using
        /// <c>CultureInfo.CurrentCulture</c>.
        /// This will return a name like "Franklin Gothic" for
        /// "Franklin Gothic Heavy Italic".
        /// </summary>
        /// <value>The font face name.</value>
        public string LocaleFaceName
        {
            get
            {
                /*
                lock (this.exclusion)
                {
                    string face = this.LocalePreferredFaceName ?? this.LocaleSimpleFaceName;
                    string style = this.LocalePreferredStyleName ?? this.LocaleSimpleStyleName;

                    face = FontIdentity.RepairBrokenFaceName(this.FullName, face);

                    if (face != null)
                    {
                        if (face.EndsWith(style))
                        {
                            face = face.Substring(0, face.Length - style.Length).Trim();
                        }
                    }

                    return face;
                }
                */
                return this.Name;
            }
        }

        /// <summary>
        /// Gets the font style name for the current locale, using
        /// <c>CultureInfo.CurrentCulture</c>.
        /// This will return a name like "FHeavy Italique" for
        /// "Franklin Gothic Heavy Italic".
        /// </summary>
        /// <value>The font style name.</value>
        public string LocaleStyleName
        {
            get
            {
                /*
                string preferred = this.LocalePreferredStyleName;
                string simple = this.LocaleSimpleStyleName;
                string adobe = this.LocaleAdobeStyleName;

                string localeName = FontIdentity.ComposeStyleName(preferred, simple, adobe);

                preferred = this.InvariantPreferredStyleName;
                simple = this.InvariantSimpleStyleName;
                adobe = this.InvariantAdobeStyleName;

                string invariantName = FontIdentity.ComposeStyleName(preferred, simple, adobe);

                if (localeName == invariantName)
                {
                    return this.InvariantStyleName;
                }
                else
                {
                    string full = this.LocaleFullName;

                    if ((localeName != null) && (full != null))
                    {
                        localeName = FontIdentity.RepairBrokenStyleName(full, localeName);
                    }

                    return localeName;
                }
                */
                return this.name.StyleName;
            }
        }

        public string LocaleFullName
        {
            get
            {
                /*
                lock (this.exclusion)
                {
                    return this.GetName(NameId.FullFontName);
                }
                */
                return this.FullName;
            }
        }

        /// <summary>
        /// Gets the font face name for the current locale, using
        /// <c>CultureInfo.CurrentCulture</c>.
        /// This will return a name like "Franklin Gothic Heavy" for
        /// "Franklin Gothic Heavy Italic".
        /// </summary>
        /// <value>The font face name.</value>
        public string LocaleSimpleFaceName
        {
            get
            {
                /*
                lock (this.exclusion)
                {
                    string face = this.GetName(NameId.FontFamily);
                    string style = this.GetName(NameId.FontSubfamily);

                    if (face.EndsWith(style))
                    {
                        face = face.Substring(0, face.Length - style.Length).Trim();
                    }

                    return face;
                }
                */
                return this.Name;
            }
        }

        /// <summary>
        /// Gets the font style name for the current locale, using
        /// <c>CultureInfo.CurrentCulture</c>.
        /// This will return a name like "Italique" for
        /// "Franklin Gothic Heavy Italic".
        /// </summary>
        /// <value>The font style name.</value>
        public string LocaleSimpleStyleName
        {
            get
            {
                /*
                lock (this.exclusion)
                {
                    return this.GetName(NameId.FontSubfamily);
                }
                */
                return this.name.StyleName;
            }
        }

        /// <summary>
        /// Gets the Adobe font style name for the current locale, using
        /// <c>CultureInfo.CurrentCulture</c>.
        /// This will return null for "Franklin Gothic Heavy Italic".
        /// </summary>
        /// <value>The Adobe font style name.</value>
        public string LocaleAdobeStyleName
        {
            get
            {
                /*
                lock (this.exclusion)
                {
                    return this.GetName(NameId.AdobeFontStyle);
                }
                */
                return this.name.StyleName;
            }
        }

        /// <summary>
        /// Gets the preferred font face name for the current locale, using
        /// <c>CultureInfo.CurrentCulture</c>.
        /// This will return null for "Franklin Gothic Heavy Italic".
        /// </summary>
        /// <value>The preferred font face name or <c>null</c>.</value>
        public string LocalePreferredFaceName
        {
            get
            {
                /*
                lock (this.exclusion)
                {
                    return this.GetName(NameId.PreferredFamily);
                }
                */
                return this.Name;
            }
        }

        /// <summary>
        /// Gets the preferred font style name for the current locale, using
        /// <c>CultureInfo.CurrentCulture</c>.
        /// This will return null for "Franklin Gothic Heavy Italic".
        /// </summary>
        /// <value>The font style name or <c>null</c>.</value>
        public string LocalePreferredStyleName
        {
            get
            {
                /*
                lock (this.exclusion)
                {
                    return this.GetName(NameId.PreferredSubfamily);
                }
                */
                return this.name.StyleName;
            }
        }

        /// <summary>
        /// Gets the invariant font face name. This name is independent
        /// of the current culture. This will return a name like "Arial"
        /// for "Arial Narrow Bold Italic".
        /// This will return a name like "Franklin Gothic" for
        /// "Franklin Gothic Heavy Italic".
        /// </summary>
        /// <value>The font face name.</value>
        public string InvariantFaceName
        {
            get
            {
                /*
                string face =
                    this.InvariantPreferredFaceName
                    ?? this.MacintoshFaceName
                    ?? this.InvariantSimpleFaceName;
                string style =
                    this.InvariantPreferredStyleName
                    ?? this.MacintoshStyleName
                    ?? this.InvariantSimpleStyleName;

                face = FontIdentity.RepairBrokenFaceName(this.FullName, face);

                if (face != null)
                {
                    if (face.EndsWith(style))
                    {
                        face = face.Substring(0, face.Length - style.Length).Trim();
                    }
                }

                return face;
                */
                return this.Name;
            }
        }

        /// <summary>
        /// Gets the invariant font style name. This name is independent
        /// of the current culture. This will return a name like "Narrow
        /// Bold Italic" for "Arial Narrow Bold Italic".
        /// This will return a name like "Heavy Italic" for
        /// "Franklin Gothic Heavy Italic".
        /// </summary>
        /// <value>The font style name.</value>
        public string InvariantStyleName
        {
            get
            {
                /*
                string preferred = this.InvariantPreferredStyleName;
                string simple = this.InvariantSimpleStyleName;
                string adobe = this.InvariantAdobeStyleName;

                string name = FontIdentity.ComposeStyleName(preferred, simple, adobe);
                string full = this.FullName;

                if ((name != null) && (full != null))
                {
                    name = FontIdentity.RepairBrokenStyleName(full, name);
                }

                return name;
                */
                return this.name.StyleName;
            }
        }

        /// <summary>
        /// Gets the invariant font full name. This name is independent
        /// of the current culture.
        /// This will return a name like "Franklin Gothic Heavy Italic"
        /// for "Franklin Gothic Heavy Italic".
        /// </summary>
        /// <value>The font full name.</value>
        public string InvariantFullName
        {
            get
            {
                /*
                lock (this.exclusion)
                {
                    return this.GetName(NameId.FullFontName, FontIdentity.InvariantLocale);
                }
                */
                return this.FullName;
            }
        }

        /// <summary>
        /// Gets the invariant font face name. This name is independent
        /// of the current culture. This will return a name like "Arial
        /// Narrow" for "Arial Narrow Bold Italic".
        /// This will return a name like "Franklin Gothic Heavy" for
        /// "Franklin Gothic Heavy Italic".
        /// </summary>
        /// <value>The font face name.</value>
        public string InvariantSimpleFaceName
        {
            get
            {
                /*
                lock (this.exclusion)
                {
                    string face = this.GetName(NameId.FontFamily, FontIdentity.InvariantLocale);
                    string style = this.GetName(NameId.FontSubfamily, FontIdentity.InvariantLocale);

                    if (face.EndsWith(style))
                    {
                        face = face.Substring(0, face.Length - style.Length).Trim();
                    }

                    return face;
                }
                */
                return this.Name;
            }
        }

        /// <summary>
        /// Gets the invariant font style name. This name is independent
        /// of the current culture. This will return a name like "Bold
        /// Italic" for "Arial Narrow Bold Italic".
        /// This will return a name like "Italic" for
        /// "Franklin Gothic Heavy Italic".
        /// </summary>
        /// <value>The font style name.</value>
        public string InvariantSimpleStyleName
        {
            get
            {
                /*
                lock (this.exclusion)
                {
                    return this.GetName(NameId.FontSubfamily, FontIdentity.InvariantLocale);
                }
                */
                return this.name.StyleName;
            }
        }

        /// <summary>
        /// Gets the invariant Adobe font style name. This name is independent
        /// of the current culture.
        /// This will return null for "Franklin Gothic Heavy Italic".
        /// </summary>
        /// <value>The Adobe font style name.</value>
        public string InvariantAdobeStyleName
        {
            get
            {
                /*
                lock (this.exclusion)
                {
                    return this.GetName(NameId.AdobeFontStyle, FontIdentity.InvariantLocale);
                }
                */
                return this.name.StyleName;
            }
        }

        /// <summary>
        /// Gets the Macintosh font face name. This name is independent
        /// of the current culture.
        /// This will return a name like "Franklin Gothic Heavy" for
        /// "Franklin Gothic Heavy Italic".
        /// </summary>
        /// <value>The font face name.</value>
        public string MacintoshFaceName
        {
            get
            {
                /*
                lock (this.exclusion)
                {
                    string face = this.GetMacName(NameId.FontFamily);
                    string style = this.GetMacName(NameId.FontSubfamily);

                    if ((face != null) && (style != null) && (face.EndsWith(style)))
                    {
                        face = face.Substring(0, face.Length - style.Length).Trim();
                    }

                    return face;
                }
                */
                return this.Name;
            }
        }

        /// <summary>
        /// Gets the Macintosh font style name. This name is independent
        /// of the current culture.
        /// This will return a name like "Italic" for
        /// "Franklin Gothic Heavy Italic".
        /// </summary>
        /// <value>The font style name.</value>
        public string MacintoshStyleName
        {
            get
            {
                /*
                lock (this.exclusion)
                {
                    return this.GetMacName(NameId.FontSubfamily);
                }
                */
                return this.name.StyleName;
            }
        }

        /// <summary>
        /// Gets the invariant preferred font face name. This name is independent
        /// of the current culture.
        /// This will return null for "Franklin Gothic Heavy Italic".
        /// </summary>
        /// <value>The preferred font face name or <c>null</c> if there is no
        /// preferred font face associated with this font.</value>
        public string InvariantPreferredFaceName
        {
            get
            {
                /*
                lock (this.exclusion)
                {
                    return this.GetName(NameId.PreferredFamily, FontIdentity.InvariantLocale);
                }
                */
                return this.Name;
            }
        }

        /// <summary>
        /// Gets the invariant preferred font style name. This name is independent
        /// of the current culture.
        /// This will return null for "Franklin Gothic Heavy Italic".
        /// </summary>
        /// <value>The preferred font style name or <c>null</c> if there is no
        /// preferred font style associated with this font.</value>
        public string InvariantPreferredStyleName
        {
            get
            {
                /*
                lock (this.exclusion)
                {
                    return this.GetName(NameId.PreferredSubfamily, FontIdentity.InvariantLocale);
                }
                */
                return this.name.StyleName;
            }
        }

        /// <summary>
        /// Gets a simplified version of the invariant font style name. The
        /// names <c>"Regular"</c> and <c>"Normal"</c> are mapped to <c>""</c>.
        /// This will return a name like "Heavy Italic" for
        /// "Franklin Gothic Heavy Italic".
        /// </summary>
        /// <value>The simplified font style name.</value>
        public string InvariantStyleHash
        {
            get
            {
                /*
                if (this.styleHash == null)
                {
                    lock (this.exclusion)
                    {
                        if (this.styleHash == null)
                        {
                            this.styleHash = FontCollection.GetStyleHash(this.InvariantStyleName);
                        }
                    }
                }

                return this.styleHash;
                */
                return this.name.StyleName;
            }
        }

        /// <summary>
        /// Gets a simplified version of the full font name. The names <c>"Regular"</c>
        /// and <c>"Normal"</c> are mapped to <c>""</c> and all elements are sorted
        /// alphabetically.
        /// This will return a name like "Franklin Gothic Heavy Italic" for
        /// "Franklin Gothic Heavy Italic".
        /// </summary>
        /// <value>The simplified version of the full font name.</value>
        public string FullHash
        {
            get
            {
                /*
                if (this.fullHash == null)
                {
                    lock (this.exclusion)
                    {
                        if (this.fullHash == null)
                        {
                            this.fullHash = FontName.GetFullHash(this.FullName);
                        }
                    }
                }

                return this.fullHash;
                */
                return this.FullName;
            }
        }

        /// <summary>
        /// Gets the number of font styles available for the font face
        /// described by this <c>FontIdentity</c> object.
        /// </summary>
        /// <value>The number of font styles.</value>
        public int FontStyleCount
        {
            get { 
                /*
                return this.fontStyleCount; 
                */
                throw new System.NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the full name of the font. This is the OpenType
        /// This will return a name like "Franklin Gothic Heavy Italic" for
        /// "Franklin Gothic Heavy Italic".
        /// </summary>
        public string FullName
        {
            get
            {
                return this.name.FullName;
            }
        }

        /// <summary>
        /// Gets the font unique id. This name is independent
        /// of the current culture.
        /// This will return a name like "Monotype - Franklin Gothic Heavy Italic"
        /// for "Franklin Gothic Heavy Italic".
        /// </summary>
        /// <value>The font unique id.</value>
        public string UniqueFontId
        {
            get {
                return this.FullName;
            }
        }

        public byte[] AssociatedBlob1
        {
            get
            {
                /*
                if (this.blob1 == null)
                {
                    return new byte[0];
                }
                else
                {
                    return this.blob1;
                }
                */
                throw new System.NotImplementedException();
            }
            set { 
                /*
                this.blob1 = value; 
                */
                throw new System.NotImplementedException();
            }
        }

        public byte[] AssociatedBlob2
        {
            get
            {
                /*
                if (this.blob2 == null)
                {
                    return new byte[0];
                }
                else
                {
                    return this.blob2;
                }
                */
                throw new System.NotImplementedException();
            }

            set { 
                /*
                this.blob2 = value; 
                */
                throw new System.NotImplementedException();
            }
        }

        public object DrawingFont
        {
            get {
                /*
                return this.drawingFont;
                */
                throw new System.NotImplementedException();
            }
            set {
                /*
                this.drawingFont = value;
                */
                throw new System.NotImplementedException();
            }
        }

        public FontWeight FontWeight
        {
            get {
                /*
                
                return (FontWeight)Platform.Neutral.GetFontWeight(this.Record);
            
                */
                throw new System.NotImplementedException();
            }
        }

        public FontStyle FontStyle
        {
            get
            {
                /*
                if (Platform.Neutral.GetFontItalic(this.Record) == 0)
                {
                    return FontStyle.Normal;
                }

                string name = this.InvariantStyleName.ToLower(
                    System.Globalization.CultureInfo.InvariantCulture
                );

                if (
                    (name.IndexOf("italic") != -1)
                    || (name.IndexOf("cursive") != -1)
                    || (name.IndexOf("kursiv") != -1)
                )
                {
                    return FontStyle.Italic;
                }
                else
                {
                    return FontStyle.Oblique;
                }
                */
                return this.name.Style;
            }
        }

        public bool IsSymbolFont
        {
            get
            {
                /*
                if (this.isSymbolFontDefined == false)
                {
                    lock (this.exclusion)
                    {
                        if (this.isSymbolFontDefined == false)
                        {
                            TableCmap cmap = this.InternalGetTableCmap();

                            this.isSymbolFont = cmap.FindFormatSubTable(3, 0, 4) != null;
                            this.isSymbolFontDefined = true;
                        }
                    }
                }

                return this.isSymbolFont;
                */
                return false;
            }
        }

        public bool IsDynamicFont
        {
            get {
            /*
            return this.isDynamicFont;
            */
            throw new System.NotImplementedException();
            }
        }

        public static IComparer<FontIdentity> Comparer
        {
            get {
                /*
                
                return new FontComparer();
            
                */
                throw new System.NotImplementedException();
            }
        }

        public static event FontIdentityCallback Serializing;

        /// <summary>
        /// Gets the name of the specified glyph.
        /// </summary>
        /// <param name="glyph">The glyph index.</param>
        /// <returns>The name of the glyph or <c>null</c> if the glyph is not
        /// supported in the font.</returns>
        public string GetGlyphName(int glyph)
        {
            /*
            TableEntry entry = this.FontData["post"];
            TablePost post = entry == null ? null : new TablePost(entry);

            if (post != null)
            {
                return post.GetGlyphName(glyph);
            }
            else
            {
                return null;
            }
            */
            throw new System.NotImplementedException();
        }

        public void InternalClearFontData()
        {
            /*
            this.fontData = null;

            if (this.fontSizes != null)
            {
                List<FontSizeInfo> sizeInfos = new List<FontSizeInfo>(this.fontSizes.Values);

                foreach (FontSizeInfo sizeInfo in sizeInfos)
                {
                    sizeInfo.Dispose();
                }

                this.fontSizes.Clear();
            }
            */
            throw new System.NotImplementedException();
        }

        private string filePath;
        private FontName name;
    }
}

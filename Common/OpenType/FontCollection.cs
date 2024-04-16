//	Copyright © 2005-2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Reflection.Metadata.Ecma335;

namespace Epsitec.Common.OpenType
{
    public delegate void FontIdentityCallback(FontIdentity fid);

    /// <summary>
    /// The <c>FontCollection</c> class manages the collection of available
    /// fonts.
    /// </summary>
    public sealed class FontCollection : IEnumerable<FontIdentity>
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FontCollection"/> class.
        /// </summary>
        public FontCollection()
        {
            /*
            this.fontDict = new Dictionary<string, FontIdentity>();
            this.fullDict = new Dictionary<string, FontIdentity>();
            this.fuidDict = new Dictionary<string, FontIdentity>();
            this.fullList = new List<FontIdentity>();
            */
            this.fontDict = new Dictionary<string, FontIdentity>();
            this.fallbackFontIdentity = null;
        }

        /// <summary>
        /// Gets the <see cref="FontIdentity"/> with the specified name.
        /// </summary>
        /// <value>The <see cref="FontIdentity"/> or <c>null</c> if it does
        /// not exist in the collection.</value>
        public FontIdentity this[string name]
        {
            get
            {
                if (this.fontDict.ContainsKey(name)) { 
                    return this.fontDict[name]; 
                }
                return null;
            }
        }

        /// <summary>
        /// Gets the <see cref="FontIdentity"/> with the specified name.
        /// </summary>
        /// <value>The <see cref="FontIdentity"/> or <c>null</c> if it does
        /// not exist in the collection.</value>
        public FontIdentity this[FontName name]
        {
            get
            {
                return this[name.FullName];
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether to load True Type Collections
        /// (TTC files).
        /// </summary>
        /// <value>
        ///		<c>true</c> if True Type Collections should be loaded; otherwise, <c>false</c>.
        /// </value>
        public static bool LoadTrueTypeCollections
        {
            get {
                /*
                
                return FontCollection.loadTtc;
            
                */
                throw new System.NotImplementedException();
            }
            set {
                /*
                
                FontCollection.loadTtc = value;
            
                */
                throw new System.NotImplementedException();
            }
        }

        /// <summary>
        /// Gets the default font collection object.
        /// </summary>
        /// <value>The default font collection object.</value>
        public static FontCollection Default
        {
            get
            {
                FontCollection.defaultCollection ??= new FontCollection();
                return FontCollection.defaultCollection;
            }
        }

        /// <summary>
        /// Gets or sets the filter used when listing the system fonts.
        /// </summary>
        public static System.Predicate<string> FontListFilter
        {
            get {
                return FontCollection.fontListFilter;
            }
            set {
                FontCollection.fontListFilter = value;
            }
        }

        /// <summary>
        /// Initializes this font collection object. If font identities have
        /// already been loaded by <see cref="LoadFromCache"/>, they will be
        /// updated.
        /// </summary>
        /// <returns><c>true</c> if the font collection has changed; otherwise, <c>false</c>.</returns>
        public void Initialize(IEnumerable<FontIdentity> fontIdentities)
        {
            foreach (FontIdentity fontIdentity in fontIdentities)
            {
                this.fontDict[fontIdentity.Name] = fontIdentity;
                this.fallbackFontIdentity ??= fontIdentity;
            }
        }

        /// <summary>
        /// Refreshes the cache.
        /// </summary>
        /// <returns><c>true</c> if the contents of the cache changed; otherwise, <c>false</c>.</returns>
        public bool RefreshCache()
        {
            return this.RefreshCache(null);
        }

        /// <summary>
        /// Refreshes the cache.
        /// </summary>
        /// <param name="callback">A callback called on every saved font identity.</param>
        /// <returns><c>true</c> if the contents of the cache changed; otherwise, <c>false</c>.</returns>
        public bool RefreshCache(FontIdentityCallback callback)
        {
            /*
            lock (this.localExclusion)
            {
                this.LockedLoadFromCache();

                if (this.LockedInitialize(callback))
                {
                    return this.LockedSaveToCache(callback);
                }
                else
                {
                    return false;
                }
            }
            */
            return false;
        }

        /// <summary>
        /// Saves the current state of the font collection to the disk cache.
        /// </summary>
        /// <returns><c>true</c> if the cache could be written; otherwise, <c>false</c>.</returns>
        public bool SaveToCache()
        {
            return this.SaveToCache(null);
        }

        /// <summary>
        /// Saves the current state of the font collection to the disk cache.
        /// </summary>
        /// <param name="callback">A callback called on every saved font identity.</param>
        /// <returns><c>true</c> if the cache could be written; otherwise, <c>false</c>.</returns>
        public bool SaveToCache(FontIdentityCallback callback)
        {
            /*
            lock (this.localExclusion)
            {
                return this.LockedSaveToCache(callback);
            }
            */
            return true;
        }

        /// <summary>
        /// Loads the font identity information from the disk cache.
        /// </summary>
        public void LoadFromCache()
        {
            /*
            lock (this.localExclusion)
            {
                this.LockedLoadFromCache();
            }
            */
        }

        /// <summary>
        /// Enumerates the font families.
        /// </summary>
        /// <returns></returns>
        public IEnumerable<string> GetFontFamilies()
        {
            /*
            lock (this.localExclusion)
            {
                return (string[])this.families.Clone();
            }
            */
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the <see cref="FontIdentity"/> with the specified unique font
        /// identifier.
        /// </summary>
        /// <returns>The <see cref="FontIdentity"/> or <c>null</c> if it does
        /// not exist in the collection.</returns>
        public FontIdentity FindFontByUniqueFontIdentifier(string fuid)
        {
            /*
            lock (this.localExclusion)
            {
                FontIdentity value;

                if (this.fuidDict.TryGetValue(fuid, out value))
                {
                    return value;
                }
                else
                {
                    return null;
                }
            }
            */
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Creates the font object based on the font face and font style.
        /// </summary>
        /// <param name="face">The font face.</param>
        /// <param name="style">The font style.</param>
        /// <returns>The font object or <c>null</c> if no font can be found.</returns>
        public Font CreateFont(string face, string style)
        {
            FontName fontName = new FontName(face, style);
            FontIdentity fid = this[fontName];

            var fallbacks = fontName.EnumerateWithFallbackStyles();
            foreach (FontName fallback in fallbacks)
            {
                fid ??= this[fallback];
            }
            fid ??= this.fallbackFontIdentity;
            if (fid == null)
            {
                throw new FontNotFoundException();
            }
            return this.CreateFont(fid);
        }

        /// <summary>
        /// Creates the font object based on the full font name.
        /// </summary>
        /// <param name="font">The full font name.</param>
        /// <returns>The font object or <c>null</c> if no font can be found.</returns>
        public Font CreateFont(string font)
        {
            return this.CreateFont(font, "");
        }

        /// <summary>
        /// Creates the font object based on the font identity.
        /// </summary>
        /// <param name="font">The font identity.</param>
        /// <returns>The font object or <c>null</c> if no font can be found.</returns>
        public Font CreateFont(FontIdentity fid)
        {
            return new Font(fid);
        }

        /// <summary>
        /// Registers the font as a dynamic font.
        /// </summary>
        /// <param name="data">The font data.</param>
        /// <returns>the font identity of the newly registered font; otherwise, <c>null</c>.</returns>
        public FontIdentity RegisterDynamicFont(byte[] data)
        {
            /*
            FontIdentity fid = FontIdentity.CreateDynamicFont(data);

            int nameTOffset = fid.FontData["name"].Offset;
            int nameTLength = fid.FontData["name"].Length;

            fid.DefineTableName(new TableName(data, nameTOffset), nameTLength);

            if (this.fullDict.ContainsKey(fid.FullName))
            {
                return null;
            }

            this.Add(fid);
            this.RefreshFullList();

            return fid;
            */
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the style hash, which is a simplified version of the style
        /// name.
        /// </summary>
        /// <param name="style">The raw style name.</param>
        /// <returns>The hashed style name.</returns>
        public static string GetStyleHash(string style)
        {
            //	Le "hash" d'un style de fonte correspond à une forme simplifiée
            //	et triée des éléments constituant un nom de style. On évite des
            //	problèmes de comparaison liés à des permutations, etc.

            //	En plus, le nom de style peut contenir des éléments "*Xyz" où "x"
            //	peut être "+", "-" ou "!" pour ajouter, supprimer ou inverser un
            //	style tel que "Bold" ou "Italic".

            if (string.IsNullOrEmpty(style))
            {
                return style;
            }

            string[] parts = style.Split(' ');

            int bold = 0;
            int italic = 0;

            List<string> list = new List<string>();

            foreach (string part in parts)
            {
                if (part.Length > 0)
                {
                    switch (part)
                    {
                        case "Regular":
                            break;
                        case "Normal":
                            break;
                        case "Roman":
                            break;

                        case "Bold":
                            bold = 1;
                            break;
                        case "+Bold":
                            bold += 1;
                            break;
                        case "-Bold":
                            bold -= 1;
                            break;

                        case "Italic":
                            italic = 1;
                            break;
                        case "+Italic":
                            italic += 1;
                            break;
                        case "-Italic":
                            italic -= 1;
                            break;

                        case "!Bold":
                            bold = (bold > 0) ? bold - 1 : bold + 1;
                            break;
                        case "!Italic":
                            italic = (italic > 0) ? italic - 1 : italic + 1;
                            break;

                        default:
                            if (list.Contains(part) == false)
                            {
                                list.Add(part);
                            }
                            break;
                    }
                }
            }

            if (bold > 0)
            {
                list.Add("Bold");
            }
            if (italic > 0)
            {
                list.Add("Italic");
            }

            list.Sort();

            return string.Join(" ", list.ToArray());
        }

        public string DebugGetFullFontList()
        {
            /*
            List<string> lines = new List<string>();

            foreach (FontIdentity id in this)
            {
                TableName name = id.OpenTypeTableName;
                TableName.NameEncoding[] encodings = name.GetAvailableNameEncodings();

                lines.Add("-----------------------------");
                lines.AddRange(
                    string.Format(
                            "{0,-32}Style={1} Weight={2} Count={3}\n"
                                + "                                Invariant={4} / {5}\n"
                                + "                                Invariant Preferred={6} / {7}\n"
                                + "                                Invariant Hash={8}\n"
                                + "                                Locale={9} / {10}\n"
                                + "                                Locale Preferred={11} / {12}\n"
                                + "                                UniqueFontId={13}, entries={14}",
                            id.FullName,
                            id.FontStyle,
                            id.FontWeight,
                            id.FontStyleCount,
                            id.InvariantSimpleFaceName,
                            id.InvariantSimpleStyleName,
                            id.InvariantPreferredFaceName,
                            id.InvariantPreferredStyleName,
                            id.InvariantStyleHash,
                            id.LocaleSimpleFaceName,
                            id.LocaleSimpleStyleName,
                            id.LocalePreferredFaceName,
                            id.LocalePreferredStyleName,
                            id.UniqueFontId,
                            encodings.Length
                        )
                        .Split('\n')
                );

                lines.Add(
                    string.Concat(
                        ">>> ",
                        id.FullName,
                        "/",
                        id.InvariantFaceName,
                        "+",
                        id.InvariantStyleName,
                        "/",
                        id.LocaleFaceName,
                        "+",
                        id.LocaleStyleName
                    )
                );

                for (int i = 0; i < encodings.Length; i++)
                {
                    string unicode =
                        encodings[i].Platform == PlatformId.Microsoft
                            ? name.GetUnicodeName(
                                encodings[i].Language,
                                encodings[i].Name,
                                encodings[i].Platform
                            )
                            : null;
                    string latin =
                        encodings[i].Platform == PlatformId.Macintosh
                            ? name.GetLatinName(
                                encodings[i].Language,
                                encodings[i].Name,
                                encodings[i].Platform
                            )
                            : null;

                    lines.Add(
                        string.Format(
                            "{0,-4} {1,-24} {2,-10} : {3}",
                            encodings[i].Language,
                            encodings[i].Name,
                            encodings[i].Platform,
                            (unicode ?? latin ?? "").Split('\n')[0]
                        )
                    );
                }
            }

            return string.Join("\n", lines.ToArray());
            */
            throw new System.NotImplementedException();
        }

        #region IEnumerable Members

        public IEnumerator<FontIdentity> GetEnumerator()
        {
            return this.fontDict.Values.GetEnumerator();
        }

        System.Collections.IEnumerator System.Collections.IEnumerable.GetEnumerator()
        {
            return this.fontDict.Values.GetEnumerator();
        }

        #endregion

        public event FontIdentityCallback FontIdentityDefined;

        private Dictionary<string, FontIdentity> fontDict;
        private FontIdentity fallbackFontIdentity;
        private static FontCollection defaultCollection;
        private static System.Predicate<string> fontListFilter;
    }
}

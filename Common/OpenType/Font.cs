//	Copyright © 2005-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System;
using System.Collections.Generic;

namespace Epsitec.Common.OpenType
{
    /// <summary>
    /// The <c>Font</c> class maps the low-level OpenType font description to
    /// the real needs of an application which has to manipulate glyphs.
    /// </summary>
    public sealed class Font
    {
        internal Font(FontIdentity identity)
        {
            this.identity = identity;
            // TODO bl-net8-cross free the underlying freetype handle when done
            this.fontHandle = AntigrainSharp.Font.LoadFromFile(identity.FilePath);
        }

        public FontIdentity FontIdentity
        {
            get { return this.identity; }
        }

        public double SpaceWidth
        {
            get
            {
                ushort glyph = this.SpaceGlyph;
                return (glyph == 0xffff) ? 0.25 : this.GetGlyphWidth(glyph, 1.0);
            }
        }

        public double FigureWidth
        {
            get
            {
                ushort glyph = this.GetGlyphIndex('0');
                return (glyph == 0xffff) ? 0.5 : this.GetGlyphWidth(glyph, 1.0);
            }
        }

        public double EmWidth
        {
            get
            {
                ushort glyph = this.GetGlyphIndex(0x2014);
                return (glyph == 0xffff) ? 1.0 : this.GetGlyphWidth(glyph, 1.0);
            }
        }

        public double EnWidth
        {
            get
            {
                ushort glyph = this.GetGlyphIndex(0x2013);
                return (glyph == 0xffff) ? 0.5 : this.GetGlyphWidth(glyph, 1.0);
            }
        }

        public char SpaceChar
        {
            get { return ' '; }
        }

        public char HyphenChar
        {
            get { return '-'; }
        }

        public char EllipsisChar
        {
            get { return (char)0x2026; }
        }

        public char PeriodChar
        {
            get { return '.'; }
        }

        public ushort SpaceGlyph
        {
            get
            {
                if (this.spaceGlyph == 0)
                {
                    this.spaceGlyph = this.GetGlyphIndex(this.SpaceChar);
                }

                return this.spaceGlyph;
            }
        }

        public ushort HyphenGlyph
        {
            get
            {
                if (this.hyphenGlyph == 0)
                {
                    this.hyphenGlyph = this.GetGlyphIndex(this.HyphenChar);
                }

                return this.hyphenGlyph;
            }
        }

        public ushort EllipsisGlyph
        {
            get
            {
                if (this.ellipsisGlyph == 0)
                {
                    this.ellipsisGlyph = this.GetGlyphIndex(this.EllipsisChar);
                }

                return this.ellipsisGlyph;
            }
        }

        public ushort PeriodGlyph
        {
            get
            {
                if (this.periodGlyph == 0)
                {
                    this.periodGlyph = this.GetGlyphIndex(this.PeriodChar);
                }

                return this.periodGlyph;
            }
        }

        public double HyphenWidth
        {
            get
            {
                //                double perEm = this.otHead.UnitsPerEm;
                //                return this.GetAdvance(this.HyphenGlyph) / perEm;
                throw new System.NotImplementedException();
            }
        }

        public double EllipsisWidth
        {
            get
            {
                /*
                double perEm = this.otHead.UnitsPerEm;
                return this.GetAdvance(this.EllipsisGlyph) / perEm};
                */
                throw new System.NotImplementedException();
            }
        }

        public double PeriodWidth
        {
            get
            {
                /*
                double perEm = this.otHead.UnitsPerEm;
                ushort glyph = this.PeriodGlyph;

                return glyph == 0xffff ? 0.5 : (this.GetAdvance(this.PeriodGlyph) / perEm);
                */
                throw new System.NotImplementedException();
            }
        }

        //public FontType FontType
        //{
        //    get { return this.fontType; }
        //}

        /// <summary>
        /// Compares the typography of two fonts and return true if both have the
        /// same glyphs (they use the same font type).
        /// </summary>
        /// <param name="a">The first font.</param>
        /// <param name="b">The second font.</param>
        /// <returns><c>true</c> if both fonts use the same font type; otherwise, <c>false</c></returns>
        public static bool HaveEqualTypography(Font a, Font b)
        {
            if (a == b)
            {
                return true;
            }

            if ((a == null) || (b == null))
            {
                return false;
            }

            return a.FontIdentity.UniqueFontId == b.FontIdentity.UniqueFontId;
        }

        public int GetTypographyHashCode()
        {
            return this.FontIdentity.UniqueFontId.GetHashCode();
        }

        /// <summary>
        /// Generates the glyphs for the specified text.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <returns>The glyphs represented by 16-bit unsigned values.</returns>
        public ushort[] GenerateGlyphs(string text)
        {
            int[] glMap;
            return this.GenerateGlyphs(text, out glMap);
        }

        /// <summary>
        /// Generates the glyphs for the specified text. Fills the glyph length
        /// map, if provided, with a character count per glyph, for each output
        /// glyph.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="glMap">The glyph length map array or <c>null</c>.</param>
        /// <returns>
        /// The glyphs represented by 16-bit unsigned values.
        /// </returns>
        public ushort[] GenerateGlyphs(string text, out int[] glMap)
        {
            int length = text.Length;
            glMap = null;
            ushort[] glyphs = new ushort[length];

            for (int i = 0; i < length; i++)
            {
                glyphs[i] = this.GetGlyphIndex(text[i]);
            }

            //this.ApplySubstitutions(ref glyphs, ref glMap);

            return glyphs;
        }

        /// <summary>
        /// Generates the glyphs for the specified text.
        /// </summary>
        /// <param name="text">The text encoded as 32-bit Unicode characters.</param>
        /// <param name="start">The start offset in the character array.</param>
        /// <param name="length">The length of the text.</param>
        /// <returns>The glyphs represented by 16-bit unsigned values.</returns>
        public ushort[] GenerateGlyphsWithMask(ulong[] text, int start, int length)
        {
            ushort[] glyphs = new ushort[length];
            int[] attributes = null;
            this.GenerateGlyphsWithMask<int>(text, start, length, out glyphs, ref attributes);

            return glyphs;
        }

        /// <summary>
        /// Generates the glyphs.
        /// The caller can provide an array with one attribute for each input
        /// character; <c>GenerateGlyphsWithMask</c> will adjust the contents of the attribute
        /// array so that there is exactly one attribute for each output glyph. If
        /// several characters map to a single glyph, the attribute of the first
        /// character will be preserved and the others will be dropped.
        /// </summary>
        /// <param name="text">The text encoded as 32-bit Unicode characters.</param>
        /// <param name="start">The start offset in the character array.</param>
        /// <param name="length">The length of the text.</param>
        /// <param name="glyphs">The output glyphs array.</param>
        /// <param name="attributes">The attributes array which must be adjusted or
        /// <c>null</c> if there are no attributes.</param>
        public void GenerateGlyphsWithMask<AttrT>(
            ulong[] text,
            int start,
            int length,
            out ushort[] glyphs,
            ref AttrT[] attributes
        )
        {
            int[] glMap;
            int count;

            glyphs = new ushort[length];
            glMap = new int[length];

            for (int i = 0; i < length; i++)
            {
                ulong bits = text[start + i];
                int code = Font.UnicodeMask & (int)bits;

                glyphs[i] =
                    (bits & Font.TransparentGlyphFlag) == 0
                        ? this.GetGlyphIndex(code)
                        : (ushort)code;
            }

            //this.ApplySubstitutions(ref glyphs, ref glMap);

            if (attributes != null)
            {
                length = glyphs.Length;
                count = attributes.Length;

                int src = 0;
                int dst = 0;

                //	TODO: gérer le cas où il y a plus de glyphes en sortie qu'il n'y a de
                //	place dans la table des attributs.

                for (int i = 0; i < length; i++)
                {
                    attributes[dst] = attributes[src];

                    dst += 1;
                    src += glMap[i] + 1;
                }
                while (src < count)
                {
                    attributes[dst++] = attributes[src++];
                }
            }
        }

        /// <summary>
        /// Gets the width of the specified space glyph.
        /// </summary>
        /// <param name="glyph">The space glyph.</param>
        /// <param name="size">The point size of the font.</param>
        /// <returns>
        /// The width of the specified space glyph.
        /// </returns>
        public double GetGlyphWidth(ushort glyph, double size)
        {
            // bl-net8-cross some space glyphs might need special treatment
            // We should also take the dpi into account
            return this.fontHandle.GetGlyphAdvance(glyph, size);
        }

        /// <summary>
        /// Gets the glyph bounds.
        /// </summary>
        /// <param name="glyph">The glyph.</param>
        /// <param name="size">The font point size.</param>
        /// <param name="xMin">X minimum.</param>
        /// <param name="xMax">X maximum.</param>
        /// <param name="yMin">Y minimum.</param>
        /// <param name="yMax">Y maximum.</param>
        public void GetGlyphBounds(
            ushort glyph,
            double size,
            out double xMin,
            out double xMax,
            out double yMin,
            out double yMax
        )
        {
            this.fontHandle.GetGlyphBBox(glyph, size, out xMin, out xMax, out yMin, out yMax);
        }

        /// <summary>
        /// Gets the total width for the specified glyphs.
        /// </summary>
        /// <param name="glyphs">The glyphs.</param>
        /// <param name="size">The font point size.</param>
        /// <returns>The total width of the glyphs.</returns>
        public double GetTotalWidth(ushort[] glyphs, double size)
        {
            return this.GetPositions(glyphs, size, 0, null);
        }

        /// <summary>
        /// Gets the individual glyph positions.
        /// </summary>
        /// <param name="glyphs">The glyphs.</param>
        /// <param name="size">The font point size.</param>
        /// <param name="ox">The x origin for the first glyph.</param>
        /// <param name="xPos">The array of positions after every glyph; the array must
        /// be allocated by the caller.</param>
        /// <returns>The total width of the glyphs.</returns>
        public double GetPositions(ushort[] glyphs, double size, double ox, double[] xPos)
        {
            return this.GetPositions(glyphs, size, ox, xPos, null, null, null);
        }

        /// <summary>
        /// Gets the individual glyph positions. The scale is used to modify the glyph
        /// width (font glyph width + specified width adjustment); the glue is added
        /// independently of the scale.
        /// </summary>
        /// <param name="glyphs">The glyphs.</param>
        /// <param name="size">The font point size.</param>
        /// <param name="ox">The x origin for the first glyph.</param>
        /// <param name="xPos">The array of positions after every glyph which must
        /// be allocated by the caller.</param>
        /// <param name="xScale">The array of horizontal scales applied for every individual glyph or <c>null</c> if every glyph has a scale of <c>1</c>.</param>
        /// <param name="xAdjust">The array of horizontal width adjustement for every individual glyph or <c>null</c> if every glyph has an adjustement of <c>0</c>.</param>
        /// <param name="xGlue">The array of horizontal glue for every individual glyph or <c>null</c> if every glyph has a glue of <c>0</c>.</param>
        /// <returns>The total width of the glyphs.</returns>
        public double GetPositions(
            ushort[] glyphs,
            double size,
            double ox,
            double[] xPos,
            double[] xScale,
            double[] xAdjust,
            double[] xGlue
        )
        {
            double advance = 0;
            ushort prevGlyph = 0xffff;

            for (int i = 0; i < glyphs.Length; i++)
            {
                ushort glyph = glyphs[i];
                double glyphSize = xScale == null ? size : size * xScale[i];
                advance += this.fontHandle.GetKerning(prevGlyph, glyph, glyphSize);
                prevGlyph = glyph;

                if (xPos != null)
                {
                    xPos[i] = ox + advance;
                }

                advance += this.GetGlyphWidth(glyph, glyphSize);

                if (xAdjust != null)
                {
                    advance += xAdjust[i] * xScale[i];
                }

                if (xGlue != null)
                {
                    advance += xGlue[i];
                }
            }
            return advance;
        }

        /// <summary>
        /// Maps a text position into coordinates.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="size">The font point size.</param>
        /// <param name="pos">The position in the text expressed as an offet from the start of the string.</param>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <returns><c>true</c> if the position exists; otherwise, <c>false</c>.</returns>
        public bool HitTest(string text, double size, int pos, out double x, out double y)
        {
            /*
            if ((pos > text.Length) || (pos < 0))
            {
                x = 0.0;
                y = 0.0;

                return false;
            }

            ushort[] glyphs;
            int[] glMap;

            this.MapToGlyphs(text, out glyphs, out glMap);

            return this.HitTest(glyphs, glMap, size, pos, out x, out y);
            */
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Maps a text position into coordinates.
        /// </summary>
        /// <param name="text">The text encoded as 32-bit Unicode characters.</param>
        /// <param name="start">The start offset in the character array.</param>
        /// <param name="length">The length of the text.</param>
        /// <param name="size">The font point size.</param>
        /// <param name="pos">The position in the text expressed as an offet from the start of the string.</param>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <returns><c>true</c> if the position exists; otherwise, <c>false</c>.</returns>
        public bool HitTest(
            ulong[] text,
            int start,
            int length,
            double size,
            int pos,
            out double x,
            out double y
        )
        {
            /*
            if ((pos > length) || (pos < 0))
            {
                x = 0.0;
                y = 0.0;

                return false;
            }

            ushort[] glyphs;
            int[] glMap;

            this.MapToGlyphs(text, start, length, out glyphs, out glMap);

            return this.HitTest(glyphs, glMap, size, pos, out x, out y);
            */
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Maps coordinates to a text position.
        /// </summary>
        /// <param name="text">The text.</param>
        /// <param name="size">The font point size.</param>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="pos">The position for the specified coordinates.</param>
        /// <param name="subpos">The sub-position relative to the position (0 means exact hit, 0.1 means
        /// that the coordinates are 10% farther than the position, etc.)</param>
        /// <returns><c>true</c> if the position exists; otherwise, <c>false</c>.</returns>
        public bool HitTest(
            string text,
            double size,
            double x,
            double y,
            out int pos,
            out double subpos
        )
        {
            /*
            ushort[] glyphs;
            int[] glMap;

            this.MapToGlyphs(text, out glyphs, out glMap);

            return this.HitTest(glyphs, glMap, size, x, y, out pos, out subpos);
            */
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Maps coordinates to a text position.
        /// </summary>
        /// <param name="text">The text encoded as 32-bit Unicode characters.</param>
        /// <param name="start">The start offset in the character array.</param>
        /// <param name="length">The length of the text.</param>
        /// <param name="size">The font point size.</param>
        /// <param name="x">The X coordinate.</param>
        /// <param name="y">The Y coordinate.</param>
        /// <param name="pos">The position for the specified coordinates.</param>
        /// <param name="subpos">The sub-position relative to the position (0 means exact hit, 0.1 means
        /// that the coordinates are 10% farther than the position, etc.)</param>
        /// <returns><c>true</c> if the position exists; otherwise, <c>false</c>.</returns>
        public bool HitTest(
            ulong[] text,
            int start,
            int length,
            double size,
            double x,
            double y,
            out int pos,
            out double subpos
        )
        {
            /*
            ushort[] glyphs;
            int[] glMap;

            this.MapToGlyphs(text, start, length, out glyphs, out glMap);

            return this.HitTest(glyphs, glMap, size, x, y, out pos, out subpos);
            */
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Selects a script for the default language.
        /// </summary>
        /// <param name="script">The OpenType script name.</param>
        public void SelectScript(string script)
        {
            this.SelectScript(script, "");
        }

        /// <summary>
        /// Selects the script for the specified language.
        /// </summary>
        /// <param name="script">The OpenType script name.</param>
        /// <param name="language">The OpenType script language.</param>
        public void SelectScript(string script, string language)
        {
            /*
            if ((this.activeScript == script) && (this.activeLanguage == language))
            {
                return;
            }

            this.activeScript = script;
            this.activeLanguage = language;

            if ((this.otGSUB != null) && (this.otGSUB.ScriptListTable.ContainsScript(script)))
            {
                int requiredFeature = this.otGSUB.GetRequiredFeatureIndex(script, language);
                int[] optionalFeatures = this.otGSUB.GetFeatureIndexes(script, language);

                if (requiredFeature == 0xffff)
                {
                    this.scriptRequiredFeature = null;
                }
                else
                {
                    this.scriptRequiredFeature = this.otGSUB.FeatureListTable.GetTaggedFeatureTable(
                        requiredFeature
                    );
                }

                this.scriptOptionalFeatures = new TaggedFeatureTable[optionalFeatures.Length];

                for (int i = 0; i < optionalFeatures.Length; i++)
                {
                    this.scriptOptionalFeatures[i] =
                        this.otGSUB.FeatureListTable.GetTaggedFeatureTable(optionalFeatures[i]);
                }
            }
            else
            {
                this.scriptRequiredFeature = null;
                this.scriptOptionalFeatures = null;
            }

            this.substitutionLookups = null;
            this.alternateLookups = null;
            */
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Selects the specified OpenType font features.
        /// </summary>
        /// <param name="features">The OpenType font features (4 character strings such as
        /// <c>"kern"</c>, <c>"liga"</c>, etc. or <c>"Mgr=System"</c> and <c>"Mgr=OpenType"</c>
        /// to select one of the font managers).</param>
        public void SelectFeatures(params string[] features)
        {
            // TODO bl-net8-cross implement or delete SelectFeatures
            // for now, we have kerning by default
            /*
            System.Text.StringBuilder buffer = new System.Text.StringBuilder();

            for (int i = 0; i < features.Length; i++)
            {
                if ((features[i] != null) && (features[i].Length > 0))
                {
                    if (buffer.Length > 0)
                    {
                        buffer.Append("/");
                    }

                    buffer.Append(features[i]);
                }
            }

            string collapsedFeatures = buffer.ToString();

            if (
                (this.activeFeatures == collapsedFeatures)
                && ((this.substitutionLookups != null) || (this.otGSUB == null))
            )
            {
                return;
            }

            this.activeFeatures = collapsedFeatures;

            FeatureListTable featureList =
                this.otGSUB == null ? null : this.otGSUB.FeatureListTable;
            System.Collections.ArrayList activeFeatures = new System.Collections.ArrayList();
            System.Collections.Hashtable activeNames = new System.Collections.Hashtable();

            for (int i = 0; i < features.Length; i++)
            {
                activeNames[features[i]] = null;
            }

            this.mapDefaultLigatures = activeNames.Contains("liga");

            this.useKerning = activeNames.Contains("kern");

            if (activeNames.Contains("Mgr=System"))
            {
                this.SelectFontManager(FontManagerType.System);
            }
            else if (activeNames.Contains("Mgr=OpenType"))
            {
                this.SelectFontManager(FontManagerType.OpenType);
            }

            if (this.scriptRequiredFeature != null)
            {
                activeFeatures.Add(this.scriptRequiredFeature);
            }

            if (this.scriptOptionalFeatures == null)
            {
                int n = featureList == null ? 0 : featureList.FeatureCount;

                for (int i = 0; i < n; i++)
                {
                    if (activeNames.Contains(featureList.GetFeatureTag(i)))
                    {
                        activeFeatures.Add(featureList.GetFeatureTable(i));
                        activeNames.Remove(featureList.GetFeatureTag(i));
                    }
                }
            }
            else
            {
                int n = this.scriptOptionalFeatures.Length;

                for (int i = 0; i < n; i++)
                {
                    if (activeNames.Contains(this.scriptOptionalFeatures[i].Tag))
                    {
                        activeFeatures.Add(this.scriptOptionalFeatures[i]);
                    }
                }
            }

            if (this.otGSUB != null)
            {
                this.GenerateSubstitutionLookups(activeFeatures);
            }
            */
        }

        /// <summary>
        /// Saves the active features on an internal stack.
        /// </summary>
        public void PushActiveFeatures()
        {
            /*
            if (this.savedFeaturesStack == null)
            {
                this.savedFeaturesStack = new Stack<string>();
            }

            this.savedFeaturesStack.Push(this.activeFeatures);
            */
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Disables some of the active features.
        /// </summary>
        /// <param name="features">The features which should be disabled.</param>
        public void DisableActiveFeatures(params string[] features)
        {
            /*
            string active = this.activeFeatures;
            int changes = 0;

            foreach (string feature in features)
            {
                if (active.IndexOf(feature) != -1)
                {
                    active = active.Replace(feature, "");
                    changes++;
                }
            }

            if (changes > 0)
            {
                this.SelectFeatures(active.Split('/'));
            }
            */
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Restore the active features by popping them from an internal stack.
        /// </summary>
        public void PopActiveFeatures()
        {
            /*
            if (this.savedFeaturesStack != null)
            {
                string features = this.savedFeaturesStack.Pop();
                this.SelectFeatures(features.Split('/'));
            }
            */
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the scripts supported by this font.
        /// </summary>
        /// <returns>An array of the scripts supported by this font.</returns>
        public string[] GetSupportedScripts()
        {
            /*
            System.Text.StringBuilder buffer = new System.Text.StringBuilder();

            ScriptListTable scriptList = this.otGSUB == null ? null : this.otGSUB.ScriptListTable;
            int n = scriptList == null ? 0 : scriptList.ScriptCount;

            for (int i = 0; i < n; i++)
            {
                string scriptTag = scriptList.GetScriptTag(i);
                ScriptTable scriptTable = scriptList.GetScriptTable(i);

                if (i > 0)
                {
                    buffer.Append("|");
                }

                buffer.Append(scriptTag);

                int m = scriptTable.LangSysCount;

                for (int j = 0; j < m; j++)
                {
                    buffer.Append("|");
                    buffer.Append(scriptTag);
                    buffer.Append(":");
                    buffer.Append(scriptTable.GetLangSysTag(j));
                }
            }

            return buffer.ToString().Split('|');
            */
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the features supported by this font.
        /// </summary>
        /// <returns>An array of the features supported by this font.</returns>
        public string[] GetSupportedFeatures()
        {
            /*
            List<string> dict = new List<string>();

            if (this.scriptOptionalFeatures == null)
            {
                FeatureListTable featureList =
                    this.otGSUB == null ? null : this.otGSUB.FeatureListTable;

                int n = featureList == null ? 0 : featureList.FeatureCount;

                for (int i = 0; i < n; i++)
                {
                    string tag = featureList.GetFeatureTag(i);

                    if (dict.Contains(tag) == false)
                    {
                        dict.Add(tag);
                    }
                }
            }
            else
            {
                int n = this.scriptOptionalFeatures.Length;

                for (int i = 0; i < n; i++)
                {
                    string tag = this.scriptOptionalFeatures[i].Tag;

                    if (dict.Contains(tag) == false)
                    {
                        dict.Add(tag);
                    }
                }
            }

            //	Ajoute des features "synthétiques" comme le crénage et les ligatures
            //	que nous savons émuler au besoin :

            if (otKernFormat0 != null)
            {
                if (dict.Contains("kern") == false)
                {
                    dict.Add("kern");
                }
            }

            if (dict.Contains("liga") == false)
            {
                dict.Add("liga");
            }

            dict.Sort();
            return dict.ToArray();
            */
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Checks if this OpenType font supports a given feature.
        /// </summary>
        /// <param name="feature">The feature to check.</param>
        /// <returns><c>true</c> if the feature is supported; otherwise, <c>false</c>.</returns>
        public bool SupportsFeature(string feature)
        {
            string[] features = this.GetSupportedFeatures();

            for (int i = 0; i < features.Length; i++)
            {
                if (features[i] == feature)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Gets the lookup tables for the specified features.
        /// </summary>
        /// <param name="features">The features.</param>
        /// <returns></returns>
        public LookupTable[] GetLookupTables(params string[] features)
        {
            /*
            List<LookupTable> list = new List<LookupTable>();

            if (this.otGSUB != null)
            {
                foreach (string feature in features)
                {
                    int[] indexes = this.otGSUB.GetFeatureIndexes(feature);
                    foreach (int index in indexes)
                    {
                        FeatureTable fTable = this.otGSUB.FeatureListTable.GetFeatureTable(index);
                        int count = fTable.LookupCount;

                        for (int i = 0; i < count; i++)
                        {
                            list.Add(
                                this.otGSUB.LookupListTable.GetLookupTable(fTable.GetLookupIndex(i))
                            );
                        }
                    }
                }
            }

            return list.ToArray();
            */
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the alternates for a given glyph (one glyph can have different
        /// representations which are encoded as <i>alternates</i>).
        /// </summary>
        /// <param name="glyph">The glyph.</param>
        /// <param name="alternates">The array of alternates.</param>
        /// <returns><c>true</c> if alternates exist; otherwise, <c>false</c>.</returns>
        public bool GetAlternates(ushort glyph, out ushort[] alternates)
        {
            /*
            if (this.alternateLookups == null)
            {
                List<BaseSubstitution> list = new List<BaseSubstitution>();

                foreach (string feature in this.activeFeatures.Split('/'))
                {
                    LookupTable[] tables = this.GetLookupTables(feature);

                    foreach (LookupTable table in tables)
                    {
                        if (table.LookupType == Common.OpenType.LookupType.Alternate)
                        {
                            for (int i = 0; i < table.SubTableCount; i++)
                            {
                                list.Add(new AlternateSubstitution(table.GetSubTable(i)));
                            }
                        }

                        if (table.LookupType == Common.OpenType.LookupType.Single)
                        {
                            for (int i = 0; i < table.SubTableCount; i++)
                            {
                                list.Add(new SingleSubstitution(table.GetSubTable(i)));
                            }
                        }
                    }
                }

                this.alternateLookups = list.ToArray();
            }

            if (this.alternateLookups != null)
            {
                List<ushort> list = null;

                foreach (BaseSubstitution subst in this.alternateLookups)
                {
                    if (subst.Coverage.FindIndex(glyph) >= 0)
                    {
                        if (list == null)
                        {
                            list = new List<ushort>();
                        }

                        AlternateSubstitution alternate = subst as AlternateSubstitution;
                        SingleSubstitution single = subst as SingleSubstitution;

                        if (alternate != null)
                        {
                            ushort[] subset = alternate.GetAlternates(glyph);

                            System.Diagnostics.Debug.Assert(subset != null);
                            System.Diagnostics.Debug.Assert(subset.Length > 0);

                            for (int i = 0; i < subset.Length; i++)
                            {
                                ushort replace = subset[i];

                                if (list.Contains(replace) == false)
                                {
                                    list.Add(replace);
                                }
                            }
                        }
                        if (single != null)
                        {
                            ushort replace = single.FindSubstitution(glyph);

                            if (list.Contains(replace) == false)
                            {
                                list.Add(replace);
                            }
                        }
                    }
                }

                if (list != null)
                {
                    alternates = list.ToArray();
                    return true;
                }
            }

            alternates = null;
            return false;
            */
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the font ascender.
        /// </summary>
        /// <param name="size">The font point size.</param>
        /// <returns>The font ascender.</returns>
        public double GetAscender(double size)
        {
            return this.fontHandle.GetAscender(size);
        }

        /// <summary>
        /// Gets the font descender.
        /// </summary>
        /// <param name="size">The font point size.</param>
        /// <returns>The font descender.</returns>
        public double GetDescender(double size)
        {
            return this.fontHandle.GetDescender(size);
        }

        /// <summary>
        /// Gets the font line gap.
        /// </summary>
        /// <param name="size">The font point size.</param>
        /// <returns>The font line gap.</returns>
        public double GetLineGap(double size)
        {
            return this.fontHandle.GetHeight(size);
        }

        /// <summary>
        /// Gets the font caret angle in radians.
        /// </summary>
        /// <returns>The font caret angle in radians.</returns>
        public double GetCaretAngleRad()
        {
            /*
            double dx = this.otHhea.CaretSlopeRun;
            double dy = this.otHhea.CaretSlopeRise;

            return System.Math.Atan2(dy, dx);
            */
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the font caret angle in degrees.
        /// </summary>
        /// <returns>The font caret angle in degrees.</returns>
        public double GetCaretAngleDeg()
        {
            return this.GetCaretAngleRad() * 180.0 / System.Math.PI;
        }

        public double GetXHeight(double size)
        {
            /*
            double scale = size / this.otHead.UnitsPerEm;
            TableOS2 os2 = this.GetTableOS2();
            double xHeight = scale * os2.XHeight;

            if (xHeight == 0)
            {
                double xMin,
                    xMax,
                    yMin,
                    yMax;
                ushort glyph = this.GetGlyphIndex('o');

                this.GetGlyphBounds(glyph, size, out xMin, out xMax, out yMin, out yMax);

                xHeight = yMax + yMin;
            }

            return xHeight;
            */
            throw new System.NotImplementedException();
        }

        public double GetCapHeight(double size)
        {
            /*
            double scale = size / this.otHead.UnitsPerEm;
            TableOS2 os2 = this.GetTableOS2();
            double capHeight = scale * os2.CapHeight;

            if (capHeight == 0)
            {
                double xMin,
                    xMax,
                    yMin,
                    yMax;
                ushort glyph = this.GetGlyphIndex('O');

                this.GetGlyphBounds(glyph, size, out xMin, out xMax, out yMin, out yMax);

                capHeight = yMax + yMin;
            }

            return capHeight;
            */
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the font maximum box.
        /// </summary>
        /// <param name="size">The font point size.</param>
        /// <param name="xMin">The minimum X value.</param>
        /// <param name="xMax">The maximum X value.</param>
        /// <param name="yMin">The minimum Y value.</param>
        /// <param name="yMax">The maximum Y value.</param>
        public void GetMaxBox(
            double size,
            out double xMin,
            out double xMax,
            out double yMin,
            out double yMax
        )
        {
            /*
            double scale = size / this.otHead.UnitsPerEm;

            xMin = this.otHead.XMin * scale;
            xMax = this.otHead.XMax * scale;
            yMin = this.otHead.YMin * scale;
            yMax = this.otHead.YMax * scale;
            */
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the maximum box for a glyph string.
        /// </summary>
        /// <param name="glyphs">The glyphs.</param>
        /// <param name="size">The font point size.</param>
        /// <param name="x">The X coordinate for each glyph.</param>
        /// <param name="y">The Y coordinate for each glyph.</param>
        /// <param name="sx">The X scale for each glyph or <c>null</c>.</param>
        /// <param name="sy">The Y scale for each glyph or <c>null</c>.</param>
        /// <param name="xMin">The minimum X value.</param>
        /// <param name="xMax">The maximum X value.</param>
        /// <param name="yMin">The minimum Y value.</param>
        /// <param name="yMax">The maximum Y value.</param>
        public void GetMaxBox(
            ushort[] glyphs,
            double size,
            double[] x,
            double[] y,
            double[] sx,
            double[] sy,
            out double xMin,
            out double xMax,
            out double yMin,
            out double yMax
        )
        {
            /*
            System.Diagnostics.Debug.Assert(glyphs.Length > 0);

            double scale = size / this.otHead.UnitsPerEm;

            double otXmin = this.otHead.XMin * scale;
            double otXmax = this.otHead.XMax * scale;
            double otYmin = this.otHead.YMin * scale;
            double otYmax = this.otHead.YMax * scale;

            xMin = x[0] + otXmin * (sx == null ? 1 : sx[0]);
            xMax = x[0] + otXmax * (sx == null ? 1 : sx[0]);
            yMin = y[0] + otYmin * (sy == null ? 1 : sy[0]);
            yMax = y[0] + otYmax * (sy == null ? 1 : sy[0]);

            for (int i = 1; i < glyphs.Length; i++)
            {
                double xxMin = x[i] + otXmin * (sx == null ? 1 : sx[i]);
                double xxMax = x[i] + otXmax * (sx == null ? 1 : sx[i]);
                double yyMin = y[i] + otYmin * (sy == null ? 1 : sy[i]);
                double yyMax = y[i] + otYmax * (sy == null ? 1 : sy[i]);

                if (xxMin < xMin)
                    xMin = xxMin;
                if (xxMax > xMax)
                    xMax = xxMax;
                if (yyMin < yMin)
                    yMin = yyMin;
                if (yyMax > yMax)
                    yMax = yyMax;
            }
            */
            throw new System.NotImplementedException();
        }

        /// <summary>
        /// Gets the glyph index for the specified character code.
        /// </summary>
        /// <param name="code">The character code.</param>
        /// <returns>The glyph index.</returns>
        public ushort GetGlyphIndex(char code)
        {
            return this.GetGlyphIndex((int)code);
        }

        /// <summary>
        /// Gets the glyph index for the specified character code.
        /// </summary>
        /// <param name="code">The character code.</param>
        /// <returns>The glyph index or <c>0xffff</c> the mapping failed.</returns>
        public ushort GetGlyphIndex(int code)
        {
            ushort glyph = 0;

            /*
            if (code < Font.GlyphCacheSize)
            {
                glyph = this.glyphCache[code];

                if (glyph != 0)
                {
                    return glyph;
                }

                if (
                    (this.glyphCacheOthers != null)
                    && (this.glyphCacheOthers.TryGetValue(code, out glyph))
                )
                {
                    return glyph;
                }
            }
            */

            if (code == 0)
            {
                return 0xffff;
            }

            if (code <= 0x0003)
            {
                //	Start of text and end of text have no graphic representation.
                //	Avoid walking through the font, looking for glyphs :

                return 0x0000;
            }

            glyph = (ushort)this.fontHandle.GetCharIndex((ulong)code);

            if (glyph == 0x0000)
            {
                //	The font does not contain a glyph definition for this character.
                //	We still handle some characters such as the variants of the space
                //	character (which we graphically map to a special glyph) and of
                //	the dash/hyphen.

                switch (code)
                {
                    case 0x2000:
                        glyph = 0xff01;
                        break; //	1/2 em
                    case 0x2001:
                        glyph = 0xff00;
                        break; //	1 em
                    case 0x2002:
                        glyph = 0xff01;
                        break; //	1/2 em
                    case 0x2003:
                        glyph = 0xff00;
                        break; //	1 em
                    case 0x2004:
                        glyph = 0xff02;
                        break; //	1/3 em
                    case 0x2005:
                        glyph = 0xff03;
                        break; //	1/4 em
                    case 0x2006:
                        glyph = 0xff06;
                        break; //	1/6 em
                    case 0x2007:
                        glyph = 0xff0a;
                        break; //	'0' (digit)
                    case 0x2008:
                        glyph = 0xff09;
                        break; //	'.' (narrow punctuation)
                    case 0x2009:
                        glyph = 0xff05;
                        break; //	1/5 em
                    case 0x200A:
                        glyph = 0xff07;
                        break; //	1/16 em
                    case 0x200B:
                        glyph = 0xff08;
                        break; //	zero width
                    case 0x200C:
                        glyph = 0xff08;
                        break; //	zero width
                    case 0x200D:
                        glyph = 0xff08;
                        break; //	zero width

                    case 0x202F:
                        glyph = 0xff0b;
                        break; //	narrow space
                    case 0x205F:
                        glyph = 0xff04;
                        break; //	4/18 em
                    case 0x2060:
                        glyph = 0xff08;
                        break; //	zero width

                    case 0x00A0:
                        glyph = this.SpaceGlyph;
                        break;

                    case 0x2010: //	Hyphen
                    case 0x2011: //	Non Breaking Hyphen
                    case 0x00AD: //	Soft Hyphen
                    case 0x1806: //	Mongolian Todo Hyphen
                        glyph = this.HyphenGlyph;
                        break;
                }
            }

            /*
            if (code < Font.GlyphCacheSize)
            {
                this.glyphCache[code] = glyph;
            }
            else
            {
                if (this.glyphCacheOthers == null)
                {
                    this.glyphCacheOthers = new Dictionary<int, ushort>();
                }

                this.glyphCacheOthers[code] = glyph;
            }
            */

            return glyph;
        }

        public static bool IsStretchableSpaceCharacter(int code)
        {
            switch (code)
            {
                case 0x200B: //	zero width (expandable)
                case 0x202F: //	narrow no-break space
                case 0x0020: //	space
                case 0x00A0: //	no-break space
                    return true;
            }

            return false;
        }

        public static bool IsSpaceCharacter(int code)
        {
            switch (code)
            {
                case 0x2000: //	1/2 em
                case 0x2001: //	1 em
                case 0x2002: //	1/2 em
                case 0x2003: //	1 em
                case 0x2004: //	1/3 em
                case 0x2005: //	1/4 em
                case 0x2006: //	1/6 em
                case 0x2007: //	'0' (digit)
                case 0x2008: //	'.' (narrow punctuation)
                case 0x2009: //	1/5 em
                case 0x200A: //	1/16 em
                case 0x200B: //	zero width (expandable)
                case 0x200C: //	zero width
                case 0x200D: //	zero width

                case 0x202F: //	narrow no-break space
                case 0x205F: //	4/18 em
                case 0x2060: //	zero width

                case 0x0020: //	space
                case 0x00A0: //	no-break space
                    return true;
            }

            return false;
        }

        private const int UnicodeMask = 0x001fffff;
        private const int TransparentGlyphFlag = 0x00800000;

        private FontIdentity identity;
        private AntigrainSharp.Font fontHandle;

        //private FontData fontData;

        //private TableGSUB otGSUB;
        //private TableGDEF otGDEF;
        //private TableCmap otCmap;
        //private TableMaxp otMaxp;
        //private TableHead otHead;
        //private TableHhea otHhea;
        //private Tablehmtx otHmtx;
        //private TableLocaShort otLocaShort;
        //private TableLocaLong otLocaLong;
        //private TableGlyf otGlyf;
        //private KerningTableFormat0 otKernFormat0;
        //private IndexMappingTable otIndexMapping;
        //private FontType fontType;

        //private ushort glyphF;
        //private ushort glyphI;
        //private ushort glyphL;
        //private ushort glyphLigFf;
        //private ushort glyphLigFi;
        //private ushort glyphLigFl;
        //private ushort glyphLigFfi;
        //private ushort glyphLigFfl;

        //private string activeScript;
        //private string activeLanguage;
        //private string activeFeatures = "";

        private bool mapDefaultLigatures;
        private bool useKerning;

        //private bool useSystemGlyphSize;

        //private TaggedFeatureTable scriptRequiredFeature;
        //private TaggedFeatureTable[] scriptOptionalFeatures;
        //private BaseSubstitution[] substitutionLookups;
        //private BaseSubstitution[] alternateLookups;

        //private Stack<string> savedFeaturesStack;
        private ushort spaceGlyph;
        private ushort hyphenGlyph;
        private ushort ellipsisGlyph;
        private ushort periodGlyph;
        //private ushort[] glyphCache;
        //private Dictionary<int, ushort> glyphCacheOthers;
        //private ushort[] advanceCache;

        //private const int GlyphCacheSize = 256;
        //private const int AdvanceCacheSize = 256;

        //[System.ThreadStatic]
        //private static ushort[][] tempGlyphs;

        //[System.ThreadStatic]
        //private static int tempGlyphMaxSize;
    }
}

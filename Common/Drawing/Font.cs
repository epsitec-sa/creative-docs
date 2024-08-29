/*
This file is part of CreativeDocs.

Copyright © 2003-2024, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland

CreativeDocs is free software: you can redistribute it and/or modify
it under the terms of the GNU General Public License as published by
the Free Software Foundation, either version 3 of the License, or
any later version.

CreativeDocs is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
GNU General Public License for more details.

You should have received a copy of the GNU General Public License
along with this program.  If not, see <https://www.gnu.org/licenses/>.
*/


using System.Collections.Generic;
using AntigrainSharp;

namespace Epsitec.Common.Drawing
{
    public sealed class Font : System.IDisposable
    {
        #region Private constructors
        Font(OpenType.FontIdentity fontIdentity)
        {
            this.openTypeFontIdentity = fontIdentity;
        }

        Font(Font baseFont, string syntheticStyle, SyntheticFontMode syntheticMode)
        {
            this.openTypeFontIdentity = baseFont.openTypeFontIdentity;
            this.openTypeFont = baseFont.OpenTypeFont;

            this.syntheticStyle = syntheticStyle;
            this.syntheticMode = syntheticMode;
        }

        #endregion

        public static void Initialize()
        {
            //Font.useSegoe = System.Environment.OSVersion.Version.Major > 5;
        }

        static Font()
        {
            Font.SetupFonts();
        }

        #region IDisposable members
        public void Dispose() { }
        #endregion

        public string FaceName
        {
            get { return this.openTypeFontIdentity.InvariantFaceName; }
        }

        public string StyleName
        {
            get { return this.syntheticStyle ?? this.openTypeFontIdentity.InvariantStyleName; }
        }

        public bool IsStyleBold
        {
            get { return (this.StyleName.IndexOf("Bold") > -1); }
        }

        public bool IsStyleItalic
        {
            get
            {
                return (this.StyleName.IndexOf("Italic") > -1)
                    || (this.StyleName.IndexOf("Oblique") > -1)
                    || (this.StyleName.IndexOf("Slanted") > -1);
            }
        }

        public bool IsStyleRegular
        {
            get
            {
                switch (this.StyleName)
                {
                    case "Roman":
                    case "Normal":
                    case "Regular":
                        return true;

                    default:
                        return false;
                }
            }
        }

        public string LocaleStyleName
        {
            get { return this.openTypeFontIdentity.LocaleStyleName; }
        }

        public string OpticalName
        {
            get
            {
                return "";
                //	TODO: handle optical name
                //				return this.openTypeFontIdentity.InvariantOpticalName;
            }
        }

        public string UniqueName
        {
            get { return this.openTypeFontIdentity.UniqueFontId; }
        }

        public string FullName
        {
            get
            {
                System.Text.StringBuilder buffer = new System.Text.StringBuilder();

                string face = this.FaceName;
                string style = this.StyleName;
                string optical = this.OpticalName;

                buffer.Append(face);

                if (style != "")
                {
                    buffer.Append(" ");
                    buffer.Append(style);
                }

                if (optical != "")
                {
                    buffer.Append(" ");
                    buffer.Append(optical);
                }

                return buffer.ToString();
            }
        }

        public double Ascender
        {
            get { return this.OpenTypeFont.GetAscender(1.0); }
        }

        public double Descender
        {
            get { return this.OpenTypeFont.GetDescender(1.0); }
        }

        public double LineHeight
        {
            get
            {
                double ascender = this.OpenTypeFont.GetAscender(1.0);
                double descender = this.OpenTypeFont.GetDescender(1.0);
                double lineGap = this.OpenTypeFont.GetLineGap(1.0);

                return ascender - descender + lineGap;
            }
        }

        public bool IsSynthetic
        {
            get { return this.syntheticMode != SyntheticFontMode.None; }
        }

        public SyntheticFontMode SyntheticFontMode
        {
            get { return this.syntheticMode; }
        }

        public Transform SyntheticTransform
        {
            get
            {
                switch (this.syntheticMode)
                {
                    case SyntheticFontMode.Oblique:
                        return new Transform(
                            1,
                            System.Math.Sin(Font.DefaultObliqueAngle * System.Math.PI / 180.0),
                            0,
                            1,
                            0,
                            0
                        );
                    default:
                        return Transform.Identity;
                }
            }
        }

        public double CaretSlope
        {
            get
            {
                if (this.syntheticMode == SyntheticFontMode.Oblique)
                {
                    return 90 - Font.DefaultObliqueAngle;
                }

                return this.OpenTypeFont.GetCaretAngleDeg();
            }
        }

        public OpenType.FontIdentity OpenTypeFontIdentity
        {
            get { return this.openTypeFontIdentity; }
        }

        public OpenType.Font OpenTypeFont
        {
            get
            {
                if ((this.openTypeFont == null) && (this.openTypeFontIdentity != null))
                {
                    lock (this)
                    {
                        if (this.openTypeFont == null)
                        {
                            this.openTypeFont = Font.fontCollection.CreateFont(
                                this.openTypeFontIdentity
                            );
                        }
                    }
                }

                return this.openTypeFont;
            }
        }

        public bool IsOpenTypeFontLoaded
        {
            get { return this.openTypeFont != null; }
        }

        public bool MatchForStyle(bool bold, bool italic)
        {
            //	Vérifie si une police match pour les 4 cas possibles (normal, bold
            //	italique ou bold-italique).
            //	Par exemple, avec Windows 8 et la police "Segoe UI", on rencontre les
            //	styles suivants:
            //	- Black Italic
            //	- Black Regular
            //	- Bold				<- à utiliser pour bold
            //	- Bold Italic		<- à utiliser pour bold-italique
            //	- Italic			<- à utiliser pour italique
            //	- Light Italic
            //	- Light Regular
            //	- Mono Regular
            //	- Regular			<- à utiliser pour normal
            //	- Semibold Italic
            //	- Semibold Regular
            //	- Semilight Regular

            if (string.IsNullOrEmpty(this.StyleName))
            {
                return !bold && !italic;
            }
            else
            {
                var styles = this.StyleName.Split(' ');

                foreach (var style in styles)
                {
                    switch (style)
                    {
                        case "Roman":
                        case "Normal":
                        case "Regular":
                            //	Si on trouve l'un de ces 3 styles et qu'on cherche du bold
                            //	ou de l'italique, ça ne match pas.
                            if (bold || italic)
                            {
                                return false;
                            }
                            break;

                        case "Bold":
                            //	Si on trouve ce style et qu'on cherche du non-bold,
                            //	ça ne match pas.
                            if (!bold)
                            {
                                return false;
                            }
                            break;

                        case "Italic":
                        case "Oblique":
                        case "Slanted":
                            //	Si on trouve l'un de ces 3 styles et qu'on cherche du non-italic,
                            //	ça ne match pas.
                            if (!italic)
                            {
                                return false;
                            }
                            break;

                        default:
                            //	Si on trouve un autre style (par exemple Black, Light ou Semibold)
                            //	ça ne match jamais.
                            return false;
                    }
                }

                //	Si on a rien trouvé qui ne matchait pas, alors ça match !
                return true;
            }
        }

        public ushort GetGlyphIndex(int unicode)
        {
            return this.OpenTypeFont.GetGlyphIndex(unicode);
        }

        public double GetGlyphAdvance(ushort glyph)
        {
            return this.OpenTypeFont.GetGlyphWidth(glyph, 1.0);
        }

        public double GetCharAdvance(int unicode)
        {
            return this.GetGlyphAdvance(this.GetGlyphIndex(unicode));
        }

        public double GetTextAdvance(string text)
        {
            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }

            ushort[] glyphs = this.OpenTypeFont.GenerateGlyphs(text);
            return this.OpenTypeFont.GetTotalWidth(glyphs, 1.0);
        }

        public Rectangle GetGlyphBounds(ushort glyph, double size)
        {
            Drawing.Rectangle rect = this.GetGlyphBounds(glyph);

            rect.Scale(size);

            return rect;
        }

        public Rectangle GetGlyphBounds(ushort glyph)
        {
            if (this.syntheticMode == SyntheticFontMode.Oblique)
            {
                Path path = new Path();
                path.Append(this, glyph, this.SyntheticTransform);
                return path.ComputeBounds();
            }

            double x1,
                y1,
                x2,
                y2;

            this.OpenTypeFont.GetGlyphBounds(glyph, 1.0, out x1, out x2, out y1, out y2);

            return new Rectangle(x1, y1, x2 - x1, y2 - y1);
        }

        public Rectangle GetCharBounds(int unicode)
        {
            return this.GetGlyphBounds(this.GetGlyphIndex(unicode));
        }

        public Rectangle GetTextBounds(string text)
        {
            if (this.syntheticMode == SyntheticFontMode.Oblique)
            {
                Path path = new Path();
                Transform ft = this.SyntheticTransform;

                ft = new Transform(ft.XX, ft.XY, ft.YX, ft.YY, 0, 0);

                foreach (char unicode in text)
                {
                    ushort glyph = this.GetGlyphIndex(unicode);
                    path.Append(this, glyph, ft);
                    ft = new Transform(
                        ft.XX,
                        ft.XY,
                        ft.YX,
                        ft.YY,
                        ft.TX + this.GetGlyphAdvance(glyph),
                        ft.TY
                    );
                }

                return path.ComputeBounds();
            }

            ushort[] glyphs = this.OpenTypeFont.GenerateGlyphs(text);
            double[] tempX = new double[glyphs.Length];

            if (glyphs.Length == 0)
            {
                return Rectangle.Empty;
            }

            this.OpenTypeFont.GetPositions(glyphs, 1.0, 0.0, tempX);

            double x1,
                y1,
                x2,
                y2;
            double xmin,
                ymin,
                xmax,
                ymax;

            this.OpenTypeFont.GetGlyphBounds(glyphs[0], 1.0, out x1, out x2, out y1, out y2);

            xmin = x1;
            ymin = y1;
            xmax = x2;
            ymax = y2;

            for (int i = 1; i < glyphs.Length; i++)
            {
                this.OpenTypeFont.GetGlyphBounds(glyphs[i], 1.0, out x1, out x2, out y1, out y2);

                x1 += tempX[i];
                x2 += tempX[i];

                xmin = System.Math.Min(xmin, x1);
                xmax = System.Math.Max(xmax, x2);
                ymin = System.Math.Min(ymin, y1);
                ymax = System.Math.Max(ymax, y2);
            }

            return new Rectangle(xmin, ymin, xmax - xmin, ymax - ymin);
        }

        public void GetTextCharEndX(string text, out double[] xPos)
        {
            xPos = new double[text.Length];
            ushort[] glyphs = this.OpenTypeFont.GenerateGlyphs(text);
            this.OpenTypeFont.GetPositions(glyphs, 1.0, 0.0, xPos);
        }

        public void GetTextCharEndX(string text, FontClassInfo[] infos, out double[] xArray)
        {
            this.GetTextCharEndX(text, out xArray);

            double scaleSpace = 1.0;
            double scalePlain = 1.0;

            for (int i = 0; i < infos.Length; i++)
            {
                switch (infos[i].GlyphClass)
                {
                    case GlyphClass.PlainText:
                        scalePlain = infos[i].Scale;
                        break;

                    case GlyphClass.Space:
                        scaleSpace = infos[i].Scale;
                        break;
                }
            }

            //	Transform absolute [x] into relative [dx], scale using the glyph class and then
            //	transform back [dx] to [x] :

            double x1 = 0;
            double x2 = 0;

            for (int i = 0; i < xArray.Length; i++)
            {
                double dx = xArray[i] - x1;

                dx *=
                    (OpenType.Font.IsStretchableSpaceCharacter(text[i])) ? scaleSpace : scalePlain;

                x1 = xArray[i];
                x2 = x2 + dx;

                xArray[i] = x2;
            }
        }

        public void GetTextClassInfos(
            string text,
            out FontClassInfo[] infos,
            out double width,
            out double elasticity
        )
        {
            int n = text.Length;

            int nText = 0;
            int nSpace = 0;
            double wText = 0;
            double wSpace = 0;

            width = 0;
            elasticity = 0;

            for (int i = 0; i < n; i++)
            {
                int unicode = text[i];

                if (OpenType.Font.IsStretchableSpaceCharacter(unicode))
                {
                    nSpace += 1;
                    wSpace += this.GetCharAdvance(unicode);
                }
                else
                {
                    nText += 1;
                    wText += this.GetCharAdvance(unicode);
                }
            }

            int m = ((nText > 0 && wText > 0) ? 1 : 0) + ((nSpace > 0 && wSpace > 0) ? 1 : 0);
            infos = new FontClassInfo[m];

            m = 0;

            if ((nText > 0) && (wText > 0))
            {
                double e = 0.00 * nText;

                infos[m++] = new FontClassInfo(GlyphClass.PlainText, nText, wText, e);
                width += wText;
                elasticity += e;
            }

            if ((nSpace > 0) && (wSpace > 0))
            {
                double e = 1.00 * nSpace;

                infos[m++] = new FontClassInfo(GlyphClass.Space, nSpace, wSpace, e);
                width += wSpace;
                elasticity += e;
            }
        }

        public void GetGlyphsEndX(string text, out double[] xPos, out ushort[] glyphs)
        {
            glyphs = this.OpenTypeFont.GenerateGlyphs(text);
            xPos = new double[glyphs.Length];

            this.OpenTypeFont.GetPositions(glyphs, 1.0, 0.0, xPos);
        }

        public void ClearOpenTypeFont()
        {
            if (this.openTypeFontIdentity != null)
            {
                this.openTypeFontIdentity.InternalClearFontData();
                this.openTypeFont = null;
            }
        }

        public double PaintText(
            GraphicContext gctx,
            string text,
            double size,
            double ox,
            double oy,
            Color color
        )
        {
            Font.FontManager.SetFont(this.openTypeFontIdentity.FilePath, size);
            gctx.SetColor(color.R, color.G, color.B, color.A);
            return gctx.DrawText(text, ox, oy);
        }

        public void PaintPixelGlyphs(
            GraphicContext gctx,
            double scale,
            ushort[] glyphs,
            double[] xa,
            double[] ya,
            double[] sxa,
            Color color,
            double xx,
            double yy,
            double tx,
            double ty
        )
        {
            var rast = new AntigrainSharp.Rasterizer();
            rast.SetTransform(xx, 0, 0, yy, tx, ty);
            gctx.RendererSolid.Color(color.R, color.G, color.B, color.A);

            double sy = scale * yy;

            for (int i = 0; i < glyphs.Length; i++)
            {
                ushort glyph = glyphs[i];
                double x = xa[i] * xx + tx;
                double y = ya[i] * yy + ty;
                double sx = sxa[i] * scale * xx;

                rast.SetTransform(sx, 0, 0, sy, 0, 0);
                rast.AddGlyph(glyph, x, y, 1.0, Font.FontManager);
            }
            rast.RenderSolid(gctx.RendererSolid);
        }

        public static void RegisterResourceFont(
            System.Reflection.Assembly assembly,
            string resourceName
        )
        {
            using (var stream = assembly.GetManifestResourceStream(resourceName))
            {
                Font.RegisterDynamicFont(stream);
            }
        }

        public static void RegisterDynamicFont(System.IO.Stream stream)
        {
            if (stream == null)
            {
                return;
            }

            long length = stream.Length;
            byte[] data = new byte[length];

            stream.Read(data, 0, (int)length);

            Font.RegisterDynamicFont(data);
        }

        public static void RegisterDynamicFont(byte[] data)
        {
            Font.fontCollection.RegisterDynamicFont(data);
        }

        private static void SetupFonts()
        {
            Font.SetFontManager(new FontManager());

            if (Font.fontArray == null)
            {
                System.Diagnostics.Debug.WriteLine(
                    "SetupFonts called",
                    "Epsitec.Common.Drawing.Font"
                );

                Font.fontCollection = OpenType.FontCollection.Default;

                var fonts = OpenType.Platform.FontFinder.FindFontIdentities();
                Font.fontCollection.Initialize(fonts);

                Font.fontArray = new List<Font>();
                Font.fontHash = new Dictionary<string, Font>();

                foreach (OpenType.FontIdentity fontIdentity in Font.fontCollection)
                {
                    Font.AddFont(fontIdentity);
                }

                System.Diagnostics.Debug.WriteLine(
                    "SetupFonts done",
                    "Epsitec.Common.Drawing.Font"
                );
            }
            //Font.fontCollection.FontIdentityDefined += Font.HandleFontCollectionFontIdentityDefined;
        }

        private static void HandleFontCollectionFontIdentityDefined(
            OpenType.FontIdentity fontIdentity
        )
        {
            Font.AddFont(fontIdentity);
        }

        private static Font AddFont(OpenType.FontIdentity fontIdentity)
        {
            Font font = new Font(fontIdentity);

            string name = font.FullName;

            if (Font.fontHash.ContainsKey(name))
            {
                System.Diagnostics.Debug.WriteLine(
                    string.Format("AddFont, font already known, name={0}", name),
                    "Epsitec.Common.Drawing.Font"
                );
                return Font.fontHash[name];
            }
            else
            {
                Font.fontArray.Add(font);
                Font.fontHash[name] = font;

                return font;
            }
        }

        public static int Count
        {
            get { return Font.fontArray.Count; }
        }

        public static Font DefaultFont
        {
            get
            {
                if (Font.defaultFont == null)
                {
                    Font.defaultFont = Font.GetFont(Font.DefaultFontFamily, "Regular");
                }

                return Font.defaultFont;
            }
        }

        public static string DefaultFontFamily
        {
            get
            {
                if (Font.useSegoe)
                {
                    return "Segoe UI";
                }
                else
                {
                    return "Tahoma";
                }
            }
        }

        public static double DefaultFontSize
        {
            get
            {
                if (Font.useSegoe)
                {
                    return 12;
                }
                else
                {
                    return 12;
                }
            }
        }

        public static double DefaultObliqueAngle
        {
            get { return 20.0; }
        }

        public static Font GetFont(int rank)
        {
            if ((rank >= 0) && (rank < Font.fontArray.Count))
            {
                return Font.fontArray[rank];
            }

            return null;
        }

        public static Font GetFontFallback(string face)
        {
            return Font.GetFont(face, "");
        }

        public static Font GetFont(string face, string style)
        {
            return Font.GetFont(face, style, "");
        }

        public static Font GetFont(string face, string style, string optical)
        {
            OpenType.Font font = Font.fontCollection.CreateFont(face, style);
            return Font.GetFont(font);
        }

        public static Font GetFont(OpenType.Font font)
        {
            return Font.GetFont(font.FontIdentity);
        }

        public static Font GetFont(OpenType.FontIdentity id)
        {
            return new Font(id);
        }

        public static void SetFontManager(FontManager fontManager)
        {
            if (Font.fontManager == null)
            {
                Font.fontManager = fontManager;
            }
            else
            {
                throw new System.Exception("FontManager already set");
            }
        }

        public static FontManager FontManager
        {
            get { return Font.fontManager; }
        }

        readonly string syntheticStyle;
        readonly SyntheticFontMode syntheticMode;
        readonly OpenType.FontIdentity openTypeFontIdentity;
        private OpenType.Font openTypeFont;

        static OpenType.FontCollection fontCollection;
        static List<Font> fontArray;
        static Dictionary<string, Font> fontHash;
        static Font defaultFont;
        static FontManager fontManager;
        static readonly bool useSegoe;

        static readonly Dictionary<string, string> registeredFonts =
            new Dictionary<string, string>();
    }
}

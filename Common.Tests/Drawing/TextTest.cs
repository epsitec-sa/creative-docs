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

using Epsitec.Common.Drawing;
using Epsitec.Common.Text;
using Epsitec.Common.Text.Cursors;
using Epsitec.Common.Text.Layout;
using Epsitec.Common.Text.Properties;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Platform;
using NUnit.Framework;

namespace Epsitec.Common.Tests.Drawing
{
    [TestFixture]
    public class TextTest
    {
        [Test]
        public void AutomatedTestEnvironment()
        {
            Epsitec.Common.Widgets.Window.RunningInAutomatedTestEnvironment = true;
        }

        [Test]
        public void CheckPainting()
        {
            Window window = new Window();
            Painter painter = new Painter();

            window.ClientSize = new Size(500, 700);
            window.WindowLocation =
                ScreenInfo.Find(new Point(10, 10)).WorkingArea.TopLeft
                + new Point(100, -100 - window.WindowSize.Height);
            window.Text = "TextTest/CheckPainting";

            painter.SetParent(window.Root);
            painter.Dock = DockStyle.Fill;

            Controller controller = new Controller(painter);

            controller.CreateWindow(window);
            controller.GenerateText();

            window.Show();
            Window.RunInTestEnvironment(window);
        }

        public class Painter : Widget, ITextRenderer
        {
            public Painter()
            {
                this.frame_ratio = 1.0 / 3.0;

                this.story = new TextStory();
                this.fitter = new TextFitter(this.story);
                this.frame1 = new SimpleTextFrame(
                    this.Client.Size.Width * this.frame_ratio,
                    this.Client.Size.Height,
                    32
                );
                this.frame2 = new SimpleTextFrame(
                    this.Client.Size.Width - this.frame1.Width,
                    this.Client.Size.Height,
                    32
                );

                this.frame1.PageNumber = 1;
                this.frame2.PageNumber = 1;

                this.fitter.FrameList.InsertAt(0, this.frame1);
                this.fitter.FrameList.InsertAt(1, this.frame2);
            }

            public TextStory TextStory
            {
                get { return this.story; }
            }

            public TextFitter TextFitter
            {
                get { return this.fitter; }
            }

            public double FrameRatio
            {
                get { return this.frame_ratio; }
                set
                {
                    if (this.frame_ratio != value)
                    {
                        this.frame_ratio = value;
                        this.UpdateFrameSizes();
                        this.Invalidate();
                    }
                }
            }

            public bool Condition
            {
                get { return this.condition; }
                set
                {
                    if (this.condition != value)
                    {
                        this.condition = value;
                        this.UpdateTextLayout();
                        this.Invalidate();
                    }
                }
            }

            public bool ShowCursors
            {
                get { return this.show_cursors; }
                set
                {
                    if (this.show_cursors != value)
                    {
                        this.show_cursors = value;
                        this.Invalidate();
                    }
                }
            }

            public Graphics Graphics
            {
                get { return this.graphics; }
            }

            public void ResetTextStory()
            {
                this.story = new TextStory();
                this.fitter.TextStory = this.story;
            }

            public void NotifyTextChanged()
            {
                this.fitter.ClearAllMarks();
                this.fitter.GenerateAllMarks();
                this.Invalidate();
            }

            protected override void SetBoundsOverride(Rectangle oldRect, Rectangle newRect)
            {
                base.SetBoundsOverride(oldRect, newRect);

                if ((this.frame1 != null) && (this.frame2 != null) && (this.frame_ratio != 0))
                {
                    this.UpdateFrameSizes();
                }
            }

            protected override void PaintBackgroundImplementation(
                Graphics graphics,
                Rectangle clip_rect
            )
            {
                graphics.AddFilledRectangle(0, 0, this.Client.Size.Width, this.Client.Size.Height);
                graphics.RenderSolid(Color.FromBrightness(1.0));

                System.Diagnostics.Debug.WriteLine("Paint called.");

                this.graphics = graphics;
                this.fitter.RenderTextFrame(this.frame1, this);
                this.fitter.RenderTextFrame(this.frame2, this);
                this.graphics = null;

                System.Diagnostics.Debug.WriteLine("Paint done.");

                double ox1 = 60;
                double oy1 = 0;
                double ox2 = 60;
                double oy2 = -1000;

                this.frame1.MapToView(ref ox1, ref oy1);
                this.frame1.MapToView(ref ox2, ref oy2);

                graphics.LineWidth = 0.25;
                graphics.AddLine(ox1, oy1, ox2, oy2);
                graphics.RenderSolid(Color.FromName("Blue"));

                ox1 = this.frame1.Width;
                ox2 = ox1;

                graphics.LineWidth = 0.75;
                graphics.AddLine(ox1, oy1, ox2, oy2);
                graphics.RenderSolid(Color.FromName("Blue"));

                if (this.show_cursors)
                {
                    this.story.DebugDisableOpletQueue = true;

                    if (
                        this.story.TextContext.StyleList[
                            "Default",
                            Common.Text.TextStyleClass.Paragraph
                        ] == null
                    )
                    {
                        System.Collections.ArrayList properties =
                            new System.Collections.ArrayList();

                        properties.Add(new FontProperty("Verdana", "Italic"));
                        properties.Add(new FontSizeProperty(32.0, SizeUnits.Points));
                        properties.Add(
                            new MarginsProperty(
                                0,
                                0,
                                0,
                                0,
                                SizeUnits.Points,
                                0.0,
                                0.0,
                                0.5,
                                15,
                                1,
                                ThreeState.True
                            )
                        );
                        properties.Add(new FontColorProperty("Black"));
                        properties.Add(
                            new LeadingProperty(
                                double.NaN,
                                SizeUnits.None,
                                5.0,
                                SizeUnits.Points,
                                5.0,
                                SizeUnits.Points,
                                AlignMode.None
                            )
                        );

                        StyleList style_list = this.story.TextContext.StyleList;
                        Epsitec.Common.Text.TextStyle style_default = style_list.NewTextStyle(
                            null,
                            "Default",
                            Common.Text.TextStyleClass.Paragraph,
                            properties
                        );

                        this.story.TextContext.DefaultParagraphStyle = style_default;
                    }

                    Epsitec.Common.Text.TextNavigator navigator =
                        new Epsitec.Common.Text.TextNavigator(this.fitter);

                    Common.Text.ITextFrame frame;
                    double cx,
                        cy,
                        ascender,
                        descender,
                        angle;

                    System.Diagnostics.Debug.WriteLine("Show cursors: start...");

                    for (int i = 0; ; i++)
                    {
                        if (i > 0)
                        {
                            navigator.MoveTo(Common.Text.TextNavigator.Target.CharacterNext, 1);
                        }

                        if (
                            navigator.GetCursorGeometry(
                                out frame,
                                out cx,
                                out cy,
                                out ascender,
                                out descender,
                                out angle
                            )
                        )
                        {
                            double dx = System.Math.Cos(angle) * (ascender - descender);
                            double dy = System.Math.Sin(angle) * (ascender - descender);

                            cx += System.Math.Cos(angle) * (descender);
                            cy += System.Math.Sin(angle) * (descender);

                            graphics.AddLine(cx, cy, cx + dx, cy + dy);
                        }

                        if (navigator.CursorPosition >= this.story.TextLength)
                        {
                            break;
                        }
                    }

                    System.Diagnostics.Debug.WriteLine("Show cursors: done...");
                    System.Diagnostics.Debug.WriteLine(
                        string.Format("{0} characters in text.", this.story.TextLength)
                    );

                    navigator.Dispose();

                    graphics.RenderSolid(Color.FromName("Green"));
                }
            }

            private void UpdateFrameSizes()
            {
                this.frame1.Width = this.Client.Size.Width * this.frame_ratio;
                this.frame1.Height = this.Client.Size.Height;

                this.frame2.Width = this.Client.Size.Width - this.frame1.Width;
                this.frame2.Height = this.Client.Size.Height;
                this.frame2.X = this.frame1.Width + this.frame1.X;

                this.fitter.ClearAllMarks();
                this.fitter.GenerateAllMarks();
            }

            #region ITextRenderer Members
            public bool IsFrameAreaVisible(
                ITextFrame frame,
                double x,
                double y,
                double width,
                double height
            )
            {
                return true;
            }

            public void RenderStartParagraph(Context context) { }

            public void RenderStartLine(Context context)
            {
                ITextFrame frame = context.Frame;

                double ox = context.LineCurrentX;
                double oy = context.LineBaseY;
                double dx = context.TextWidth;

                this.graphics.LineWidth = 0.3;
                this.graphics.AddLine(ox, oy, ox + dx, oy);
                this.graphics.RenderSolid(Color.FromName("Green"));

                context.DisableSimpleRendering();
            }

            public void RenderTab(
                Context layout,
                string tag,
                double tab_origin,
                double tab_stop,
                ulong tab_code,
                bool is_tab_defined,
                bool is_tab_auto
            ) { }

            public void Render(
                Context layout,
                Epsitec.Common.OpenType.Font font,
                double size,
                string color,
                TextToGlyphMapping mapping,
                ushort[] glyphs,
                double[] x,
                double[] y,
                double[] sx,
                double[] sy,
                bool is_last_run
            )
            {
                ITextFrame frame = layout.Frame;

                System.Diagnostics.Debug.Assert(mapping != null);

                //	Vérifions d'abord que le mapping du texte vers les glyphes est
                //	correct et correspond à quelque chose de valide :

                int offset = 0;

                int[] c_array;
                ushort[] g_array;

                System.Text.StringBuilder buffer = new System.Text.StringBuilder();

                while (mapping.GetNextMapping(out c_array, out g_array))
                {
                    for (int i = 0; i < g_array.Length; i++)
                    {
                        System.Diagnostics.Debug.Assert(g_array[i] == glyphs[offset++]);
                    }
                    for (int i = 0; i < c_array.Length; i++)
                    {
                        buffer.Append((char)(c_array[i]));
                    }
                }

                //-				System.Diagnostics.Debug.WriteLine (buffer.ToString ());

                Font drawing_font = Font.GetFont(
                    font.FontIdentity.InvariantFaceName,
                    font.FontIdentity.InvariantStyleName
                );

                if (drawing_font != null)
                {
                    for (int i = 0; i < glyphs.Length; i++)
                    {
                        if (glyphs[i] < 0xffff)
                        {
                            this.graphics.Rasterizer.AddGlyph(
                                drawing_font,
                                glyphs[i],
                                x[i],
                                y[i],
                                size,
                                sx == null ? 1.0 : sx[i],
                                sy == null ? 1.0 : sy[i]
                            );
                        }
                    }
                }

                this.graphics.RenderSolid(Color.FromName(color));
            }

            public void Render(
                Context layout,
                IGlyphRenderer glyph_renderer,
                string color,
                double x,
                double y,
                bool is_last_run
            )
            {
                glyph_renderer.RenderGlyph(layout.Frame, x, y);
            }

            public void RenderEndLine(Context context) { }

            public void RenderEndParagraph(Context context)
            {
                XlineRecord[] records = context.XlineRecords;

                double x1 = 0;
                double y1 = 0;

                //	Dans ce test, la couleur est stockée directement comme LineStyle pour la propriété
                //	"underline".

                string color = "Yellow";

                if (records.Length > 0)
                {
                    for (int i = 0; i < records.Length; i++)
                    {
                        if (
                            (records[i].Type == Common.Text.Layout.XlineRecord.RecordType.LineEnd)
                            || (records[i].Xlines.Length == 0)
                        )
                        {
                            this.graphics.LineWidth = 1.0;
                            this.graphics.AddLine(
                                x1,
                                y1,
                                records[i].X,
                                records[i].Y + records[i].Descender * 0.8
                            );
                            this.graphics.RenderSolid(Color.FromName(color));
                        }

                        x1 = records[i].X;
                        y1 = records[i].Y + records[i].Descender * 0.8;

                        if (records[i].Xlines.Length > 0)
                        {
                            color = records[i].Xlines[0].DrawStyle;
                        }
                    }
                }
            }
            #endregion

            private TextStory story;
            private TextFitter fitter;
            private SimpleTextFrame frame1,
                frame2;
            private double frame_ratio;
            private Graphics graphics;
            private bool condition;
            private bool show_cursors;
        }

        private class Controller
        {
            public Controller(Painter painter)
            {
                this.painter = painter;
                this.features = new string[] { };
            }

            public void CreateWindow(Window owner)
            {
                this.window = new Window(WindowFlags.HideFromTaskbar | WindowFlags.NoBorder);
                this.window.Text = "Réglages pour TextTest/CheckPainting";

                StaticText st1 = new StaticText(this.window.Root);
                CheckButton cb1 = new CheckButton(this.window.Root);
                CheckButton cb2 = new CheckButton(this.window.Root);
                CheckButton cb3 = new CheckButton(this.window.Root);
                CheckButton cb4 = new CheckButton(this.window.Root);
                RadioButton rb1 = new RadioButton(this.window.Root, "g1", 0);
                RadioButton rb2 = new RadioButton(this.window.Root, "g1", 1);
                CheckButton cb5 = new CheckButton(this.window.Root);
                CheckButton cb6 = new CheckButton(this.window.Root);
                CheckButton cb7 = new CheckButton(this.window.Root);

                rb1.ActiveState = ActiveState.Yes;

                st1.Dock = DockStyle.Top;
                st1.Margins = new Margins(4, 4, 4, 4);
                cb1.Dock = DockStyle.Top;
                cb1.Margins = new Margins(4, 4, 4, 0);
                cb2.Dock = DockStyle.Top;
                cb2.Margins = new Margins(4, 4, 0, 0);
                cb3.Dock = DockStyle.Top;
                cb3.Margins = new Margins(4, 4, 0, 0);
                cb4.Dock = DockStyle.Top;
                cb4.Margins = new Margins(4, 4, 0, 0);

                rb1.Dock = DockStyle.Top;
                rb1.Margins = new Margins(4, 4, 4, 0);
                rb2.Dock = DockStyle.Top;
                rb2.Margins = new Margins(4, 4, 0, 0);

                cb5.Dock = DockStyle.Top;
                cb5.Margins = new Margins(4, 4, 4, 0);
                cb6.Dock = DockStyle.Top;
                cb6.Margins = new Margins(4, 4, 0, 0);
                cb7.Dock = DockStyle.Top;
                cb7.Margins = new Margins(4, 4, 0, 0);

                st1.Text = "Réglages pour le rendu du pavé de texte :";

                cb1.Name = "liga";
                cb1.Text = "ligatures simples";
                cb1.ActiveStateChanged += this.HandleCheckButtonActiveStateChanged;
                cb2.Name = "dlig";
                cb2.Text = "ligatures avancées";
                cb2.ActiveStateChanged += this.HandleCheckButtonActiveStateChanged;
                cb3.Name = "kern";
                cb3.Text = "crénage";
                cb3.ActiveStateChanged += this.HandleCheckButtonActiveStateChanged;
                cb4.Name = "Mgr=System";
                cb4.Text = "utilise GDI";
                cb4.ActiveStateChanged += this.HandleCheckButtonActiveStateChanged;

                rb1.Text = "paragraphes avec diverses justifications";
                rb1.ActiveStateChanged += this.HandleRadioButtonActiveStateChanged;
                rb2.Text = "paragraphes avec tabulateurs";
                rb2.ActiveStateChanged += this.HandleRadioButtonActiveStateChanged;

                cb5.Name = "equal frames";
                cb5.Text = "2 colonnes égales";
                cb5.ActiveStateChanged += this.HandleCheckButton5ActiveStateChanged;
                cb6.Name = "condition true";
                cb6.Text = "affiche texte conditionnel";
                cb6.ActiveStateChanged += this.HandleCheckButton6ActiveStateChanged;
                cb7.Name = "cursors";
                cb7.Text = "affiche la pos. des curseurs";
                cb7.ActiveStateChanged += this.HandleCheckButton7ActiveStateChanged;

                this.window.ClientSize = new Size(260, 180);
                this.window.Owner = owner;

                this.window.Show();
            }

            public void GenerateText()
            {
                this.painter.ResetTextStory();

                ICursor cursor = new SimpleCursor();
                this.painter.TextStory.NewCursor(cursor);

                ulong[] text;
                string words;

                Epsitec.Common.Text.TextStyle[] no_styles = new Epsitec.Common.Text.TextStyle[1];

                if (no_styles.Length > 0)
                {
                    System.Collections.ArrayList properties = new System.Collections.ArrayList();
                    properties.Add(new FontProperty("Arial", "Regular", this.features));
                    properties.Add(new FontSizeProperty(12.0, SizeUnits.Points));
                    properties.Add(new FontColorProperty("Black"));
                    no_styles[0] = this.painter.TextStory.StyleList.NewTextStyle(
                        null,
                        "Default",
                        TextStyleClass.Paragraph,
                        properties
                    );
                }

                if (this.active_test == 0)
                {
                    System.Collections.ArrayList properties = new System.Collections.ArrayList();

                    FontProperty fp;
                    properties.Clear();
                    fp = new FontProperty("Arial", "Regular", this.features);

                    properties.Add(fp);
                    properties.Add(new FontSizeProperty(12.0, SizeUnits.Points));
                    properties.Add(
                        new MarginsProperty(
                            60,
                            10,
                            10,
                            10,
                            SizeUnits.Points,
                            0.0,
                            0.0,
                            0.0,
                            15,
                            1,
                            ThreeState.True
                        )
                    );
                    properties.Add(new FontColorProperty("Black"));
                    properties.Add(new LanguageProperty("fr-ch", 1.0));
                    properties.Add(
                        new LeadingProperty(
                            10.0,
                            SizeUnits.Points,
                            15.0,
                            SizeUnits.Points,
                            5.0,
                            SizeUnits.Points,
                            AlignMode.None
                        )
                    );

                    words =
                        "Bonjour, ceci est un texte d'exemple permettant de vérifier le bon fonctionnement des divers algorithmes de découpe et d'affichage. Le nombre de mots moyen s'élève à environ 40 mots par paragraphe, ce qui correspond à des paragraphes de taille réduite. Quelle idée, un fjord finlandais ! Avocat.\nAWAY.\n______\n";

                    this.painter.TextStory.ConvertToStyledText(
                        words,
                        no_styles,
                        properties,
                        out text
                    );
                    this.painter.TextStory.InsertText(cursor, text);
#if true
                    properties.Clear();
                    fp = new FontProperty("Palatino Linotype", "Regular", this.features);

                    properties.Add(fp);
                    properties.Add(new FontSizeProperty(24.0, SizeUnits.Points));
                    properties.Add(
                        new MarginsProperty(
                            10,
                            10,
                            10,
                            10,
                            SizeUnits.Points,
                            1.0,
                            0.0,
                            0.0,
                            15,
                            1,
                            ThreeState.True
                        )
                    );
                    properties.Add(new FontColorProperty("Blue"));
                    properties.Add(new LanguageProperty("fr-ch", 1.0));
                    properties.Add(new LeadingProperty(24.0, SizeUnits.Points, AlignMode.All));
                    properties.Add(
                        new KeepProperty(
                            3,
                            2,
                            ParagraphStartMode.Anywhere,
                            ThreeState.False,
                            ThreeState.False
                        )
                    );

                    this.painter.TextStory.ConvertToStyledText(
                        words,
                        no_styles,
                        properties,
                        out text
                    );
                    this.painter.TextStory.InsertText(cursor, text);

                    properties.Add(new FontKernProperty(8, SizeUnits.Points));

                    this.painter.TextStory.ConvertToStyledText(
                        "Titre sur deux lignes pour voir\n",
                        no_styles,
                        properties,
                        out text
                    );
                    this.painter.TextStory.InsertText(cursor, text);

                    properties.Clear();
                    fp = new FontProperty("Times New Roman", "Italic", this.features);

                    properties.Add(fp);
                    properties.Add(new FontSizeProperty(12.0, SizeUnits.Points));
                    properties.Add(
                        new MarginsProperty(
                            0,
                            0,
                            0,
                            0,
                            SizeUnits.Points,
                            1.0,
                            1.0,
                            0.0,
                            15,
                            1,
                            ThreeState.False
                        )
                    );
                    properties.Add(new FontColorProperty("Red"));
                    properties.Add(
                        new LeadingProperty(
                            4.0,
                            SizeUnits.Millimeters,
                            1.0,
                            SizeUnits.Points,
                            1.0,
                            SizeUnits.Points,
                            AlignMode.First
                        )
                    );
                    properties.Add(
                        new KeepProperty(
                            3,
                            3,
                            ParagraphStartMode.Anywhere,
                            ThreeState.True,
                            ThreeState.False
                        )
                    );

                    this.painter.TextStory.ConvertToStyledText(
                        words,
                        no_styles,
                        properties,
                        out text
                    );
                    this.painter.TextStory.InsertText(cursor, text);

                    properties.Clear();
                    fp = new FontProperty("Arial", "Regular", this.features);

                    properties.Add(fp);
                    properties.Add(new FontSizeProperty(12.0, SizeUnits.Points));
                    properties.Add(
                        new MarginsProperty(
                            0,
                            0,
                            0,
                            0,
                            SizeUnits.Points,
                            0.0,
                            0.0,
                            0.5,
                            15,
                            1,
                            ThreeState.False
                        )
                    );
                    properties.Add(new FontColorProperty("Black"));
                    properties.Add(new ConditionalProperty("ShowTitle", true));

                    if (this.painter.Condition)
                    {
                        this.painter.TextStory.TextContext.SetCondition("ShowTitle");
                    }
                    else
                    {
                        this.painter.TextStory.TextContext.ClearCondition("ShowTitle");
                    }

                    this.painter.TextStory.ConvertToStyledText(
                        words,
                        no_styles,
                        properties,
                        out text
                    );
                    this.painter.TextStory.InsertText(cursor, text);

                    properties.Clear();
                    fp = new FontProperty("Arial", "Regular", this.features);
                    properties.Add(fp);
                    properties.Add(new FontSizeProperty(12.0, SizeUnits.Points));
                    properties.Add(
                        new MarginsProperty(
                            0,
                            0,
                            0,
                            0,
                            SizeUnits.Points,
                            0.4,
                            0.0,
                            1.0,
                            15,
                            1,
                            ThreeState.False
                        )
                    );
                    properties.Add(new FontColorProperty("Black"));
                    properties.Add(
                        new LeadingProperty(
                            double.NaN,
                            SizeUnits.None,
                            5.0,
                            SizeUnits.Points,
                            5.0,
                            SizeUnits.Points,
                            AlignMode.None
                        )
                    );

                    this.painter.TextStory.ConvertToStyledText(
                        words,
                        no_styles,
                        properties,
                        out text
                    );
                    this.painter.TextStory.InsertText(cursor, text);

                    words =
                        "Une phrase contenant un \"non\u2011breaking hyphen\" mais aussi un \"soft\u2010hyphen\" au milieu du mot \"Merk\u00ADwürdig\". Voici une césure mongloienne au milieu du mot \"Abra\u1806cadabra\".\n";

                    properties.Clear();
                    fp = new FontProperty("Verdana", "Regular", this.features);
                    properties.Add(fp);
                    properties.Add(new FontSizeProperty(16.0, SizeUnits.Points));
                    properties.Add(
                        new MarginsProperty(
                            0,
                            0,
                            0,
                            0,
                            SizeUnits.Points,
                            0.0,
                            0.0,
                            0.5,
                            15,
                            1,
                            ThreeState.True
                        )
                    );
                    properties.Add(new FontColorProperty("Black"));
                    properties.Add(
                        new LeadingProperty(
                            1.0,
                            SizeUnits.Percent,
                            5.0,
                            SizeUnits.Points,
                            5.0,
                            SizeUnits.Points,
                            AlignMode.None
                        )
                    );

                    this.painter.TextStory.ConvertToStyledText(
                        words,
                        no_styles,
                        properties,
                        out text
                    );
                    this.painter.TextStory.InsertText(cursor, text);

                    TextContext context = this.painter.TextStory.TextContext;
                    StyleList style_list = context.StyleList;
                    Epsitec.Common.Text.TextStyle style_normal = style_list.NewTextStyle(
                        null,
                        "Normal",
                        TextStyleClass.Paragraph,
                        properties
                    );

                    this.painter.TextStory.ConvertToStyledText(
                        "Un petit texte juste avant l'image >",
                        style_normal,
                        null,
                        out text
                    );
                    this.painter.TextStory.InsertText(cursor, text);

                    context.DefineResource("image1", new ImageRenderer(this.painter));

                    properties.Clear();
                    properties.Add(new ImageProperty("image1", context));

                    this.painter.TextStory.ConvertToStyledText(
                        "\uFFFC",
                        style_normal,
                        properties,
                        out text
                    );
                    this.painter.TextStory.InsertText(cursor, text);

                    this.painter.TextStory.ConvertToStyledText(
                        "< et la suite juste après, pour voir ce que ça donne dans un paragraphe.\n",
                        style_normal,
                        null,
                        out text
                    );
                    this.painter.TextStory.InsertText(cursor, text);

                    //	Un texte avec quelque passages soulignés...

                    double just = 1.0;
                    double disp = 0.0;

                    properties.Clear();
                    fp = new FontProperty("Verdana", "Regular", this.features);
                    properties.Add(fp);
                    properties.Add(new FontSizeProperty(16.0, SizeUnits.Points));
                    properties.Add(
                        new MarginsProperty(
                            0,
                            0,
                            0,
                            0,
                            SizeUnits.Points,
                            just,
                            0.0,
                            disp,
                            15,
                            1,
                            ThreeState.True
                        )
                    );
                    properties.Add(new FontColorProperty("Black"));
                    properties.Add(
                        new LeadingProperty(
                            double.NaN,
                            SizeUnits.None,
                            5.0,
                            SizeUnits.Points,
                            5.0,
                            SizeUnits.Points,
                            AlignMode.None
                        )
                    );

                    this.painter.TextStory.ConvertToStyledText(
                        "Un texte avec quelques ",
                        no_styles,
                        properties,
                        out text
                    );
                    this.painter.TextStory.InsertText(cursor, text);

                    properties.Clear();
                    fp = new FontProperty("Verdana", "Regular", this.features);
                    properties.Add(fp);
                    properties.Add(new FontSizeProperty(16.0, SizeUnits.Points));
                    properties.Add(
                        new MarginsProperty(
                            0,
                            0,
                            0,
                            0,
                            SizeUnits.Points,
                            just,
                            0.0,
                            disp,
                            15,
                            1,
                            ThreeState.True
                        )
                    );
                    properties.Add(new FontColorProperty("Black"));
                    properties.Add(
                        new UnderlineProperty(
                            double.NaN,
                            SizeUnits.None,
                            double.NaN,
                            SizeUnits.None,
                            "underline",
                            "Black"
                        )
                    );
                    properties.Add(
                        new LeadingProperty(
                            double.NaN,
                            SizeUnits.None,
                            5.0,
                            SizeUnits.Points,
                            5.0,
                            SizeUnits.Points,
                            AlignMode.None
                        )
                    );

                    this.painter.TextStory.ConvertToStyledText(
                        "passages soulignés",
                        no_styles,
                        properties,
                        out text
                    );
                    this.painter.TextStory.InsertText(cursor, text);

                    properties.Clear();
                    fp = new FontProperty("Verdana", "Regular", this.features);
                    properties.Add(fp);
                    properties.Add(new FontSizeProperty(16.0, SizeUnits.Points));
                    properties.Add(
                        new MarginsProperty(
                            0,
                            0,
                            0,
                            0,
                            SizeUnits.Points,
                            just,
                            0.0,
                            disp,
                            15,
                            1,
                            ThreeState.True
                        )
                    );
                    properties.Add(new FontColorProperty("Black"));
                    properties.Add(
                        new LeadingProperty(
                            double.NaN,
                            SizeUnits.None,
                            5.0,
                            SizeUnits.Points,
                            5.0,
                            SizeUnits.Points,
                            AlignMode.None
                        )
                    );

                    this.painter.TextStory.ConvertToStyledText(
                        " permettant de tester",
                        no_styles,
                        properties,
                        out text
                    );
                    this.painter.TextStory.InsertText(cursor, text);

                    properties.Clear();
                    fp = new FontProperty("Verdana", "Regular", this.features);
                    properties.Add(fp);
                    properties.Add(new FontSizeProperty(16.0, SizeUnits.Points));
                    properties.Add(
                        new MarginsProperty(
                            0,
                            0,
                            0,
                            0,
                            SizeUnits.Points,
                            just,
                            0.0,
                            disp,
                            15,
                            1,
                            ThreeState.True
                        )
                    );
                    properties.Add(new FontColorProperty("Black"));
                    properties.Add(new FontSizeProperty(0.75, SizeUnits.Percent));
                    properties.Add(new FontOffsetProperty(0.80, SizeUnits.Percent)); //	 80% de l'ascender à 75% de Verdana 16pt
                    properties.Add(
                        new LeadingProperty(
                            double.NaN,
                            SizeUnits.None,
                            5.0,
                            SizeUnits.Points,
                            5.0,
                            SizeUnits.Points,
                            AlignMode.None
                        )
                    );

                    this.painter.TextStory.ConvertToStyledText(
                        "(a)",
                        no_styles,
                        properties,
                        out text
                    );
                    this.painter.TextStory.InsertText(cursor, text);

                    properties.Clear();
                    fp = new FontProperty("Verdana", "Regular", this.features);
                    properties.Add(fp);
                    properties.Add(new FontSizeProperty(16.0, SizeUnits.Points));
                    properties.Add(
                        new MarginsProperty(
                            0,
                            0,
                            0,
                            0,
                            SizeUnits.Points,
                            just,
                            0.0,
                            disp,
                            15,
                            1,
                            ThreeState.True
                        )
                    );
                    properties.Add(new FontColorProperty("Black"));
                    properties.Add(
                        new LeadingProperty(
                            double.NaN,
                            SizeUnits.None,
                            5.0,
                            SizeUnits.Points,
                            5.0,
                            SizeUnits.Points,
                            AlignMode.None
                        )
                    );

                    this.painter.TextStory.ConvertToStyledText(
                        " le fonctionnement des divers ",
                        no_styles,
                        properties,
                        out text
                    );
                    this.painter.TextStory.InsertText(cursor, text);

                    properties.Clear();
                    fp = new FontProperty("Verdana", "Regular", this.features);
                    properties.Add(fp);
                    properties.Add(new FontSizeProperty(16.0, SizeUnits.Points));
                    properties.Add(
                        new MarginsProperty(
                            0,
                            0,
                            0,
                            0,
                            SizeUnits.Points,
                            just,
                            0.0,
                            disp,
                            15,
                            1,
                            ThreeState.True
                        )
                    );
                    properties.Add(new FontColorProperty("Black"));
                    properties.Add(
                        new UnderlineProperty(
                            double.NaN,
                            SizeUnits.None,
                            double.NaN,
                            SizeUnits.None,
                            "wave",
                            "Red"
                        )
                    );
                    properties.Add(
                        new LeadingProperty(
                            double.NaN,
                            SizeUnits.None,
                            5.0,
                            SizeUnits.Points,
                            5.0,
                            SizeUnits.Points,
                            AlignMode.None
                        )
                    );

                    this.painter.TextStory.ConvertToStyledText(
                        "algoritmes",
                        no_styles,
                        properties,
                        out text
                    );
                    this.painter.TextStory.InsertText(cursor, text);

                    properties.Clear();
                    fp = new FontProperty("Verdana", "Regular", this.features);
                    properties.Add(fp);
                    properties.Add(new FontSizeProperty(16.0, SizeUnits.Points));
                    properties.Add(
                        new MarginsProperty(
                            0,
                            0,
                            0,
                            0,
                            SizeUnits.Points,
                            just,
                            0.0,
                            disp,
                            15,
                            1,
                            ThreeState.True
                        )
                    );
                    properties.Add(new FontColorProperty("Black"));
                    properties.Add(
                        new UnderlineProperty(
                            double.NaN,
                            SizeUnits.None,
                            double.NaN,
                            SizeUnits.None,
                            "wave",
                            "Red"
                        )
                    );
                    properties.Add(new FontSizeProperty(0.75, SizeUnits.Percent));
                    properties.Add(new FontOffsetProperty(0.80, SizeUnits.Percent)); //	 80% de l'ascender à 75% de Verdana 16pt
                    properties.Add(
                        new LeadingProperty(
                            double.NaN,
                            SizeUnits.None,
                            5.0,
                            SizeUnits.Points,
                            5.0,
                            SizeUnits.Points,
                            AlignMode.None
                        )
                    );

                    this.painter.TextStory.ConvertToStyledText(
                        "\u2060(b)",
                        no_styles,
                        properties,
                        out text
                    );
                    this.painter.TextStory.InsertText(cursor, text);

                    properties.Clear();
                    fp = new FontProperty("Verdana", "Regular", this.features);
                    properties.Add(fp);
                    properties.Add(new FontSizeProperty(16.0, SizeUnits.Points));
                    properties.Add(
                        new MarginsProperty(
                            0,
                            0,
                            0,
                            0,
                            SizeUnits.Points,
                            just,
                            0.0,
                            disp,
                            15,
                            1,
                            ThreeState.True
                        )
                    );
                    properties.Add(new FontColorProperty("Black"));
                    properties.Add(
                        new LeadingProperty(
                            double.NaN,
                            SizeUnits.None,
                            5.0,
                            SizeUnits.Points,
                            5.0,
                            SizeUnits.Points,
                            AlignMode.None
                        )
                    );

                    this.painter.TextStory.ConvertToStyledText(
                        "...\n",
                        no_styles,
                        properties,
                        out text
                    );
                    this.painter.TextStory.InsertText(cursor, text);
#endif
#if false
					int glyph = 1;
					string symbol = "Symbol"; //"ZapfDingbats BT";
					
					while (glyph < 205)
					{
						for (int i = 0; (i < 15) && (glyph < 205); i++)
						{
							properties.Clear ();
							fp = new FontProperty (symbol, "Regular", this.features);
							properties.Add (fp);
							properties.Add (new FontSizeProperty (24.0, SizeUnits.Points));
							properties.Add (new MarginsProperty (0, 0, 0, 0, SizeUnits.Points, 0.0, 0.0, 0.0, 15, 1, ThreeState.False));
							properties.Add (new FontColorProperty (Drawing.Color.FromName ("Black")));
							properties.Add (new LeadingProperty (28.0, SizeUnits.Points, 0.0, SizeUnits.Points, 0.0, SizeUnits.Points, AlignMode.None));
							properties.Add (new Common.OpenTypeProperty (symbol, glyph++));
							
							this.painter.TextStory.ConvertToStyledText ("X", no_styles, properties, out text);
							this.painter.TextStory.InsertText (cursor, text);
						}
						
						properties.Clear ();
						fp = new FontProperty ("Verdana", "Regular", this.features);
						properties.Add (fp);
						properties.Add (new FontSizeProperty (16.0, SizeUnits.Points));
						properties.Add (new MarginsProperty (0, 0, 0, 0, SizeUnits.Points, 0.0, 0.0, 0.0, 15, 1, ThreeState.False));
						properties.Add (new FontColorProperty (Drawing.Color.FromName ("Black")));
						properties.Add (new LeadingProperty (16.0, SizeUnits.Points, 0.0, SizeUnits.Points, 0.0, SizeUnits.Points, AlignMode.None));
						
						this.painter.TextStory.ConvertToStyledText ("\n", no_styles, properties, out text);
						this.painter.TextStory.InsertText (cursor, text);
					}
#endif
                    this.painter.TextStory.MoveCursor(cursor, -this.painter.TextStory.TextLength);
                    text = new ulong[this.painter.TextStory.TextLength];
                    this.painter.TextStory.ReadText(cursor, text.Length, text);

                    System.Diagnostics.Debug.Write(this.painter.TextStory.GetDebugStyledText(text));
                }

                if (this.active_test == 1)
                {
                    double tab = 60;

                    System.Collections.ArrayList properties_1 = new System.Collections.ArrayList();
                    System.Collections.ArrayList properties_2 = new System.Collections.ArrayList();

                    TabList tabs = this.painter.TextStory.TextContext.TabList;
#if true
                    properties_1.Add(new FontProperty("Arial", "Regular", this.features));
                    properties_1.Add(new FontSizeProperty(12.0, SizeUnits.Points));
                    properties_1.Add(
                        new MarginsProperty(
                            0,
                            0,
                            0,
                            0,
                            SizeUnits.Points,
                            0.0,
                            0.0,
                            0.0,
                            15,
                            1,
                            ThreeState.False
                        )
                    );
                    properties_1.Add(
                        tabs.NewTab("T1", tab, SizeUnits.Points, 0, null, TabPositionMode.Absolute)
                    );
                    properties_1.Add(new FontColorProperty("Black"));
                    properties_1.Add(
                        new LeadingProperty(
                            18.0,
                            SizeUnits.Points,
                            10.0,
                            SizeUnits.Points,
                            0.0,
                            SizeUnits.Points,
                            AlignMode.None
                        )
                    );

                    properties_2.Add(new FontProperty("Arial", "Bold", this.features));
                    properties_2.Add(new FontSizeProperty(12.0, SizeUnits.Points));
                    properties_2.Add(
                        new MarginsProperty(
                            0,
                            0,
                            0,
                            0,
                            SizeUnits.Points,
                            0.0,
                            0.0,
                            0.0,
                            15,
                            1,
                            ThreeState.False
                        )
                    );
                    properties_2.Add(new FontColorProperty("Red"));
                    properties_2.Add(
                        new LeadingProperty(
                            18.0,
                            SizeUnits.Points,
                            10.0,
                            SizeUnits.Points,
                            0.0,
                            SizeUnits.Points,
                            AlignMode.None
                        )
                    );

                    this.painter.TextStory.ConvertToStyledText(
                        "Tabulateurs en folie\nTest:\t",
                        no_styles,
                        properties_1,
                        out text
                    );
                    this.painter.TextStory.InsertText(cursor, text);

                    this.painter.TextStory.ConvertToStyledText(
                        "Tab1",
                        no_styles,
                        properties_2,
                        out text
                    );
                    this.painter.TextStory.InsertText(cursor, text);

                    this.painter.TextStory.ConvertToStyledText(
                        "...\nCet exemple utilise un tabulateur aligné à gauche.\tTab2; enfin du texte pour la suite...\n\n",
                        no_styles,
                        properties_1,
                        out text
                    );
                    this.painter.TextStory.InsertText(cursor, text);
#endif
#if true
                    properties_1.Clear();
                    properties_2.Clear();

                    properties_1.Add(new FontProperty("Arial", "Regular", this.features));
                    properties_1.Add(new FontSizeProperty(12.0, SizeUnits.Points));
                    properties_1.Add(
                        new MarginsProperty(
                            0,
                            0,
                            0,
                            0,
                            SizeUnits.Points,
                            0.0,
                            0.0,
                            0.0,
                            15,
                            1,
                            ThreeState.False
                        )
                    );
                    properties_1.Add(
                        tabs.NewTab(
                            "T2",
                            tab,
                            SizeUnits.Points,
                            0.5,
                            null,
                            TabPositionMode.Absolute
                        )
                    );
                    properties_1.Add(new FontColorProperty("Black"));

                    properties_2.Add(new FontProperty("Arial", "Bold", this.features));
                    properties_2.Add(new FontSizeProperty(12.0, SizeUnits.Points));
                    properties_2.Add(
                        new MarginsProperty(
                            0,
                            0,
                            0,
                            0,
                            SizeUnits.Points,
                            0.0,
                            0.0,
                            0.0,
                            15,
                            1,
                            ThreeState.False
                        )
                    );
                    properties_2.Add(new FontColorProperty("Red"));

                    this.painter.TextStory.ConvertToStyledText(
                        "Test:\t",
                        no_styles,
                        properties_1,
                        out text
                    );
                    this.painter.TextStory.InsertText(cursor, text);

                    this.painter.TextStory.ConvertToStyledText(
                        "Tab3",
                        no_styles,
                        properties_2,
                        out text
                    );
                    this.painter.TextStory.InsertText(cursor, text);

                    this.painter.TextStory.ConvertToStyledText(
                        "...\nCet exemple utilise un tabulateur centré.\tTab4; enfin du texte pour la suite...\n\n",
                        no_styles,
                        properties_1,
                        out text
                    );
                    this.painter.TextStory.InsertText(cursor, text);
#endif
#if true
                    properties_1.Clear();
                    properties_2.Clear();

                    properties_1.Add(new FontProperty("Arial", "Regular", this.features));
                    properties_1.Add(new FontSizeProperty(12.0, SizeUnits.Points));
                    properties_1.Add(
                        new MarginsProperty(
                            0,
                            0,
                            0,
                            0,
                            SizeUnits.Points,
                            1.0,
                            0.0,
                            0.0,
                            15,
                            1,
                            ThreeState.True
                        )
                    );
                    properties_1.Add(
                        tabs.NewTab(
                            "T3",
                            tab,
                            SizeUnits.Points,
                            0.0,
                            null,
                            TabPositionMode.Absolute
                        )
                    );
                    properties_1.Add(new FontColorProperty("Black"));
                    properties_1.Add(new LanguageProperty("fr-ch", 1.0));

                    properties_2.Add(new FontProperty("Arial", "Bold", this.features));
                    properties_2.Add(new FontSizeProperty(12.0, SizeUnits.Points));
                    properties_2.Add(
                        new MarginsProperty(
                            0,
                            0,
                            0,
                            0,
                            SizeUnits.Points,
                            1.0,
                            0.0,
                            0.0,
                            15,
                            1,
                            ThreeState.False
                        )
                    );
                    properties_2.Add(new FontColorProperty("Red"));

                    this.painter.TextStory.ConvertToStyledText(
                        "Test:\t",
                        no_styles,
                        properties_1,
                        out text
                    );
                    this.painter.TextStory.InsertText(cursor, text);

                    this.painter.TextStory.ConvertToStyledText(
                        "Tab5",
                        no_styles,
                        properties_2,
                        out text
                    );
                    this.painter.TextStory.InsertText(cursor, text);

                    this.painter.TextStory.ConvertToStyledText(
                        "...\nCet exemple utilise un tabulateur aligné à gauche dans un paragraphe justifié."
                            + "\tTab6-a; enfin du texte pour la suite..."
                            + "\tTab6-b; et aussi pour afficher la fin.\n\n",
                        no_styles,
                        properties_1,
                        out text
                    );
                    this.painter.TextStory.InsertText(cursor, text);

                    properties_1.Clear();
                    properties_2.Clear();

                    properties_1.Add(new FontProperty("Arial", "Regular", this.features));
                    properties_1.Add(new FontSizeProperty(12.0, SizeUnits.Points));
                    properties_1.Add(
                        new MarginsProperty(
                            0,
                            0,
                            0,
                            0,
                            SizeUnits.Points,
                            0.0,
                            0.0,
                            1.0,
                            15,
                            1,
                            ThreeState.False
                        )
                    );
                    properties_1.Add(
                        tabs.NewTab(
                            "T4",
                            tab,
                            SizeUnits.Points,
                            0.0,
                            null,
                            TabPositionMode.Absolute
                        )
                    );
                    properties_1.Add(new FontColorProperty("Black"));

                    properties_2.Add(new FontProperty("Arial", "Bold", this.features));
                    properties_2.Add(new FontSizeProperty(12.0, SizeUnits.Points));
                    properties_2.Add(
                        new MarginsProperty(
                            0,
                            0,
                            0,
                            0,
                            SizeUnits.Points,
                            1.0,
                            0.0,
                            1.0,
                            15,
                            1,
                            ThreeState.False
                        )
                    );
                    properties_2.Add(new FontColorProperty("Red"));

                    this.painter.TextStory.ConvertToStyledText(
                        "Test:\t",
                        no_styles,
                        properties_1,
                        out text
                    );
                    this.painter.TextStory.InsertText(cursor, text);

                    this.painter.TextStory.ConvertToStyledText(
                        "Tab7",
                        no_styles,
                        properties_2,
                        out text
                    );
                    this.painter.TextStory.InsertText(cursor, text);

                    this.painter.TextStory.ConvertToStyledText(
                        "...\nCet exemple utilise un tabulateur aligné à gauche dans un paragraphe aligné à droite.\tTab8; enfin du texte pour la suite...\n\n",
                        no_styles,
                        properties_1,
                        out text
                    );
                    this.painter.TextStory.InsertText(cursor, text);
#endif
#if true
                    properties_1.Clear();
                    properties_2.Clear();

                    properties_1.Add(new FontProperty("Arial", "Regular", this.features));
                    properties_1.Add(new FontSizeProperty(12.0, SizeUnits.Points));
                    properties_1.Add(
                        new MarginsProperty(
                            0,
                            0,
                            0,
                            0,
                            SizeUnits.Points,
                            0.0,
                            0.0,
                            1.0,
                            15,
                            1,
                            ThreeState.False
                        )
                    );
                    properties_1.Add(
                        tabs.NewTab(
                            "T5",
                            tab,
                            SizeUnits.Points,
                            0.5,
                            null,
                            TabPositionMode.Absolute
                        )
                    );
                    properties_1.Add(new FontColorProperty("Black"));

                    properties_2.Add(new FontProperty("Arial", "Bold", this.features));
                    properties_2.Add(new FontSizeProperty(12.0, SizeUnits.Points));
                    properties_2.Add(
                        new MarginsProperty(
                            0,
                            0,
                            0,
                            0,
                            SizeUnits.Points,
                            1.0,
                            0.0,
                            1.0,
                            15,
                            1,
                            ThreeState.False
                        )
                    );
                    properties_2.Add(new FontColorProperty("Red"));

                    this.painter.TextStory.ConvertToStyledText(
                        "Test:\t",
                        no_styles,
                        properties_1,
                        out text
                    );
                    this.painter.TextStory.InsertText(cursor, text);

                    this.painter.TextStory.ConvertToStyledText(
                        "Tab9",
                        no_styles,
                        properties_2,
                        out text
                    );
                    this.painter.TextStory.InsertText(cursor, text);

                    this.painter.TextStory.ConvertToStyledText(
                        "...\nCet exemple utilise un tabulateur centré dans un paragraphe aligné à droite.\tTab-10 centré...\n\n",
                        no_styles,
                        properties_1,
                        out text
                    );
                    this.painter.TextStory.InsertText(cursor, text);
#endif
                    properties_1.Clear();
                    properties_1.Add(new FontProperty("Verdana", "Regular"));
                    properties_1.Add(new FontSizeProperty(12.0, SizeUnits.Points));
                    properties_1.Add(
                        new LeadingProperty(
                            18.0,
                            SizeUnits.Points,
                            10.0,
                            SizeUnits.Points,
                            0.0,
                            SizeUnits.Points,
                            AlignMode.None
                        )
                    );
                    properties_1.Add(
                        new MarginsProperty(
                            5.0,
                            5.0,
                            5.0,
                            5.0,
                            SizeUnits.Points,
                            1.0,
                            0.0,
                            0.0,
                            15,
                            5,
                            ThreeState.True
                        )
                    );
                    properties_1.Add(new FontColorProperty("Black"));

                    Epsitec.Common.Text.TextStyle style1 =
                        this.painter.TextStory.StyleList.NewTextStyle(
                            null,
                            "Normal",
                            TextStyleClass.Paragraph,
                            properties_1
                        );

                    Generator generator =
                        this.painter.TextStory.TextContext.GeneratorList.NewGenerator("liste");

                    generator.Add(
                        Generator.CreateSequence(Generator.SequenceType.Alphabetic, "", ")")
                    );

                    Epsitec.Common.Text.ParagraphManagers.ItemListManager.Parameters items =
                        new Epsitec.Common.Text.ParagraphManagers.ItemListManager.Parameters();

                    items.Generator = generator;
                    items.TabItem = tabs.NewTab(
                        "T10-item",
                        10.0,
                        SizeUnits.Points,
                        0,
                        null,
                        TabPositionMode.Absolute
                    );
                    items.TabBody = tabs.NewTab(
                        "T10-body",
                        tab,
                        SizeUnits.Points,
                        0,
                        null,
                        TabPositionMode.Absolute
                    );

                    ManagedParagraphProperty mp = new ManagedParagraphProperty(
                        "ItemList",
                        items.Save()
                    );

                    properties_1.Clear();
                    properties_1.Add(mp);
                    properties_1.Add(
                        new MarginsProperty(
                            0,
                            tab,
                            double.NaN,
                            double.NaN,
                            SizeUnits.Points,
                            1.0,
                            0.0,
                            0.0,
                            15,
                            5,
                            ThreeState.Undefined
                        )
                    );
                    properties_1.Add(new FontColorProperty("Navy"));

                    Epsitec.Common.Text.TextStyle style2 =
                        this.painter.TextStory.StyleList.NewTextStyle(
                            null,
                            "Puces",
                            TextStyleClass.Paragraph,
                            properties_1
                        );

                    words = "Voici une liste à puces pour faire un test.\n";

                    this.painter.TextStory.ConvertToStyledText(words, style1, null, out text);
                    this.painter.TextStory.InsertText(cursor, text);

                    Epsitec.Common.Text.TextNavigator navigator =
                        new Epsitec.Common.Text.TextNavigator(this.painter.TextFitter);

                    navigator.MoveTo(Epsitec.Common.Text.TextNavigator.Target.TextEnd, 1);
                    navigator.MoveTo(Epsitec.Common.Text.TextNavigator.Target.CharacterPrevious, 1); //	juste après le point final
                    navigator.SetParagraphStyles(style1, style2);
                    navigator.Insert("..\nComplément 1.\nComplément 2.");
                    navigator.MoveTo(Epsitec.Common.Text.TextNavigator.Target.CharacterNext, 1); //	fin du texte
                    navigator.SetParagraphStyles(style1);
                    navigator.Insert("Texte normal.\n");
                    navigator.MoveTo(Epsitec.Common.Text.TextNavigator.Target.ParagraphStart, 1); //	juste avant "Texte normal.\n"
                    navigator.MoveTo(Epsitec.Common.Text.TextNavigator.Target.CharacterPrevious, 2); //	juste avant le "." de la ligne précédente
                    navigator.StartSelection();
                    navigator.MoveTo(Epsitec.Common.Text.TextNavigator.Target.CharacterNext, 3); //	sélectionne ".\nT"
                    navigator.EndSelection();
                    navigator.Delete();
                    navigator.MoveTo(Epsitec.Common.Text.TextNavigator.Target.TextEnd, 1);
                    navigator.SetParagraphStyles(style1);
                    navigator.Insert("aaa\nbbb\n");
                    navigator.Insert("ccc");
                    navigator.SetParagraphStyles(style1, style2);
                    navigator.Insert("\n");
                    navigator.Insert("ddd\neee\nfff\n");
                    navigator.SetParagraphStyles(style1);
                    navigator.Insert("Texte final.\n");
                    navigator.MoveTo(Epsitec.Common.Text.TextNavigator.Target.ParagraphStart, 5); //	au début de "ccc"
                    navigator.StartSelection();
                    navigator.MoveTo(Epsitec.Common.Text.TextNavigator.Target.CharacterPrevious, 2); //	après "bb" et avant "b\n"
                    //-					navigator.MoveTo (Text.TextNavigator.Target.CharacterPrevious, 4);	//	avant "bbb\n"
                    navigator.EndSelection();
                    navigator.Delete();
                    navigator.MoveTo(Epsitec.Common.Text.TextNavigator.Target.ParagraphEnd, 0); //	après "ccc"
                    navigator.MoveTo(Epsitec.Common.Text.TextNavigator.Target.CharacterNext, 1); //	entre "e)" et "ddd"
                    navigator.StartSelection();
                    navigator.MoveTo(Epsitec.Common.Text.TextNavigator.Target.ParagraphEnd, 0); //	après "ddd" (sans la marque de fin de paragraphe)
                    navigator.MoveTo(Epsitec.Common.Text.TextNavigator.Target.CharacterNext, 1); //	y compris le "\n"
                    navigator.EndSelection();
                    navigator.Delete();
                    //					this.painter.TextStory.OpletQueue.UndoAction ();					//	remet 'ddd' tel quel
                    //					this.painter.TextStory.OpletQueue.UndoAction ();					//	annule move
                    //					this.painter.TextStory.OpletQueue.UndoAction ();					//	annule move
                    //					this.painter.TextStory.OpletQueue.UndoAction ();					//	remet 'bbb' tel quel
                    navigator.Dispose();

                    generator.UpdateAllFields(
                        this.painter.TextStory,
                        null,
                        System.Globalization.CultureInfo.CurrentCulture
                    );
                }

                this.painter.TextStory.RecycleCursor(cursor);

                this.painter.NotifyTextChanged();
            }

            private Painter painter;
            private Window window;
            private string[] features;
            private int active_test;

            private void HandleCheckButtonActiveStateChanged(object sender)
            {
                CheckButton cb = sender as CheckButton;

                string f = string.Join(":", this.features);

                f = f.Replace(cb.Name, "").Replace("::", ":");

                if (cb.IsActive)
                {
                    f = string.Concat(f, ":", cb.Name);
                }

                while (f.EndsWith(":"))
                {
                    f = f.Substring(0, f.Length - 1);
                }

                while (f.StartsWith(":"))
                {
                    f = f.Substring(1);
                }

                this.features = f.Split(':');

                System.Diagnostics.Debug.WriteLine("Features: " + f);

                this.GenerateText();
            }

            private void HandleCheckButton5ActiveStateChanged(object sender)
            {
                CheckButton cb = sender as CheckButton;

                if (cb.IsActive)
                {
                    this.painter.FrameRatio = 1.0 / 2.0;
                }
                else
                {
                    this.painter.FrameRatio = 1.0 / 3.0;
                }
            }

            private void HandleCheckButton6ActiveStateChanged(object sender)
            {
                CheckButton cb = sender as CheckButton;

                this.painter.Condition = cb.IsActive;
                this.GenerateText();
            }

            private void HandleCheckButton7ActiveStateChanged(object sender)
            {
                CheckButton cb = sender as CheckButton;

                this.painter.ShowCursors = cb.IsActive;
            }

            private void HandleRadioButtonActiveStateChanged(object sender)
            {
                RadioButton rb = sender as RadioButton;

                if (rb.IsActive)
                {
                    this.active_test = rb.Index;
                    this.GenerateText();
                }
            }
        }

        public class ImageRenderer : IGlyphRenderer
        {
            public ImageRenderer(Painter painter)
            {
                this.painter = painter;
            }

            #region IGlyphRenderer Members
            public bool GetGeometry(
                out double ascender,
                out double descender,
                out double advance,
                out double x1,
                out double x2
            )
            {
                //				System.Diagnostics.Debug.WriteLine ("ImageRenderer.GetGeometry called.");

                ascender = 40;
                descender = -20;
                advance = 100;
                x1 = 0;
                x2 = 100;

                return true;
            }

            public void RenderGlyph(ITextFrame frame, double x, double y)
            {
                //				System.Diagnostics.Debug.WriteLine (string.Format ("ImageRenderer.RenderGlyph called at {0}:{1}", x, y));

                Graphics graphics = this.painter.Graphics;

                graphics.AddFilledRectangle(x, y - 20, 100, 60);
                graphics.RenderSolid(Color.FromRgb(0, 1.0, 0.5));
            }
            #endregion

            private Painter painter;
        }
    }
}

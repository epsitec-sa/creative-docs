using System.Collections.Generic;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

namespace Epsitec.Common.Document
{
    public class MiniBarWindow : Window
    {
        public MiniBarWindow(double hot, double distance, System.Action<bool> closeMiniBarCallback)
            : base(WindowFlags.NoBorder | WindowFlags.HideFromTaskbar)
        {
            this.DisableMouseActivation();
            this.MakeLayeredWindow(true);
            this.Root.SyncPaint = true;
            this.Root.BackColor = Color.FromAlphaRgb(0, 1, 1, 1);

            this.balloon = new Balloon();
            this.balloon.Hot = hot;
            this.balloon.Distance = distance;
            this.balloon.SetParent(this.Root);
            this.balloon.Anchor = AnchorStyles.All;
            this.closeMiniBarCallback = closeMiniBarCallback;
        }

        public override Point WindowLocation
        {
            // this is set to have the MiniBarWindow origin at the center of the bottom side
            get { return base.WindowLocation - this.WindowLocationOffset; }
            set { base.WindowLocation = value + this.WindowLocationOffset; }
        }

        private Point WindowLocationOffset
        {
            get { return new Point(-this.WindowSize.Width / 2, -this.WindowSize.Height); }
        }

        public bool IsAway(Point mouse)
        {
            return this.balloon.IsAway(mouse);
        }

        public void ProcessMiniBarCommands(List<string> commands)
        {
            bool beginOfLine = true;
            Widget line = null;
            foreach (string cmd in commands)
            {
                if (beginOfLine)
                {
                    double m = (line == null) ? 0 : this.balloon.Margin;
                    IconButton button = new IconButton();
                    line = new Widget(this.balloon);
                    line.PreferredHeight = button.PreferredHeight;
                    line.Dock = DockStyle.Top;
                    line.Margins = new Margins(0, 0, m, 0);
                    beginOfLine = false;
                }

                if (cmd == "") // séparateur ?
                {
                    IconSeparator sep = new IconSeparator();
                    sep.PreferredWidth = MiniBarWindow.CommandWidth(cmd);
                    sep.Dock = DockStyle.Left;
                    sep.SetParent(line);
                }
                else if (cmd == "#") // fin de ligne ?
                {
                    beginOfLine = true;
                }
                else
                {
                    Command c = Common.Widgets.Command.Find(cmd);

                    IconButton button = new IconButton(c.CommandId, c.Icon, c.CommandId);

                    if (c.Statefull)
                    {
                        button.ButtonStyle = ButtonStyle.ActivableIcon;
                    }

                    button.PreferredWidth = MiniBarWindow.CommandWidth(cmd);
                    button.Dock = DockStyle.Left;
                    button.SetParent(line);
                    button.Clicked += this.HandleMiniBarButtonClicked;

                    ToolTip.Default.SetToolTip(button, c.GetDescriptionWithShortcut());
                }
            }
        }

        private void HandleMiniBarButtonClicked(object sender, MessageEventArgs e)
        {
            Widget button = sender as Widget;
            if (button != null)
            {
                if (
                    button.Name == "OrderUpAll"
                    || button.Name == "OrderUpOne"
                    || button.Name == "OrderDownOne"
                    || button.Name == "OrderDownAll"
                    || button.Name == Res.Commands.FontBold.CommandId
                    || button.Name == Res.Commands.FontItalic.CommandId
                    || button.Name == Res.Commands.FontUnderline.CommandId
                    || button.Name == Commands.FontOverline
                    || button.Name == Commands.FontStrikeout
                    || button.Name == Commands.FontSubscript
                    || button.Name == Commands.FontSuperscript
                    || button.Name == Commands.FontSizePlus
                    || button.Name == Commands.FontSizeMinus
                    || button.Name == Commands.FontClear
                    || button.Name == "ParagraphLeading08"
                    || button.Name == "ParagraphLeading10"
                    || button.Name == "ParagraphLeading15"
                    || button.Name == "ParagraphLeading20"
                    || button.Name == "ParagraphLeading30"
                    || button.Name == "ParagraphLeadingPlus"
                    || button.Name == "ParagraphLeadingMinus"
                    || button.Name == "ParagraphIndentPlus"
                    || button.Name == "ParagraphIndentMinus"
                    || button.Name == "ParagraphClear"
                    || button.Name == "JustifHLeft"
                    || button.Name == "JustifHCenter"
                    || button.Name == "JustifHRight"
                    || button.Name == "JustifHJustif"
                    || button.Name == "JustifHAll"
                )
                    return;
            }

            this.closeMiniBarCallback(false);
        }

        public static (int lines, double maxWidth) ComputeLinesAndMaxWidth(List<string> cmds)
        {
            int lines = 1;
            double maxWidth = 0;
            double width = 0;
            foreach (string cmd in cmds)
            {
                if (cmd == "#") // fin de ligne ?
                {
                    maxWidth = System.Math.Max(maxWidth, width);
                    width = 0;
                    lines++;
                }
                else // commande ou séparateur ?
                {
                    width += MiniBarWindow.CommandWidth(cmd);
                }
            }
            maxWidth = System.Math.Max(maxWidth, width);
            return (lines, maxWidth);
        }

        protected static double CommandWidth(string cmd)
        {
            //	Retourne la largeur du widget d'une commande.
            if (cmd == "") // séparateur ?
            {
                return 12;
            }
            else if (cmd == "#") // fin de ligne ?
            {
                return 0;
            }
            else // commande ?
            {
                return 22;
            }
        }

        public static void ComputeBestJustification(List<string> list)
        {
            //	Essaie différentes largeurs de justifications, pour retenir la meilleure,
            //	c'est-à-dire celle qui a le moins de déchets (place perdue sur la dernière ligne).
            double bestScraps = 10000;
            double bestHope = 8 * 22;
            int linesRequired = MiniBarWindow.Count(list) / 8 + 1;
            for (double hope = 2 * 22; hope <= 16 * 22; hope += 22)
            {
                if (MiniBarWindow.JustifDo(list, hope) == linesRequired)
                {
                    double scraps = MiniBarWindow.JustifScraps(list);
                    if (bestScraps > scraps)
                    {
                        bestScraps = scraps;
                        bestHope = hope;
                    }
                }

                MiniBarWindow.JustifClear(list);
            }
            MiniBarWindow.JustifDo(list, bestHope);
        }

        protected static int Count(IEnumerable<string> list)
        {
            //	Compte le nombre de commandes dans une liste.
            int count = 0;
            foreach (string cmd in list)
            {
                if (cmd != "")
                    count++;
            }
            return count;
        }

        protected static int JustifDo(List<string> list, double widthHope)
        {
            //	Justifie la mini-palette, en remplaçant certains séparateurs ("") par une marque
            //	de fin de ligne ("#").
            //	Retourne le nombre de lignes nécessaires.
            double width = 0;
            int lines = 1;
            for (int i = 0; i < list.Count; i++)
            {
                string cmd = list[i] as string;

                if (cmd == "") // séparateur ?
                {
                    if (width >= widthHope)
                    {
                        list.RemoveAt(i); // supprime le séparateur...
                        list.Insert(i, "#"); // ...et remplace-le par une marque de fin de ligne
                        width = 0;
                        lines++;
                    }
                    else
                    {
                        width += MiniBarWindow.CommandWidth(cmd);
                    }
                }
                else // commande ?
                {
                    width += MiniBarWindow.CommandWidth(cmd);
                }
            }
            return lines;
        }

        protected static void JustifClear(List<string> list)
        {
            //	Supprime la justification de la mini-palette.
            for (int i = 0; i < list.Count; i++)
            {
                string cmd = list[i] as string;

                if (cmd == "#") // fin de ligne d'un essai précédent ?
                {
                    list.RemoveAt(i); // supprime la marque de fin de ligne...
                    list.Insert(i, ""); // ...et remplace-la par un séparateur
                }
            }
        }

        protected static double JustifScraps(List<string> list)
        {
            //	Retourne la longueur inutilisée la plus grande. Il s'agit généralement de la place
            //	perdue à la fin de la dernière ligne.
            double shortestLine = 10000;
            double longestLine = 0;
            double width = 0;
            foreach (string cmd in list)
            {
                if (cmd == "#") // fin de ligne ?
                {
                    shortestLine = System.Math.Min(shortestLine, width);
                    longestLine = System.Math.Max(longestLine, width);
                    width = 0;
                }
                else // commande ou séparateur ?
                {
                    width += MiniBarWindow.CommandWidth(cmd);
                }
            }
            shortestLine = System.Math.Min(shortestLine, width);
            longestLine = System.Math.Max(longestLine, width);

            return longestLine - shortestLine;
        }

        private Balloon balloon;
        private System.Action<bool> closeMiniBarCallback;
    }
}

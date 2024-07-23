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
using System.Collections.Generic;

namespace Epsitec.Common.Widgets
{
    /// <summary>
    /// La classe ProgressIndicator implémente la barre d'avance.
    /// </summary>
    public class ProgressIndicator : Widget
    {
        public ProgressIndicator()
            : base() { }

        public ProgressIndicator(Widget embedder)
            : this()
        {
            this.SetEmbedder(embedder);
        }

        public ProgressIndicatorStyle ProgressStyle
        {
            //	Style graphique du widget.
            get { return this.progressStyle; }
            set { this.progressStyle = value; }
        }

        public double ProgressValue
        {
            //	Valeur de la progression (0..1).
            get { return this.progressValue; }
            set
            {
                if (this.progressValue != value)
                {
                    this.progressValue = value;
                    this.Invalidate();
                }
            }
        }

        public void UpdateProgress()
        {
            if (this.progressInitialTicks == 0)
            {
                this.progressInitialTicks = System.Environment.TickCount;
            }

            int delta = System.Environment.TickCount - this.progressInitialTicks;

            this.ProgressValue =
                (delta / (1000.0 * ProgressIndicator.AnimationDuration) + 0.5) % 1.0;
        }

        protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
        {
            IAdorner adorner = Widgets.Adorners.Factory.Active;
            adorner.PaintProgressIndicator(
                graphics,
                this.Client.Bounds,
                this.progressStyle,
                this.progressValue
            );
        }

        public static void Animate(ProgressIndicator indicator)
        {
            ProgressIndicator.animatedIndicators.Add(indicator);

            indicator.Disposed += _ =>
            {
                ProgressIndicator.animatedIndicators.Remove(indicator);

                if (ProgressIndicator.animatedIndicators.Count == 0)
                {
                    ProgressIndicator.animator.Stop();
                }
            };

            if (ProgressIndicator.animatedIndicators.Count == 1)
            {
                ProgressIndicator.animator.Start();
            }
        }

        private static void AnimateIndicators(double spin)
        {
            ProgressIndicator.animatedIndicators.ForEach(x => x.UpdateProgress());
        }

        static ProgressIndicator()
        {
            ProgressIndicator.animator.SetCallback<double>(ProgressIndicator.AnimateIndicators);
            ProgressIndicator.animator.SetValue(0.0, 1.0);
        }

        private static List<ProgressIndicator> animatedIndicators = new List<ProgressIndicator>();
        private static Animator animator = new Animator(1.0, AnimatorMode.AutoRestart);

        private const double AnimationDuration = 2.0;

        private ProgressIndicatorStyle progressStyle;
        private double progressValue;
        private int progressInitialTicks;
    }
}

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
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets.Platform;

[assembly: Epsitec.Common.Types.DependencyClass(typeof(Epsitec.Common.Widgets.ToolTip))]

namespace Epsitec.Common.Widgets
{
    /// <summary>
    /// La classe ToolTip implémente les "info bulles".
    /// </summary>
    public sealed class ToolTip : DependencyObject
    {
        private ToolTip()
        {
            this.hash = new Dictionary<long, object>();
            this.behaviour = ToolTipBehaviour.Normal;

            this.window = new Window(
                WindowFlags.NoBorder | WindowFlags.HideFromTaskbar | WindowFlags.AlwaysOnTop
            );
            this.window.Name = "$ToolTip";
            this.window.DisableMouseActivation();
            this.window.WindowBounds = new Drawing.Rectangle(0, 0, 8, 8);
            this.window.Root.SyncPaint = true;

            this.timer = new Timer();
            this.timer.TimeElapsed += this.HandleTimerTimeElapsed;
        }

        #region public safe API
        public ToolTipBehaviour Behaviour
        {
            get { return this.behaviour; }
            set { this.behaviour = value; }
        }

        public Drawing.Point InitialLocation
        {
            get { return this.initialPos; }
            set { this.initialPos = value; }
        }

        public void CloseToolTip()
        {
            this.window?.Dispose();
        }

        public void HideToolTip()
        {
            this.timer.Stop();

            if (this.IsDisplayed)
            {
                this.window.Hide();
                this.lastChangeTime = System.DateTime.Now;
            }
        }

        public void SetToolTip(Widget widget, string caption)
        {
            this.SetToolTipText(widget, caption);
            this.DefineToolTip(widget, caption);
        }

        public void SetToolTip(Widget widget, FormattedText caption)
        {
            this.SetToolTipText(widget, caption.ToString());
            this.DefineToolTip(widget, caption);
        }

        public void SetToolTip(Widget widget, Widget caption)
        {
            this.SetToolTipWidget(widget, caption);
            this.DefineToolTip(widget, caption);
        }

        public void SetToolTip(Widget widget, Caption caption)
        {
            this.SetToolTipCaption(widget, caption);
            this.DefineToolTip(widget, caption);
        }

        public void ClearToolTip(Widget widget)
        {
            this.SetToolTipText(widget, null);
            this.UnregisterWidget(widget);
        }

        public void SetToolTipColor(DependencyObject obj, Color value)
        {
            obj.SetValue(ToolTip.ToolTipColorProperty, value);
        }

        public void ClearToolTipColor(DependencyObject obj)
        {
            obj.ClearValue(ToolTip.ToolTipColorProperty);
        }

        #endregion

        #region event handlers
        private void HandleTimerTimeElapsed(object sender)
        {
            if (this.IsDisplayed)
            {
                this.HideToolTip();
                System.Diagnostics.Debug.Assert(this.IsDisplayed == false);
            }
            else
            {
                this.ShowToolTip();
                if (this.widget != null)
                {
                    this.RestartTimer(this.GetTooltipAutoCloseDelay(this.widget));
                }
            }
        }

        private void HandleWidgetEntered(object sender, MessageEventArgs e)
        {
            Widget widget = sender as Widget;

            this.AttachToWidget(widget);

            System.Diagnostics.Debug.Assert(this.widget != null);

            Drawing.Point mouse = this.widget.MapRootToClient(Message.CurrentState.LastPosition);

            if (this.ProcessToolTipHost(this.widget as Helpers.IToolTipHost, mouse))
            {
                return;
            }

            if (this.behaviour != ToolTipBehaviour.Manual)
            {
                this.DelayShow();
            }
        }

        private void HandleWidgetExited(object sender, MessageEventArgs e)
        {
            Widget widget = sender as Widget;

            if (this.behaviour != ToolTipBehaviour.Manual)
            {
                if (this.widget == widget)
                {
                    this.HideToolTip();
                    this.DetachFromWidget(widget);
                }
            }
        }

        private void HandleWidgetPreProcessing(object sender, MessageEventArgs e)
        {
            if (e.Message.MessageType != MessageType.MouseMove)
            {
                return;
            }
            bool done = this.ProcessToolTipHost(this.widget as Helpers.IToolTipHost, e.Point);
            if (done)
            {
                return;
            }
            if (!this.IsDisplayed)
            {
                return;
            }

            Drawing.Point mouse = Helpers.VisualTree.MapVisualToScreen(this.widget, e.Point);

            switch (this.behaviour)
            {
                case ToolTipBehaviour.Normal:
                    if (Point.Distance(mouse, this.birthPos) > ToolTip.hideDistance)
                    {
                        this.HideToolTip();
                        this.RestartTimer(SystemInformation.ToolTipShowDelay);
                    }
                    break;

                case ToolTipBehaviour.FollowMouse:
                    mouse += ToolTip.offset;
                    mouse.Y -= this.window.Root.ActualHeight;
                    this.window.WindowLocation = mouse;
                    break;

                case ToolTipBehaviour.Manual:
                    break;
            }
        }

        private void HandleWidgetDisposed(object sender)
        {
            Widget widget = sender as Widget;

            if (this.widget == widget)
            {
                this.DetachFromWidget(widget);
            }

            System.Diagnostics.Debug.Assert(this.widget != widget);
            //-			System.Diagnostics.Debug.Assert(this.hash.Contains(widget.GetVisualSerialId ()));

            if (this.HasToolTip(widget))
            {
                this.SetToolTipText(widget, null);
            }
            this.DefineToolTip(widget, null);
        }
        #endregion

        #region private unsafe implementation
        private bool IsDisplayed
        {
            get
            {
                if (this.window == null)
                {
                    return false;
                }
                return this.window.IsVisible;
            }
        }

        private void DefineToolTip(Widget widget, object caption)
        {
            if (this.hash == null)
            {
                return;
            }

            System.Diagnostics.Debug.Assert(widget != null);

            if ((this.hash.ContainsKey(widget.GetVisualSerialId())) && (caption == null))
            {
                this.UnregisterWidget(widget);
            }

            if ((caption == null) || (string.IsNullOrEmpty(caption.ToString())))
            {
                return;
            }

            if (
                caption is not string
                && caption is not Caption
                && caption is not FormattedText
                && caption is not Widget
            )
            {
                throw new System.ArgumentException(
                    "Specified tool tip caption is of type " + caption.GetType().FullName
                );
            }

            this.RegisterWidget(widget);

            this.hash[widget.GetVisualSerialId()] = caption;

            if ((this.widget == widget) && (this.IsDisplayed))
            {
                this.caption = caption;
                this.ShowToolTip(
                    this.birthPos,
                    this.caption,
                    ToolTip.GetDefaultToolTipColor(widget)
                );
            }
        }

        private void RegisterWidget(Widget widget)
        {
            if (this.hash.ContainsKey(widget.GetVisualSerialId()) == false)
            {
                widget.Entered += this.HandleWidgetEntered;
                widget.Exited += this.HandleWidgetExited;
                widget.Disposed += this.HandleWidgetDisposed;

                this.hash[widget.GetVisualSerialId()] = null;
            }
        }

        private void UnregisterWidget(Widget widget)
        {
            if (this.widget == widget)
            {
                this.DetachFromWidget(this.widget);
            }

            this.hash.Remove(widget.GetVisualSerialId());

            widget.Entered -= this.HandleWidgetEntered;
            widget.Exited -= this.HandleWidgetExited;
            widget.Disposed -= this.HandleWidgetDisposed;
        }

        private void AttachToWidget(Widget widget)
        {
            if (this.widget != null)
            {
                this.DetachFromWidget(this.widget);
            }

            System.Diagnostics.Debug.Assert(this.widget == null);
            System.Diagnostics.Debug.Assert(widget != null);

            this.widget = widget;
            this.caption = this.hash[this.widget.GetVisualSerialId()];
            this.hostProvidedCaption = null;

            this.widget.PreProcessing += this.HandleWidgetPreProcessing;
        }

        private void DetachFromWidget(Widget widget)
        {
            System.Diagnostics.Debug.Assert(this.widget == widget);
            System.Diagnostics.Debug.Assert(this.widget != null);

            this.widget.PreProcessing -= this.HandleWidgetPreProcessing;

            this.widget = null;
            this.caption = null;
        }

        private void RestartTimer(double delay)
        {
            this.timer.Suspend();
            this.timer.Period = delay;
            this.timer.Start();
        }

        private void DelayShow()
        {
            System.TimeSpan delta = System.DateTime.Now.Subtract(this.lastChangeTime);

            long deltaTicks = delta.Ticks;
            double deltaSeconds = (deltaTicks / System.TimeSpan.TicksPerMillisecond) / 1000.0;

            if (deltaSeconds < SystemInformation.ToolTipShowDelay)
            {
                this.RestartTimer(SystemInformation.ToolTipShowDelay / 10.0);
            }
            else
            {
                this.RestartTimer(SystemInformation.ToolTipShowDelay);
            }
        }

        private bool ProcessToolTipHost(Helpers.IToolTipHost host, Drawing.Point pos)
        {
            if (host != null)
            {
                object caption = host.GetToolTipCaption(pos);

                if (
                    (caption == this.hostProvidedCaption)
                    || ((caption != null) && caption.Equals(this.hostProvidedCaption))
                )
                {
                    return true;
                }

                if (this.hostProvidedCaption == null)
                {
                    this.DelayShow();
                }
                else if (caption == null)
                {
                    this.HideToolTip();
                }

                this.hostProvidedCaption = caption;
                this.caption = caption;

                if ((caption != null) && (this.IsDisplayed))
                {
                    Drawing.Point mouse = Helpers.VisualTree.MapVisualToScreen(this.widget, pos);
                    this.ShowToolTip(mouse, caption, ToolTip.GetDefaultToolTipColor(this.widget));
                    this.RestartTimer(this.GetTooltipAutoCloseDelay(this.widget));
                }

                return true;
            }

            return false;
        }

        private bool HasToolTip(DependencyObject obj)
        {
            if (
                (obj.ContainsValue(ToolTip.ToolTipTextProperty))
                || (obj.ContainsValue(ToolTip.ToolTipWidgetProperty))
                || (obj.ContainsValue(ToolTip.ToolTipCaptionProperty))
            )
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ShowToolTip()
        {
            if ((this.widget != null) && (this.caption != null))
            {
                try
                {
                    this.birthPos =
                        (this.behaviour == ToolTipBehaviour.Manual)
                            /**/? this.initialPos
                            : Message.CurrentState.LastScreenPosition;
                }
                catch (System.ObjectDisposedException)
                {
                    return;
                }

                this.ShowToolTip(
                    this.birthPos,
                    this.caption,
                    ToolTip.GetDefaultToolTipColor(this.widget)
                );
            }
        }

        private static Color GetDefaultToolTipColor(Widget widget)
        {
            if ((widget != null) && (widget.ContainsLocalValue(ToolTip.ToolTipColorProperty)))
            {
                return (Color)widget.GetLocalValue(ToolTip.ToolTipColorProperty);
            }
            else
            {
                return Color.Empty;
            }
        }

        private void ShowToolTip(Drawing.Point mouse, object caption, Color color)
        {
            if (this.widget == null)
            {
                return;
            }
            Widget tip;

            Caption realCaption = caption as Caption;
            string textCaption = caption as string;

            if (caption is FormattedText)
            {
                textCaption = ((FormattedText)caption).ToString();
            }

            if (realCaption != null)
            {
                textCaption = realCaption.Description;

                if (string.IsNullOrEmpty(textCaption))
                {
                    textCaption = realCaption.DefaultLabel;
                }

                if ((textCaption != null) && (textCaption.Length == 0))
                {
                    textCaption = null;
                }
            }

            if (textCaption != null)
            {
                tip = new Contents();

                if (string.IsNullOrEmpty(textCaption))
                {
                    textCaption = " ";
                }

                tip.Text = textCaption;
                tip.TextLayout.Alignment = Drawing.ContentAlignment.MiddleLeft;
                tip.BackColor = color;

                Drawing.Size size = tip.TextLayout.GetSingleLineSize();

                double dx = size.Width + ToolTip.margin.X * 2;
                double dy = size.Height + ToolTip.margin.Y * 2;

                tip.PreferredWidth = dx;
                tip.PreferredHeight = dy;
            }
            else if (caption is Widget)
            {
                tip = caption as Widget;
            }
            else
            {
                throw new System.InvalidOperationException("Caption neither a string nor a widget");
            }

            if (this.behaviour != ToolTipBehaviour.Manual)
            {
                mouse += ToolTip.offset;
            }

            mouse.Y -= tip.PreferredHeight;

            //	Modifie la position du tool-tip pour qu'il ne dépasse pas de l'écran.

            Rectangle wa = ScreenInfo.Find(mouse).WorkingArea;

            if (mouse.Y < wa.Bottom)
            {
                mouse.Y = wa.Bottom;
            }
            if (mouse.X + tip.PreferredWidth > wa.Right) // dépasse à droite ?
            {
                mouse.X = wa.Right - tip.PreferredWidth;
            }

            this.window.WindowBounds = new Rectangle(mouse, tip.PreferredSize);
            this.window.Owner = this.widget.Window;

            if (tip.Parent != this.window.Root)
            {
                tip.Dock = DockStyle.Fill;
                this.window.Root.Children.Clear();
                this.window.Root.Children.Add(tip);
            }

            if (this.IsDisplayed == false)
            {
                this.window.ShowWithoutFocus();
            }

            this.lastChangeTime = System.DateTime.Now;
        }

        private void AttachToolTipSource(DependencyObject sender)
        {
            Widget widget = sender as Widget;

            System.Diagnostics.Debug.Assert(widget != null);
            System.Diagnostics.Debug.Assert(
                this.hash.ContainsKey(widget.GetVisualSerialId()) == false
            );
        }

        private void DetachToolTipSource(DependencyObject sender)
        {
            Widget widget = sender as Widget;

            System.Diagnostics.Debug.Assert(widget != null);
            System.Diagnostics.Debug.Assert(
                this.hash.ContainsKey(widget.GetVisualSerialId()) == true
            );
        }

        private double GetTooltipAutoCloseDelay(DependencyObject obj)
        {
            //	Retourne le délai de fermeture pour le tooltip, en se basant sur une vitesse de lecture
            //	de 40 caractères/seconde. Le délai est toujours compris entre 5 et 20 secondes.
            //	Si le tooltip n'est pas défini avec un texte (par exemple un Caption ou un Widget), le délai
            //	est de 5 secondes.
            int length = this.GetTooltipTextLength(obj);
            return System.Math.Min(
                System.Math.Max(length / 40, SystemInformation.ToolTipAutoCloseDelay),
                SystemInformation.ToolTipAutoCloseDelay * 4
            );
        }

        private int GetTooltipTextLength(DependencyObject obj)
        {
            //	Retourne le nombre de caractères du texte, ou zéro si ce n'est pas un texte.
            string text = this.GetToolTipText(obj);

            if (string.IsNullOrEmpty(text))
            {
                return 0;
            }
            else
            {
                return text.Length;
            }
        }

        private string GetToolTipText(DependencyObject obj)
        {
            return obj.GetValue(ToolTip.ToolTipTextProperty) as string;
        }

        private void SetToolTipText(DependencyObject obj, string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                obj.ClearValue(ToolTip.ToolTipCaptionProperty);
                obj.ClearValue(ToolTip.ToolTipTextProperty);
                obj.ClearValue(ToolTip.ToolTipWidgetProperty);
            }
            else
            {
                obj.ClearValue(ToolTip.ToolTipCaptionProperty);
                obj.ClearValue(ToolTip.ToolTipWidgetProperty);
                obj.SetValue(ToolTip.ToolTipTextProperty, value);
            }
        }

        private void SetToolTipWidget(DependencyObject obj, Widget value)
        {
            if (value == null)
            {
                obj.ClearValue(ToolTip.ToolTipCaptionProperty);
                obj.ClearValue(ToolTip.ToolTipTextProperty);
                obj.ClearValue(ToolTip.ToolTipWidgetProperty);
            }
            else
            {
                obj.ClearValue(ToolTip.ToolTipCaptionProperty);
                obj.ClearValue(ToolTip.ToolTipTextProperty);
                obj.SetValue(ToolTip.ToolTipWidgetProperty, value);
            }
        }

        private void SetToolTipCaption(DependencyObject obj, Caption value)
        {
            if (value == null)
            {
                obj.ClearValue(ToolTip.ToolTipCaptionProperty);
                obj.ClearValue(ToolTip.ToolTipTextProperty);
                obj.ClearValue(ToolTip.ToolTipWidgetProperty);
            }
            else
            {
                obj.SetValue(ToolTip.ToolTipCaptionProperty, value);
                obj.ClearValue(ToolTip.ToolTipTextProperty);
                obj.ClearValue(ToolTip.ToolTipWidgetProperty);
            }
        }

        #endregion

        #region Contents Class
        public sealed class Contents : Widget
        {
            public Contents() { }

            protected override void PaintBackgroundImplementation(
                Drawing.Graphics graphics,
                Drawing.Rectangle clipRect
            )
            {
                IAdorner adorner = Widgets.Adorners.Factory.Active;

                Drawing.Rectangle rect = this.Client.Bounds;
                Drawing.Point pos = new Drawing.Point(ToolTip.margin.X, 0);
                Drawing.Color color = this.BackColor;

                adorner.PaintTooltipBackground(graphics, rect, color);

                if (this.TextLayout != null)
                {
                    adorner.PaintTooltipTextLayout(graphics, pos, this.TextLayout);
                }

                base.PaintBackgroundImplementation(graphics, clipRect);
            }
        }
        #endregion

        #region PrivateDependencyPropertyMetadata Class

        private sealed class PrivateDependencyPropertyMetadata : DependencyPropertyMetadata
        {
            public PrivateDependencyPropertyMetadata() { }

            protected override void OnPropertyInvalidated(
                DependencyObject sender,
                object oldValue,
                object newValue
            )
            {
                base.OnPropertyInvalidated(sender, oldValue, newValue);

                if (oldValue == null)
                {
                    //	A tool tip is being defined for the first time.

                    ToolTip.Default.AttachToolTipSource(sender);
                }
                else if (newValue == null)
                {
                    //	A previously defined tool tip is being cleared.

                    ToolTip.Default.DetachToolTipSource(sender);
                }
            }

            protected override DependencyPropertyMetadata CloneNewObject()
            {
                return new PrivateDependencyPropertyMetadata();
            }

            public static readonly PrivateDependencyPropertyMetadata Default =
                new PrivateDependencyPropertyMetadata();
        }

        #endregion

        public static readonly DependencyProperty ToolTipTextProperty =
            DependencyProperty<ToolTip>.RegisterAttached<string>(
                "ToolTipText",
                PrivateDependencyPropertyMetadata.Default
            );
        public static readonly DependencyProperty ToolTipWidgetProperty =
            DependencyProperty<ToolTip>.RegisterAttached<Widget>(
                "ToolTipWidget",
                PrivateDependencyPropertyMetadata.Default
            );
        public static readonly DependencyProperty ToolTipCaptionProperty =
            DependencyProperty<ToolTip>.RegisterAttached<Caption>(
                "ToolTipCaption",
                PrivateDependencyPropertyMetadata.Default
            );
        public static readonly DependencyProperty ToolTipColorProperty =
            DependencyProperty<ToolTip>.RegisterAttached<Color>(
                "ToolTipColor",
                PrivateDependencyPropertyMetadata.Default
            );

        private readonly Dictionary<long, object> hash;
        private ToolTipBehaviour behaviour;

        private Window window;
        private Timer timer;
        private System.DateTime lastChangeTime;

        private Widget widget;
        private object caption;
        private object hostProvidedCaption;

        //private object refreshedCaption;

        private Drawing.Point birthPos;
        private Drawing.Point initialPos;

        private static readonly double hideDistance = 24;
        private static readonly Drawing.Point margin = new Drawing.Point(3, 2);
        private static readonly Drawing.Point offset = new Drawing.Point(8, -16);

        private static readonly ToolTip defaultToolTip = new ToolTip();

        public static ToolTip Default
        {
            get { return ToolTip.defaultToolTip; }
        }
    }
}

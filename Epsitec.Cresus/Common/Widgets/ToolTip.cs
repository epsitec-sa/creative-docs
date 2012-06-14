//	Copyright © 2003-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

using System.Collections.Generic;

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Widgets.ToolTip))]

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe ToolTip implémente les "info bulles".
	/// </summary>
	public sealed class ToolTip : DependencyObject
	{
		private ToolTip()
		{
			this.hash = new Dictionary<long, object> ();
			this.behaviour = ToolTipBehaviour.Normal;

			this.window = new Window ();
			this.window.MakeFramelessWindow ();
			this.window.MakeFloatingWindow ();
			this.window.Name = "$ToolTip";
			this.window.DisableMouseActivation ();
			this.window.WindowBounds = new Drawing.Rectangle (0, 0, 8, 8);
			this.window.Root.SyncPaint = true;
			
			this.timer = new Timer ();
			this.timer.TimeElapsed += this.HandleTimerTimeElapsed;
		}

		
		public ToolTipBehaviour					Behaviour
		{
			get
			{
				return this.behaviour;
			}
			set
			{
				this.behaviour = value;
			}
		}

		public Drawing.Point					InitialLocation
		{
			get
			{
				return this.initialPos;
			}
			set
			{
				this.initialPos = value;
			}
		}
		
		
		public static readonly ToolTip			Default = new ToolTip ();


		public static void HideAllToolTips()
		{
			ToolTip.Default.HideToolTip ();
		}

		public static bool HasToolTip(DependencyObject obj)
		{
			if ((obj.ContainsValue (ToolTip.ToolTipTextProperty)) ||
				(obj.ContainsValue (ToolTip.ToolTipWidgetProperty)) ||
				(obj.ContainsValue (ToolTip.ToolTipCaptionProperty)))
			{
				return true;
			}
			else
			{
				return false;
			}
		}
		
		
		public void ShowToolTipForWidget(Widget widget)
		{
			if (ToolTip.HasToolTip (widget))
			{
				this.HideToolTip ();
				this.AttachToWidget (widget);
				this.ShowToolTip ();
			}
		}
		
		public void HideToolTipForWidget(Widget widget)
		{
			if ((this.widget == widget) &&
				(widget != null))
			{
				this.HideToolTip ();
			}
		}

		public void RegisterDynamicToolTipHost(Widget widget)
		{
			Helpers.IToolTipHost host = widget as Helpers.IToolTipHost;

			if (host == null)
			{
				throw new System.ArgumentException ("Widget does not implement IToolTipHost");
			}

			this.RegisterWidget (widget);
		}

		public void UnregisterDynamicToolTipHost(Widget widget)
		{
			Helpers.IToolTipHost host = widget as Helpers.IToolTipHost;

			if (host == null)
			{
				throw new System.ArgumentException ("Widget does not implement IToolTipHost");
			}

			this.UnregisterWidget (widget);
		}
		
		public void UpdateManualToolTip(Drawing.Point mouse, string caption, Color color)
		{
			if (this.behaviour == ToolTipBehaviour.Manual)
			{
				this.ShowToolTip (mouse, caption, color);
			}
		}
		
		public void UpdateManualToolTip(Drawing.Point mouse, Widget caption)
		{
			if (this.behaviour == ToolTipBehaviour.Manual)
			{
				this.ShowToolTip (mouse, caption, Color.Empty);
			}
		}

		public void RefreshToolTip(Widget widget, Drawing.Point mouse)
		{
			if (this.widget == widget)
			{
				this.ProcessToolTipHost (this.widget as Helpers.IToolTipHost, mouse);

				if ((!this.isDisplayed) &&
					(this.hostProvidedCaption != this.refreshedCaption))
				{
					this.refreshedCaption = this.hostProvidedCaption;
					this.ShowToolTip ();
					this.RestartTimer (ToolTip.GetTooltipAutoCloseDelay (widget));
				}
			}
		}

		
		public void SetToolTip(Widget widget, string caption)
		{
			ToolTip.SetToolTipText (widget, caption);
			this.DefineToolTip (widget, caption);
		}

		public void SetToolTip(Widget widget, FormattedText caption)
		{
			ToolTip.SetToolTipText (widget, caption.ToString ());
			this.DefineToolTip (widget, caption);
		}

		public void SetToolTip(Widget widget, Widget caption)
		{
			ToolTip.SetToolTipWidget (widget, caption);
			this.DefineToolTip (widget, caption);
		}

		public void SetToolTip(Widget widget, Caption caption)
		{
			ToolTip.SetToolTipCaption (widget, caption);
			this.DefineToolTip (widget, caption);
		}

		public void ClearToolTip(Widget widget)
		{
			ToolTip.SetToolTipText (widget, null);
			this.UnregisterWidget (widget);
		}


		private void DefineToolTip(Widget widget, object caption)
		{
			if (this.hash == null)
			{
				return;
			}
			
			System.Diagnostics.Debug.Assert (widget != null);
			
			if ((this.hash.ContainsKey (widget.GetVisualSerialId ())) &&
				(caption == null))
			{
				this.UnregisterWidget (widget);
			}

			if ((caption == null) ||
				(string.IsNullOrEmpty (caption.ToString ())))
			{
				return;
			}

			if ((caption is string) ||
				(caption is Caption) ||
				(caption is FormattedText) ||
				(caption is Widget))
			{
				//	OK.
			}
			else
			{
				throw new System.ArgumentException ("Specified tool tip caption is of type " + caption.GetType ().FullName);
			}

			this.RegisterWidget (widget);

			this.hash[widget.GetVisualSerialId ()] = caption;
			
			if ((this.widget == widget) &&
				(this.isDisplayed))
			{
				this.caption = caption;
				this.ShowToolTip (this.birthPos, this.caption, ToolTip.GetDefaultToolTipColor (widget));
			}
		}

		private void RegisterWidget(Widget widget)
		{
			if (this.hash.ContainsKey (widget.GetVisualSerialId ()) == false)
			{
				widget.Entered   += this.HandleWidgetEntered;
				widget.Exited    += this.HandleWidgetExited;
				widget.Disposed  += this.HandleWidgetDisposed;

				this.hash[widget.GetVisualSerialId ()] = null;
			}
		}

		private void UnregisterWidget(Widget widget)
		{
			if (this.widget == widget)
			{
				this.DetachFromWidget (this.widget);
			}

			this.hash.Remove (widget.GetVisualSerialId ());

			widget.Entered  -= this.HandleWidgetEntered;
			widget.Exited   -= this.HandleWidgetExited;
			widget.Disposed -= this.HandleWidgetDisposed;
		}
		
		private void AttachToWidget(Widget widget)
		{
			if (this.widget != null)
			{
				this.DetachFromWidget (this.widget);
			}
			
			System.Diagnostics.Debug.Assert (this.widget == null);
			System.Diagnostics.Debug.Assert (widget != null);
			
			this.widget  = widget;
			this.caption = this.hash[this.widget.GetVisualSerialId ()];
			this.hostProvidedCaption = null;
			
			this.widget.PreProcessing += this.HandleWidgetPreProcessing;
		}
		
		private void DetachFromWidget(Widget widget)
		{
			System.Diagnostics.Debug.Assert (this.widget == widget);
			System.Diagnostics.Debug.Assert (this.widget != null);
			
			this.widget.PreProcessing -= this.HandleWidgetPreProcessing;
			
			this.widget  = null;
			this.caption = null;
		}
		
		
		private void RestartTimer(double delay)
		{
			this.timer.Suspend ();
			this.timer.Delay = delay;
			this.timer.Start();
		}
		
		private void DelayShow()
		{
			System.TimeSpan delta = System.DateTime.Now.Subtract (this.lastChangeTime);
			
			long   deltaTicks   = delta.Ticks;
			double deltaSeconds = (deltaTicks / System.TimeSpan.TicksPerMillisecond) / 1000.0;
			
			if (deltaSeconds < SystemInformation.ToolTipShowDelay)
			{
				this.RestartTimer (SystemInformation.ToolTipShowDelay / 10.0);
			}
			else
			{
				this.RestartTimer (SystemInformation.ToolTipShowDelay);
			}
		}
		
		
		private bool ProcessToolTipHost(Helpers.IToolTipHost host, Drawing.Point pos)
		{
			if (host != null)
			{
				object caption = host.GetToolTipCaption (pos);
				
				if ((caption == this.hostProvidedCaption) ||
					((caption != null) && caption.Equals (this.hostProvidedCaption)))
				{
					return true;
				}
				
				if (this.hostProvidedCaption == null)
				{
					this.DelayShow ();
				}
				else if (caption == null)
				{
					this.HideToolTip ();
				}
				
				this.hostProvidedCaption = caption;
				this.caption = caption;
				
				if ((caption != null) &&
					(this.isDisplayed))
				{
					Drawing.Point mouse = Helpers.VisualTree.MapVisualToScreen (this.widget, pos);
					this.ShowToolTip (mouse, caption, ToolTip.GetDefaultToolTipColor (this.widget));
					this.RestartTimer (ToolTip.GetTooltipAutoCloseDelay (this.widget));
				}
				
				return true;
			}
			
			return false;
		}
		
		
		private void HandleWidgetEntered(object sender, MessageEventArgs e)
		{
			Widget widget = sender as Widget;
			
//-			System.Diagnostics.Debug.WriteLine ("HandleWidgetEntered: " + widget.ToString ());
			
			this.AttachToWidget (widget);
			
			System.Diagnostics.Debug.Assert (this.widget != null);

			Drawing.Point mouse = this.widget.MapRootToClient (Message.CurrentState.LastPosition);
			
			if (this.ProcessToolTipHost (this.widget as Helpers.IToolTipHost, mouse))
			{
				return;
			}
			
			if (this.behaviour != ToolTipBehaviour.Manual)
			{
				this.DelayShow ();
			}
		}

		private void HandleWidgetExited(object sender, MessageEventArgs e)
		{
			Widget widget = sender as Widget;
			
			if (this.behaviour != ToolTipBehaviour.Manual)
			{
//-				System.Diagnostics.Debug.WriteLine ("HandleWidgetExited: " + widget.ToString ());
				if (this.widget == widget)
				{
					this.HideToolTip ();
					this.DetachFromWidget (widget);
				}
			}
		}
		
		private void HandleWidgetPreProcessing(object sender, MessageEventArgs e)
		{
			if ((e.Message.MessageType == MessageType.MouseMove) &&
				(this.ProcessToolTipHost (this.widget as Helpers.IToolTipHost, e.Point)))
			{
				return;
			}

			if ((this.isDisplayed) &&
				(e.Message.MessageType == MessageType.MouseMove))
			{
				Drawing.Point mouse = Helpers.VisualTree.MapVisualToScreen (this.widget, e.Point);
				
				switch (this.behaviour)
				{
					case ToolTipBehaviour.Normal:
						if (Drawing.Point.Distance (mouse, this.birthPos) > ToolTip.hideDistance)
						{
							this.HideToolTip ();
							this.RestartTimer (SystemInformation.ToolTipShowDelay);
						}
						break;

					case ToolTipBehaviour.FollowMouse:
						mouse   += ToolTip.offset;
						mouse.Y -= this.window.Root.ActualHeight;
						this.window.WindowLocation = mouse;
						break;
						
					case ToolTipBehaviour.Manual:
						break;
				}
			}
		}
		
		private void HandleWidgetDisposed(object sender)
		{
			Widget widget = sender as Widget;
			
			if (this.widget == widget)
			{
				this.DetachFromWidget (widget);
			}
			
			System.Diagnostics.Debug.Assert(this.widget != widget);
//-			System.Diagnostics.Debug.Assert(this.hash.Contains(widget.GetVisualSerialId ()));

			if (ToolTip.HasToolTip (widget))
			{
				ToolTip.SetToolTipText (widget, null);
			}
			this.DefineToolTip (widget, null);
		}
		
		private void HandleTimerTimeElapsed(object sender)
		{
			if (this.isDisplayed)
			{
				this.HideToolTip ();
				System.Diagnostics.Debug.Assert (this.isDisplayed == false);
			}
			else
			{
				this.ShowToolTip ();
				this.RestartTimer (ToolTip.GetTooltipAutoCloseDelay (this.widget));
			}
		}
		
		
		private void ShowToolTip()
		{
			if ((this.widget != null) &&
				(this.caption != null))
			{
				this.birthPos = (this.behaviour == ToolTipBehaviour.Manual)
					/**/	   ? this.initialPos
					/**/	   : Message.CurrentState.LastWindow.MapWindowToScreen (Message.CurrentState.LastPosition);

				this.ShowToolTip (this.birthPos, this.caption, ToolTip.GetDefaultToolTipColor (this.widget));
			}
		}

		private static Color GetDefaultToolTipColor(Widget widget)
		{
			if ((widget != null) &&
				(widget.ContainsLocalValue (ToolTip.ToolTipColorProperty)))
			{
				return (Color) widget.GetLocalValue (ToolTip.ToolTipColorProperty);
			}
			else
			{
				return Color.Empty;
			}
		}
		
		private void ShowToolTip(Drawing.Point mouse, object caption, Color color)
		{
			if (Application.IsRunningOnMainUIThread == false)
			{
				return;
			}

			Widget tip = null;
			
			Caption realCaption = caption as Caption;
			string  textCaption = caption as string;
			
			if (caption is FormattedText)
			{
				textCaption = ((FormattedText)caption).ToString ();
			}

			if (realCaption != null)
			{
				textCaption = realCaption.Description;

				if (string.IsNullOrEmpty (textCaption))
				{
					textCaption = realCaption.DefaultLabel;
				}

				if ((textCaption != null) &&
					(textCaption.Length == 0))
				{
					textCaption = null;
				}
			}
			
			if (textCaption != null)
			{
				tip = new Contents ();

				if (string.IsNullOrEmpty (textCaption))
				{
					textCaption = " ";
				}

				tip.Text                 = textCaption;
				tip.TextLayout.Alignment = Drawing.ContentAlignment.MiddleLeft;
				tip.BackColor            = color;

				Drawing.Size size = tip.TextLayout.GetSingleLineSize ();
				
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
				throw new System.InvalidOperationException ("Caption neither a string nor a widget");
			}
			
			if (this.behaviour != ToolTipBehaviour.Manual)
			{
				mouse += ToolTip.offset;
			}
			
			mouse.Y -= tip.PreferredHeight;
			
			//	Modifie la position du tool-tip pour qu'il ne dépasse pas de l'écran.
			
			Drawing.Rectangle wa = ScreenInfo.Find (mouse).WorkingArea;
			
			if (mouse.Y < wa.Bottom)
			{
				mouse.Y = wa.Bottom;
			}
			if (mouse.X + tip.PreferredWidth > wa.Right)  // dépasse à droite ?
			{
				mouse.X = wa.Right - tip.PreferredWidth;
			}
			
			this.window.WindowBounds = new Drawing.Rectangle (mouse, tip.PreferredSize);
			this.window.Owner        = this.widget.Window;
			
			if (tip.Parent != this.window.Root)
			{
				tip.Dock = DockStyle.Fill;
				this.window.Root.Children.Clear ();
				this.window.Root.Children.Add (tip);
			}
			
			if (this.isDisplayed == false)
			{
				this.window.Show ();
				this.isDisplayed = true;
			}
			
			this.lastChangeTime = System.DateTime.Now;
		}

		private void HideToolTip()
		{
			if (Application.IsRunningOnMainUIThread == false)
			{
				return;
			}

			this.timer.Stop ();

			if (this.isDisplayed)
			{
				this.window.Hide ();
				this.isDisplayed = false;
				this.lastChangeTime = System.DateTime.Now;
			}
		}


		internal void AttachToolTipSource(DependencyObject sender)
		{
			Widget widget = sender as Widget;

			System.Diagnostics.Debug.Assert (widget != null);
			System.Diagnostics.Debug.Assert (this.hash.ContainsKey (widget.GetVisualSerialId ()) == false);
		}

		internal void DetachToolTipSource(DependencyObject sender)
		{
			Widget widget = sender as Widget;

			System.Diagnostics.Debug.Assert (widget != null);
			System.Diagnostics.Debug.Assert (this.hash.ContainsKey (widget.GetVisualSerialId ()) == true);
		}
		
		#region Contents Class
		public sealed class Contents : Widget
		{
			public Contents()
			{
			}
			
			
			protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
			{
				IAdorner adorner = Widgets.Adorners.Factory.Active;
				
				Drawing.Rectangle rect  = this.Client.Bounds;
				Drawing.Point     pos   = new Drawing.Point (ToolTip.margin.X, 0);
				Drawing.Color     color = this.BackColor;
				
				adorner.PaintTooltipBackground (graphics, rect, color);
				
				if (this.TextLayout != null)
				{
					adorner.PaintTooltipTextLayout (graphics, pos, this.TextLayout);
				}
				
				base.PaintBackgroundImplementation (graphics, clipRect);
			}
		}
		#endregion

		#region PrivateDependencyPropertyMetadata Class

		private sealed class PrivateDependencyPropertyMetadata : DependencyPropertyMetadata
		{
			public PrivateDependencyPropertyMetadata()
			{
			}

			protected override void OnPropertyInvalidated(DependencyObject sender, object oldValue, object newValue)
			{
				base.OnPropertyInvalidated (sender, oldValue, newValue);

				if (oldValue == null)
				{
					//	A tool tip is being defined for the first time.

					ToolTip.Default.AttachToolTipSource (sender);
				}
				else if (newValue == null)
				{
					//	A previously defined tool tip is being cleared.
					
					ToolTip.Default.DetachToolTipSource (sender);
				}
			}

			protected override DependencyPropertyMetadata CloneNewObject()
			{
				return new PrivateDependencyPropertyMetadata ();
			}

			public static readonly PrivateDependencyPropertyMetadata Default = new PrivateDependencyPropertyMetadata ();
		}

		#endregion

		public static string GetToolTipText(DependencyObject obj)
		{
			return obj.GetValue (ToolTip.ToolTipTextProperty) as string;
		}

		public static bool HasToolTipText(DependencyObject obj)
		{
			return obj.ContainsValue (ToolTip.ToolTipTextProperty);
		}

		public static Widget GetToolTipWidget(DependencyObject obj)
		{
			return obj.GetValue (ToolTip.ToolTipWidgetProperty) as Widget;
		}

		public static bool HasToolTipWidget(DependencyObject obj)
		{
			return obj.ContainsValue (ToolTip.ToolTipWidgetProperty);
		}

		public static Caption GetToolTipCaption(DependencyObject obj)
		{
			return obj.GetValue (ToolTip.ToolTipCaptionProperty) as Caption;
		}

		public static bool HasToolTipCaption(DependencyObject obj)
		{
			return obj.ContainsValue (ToolTip.ToolTipCaptionProperty);
		}

		public static void SetToolTipText(DependencyObject obj, string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				obj.ClearValue (ToolTip.ToolTipCaptionProperty);
				obj.ClearValue (ToolTip.ToolTipTextProperty);
				obj.ClearValue (ToolTip.ToolTipWidgetProperty);
			}
			else
			{
				obj.ClearValue (ToolTip.ToolTipCaptionProperty);
				obj.ClearValue (ToolTip.ToolTipWidgetProperty);
				obj.SetValue (ToolTip.ToolTipTextProperty, value);
			}
		}

		public static void SetToolTipColor(DependencyObject obj, Color value)
		{
			obj.SetValue (ToolTip.ToolTipColorProperty, value);
		}

		public static void ClearToolTipColor(DependencyObject obj)
		{
			obj.ClearValue (ToolTip.ToolTipColorProperty);
		}

		public static void SetToolTipWidget(DependencyObject obj, Widget value)
		{
			if (value == null)
			{
				obj.ClearValue (ToolTip.ToolTipCaptionProperty);
				obj.ClearValue (ToolTip.ToolTipTextProperty);
				obj.ClearValue (ToolTip.ToolTipWidgetProperty);
			}
			else
			{
				obj.ClearValue (ToolTip.ToolTipCaptionProperty);
				obj.ClearValue (ToolTip.ToolTipTextProperty);
				obj.SetValue (ToolTip.ToolTipWidgetProperty, value);
			}
		}

		public static void SetToolTipCaption(DependencyObject obj, Caption value)
		{
			if (value == null)
			{
				obj.ClearValue (ToolTip.ToolTipCaptionProperty);
				obj.ClearValue (ToolTip.ToolTipTextProperty);
				obj.ClearValue (ToolTip.ToolTipWidgetProperty);
			}
			else
			{
				obj.SetValue (ToolTip.ToolTipCaptionProperty, value);
				obj.ClearValue (ToolTip.ToolTipTextProperty);
				obj.ClearValue (ToolTip.ToolTipWidgetProperty);
			}
		}


		private static double GetTooltipAutoCloseDelay(DependencyObject obj)
		{
			//	Retourne le délai de fermeture pour le tooltip, en se basant sur une vitesse de lecture
			//	de 40 caractères/seconde. Le délai est toujours compris entre 5 et 20 secondes.
			//	Si le tooltip n'est pas défini avec un texte (par exemple un Caption ou un Widget), le délai
			//	est de 5 secondes.
			int length = ToolTip.GetTooltipTextLength (obj);
			return System.Math.Min (System.Math.Max (length/40, SystemInformation.ToolTipAutoCloseDelay), SystemInformation.ToolTipAutoCloseDelay*4);
		}

		private static int GetTooltipTextLength(DependencyObject obj)
		{
			//	Retourne le nombre de caractères du texte, ou zéro si ce n'est pas un texte.
			string text = ToolTip.GetToolTipText (obj);

			if (string.IsNullOrEmpty (text))
			{
				return 0;
			}
			else
			{
				return text.Length;
			}
		}


		public static readonly DependencyProperty ToolTipTextProperty    = DependencyProperty<ToolTip>.RegisterAttached<string> ("ToolTipText", PrivateDependencyPropertyMetadata.Default);
		public static readonly DependencyProperty ToolTipWidgetProperty  = DependencyProperty<ToolTip>.RegisterAttached<Widget> ("ToolTipWidget", PrivateDependencyPropertyMetadata.Default);
		public static readonly DependencyProperty ToolTipCaptionProperty = DependencyProperty<ToolTip>.RegisterAttached<Caption> ("ToolTipCaption", PrivateDependencyPropertyMetadata.Default);
		public static readonly DependencyProperty ToolTipColorProperty   = DependencyProperty<ToolTip>.RegisterAttached<Color> ("ToolTipColor", PrivateDependencyPropertyMetadata.Default);

		private readonly Dictionary<long, object> hash;
		private ToolTipBehaviour				behaviour;
		
		private Window							window;
		private bool							isDisplayed;
		private Timer							timer;
		private System.DateTime					lastChangeTime;
		
		private Widget							widget;
		private object							caption;
		private object							hostProvidedCaption;
		private object							refreshedCaption;
		
		private Drawing.Point					birthPos;
		private Drawing.Point					initialPos;
		
		private static readonly double			hideDistance = 24;
		private static readonly Drawing.Point	margin = new Drawing.Point(3, 2);
		private static readonly Drawing.Point	offset = new Drawing.Point(8, -16);
	}
}

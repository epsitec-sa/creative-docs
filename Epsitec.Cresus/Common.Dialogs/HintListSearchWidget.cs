//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Common.Dialogs
{
	/// <summary>
	/// The <c>HintListSearchWidget</c> class implements a search widget where the
	/// user can type text to narrow a search in the hint list.
	/// </summary>
	public class HintListSearchWidget : FrameBox
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="HintListSearchWidget"/> class.
		/// </summary>
		public HintListSearchWidget()
		{
			this.textField = new FlatTextField ()
			{
				Embedder = this,
				Dock = DockStyle.Fill,
				Margins = new Margins (5, 5, 2, 2),
				VerticalAlignment = VerticalAlignment.Center
			};

			this.cachedValue = "";
			this.textField.TextChanged += this.HandleTextFieldTextChanged;
		}


		/// <summary>
		/// Gets or sets the search value, which is a simple string, without any
		/// formatting.
		/// </summary>
		/// <value>The value.</value>
		public string Value
		{
			get
			{
				return this.cachedValue;
			}
			set
			{
				FormattedText text = FormattedText.FromSimpleText (value);

				if (this.textField.FormattedText != text)
				{
					this.textField.FormattedText = text;
					this.textField.SelectAll ();
				}
			}
		}

		/// <summary>
		/// Gets or sets the associated hint list widget.
		/// </summary>
		/// <value>The hint list widget.</value>
		public HintListWidget HintListWidget
		{
			get;
			internal set;
		}


		/// <summary>
		/// Attaches a source widget, which has delegated us to interact with
		/// the user instead of itself. This clears the text field.
		/// </summary>
		/// <param name="widget">The source widget.</param>
		public void AttachSourceWidget(Widget widget)
		{
			if (this.sourceWidget != widget)
			{
				this.sourceWidget = widget;

				if (this.sourceWidget == null)
				{
					this.InternalState &= ~WidgetInternalState.Focusable;
				}
				else
				{
					this.InternalState |= WidgetInternalState.Focusable;
				}

				this.Value = "";
			}
		}

		/// <summary>
		/// Detaches the source widget.
		/// </summary>
		/// <returns><c>true</c> if a source widget was detached; otherwise, <c>false</c>.</returns>
		public bool DetachSourceWidget()
		{
			if (this.sourceWidget == null)
			{
				return false;
			}
			else
			{
				this.AttachSourceWidget (null);
				return true;
			}
		}

		/// <summary>
		/// Sets the focus on the text field used by the search widget.
		/// </summary>
		/// <returns><c>true</c> if the focus was successfully set; otherwise, <c>false</c>.</returns>
		public bool StealFocus()
		{
			return this.textField.Focus ();
		}


		/// <summary>
		/// Processes the exit of the first or last child when navigating with the
		/// TAB key. This class overrides TAB navigation in order to tunnel the
		/// focus back into the source widget TAB-chain.
		/// </summary>
		/// <param name="dir">The tab navigation direction.</param>
		/// <param name="mode">The tab navigation mode.</param>
		/// <param name="focus">The widget with should get the focus.</param>
		/// <returns>
		/// 	<c>true</c> if the method has a focus suggestion; otherwise, <c>false</c>.
		/// </returns>
		protected override bool ProcessTabChildrenExit(TabNavigationDir dir, TabNavigationMode mode, out Widget focus)
		{
			if (this.sourceWidget == null)
			{
				return base.ProcessTabChildrenExit (dir, mode, out focus);
			}
			else
			{
				focus = this.sourceWidget.FindTabWidget (dir, mode);

				HashSet<Widget> stop = new HashSet<Widget> ();

				while (Widgets.Helpers.VisualTree.IsAncestor (focus, this.sourceWidget))
				{
					stop.Add (focus);
					
					focus = focus.FindTabWidget (dir, mode);

					if ((focus == null) ||
						(stop.Contains (focus)))
					{
						System.Diagnostics.Debug.Fail ("Cannot TAB-navigate to child");
						break;
					}
				}

				return true;
			}
		}

		protected override bool PostProcessMessage(Message message, Point pos)
		{
			if (this.HintListWidget != null)
			{
				switch (message.MessageType)
				{
					case MessageType.KeyDown:
						
						//	Handle keyboard events such as up and down, in order to scroll
						//	the contents of the hint list.
						if (this.HintListWidget.Navigate (message))
						{
							message.Handled = true;
						}
						break;
				}
			}
			
			return base.PostProcessMessage (message, pos);
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			Rectangle frame = this.Client.Bounds;
			Color     color = Color.FromBrightness (this.IsEnabled ? 0.4 : 0.8);
			
			frame.Deflate (0.5);
			
			using (Path path = new Path ())
			{
				//	Paint the rounded frame for the search box

				path.AppendRoundedRectangle (frame, 6);
				graphics.Rasterizer.AddSurface (path);
				graphics.RenderSolid (Color.FromBrightness (1));

				graphics.Rasterizer.AddOutline (path, 1, CapStyle.Round, JoinStyle.Round);
				graphics.RenderSolid (color);
			}

			using (Path path = new Path ())
			{
				//	Paint a stylized magnifying glass

				double cx = frame.Right - 10;
				double cy = frame.Top - 10;
				double r  = 5;

				path.AppendCircle (cx, cy, r);
				path.MoveTo (cx - 0.707*r, cy - 0.707*r);
				path.LineTo (cx - 1.707*r, cy - 1.707*r);

				graphics.Rasterizer.AddOutline (path, 2.5, CapStyle.Round, JoinStyle.Round);
				graphics.RenderSolid (color);
			}
		}


		private void HandleTextFieldTextChanged(object sender)
		{
			string oldValue = this.cachedValue;
			string newValue = this.textField.FormattedText.ToSimpleText () ?? "";

			if (oldValue != newValue)
			{
				this.cachedValue = newValue;
				this.InvalidateProperty (HintListSearchWidget.ValueProperty, oldValue, newValue);
			}
		}


		/// <summary>
		/// Occurs when the search box value changed.
		/// </summary>
		public event EventHandler<DependencyPropertyChangedEventArgs> ValueChanged
		{
			add
			{
				this.AddEventHandler (HintListSearchWidget.ValueProperty, value);
			}
			remove
			{
				this.RemoveEventHandler (HintListSearchWidget.ValueProperty, value);
			}
		}
		
		
		private static object GetValue(DependencyObject obj)
		{
			HintListSearchWidget that = (HintListSearchWidget) obj;

			return that.Value;
		}

		private static void SetValue(DependencyObject obj, object value)
		{
			HintListSearchWidget that = (HintListSearchWidget) obj;

			that.Value = (string) value;
		}


		public static readonly DependencyProperty ValueProperty = DependencyProperty.Register ("Value", typeof (string), typeof (HintListSearchWidget), new DependencyPropertyMetadata (HintListSearchWidget.GetValue, HintListSearchWidget.SetValue));
		
		private readonly FlatTextField textField;
		private Widget sourceWidget;
		private string cachedValue;
	}
}

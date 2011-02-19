//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;
using Epsitec.Common.UI;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Collections;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Support;

namespace Epsitec.Cresus.Graph.Widgets
{
	/// <summary>
	/// The <c>CommandSelectionBar</c> class represents a special tool bar where
	/// a single command button is active, which gets underlined by a tiny arrow
	/// overlapping the view below the command.
	/// </summary>
	public class CommandSelectionBar : FrameBox, IListHost<Command>
	{
		public CommandSelectionBar()
		{
			this.commands = new CommandCollection (this);
			this.metabuttons = new List<MetaButton> ();

			this.Padding = new Margins (1, 1, 0, 1);
		}


		public CommandCollection Items
		{
			get
			{
				return this.commands;
			}
		}

		public Size ItemSize
		{
			get
			{
				return this.GetValue<Size> (CommandSelectionBar.ItemSizeProperty);
			}
			set
			{
				if (value.IsEmpty)
				{
					this.ClearValue (CommandSelectionBar.ItemSizeProperty);
				}
				else
				{
					this.SetValue (CommandSelectionBar.ItemSizeProperty, value);
				}
			}
		}

		public double MarkLength
		{
			get
			{
				return this.GetValue<double> (CommandSelectionBar.MarkLengthProperty);
			}
			set
			{
				this.SetValue (CommandSelectionBar.MarkLengthProperty, value);
			}
		}

		public Command SelectedItem
		{
			get
			{
				return this.selectedItem;
			}
			set
			{
				if (this.selectedItem != value)
				{
					var oldValue = this.selectedItem;
					var newValue = value;

					this.selectedItem = value;

					this.OnSelectedItemChanged (oldValue, newValue);
				}
			}
		}

		
		public override Margins GetPaintMargins()
		{
			return new Margins (0, 0, 0, this.MarkLength);
		}

		public override Margins GetShapeMargins()
		{
			return new Margins (0, 0, 0, this.MarkLength);
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			base.PaintBackgroundImplementation (graphics, clipRect);

			var rect   = this.Client.Bounds;
			var button = this.FindButton (this.SelectedItem);

			double oy = rect.Bottom;
			double x1 = rect.Left;
			double x4 = rect.Right;

			double length = this.MarkLength;

			if ((this.BackColor.IsVisible) &&
				(button != null))
			{
				using (Path path = new Path ())
				{
					path.MoveTo (x1, oy);

					var rect2 = button.ActualBounds;

					double x2 = rect2.Left;
					double x3 = rect2.Right;
					double xm = (x2 + x3) / 2;

					path.LineTo (xm - 1.5*length, oy);
					path.LineTo (xm, oy - length);
					path.LineTo (xm + 1.5*length, oy);
					
					path.LineTo (x4, oy);
					path.LineTo (x4, rect.Top);
					path.LineTo (x1, rect.Top);
					path.Close ();

					graphics.Color = this.BackColor;
					graphics.PaintSurface (path);
				}
			}

			oy += 0.5;
			x1 += 0.5;
			x4 -= 0.5;

			using (Path path = new Path ())
			{
				path.MoveTo (x1, oy);

				if (button != null)
				{
					var rect2 = button.ActualBounds;

					double x2 = rect2.Left;
					double x3 = rect2.Right;
					double xm = (x2 + x3) / 2;

					path.LineTo (xm - 1.5*length, oy);
					path.LineTo (xm, oy - length);
					path.LineTo (xm + 1.5*length, oy);
				}
				
				path.LineTo (x4, oy);

				graphics.LineCap = CapStyle.Square;
				graphics.LineJoin = JoinStyle.Miter;
				graphics.LineWidth = 1;
				graphics.Color = Epsitec.Common.Widgets.Adorners.Factory.Active.ColorBorder;
				graphics.PaintOutline (path);
			}
		}

		
		#region IListHost<Command> Members

		Epsitec.Common.Types.Collections.HostedList<Command> IListHost<Command>.Items
		{
			get
			{
				return this.Items;
			}
		}

		void IListHost<Command>.NotifyListInsertion(Command item)
		{
			this.Add (
				new MetaButton ()
				{
					CommandObject = item,
					Margins = new Margins (0, 0, 1, 1),
				});
		}

		void IListHost<Command>.NotifyListRemoval(Command item)
		{
			this.Remove (this.FindButton (item));
		}

		#endregion

		
		private void Add(MetaButton button)
		{
			this.metabuttons.Add (button);
			
			Size size = this.ItemSize;

			if (size.IsEmpty)
			{
				size = new Size (40, 40);
			}

			button.Dock = DockStyle.Stacked;
			button.ButtonClass = ButtonClass.FlatButton;
			button.Parent = this;
			button.PreferredSize = size;
		}

		private void Remove(MetaButton button)
		{
			if ((button == null) ||
				(this.metabuttons.Remove (button) == false))
			{
				return;
			}

			button.Parent = null;
			button.Dispose ();
		}

		private MetaButton FindButton(Command command)
		{
			if (command == null)
			{
				return null;
			}
			else
			{
				return this.metabuttons.FirstOrDefault (x => x.CommandObject == command);
			}
		}

		private void OnItemSizeChanged(Size oldSize, Size newSize)
		{
			if (newSize.Height > 0)
			{
				this.PreferredHeight = newSize.Height + this.Padding.Height;
				this.metabuttons.ForEach (x => x.PreferredSize = newSize);
			}
		}

		private void OnSelectedItemChanged(Command oldValue, Command newValue)
		{
			this.Invalidate ();

			var handler = this.GetUserEventHandler<DependencyPropertyChangedEventArgs> (CommandSelectionBar.SelectedItemChangedEvent);

			if (handler != null)
			{
				handler (this, new DependencyPropertyChangedEventArgs ("SelectedItem", oldValue, newValue));
			}
		}


		private static void NotifyItemSizeChanged(DependencyObject o, object oldValue, object newValue)
		{
			CommandSelectionBar that = o as CommandSelectionBar;
			Size oldSize = (Size) oldValue;
			Size newSize = (Size) newValue;
			that.OnItemSizeChanged (oldSize, newSize);
		}

		public event EventHandler<DependencyPropertyChangedEventArgs> SelectedItemChanged
		{
			add
			{
				this.AddUserEventHandler (CommandSelectionBar.SelectedItemChangedEvent, value);
			}
			remove
			{
				this.RemoveUserEventHandler (CommandSelectionBar.SelectedItemChangedEvent, value);
			}
		}
		
		public static readonly DependencyProperty ItemSizeProperty = DependencyProperty.Register ("ItemSize", typeof (Size), typeof (CommandSelectionBar), new Epsitec.Common.Widgets.Helpers.VisualPropertyMetadata (Size.Empty, CommandSelectionBar.NotifyItemSizeChanged, Epsitec.Common.Widgets.Helpers.VisualPropertyMetadataOptions.AffectsDisplay | Epsitec.Common.Widgets.Helpers.VisualPropertyMetadataOptions.AffectsMeasure));
		public static readonly DependencyProperty MarkLengthProperty = DependencyProperty.Register ("MarkLength", typeof (double), typeof (CommandSelectionBar), new Epsitec.Common.Widgets.Helpers.VisualPropertyMetadata (8.0, Epsitec.Common.Widgets.Helpers.VisualPropertyMetadataOptions.AffectsDisplay));

		private const string SelectedItemChangedEvent = "SelectedItemChanged";

		private readonly CommandCollection commands;
		private readonly List<MetaButton> metabuttons;
		private Command selectedItem;
	}
}

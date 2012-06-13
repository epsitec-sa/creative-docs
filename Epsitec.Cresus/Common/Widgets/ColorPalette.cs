//	Copyright © 2003-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>ColorPalette</c> class manages a color palette presented in a grid.
	/// The default size of the grid is 4 columns of 8 rows.
	/// </summary>
	public class ColorPalette : Widget
	{
		public ColorPalette()
		{
			this.CreateColorSamples (4, 8);
			this.ColorCollection = new Drawing.ColorCollection (Drawing.ColorCollectionType.Default);

			this.optionButton = new GlyphButton (this);
			this.optionButton.GlyphShape  = GlyphShape.ArrowLeft;
			this.optionButton.ButtonStyle = ButtonStyle.Normal;
			this.optionButton.Clicked += this.HandleOptionButtonClicked;

			this.includeOpenSaveCommands = true;
			this.menuCommands = new List<Command> ();

			this.menuCommands.Add (Res.Commands.ColorPalette.SelectDefaultColors);
			this.menuCommands.Add (Res.Commands.ColorPalette.SelectRainbowColors);
			this.menuCommands.Add (Res.Commands.ColorPalette.SelectLightColors);
			this.menuCommands.Add (Res.Commands.ColorPalette.SelectDarkColors);
			this.menuCommands.Add (Res.Commands.ColorPalette.SelectGrayColors);

			ToolTip.Default.SetToolTip (this.optionButton, Res.Strings.ColorPalette.Options);
		}

		public ColorPalette(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		public int								ColumnCount
		{
			get
			{
				return this.columnCount;
			}
			set
			{
				if (this.columnCount != value)
				{
					this.CreateColorSamples (value, this.RowCount);
				}
			}
		}

		public int								RowCount
		{
			get
			{
				return this.rowCount;
			}
			set
			{
				if (this.rowCount != value)
				{
					this.CreateColorSamples (this.ColumnCount, value);
				}
			}
		}

		public int								ColorCount
		{
			get
			{
				return this.columnCount * this.rowCount;
			}
		}

		public bool								OptionButtonVisibility
		{
			get
			{
				return this.optionButton.Visibility;
			}
			set
			{
				if (this.OptionButtonVisibility != value)
				{
					this.optionButton.Visibility = value;
					Layouts.LayoutContext.AddToMeasureQueue (this);
//-					this.UpdateGeometry (this.Client.Bounds);
				}
			}
		}

		public bool								HiliteSelectedColor
		{
			get
			{
				return (bool) this.GetValue (ColorPalette.HiliteSelectedColorProperty);
			}
			set
			{
				this.SetValue (ColorPalette.HiliteSelectedColorProperty, value);
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether to include open and save
		/// commands in the palette menu.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if open and save commands should be included in
		/// the palette menu; otherwise, <c>false</c>.
		/// </value>
		public bool								IncludeOpenSaveCommandsInMenu
		{
			get
			{
				return this.includeOpenSaveCommands;
			}
			set
			{
				this.includeOpenSaveCommands = value;
			}
		}

		public int								SelectedColorIndex
		{
			get
			{
				return (int) this.GetValue (ColorPalette.SelectedColorIndexProperty);
			}
			set
			{
				if (value == -1)
				{
					this.ClearValue (ColorPalette.SelectedColorIndexProperty);
				}
				else
				{
					this.SetValue (ColorPalette.SelectedColorIndexProperty, value);
				}
			}
		}

		public Drawing.RichColor				SelectedColor
		{
			get
			{
				ColorSample sample = this.SelectedColorSample;

				if (sample == null)
				{
					return Drawing.RichColor.Empty;
				}
				else
				{
					return sample.Color;
				}
			}
		}

		public ColorSample						SelectedColorSample
		{
			get
			{
				return this.GetColorSample (this.SelectedColorIndex);
			}
		}

		public Drawing.ColorCollection			ColorCollection
		{
			//	Donne la liste de couleurs à lier avec la palette.
			get
			{
				return this.colorCollection;
			}

			set
			{
				if (this.colorCollection != value)
				{
					if (this.colorCollection != null)
					{
						this.colorCollection.Changed -= this.HandleColorCollectionChanged;
					}

					this.colorCollection = value;

					if (this.colorCollection != null)
					{
						this.colorCollection.Changed += this.HandleColorCollectionChanged;
						this.HandleColorCollectionChanged (this);
					}
				}
			}
		}

		public bool								DragSourceEnable
		{
			get
			{
				return (bool) this.GetValue (ColorPalette.DragSourceEnableProperty);
			}
			set
			{
				if (value)
				{
					this.ClearValue (ColorPalette.DragSourceEnableProperty);
				}
				else
				{
					this.SetValue (ColorPalette.DragSourceEnableProperty, value);
				}
			}
		}

		public IList<Command>					BaseMenuCommands
		{
			get
			{
				return this.menuCommands;
			}
		}

		public int FindColorIndex(Drawing.RichColor color)
		{
			foreach (ColorSample sample in this.palette)
			{
				if (sample.Color == color)
				{
					return sample.Index;
				}
			}

			return -1;
		}

		public int FindColorIndex(Drawing.Color color)
		{
			foreach (ColorSample sample in this.palette)
			{
				if (sample.Color.Basic == color)
				{
					return sample.Index;
				}
			}

			return -1;
		}

		public ColorSample GetColorSample(int index)
		{
			if ((index >= 0) &&
				(index < this.palette.Length))
			{
				return this.palette[index];
			}
			else
			{
				return null;
			}
		}

		public bool WriteSelectedColor(Drawing.RichColor color)
		{
			ColorSample sample = this.SelectedColorSample;

			if (sample == null)
			{
				return false;
			}
			else
			{
				sample.Color = color;
				return true;
			}
		}

		public bool Navigate(ColorSample sample, KeyCode key)
		{
			ColorSample dest = null;

			int sampleColumn = sample.Index / this.RowCount;
			int sampleRow    = sample.Index % this.RowCount;

			switch (key)
			{
				case KeyCode.ArrowUp:
					dest = this.Find (sampleColumn, sampleRow-1);
					break;

				case KeyCode.ArrowDown:
					dest = this.Find (sampleColumn, sampleRow+1);
					break;

				case KeyCode.ArrowLeft:
					dest = this.Find (sampleColumn-1, sampleRow) ?? this.Find (sample.Index-1);
					break;

				case KeyCode.ArrowRight:
					dest = this.Find (sampleColumn+1, sampleRow) ?? this.Find (sample.Index+1);
					break;

				default:
					return false;
			}

			if (dest == null)
			{
				return false;
			}

			this.SelectSample (dest, Operation.Export);
			dest.Focus ();

			return true;
		}


		protected override void SetBoundsOverride(Drawing.Rectangle oldRect, Drawing.Rectangle newRect)
		{
			base.SetBoundsOverride (oldRect, newRect);
			this.UpdateGeometry (new Drawing.Rectangle (Drawing.Point.Zero, newRect.Size));
		}

		protected override void Dispose(bool disposing)
		{
			if (disposing)
			{
				this.DisposeColorSamples ();

				if (this.optionButton != null)
				{
					this.optionButton.Clicked += this.HandleOptionButtonClicked;
				}
			}

			base.Dispose (disposing);
		}

		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			if (!this.BackColor.IsEmpty)
			{
				graphics.AddFilledRectangle (this.Client.Bounds);
				graphics.RenderSolid (this.BackColor);
			}
		}


		private void CreateColorSamples(int columns, int rows)
		{
			this.DisposeColorSamples ();

			this.columnCount = columns;
			this.rowCount    = rows;

			int n = this.ColorCount;

			this.palette = new ColorSample[n];

			for (int i = 0; i < n; i++)
			{
				this.palette[i] = new ColorSample (this);
				this.palette[i].Clicked += this.HandleColorClicked;
				this.palette[i].TabIndex = i;
				this.palette[i].TabNavigationMode = TabNavigationMode.ActivateOnTab;
				this.palette[i].AddEventHandler (ColorSample.ColorProperty, this.HandleColorSampleColorChanged);
				this.palette[i].Index = i;
				this.palette[i].DragSourceEnable = this.DragSourceEnable;
			}

			this.SelectedColorIndex = -1;
		}

		private void DisposeColorSamples()
		{
			if (this.palette != null)
			{
				foreach (ColorSample sample in this.palette)
				{
					sample.Clicked -= this.HandleColorClicked;
					sample.RemoveEventHandler (ColorSample.ColorProperty, this.HandleColorSampleColorChanged);
					sample.Dispose ();
				}

				this.palette = null;
			}
		}

		private ColorSample Find(int column, int row)
		{
			return this.Find (row + column * this.RowCount);
		}

		private ColorSample Find(int index)
		{
			if ((index < 0) ||
				(index >= this.palette.Length))
			{
				return null;
			}
			else
			{
				return this.palette[index];
			}
		}

		#region Operation Enumeration

		private enum Operation
		{
			Export,
			Import
		}

		#endregion

		private void SelectSample(ColorSample sample, Operation operation)
		{
			int index = sample.Index;

			if ((index < 0) ||
				(index >= this.palette.Length))
			{
				return;
			}

			this.SelectedColorIndex = index;

			switch (operation)
			{
				case Operation.Import:
					this.OnImportSelectedColor ();
					break;

				case Operation.Export:
					this.OnExportSelectedColor ();
					break;
			}
		}

		public Drawing.Size GetBestFitSize(int sampleLength)
		{
			double dx = 1 + (sampleLength-1) * this.ColumnCount;
			double dy = 1 + (sampleLength-1) * this.RowCount;

			if (this.OptionButtonVisibility)
			{
				dx += 17;
			}

			return new Drawing.Size (dx, dy);
		}

		public override Drawing.Size GetBestFitSize()
		{
			return this.GetBestFitSize (19);
		}

		private void UpdateGeometry(Drawing.Rectangle rect)
		{
			if ((this.palette == null) ||
				(this.optionButton == null))
			{
				return;
			}

			double dx = System.Math.Floor (rect.Width/this.columnCount);
			double dy = System.Math.Floor (rect.Height/this.rowCount);
			
			dx = dy = System.Math.Min (dx, dy);

			double contentsWidth  = dx * this.columnCount + 1;
			double availableWidth = rect.Width;
			double internalMargin = 0;

			if (this.OptionButtonVisibility)
			{
				internalMargin = 17;
			}

			double offset = 0;
			
			switch (this.ContentAlignment)
			{
				case Drawing.ContentAlignment.BottomLeft:
				case Drawing.ContentAlignment.MiddleLeft:
				case Drawing.ContentAlignment.TopLeft:
					offset = internalMargin;
					break;

				case Drawing.ContentAlignment.BottomCenter:
				case Drawing.ContentAlignment.MiddleCenter:
				case Drawing.ContentAlignment.TopCenter:
					offset = System.Math.Floor ((availableWidth - internalMargin - contentsWidth) / 2);
					break;
					
				case Drawing.ContentAlignment.BottomRight:
				case Drawing.ContentAlignment.MiddleRight:
				case Drawing.ContentAlignment.TopRight:
					offset = availableWidth - contentsWidth;
					break;
			}
			
			Drawing.Point pos = new Drawing.Point (offset, 0);
			int i = 0;
			for (int x = 0; x < this.columnCount; x++)
			{
				pos.Y = rect.Top-(dy+1);
				for (int y = 0; y < this.rowCount; y++)
				{
					Drawing.Rectangle r = new Drawing.Rectangle (pos.X, pos.Y, dx+1, dy+1);
					this.palette[i++].SetManualBounds (r);
					pos.Y -= dy;
				}
				pos.X += dx;
			}

			if (this.OptionButtonVisibility)
			{
				Drawing.Rectangle r = new Drawing.Rectangle (offset - internalMargin, rect.Top-14, 14, 14);
				this.optionButton.SetManualBounds (r);
				this.optionButton.Visibility = true;
			}
			else
			{
				this.optionButton.Visibility = false;
			}
		}

		private void OnExportSelectedColor()
		{
			//	Génère un événement pour dire qu'on exporte une couleur.
			var handler = this.GetUserEventHandler (ColorPalette.ExportEvent);

			if (handler != null)
			{
				handler (this);
			}
		}

		private void OnImportSelectedColor()
		{
			//	Génère un événement pour dire qu'on importe une couleur.
			var handler = this.GetUserEventHandler (ColorPalette.ImportEvent);

			if (handler != null)
			{
				handler (this);
			}
		}



		private void HandleOptionButtonClicked(object sender, MessageEventArgs e)
		{
			if (e.Message.Button == MouseButtons.Left)
			{
				GlyphButton button = sender as GlyphButton;

				VMenu menu = new VMenu ();
				menu.Host = this;

				foreach (Command command in this.menuCommands)
				{
					if (command == null)
					{
						menu.Items.Add (new MenuSeparator ());
					}
					else
					{
						menu.Items.Add (new MenuItem (command));
					}
				}

				this.IncludeAdditionalMenuItems (menu);

				if (this.includeOpenSaveCommands)
				{
					menu.Items.Add (new MenuSeparator ());
					menu.Items.Add (new MenuItem (Res.Commands.ColorPalette.Load));
					menu.Items.Add (new MenuItem (Res.Commands.ColorPalette.Save));
				}

				menu.AdjustSize ();

				Drawing.Point pos = button.MapClientToScreen (new Drawing.Point (0, button.ActualHeight));
				pos.X -= menu.PreferredWidth;
				menu.ShowAsContextMenu (this.Window, pos);
			}
		}

		/// <summary>
		/// When overridden, this method can be used to add more items into the
		/// predefined palette menu.
		/// </summary>
		/// <param name="menu">The menu.</param>
		protected virtual void IncludeAdditionalMenuItems(VMenu menu)
		{
		}

		private void HandleColorClicked(object sender, MessageEventArgs e)
		{
			ColorSample cs = sender as ColorSample;

			if (e == null)
			{
				this.SelectSample (cs, Operation.Export);
				return;
			}

			if ((e.Message.IsShiftPressed) ||
				(e.Message.IsControlPressed))
			{
				this.SelectSample (cs, Operation.Import);
			}
			else
			{
				this.SelectSample (cs, Operation.Export);
			}
		}

		private void HandleColorCollectionChanged(object sender)
		{
			if ((this.colorCollection != null) &&
				(this.palette != null))
			{
				for (int i = 0; i < this.colorCollection.Count; i++)
				{
					if (i < this.palette.Length)
					{
						this.palette[i].Color = this.colorCollection[i];
					}
				}
			}
		}

		private void HandleColorSampleColorChanged(object sender, DependencyPropertyChangedEventArgs e)
		{
			ColorSample sample = sender as ColorSample;
			int index = sample.Index;

			Drawing.RichColor oldColor = e.OldValue == null ? Drawing.RichColor.Empty : (Drawing.RichColor) e.OldValue;
			Drawing.RichColor newColor = e.NewValue == null ? Drawing.RichColor.Empty : (Drawing.RichColor) e.NewValue;

			if (this.colorCollection != null)
			{
				this.colorCollection[index] = newColor;
			}
		}


		public event EventHandler				ExportSelectedColor
		{
			add
			{
				this.AddUserEventHandler (ColorPalette.ExportEvent, value);
			}
			remove
			{
				this.RemoveUserEventHandler (ColorPalette.ExportEvent, value);
			}
		}

		public event EventHandler				ImportSelectedColor
		{
			add
			{
				this.AddUserEventHandler (ColorPalette.ImportEvent, value);
			}
			remove
			{
				this.RemoveUserEventHandler (ColorPalette.ImportEvent, value);
			}
		}


		static ColorPalette()
		{
			Visual.PreferredWidthProperty.OverrideMetadataDefaultValue<ColorPalette> (80.0-1);
			Visual.PreferredHeightProperty.OverrideMetadataDefaultValue<ColorPalette> (160.0-1);
		}

		internal static void NotifySelectedColorIndexChanged(DependencyObject obj, object oldValue, object newValue)
		{
			ColorPalette that = (ColorPalette) obj;

			if (that.HiliteSelectedColor)
			{
				int oldIndex = (int) (oldValue ?? -1);
				int newIndex = (int) (newValue ?? -1);

				ColorSample oldSample = that.GetColorSample (oldIndex);
				ColorSample newSample = that.GetColorSample (newIndex);

				if (oldSample != null)
				{
					oldSample.SetSelected (false);
				}
				if (newSample != null)
				{
					newSample.SetSelected (true);
				}
			}
		}

		internal static void NotifyDragSourceEnableChanged(DependencyObject obj, object oldValue, object newValue)
		{
			ColorPalette that = (ColorPalette) obj;
			bool enable = (bool) newValue;

			foreach (ColorSample sample in that.palette)
			{
				sample.DragSourceEnable = enable;
			}
		}

		public static readonly DependencyProperty SelectedColorIndexProperty = DependencyProperty.Register ("SelectedColorIndex", typeof (int), typeof (ColorPalette), new DependencyPropertyMetadata (-1, ColorPalette.NotifySelectedColorIndexChanged));
		public static readonly DependencyProperty HiliteSelectedColorProperty = DependencyProperty.Register ("HiliteSelectedColor", typeof (bool), typeof (ColorPalette), new DependencyPropertyMetadata (false));
		public static readonly DependencyProperty DragSourceEnableProperty = DependencyProperty.Register ("DragSourceEnable", typeof (bool), typeof (ColorPalette), new DependencyPropertyMetadata (true, ColorPalette.NotifyDragSourceEnableChanged));

		private const string					ExportEvent = "ExportSelectedColor";
		private const string					ImportEvent = "ImportSelectedColor";

		private int								columnCount;
		private int								rowCount;
		private ColorSample[]					palette;
		private GlyphButton						optionButton;
		private Drawing.ColorCollection			colorCollection;
		private bool							includeOpenSaveCommands;
		private List<Command>					menuCommands;
	}
}

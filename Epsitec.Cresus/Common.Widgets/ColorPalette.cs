//	Copyright © 2003-2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Daniel ROUX, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

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
					this.UpdateGeometry (this.Client.Bounds);
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
						this.colorCollection.Changed -= new EventHandler (this.HandleColorCollectionChanged);
					}

					this.colorCollection = value;

					if (this.colorCollection != null)
					{
						this.colorCollection.Changed += new EventHandler (this.HandleColorCollectionChanged);
						this.HandleColorCollectionChanged (this);
					}
				}
			}
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

		private void UpdateGeometry(Drawing.Rectangle rect)
		{
			if ((this.palette == null) ||
				(this.optionButton == null))
			{
				return;
			}

			double dx = (rect.Width+1.0)/this.columnCount;
			double dy = (rect.Height+1.0)/this.rowCount;
			dx = dy = System.Math.Min (dx, dy);

			Drawing.Point pos = new Drawing.Point (rect.Right-(dx-1.0)*this.columnCount-1.0, 0);
			int i = 0;
			for (int x = 0; x < this.columnCount; x++)
			{
				pos.Y = rect.Top-dy;
				for (int y = 0; y < this.rowCount; y++)
				{
					Drawing.Rectangle r = new Drawing.Rectangle (pos.X, pos.Y, dx, dy);
					this.palette[i++].SetManualBounds (r);
					pos.Y -= dy-1.0;
				}
				pos.X += dx-1.0;
			}

			if (this.OptionButtonVisibility)
			{
				Drawing.Rectangle r = new Drawing.Rectangle (rect.Left, rect.Top-14, 14, 14);
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
			EventHandler handler = (EventHandler) this.GetUserEventHandler (ColorPalette.ExportEvent);

			if (handler != null)
			{
				handler (this);
			}
		}

		private void OnImportSelectedColor()
		{
			//	Génère un événement pour dire qu'on importe une couleur.
			EventHandler handler = (EventHandler) this.GetUserEventHandler (ColorPalette.ImportEvent);

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
				menu.Items.Add (new MenuItem ("NewPaletteDefault", "", Res.Strings.ColorPalette.PaletteDefault, ""));
				menu.Items.Add (new MenuItem ("NewPaletteRainbow", "", Res.Strings.ColorPalette.PaletteRainbow, ""));
				menu.Items.Add (new MenuItem ("NewPaletteLight", "", Res.Strings.ColorPalette.PaletteLight, ""));
				menu.Items.Add (new MenuItem ("NewPaletteDark", "", Res.Strings.ColorPalette.PaletteDark, ""));
				menu.Items.Add (new MenuItem ("NewPaletteGray", "", Res.Strings.ColorPalette.PaletteGray, ""));
				menu.Items.Add (new MenuSeparator ());
				menu.Items.Add (new MenuItem ("OpenPalette", "", Res.Strings.ColorPalette.OpenPalette, ""));
				menu.Items.Add (new MenuItem ("SavePalette", "", Res.Strings.ColorPalette.SavePalette, ""));
				menu.AdjustSize ();

				Drawing.Point pos = button.MapClientToScreen (new Drawing.Point (0, button.ActualHeight));
				pos.X -= menu.PreferredWidth;
				menu.ShowAsContextMenu (this.Window, pos);
			}
		}

		private void HandleColorClicked(object sender, MessageEventArgs e)
		{
			ColorSample cs = sender as ColorSample;

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
			Types.DependencyPropertyMetadata metadataDx = Visual.PreferredWidthProperty.DefaultMetadata.Clone ();
			Types.DependencyPropertyMetadata metadataDy = Visual.PreferredHeightProperty.DefaultMetadata.Clone ();

			metadataDx.DefineDefaultValue (80.0-1);
			metadataDy.DefineDefaultValue (160.0-1);

			Visual.PreferredWidthProperty.OverrideMetadata (typeof (ColorPalette), metadataDx);
			Visual.PreferredHeightProperty.OverrideMetadata (typeof (ColorPalette), metadataDy);
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

		public static readonly DependencyProperty SelectedColorIndexProperty = DependencyProperty.Register ("SelectedColorIndex", typeof (int), typeof (ColorPalette), new DependencyPropertyMetadata (-1, ColorPalette.NotifySelectedColorIndexChanged));
		public static readonly DependencyProperty HiliteSelectedColorProperty = DependencyProperty.Register ("HiliteSelectedColor", typeof (bool), typeof (ColorPalette), new DependencyPropertyMetadata (false));

		private const string					ExportEvent = "ExportSelectedColor";
		private const string					ImportEvent = "ImportSelectedColor";

		private int								columnCount;
		private int								rowCount;
		private ColorSample[]					palette;
		private GlyphButton						optionButton;
		private Drawing.ColorCollection			colorCollection;
	}
}

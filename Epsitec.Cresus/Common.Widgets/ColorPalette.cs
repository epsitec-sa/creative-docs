using Epsitec.Common.Support;

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe ColorPalette propose une palette de couleurs sous forme d'un tableau.
	/// Pour l'instant, le tableau est fixe et contient 2x8 échantillons.
	/// </summary>
	public class ColorPalette : Widget
	{
		public ColorPalette()
		{
			this.nbColumns = 4;
			this.nbRows = 8;
			this.nbTotal = this.nbColumns*this.nbRows;

			this.palette = new ColorSample[this.nbTotal];
			for ( int i=0 ; i<this.nbTotal ; i++ )
			{
				this.palette[i] = new ColorSample(this);
				this.palette[i].Clicked += new MessageEventHandler(this.HandleColorClicked);
				this.palette[i].TabIndex = i;
				this.palette[i].TabNavigationMode = TabNavigationMode.ActivateOnTab;

				int x = i/this.nbRows;
				int y = i%this.nbRows;
				this.palette[i].Column = x;
				this.palette[i].Row    = y;
				this.palette[i].Rank   = x+y*this.nbColumns;
			}

			this.palette[ 0].Color = Drawing.RichColor.FromAlphaRgb(0.0, 1.0, 1.0, 1.0);
			this.palette[ 1].Color = Drawing.RichColor.FromAlphaRgb(1.0, 1.0, 0.0, 0.0);
			this.palette[ 2].Color = Drawing.RichColor.FromAlphaRgb(1.0, 1.0, 1.0, 0.0);
			this.palette[ 3].Color = Drawing.RichColor.FromAlphaRgb(1.0, 0.0, 1.0, 0.0);
			this.palette[ 4].Color = Drawing.RichColor.FromAlphaRgb(1.0, 0.0, 1.0, 1.0);
			this.palette[ 5].Color = Drawing.RichColor.FromAlphaRgb(1.0, 0.0, 0.0, 1.0);
			this.palette[ 6].Color = Drawing.RichColor.FromAlphaRgb(1.0, 1.0, 0.0, 1.0);
			this.palette[ 7].Color = Drawing.RichColor.FromAlphaRgb(0.5, 0.5, 0.5, 0.5);

			this.palette[ 8].Color = Drawing.RichColor.FromAlphaRgb(1.0, 1.0, 1.0, 1.0);
			this.palette[ 9].Color = Drawing.RichColor.FromAlphaRgb(1.0, 0.9, 0.9, 0.9);
			this.palette[10].Color = Drawing.RichColor.FromAlphaRgb(1.0, 0.8, 0.8, 0.8);
			this.palette[11].Color = Drawing.RichColor.FromAlphaRgb(1.0, 0.7, 0.7, 0.7);
			this.palette[12].Color = Drawing.RichColor.FromAlphaRgb(1.0, 0.6, 0.6, 0.6);
			this.palette[13].Color = Drawing.RichColor.FromAlphaRgb(1.0, 0.5, 0.5, 0.5);
			this.palette[14].Color = Drawing.RichColor.FromAlphaRgb(1.0, 0.4, 0.4, 0.4);
			this.palette[15].Color = Drawing.RichColor.FromAlphaRgb(1.0, 0.0, 0.0, 0.0);

			this.selected = -1;

			this.buttonOption = new GlyphButton(this);
			this.buttonOption.GlyphShape = GlyphShape.ArrowLeft;
			this.buttonOption.ButtonStyle = ButtonStyle.Normal;
			this.buttonOption.Clicked += new MessageEventHandler(this.HandleButtonOptionClicked);
			ToolTip.Default.SetToolTip(this.buttonOption, Res.Strings.ColorPalette.Options);
		}
		
		public ColorPalette(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				if ( this.palette != null )
				{
					foreach ( ColorSample cs in this.palette )
					{
						cs.Clicked -= new MessageEventHandler(this.HandleColorClicked);
					}
				}

				if ( this.buttonOption != null )
				{
					this.buttonOption.Clicked += new MessageEventHandler(this.HandleButtonOptionClicked);
				}
			}
			
			base.Dispose(disposing);
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
		
		public int								Columns
		{
			get
			{
				return this.nbColumns;
			}

			set
			{
				//	TODO
				throw new System.InvalidOperationException("Not implemented !");
			}
		}

		public int								Rows
		{
			get
			{
				return this.nbRows;
			}

			set
			{
				//	TODO
				throw new System.InvalidOperationException("Not implemented !");
			}
		}

		public bool								HasOptionButton
		{
			get
			{
				return this.hasOptionButton;
			}

			set
			{
				if ( this.hasOptionButton != value )
				{
					this.hasOptionButton = value;
					this.UpdateGeometry();
				}
			}
		}

		public int								ColorSelected
		{
			get
			{
				return this.selected;
			}

			set
			{
				this.selected = value;
			}
		}

		public Drawing.RichColor				Color
		{
			get
			{
				if ( this.selected >= 0 && this.selected < this.nbTotal )
				{
					return this.palette[this.selected].Color;
				}
				else
				{
					return Drawing.RichColor.Empty;
				}
			}

			set
			{
				if ( this.selected >= 0 && this.selected < this.nbTotal )
				{
					this.palette[this.selected].Color = value;
				}
			}
		}

		public ColorSample						SelectedColorSample
		{
			get
			{
				if ( this.selected == -1 )
				{
					return this.palette[0];
				}
				return this.palette[this.selected];
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
				if ( this.colorCollection != value )
				{
					if ( this.colorCollection != null )
					{
						this.colorCollection.Changed -= new EventHandler(this.HandleColorCollectionChanged);
					}

					this.colorCollection = value;

					if ( this.colorCollection != null )
					{
						this.colorCollection.Changed += new EventHandler(this.HandleColorCollectionChanged);
					}

					for ( int i=0 ; i<this.nbTotal ; i++ )
					{
						if ( this.colorCollection == null )
						{
							this.palette[i].DetachColorCollection();
						}
						else
						{
							this.palette[i].AttachColorCollection(this.colorCollection, i);
						}
					}
				}
			}
		}
		

		public bool Navigate(ColorSample sample, KeyCode key)
		{
			//	Détermine quel widget il faut activer, en fonction de la
			//	ligne et de la colonne où l'on se trouve.
			ColorSample dest;

			switch ( key )
			{
				case KeyCode.ArrowUp:
					dest = this.Search(sample.Column, sample.Row-1);
					if ( dest != null )
					{
						this.SelectSample(dest, false);
						dest.Focus();
						return true;
					}
					return false;

				case KeyCode.ArrowDown:
					dest = this.Search(sample.Column, sample.Row+1);
					if ( dest != null )
					{
						this.SelectSample(dest, false);
						dest.Focus();
						return true;
					}
					return false;

				case KeyCode.ArrowLeft:
					dest = this.Search(sample.Column-1, sample.Row);
					if ( dest != null )
					{
						this.SelectSample(dest, false);
						dest.Focus();
						return true;
					}
					dest = this.Search(sample.Rank-1);
					if ( dest != null )
					{
						this.SelectSample(dest, false);
						dest.Focus();
						return true;
					}
					return false;

				case KeyCode.ArrowRight:
					dest = this.Search(sample.Column+1, sample.Row);
					if ( dest != null )
					{
						this.SelectSample(dest, false);
						dest.Focus();
						return true;
					}
					dest = this.Search(sample.Rank+1);
					if ( dest != null )
					{
						this.SelectSample(dest, false);
						dest.Focus();
						return true;
					}
					return false;
				
				default:
					return false;
			}
		}

		protected ColorSample Search(int column, int row)
		{
			for ( int i=0 ; i<this.nbTotal ; i++ )
			{
				if ( this.palette[i].Column == column &&
					 this.palette[i].Row    == row    )
				{
					return this.palette[i];
				}
			}

			return null;
		}

		protected ColorSample Search(int rank)
		{
			for ( int i=0 ; i<this.nbTotal ; i++ )
			{
				if ( this.palette[i].Rank == rank )  return this.palette[i];
			}

			return null;
		}

		protected void SelectSample(ColorSample sample, bool import)
		{
			//	Sélectionne un échantillon.
			for ( int i=0 ; i<this.nbTotal ; i++ )
			{
				if ( this.palette[i] == sample )
				{
					this.ColorSelected = i;

					if ( import )
					{
						this.OnImport();
					}
					else
					{
						this.OnExport();
					}
				}
			}
		}


		protected override void SetBoundsOverride(Drawing.Rectangle oldRect, Drawing.Rectangle newRect)
		{
			base.SetBoundsOverride(oldRect, newRect);
			this.UpdateGeometry ();
		}
		
		protected void UpdateGeometry()
		{
			//	Met à jour la géométrie.

			if ( this.palette == null )  return;

			Drawing.Rectangle rect = this.Client.Bounds;

			double dx = (rect.Width+1.0)/this.nbColumns;
			double dy = (rect.Height+1.0)/this.nbRows;
			dx = dy = System.Math.Min(dx, dy);

			Drawing.Point pos = new Drawing.Point();
			pos.X = rect.Right-(dx-1.0)*this.nbColumns-1.0;
			int i = 0;
			for ( int x=0 ; x<this.nbColumns ; x++ )
			{
				pos.Y = rect.Top-dy;
				for ( int y=0 ; y<this.nbRows ; y++ )
				{
					Drawing.Rectangle r = new Drawing.Rectangle(pos.X, pos.Y, dx, dy);
					this.palette[i].SetManualBounds(r);
					i ++;
					pos.Y -= dy-1.0;
				}
				pos.X += dx-1.0;
			}

			if ( this.hasOptionButton )
			{
				Drawing.Rectangle r = new Drawing.Rectangle(rect.Left, rect.Top-14, 14, 14);
				this.buttonOption.SetManualBounds(r);
				this.buttonOption.Visibility = true;
			}
			else
			{
				this.buttonOption.Visibility = false;
			}
		}
		
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			if ( !this.BackColor.IsEmpty )
			{
				graphics.AddFilledRectangle(this.Client.Bounds);
				graphics.RenderSolid(this.BackColor);
			}
		}

		
		private void HandleButtonOptionClicked(object sender, MessageEventArgs e)
		{
			//	Bouton des options cliqué.
			GlyphButton button = sender as GlyphButton;

			VMenu menu = new VMenu();
			menu.Host = this;
			menu.Items.Add(new MenuItem("NewPaletteDefault", "", Res.Strings.ColorPalette.PaletteDefault, ""));
			menu.Items.Add(new MenuItem("NewPaletteRainbow", "", Res.Strings.ColorPalette.PaletteRainbow, ""));
			menu.Items.Add(new MenuItem("NewPaletteLight",   "", Res.Strings.ColorPalette.PaletteLight,   ""));
			menu.Items.Add(new MenuItem("NewPaletteDark",    "", Res.Strings.ColorPalette.PaletteDark,    ""));
			menu.Items.Add(new MenuItem("NewPaletteGray",    "", Res.Strings.ColorPalette.PaletteGray,    ""));
			menu.Items.Add(new MenuSeparator());
			menu.Items.Add(new MenuItem("OpenPalette", "", Res.Strings.ColorPalette.OpenPalette, ""));
			menu.Items.Add(new MenuItem("SavePalette", "", Res.Strings.ColorPalette.SavePalette, ""));
			menu.AdjustSize();

			Drawing.Point pos = button.MapClientToScreen(new Drawing.Point(0, button.ActualHeight));
			pos.X -= menu.ActualWidth;
			menu.ShowAsContextMenu(this.Window, pos);
		}


		private void HandleColorClicked(object sender, MessageEventArgs e)
		{
			//	Couleur dans la palette cliquée.
			ColorSample cs = sender as ColorSample;
			bool import = ( e != null && (e.Message.IsShiftPressed || e.Message.IsControlPressed) );
			this.SelectSample(cs, import);
		}

		
		private void HandleColorCollectionChanged(object sender)
		{
			//	La collection de couleurs a changé.
			this.Invalidate();
		}

		protected virtual void OnExport()
		{
			//	Génère un événement pour dire qu'on exporte une couleur.
			EventHandler handler = (EventHandler) this.GetUserEventHandler("Export");
			if (handler != null)
			{
				handler(this);
			}
		}

		protected virtual void OnImport()
		{
			//	Génère un événement pour dire qu'on importe une couleur.
			EventHandler handler = (EventHandler) this.GetUserEventHandler("Import");
			if (handler != null)
			{
				handler(this);
			}
		}

		
		public event EventHandler				Export
		{
			add
			{
				this.AddUserEventHandler("Export", value);
			}
			remove
			{
				this.RemoveUserEventHandler("Export", value);
			}
		}

		public event EventHandler				Import
		{
			add
			{
				this.AddUserEventHandler("Import", value);
			}
			remove
			{
				this.RemoveUserEventHandler("Import", value);
			}
		}


		protected int							nbColumns;
		protected int							nbRows;
		protected int							nbTotal;
		protected ColorSample[]					palette;
		protected int							selected;
		protected bool							hasOptionButton = false;
		protected GlyphButton					buttonOption;
		protected Drawing.ColorCollection		colorCollection;
	}
}

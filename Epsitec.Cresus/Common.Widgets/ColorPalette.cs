namespace Epsitec.Common.Widgets
{
	using BundleAttribute = Epsitec.Common.Support.BundleAttribute;
	
	/// <summary>
	/// La classe ColorPalette propose une palette de couleurs sous forme d'un tableau.
	/// Pour l'instant, le tableau est fixe et contient 2x8 échantillons.
	/// </summary>
	public class ColorPalette : Widget
	{
		public ColorPalette()
		{
			if ( Support.ObjectBundler.IsBooting )
			{
				//	N'initialise rien, car cela prend passablement de temps... et de toute
				//	manière, on n'a pas besoin de toutes ces informations pour pouvoir
				//	utiliser IBundleSupport.
				
				return;
			}
			
			this.nbColumns = 4;
			this.nbRows = 8;
			this.nbTotal = this.nbColumns*this.nbRows;

			this.palette = new ColorSample[this.nbTotal];
			for ( int i=0 ; i<this.nbTotal ; i++ )
			{
				this.palette[i] = new ColorSample(this);
				this.palette[i].Clicked += new MessageEventHandler(this.HandleColorClicked);
				this.palette[i].TabIndex = i;
				this.palette[i].TabNavigation = Widget.TabNavigationMode.ActivateOnTab;
			}

			this.palette[ 0].Color = Drawing.Color.FromARGB(0.0, 1.0, 1.0, 1.0);
			this.palette[ 1].Color = Drawing.Color.FromARGB(1.0, 1.0, 0.0, 0.0);
			this.palette[ 2].Color = Drawing.Color.FromARGB(1.0, 1.0, 1.0, 0.0);
			this.palette[ 3].Color = Drawing.Color.FromARGB(1.0, 0.0, 1.0, 0.0);
			this.palette[ 4].Color = Drawing.Color.FromARGB(1.0, 0.0, 1.0, 1.0);
			this.palette[ 5].Color = Drawing.Color.FromARGB(1.0, 0.0, 0.0, 1.0);
			this.palette[ 6].Color = Drawing.Color.FromARGB(1.0, 1.0, 0.0, 1.0);
			this.palette[ 7].Color = Drawing.Color.FromARGB(0.5, 0.5, 0.5, 0.5);

			this.palette[ 8].Color = Drawing.Color.FromARGB(1.0, 1.0, 1.0, 1.0);
			this.palette[ 9].Color = Drawing.Color.FromARGB(1.0, 0.9, 0.9, 0.9);
			this.palette[10].Color = Drawing.Color.FromARGB(1.0, 0.8, 0.8, 0.8);
			this.palette[11].Color = Drawing.Color.FromARGB(1.0, 0.7, 0.7, 0.7);
			this.palette[12].Color = Drawing.Color.FromARGB(1.0, 0.6, 0.6, 0.6);
			this.palette[13].Color = Drawing.Color.FromARGB(1.0, 0.5, 0.5, 0.5);
			this.palette[14].Color = Drawing.Color.FromARGB(1.0, 0.4, 0.4, 0.4);
			this.palette[15].Color = Drawing.Color.FromARGB(1.0, 0.0, 0.0, 0.0);

			this.selected = -1;

			this.buttonOption = new GlyphButton(this);
			this.buttonOption.GlyphShape = GlyphShape.ArrowLeft;
			this.buttonOption.ButtonStyle = ButtonStyle.Normal;
			this.buttonOption.Clicked += new MessageEventHandler(this.HandleButtonOptionClicked);
			ToolTip.Default.SetToolTip(this.buttonOption, "Options");
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

		
		public override double					DefaultWidth
		{
			get
			{
				return 20*this.nbColumns-1;
			}
		}

		public override double					DefaultHeight
		{
			get
			{
				return 20*this.nbRows-1;
			}
		}

		public int								Columns
		{
			get
			{
				return this.nbColumns;
			}

			set
			{
				// TODO
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
				// TODO
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
					this.UpdateClientGeometry();
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

		public Drawing.Color					Color
		{
			get
			{
				if ( this.selected >= 0 && this.selected < this.nbTotal )
				{
					return this.palette[this.selected].Color;
				}
				else
				{
					return Drawing.Color.Empty;
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

		// Donne la liste de couleurs à lier avec la palette.
		public Drawing.ColorCollection			ColorCollection
		{
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
						this.colorCollection.Changed -= new Support.EventHandler(this.HandleColorCollectionChanged);
					}

					this.colorCollection = value;

					if ( this.colorCollection != null )
					{
						this.colorCollection.Changed += new Support.EventHandler(this.HandleColorCollectionChanged);
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
		

		// Met à jour la géométrie.
		protected override void UpdateClientGeometry()
		{
			base.UpdateClientGeometry();

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
					this.palette[i].Bounds = r;
					i ++;
					pos.Y -= dy-1.0;
				}
				pos.X += dx-1.0;
			}

			if ( this.hasOptionButton )
			{
				Drawing.Rectangle r = new Drawing.Rectangle(rect.Left, rect.Top-14, 14, 14);
				this.buttonOption.Bounds = r;
				this.buttonOption.SetVisible(true);
			}
			else
			{
				this.buttonOption.SetVisible(false);
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

		
		// Bouton des options cliqué.
		private void HandleButtonOptionClicked(object sender, MessageEventArgs e)
		{
			GlyphButton button = sender as GlyphButton;

			VMenu menu = new VMenu();
			menu.Host = this;
			menu.Items.Add(new MenuItem("NewPaletteDefault", "", "Palette standard",    ""));
			menu.Items.Add(new MenuItem("NewPaletteRainbow", "", "Palette arc-en-ciel", ""));
			menu.Items.Add(new MenuItem("NewPaletteLight",   "", "Palette pastel",      ""));
			menu.Items.Add(new MenuItem("NewPaletteDark",    "", "Palette foncée",      ""));
			menu.Items.Add(new MenuItem("NewPaletteGray",    "", "Palette monochrome",  ""));
			menu.Items.Add(new MenuSeparator());
			menu.Items.Add(new MenuItem("OpenPalette", "", "Ouvrir palette...",      ""));
			menu.Items.Add(new MenuItem("SavePalette", "", "Enregistrer palette...", ""));
			menu.AdjustSize();

			Drawing.Point pos = button.MapClientToScreen(new Drawing.Point(0, button.Height));
			pos.X -= menu.Width;
			menu.ShowAsContextMenu(this.Window, pos);
		}


		// Couleur dans la palette cliquée.
		private void HandleColorClicked(object sender, MessageEventArgs e)
		{
			ColorSample cs = sender as ColorSample;

			for ( int i=0 ; i<this.nbTotal ; i++ )
			{
				if ( cs == this.palette[i] )
				{
					this.ColorSelected = i;

					if ( e != null && (e.Message.IsShiftPressed || e.Message.IsCtrlPressed) )
					{
						this.OnImport();
					}
					else
					{
						this.OnExport();
					}
					return;
				}
			}
		}

		
		// La collection de couleurs a changé.
		private void HandleColorCollectionChanged(object sender)
		{
			this.Invalidate();
		}

		// Génère un événement pour dire qu'on exporte une couleur.
		protected virtual void OnExport()
		{
			if ( this.Export != null )  // qq'un écoute ?
			{
				this.Export(this);
			}
		}

		// Génère un événement pour dire qu'on importe une couleur.
		protected virtual void OnImport()
		{
			if ( this.Import != null )  // qq'un écoute ?
			{
				this.Import(this);
			}
		}

		
		public event Support.EventHandler		Export;
		public event Support.EventHandler		Import;

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

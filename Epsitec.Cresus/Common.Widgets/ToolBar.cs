namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// La classe ToolBar permet de réaliser des tool bars.
	/// </summary>
	public class ToolBar : Widget
	{
		public ToolBar()
		{
			IconButton button = new IconButton();
			this.defaultButtonWidth = button.DefaultWidth;
			this.defaultButtonHeight = button.DefaultHeight;
			button.Dispose();

			this.colorControlLight      = Drawing.Color.FromName("ControlLight");
			this.colorControlLightLight = Drawing.Color.FromName("ControlLightLight");
			this.colorControlDark       = Drawing.Color.FromName("ControlDark");
			this.colorControlDarkDark   = Drawing.Color.FromName("ControlDarkDark");
		}

		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.array = null;
			}
			
			base.Dispose(disposing);
		}


		// Retourne la hauteur standard d'une barre.
		public override double DefaultHeight
		{
			get
			{
				return 28;
			}
		}

		// Ajoute une cellule IconButton.
		public int InsertIconButton(string name)
		{
			IconButton button = new IconButton();
			button.IconName = name;
			return this.Insert(button);
		}

		// Ajoute un séparateur.
		public int InsertSep(double width)
		{
			Widget sep = new Widget();
			sep.Size = new Drawing.Size(width, this.defaultButtonHeight);
			return this.Insert(sep);
		}

		// Ajoute une cellule.
		public int Insert(Widget cell)
		{
			int rank = this.totalUsed;
			this.AllocateArray(rank+1);
			this.array[rank].Dispose();
			this.array[rank] = cell;
			this.Justif();
			this.Children.Add(cell);
			this.totalUsed ++;
			return rank;
		}

		// Modifie une cellule.
		public void Modify(int rank, Widget cell)
		{
			this.AllocateArray(rank+1);
			this.array[rank].Dispose();
			this.array[rank] = cell;
			this.Justif();
			this.Children.Add(cell);
		}

		// Objet occupant une case.		
		public Widget this[int rank]
		{
			get
			{
				System.Diagnostics.Debug.Assert(this.array[rank] != null);
				return this.array[rank];
			}
			
			set
			{
				System.Diagnostics.Debug.Assert(this.array[rank] != null);
				if ( value == null )  value = new Widget();
				
				this.array[rank] = value;
			}
		}

		// Spécifie le nombre de cellules qui seront contenues dans la barre.
		// Cet appel est facultatif.
		public void SetSize(int max)
		{
			this.AllocateArray(max);
		}

		// Dimensionne le tableau des cellules si nécessaire.
		protected void AllocateArray(int max)
		{
			if ( this.array == null )
			{
				this.array = new Widget[0];  // alloue un tableau vide
			}

			if ( max <= this.array.Length )  return;  // déjà assez grand ?

			Widget[] newArray = new Widget[max];  // nouveau tableau
			for ( int i=0 ; i<max ; i++ )
			{
				if ( i < this.array.Length )
				{
					newArray[i] = this.array[i];
				}
				else
				{
					newArray[i] = new Widget();
					newArray[i].Size = new Drawing.Size(this.defaultButtonWidth, this.defaultButtonHeight);
				}
			}
			this.array = newArray;
		}

		// Positionne toutes les cellules, de gauche à droite.
		protected void Justif()
		{
			double x = (this.DefaultHeight-this.defaultButtonHeight)/2;
			foreach ( Widget cell in this.array )
			{
				Drawing.Size dim = cell.Size;
				Drawing.Point pos = new Drawing.Point();
				pos.X = x;
				pos.Y = System.Math.Floor((this.Height-dim.Height)/2);  // centré verticalement
				cell.Location = pos;

				x += dim.Width;
			}
		}

		// Dessine la barre.
		protected override void PaintBackgroundImplementation(Drawing.Graphics graphics, Drawing.Rectangle clipRect)
		{
			IAdorner adorner = Widgets.Adorner.Factory.Active;

			Drawing.Rectangle rect  = new Drawing.Rectangle(0, 0, this.Client.Width, this.Client.Height);
			adorner.PaintToolBackground(graphics, rect, WidgetState.None, Direction.None, Direction.None);
		}


		protected double			defaultButtonWidth;
		protected double			defaultButtonHeight;
		protected Widget[]			array;  // tableau des cellules
		protected int				totalUsed;
		protected Drawing.Color		colorControlLight;
		protected Drawing.Color		colorControlLightLight;
		protected Drawing.Color		colorControlDark;
		protected Drawing.Color		colorControlDarkDark;
	}
}

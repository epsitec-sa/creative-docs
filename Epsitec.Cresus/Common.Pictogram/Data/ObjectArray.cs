using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe ObjectArray est la classe de l'objet graphique "tableau".
	/// </summary>
	public class ObjectArray : AbstractObject
	{
		protected enum OutlinePos
		{
			Left,
			Right,
			Bottom,
			Top,
		}


		public ObjectArray()
		{
			PropertyLine lineMode = new PropertyLine();
			lineMode.Type = PropertyType.LineMode;
			this.AddProperty(lineMode);

			PropertyColor lineColor = new PropertyColor();
			lineColor.Type = PropertyType.LineColor;
			this.AddProperty(lineColor);

			PropertyColor backColor = new PropertyColor();
			backColor.Type = PropertyType.BackColor;
			this.AddProperty(backColor);

			PropertyString textString = new PropertyString();
			textString.Type = PropertyType.TextString;
			this.AddProperty(textString);

			PropertyFont textFont = new PropertyFont();
			textFont.Type = PropertyType.TextFont;
			this.AddProperty(textFont);

			PropertyJustif textJustif = new PropertyJustif();
			textJustif.Type = PropertyType.TextJustif;
			this.AddProperty(textJustif);

			this.columns = 1;
			this.rows    = 1;
			this.InitWidths();
			this.InitHeights();
			this.InitCells();

			this.draftStep = 0;  // choix taille d'une cellule
		}

		protected override AbstractObject CreateNewObject()
		{
			return new ObjectArray();
		}

		public override void Dispose()
		{
			base.Dispose();
		}


		// Nom de l'icône.
		public override string IconName
		{
			get { return @"file:images/array.icon"; }
		}


		// Nombre de colonnes.
		[XmlAttribute]
		public int Columns
		{
			get
			{
				return this.columns;
			}

			set
			{
				if ( this.columns != value )
				{
					this.columns = value;
					this.InitWidths();
					this.InitCells();
				}
			}
		}

		// Nombre de lignes.
		[XmlAttribute]
		public int Rows
		{
			get
			{
				return this.rows;
			}

			set
			{
				if ( this.rows != value )
				{
					this.rows = value;
					this.InitHeights();
					this.InitCells();
				}
			}
		}

		[XmlArrayItem("Cell", Type=typeof(ArrayCell))]
		public System.Collections.ArrayList Cells
		{
			get { return this.cells; }
			set { this.cells = value; }
		}

		// Accède à une cellule du tableau.
		protected ArrayCell Cell(int c, int r)
		{
			System.Diagnostics.Debug.Assert(c >= 0 && c <= this.columns);
			System.Diagnostics.Debug.Assert(r >= 0 && r <= this.rows);
			return this.cells[c+r*(this.columns+1)] as ArrayCell;
		}


		// Sélectionne ou désélectionne toutes les poignées de l'objet.
		public override void Select(bool select, bool edit)
		{
			base.Select(select, edit);
			this.DeselectAllCells();
			if ( select && edit )
			{
				this.cellToEdit = this.cellToHilite;
				if ( this.cellToEdit != -1 )
				{
					int c = this.cellToEdit%(this.columns+1);
					int r = this.cellToEdit/(this.columns+1);
					this.Cell(c,r).Selected = true;
				}
			}
			else
			{
				this.cellToHilite = -1;
				this.cellToEdit = -1;
			}
		}


		// Choix du mode de modification.
		public bool OutlineFrame
		{
			get { return this.outlineFrame; }
		}

		public bool OutlineHoriz
		{
			get { return this.outlineHoriz; }
		}

		public bool OutlineVerti
		{
			get { return this.outlineVerti; }
		}


		// Ajoute toutes les propriétés de l'objet dans une liste.
		// Un type de propriété donné n'est qu'une fois dans la liste.
		public override void PropertiesList(System.Collections.ArrayList list)
		{
			foreach ( AbstractProperty property in this.properties )
			{
				PropertyType type = property.Type;
				AbstractProperty p;

				if ( type == PropertyType.LineMode  ||
					 type == PropertyType.LineColor )  // trait ?
				{
					for ( int c=0 ; c<this.columns ; c++ )
					{
						for ( int r=0 ; r<this.rows ; r++ )
						{
							if ( this.selected && !this.Cell(c,r).Selected )  continue;

							p = this.GetPropertySel(type, c,r, OutlinePos.Left);
							if ( p != null )  this.PropertyAllList(list, p);

							p = this.GetPropertySel(type, c,r, OutlinePos.Right);
							if ( p != null )  this.PropertyAllList(list, p);

							p = this.GetPropertySel(type, c,r, OutlinePos.Bottom);
							if ( p != null )  this.PropertyAllList(list, p);

							p = this.GetPropertySel(type, c,r, OutlinePos.Top);
							if ( p != null )  this.PropertyAllList(list, p);
						}
					}
				}
				else
				{
					for ( int c=0 ; c<this.columns ; c++ )
					{
						for ( int r=0 ; r<this.rows ; r++ )
						{
							if ( this.selected && !this.Cell(c,r).Selected )  continue;

							p = this.Cell(c,r).Property(type, 0);
							if ( p != null )  this.PropertyAllList(list, p);
						}
					}
				}
			}
		}

		// Cherche une propriété d'après son type.
		protected override AbstractProperty SearchProperty(PropertyType type)
		{
			if ( this.onlyBase )
			{
				return base.SearchProperty(type);
			}
			else
			{
				for ( int c=0 ; c<this.columns+1 ; c++ )
				{
					for ( int r=0 ; r<this.rows+1 ; r++ )
					{
						if ( this.Cell(c,r).Selected )
						{
							return this.Cell(c,r).Property(type, 0);
						}
					}
				}
			}
			return null;
		}

		// Cherche si une propriété est liée à un style.
		public override bool IsLinkProperty(AbstractProperty property)
		{
			if ( property.StyleID == 0 )  return false;

			for ( int c=0 ; c<this.columns+1 ; c++ )
			{
				for ( int r=0 ; r<this.rows+1 ; r++ )
				{
					for ( int sub=0 ; sub<2 ; sub++ )
					{
						AbstractProperty actual = this.Cell(c,r).Property(property.Type, sub);
						if ( actual != null )
						{
							if ( actual.StyleID == property.StyleID )  return true;
						}
					}
				}
			}
			return false;
		}

		// Retourne une copie d'une propriété.
		public override AbstractProperty GetProperty(PropertyType type)
		{
			AbstractProperty property = null;

			if ( type == PropertyType.LineMode  ||
				 type == PropertyType.LineColor )  // trait ?
			{
				for ( int c=0 ; c<this.columns ; c++ )
				{
					for ( int r=0 ; r<this.rows ; r++ )
					{
						if ( this.selected && !this.Cell(c,r).Selected )  continue;

						property = this.GetPropertySel(type, c,r, OutlinePos.Left);
						if ( property != null )  goto found;

						property = this.GetPropertySel(type, c,r, OutlinePos.Right);
						if ( property != null )  goto found;

						property = this.GetPropertySel(type, c,r, OutlinePos.Bottom);
						if ( property != null )  goto found;

						property = this.GetPropertySel(type, c,r, OutlinePos.Top);
						if ( property != null )  goto found;
					}
				}
			}
			else	// surface ?
			{
				for ( int c=0 ; c<this.columns ; c++ )
				{
					for ( int r=0 ; r<this.rows ; r++ )
					{
						if ( this.selected && !this.Cell(c,r).Selected )  continue;

						property = this.Cell(c,r).Property(type, 0);
						if ( property != null )  goto found;
					}
				}
			}
			return null;

			found:
			AbstractProperty copy = AbstractProperty.NewProperty(type);
			if ( copy == null )  return null;
			property.CopyTo(copy);
			return copy;
		}

		// Modifie une propriété.
		public override void SetProperty(AbstractProperty property)
		{
			this.onlyBase = true;
			base.SetProperty(property);
			this.onlyBase = false;

			if ( property.Type == PropertyType.LineMode  ||
				 property.Type == PropertyType.LineColor )  // trait ?
			{
				for ( int c=0 ; c<this.columns ; c++ )
				{
					for ( int r=0 ; r<this.rows ; r++ )
					{
						if ( this.PropertySelected(property, c,r, OutlinePos.Left) )
						{
							this.SetProperty(property, c,r, OutlinePos.Left);
						}
						if ( this.PropertySelected(property, c,r, OutlinePos.Right) )
						{
							this.SetProperty(property, c,r, OutlinePos.Right);
						}
						if ( this.PropertySelected(property, c,r, OutlinePos.Bottom) )
						{
							this.SetProperty(property, c,r, OutlinePos.Bottom);
						}
						if ( this.PropertySelected(property, c,r, OutlinePos.Top) )
						{
							this.SetProperty(property, c,r, OutlinePos.Top);
						}
					}
				}
			}
			else	// surface ?
			{
				for ( int c=0 ; c<this.columns ; c++ )
				{
					for ( int r=0 ; r<this.rows ; r++ )
					{
						if ( this.PropertySelected(property, c,r) )
						{
							this.Cell(c,r).SetProperty(property, 0);
						}
					}
				}
			}
		}

		// Indique si une cellule est sélectionnée, soit si elle est directement
		// sélectionnée, ou si elle utilise le même style que la propriété modifiée.
		protected bool PropertySelected(AbstractProperty property, int c, int r, OutlinePos pos)
		{
			if ( this.selected )
			{
				if ( this.Cell(c,r).Selected && this.PropertyOutline(c,r, pos) )  return true;
				if ( property.StyleID == 0 )  return false;
			}
			else
			{
				if ( property.StyleID == 0 )  return true;
			}

			AbstractProperty actual = this.GetProperty(property.Type, c,r, pos);
			if ( actual == null )  return false;
			return ( actual.StyleID == property.StyleID );
		}

		protected bool PropertySelected(AbstractProperty property, int c, int r)
		{
			if ( this.selected )
			{
				if ( this.Cell(c,r).Selected )  return true;
				if ( property.StyleID == 0 )  return false;
			}
			else
			{
				if ( property.StyleID == 0 )  return true;
			}

			AbstractProperty actual = this.Cell(c,r).Property(property.Type, 0);
			if ( actual == null )  return false;
			return ( actual.StyleID == property.StyleID );
		}

		// Regarde si un trait autour de la cellule fait partie de la sélection.
		protected bool PropertyOutline(int c, int r, OutlinePos pos)
		{
			if ( this.outlineFrame )
			{
				switch ( pos  )
				{
					case OutlinePos.Left:    if ( !this.IsSelectCell(c-1, r+0) )  return true;  break;
					case OutlinePos.Right:   if ( !this.IsSelectCell(c+1, r+0) )  return true;  break;
					case OutlinePos.Bottom:  if ( !this.IsSelectCell(c+0, r-1) )  return true;  break;
					case OutlinePos.Top:     if ( !this.IsSelectCell(c+0, r+1) )  return true;  break;
				}
			}

			if ( this.outlineHoriz )
			{
				switch ( pos  )
				{
					case OutlinePos.Bottom:  if ( this.IsSelectCell(c+0, r-1) )  return true;  break;
					case OutlinePos.Top:     if ( this.IsSelectCell(c+0, r+1) )  return true;  break;
				}
			}

			if ( this.outlineVerti )
			{
				switch ( pos  )
				{
					case OutlinePos.Left:    if ( this.IsSelectCell(c-1, r+0) )  return true;  break;
					case OutlinePos.Right:   if ( this.IsSelectCell(c+1, r+0) )  return true;  break;
				}
			}

			return false;
		}

		// Donne une propriété d'un trait autour de la cellule, si elle est sélectionnée.
		protected AbstractProperty GetPropertySel(PropertyType type, int c, int r, OutlinePos pos)
		{
			if ( !this.PropertyOutline(c,r, pos) )  return null;
			return this.GetProperty(type, c,r, pos);
		}

		// Donne une propriété d'un trait autour de la cellule.
		protected AbstractProperty GetProperty(PropertyType type, int c, int r, OutlinePos pos)
		{
			switch ( pos  )
			{
				case OutlinePos.Left:    return this.Cell(c+0,r+0).Property(type, 0);
				case OutlinePos.Right:   return this.Cell(c+1,r+0).Property(type, 0);
				case OutlinePos.Bottom:  return this.Cell(c+0,r+0).Property(type, 1);
				case OutlinePos.Top:     return this.Cell(c+0,r+1).Property(type, 1);
			}
			return null;
		}

		// Modifie une propriété d'un trait autour de la cellule.
		protected void SetProperty(AbstractProperty property, int c, int r, OutlinePos pos)
		{
			switch ( pos  )
			{
				case OutlinePos.Left:    this.Cell(c+0,r+0).SetProperty(property, 0);  break;
				case OutlinePos.Right:   this.Cell(c+1,r+0).SetProperty(property, 0);  break;
				case OutlinePos.Bottom:  this.Cell(c+0,r+0).SetProperty(property, 1);  break;
				case OutlinePos.Top:     this.Cell(c+0,r+1).SetProperty(property, 1);  break;
			}
		}

		// Reprend toutes les propriétés d'un objet source.
		public override void CloneProperties(AbstractObject src)
		{
			if ( src == null )  return;
			base.CloneProperties(src);

			AbstractProperty property;
			for ( int c=0 ; c<this.columns+1 ; c++ )
			{
				for ( int r=0 ; r<this.rows+1 ; r++ )
				{
					int total = src.TotalProperty;
					for ( int i=0 ; i<total ; i++ )
					{
						property = src.Property(i);
						if ( property == null )  continue;
						this.Cell(c,r).SetProperty(property, 0);
						this.Cell(c,r).SetProperty(property, 1);
					}
				}
			}
		}

		// Adapte les styles de l'objet collé, qui peut provenir d'un autre fichier,
		// donc d'une autre collection de styles. On se base sur le nom des styles
		// (StyleName) pour faire la correspondance.
		// Si on trouve un nom identique -> le style de l'objet collé est modifié
		// en fonction du style existant.
		// Si on ne trouve pas un nom identique -> on crée un nouveau style, en
		// modifiant bien entendu l'identificateur (StyleID) de l'objet collé.
		public override void PasteAdaptStyles(StylesCollection stylesCollection)
		{
			for ( int c=0 ; c<this.columns+1 ; c++ )
			{
				for ( int r=0 ; r<this.rows+1 ; r++ )
				{
					int total = this.TotalProperty;
					for ( int i=0 ; i<total ; i++ )
					{
						PropertyType type = base.Property(i).Type;
						for ( int sub=0 ; sub<2 ; sub++ )
						{
							AbstractProperty property = this.Cell(c,r).Property(type, sub);
							if ( property == null )  break;

							if ( property.StyleID == 0 )  continue;  // n'utilise pas un style ?

							AbstractProperty style = stylesCollection.SearchProperty(property);
							if ( style == null )
							{
								int rank = stylesCollection.AddProperty(property);
								style = stylesCollection.GetProperty(rank);
							}
							style.CopyTo(property);
						}
					}
				}
			}
		}


		// Détecte si la souris est sur l'objet.
		public override bool Detect(Drawing.Point pos)
		{
			if ( this.isHide )  return false;

			Drawing.Rectangle bbox = this.BoundingBox;
			if ( !bbox.Contains(pos) )  return false;

			Drawing.Path path = this.PathBuild();
			path.Close();
			return AbstractObject.DetectSurface(path, pos);
		}

		// Détecte si la souris est sur l'objet pour l'éditer.
		public override bool DetectEdit(Drawing.Point pos)
		{
			if ( this.isHide )  return false;

			this.cellToHilite = this.DetectCell(pos);
			return (this.cellToHilite != -1);
		}


		// Déplace une poignée.
		public override void MoveHandleProcess(int rank, Drawing.Point pos, IconContext iconContext)
		{
			if ( rank >= this.handles.Count )  // poignée d'une propriété ?
			{
				base.MoveHandleProcess(rank, pos, iconContext);
				return;
			}

			iconContext.ConstrainSnapPos(ref pos);
			iconContext.SnapGrid(ref pos);

			if ( rank < 4 )
			{
				if ( AbstractObject.IsRectangular(this.Handle(0).Position, this.Handle(1).Position, this.Handle(2).Position, this.Handle(3).Position) )
				{
					this.Handle(rank).Position = pos;

					if ( rank == 0 )
					{
						this.Handle(2).Position = Drawing.Point.Projection(this.Handle(2).Position, this.Handle(1).Position, pos);
						this.Handle(3).Position = Drawing.Point.Projection(this.Handle(3).Position, this.Handle(1).Position, pos);
					}
					if ( rank == 1 )
					{
						this.Handle(2).Position = Drawing.Point.Projection(this.Handle(2).Position, this.Handle(0).Position, pos);
						this.Handle(3).Position = Drawing.Point.Projection(this.Handle(3).Position, this.Handle(0).Position, pos);
					}
					if ( rank == 2 )
					{
						this.Handle(0).Position = Drawing.Point.Projection(this.Handle(0).Position, this.Handle(3).Position, pos);
						this.Handle(1).Position = Drawing.Point.Projection(this.Handle(1).Position, this.Handle(3).Position, pos);
					}
					if ( rank == 3 )
					{
						this.Handle(0).Position = Drawing.Point.Projection(this.Handle(0).Position, this.Handle(2).Position, pos);
						this.Handle(1).Position = Drawing.Point.Projection(this.Handle(1).Position, this.Handle(2).Position, pos);
					}
				}
				else
				{
					this.Handle(rank).Position = pos;
				}
			}
			else if ( rank == 4 )  // déplace tout l'objet ?
			{
				Drawing.Point move = pos-this.Handle(4).Position;
				this.Handle(0).Position += move;
				this.Handle(1).Position += move;
				this.Handle(2).Position += move;
				this.Handle(3).Position += move;
			}
			else
			{
				rank -= 4+1;  // 0..n
				if ( this.columnWidth != 0 )
				{
					if ( rank >= 0 && rank < this.columns-1 )
					{
						double dist = Drawing.Point.Distance(this.columnStart, pos)/this.columnWidth;
						double min = (rank==0) ? 0 : this.widths[rank-1];
						double max = (rank<this.columns-2) ? this.widths[rank+1] : 1;
						dist = System.Math.Max(dist, min+0.02);
						dist = System.Math.Min(dist, max-0.02);
						this.widths[rank] = dist;
					}
					rank -= this.columns-1;  // 0..n
				}
				if ( this.rowHeight != 0 )
				{
					if ( rank >= 0 && rank < this.rows-1 )
					{
						double dist = Drawing.Point.Distance(this.rowStart, pos)/this.rowHeight;
						double min = (rank==0) ? 0 : this.heights[rank-1];
						double max = (rank<this.rows-2) ? this.heights[rank+1] : 1;
						dist = System.Math.Max(dist, min+0.02);
						dist = System.Math.Min(dist, max-0.02);
						this.heights[rank] = dist;
					}
					rank -= this.rows-1;  // 0..n
				}
			}

			this.UpdateHandle();
			this.dirtyBbox = true;
		}

		// Déplace tout l'objet.
		public override void MoveAll(Drawing.Point move, bool all)
		{
			base.MoveAll(move, all);
			this.UpdateHandle();
		}


		// Déplace globalement l'objet.
		public override void MoveGlobal(GlobalModifierData initial, GlobalModifierData final, bool all)
		{
			base.MoveGlobal(initial, final, all);
			this.UpdateHandle();
		}


		// Retourne les noms des assistants.
		public static bool CommandLook(int rank, out string text, out string name)
		{
			switch ( rank )
			{
				case  0:  text = "Numéros";              name = "Num";      return true;
				case  1:  text = "Lettres";              name = "Letter";   return true;
				case  2:  text = "Jours de la semaine";  name = "Week";     return true;
				case  3:  text = "Mois";                 name = "Month";    return true;
				case  4:  text = "";                     name = "";         return true;
				case  5:  text = "Alterné blanc/gris";   name = "Alter1";   return true;
				case  6:  text = "Alterné bleu/jaune";   name = "Alter2";   return true;
				case  7:  text = "";                     name = "";         return true;
				case  8:  text = "En-tête claire";       name = "Header1";  return true;
				case  9:  text = "En-tête foncée";       name = "Header2";  return true;
			}
			text = "";
			name = "";
			return false;
		}

		// Indique l'état des commandes.
		public bool EnabledAddColumnLeft
		{
			get
			{
				int c1,r1, c2,r2;
				if ( !this.RetSelectedRect(out c1, out r1, out c2, out r2) )  return false;
				return ( this.columns+(c2-c1+1) <= this.maxColumns );
			}
		}

		public bool EnabledAddColumnRight
		{
			get { return this.EnabledAddColumnLeft; }
		}

		public bool EnabledAddRowTop
		{
			get
			{
				int c1,r1, c2,r2;
				if ( !this.RetSelectedRect(out c1, out r1, out c2, out r2) )  return false;
				return ( this.rows+(r2-r1+1) <= this.maxRows );
			}
		}

		public bool EnabledAddRowBottom
		{
			get { return this.EnabledAddRowTop; }
		}

		public bool EnabledDelColumn
		{
			get
			{
				int c1,r1, c2,r2;
				if ( !this.RetSelectedRect(out c1, out r1, out c2, out r2) )  return false;
				return ( this.columns-(c2-c1+1) >= 1 );
			}
		}

		public bool EnabledDelRow
		{
			get
			{
				int c1,r1, c2,r2;
				if ( !this.RetSelectedRect(out c1, out r1, out c2, out r2) )  return false;
				return ( this.rows-(r2-r1+1) >= 1 );
			}
		}

		public bool EnabledAlignColumn
		{
			get
			{
				int c1,r1, c2,r2;
				if ( !this.RetSelectedRect(out c1, out r1, out c2, out r2) )  return false;
				return ( (c2-c1+1) > 1 );
			}
		}

		public bool EnabledAlignRow
		{
			get
			{
				int c1,r1, c2,r2;
				if ( !this.RetSelectedRect(out c1, out r1, out c2, out r2) )  return false;
				return ( (r2-r1+1) > 1 );
			}
		}

		public bool EnabledSwapColumn
		{
			get
			{
				int c1,r1, c2,r2;
				if ( !this.RetSelectedRect(out c1, out r1, out c2, out r2) )  return false;
				return ( (c2-c1+1) > 1 );
			}
		}

		public bool EnabledSwapRow
		{
			get
			{
				int c1,r1, c2,r2;
				if ( !this.RetSelectedRect(out c1, out r1, out c2, out r2) )  return false;
				return ( (r2-r1+1) > 1 );
			}
		}

		public bool EnabledLook
		{
			get { return this.RetSelectedRect(); }
		}

		// Exécute une commande.
		public void ExecuteCommand(string cmd, string arg)
		{
			int c1,r1, c2,r2;
			this.RetSelectedRect(out c1, out r1, out c2, out r2);

			if ( cmd == "ArrayOutlineFrame" )
			{
				this.outlineFrame = !this.outlineFrame;
			}
			if ( cmd == "ArrayOutlineHoriz" )
			{
				this.outlineHoriz = !this.outlineHoriz;
			}
			if ( cmd == "ArrayOutlineVerti" )
			{
				this.outlineVerti = !this.outlineVerti;
			}

			if ( cmd == "ArrayAddColumnLeft" )
			{
				this.InsertColumnLeft(c1, c2-c1+1, false);
			}
			if ( cmd == "ArrayAddColumnRight" )
			{
				this.InsertColumnRight(c1, c2-c1+1, false);
			}

			if ( cmd == "ArrayAddRowTop" )
			{
				this.InsertRowTop(r1, r2-r1+1, false);
			}
			if ( cmd == "ArrayAddRowBottom" )
			{
				this.InsertRowBottom(r1, r2-r1+1, false);
			}

			if ( cmd == "ArrayDelColumn" )
			{
				this.DeleteColumn(c1, c2-c1+1);
			}
			if ( cmd == "ArrayDelRow" )
			{
				this.DeleteRow(r1, r2-r1+1);
			}

			if ( cmd == "ArrayAlignColumn" )
			{
				this.AlignColumn(c1, c2-c1+1);
			}
			if ( cmd == "ArrayAlignRow" )
			{
				this.AlignRow(r1, r2-r1+1);
			}

			if ( cmd == "ArraySwapColumn" )
			{
				this.SwapColumn(c1, c2-c1+1, r1,r2);
			}
			if ( cmd == "ArraySwapRow" )
			{
				this.SwapRow(r1, r2-r1+1, c1,c2);
			}

			if ( cmd == "ArrayLook" )
			{
				switch ( arg )
				{
					case "Header1":  this.LookHeader1();   break;
					case "Header2":  this.LookHeader2();   break;
					case "Alter1":   this.LookAlter1();    break;
					case "Alter2":   this.LookAlter2();    break;
					default:         this.LookSerie(arg);  break;
				}
			}
		}

		// Insère des colonnes avant la colonne spécifiée.
		protected void InsertColumnLeft(int rank, int total, bool duplicate)
		{
			int ranksrc = rank;
			int rankdst = rank;
			for ( int t=0 ; t<total ; t++ )
			{
				for ( int r=this.rows ; r>=0 ; r-- )
				{
					int isrc = r*(this.columns+1)+ranksrc;
					int idst = r*(this.columns+1)+rankdst;
					ArrayCell newCell = this.NewCell();
					ArrayCell actualCell = this.cells[isrc] as ArrayCell;
					actualCell.CopyTo(newCell);
					if ( !duplicate )  newCell.TextString.String = "";
					this.cells.Insert(idst, newCell);
					newCell.Selected = (r < this.rows);
					actualCell.Selected = false;
				}
				ranksrc += 2;
				rankdst += 1;
				this.columns ++;
			}
			this.InitWidths();
			this.UpdateHandle();
		}

		// Insère des colonnes après la colonne spécifiée.
		protected void InsertColumnRight(int rank, int total, bool duplicate)
		{
			int ranksrc = rank+(total-1);
			int rankdst = rank+total;
			for ( int t=0 ; t<total ; t++ )
			{
				for ( int r=this.rows ; r>=0 ; r-- )
				{
					int isrc = r*(this.columns+1)+ranksrc;
					int idst = r*(this.columns+1)+rankdst;
					ArrayCell newCell = this.NewCell();
					ArrayCell actualCell = this.cells[isrc] as ArrayCell;
					actualCell.CopyTo(newCell);
					if ( !duplicate )  newCell.TextString.String = "";
					this.cells.Insert(idst, newCell);
					newCell.Selected = (r < this.rows);
					actualCell.Selected = false;
				}
				ranksrc -= 1;
				this.columns ++;
			}
			this.InitWidths();
			this.UpdateHandle();
		}

		// Insère des lignes avant la ligne spécifiée.
		protected void InsertRowBottom(int rank, int total, bool duplicate)
		{
			int ranksrc = rank;
			int rankdst = rank;
			for ( int t=0 ; t<total ; t++ )
			{
				int shift = 0;
				for ( int c=0 ; c<=this.columns ; c++ )
				{
					int isrc = c+ranksrc*(this.columns+1);
					int idst = c+rankdst*(this.columns+1);
					ArrayCell newCell = this.NewCell();
					ArrayCell actualCell = this.cells[isrc+shift] as ArrayCell;
					actualCell.CopyTo(newCell);
					if ( !duplicate )  newCell.TextString.String = "";
					this.cells.Insert(idst, newCell);
					shift ++;
					newCell.Selected = (c < this.columns);
					actualCell.Selected = false;
				}
				ranksrc += 2;
				rankdst += 1;
				this.rows ++;
			}
			this.InitHeights();
			this.UpdateHandle();
		}

		// Insère des lignes après la ligne spécifiée.
		protected void InsertRowTop(int rank, int total, bool duplicate)
		{
			int ranksrc = rank+(total-1);
			int rankdst = rank+total;
			for ( int t=0 ; t<total ; t++ )
			{
				for ( int c=0 ; c<=this.columns ; c++ )
				{
					int isrc = c+ranksrc*(this.columns+1);
					int idst = c+rankdst*(this.columns+1);
					ArrayCell newCell = this.NewCell();
					ArrayCell actualCell = this.cells[isrc] as ArrayCell;
					actualCell.CopyTo(newCell);
					if ( !duplicate )  newCell.TextString.String = "";
					this.cells.Insert(idst, newCell);
					newCell.Selected = (c < this.columns);
					actualCell.Selected = false;
				}
				ranksrc -= 1;
				this.rows ++;
			}
			this.InitHeights();
			this.UpdateHandle();
		}

		// Supprime une colonne.
		protected void DeleteColumn(int rank, int total)
		{
			while ( total > 0 )
			{
				for ( int r=this.rows ; r>=0 ; r-- )
				{
					int i = r*(this.columns+1)+rank;
					this.cells.RemoveAt(i);
				}
				this.columns --;
				total --;
			}
			this.InitWidths();
			this.UpdateHandle();
		}

		// Supprime une ligne.
		protected void DeleteRow(int rank, int total)
		{
			while ( total > 0 )
			{
				for ( int c=this.columns ; c>=0 ; c-- )
				{
					int i = c+rank*(this.columns+1);
					this.cells.RemoveAt(i);
				}
				this.rows --;
				total --;
			}
			this.InitHeights();
			this.UpdateHandle();
		}

		// Aligne les largeurs des colonnes.
		protected void AlignColumn(int rank, int total)
		{
			double start;
			if ( rank == 0 )  start = 0;
			else              start = this.widths[rank-1];

			int rr = rank+total-1;
			double end;
			if ( rr < this.columns-1 )  end = this.widths[rr];
			else                        end = 1;

			double width = (end-start)/total;
			for ( int c=rank ; c<rank+total-1 ; c++ )
			{
				start += width;
				this.widths[c] = start;
			}

			this.UpdateHandle();
		}

		// Aligne les hauteurs des lignes.
		protected void AlignRow(int rank, int total)
		{
			double start;
			if ( rank == 0 )  start = 0;
			else              start = this.heights[rank-1];

			int rr = rank+total-1;
			double end;
			if ( rr < this.rows-1 )  end = this.heights[rr];
			else                     end = 1;

			double height = (end-start)/total;
			for ( int r=rank ; r<rank+total-1 ; r++ )
			{
				start += height;
				this.heights[r] = start;
			}

			this.UpdateHandle();
		}

		// Permute les colonnes.
		protected void SwapColumn(int rank, int total, int r1, int r2)
		{
			int cc = rank+total-1;
			for ( int c=rank ; c<rank+total/2 ; c++ )
			{
				for ( int r=r1 ; r<=r2 ; r++ )
				{
					string text = this.Cell(c,r).TextString.String;
					this.Cell(c,r).TextString.String = this.Cell(cc,r).TextString.String;
					this.Cell(cc,r).TextString.String = text;
				}
				cc --;
			}
			this.UpdateHandle();
		}

		// Permute les lignes.
		protected void SwapRow(int rank, int total, int c1, int c2)
		{
			int rr = rank+total-1;
			for ( int r=rank ; r<rank+total/2 ; r++ )
			{
				for ( int c=c1 ; c<=c2 ; c++ )
				{
					string text = this.Cell(c,r).TextString.String;
					this.Cell(c,r).TextString.String = this.Cell(c,rr).TextString.String;
					this.Cell(c,rr).TextString.String = text;
				}
				rr --;
			}
			this.UpdateHandle();
		}

		// Met le look à la sélection.
		protected void LookHeader1()
		{
			int c1,r1, c2,r2;
			if ( !this.RetSelectedRect(out c1, out r1, out c2, out r2) )  return;

			for ( int c=c1 ; c<=c2 ; c++ )
			{
				for ( int r=r1 ; r<=r2 ; r++ )
				{
					Drawing.Color color;
					if ( c == c1 || r == r2 )
					{
						color = Drawing.Color.FromBrightness(0.7);
					}
					else
					{
						color = Drawing.Color.FromBrightness(1.0);
					}
					this.Cell(c,r).BackColor.Color = color;

					this.Cell(c,r).TextFont.FontColor = Drawing.Color.FromBrightness(0.0);
				}
			}
		}

		// Met le look à la sélection.
		protected void LookHeader2()
		{
			int c1,r1, c2,r2;
			if ( !this.RetSelectedRect(out c1, out r1, out c2, out r2) )  return;

			for ( int c=c1 ; c<=c2 ; c++ )
			{
				for ( int r=r1 ; r<=r2 ; r++ )
				{
					Drawing.Color color;
					if ( c == c1 || r == r2 )
					{
						color = Drawing.Color.FromBrightness(0.3);
					}
					else
					{
						color = Drawing.Color.FromBrightness(1.0);
					}
					this.Cell(c,r).BackColor.Color = color;

					if ( c == c1 || r == r2 )
					{
						color = Drawing.Color.FromBrightness(1.0);
					}
					else
					{
						color = Drawing.Color.FromBrightness(0.0);
					}
					this.Cell(c,r).TextFont.FontColor = color;
				}
			}
		}

		// Met le look à la sélection.
		protected void LookAlter1()
		{
			int c1,r1, c2,r2;
			if ( !this.RetSelectedRect(out c1, out r1, out c2, out r2) )  return;

			for ( int c=c1 ; c<=c2 ; c++ )
			{
				for ( int r=r1 ; r<=r2 ; r++ )
				{
					Drawing.Color color;
					if ( r%2 ==0 )  color = Drawing.Color.FromBrightness(1.0);
					else            color = Drawing.Color.FromBrightness(0.9);
					this.Cell(c,r).BackColor.Color = color;

					this.Cell(c,r).TextFont.FontColor = Drawing.Color.FromBrightness(0.0);
				}
			}
		}

		// Met le look à la sélection.
		protected void LookAlter2()
		{
			int c1,r1, c2,r2;
			if ( !this.RetSelectedRect(out c1, out r1, out c2, out r2) )  return;

			for ( int c=c1 ; c<=c2 ; c++ )
			{
				for ( int r=r1 ; r<=r2 ; r++ )
				{
					Drawing.Color color;
					if ( r%2 ==0 )  color = Drawing.Color.FromRGB(0.9, 1.0, 1.0);  // bleuté
					else            color = Drawing.Color.FromRGB(1.0, 1.0, 0.9);  // jaune
					this.Cell(c,r).BackColor.Color = color;

					this.Cell(c,r).TextFont.FontColor = Drawing.Color.FromBrightness(0.0);
				}
			}
		}

		// Retourne la chaîne d'une série.
		protected static string Serie(string arg, int rank)
		{
			switch ( arg )
			{
				case "Num":
					return (rank+1).ToString();
					
				case "Letter":
					rank %= 26;
					return ((char)('A'+rank)).ToString();

				case "Week":
					rank %= 7;
					if ( rank == 0 )  return "Lundi";
					if ( rank == 1 )  return "Mardi";
					if ( rank == 2 )  return "Mercredi";
					if ( rank == 3 )  return "Jeudi";
					if ( rank == 4 )  return "Vendredi";
					if ( rank == 5 )  return "Samedi";
					if ( rank == 6 )  return "Dimanche";
					break;

				case "Month":
					rank %= 12;
					if ( rank ==  0 )  return "Janvier";
					if ( rank ==  1 )  return "Février";
					if ( rank ==  2 )  return "Mars";
					if ( rank ==  3 )  return "Avril";
					if ( rank ==  4 )  return "Mai";
					if ( rank ==  5 )  return "Juin";
					if ( rank ==  6 )  return "Juillet";
					if ( rank ==  7 )  return "Août";
					if ( rank ==  8 )  return "Septembre";
					if ( rank ==  9 )  return "Octobre";
					if ( rank == 10 )  return "Novembre";
					if ( rank == 11 )  return "Décembre";
					break;
			}
			return "";
		}

		// Met le look à la sélection.
		protected void LookSerie(string arg)
		{
			int c1,r1, c2,r2;
			if ( !this.RetSelectedRect(out c1, out r1, out c2, out r2) )  return;

			int rank = 0;
			for ( int c=c1 ; c<=c2 ; c++ )
			{
				for ( int r=r2 ; r>=r1 ; r-- )  // de haut en bas !
				{
					this.Cell(c,r).TextString.String = ObjectArray.Serie(arg, rank++);
				}
			}
		}

		// Indique si la zone sélectionnée est rectangulaire.
		protected bool RetSelectedRect()
		{
			int c1,r1, c2,r2;
			return this.RetSelectedRect(out c1, out r1, out c2, out r2);
		}

		// Retourne la zone rectangulaire sélectionnée.
		protected bool RetSelectedRect(out int c1, out int r1, out int c2, out int r2)
		{
			c1 = r1 = c2 = r2 = -1;
			int total = (this.columns+1) * (this.rows+1);

			int min = -1;
			for ( int i=0 ; i<total ; i++ )
			{
				ArrayCell cell = this.cells[i] as ArrayCell;
				if ( cell.Selected )  { min = i;  break; }
			}
			if ( min == -1 )  return false;
			c1 = min%(this.columns+1);
			r1 = min/(this.columns+1);

			int max = -1;
			for ( int i=total-1 ; i>=0 ; i-- )
			{
				ArrayCell cell = this.cells[i] as ArrayCell;
				if ( cell.Selected )  { max = i;  break; }
			}
			if ( max == -1 )  return false;
			c2 = max%(this.columns+1);
			r2 = max/(this.columns+1);

			for ( int c=0 ; c<this.columns ; c++ )
			{
				for ( int r=0 ; r<this.rows ; r++ )
				{
					bool inside = (c >= c1 && c <= c2 && r >= r1 && r <= r2);
					if ( this.Cell(c,r).Selected != inside )  return false;
				}
			}
			return true;
		}


		// Détecte la cellule pointée par la souris.
		public override int DetectCell(Drawing.Point pos)
		{
			for ( int c=0 ; c<this.columns ; c++ )
			{
				for ( int r=0 ; r<this.rows ; r++ )
				{
					InsideSurface inside = new InsideSurface(pos, 4);
					inside.AddLine(this.Cell(c,r).BottomLeft,  this.Cell(c,r).TopLeft    );
					inside.AddLine(this.Cell(c,r).TopLeft,     this.Cell(c,r).TopRight   );
					inside.AddLine(this.Cell(c,r).TopRight,    this.Cell(c,r).BottomRight);
					inside.AddLine(this.Cell(c,r).BottomRight, this.Cell(c,r).BottomLeft );
					if ( inside.IsInside() )  return c + r*(this.columns+1);
				}
			}
			return -1;
		}

		// Début du déplacement d'une cellule.
		public override void MoveCellStarting(int rank, Drawing.Point pos,
											  bool isShift, bool isCtrl, int downCount,
											  IconContext iconContext)
		{
			if ( rank == -1 )  return;
			int c = rank%(this.columns+1);
			int r = rank/(this.columns+1);
			System.Diagnostics.Debug.Assert(c >= 0 && c < this.columns);
			System.Diagnostics.Debug.Assert(r >= 0 && r < this.rows);
			this.startColumn = c;
			this.startRow    = r;

			if ( !isCtrl )  this.DeselectAllCells();

			if ( downCount <= 1 )  // simple clic ?
			{
				this.Cell(c,r).Selected = !this.Cell(c,r).Selected;
			}
			else if ( downCount <= 2 )  // double clic ?
			{
				if ( c == 0 )  // dans la colonne de gauche ?
				{
					for ( c=0 ; c<this.columns ; c++ )
					{
						this.Cell(c,r).Selected = true;  // sélectionne toute la ligne
					}
				}
				else
				{
					for ( r=0 ; r<this.rows ; r++ )
					{
						this.Cell(c,r).Selected = true;  // sélectionne toute la colonne
					}
				}
			}
			else	// triple clic ?
			{
				this.SelectAllCells();  // sélectionne tout le tableau
			}
		}

		// Déplace une cellule.
		public override void MoveCellProcess(int rank, Drawing.Point pos,
											 bool isShift, bool isCtrl,
											 IconContext iconContext)
		{
			if ( rank == -1 )  return;
			int c = rank%(this.columns+1);
			int r = rank/(this.columns+1);
			System.Diagnostics.Debug.Assert(c >= 0 && c < this.columns);
			System.Diagnostics.Debug.Assert(r >= 0 && r < this.rows);
			if ( this.startColumn != c || this.startRow != r )
			{
				int c1 = System.Math.Min(c, this.startColumn);
				int c2 = System.Math.Max(c, this.startColumn);
				int r1 = System.Math.Min(r, this.startRow);
				int r2 = System.Math.Max(r, this.startRow);

				for ( c=0 ; c<this.columns ; c++ )
				{
					for ( r=0 ; r<this.rows ; r++ )
					{
						this.Cell(c,r).Selected = (c >= c1 && c <= c2 && r >= r1 && r <= r2);
					}
				}
			}
		}


		// Début de la création d'un objet.
		public override void CreateMouseDown(Drawing.Point pos, IconContext iconContext)
		{
			if ( this.draftStep == 0 )  // choix taille d'une cellule ?
			{
				iconContext.ConstrainFixStarting(pos, ConstrainType.Square);
				this.HandleAdd(pos, HandleType.Primary);  // rang = 0
				this.HandleAdd(pos, HandleType.Primary);  // rang = 1
			}
		}

		// Déplacement pendant la création d'un objet.
		public override void CreateMouseMove(Drawing.Point pos, IconContext iconContext)
		{
			iconContext.ConstrainSnapPos(ref pos);
			iconContext.SnapGrid(ref pos);
			this.Handle(1).Position = pos;
			this.dirtyBbox = true;
		}

		// Fin de la création d'un objet.
		public override void CreateMouseUp(Drawing.Point pos, IconContext iconContext)
		{
			if ( this.draftStep == 0 )  // choix taille d'une cellule ?
			{
				Drawing.Point p0 = this.Handle(0).Position;
				Drawing.Point p1 = this.Handle(1).Position;
				this.draftCellWidth  = p1.X-p0.X;
				this.draftCellHeight = p1.Y-p0.Y;
			}

			if ( this.draftStep == 1 )  // choix du nombre de cellules ?
			{
				iconContext.ConstrainSnapPos(ref pos);
				iconContext.SnapGrid(ref pos);
				this.Handle(1).Position = pos;
				iconContext.ConstrainDelStarting();

				this.columns = this.DraftColumns();
				this.rows    = this.DraftRows();
				this.InitWidths();
				this.InitHeights();
				this.InitCells(this.Cell(0,0));

				// Crée les 2 autres poignées dans les coins opposés.
				Drawing.Point pp1 = this.DraftP1();
				Drawing.Rectangle rect = Drawing.Rectangle.FromCorners(this.Handle(0).Position, pp1);
				Drawing.Point p1 = rect.BottomLeft;
				Drawing.Point p2 = rect.TopRight;
				this.Handle(0).Position = p1;
				this.Handle(1).Position = p2;
				this.HandleAdd(new Drawing.Point(p1.X, p2.Y), HandleType.Primary);  // rang = 2
				this.HandleAdd(new Drawing.Point(p2.X, p1.Y), HandleType.Primary);  // rang = 3
				this.HandleAdd(rect.Center, HandleType.Primary);  // rang = 4

				this.UpdateHandle();
			}

			this.draftStep ++;  // passe à la phase suivante
		}

		// Indique si la création de l'objet est terminée.
		public override bool CreateIsEnding(IconContext iconContext)
		{
			if ( this.draftCellWidth == 0 || this.draftCellHeight == 0 )  return true;
			return (this.draftStep >= 2);
		}

		// Indique si l'objet doit exister. Retourne false si l'objet ne peut
		// pas exister et doit être détruit.
		public override bool CreateIsExist(IconContext iconContext)
		{
			Drawing.Rectangle rect = Drawing.Rectangle.FromCorners(this.Handle(0).Position, this.Handle(1).Position);
			return ( rect.Width > this.minimalSize && rect.Height > this.minimalSize );
		}

		// Indique s'il faut sélectionner l'objet après sa création.
		public override bool SelectAfterCreation()
		{
			return true;
		}

		// Indique si un objet est éditable.
		public override bool IsEditable()
		{
			return true;
		}

		
		// Met à jour les poignées pour les largeurs/hauteurs.
		protected void UpdateHandle()
		{
			this.columnStart = this.Handle(2).Position;
			this.columnEnd   = this.Handle(1).Position;
			this.columnWidth = Drawing.Point.Distance(this.columnStart, this.columnEnd);

			this.rowStart    = this.Handle(0).Position;
			this.rowEnd      = this.Handle(2).Position;
			this.rowHeight   = Drawing.Point.Distance(this.rowStart, this.rowEnd);

			int total = 4+1;
			if ( this.columnWidth != 0 )  total += this.columns-1;
			if ( this.rowHeight   != 0 )  total += this.rows-1;

			// Supprime les poignées en trop.
			while ( this.handles.Count > total )
			{
				this.HandleDelete(this.handles.Count-1);
			}

			// Ajoute les poignées manquantes.
			while ( this.handles.Count < total )
			{
				this.HandleAdd(new Drawing.Point(0,0), HandleType.Secondary);
				this.Handle(this.handles.Count-1).IsSelected = true;
			}

			// Positionne la poignée centrale de déplacement global.
			this.Handle(4).Position = (this.Handle(0).Position+this.Handle(1).Position)/2;

			// Positionne les poignées pour les largeurs de colonnes.
			for ( int i=0 ; i<this.columns-1 ; i++ )
			{
				int rank = this.RankColumnHandle(i);
				if ( rank == -1 )  continue;
				this.Handle(rank).Position = Drawing.Point.Move(this.columnStart, this.columnEnd, this.columnWidth*this.widths[i]);
			}

			// Positionne les poignées pour les hauteurs de lignes.
			for ( int i=0 ; i<this.rows-1 ; i++ )
			{
				int rank = this.RankRowHandle(i);
				if ( rank == -1 )  continue;
				this.Handle(rank).Position = Drawing.Point.Move(this.rowStart, this.rowEnd, this.rowHeight*this.heights[i]);
			}

			// Positionne les cellules.
			for ( int c=0 ; c<this.columns+1 ; c++ )
			{
				for ( int r=0 ; r<this.rows+1 ; r++ )
				{
					this.Cell(c,r).BottomLeft  = this.CrossDot(c+0, r+0);
					this.Cell(c,r).BottomRight = this.CrossDot(c+1, r+0);
					this.Cell(c,r).TopLeft     = this.CrossDot(c+0, r+1);
					this.Cell(c,r).TopRight    = this.CrossDot(c+1, r+1);
				}
			}
		}

		// Met à jour les largeurs/hauteurs en fonction des poignées.
		protected void UpdateWidthsHeights()
		{
			this.columnStart = this.Handle(2).Position;
			this.columnEnd   = this.Handle(1).Position;
			this.columnWidth = Drawing.Point.Distance(this.columnStart, this.columnEnd);

			this.rowStart    = this.Handle(0).Position;
			this.rowEnd      = this.Handle(2).Position;
			this.rowHeight   = Drawing.Point.Distance(this.rowStart, this.rowEnd);

			for ( int i=0 ; i<this.columns-1 ; i++ )
			{
				int rank = this.RankColumnHandle(i);
				if ( rank == -1 )  continue;
				Drawing.Point pos = this.Handle(rank).Position;
				this.widths[i] = Drawing.Point.Distance(this.columnStart, pos)/this.columnWidth;
			}

			for ( int i=0 ; i<this.rows-1 ; i++ )
			{
				int rank = this.RankRowHandle(i);
				if ( rank == -1 )  continue;
				Drawing.Point pos = this.Handle(rank).Position;
				this.heights[i] = Drawing.Point.Distance(this.rowStart, pos)/this.rowHeight;
			}
		}

		// Retourne le rang d'une poignée secondaire de colonne.
		protected int RankColumnHandle(int index)
		{
			if ( this.columnWidth == 0 )  return -1;
			return 4+1+index;
		}

		// Retourne le rang d'une poignée secondaire de ligne.
		protected int RankRowHandle(int index)
		{
			if ( this.rowHeight == 0 )  return -1;
			int rank = 4+1;
			if ( this.columnWidth != 0 )  rank += this.columns-1;
			return rank+index;
		}


		// Retourne l'épaisseur de trait maximale.
		protected double MaxWidth()
		{
			double max = 0;
			for ( int c=0 ; c<=this.columns ; c++ )
			{
				for ( int r=0 ; r<=this.rows ; r++ )
				{
					max = System.Math.Max(max, this.Cell(c,r).LeftLine.Width);
					max = System.Math.Max(max, this.Cell(c,r).BottomLine.Width);
				}
			}
			return max;
		}

		// Met à jour le rectangle englobant l'objet.
		protected override void UpdateBoundingBox()
		{
			Drawing.Path path = this.PathBuild();
			this.bboxThin = path.ComputeBounds();

			if ( this.draftStep == 1 )  // choix du nombre de cellules ?
			{
				Drawing.Point p = this.Handle(0).Position;
				p.X += this.draftCellWidth;
				p.Y += this.draftCellHeight;
				this.bboxThin.MergeWith(p);
			}

			this.bboxGeom = this.bboxThin;
			this.bboxGeom.Inflate(this.MaxWidth()*0.5);

			this.bboxFull = this.bboxGeom;
		}


		// Désélectionne toutes les cellules.
		protected void DeselectAllCells()
		{
			this.SelectAllCells(false);
		}

		// Sélectionne toutes les cellules.
		protected void SelectAllCells()
		{
			this.SelectAllCells(true);
		}

		// Sélectionne toutes les cellules.
		protected void SelectAllCells(bool selected)
		{
			for ( int c=0 ; c<=this.columns ; c++ )
			{
				for ( int r=0 ; r<=this.rows ; r++ )
				{
					if ( c == this.columns || r == this.rows )
					{
						this.Cell(c,r).Selected = false;
					}
					else
					{
						this.Cell(c,r).Selected = selected;
					}
				}
			}
		}

		// Indique si une cellule est sélecitonnée.
		protected bool IsSelectCell(int c, int r)
		{
			if ( c < 0 || c >= this.columns )  return false;
			if ( r < 0 || r >= this.rows    )  return false;
			return this.Cell(c,r).Selected;
		}

		// Initialise la table des largeurs.
		protected void InitWidths()
		{
			this.widths = new double[this.columns-1];
			double v = 0;
			for ( int i=0 ; i<this.columns-1 ; i++ )
			{
				v += 1.0/this.columns;
				this.widths[i] = v;
			}
		}

		// Initialise la table des hauteurs.
		protected void InitHeights()
		{
			this.heights = new double[this.rows-1];
			double v = 0;
			for ( int i=0 ; i<this.rows-1 ; i++ )
			{
				v += 1.0/this.rows;
				this.heights[i] = v;
			}
		}

		// Initialise les cellules.
		protected void InitCells()
		{
			this.cells.Clear();

			for ( int c=0 ; c<this.columns+1 ; c++ )
			{
				for ( int r=0 ; r<this.rows+1 ; r++ )
				{
					this.cells.Add(this.NewCell());
				}
			}
		}

		// Initialise les cellules d'après une cellule modèle.
		protected void InitCells(ArrayCell model)
		{
			this.cells.Clear();

			for ( int c=0 ; c<this.columns+1 ; c++ )
			{
				for ( int r=0 ; r<this.rows+1 ; r++ )
				{
					this.cells.Add(this.NewCell(model));
				}
			}
		}

		// Initialise une nouvelle cellule.
		protected ArrayCell NewCell()
		{
			ArrayCell cell = new ArrayCell();
			cell.Selected = false;

			cell.LeftLine = new PropertyLine();
			this.PropertyLine(0).CopyTo(cell.LeftLine);

			cell.BottomLine = new PropertyLine();
			this.PropertyLine(0).CopyTo(cell.BottomLine);

			cell.LeftColor = new PropertyColor();
			this.PropertyColor(1).CopyTo(cell.LeftColor);

			cell.BottomColor = new PropertyColor();
			this.PropertyColor(1).CopyTo(cell.BottomColor);

			cell.BackColor = new PropertyColor();
			this.PropertyColor(2).CopyTo(cell.BackColor);

			cell.TextString = new PropertyString();
			this.PropertyString(3).CopyTo(cell.TextString);

			cell.TextFont = new PropertyFont();
			this.PropertyFont(4).CopyTo(cell.TextFont);

			cell.TextJustif = new PropertyJustif();
			this.PropertyJustif(5).CopyTo(cell.TextJustif);

			return cell;
		}

		// Initialise une nouvelle cellule d'après une cellule modèle.
		protected ArrayCell NewCell(ArrayCell model)
		{
			ArrayCell cell = new ArrayCell();
			cell.Selected = false;

			cell.LeftLine = new PropertyLine();
			model.LeftLine.CopyTo(cell.LeftLine);

			cell.BottomLine = new PropertyLine();
			model.BottomLine.CopyTo(cell.BottomLine);

			cell.LeftColor = new PropertyColor();
			model.LeftColor.CopyTo(cell.LeftColor);

			cell.BottomColor = new PropertyColor();
			model.BottomColor.CopyTo(cell.BottomColor);

			cell.BackColor = new PropertyColor();
			model.BackColor.CopyTo(cell.BackColor);

			cell.TextString = new PropertyString();
			model.TextString.CopyTo(cell.TextString);

			cell.TextFont = new PropertyFont();
			model.TextFont.CopyTo(cell.TextFont);

			cell.TextJustif = new PropertyJustif();
			model.TextJustif.CopyTo(cell.TextJustif);

			return cell;
		}

		
		// Reprend toutes les caractéristiques d'un objet.
		public override void CloneObject(AbstractObject src)
		{
			base.CloneObject(src);

			ObjectArray array = src as ObjectArray;
			this.columns = array.columns;
			this.rows    = array.rows;

			this.InitWidths();
			for ( int c=0 ; c<this.columns-1 ; c++ )
			{
				this.widths[c] = array.widths[c];
			}

			this.InitHeights();
			for ( int r=0 ; r<this.rows-1 ; r++ )
			{
				this.heights[r] = array.heights[r];
			}

			this.InitCells();
			for ( int c=0 ; c<this.columns+1 ; c++ )
			{
				for ( int r=0 ; r<this.rows+1 ; r++ )
				{
					array.Cell(c,r).CopyTo(this.Cell(c,r));
				}
			}

			this.UpdateHandle();
		}


		// Calcule le nombre de colonnes pendant la création.
		protected int DraftColumns()
		{
			Drawing.Point p0 = this.Handle(0).Position;
			Drawing.Point p1 = this.Handle(1).Position;
			int total = (int)((p1.X-p0.X)/this.draftCellWidth);
			if ( total < 1 )  total = 1;
			if ( total > this.maxColumns )  total = this.maxColumns;
			return total;
		}

		// Calcule le nombre de lignes pendant la création.
		protected int DraftRows()
		{
			Drawing.Point p0 = this.Handle(0).Position;
			Drawing.Point p1 = this.Handle(1).Position;
			int total = (int)((p1.Y-p0.Y)/this.draftCellHeight);
			if ( total < 1 )  total = 1;
			if ( total > this.maxRows )  total = this.maxRows;
			return total;
		}

		// Calcule le point p1' pendant la création.
		protected Drawing.Point DraftP1()
		{
			Drawing.Point p0 = this.Handle(0).Position;
			Drawing.Point p1 = this.Handle(1).Position;
			Drawing.Point p = p0;
			p.X += this.draftCellWidth*this.DraftColumns();
			p.Y += this.draftCellHeight*this.DraftRows();
			return p;
		}

		// Dessine les dimensions du tableau.
		protected void DrawDraftDim(Drawing.Graphics graphics, IconContext iconContext)
		{
			Drawing.Point p0 = this.Handle(0).Position;
			Drawing.Point p1 = new Drawing.Point(p0.X+this.draftCellWidth, p0.Y+this.draftCellHeight);
			string text = string.Format(" {0} x {1} ", this.DraftColumns(), this.DraftRows());
			Drawing.Font font = Drawing.Font.GetFont("Tahoma", "Regular");
			double ta = font.GetTextAdvance(text);
			double fa = font.Ascender;
			double rapport = ta/fa;
			double w = System.Math.Abs(this.draftCellWidth);
			double h = System.Math.Abs(this.draftCellHeight);
			w = System.Math.Min(w, h*rapport*0.7);
			double fs = w/ta;
			if ( p1.X < p0.X )  p0.X -= ta*fs;
			p0.Y += (h-fa*fs*0.8)/2;
			if ( p1.Y < p0.Y )  p0.Y -= h;
			graphics.AddText(p0.X, p0.Y, text, font, fs);
			graphics.RenderSolid(Drawing.Color.FromBrightness(0));
		}

		// Dessine un brouillon du tableau (pendant la création).
		protected void DrawDraft(Drawing.Graphics graphics, IconContext iconContext)
		{
			ArrayCell cell = this.Cell(0,0);  // prend la 1ère cellule comme attributs

			double initialWidth = graphics.LineWidth;

			if ( this.draftStep == 0 )  // choix taille d'une cellule ?
			{
				Drawing.Path path = this.PathBuild();

				graphics.Rasterizer.AddSurface(path);
				graphics.RenderSolid(cell.BackColor.Color);

				graphics.Rasterizer.AddOutline(path, cell.LeftLine.Width);
				graphics.RenderSolid(cell.LeftColor.Color);
			}

			if ( this.draftStep == 1 )  // choix du nombre de cellules ?
			{
				Drawing.Point p0 = this.Handle(0).Position;
				Drawing.Point p1 = this.Handle(1).Position;
				Drawing.Point pp1 = this.DraftP1();
				int nbColumns = this.DraftColumns();
				int nbRows    = this.DraftRows();

				Drawing.Rectangle rect = Drawing.Rectangle.FromCorners(p0, p1);
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(iconContext.HiliteOutlineColor);

				graphics.LineWidth = cell.LeftLine.Width;

				rect = Drawing.Rectangle.FromCorners(p0, pp1);
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(cell.BackColor.Color);
				graphics.AddRectangle(rect);

				double x = p0.X;
				for ( int i=0 ; i<nbColumns-1 ; i++ )
				{
					x += this.draftCellWidth;
					graphics.AddLine(x, rect.Bottom, x, rect.Top);
				}

				double y = p0.Y;
				for ( int i=0 ; i<nbRows-1 ; i++ )
				{
					y += this.draftCellHeight;
					graphics.AddLine(rect.Left, y, rect.Right, y);
				}

				graphics.RenderSolid(cell.LeftColor.Color);

				this.DrawDraftDim(graphics, iconContext);
			}

			graphics.LineWidth = initialWidth;
		}


		// Calcule le point à l'intersection d'une ligne et d'une colonne.
		// c: [0..this.columns+1]
		// r: [0..this.rows+1]
		protected Drawing.Point CrossDot(int c, int r)
		{
			Drawing.Point p0,p1,p2,p3, p03,p21, p;

			p0 = this.Handle(0).Position;
			p1 = this.Handle(1).Position;

			if ( this.handles.Count < 4 )
			{
				p2 = new Drawing.Point(p0.X, p1.Y);
				p3 = new Drawing.Point(p1.X, p0.Y);
			}
			else
			{
				p2 = this.Handle(2).Position;
				p3 = this.Handle(3).Position;
			}

			double d03 = Drawing.Point.Distance(p0, p3);
			if ( c == 0 || d03 == 0 )
			{
				p03 = p0;
			}
			else if ( c < this.columns )
			{
				p03 = Drawing.Point.Move(p0, p3, this.widths[c-1]*d03);
			}
			else
			{
				p03 = p3;
			}

			double d21 = Drawing.Point.Distance(p2, p1);
			if ( c == 0 || d21 == 0 )
			{
				p21 = p2;
			}
			else if ( c < this.columns )
			{
				p21 = Drawing.Point.Move(p2, p1, this.widths[c-1]*d21);
			}
			else
			{
				p21 = p1;
			}

			double d = Drawing.Point.Distance(p03, p21);
			if ( r == 0 || d == 0 )
			{
				p = p03;
			}
			else if ( r < this.rows )
			{
				p = Drawing.Point.Move(p03, p21, this.heights[r-1]*d);
			}
			else
			{
				p = p21;
			}
			return p;
		}

		// Crée le chemin de l'objet.
		protected Drawing.Path PathBuild()
		{
			Drawing.Point p1 = this.Handle(0).Position;
			Drawing.Point p2 = new Drawing.Point();
			Drawing.Point p3 = this.Handle(1).Position;
			Drawing.Point p4 = new Drawing.Point();

			if ( this.handles.Count < 4 )
			{
				p2.X = p1.X;
				p2.Y = p3.Y;
				p4.X = p3.X;
				p4.Y = p1.Y;
			}
			else
			{
				p2 = this.Handle(2).Position;
				p4 = this.Handle(3).Position;
			}

			Drawing.Path path = new Drawing.Path();
			path.MoveTo(p1);
			path.LineTo(p2);
			path.LineTo(p3);
			path.LineTo(p4);
			path.Close();
			return path;
		}

		// Retourne la fonte à utiliser.
		protected Drawing.Font GetFont(int c, int r)
		{
			return this.Cell(c,r).TextFont.GetFont();
		}

		// Retourne les 4 coins d'une cellule en fonction de son orientation.
		protected void CellCorners(int c, int r, out Drawing.Point pbl, out Drawing.Point pbr, out Drawing.Point ptl, out Drawing.Point ptr)
		{
			switch ( this.Cell(c,r).TextJustif.Orientation )
			{
				case JustifOrientation.RightToLeft:  // <-
					pbl = this.Cell(c,r).TopRight;
					pbr = this.Cell(c,r).TopLeft;
					ptl = this.Cell(c,r).BottomRight;
					ptr = this.Cell(c,r).BottomLeft;
					break;
				case JustifOrientation.BottomToTop:  // ^
					pbl = this.Cell(c,r).BottomRight;
					pbr = this.Cell(c,r).TopRight;
					ptl = this.Cell(c,r).BottomLeft;
					ptr = this.Cell(c,r).TopLeft;
					break;
				case JustifOrientation.TopToBottom:  // v
					pbl = this.Cell(c,r).TopLeft;
					pbr = this.Cell(c,r).BottomLeft;
					ptl = this.Cell(c,r).TopRight;
					ptr = this.Cell(c,r).BottomRight;
					break;
				default:							// -> (normal)
					pbl = this.Cell(c,r).BottomLeft;
					pbr = this.Cell(c,r).BottomRight;
					ptl = this.Cell(c,r).TopLeft;
					ptr = this.Cell(c,r).TopRight;
					break;
			}
		}

		// Dessine le texte d'une cellule.
		protected void DrawCellText(Drawing.Graphics graphics, IconContext iconContext, int c, int r)
		{
			string text = this.Cell(c,r).TextString.String;
			if ( text == "" )  return;

			if ( this.Cell(c,r).TextLayout == null )
			{
				this.Cell(c,r).TextLayout = new TextLayout();
			}
			TextLayout textLayout = this.Cell(c,r).TextLayout;
			textLayout.Text = text;

			Drawing.Point p1, p2, p3, p4;
			this.CellCorners(c,r, out p1, out p2, out p3, out p4);
			if ( !this.Cell(c,r).TextJustif.DeflateBox(ref p1, ref p2, ref p3, ref p4) )  return;

			Drawing.Size size = new Drawing.Size();
			size.Width  = Drawing.Point.Distance(p1,p2);
			size.Height = Drawing.Point.Distance(p1,p3);
			textLayout.LayoutSize = size;

			textLayout.Font = this.GetFont(c,r);
			textLayout.FontSize = this.Cell(c,r).TextFont.FontSize;

			JustifVertical   jv = this.Cell(c,r).TextJustif.Vertical;
			JustifHorizontal jh = this.Cell(c,r).TextJustif.Horizontal;

			if ( jv == JustifVertical.Top )
			{
				if ( jh == JustifHorizontal.Left   )  textLayout.Alignment = Drawing.ContentAlignment.TopLeft;
				if ( jh == JustifHorizontal.Center )  textLayout.Alignment = Drawing.ContentAlignment.TopCenter;
				if ( jh == JustifHorizontal.Right  )  textLayout.Alignment = Drawing.ContentAlignment.TopRight;
			}
			if ( jv == JustifVertical.Center )
			{
				if ( jh == JustifHorizontal.Left   )  textLayout.Alignment = Drawing.ContentAlignment.MiddleLeft;
				if ( jh == JustifHorizontal.Center )  textLayout.Alignment = Drawing.ContentAlignment.MiddleCenter;
				if ( jh == JustifHorizontal.Right  )  textLayout.Alignment = Drawing.ContentAlignment.MiddleRight;
			}
			if ( jv == JustifVertical.Bottom )
			{
				if ( jh == JustifHorizontal.Left   )  textLayout.Alignment = Drawing.ContentAlignment.BottomLeft;
				if ( jh == JustifHorizontal.Center )  textLayout.Alignment = Drawing.ContentAlignment.BottomCenter;
				if ( jh == JustifHorizontal.Right  )  textLayout.Alignment = Drawing.ContentAlignment.BottomRight;
			}

			Drawing.Transform ot = graphics.SaveTransform();

			double angle = Drawing.Point.ComputeAngle(p1, p2);
			angle *= 180.0/System.Math.PI;  // radians -> degrés
			graphics.RotateTransform(angle, p1.X, p1.Y);

			Drawing.Color color = iconContext.AdaptColor(this.Cell(c,r).TextFont.FontColor);
			textLayout.Paint(p1, graphics, Drawing.Rectangle.Empty, color, Drawing.GlyphPaintStyle.Normal);

			graphics.Transform = ot;
		}

		// Dessine le fond d'une cellule.
		protected void DrawCellSurface(Drawing.Graphics graphics, IconContext iconContext, int c, int r)
		{
			Drawing.Path path = new Drawing.Path();
			path.MoveTo(this.Cell(c,r).BottomLeft);
			path.LineTo(this.Cell(c,r).TopLeft);
			path.LineTo(this.Cell(c,r).TopRight);
			path.LineTo(this.Cell(c,r).BottomRight);
			path.Close();
			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(iconContext.AdaptColor(this.Cell(c,r).BackColor.Color));
		}

		// Dessine les traits d'une cellule.
		protected void DrawCellOutline(Drawing.Graphics graphics, IconContext iconContext, int c, int r)
		{
			PropertyLine line;
			PropertyColor color;

			Drawing.Path path = new Drawing.Path();
			path.MoveTo(this.Cell(c,r).BottomLeft);
			path.LineTo(this.Cell(c,r).BottomRight);
			line = this.Cell(c,r).BottomLine;
			graphics.Rasterizer.AddOutline(path, line.Width, line.Cap, line.Join, line.Limit);
			color = this.Cell(c,r).BottomColor;
			graphics.RenderSolid(iconContext.AdaptColor(color.Color));

			path = new Drawing.Path();
			path.MoveTo(this.Cell(c,r).BottomLeft);
			path.LineTo(this.Cell(c,r).TopLeft);
			line = this.Cell(c,r).LeftLine;
			graphics.Rasterizer.AddOutline(path, line.Width, line.Cap, line.Join, line.Limit);
			color = this.Cell(c,r).LeftColor;
			graphics.RenderSolid(iconContext.AdaptColor(color.Color));
		}

		// Dessine la mise en évidence d'une cellule.
		protected void DrawCellHilite(Drawing.Graphics graphics, IconContext iconContext, int c, int r)
		{
			Drawing.Point pbl, pbr, ptl, ptr;
			pbl = this.Cell(c,r).BottomLeft;
			pbr = this.Cell(c,r).BottomRight;
			ptl = this.Cell(c,r).TopLeft;
			ptr = this.Cell(c,r).TopRight;

			Drawing.Path path = new Drawing.Path();
			path.MoveTo(pbl);
			path.LineTo(ptl);
			path.LineTo(ptr);
			path.LineTo(pbr);
			path.Close();
			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(Drawing.Color.FromColor(iconContext.HiliteOutlineColor, 0.3));

			double initialWidth = graphics.LineWidth;
			Drawing.CapStyle initialCap = graphics.LineCap;
			graphics.LineWidth = 6.0/iconContext.ScaleX;
			graphics.LineCap = Drawing.CapStyle.Square;
			if ( this.PropertyOutline(c,r, OutlinePos.Left  ) )  graphics.AddLine(pbl, ptl);
			if ( this.PropertyOutline(c,r, OutlinePos.Right ) )  graphics.AddLine(pbr, ptr);
			if ( this.PropertyOutline(c,r, OutlinePos.Bottom) )  graphics.AddLine(pbl, pbr);
			if ( this.PropertyOutline(c,r, OutlinePos.Top   ) )  graphics.AddLine(ptl, ptr);
			graphics.RenderSolid(iconContext.HiliteOutlineColor);
			graphics.LineWidth = initialWidth;
			graphics.LineCap = initialCap;
		}

		// Dessine la cellule en édition.
		protected void DrawCellEdit(Drawing.Graphics graphics, IconContext iconContext, int c, int r)
		{
			Drawing.Point pbl, pbr, ptl, ptr;
			pbl = this.Cell(c,r).BottomLeft;
			pbr = this.Cell(c,r).BottomRight;
			ptl = this.Cell(c,r).TopLeft;
			ptr = this.Cell(c,r).TopRight;

			Drawing.Path path = new Drawing.Path();
			path.MoveTo(pbl);
			path.LineTo(ptl);
			path.LineTo(ptr);
			path.LineTo(pbr);
			path.Close();
			graphics.Rasterizer.AddOutline(path, 2.0/iconContext.ScaleX);
			graphics.RenderSolid(IconContext.ColorFrameEdit);
		}

		// Dessine l'objet.
		public override void DrawGeometry(Drawing.Graphics graphics, IconContext iconContext)
		{
			if ( base.IsFullHide(iconContext) )  return;
			base.DrawGeometry(graphics, iconContext);

			if ( this.TotalHandle < 2 )  return;

			if ( this.TotalHandle <= 2 )  // en construction ?
			{
				this.DrawDraft(graphics, iconContext);
				return;
			}

			// Dessine tous fonds de cellules.
			for ( int c=0 ; c<this.columns ; c++ )
			{
				for ( int r=0 ; r<this.rows ; r++ )
				{
					this.DrawCellSurface(graphics, iconContext, c,r);
				}
			}

			// Dessine tous les textes des cellules.
			for ( int c=0 ; c<this.columns ; c++ )
			{
				for ( int r=0 ; r<this.rows ; r++ )
				{
					this.DrawCellText(graphics, iconContext, c,r);
				}
			}

			// Dessine tous les traits des cellules.
			for ( int c=0 ; c<this.columns+1 ; c++ )
			{
				for ( int r=0 ; r<this.rows+1 ; r++ )
				{
					this.DrawCellOutline(graphics, iconContext, c,r);
				}
			}

			if ( !iconContext.IsEditable )  return;

			if ( this.edited && this.cellToEdit != -1 )  // en cours d'édition ?
			{
				int c = this.cellToEdit%(this.columns+1);
				int r = this.cellToEdit/(this.columns+1);
				this.DrawCellEdit(graphics, iconContext, c,r);
			}

			if ( this.cellToHilite != -1 && this.cellToHilite != this.cellToEdit )
			{
				int c = this.cellToHilite%(this.columns+1);
				int r = this.cellToHilite/(this.columns+1);
				this.DrawCellHilite(graphics, iconContext, c,r);
			}

			if ( !this.edited )
			{
				if ( this.selected )  // tableau sélectionné ?
				{
					// Dessine les cellules en évidence.
					for ( int c=0 ; c<this.columns ; c++ )
					{
						for ( int r=0 ; r<this.rows ; r++ )
						{
							if ( !this.Cell(c,r).Selected )  continue;
							this.DrawCellHilite(graphics, iconContext, c,r);
						}
					}
				}
				else	// tableau non sélectionné ?
				{
					// Dessine la mise en évidence complète.
					if ( this.IsHilite )
					{
						Drawing.Path path = this.PathBuild();
						graphics.Rasterizer.AddSurface(path);
						graphics.RenderSolid(iconContext.HiliteSurfaceColor);
					}
				}
			}
		}


		// Arrange un objet après sa désérialisation. Il faut supprimer les
		// premières cellules crées dans le constructeur, pour laisser celles
		// qui ont été désérialisées prendre leurs places.
		public override void ArrangeAfterRead()
		{
			base.ArrangeAfterRead();

			int total = (this.columns+1) * (this.rows+1);
			for ( int i=0 ; i<total ; i++ )
			{
				this.cells.RemoveAt(0);
			}
			this.UpdateWidthsHeights();
			this.UpdateHandle();
		}


		protected int							columns;
		protected int							rows;
		protected double[]						widths;
		protected double[]						heights;
		protected System.Collections.ArrayList	cells = new System.Collections.ArrayList();

		protected Drawing.Point					columnStart;
		protected Drawing.Point					columnEnd;
		protected double						columnWidth;
		protected Drawing.Point					rowStart;
		protected Drawing.Point					rowEnd;
		protected double						rowHeight;

		protected int							draftStep;
		protected double						draftCellWidth;
		protected double						draftCellHeight;
		protected int							startColumn;
		protected int							startRow;
		protected bool							outlineFrame = true;
		protected bool							outlineHoriz = true;
		protected bool							outlineVerti = true;
		protected bool							onlyBase = false;
		protected int							cellToHilite = -1;
		protected int							cellToEdit = -1;

		protected int							maxColumns = 10;
		protected int							maxRows    = 10;
	}
}

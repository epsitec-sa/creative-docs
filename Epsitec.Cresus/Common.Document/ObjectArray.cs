using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document
{
	/// <summary>
	/// La classe ObjectArray est la classe de l'objet graphique "tableau".
	/// </summary>
	[System.Serializable()]
	public class ObjectArray : AbstractObject
	{
		protected enum OutlinePos
		{
			Left,
			Right,
			Bottom,
			Top,
		}


		public ObjectArray(Document document, AbstractObject model) : base(document, model)
		{
			if ( this.document == null )  return;  // objet factice ?
			this.CreateProperties(model, false);

			this.columns = 1;
			this.rows    = 1;
			this.InitWidths();
			this.InitHeights();
			this.InitCells();

			this.draftStep = 0;  // choix taille d'une cellule
		}

		protected override bool ExistingProperty(PropertyType type)
		{
			if ( type == PropertyType.Name )  return true;
			if ( type == PropertyType.LineMode )  return true;
			if ( type == PropertyType.LineColor )  return true;
			if ( type == PropertyType.BackColor )  return true;
			if ( type == PropertyType.TextJustif )  return true;
			if ( type == PropertyType.TextFont )  return true;
			return false;
		}

		protected override AbstractObject CreateNewObject(Document document, AbstractObject model)
		{
			return new ObjectArray(document, model);
		}

		public override void Dispose()
		{
			base.Dispose();
		}


		// Nom de l'ic�ne.
		public override string IconName
		{
			get { return @"file:images/array.icon"; }
		}


		// Nombre de colonnes.
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

		public System.Collections.ArrayList Cells
		{
			get { return this.cells; }
			set { this.cells = value; }
		}

		// Acc�de � une cellule du tableau.
		protected ArrayCell Cell(int c, int r)
		{
			System.Diagnostics.Debug.Assert(c >= 0 && c <= this.columns);
			System.Diagnostics.Debug.Assert(r >= 0 && r <= this.rows);
			return this.cells[c+r*(this.columns+1)] as ArrayCell;
		}


		// S�lectionne ou d�s�lectionne toutes les poign�es de l'objet.
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


#if false
		// Retourne une copie d'une propri�t�.
		public override AbstractProperty GetProperty(PropertyType type)
		{
			if ( type == PropertyType.Name     ||
				 type == PropertyType.TextFont )
			{
				return base.GetProperty(type);
			}

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
			AbstractProperty copy = AbstractProperty.NewProperty(this.document, type);
			if ( copy == null )  return null;
			property.CopyTo(copy);
			return copy;
		}
#endif

#if false
		// Modifie une propri�t�.
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
#endif

		// Indique si une cellule est s�lectionn�e, soit si elle est directement
		// s�lectionn�e, ou si elle utilise le m�me style que la propri�t� modifi�e.
		protected bool PropertySelected(AbstractProperty property, int c, int r, OutlinePos pos)
		{
			if ( this.selected )
			{
				if ( this.Cell(c,r).Selected && this.PropertyOutline(c,r, pos) )  return true;
				if ( !property.IsStyle )  return false;
			}
			else
			{
				if ( !property.IsStyle )  return true;
			}

			AbstractProperty actual = this.GetProperty(property.Type, c,r, pos);
			if ( actual == null )  return false;
			return ( actual == property );
		}

		protected bool PropertySelected(AbstractProperty property, int c, int r)
		{
			if ( this.selected )
			{
				if ( this.Cell(c,r).Selected )  return true;
				if ( !property.IsStyle )  return false;
			}
			else
			{
				if ( !property.IsStyle )  return true;
			}

			AbstractProperty actual = this.Cell(c,r).Property(property.Type, 0);
			if ( actual == null )  return false;
			return ( actual == property );
		}

		// Regarde si un trait autour de la cellule fait partie de la s�lection.
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

		// Donne une propri�t� d'un trait autour de la cellule, si elle est s�lectionn�e.
		protected AbstractProperty GetPropertySel(PropertyType type, int c, int r, OutlinePos pos)
		{
			if ( !this.PropertyOutline(c,r, pos) )  return null;
			return this.GetProperty(type, c,r, pos);
		}

		// Donne une propri�t� d'un trait autour de la cellule.
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

		// Modifie une propri�t� d'un trait autour de la cellule.
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


		// D�tecte si la souris est sur l'objet.
		public override bool Detect(Point pos)
		{
			if ( this.isHide )  return false;

			Rectangle bbox = this.BoundingBox;
			if ( !bbox.Contains(pos) )  return false;

			Path path = this.PathBuild();
			path.Close();
			return Geometry.DetectSurface(path, pos);
		}

		// D�tecte si la souris est sur l'objet pour l'�diter.
		public override bool DetectEdit(Point pos)
		{
			if ( this.isHide )  return false;

			this.cellToHilite = this.DetectCell(pos);
			return (this.cellToHilite != -1);
		}


		// D�place une poign�e.
		public override void MoveHandleProcess(int rank, Point pos, DrawingContext drawingContext)
		{
			if ( rank >= this.handles.Count )  // poign�e d'une propri�t� ?
			{
				base.MoveHandleProcess(rank, pos, drawingContext);
				return;
			}

			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.ConstrainSnapPos(ref pos);
			drawingContext.SnapGrid(ref pos);

			if ( rank < 4 )
			{
				if ( Geometry.IsRectangular(this.Handle(0).Position, this.Handle(1).Position, this.Handle(2).Position, this.Handle(3).Position) )
				{
					this.Handle(rank).Position = pos;

					if ( rank == 0 )
					{
						this.Handle(2).Position = Point.Projection(this.Handle(2).Position, this.Handle(1).Position, pos);
						this.Handle(3).Position = Point.Projection(this.Handle(3).Position, this.Handle(1).Position, pos);
					}
					if ( rank == 1 )
					{
						this.Handle(2).Position = Point.Projection(this.Handle(2).Position, this.Handle(0).Position, pos);
						this.Handle(3).Position = Point.Projection(this.Handle(3).Position, this.Handle(0).Position, pos);
					}
					if ( rank == 2 )
					{
						this.Handle(0).Position = Point.Projection(this.Handle(0).Position, this.Handle(3).Position, pos);
						this.Handle(1).Position = Point.Projection(this.Handle(1).Position, this.Handle(3).Position, pos);
					}
					if ( rank == 3 )
					{
						this.Handle(0).Position = Point.Projection(this.Handle(0).Position, this.Handle(2).Position, pos);
						this.Handle(1).Position = Point.Projection(this.Handle(1).Position, this.Handle(2).Position, pos);
					}
				}
				else
				{
					this.Handle(rank).Position = pos;
				}
			}
			else if ( rank == 4 )  // d�place tout l'objet ?
			{
				Point move = pos-this.Handle(4).Position;
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
						double dist = Point.Distance(this.columnStart, pos)/this.columnWidth;
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
						double dist = Point.Distance(this.rowStart, pos)/this.rowHeight;
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
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// D�place tout l'objet.
		public override void MoveAllProcess(Point move)
		{
			base.MoveAllProcess(move);
			this.UpdateHandle();
		}


		// D�place globalement l'objet.
		public override void MoveGlobalProcess(SelectorData initial, SelectorData final)
		{
			base.MoveGlobalProcess(initial, final);
			this.UpdateHandle();
		}


		// Retourne les noms des assistants.
		public static bool CommandLook(int rank, out string text, out string name)
		{
			switch ( rank )
			{
				case  0:  text = "Num�ros";              name = "Num";      return true;
				case  1:  text = "Lettres";              name = "Letter";   return true;
				case  2:  text = "Jours de la semaine";  name = "Week";     return true;
				case  3:  text = "Mois";                 name = "Month";    return true;
				case  4:  text = "";                     name = "";         return true;
				case  5:  text = "Altern� blanc/gris";   name = "Alter1";   return true;
				case  6:  text = "Altern� bleu/jaune";   name = "Alter2";   return true;
				case  7:  text = "";                     name = "";         return true;
				case  8:  text = "En-t�te claire";       name = "Header1";  return true;
				case  9:  text = "En-t�te fonc�e";       name = "Header2";  return true;
			}
			text = "";
			name = "";
			return false;
		}

		// Indique l'�tat des commandes.
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

		// Ex�cute une commande.
		public void ExecuteCommand(string cmd, string arg)
		{
			int c1,r1, c2,r2;
			this.RetSelectedRect(out c1, out r1, out c2, out r2);

			int c = this.cellToEdit%(this.columns+1);
			int r = this.cellToEdit/(this.columns+1);

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

			c = System.Math.Min(c, this.columns-1);
			r = System.Math.Min(r, this.rows-1);
			this.cellToEdit = c + r*(this.columns+1);
		}

		// Ins�re des colonnes avant la colonne sp�cifi�e.
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
					if ( !duplicate )  newCell.Content = "";
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

		// Ins�re des colonnes apr�s la colonne sp�cifi�e.
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
					if ( !duplicate )  newCell.Content = "";
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

		// Ins�re des lignes avant la ligne sp�cifi�e.
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
					if ( !duplicate )  newCell.Content = "";
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

		// Ins�re des lignes apr�s la ligne sp�cifi�e.
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
					if ( !duplicate )  newCell.Content = "";
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
					string text = this.Cell(c,r).Content;
					this.Cell(c,r).Content = this.Cell(cc,r).Content;
					this.Cell(cc,r).Content = text;
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
					string text = this.Cell(c,r).Content;
					this.Cell(c,r).Content = this.Cell(c,rr).Content;
					this.Cell(c,rr).Content = text;
				}
				rr --;
			}
			this.UpdateHandle();
		}

		// Met le look � la s�lection.
		protected void LookHeader1()
		{
			int c1,r1, c2,r2;
			if ( !this.RetSelectedRect(out c1, out r1, out c2, out r2) )  return;

			for ( int c=c1 ; c<=c2 ; c++ )
			{
				for ( int r=r1 ; r<=r2 ; r++ )
				{
					Color color;
					if ( c == c1 || r == r2 )
					{
						color = Color.FromBrightness(0.7);
					}
					else
					{
						color = Color.FromBrightness(1.0);
					}
					this.Cell(c,r).BackColor.Color = color;

					//?this.Cell(c,r).TextFont.FontColor = Color.FromBrightness(0.0);
				}
			}
		}

		// Met le look � la s�lection.
		protected void LookHeader2()
		{
			int c1,r1, c2,r2;
			if ( !this.RetSelectedRect(out c1, out r1, out c2, out r2) )  return;

			for ( int c=c1 ; c<=c2 ; c++ )
			{
				for ( int r=r1 ; r<=r2 ; r++ )
				{
					Color color;
					if ( c == c1 || r == r2 )
					{
						color = Color.FromBrightness(0.3);
					}
					else
					{
						color = Color.FromBrightness(1.0);
					}
					this.Cell(c,r).BackColor.Color = color;

					if ( c == c1 || r == r2 )
					{
						color = Color.FromBrightness(1.0);
					}
					else
					{
						color = Color.FromBrightness(0.0);
					}
					//?this.Cell(c,r).TextFont.FontColor = color;
				}
			}
		}

		// Met le look � la s�lection.
		protected void LookAlter1()
		{
			int c1,r1, c2,r2;
			if ( !this.RetSelectedRect(out c1, out r1, out c2, out r2) )  return;

			for ( int c=c1 ; c<=c2 ; c++ )
			{
				for ( int r=r1 ; r<=r2 ; r++ )
				{
					Color color;
					if ( r%2 ==0 )  color = Color.FromBrightness(1.0);
					else            color = Color.FromBrightness(0.9);
					this.Cell(c,r).BackColor.Color = color;

					//?this.Cell(c,r).TextFont.FontColor = Color.FromBrightness(0.0);
				}
			}
		}

		// Met le look � la s�lection.
		protected void LookAlter2()
		{
			int c1,r1, c2,r2;
			if ( !this.RetSelectedRect(out c1, out r1, out c2, out r2) )  return;

			for ( int c=c1 ; c<=c2 ; c++ )
			{
				for ( int r=r1 ; r<=r2 ; r++ )
				{
					Color color;
					if ( r%2 ==0 )  color = Color.FromRGB(0.9, 1.0, 1.0);  // bleut�
					else            color = Color.FromRGB(1.0, 1.0, 0.9);  // jaune
					this.Cell(c,r).BackColor.Color = color;

					//?this.Cell(c,r).TextFont.FontColor = Color.FromBrightness(0.0);
				}
			}
		}

		// Retourne la cha�ne d'une s�rie.
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
					if ( rank ==  1 )  return "F�vrier";
					if ( rank ==  2 )  return "Mars";
					if ( rank ==  3 )  return "Avril";
					if ( rank ==  4 )  return "Mai";
					if ( rank ==  5 )  return "Juin";
					if ( rank ==  6 )  return "Juillet";
					if ( rank ==  7 )  return "Ao�t";
					if ( rank ==  8 )  return "Septembre";
					if ( rank ==  9 )  return "Octobre";
					if ( rank == 10 )  return "Novembre";
					if ( rank == 11 )  return "D�cembre";
					break;
			}
			return "";
		}

		// Met le look � la s�lection.
		protected void LookSerie(string arg)
		{
			int c1,r1, c2,r2;
			if ( !this.RetSelectedRect(out c1, out r1, out c2, out r2) )  return;

			int rank = 0;
			for ( int c=c1 ; c<=c2 ; c++ )
			{
				for ( int r=r2 ; r>=r1 ; r-- )  // de haut en bas !
				{
					this.Cell(c,r).Content = ObjectArray.Serie(arg, rank++);
				}
			}
		}

		// Indique si la zone s�lectionn�e est rectangulaire.
		protected bool RetSelectedRect()
		{
			int c1,r1, c2,r2;
			return this.RetSelectedRect(out c1, out r1, out c2, out r2);
		}

		// Retourne la zone rectangulaire s�lectionn�e.
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


		// D�tecte la cellule point�e par la souris.
		public override int DetectCell(Point pos)
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

		// D�but du d�placement d'une cellule.
		public override void MoveCellStarting(int rank, Point pos,
											  bool isShift, bool isCtrl, int downCount,
											  DrawingContext drawingContext)
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
						this.Cell(c,r).Selected = true;  // s�lectionne toute la ligne
					}
				}
				else
				{
					for ( r=0 ; r<this.rows ; r++ )
					{
						this.Cell(c,r).Selected = true;  // s�lectionne toute la colonne
					}
				}
			}
			else	// triple clic ?
			{
				this.SelectAllCells();  // s�lectionne tout le tableau
			}
		}

		// D�place une cellule.
		public override void MoveCellProcess(int rank, Point pos, DrawingContext drawingContext)
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


		// D�but de la cr�ation d'un objet.
		public override void CreateMouseDown(Point pos, DrawingContext drawingContext)
		{
			if ( this.draftStep == 0 )  // choix taille d'une cellule ?
			{
				drawingContext.ConstrainFixStarting(pos);
				drawingContext.ConstrainFixType(ConstrainType.Square);
				this.HandleAdd(pos, HandleType.Primary);  // rang = 0
				this.HandleAdd(pos, HandleType.Primary);  // rang = 1
				this.document.Notifier.NotifyArea(this.BoundingBox);
			}
		}

		// D�placement pendant la cr�ation d'un objet.
		public override void CreateMouseMove(Point pos, DrawingContext drawingContext)
		{
			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapGrid(ref pos);
			drawingContext.ConstrainSnapPos(ref pos);
			this.Handle(1).Position = pos;
			this.dirtyBbox = true;
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// Fin de la cr�ation d'un objet.
		public override void CreateMouseUp(Point pos, DrawingContext drawingContext)
		{
			this.document.Notifier.NotifyArea(this.BoundingBox);

			if ( this.draftStep == 0 )  // choix taille d'une cellule ?
			{
				Point p0 = this.Handle(0).Position;
				Point p1 = this.Handle(1).Position;
				this.draftCellWidth  = p1.X-p0.X;
				this.draftCellHeight = p1.Y-p0.Y;
			}

			if ( this.draftStep == 1 )  // choix du nombre de cellules ?
			{
				drawingContext.SnapGrid(ref pos);
				drawingContext.ConstrainSnapPos(ref pos);
				this.Handle(1).Position = pos;
				drawingContext.ConstrainDelStarting();

				this.columns = this.DraftColumns();
				this.rows    = this.DraftRows();
				this.InitWidths();
				this.InitHeights();
				this.InitCells(this.Cell(0,0));

				// Cr�e les 2 autres poign�es dans les coins oppos�s.
				Point pp1 = this.DraftP1();
				Rectangle rect = Rectangle.FromCorners(this.Handle(0).Position, pp1);
				Point p1 = rect.BottomLeft;
				Point p2 = rect.TopRight;
				this.Handle(0).Position = p1;
				this.Handle(1).Position = p2;
				this.HandleAdd(new Point(p1.X, p2.Y), HandleType.Primary);  // rang = 2
				this.HandleAdd(new Point(p2.X, p1.Y), HandleType.Primary);  // rang = 3
				this.HandleAdd(rect.Center, HandleType.Primary);  // rang = 4

				this.UpdateHandle();
			}

			this.draftStep ++;  // passe � la phase suivante
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// Indique si la cr�ation de l'objet est termin�e.
		public override bool CreateIsEnding(DrawingContext drawingContext)
		{
			if ( this.draftCellWidth == 0 || this.draftCellHeight == 0 )  return true;
			return (this.draftStep >= 2);
		}

		// Indique si l'objet doit exister. Retourne false si l'objet ne peut
		// pas exister et doit �tre d�truit.
		public override bool CreateIsExist(DrawingContext drawingContext)
		{
			Rectangle rect = Rectangle.FromCorners(this.Handle(0).Position, this.Handle(1).Position);
			return ( rect.Width > drawingContext.MinimalSize && rect.Height > drawingContext.MinimalSize );
		}

		// Indique s'il faut s�lectionner l'objet apr�s sa cr�ation.
		public override bool EditAfterCreation()
		{
			return true;
		}

		// Indique si un objet est �ditable.
		public override bool IsEditable
		{
			get { return true; }
		}

		// Lie l'objet �ditable � une r�gle.
		public override bool EditRulerLink(TextRuler ruler, DrawingContext drawingContext)
		{
			if ( this.cellToEdit == -1 )  return false;
			int c = this.cellToEdit%(this.columns+1);
			int r = this.cellToEdit/(this.columns+1);

			TextLayout textLayout = this.Cell(c,r).TextLayout;
			TextNavigator textNavigator = this.Cell(c,r).TextNavigator;

			ruler.TabCapability = true;
			ruler.AttachToText(textNavigator);

			double left = 0.0;
			if ( c > 0 )  left = this.columnWidth*this.widths[c-1];

			double right = 0.0;
			if ( c < this.widths.Length )  right = this.columnWidth*(1.0-this.widths[c]);

			PropertyJustif justif = this.Cell(c,r).TextJustif;
			left  += justif.MarginH;
			right += justif.MarginH;

			ruler.LeftMargin  = left*drawingContext.ScaleX;
			ruler.RightMargin = right*drawingContext.ScaleX;
			return true;
		}

		
		// Gestion d'un �v�nement pendant l'�dition.
		public override bool EditProcessMessage(Message message, Point pos)
		{
			if ( this.cellToEdit == -1 )  return false;
			int c = this.cellToEdit%(this.columns+1);
			int r = this.cellToEdit/(this.columns+1);

			TextLayout textLayout = this.Cell(c,r).TextLayout;
			TextNavigator textNavigator = this.Cell(c,r).TextNavigator;
			Transform transform = this.Cell(c,r).Transform;
			if ( transform == null )  return false;

			pos = transform.TransformInverse(pos);
			if ( textNavigator.ProcessMessage(message, pos) )
			{
				return true;
			}
			else
			{
				if ( message.Type == MessageType.KeyDown )
				{
					switch ( message.KeyCode )
					{
						case KeyCode.ArrowDown:
							if ( r <= 0 )  break;
							this.cellToHilite = this.cellToEdit-(this.columns+1);
							this.Select(true, true);
							return true;

						case KeyCode.ArrowUp:
							if ( r >= this.rows-1 )  break;
							this.cellToHilite = this.cellToEdit+(this.columns+1);
							this.Select(true, true);
							return true;

						case KeyCode.ArrowLeft:
							if ( c <= 0 )  break;
							this.cellToHilite = this.cellToEdit-1;
							this.Select(true, true);
							return true;

						case KeyCode.ArrowRight:
							if ( c >= this.columns-1 )  break;
							this.cellToHilite = this.cellToEdit+1;
							this.Select(true, true);
							return true;
					}
				}
				return false;
			}
		}

		// Gestion d'un �v�nement pendant l'�dition.
		public override void EditMouseDownMessage(Point pos)
		{
			if ( this.cellToEdit == -1 )  return;
			int c = this.cellToEdit%(this.columns+1);
			int r = this.cellToEdit/(this.columns+1);

			TextNavigator textNavigator = this.Cell(c,r).TextNavigator;
			Transform transform = this.Cell(c,r).Transform;
			if ( transform == null )  return;

			pos = transform.TransformInverse(pos);
			textNavigator.MouseDownMessage(pos);
		}


		// Met � jour les poign�es pour les largeurs/hauteurs.
		protected void UpdateHandle()
		{
			this.columnStart = this.Handle(2).Position;
			this.columnEnd   = this.Handle(1).Position;
			this.columnWidth = Point.Distance(this.columnStart, this.columnEnd);

			this.rowStart    = this.Handle(0).Position;
			this.rowEnd      = this.Handle(2).Position;
			this.rowHeight   = Point.Distance(this.rowStart, this.rowEnd);

			int total = 4+1;
			if ( this.columnWidth != 0 )  total += this.columns-1;
			if ( this.rowHeight   != 0 )  total += this.rows-1;

			// Supprime les poign�es en trop.
			while ( this.handles.Count > total )
			{
				this.HandleDelete(this.handles.Count-1);
			}

			// Ajoute les poign�es manquantes.
			while ( this.handles.Count < total )
			{
				this.HandleAdd(new Point(0,0), HandleType.Secondary);
				if ( !this.IsEdited )
				{
					this.Handle(this.handles.Count-1).IsVisible = true;
				}
			}

			// Positionne la poign�e centrale de d�placement global.
			this.Handle(4).Position = (this.Handle(0).Position+this.Handle(1).Position)/2;

			// Positionne les poign�es pour les largeurs de colonnes.
			for ( int i=0 ; i<this.columns-1 ; i++ )
			{
				int rank = this.RankColumnHandle(i);
				if ( rank == -1 )  continue;
				this.Handle(rank).Position = Point.Move(this.columnStart, this.columnEnd, this.columnWidth*this.widths[i]);
			}

			// Positionne les poign�es pour les hauteurs de lignes.
			for ( int i=0 ; i<this.rows-1 ; i++ )
			{
				int rank = this.RankRowHandle(i);
				if ( rank == -1 )  continue;
				this.Handle(rank).Position = Point.Move(this.rowStart, this.rowEnd, this.rowHeight*this.heights[i]);
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

		// Met � jour les largeurs/hauteurs en fonction des poign�es.
		protected void UpdateWidthsHeights()
		{
			this.columnStart = this.Handle(2).Position;
			this.columnEnd   = this.Handle(1).Position;
			this.columnWidth = Point.Distance(this.columnStart, this.columnEnd);

			this.rowStart    = this.Handle(0).Position;
			this.rowEnd      = this.Handle(2).Position;
			this.rowHeight   = Point.Distance(this.rowStart, this.rowEnd);

			for ( int i=0 ; i<this.columns-1 ; i++ )
			{
				int rank = this.RankColumnHandle(i);
				if ( rank == -1 )  continue;
				Point pos = this.Handle(rank).Position;
				this.widths[i] = Point.Distance(this.columnStart, pos)/this.columnWidth;
			}

			for ( int i=0 ; i<this.rows-1 ; i++ )
			{
				int rank = this.RankRowHandle(i);
				if ( rank == -1 )  continue;
				Point pos = this.Handle(rank).Position;
				this.heights[i] = Point.Distance(this.rowStart, pos)/this.rowHeight;
			}
		}

		// Retourne le rang d'une poign�e secondaire de colonne.
		protected int RankColumnHandle(int index)
		{
			if ( this.columnWidth == 0 )  return -1;
			return 4+1+index;
		}

		// Retourne le rang d'une poign�e secondaire de ligne.
		protected int RankRowHandle(int index)
		{
			if ( this.rowHeight == 0 )  return -1;
			int rank = 4+1;
			if ( this.columnWidth != 0 )  rank += this.columns-1;
			return rank+index;
		}


		// Retourne l'�paisseur de trait maximale.
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

		// Met � jour le rectangle englobant l'objet.
		protected override void UpdateBoundingBox()
		{
			Path path = this.PathBuild();
			this.bboxThin = path.ComputeBounds();

			if ( this.draftStep == 1 )  // choix du nombre de cellules ?
			{
				Point p = this.Handle(0).Position;
				p.X += this.draftCellWidth;
				p.Y += this.draftCellHeight;
				this.bboxThin.MergeWith(p);
			}

			this.bboxGeom = this.bboxThin;
			this.bboxGeom.Inflate(this.MaxWidth()*0.5);

			this.bboxFull = this.bboxGeom;
		}


		// D�s�lectionne toutes les cellules.
		protected void DeselectAllCells()
		{
			this.SelectAllCells(false);
		}

		// S�lectionne toutes les cellules.
		protected void SelectAllCells()
		{
			this.SelectAllCells(true);
		}

		// S�lectionne toutes les cellules.
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

		// Indique si une cellule est s�lecitonn�e.
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

		// Initialise les cellules d'apr�s une cellule mod�le.
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
			ArrayCell cell = new ArrayCell(this.document);
			cell.Selected = false;

			cell.LeftLine = new PropertyLine(this.document);
			this.PropertyLineMode.CopyTo(cell.LeftLine);

			cell.BottomLine = new PropertyLine(this.document);
			this.PropertyLineMode.CopyTo(cell.BottomLine);

			cell.LeftColor = new PropertyColor(this.document);
			this.PropertyLineColor.CopyTo(cell.LeftColor);

			cell.BottomColor = new PropertyColor(this.document);
			this.PropertyLineColor.CopyTo(cell.BottomColor);

			cell.BackColor = new PropertyColor(this.document);
			this.PropertyBackColor.CopyTo(cell.BackColor);

			cell.TextJustif = new PropertyJustif(this.document);
			this.PropertyTextJustif.CopyTo(cell.TextJustif);

			return cell;
		}

		// Initialise une nouvelle cellule d'apr�s une cellule mod�le.
		protected ArrayCell NewCell(ArrayCell model)
		{
			ArrayCell cell = new ArrayCell(this.document);
			cell.Selected = false;

			cell.Content = model.Content;

			cell.LeftLine = new PropertyLine(this.document);
			model.LeftLine.CopyTo(cell.LeftLine);

			cell.BottomLine = new PropertyLine(this.document);
			model.BottomLine.CopyTo(cell.BottomLine);

			cell.LeftColor = new PropertyColor(this.document);
			model.LeftColor.CopyTo(cell.LeftColor);

			cell.BottomColor = new PropertyColor(this.document);
			model.BottomColor.CopyTo(cell.BottomColor);

			cell.BackColor = new PropertyColor(this.document);
			model.BackColor.CopyTo(cell.BackColor);

			cell.TextJustif = new PropertyJustif(this.document);
			model.TextJustif.CopyTo(cell.TextJustif);

			return cell;
		}

		
		// Reprend toutes les caract�ristiques d'un objet.
		public override void CloneObject(AbstractObject src)
		{
			base.CloneObject(src);

			ObjectArray array = src as ObjectArray;
			this.columns    = array.columns;
			this.rows       = array.rows;
			this.cellToEdit = array.cellToEdit;

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

			if ( this.cells.Count != array.cells.Count )
			{
				this.InitCells();
			}
			for ( int c=0 ; c<this.columns+1 ; c++ )
			{
				for ( int r=0 ; r<this.rows+1 ; r++ )
				{
					array.Cell(c,r).CopyTo(this.Cell(c,r));
				}
			}

			this.UpdateHandle();
		}


		// Calcule le nombre de colonnes pendant la cr�ation.
		protected int DraftColumns()
		{
			Point p0 = this.Handle(0).Position;
			Point p1 = this.Handle(1).Position;
			int total = (int)((p1.X-p0.X)/this.draftCellWidth);
			if ( total < 1 )  total = 1;
			if ( total > this.maxColumns )  total = this.maxColumns;
			return total;
		}

		// Calcule le nombre de lignes pendant la cr�ation.
		protected int DraftRows()
		{
			Point p0 = this.Handle(0).Position;
			Point p1 = this.Handle(1).Position;
			int total = (int)((p1.Y-p0.Y)/this.draftCellHeight);
			if ( total < 1 )  total = 1;
			if ( total > this.maxRows )  total = this.maxRows;
			return total;
		}

		// Calcule le point p1' pendant la cr�ation.
		protected Point DraftP1()
		{
			Point p0 = this.Handle(0).Position;
			Point p1 = this.Handle(1).Position;
			Point p = p0;
			p.X += this.draftCellWidth*this.DraftColumns();
			p.Y += this.draftCellHeight*this.DraftRows();
			return p;
		}

		// Dessine les dimensions du tableau.
		protected void DrawDraftDim(Graphics graphics, DrawingContext drawingContext)
		{
			Point p0 = this.Handle(0).Position;
			Point p1 = new Point(p0.X+this.draftCellWidth, p0.Y+this.draftCellHeight);
			string text = string.Format(" {0} x {1} ", this.DraftColumns(), this.DraftRows());
			Font font = Font.GetFont("Tahoma", "Regular");
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
			graphics.RenderSolid(Color.FromBrightness(0));
		}

		// Dessine un brouillon du tableau (pendant la cr�ation).
		protected void DrawDraft(Graphics graphics, DrawingContext drawingContext)
		{
			ArrayCell cell = this.Cell(0,0);  // prend la 1�re cellule comme attributs

			double initialWidth = graphics.LineWidth;

			if ( this.draftStep == 0 )  // choix taille d'une cellule ?
			{
				Path path = this.PathBuild();

				graphics.Rasterizer.AddSurface(path);
				graphics.RenderSolid(cell.BackColor.Color);

				graphics.Rasterizer.AddOutline(path, cell.LeftLine.Width);
				graphics.RenderSolid(cell.LeftColor.Color);
			}

			if ( this.draftStep == 1 )  // choix du nombre de cellules ?
			{
				Point p0 = this.Handle(0).Position;
				Point p1 = this.Handle(1).Position;
				Point pp1 = this.DraftP1();
				int nbColumns = this.DraftColumns();
				int nbRows    = this.DraftRows();

				Rectangle rect = Rectangle.FromCorners(p0, p1);
				graphics.AddFilledRectangle(rect);
				graphics.RenderSolid(drawingContext.HiliteOutlineColor);

				graphics.LineWidth = cell.LeftLine.Width;

				rect = Rectangle.FromCorners(p0, pp1);
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

				this.DrawDraftDim(graphics, drawingContext);
			}

			graphics.LineWidth = initialWidth;
		}


		// Calcule le point � l'intersection d'une ligne et d'une colonne.
		// c: [0..this.columns+1]
		// r: [0..this.rows+1]
		protected Point CrossDot(int c, int r)
		{
			Point p0,p1,p2,p3, p03,p21, p;

			p0 = this.Handle(0).Position;
			p1 = this.Handle(1).Position;

			if ( this.handles.Count < 4 )
			{
				p2 = new Point(p0.X, p1.Y);
				p3 = new Point(p1.X, p0.Y);
			}
			else
			{
				p2 = this.Handle(2).Position;
				p3 = this.Handle(3).Position;
			}

			double d03 = Point.Distance(p0, p3);
			if ( c == 0 || d03 == 0 )
			{
				p03 = p0;
			}
			else if ( c < this.columns )
			{
				p03 = Point.Move(p0, p3, this.widths[c-1]*d03);
			}
			else
			{
				p03 = p3;
			}

			double d21 = Point.Distance(p2, p1);
			if ( c == 0 || d21 == 0 )
			{
				p21 = p2;
			}
			else if ( c < this.columns )
			{
				p21 = Point.Move(p2, p1, this.widths[c-1]*d21);
			}
			else
			{
				p21 = p1;
			}

			double d = Point.Distance(p03, p21);
			if ( r == 0 || d == 0 )
			{
				p = p03;
			}
			else if ( r < this.rows )
			{
				p = Point.Move(p03, p21, this.heights[r-1]*d);
			}
			else
			{
				p = p21;
			}
			return p;
		}

		// Cr�e le chemin de l'objet.
		protected Path PathBuild()
		{
			Point p1 = this.Handle(0).Position;
			Point p2 = new Point();
			Point p3 = this.Handle(1).Position;
			Point p4 = new Point();

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

			Path path = new Path();
			path.MoveTo(p1);
			path.LineTo(p2);
			path.LineTo(p3);
			path.LineTo(p4);
			path.Close();
			return path;
		}

		// Retourne les 4 coins d'une cellule en fonction de son orientation.
		protected void CellCorners(int c, int r, out Point pbl, out Point pbr, out Point ptl, out Point ptr)
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
		protected void DrawCellText(IPaintPort port, DrawingContext drawingContext, int c, int r)
		{
			TextLayout textLayout = this.Cell(c,r).TextLayout;
			TextNavigator textNavigator = this.Cell(c,r).TextNavigator;

			Point p1, p2, p3, p4;
			this.CellCorners(c,r, out p1, out p2, out p3, out p4);
			if ( !this.Cell(c,r).TextJustif.DeflateBox(ref p1, ref p2, ref p3, ref p4) )  return;

			Size size = new Size();
			size.Width  = Point.Distance(p1,p2);
			size.Height = Point.Distance(p1,p3);
			textLayout.LayoutSize = size;
			textLayout.DrawingScale = drawingContext.ScaleX;

			textLayout.DefaultFont     = this.PropertyTextFont.GetFont();
			textLayout.DefaultFontSize = this.PropertyTextFont.FontSize;
			textLayout.DefaultColor    = this.PropertyTextFont.FontColor;

			JustifVertical   jv = this.Cell(c,r).TextJustif.Vertical;
			JustifHorizontal jh = this.Cell(c,r).TextJustif.Horizontal;

			if ( jv == JustifVertical.Top )
			{
				     if ( jh == JustifHorizontal.Center )  textLayout.Alignment = ContentAlignment.TopCenter;
				else if ( jh == JustifHorizontal.Right  )  textLayout.Alignment = ContentAlignment.TopRight;
				else                                       textLayout.Alignment = ContentAlignment.TopLeft;
			}
			if ( jv == JustifVertical.Center )
			{
				     if ( jh == JustifHorizontal.Center )  textLayout.Alignment = ContentAlignment.MiddleCenter;
				else if ( jh == JustifHorizontal.Right  )  textLayout.Alignment = ContentAlignment.MiddleRight;
				else                                       textLayout.Alignment = ContentAlignment.MiddleLeft;
			}
			if ( jv == JustifVertical.Bottom )
			{
				     if ( jh == JustifHorizontal.Center )  textLayout.Alignment = ContentAlignment.BottomCenter;
				else if ( jh == JustifHorizontal.Right  )  textLayout.Alignment = ContentAlignment.BottomRight;
				else                                       textLayout.Alignment = ContentAlignment.BottomLeft;
			}

			     if ( jh == JustifHorizontal.Justif )  textLayout.JustifMode = TextJustifMode.AllButLast;
			else if ( jh == JustifHorizontal.All    )  textLayout.JustifMode = TextJustifMode.All;
			else                                       textLayout.JustifMode = TextJustifMode.NoLine;

			Transform ot = port.Transform;

			double angle = Point.ComputeAngleDeg(p1, p2);
			Transform transform = new Transform();
			transform.Translate(p1);
			transform.RotateDeg(angle, p1);
			this.Cell(c,r).Transform = transform;
			port.MergeTransform(transform);

			bool edited = this.edited;
			int ce = this.cellToEdit%(this.columns+1);
			int re = this.cellToEdit/(this.columns+1);
			if ( ce != c || re != r )  edited = false;

			bool active = (this.document.Modifier.ActiveViewer.DrawingContext == drawingContext);

			if ( port is Graphics &&
				 active &&
				 edited &&
				 textNavigator.Context.CursorFrom != textNavigator.Context.CursorTo )
			{
				Graphics graphics = port as Graphics;
				int from = System.Math.Min(textNavigator.Context.CursorFrom, textNavigator.Context.CursorTo);
				int to   = System.Math.Max(textNavigator.Context.CursorFrom, textNavigator.Context.CursorTo);
				TextLayout.SelectedArea[] areas = textLayout.FindTextRange(new Point(0,0), from, to);
				for ( int i=0 ; i<areas.Length ; i++ )
				{
					graphics.Align(ref areas[i].Rect);
					graphics.AddFilledRectangle(areas[i].Rect);
					graphics.RenderSolid(DrawingContext.ColorSelectEdit);
				}
			}

			textLayout.ShowLineBreak = edited;
			textLayout.ShowTab       = edited;
			textLayout.Paint(new Point(0,0), port);

			if ( port is Graphics &&
				 active &&
				 edited &&
				 textNavigator.Context.CursorTo != -1 )
			{
				Graphics graphics = port as Graphics;
				Point c1, c2;
				if ( textLayout.FindTextCursor(textNavigator.Context, out c1, out c2) )
				{
					graphics.LineWidth = 1.0/drawingContext.ScaleX;
					graphics.AddLine(c1, c2);
					graphics.RenderSolid(DrawingContext.ColorFrameEdit);
				}
			}

			port.Transform = ot;
		}

		// Dessine le fond d'une cellule.
		protected void DrawCellSurface(IPaintPort port, DrawingContext drawingContext, int c, int r)
		{
			Path path = new Path();
			path.MoveTo(this.Cell(c,r).BottomLeft);
			path.LineTo(this.Cell(c,r).TopLeft);
			path.LineTo(this.Cell(c,r).TopRight);
			path.LineTo(this.Cell(c,r).BottomRight);
			path.Close();
			if ( port is Graphics )
			{
				Graphics graphics = port as Graphics;
				graphics.Rasterizer.AddSurface(path);
				graphics.RenderSolid(drawingContext.AdaptColor(this.Cell(c,r).BackColor.Color));
			}
			else
			{
				port.Color = this.Cell(c,r).BackColor.Color;
				port.PaintSurface(path);
			}
		}

		// Dessine les traits d'une cellule.
		protected void DrawCellOutline(IPaintPort port, DrawingContext drawingContext, int c, int r)
		{
			PropertyLine line;
			PropertyColor color;

			Path path = new Path();
			path.MoveTo(this.Cell(c,r).BottomLeft);
			path.LineTo(this.Cell(c,r).BottomRight);
			line = this.Cell(c,r).BottomLine;
			color = this.Cell(c,r).BottomColor;
			if ( port is Graphics )
			{
				Graphics graphics = port as Graphics;
				line.DrawPath(graphics, drawingContext, path, color.Color);
			}
			else
			{
				Printing.PrintPort pp = port as Printing.PrintPort;
				pp.Color = color.Color;
				line.PaintOutline(pp, drawingContext, path);
			}

			path = new Path();
			path.MoveTo(this.Cell(c,r).BottomLeft);
			path.LineTo(this.Cell(c,r).TopLeft);
			line = this.Cell(c,r).LeftLine;
			color = this.Cell(c,r).LeftColor;
			if ( port is Graphics )
			{
				Graphics graphics = port as Graphics;
				line.DrawPath(graphics, drawingContext, path, color.Color);
			}
			else
			{
				Printing.PrintPort pp = port as Printing.PrintPort;
				pp.Color = color.Color;
				line.PaintOutline(pp, drawingContext, path);
			}
		}

		// Dessine la mise en �vidence d'une cellule.
		protected void DrawCellHilite(Graphics graphics, DrawingContext drawingContext, int c, int r)
		{
			Point pbl, pbr, ptl, ptr;
			pbl = this.Cell(c,r).BottomLeft;
			pbr = this.Cell(c,r).BottomRight;
			ptl = this.Cell(c,r).TopLeft;
			ptr = this.Cell(c,r).TopRight;

			Path path = new Path();
			path.MoveTo(pbl);
			path.LineTo(ptl);
			path.LineTo(ptr);
			path.LineTo(pbr);
			path.Close();
			graphics.Rasterizer.AddSurface(path);
			graphics.RenderSolid(Color.FromColor(drawingContext.HiliteOutlineColor, 0.3));

			double initialWidth = graphics.LineWidth;
			CapStyle initialCap = graphics.LineCap;
			graphics.LineWidth = 6.0/drawingContext.ScaleX;
			graphics.LineCap = CapStyle.Square;
			if ( this.PropertyOutline(c,r, OutlinePos.Left  ) )  graphics.AddLine(pbl, ptl);
			if ( this.PropertyOutline(c,r, OutlinePos.Right ) )  graphics.AddLine(pbr, ptr);
			if ( this.PropertyOutline(c,r, OutlinePos.Bottom) )  graphics.AddLine(pbl, pbr);
			if ( this.PropertyOutline(c,r, OutlinePos.Top   ) )  graphics.AddLine(ptl, ptr);
			graphics.RenderSolid(drawingContext.HiliteOutlineColor);
			graphics.LineWidth = initialWidth;
			graphics.LineCap = initialCap;
		}

		// Dessine la cellule en �dition.
		protected void DrawCellEdit(Graphics graphics, DrawingContext drawingContext, int c, int r)
		{
			Point pbl, pbr, ptl, ptr;
			pbl = this.Cell(c,r).BottomLeft;
			pbr = this.Cell(c,r).BottomRight;
			ptl = this.Cell(c,r).TopLeft;
			ptr = this.Cell(c,r).TopRight;

			Path path = new Path();
			path.MoveTo(pbl);
			path.LineTo(ptl);
			path.LineTo(ptr);
			path.LineTo(pbr);
			path.Close();
			graphics.Rasterizer.AddOutline(path, 2.0/drawingContext.ScaleX);
			graphics.RenderSolid(DrawingContext.ColorFrameEdit);
		}

		// Dessine l'objet.
		public override void DrawGeometry(Graphics graphics, DrawingContext drawingContext)
		{
			base.DrawGeometry(graphics, drawingContext);

			if ( this.TotalHandle < 2 )  return;

			if ( this.TotalHandle <= 2 )  // en construction ?
			{
				this.DrawDraft(graphics, drawingContext);
				return;
			}

			// Dessine tous fonds de cellules.
			for ( int c=0 ; c<this.columns ; c++ )
			{
				for ( int r=0 ; r<this.rows ; r++ )
				{
					this.DrawCellSurface(graphics, drawingContext, c,r);
				}
			}

			// Dessine tous les textes des cellules.
			for ( int c=0 ; c<this.columns ; c++ )
			{
				for ( int r=0 ; r<this.rows ; r++ )
				{
					this.DrawCellText(graphics, drawingContext, c,r);
				}
			}

			// Dessine tous les traits des cellules.
			for ( int c=0 ; c<this.columns+1 ; c++ )
			{
				for ( int r=0 ; r<this.rows+1 ; r++ )
				{
					this.DrawCellOutline(graphics, drawingContext, c,r);
				}
			}

			if ( !drawingContext.IsActive )  return;

			if ( this.edited && this.cellToEdit != -1 )  // en cours d'�dition ?
			{
				int c = this.cellToEdit%(this.columns+1);
				int r = this.cellToEdit/(this.columns+1);
				this.DrawCellEdit(graphics, drawingContext, c,r);
			}

			if ( this.cellToHilite != -1 && this.cellToHilite != this.cellToEdit )
			{
				int c = this.cellToHilite%(this.columns+1);
				int r = this.cellToHilite/(this.columns+1);
				this.DrawCellHilite(graphics, drawingContext, c,r);
			}

			if ( !this.edited )
			{
				if ( this.selected )  // tableau s�lectionn� ?
				{
					// Dessine les cellules en �vidence.
					for ( int c=0 ; c<this.columns ; c++ )
					{
						for ( int r=0 ; r<this.rows ; r++ )
						{
							if ( !this.Cell(c,r).Selected )  continue;
							this.DrawCellHilite(graphics, drawingContext, c,r);
						}
					}
				}
				else	// tableau non s�lectionn� ?
				{
					// Dessine la mise en �vidence compl�te.
					if ( this.IsHilite )
					{
						Path path = this.PathBuild();
						graphics.Rasterizer.AddSurface(path);
						graphics.RenderSolid(drawingContext.HiliteSurfaceColor);
					}
				}
			}
		}


		// Imprime l'objet.
		public override void PrintGeometry(Printing.PrintPort port, DrawingContext drawingContext)
		{
			base.PrintGeometry(port, drawingContext);

			if ( this.TotalHandle < 2 )  return;

			// Imprime tous fonds de cellules.
			for ( int c=0 ; c<this.columns ; c++ )
			{
				for ( int r=0 ; r<this.rows ; r++ )
				{
					this.DrawCellSurface(port, drawingContext, c,r);
				}
			}

			// Imprime tous les textes des cellules.
			for ( int c=0 ; c<this.columns ; c++ )
			{
				for ( int r=0 ; r<this.rows ; r++ )
				{
					this.DrawCellText(port, drawingContext, c,r);
				}
			}

			// Imprime tous les traits des cellules.
			for ( int c=0 ; c<this.columns+1 ; c++ )
			{
				for ( int r=0 ; r<this.rows+1 ; r++ )
				{
					this.DrawCellOutline(port, drawingContext, c,r);
				}
			}
		}

		
		#region Serialization
		// S�rialise l'objet.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		// Constructeur qui d�s�rialise l'objet.
		protected ObjectArray(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
		#endregion

		
		protected int							columns;
		protected int							rows;
		protected double[]						widths;
		protected double[]						heights;
		protected System.Collections.ArrayList	cells = new System.Collections.ArrayList();

		protected Point							columnStart;
		protected Point							columnEnd;
		protected double						columnWidth;
		protected Point							rowStart;
		protected Point							rowEnd;
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

		protected int							maxColumns = 13;
		protected int							maxRows    = 13;
	}
}

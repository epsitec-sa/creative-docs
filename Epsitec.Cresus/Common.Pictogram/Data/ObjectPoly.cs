using Epsitec.Common.Support;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe ObjectPoly est la classe de l'objet graphique "polygone".
	/// </summary>
	public class ObjectPoly : AbstractObject
	{
		public ObjectPoly()
		{
			PropertyLine lineMode = new PropertyLine();
			lineMode.Type = PropertyType.LineMode;
			this.AddProperty(lineMode);

			PropertyColor lineColor = new PropertyColor();
			lineColor.Type = PropertyType.LineColor;
			this.AddProperty(lineColor);

			PropertyArrow arrow = new PropertyArrow();
			arrow.Type = PropertyType.Arrow;
			arrow.Changed += new EventHandler(this.HandleChanged);
			this.AddProperty(arrow);

			PropertyGradient fillGradient = new PropertyGradient();
			fillGradient.Type = PropertyType.FillGradient;
			this.AddProperty(fillGradient);

			PropertyBool fillClose = new PropertyBool();
			fillClose.Type = PropertyType.PolyClose;
			this.AddProperty(fillClose);

			PropertyCorner corner = new PropertyCorner();
			corner.Type = PropertyType.Corner;
			this.AddProperty(corner);
		}

		protected override AbstractObject CreateNewObject()
		{
			return new ObjectPoly();
		}

		public override void Dispose()
		{
			if ( this.ExistProperty(2) )  this.PropertyArrow(2).Changed -= new EventHandler(this.HandleChanged);
			base.Dispose();
		}


		// Nom de l'icône.
		public override string IconName
		{
			get { return @"file:images/poly.icon"; }
		}


		// Détecte si la souris est sur l'objet.
		public override bool Detect(Drawing.Point pos)
		{
			if ( this.isHide )  return false;

			Drawing.Rectangle bbox = this.BoundingBox;
			if ( !bbox.Contains(pos) )  return false;

			Drawing.Path pathStart;  bool outlineStart, surfaceStart;
			Drawing.Path pathEnd;    bool outlineEnd,   surfaceEnd;
			Drawing.Path pathLine;
			this.PathBuild(out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine);

			double width = System.Math.Max(this.PropertyLine(0).Width/2, this.minimalWidth);
			
			if (                 AbstractObject.DetectOutline(pathLine,  width, pos) )  return true;
			if ( outlineStart && AbstractObject.DetectOutline(pathStart, width, pos) )  return true;
			if ( outlineEnd   && AbstractObject.DetectOutline(pathEnd,   width, pos) )  return true;

			if ( surfaceStart && AbstractObject.DetectSurface(pathStart, pos) )  return true;
			if ( surfaceEnd   && AbstractObject.DetectSurface(pathEnd,   pos) )  return true;

			if ( this.PropertyGradient(3).IsVisible() )
			{
				pathLine.Close();
				if ( AbstractObject.DetectSurface(pathLine, pos) )  return true;
			}

			return false;
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

			if ( this.Handle(rank).Type == HandleType.Primary )  // principale ?
			{
				this.Handle(rank).Position = pos;
			}
			else
			{
				if ( rank == this.HandleArrowRank(0) )  // pp1 ?
				{
					double d = Drawing.Point.Distance(this.Handle(0).Position, pos);
					this.PropertyArrow(2).Length1 = d;
				}

				if ( rank == this.HandleArrowRank(1) )  // pp2 ?
				{
					double d = Drawing.Point.Distance(this.Handle(this.TotalHandlePrimary-1).Position, pos);
					this.PropertyArrow(2).Length2 = d;
				}
			}
			this.UpdateHandle();
			this.dirtyBbox = true;
		}

		// Indique si le déplacement d'une poignée doit se répercuter sur les propriétés.
		public override bool IsMoveHandlePropertyChanged(int rank)
		{
			if ( rank >= this.handles.Count )  // poignée d'une propriété ?
			{
				return base.IsMoveHandlePropertyChanged(rank);
			}
			return ( rank >= this.TotalHandlePrimary );
		}

		// Retourne la propriété modifiée en déplaçant une poignée.
		public override AbstractProperty MoveHandleProperty(int rank)
		{
			if ( rank >= this.handles.Count )  // poignée d'une propriété ?
			{
				return base.MoveHandleProperty(rank);
			}
			if ( rank >= this.TotalHandlePrimary )  return this.PropertyArrow(2);
			return null;
		}


		// Déplace globalement l'objet.
		public override void MoveGlobal(GlobalModifierData initial, GlobalModifierData final, bool all)
		{
			base.MoveGlobal(initial, final, all);
			this.UpdateHandle();
		}


		// Donne le contenu du menu contextuel.
		public override void ContextMenu(System.Collections.ArrayList list, Drawing.Point pos, int handleRank)
		{
			ContextMenuItem item;

			if ( handleRank == -1 )
			{
				if ( this.DetectOutline(pos) == -1 )  return;

				item = new ContextMenuItem();
				list.Add(item);  // séparateur

				item = new ContextMenuItem();
				item.Command = "Object";
				item.Name = "HandleAdd";
				item.Icon = @"file:images/add.icon";
				item.Text = "Ajouter un point";
				list.Add(item);
			}
			else
			{
				if ( this.handles.Count > 2 && this.Handle(handleRank).Type == HandleType.Primary )
				{
					item = new ContextMenuItem();
					list.Add(item);  // séparateur

					HandleConstrainType type = this.Handle(handleRank).ConstrainType;

					item = new ContextMenuItem();
					item.Command = "Object";
					item.Name = "HandleSym";
					item.IconActiveNo = @"file:images/activeno.icon";
					item.IconActiveYes = @"file:images/activeyes.icon";
					item.Active = ( type == HandleConstrainType.Symmetric );
					item.Text = "Coin quelconque";
					list.Add(item);

					item = new ContextMenuItem();
					item.Command = "Object";
					item.Name = "HandleSimply";
					item.IconActiveNo = @"file:images/activeno.icon";
					item.IconActiveYes = @"file:images/activeyes.icon";
					item.Active = ( type == HandleConstrainType.Simply );
					item.Text = "Coin toujours droit";
					list.Add(item);

					item = new ContextMenuItem();
					list.Add(item);  // séparateur

					item = new ContextMenuItem();
					item.Command = "Object";
					item.Name = "HandleDelete";
					item.Icon = @"file:images/sub.icon";
					item.Text = "Enlever le point";
					list.Add(item);
				}
			}
		}

		// Exécute une commande du menu contextuel.
		public override void ContextCommand(string cmd, Drawing.Point pos, int handleRank)
		{
			if ( cmd == "HandleAdd" )
			{
				int rank = this.DetectOutline(pos);
				if ( rank == -1 )  return;

				int next = rank+1;
				if ( next >= this.handles.Count )  next = 0;
				Drawing.Point p = Drawing.Point.Projection(this.Handle(rank).Position, this.Handle(next).Position, pos);

				Handle handle = new Handle();
				handle.Position = p;
				handle.Type = HandleType.Primary;
				handle.IsSelected = true;
				this.HandleInsert(rank+1, handle);
				this.UpdateHandle();
			}

			if ( cmd == "HandleDelete" )
			{
				this.HandleDelete(handleRank);
				this.UpdateHandle();
			}

			if ( cmd == "HandleSym" )
			{
				this.Handle(handleRank).ConstrainType = HandleConstrainType.Symmetric;
			}

			if ( cmd == "HandleSimply" )
			{
				this.Handle(handleRank).ConstrainType = HandleConstrainType.Simply;
			}
		}

		// Détecte si la souris est sur le pourtour de l'objet.
		// Retourne le rank de la poignée de départ, ou -1
		protected int DetectOutline(Drawing.Point pos)
		{
			Drawing.Path pathStart;  bool outlineStart, surfaceStart;
			Drawing.Path pathEnd;    bool outlineEnd,   surfaceEnd;
			Drawing.Path pathLine;
			this.PathBuild(out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine);

			double width = System.Math.Max(this.PropertyLine(0).Width/2, this.minimalWidth);
			return AbstractObject.DetectOutlineRank(pathLine, width, pos);
		}


		// Début de la création d'un objet.
		public override void CreateMouseDown(Drawing.Point pos, IconContext iconContext)
		{
			iconContext.ConstrainSnapPos(ref pos);

			if ( this.TotalHandle == 0 )
			{
				this.PropertyBool(4).Bool = false;
				this.HandleAdd(pos, HandleType.Starting);
				this.Handle(0).IsSelected = true;
			}
			else
			{
				this.HandleAdd(pos, HandleType.Primary);
				this.Handle(this.TotalHandle-1).IsSelected = true;
			}

			this.mouseDown = true;
		}

		// Déplacement pendant la création d'un objet.
		public override void CreateMouseMove(Drawing.Point pos, IconContext iconContext)
		{
			iconContext.ConstrainSnapPos(ref pos);

			int rank = this.TotalHandle-1;
			if ( rank > 0 )
			{
				double len = Drawing.Point.Distance(this.Handle(0).Position, pos);
				if ( len <= this.closeMargin )
				{
					this.Handle(0).Type = HandleType.Ending;
				}
				else
				{
					this.Handle(0).Type = HandleType.Starting;
				}
			}

			this.TempMove(pos, iconContext);

			if ( this.mouseDown )
			{
				this.Handle(rank).Position = pos;
			}
			this.dirtyBbox = true;
		}

		// Fin de la création d'un objet.
		public override void CreateMouseUp(Drawing.Point pos, IconContext iconContext)
		{
			iconContext.ConstrainSnapPos(ref pos);
			int rank = this.TotalHandle-1;
			this.Handle(rank).Position = pos;
			iconContext.ConstrainFixStarting(pos);
			this.mouseDown = false;
			this.TempCreate(pos, iconContext);
		}

		// Indique si la création de l'objet est terminée.
		public override bool CreateIsEnding(IconContext iconContext)
		{
			if ( this.TotalHandle < 2 )  return false;

			int rank = this.TotalHandle-1;
			double len = Drawing.Point.Distance(this.Handle(0).Position, this.Handle(rank).Position);
			if ( len > this.closeMargin )  return false;  // pas fini

			this.HandleDelete(rank);
			this.PropertyBool(4).Bool = true;

			this.TempDelete();
			this.Handle(0).Type = HandleType.Primary;
			this.Deselect();
			iconContext.ConstrainDelStarting();
			this.UpdateHandle();
			return true;
		}

		// Indique si l'objet doit exister. Retourne false si l'objet ne peut
		// pas exister et doit être détruit.
		public override bool CreateIsExist(IconContext iconContext)
		{
			return ( this.TotalHandle >= 2 );
		}

		// Termine la création de l'objet. Retourne false si l'objet ne peut
		// pas exister et doit être détruit.
		public override bool CreateEnding(IconContext iconContext)
		{
			if ( this.TotalHandle < 2 )  return false;

			this.TempDelete();
			this.Handle(0).Type = HandleType.Primary;
			this.Deselect();
			iconContext.ConstrainDelStarting();
			this.UpdateHandle();
			return true;
		}

		// Retourne un bouton d'action pendant la création.
		public override bool CreateAction(int rank, out string cmd, out string name, out string text)
		{
			if ( rank == 0 )
			{
				cmd  = "Object";
				name = "CreateEnding";
				text = "Terminer la création";
				return true;
			}
			if ( rank == 1 )
			{
				cmd  = "Object";
				name = "CreateAndSelect";
				text = "Terminer et sélectionner";
				return true;
			}
			return base.CreateAction(rank, out cmd, out name, out text);
		}

		// Crée l'objet temporaire pour montrer le nouveau segment.
		protected void TempCreate(Drawing.Point pos, IconContext iconContext)
		{
			this.TempDelete();

			this.tempLine = new ObjectLine();
			this.tempLine.CloneProperties(this);

			AbstractProperty ap = this.tempLine.GetProperty(PropertyType.LineColor);
			PropertyColor pc = ap as PropertyColor;
			pc.Color = Drawing.Color.FromARGB(0.2, pc.Color.R, pc.Color.G, pc.Color.B);
			this.tempLine.SetProperty(pc);

			ap = this.tempLine.GetProperty(PropertyType.LineMode);
			PropertyLine pl = ap as PropertyLine;
			if ( pl.Width == 0 )  pl.Width = 1.0*this.scaleX;
			this.tempLine.SetProperty(pl);

			ap = this.tempLine.GetProperty(PropertyType.Arrow);
			PropertyArrow pa = ap as PropertyArrow;
			pa.ArrowType1 = ArrowType.Right;
			pa.ArrowType2 = ArrowType.Right;
			this.tempLine.SetProperty(pa);

			this.tempLine.CreateMouseDown(pos, iconContext);
		}

		// Déplace l'objet temporaire pour montrer le nouveau segment.
		protected void TempMove(Drawing.Point pos, IconContext iconContext)
		{
			if ( this.tempLine != null )
			{
				this.tempLine.CreateMouseMove(pos, iconContext);
			}
		}

		// Détruit l'objet temporaire pour montrer le nouveau segment.
		protected void TempDelete()
		{
			this.tempLine = null;
		}

		
		private void HandleChanged(object sender)
		{
			this.UpdateHandle();
		}

		// Met à jour les poignées pour les profondeurs des flèches.
		protected void UpdateHandle()
		{
			int total = this.TotalHandlePrimary;
			if ( total < 2 )  return;

			Drawing.Point p1, p2, pp1, pp2;
			p1 = this.Handle(0).Position;
			p2 = this.Handle(1).Position;
			pp1 = Drawing.Point.Move(p1, p2, this.PropertyArrow(2).GetLength(0));
			p1 = this.Handle(total-1).Position;
			p2 = this.Handle(total-2).Position;
			pp2 = Drawing.Point.Move(p1, p2, this.PropertyArrow(2).GetLength(1));
			int r1 = this.HandleArrowRank(0);
			int r2 = this.HandleArrowRank(1);
			total += ((r1==-1)?0:1) + ((r2==-1)?0:1);

			// Supprime les poignées en trop.
			while ( this.handles.Count > total )
			{
				this.HandleDelete(this.handles.Count-1);
			}

			// Ajoute les poignées manquantes.
			while ( this.handles.Count < total )
			{
				this.HandleAdd(pp1, HandleType.Secondary);
			}

			if ( r1 != -1 )
			{
				this.Handle(r1).Position = pp1;
				this.Handle(r1).IsSelected = this.Handle(0).IsSelected;
			}

			if ( r2 != -1 )
			{
				this.Handle(r2).Position = pp2;
				this.Handle(r2).IsSelected = this.Handle(this.TotalHandlePrimary-1).IsSelected;
			}
		}

		// Retourne le rang d'une poignée secondaire.
		protected int HandleArrowRank(int extremity)
		{
			if ( this.PropertyArrow(2).GetArrowType(extremity) == ArrowType.Right )  return -1;

			int total = this.TotalHandlePrimary;
			if ( extremity == 0 )
			{
				return total;
			}
			else
			{
				if ( this.PropertyArrow(2).GetArrowType(0) == ArrowType.Right )  return total;
				return total+1;
			}
		}


		// Met à jour le rectangle englobant l'objet.
		protected override void UpdateBoundingBox()
		{
			Drawing.Path pathStart;  bool outlineStart, surfaceStart;
			Drawing.Path pathEnd;    bool outlineEnd,   surfaceEnd;
			Drawing.Path pathLine;
			this.PathBuild(out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine);

			Drawing.Rectangle bboxStart = AbstractObject.ComputeBoundingBox(pathStart);
			Drawing.Rectangle bboxEnd   = AbstractObject.ComputeBoundingBox(pathEnd);
			Drawing.Rectangle bboxLine  = AbstractObject.ComputeBoundingBox(pathLine);

			this.bboxThin = bboxLine;
			this.bboxThin.MergeWith(this.Handle(0).Position);
			this.bboxThin.MergeWith(this.Handle(this.TotalHandlePrimary-1).Position);

			this.PropertyLine(0).InflateBoundingBox(ref bboxLine);
			this.bboxGeom = bboxLine;

			if ( outlineStart )  this.PropertyLine(0).InflateBoundingBox(ref bboxStart);
			this.bboxGeom.MergeWith(bboxStart);

			if ( outlineEnd )  this.PropertyLine(0).InflateBoundingBox(ref bboxEnd);
			this.bboxGeom.MergeWith(bboxEnd);

			this.bboxGeom.MergeWith(this.bboxThin);
			this.bboxFull = this.bboxGeom;

			this.bboxGeom.MergeWith(this.PropertyGradient(3).BoundingBoxGeom(this.bboxThin));
			this.bboxFull.MergeWith(this.PropertyGradient(3).BoundingBoxFull(this.bboxThin));
			this.bboxFull.MergeWith(this.bboxGeom);

			if ( this.tempLine != null )
			{
				this.bboxFull.MergeWith(this.tempLine.BoundingBox);
			}
		}

		// Retourne le nombre de poignées principales.
		// Ne compte pas les 1 ou 2 poignées secondaires à la fin, utilisées
		// pour PropertyArrow.
		protected int TotalHandlePrimary
		{
			get
			{
				int total = this.handles.Count;
				while ( total > 0 && this.Handle(total-1).Type == HandleType.Secondary )
				{
					total --;
				}
				return total;
			}
		}

		// Crée les chemins de l'objet.
		protected void PathBuild(out Drawing.Path pathStart, out bool outlineStart, out bool surfaceStart,
								 out Drawing.Path pathEnd,   out bool outlineEnd,   out bool surfaceEnd,
								 out Drawing.Path pathLine)
		{
			pathStart = new Drawing.Path();
			pathEnd   = new Drawing.Path();
			pathLine  = new Drawing.Path();

			int total = this.TotalHandlePrimary;
			if ( total < 2 )
			{
				outlineStart = false;
				surfaceStart = false;
				outlineEnd   = false;
				surfaceEnd   = false;
				return;
			}

			Drawing.Point p1, p2, pp1, pp2, s;
			double w = this.PropertyLine(0).Width;
			Drawing.CapStyle cap = this.PropertyLine(0).Cap;
			p1 = this.Handle(0).Position;
			p2 = this.Handle(1).Position;
			pp1 = this.PropertyArrow(2).PathExtremity(pathStart, 0, w,cap, p1,p2, out outlineStart, out surfaceStart);
			p1 = this.Handle(total-1).Position;
			p2 = this.Handle(total-2).Position;
			pp2 = this.PropertyArrow(2).PathExtremity(pathEnd,   1, w,cap, p1,p2, out outlineEnd,   out surfaceEnd);

			bool close = ( this.PropertyBool(4).Bool && total > 2 );
			PropertyCorner corner = this.PropertyCorner(5);
			if ( corner.CornerType == CornerType.Right )  // coins droits ?
			{
				for ( int i=0 ; i<total ; i++ )
				{
					p1 = this.Handle(i).Position;

					if ( i == 0 )  // premier point ?
					{
						pathLine.MoveTo(pp1);
					}
					else if ( i < total-1 )  // point intermédiaire ?
					{
						pathLine.LineTo(p1);
					}
					else	// dernier point ?
					{
						pathLine.LineTo(pp2);
					}
				}
				if ( close )
				{
					pathLine.LineTo(pp1);
					pathLine.Close();
				}
			}
			else	// coins spéciaux ?
			{
				for ( int i=0 ; i<total ; i++ )
				{
					p1 = this.Handle(i).Position;
					int prev = i-1;  if ( prev < 0 )  prev = total-1;
					int next = i+1;  if ( next >= total )  next = 0;
					bool simply = ( this.Handle(i).ConstrainType == HandleConstrainType.Simply );

					if ( i == 0 )  // premier point ?
					{
						if ( outlineStart || surfaceStart || !close )
						{
							pathLine.MoveTo(pp1);
						}
						else
						{
							p1 = this.Handle(prev).Position;
							s  = this.Handle(i).Position;
							p2 = this.Handle(next).Position;
							this.PathCorner(pathLine, p1,s,p2, corner, simply);
						}
					}
					else if ( i < total-1 )  // point intermédiaire ?
					{
						p1 = this.Handle(prev).Position;
						s  = this.Handle(i).Position;
						p2 = this.Handle(next).Position;
						this.PathCorner(pathLine, p1,s,p2, corner, simply);
					}
					else	// dernier point ?
					{
						if ( outlineEnd || surfaceEnd || !close )
						{
							pathLine.LineTo(pp2);
						}
						else
						{
							p1 = this.Handle(prev).Position;
							s  = this.Handle(i).Position;
							p2 = this.Handle(next).Position;
							this.PathCorner(pathLine, p1,s,p2, corner, simply);
						}
					}
				}
				if ( close )
				{
					pathLine.Close();
				}
			}
		}

		// Crée le chemin d'un coin.
		protected void PathCorner(Drawing.Path path, Drawing.Point p1, Drawing.Point s, Drawing.Point p2, PropertyCorner corner, bool simply)
		{
			if ( simply )
			{
				if ( path.IsEmpty )  path.MoveTo(s);
				else                 path.LineTo(s);
			}
			else
			{
				double l1 = Drawing.Point.Distance(p1, s);
				double l2 = Drawing.Point.Distance(p2, s);
				double radius = System.Math.Min(corner.Radius, System.Math.Min(l1,l2)/2);
				Drawing.Point c1 = Drawing.Point.Move(s, p1, radius);
				Drawing.Point c2 = Drawing.Point.Move(s, p2, radius);
				if ( path.IsEmpty )  path.MoveTo(c1);
				else                 path.LineTo(c1);
				corner.PathCorner(path, c1,s,c2, radius);
			}
		}

		// Dessine l'objet.
		public override void DrawGeometry(Drawing.Graphics graphics, IconContext iconContext)
		{
			if ( this.isHide )  return;
			base.DrawGeometry(graphics, iconContext);

			if ( this.TotalHandle < 1 )  return;

			Drawing.Path pathStart;  bool outlineStart, surfaceStart;
			Drawing.Path pathEnd;    bool outlineEnd,   surfaceEnd;
			Drawing.Path pathLine;
			this.PathBuild(out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine);

			this.PropertyGradient(3).Render(graphics, iconContext, pathLine, this.BoundingBoxThin);

			if ( outlineStart )
			{
				graphics.Rasterizer.AddOutline(pathStart, this.PropertyLine(0).Width, this.PropertyLine(0).Cap, this.PropertyLine(0).Join, this.PropertyLine(0).Limit);
				graphics.RenderSolid(iconContext.AdaptColor(this.PropertyColor(1).Color));
			}
			if ( surfaceStart )
			{
				graphics.Rasterizer.AddSurface(pathStart);
				graphics.RenderSolid(iconContext.AdaptColor(this.PropertyColor(1).Color));
			}

			if ( outlineEnd )
			{
				graphics.Rasterizer.AddOutline(pathEnd, this.PropertyLine(0).Width, this.PropertyLine(0).Cap, this.PropertyLine(0).Join, this.PropertyLine(0).Limit);
				graphics.RenderSolid(iconContext.AdaptColor(this.PropertyColor(1).Color));
			}
			if ( surfaceEnd )
			{
				graphics.Rasterizer.AddSurface(pathEnd);
				graphics.RenderSolid(iconContext.AdaptColor(this.PropertyColor(1).Color));
			}

			graphics.Rasterizer.AddOutline(pathLine, this.PropertyLine(0).Width, this.PropertyLine(0).Cap, this.PropertyLine(0).Join, this.PropertyLine(0).Limit);
			graphics.RenderSolid(iconContext.AdaptColor(this.PropertyColor(1).Color));

			if ( this.IsHilite && iconContext.IsEditable )
			{
				if ( this.PropertyGradient(3).IsVisible() )
				{
					graphics.Rasterizer.AddSurface(pathLine);
					graphics.RenderSolid(iconContext.HiliteSurfaceColor);
				}

				if ( outlineStart )
				{
					graphics.Rasterizer.AddOutline(pathStart, this.PropertyLine(0).Width, this.PropertyLine(0).Cap, this.PropertyLine(0).Join, this.PropertyLine(0).Limit);
					graphics.RenderSolid(iconContext.HiliteOutlineColor);
				}
				if ( surfaceStart )
				{
					graphics.Rasterizer.AddSurface(pathStart);
					graphics.RenderSolid(iconContext.HiliteOutlineColor);
				}

				if ( outlineEnd )
				{
					graphics.Rasterizer.AddOutline(pathEnd, this.PropertyLine(0).Width, this.PropertyLine(0).Cap, this.PropertyLine(0).Join, this.PropertyLine(0).Limit);
					graphics.RenderSolid(iconContext.HiliteOutlineColor);
				}
				if ( surfaceEnd )
				{
					graphics.Rasterizer.AddSurface(pathEnd);
					graphics.RenderSolid(iconContext.HiliteOutlineColor);
				}

				graphics.Rasterizer.AddOutline(pathLine, this.PropertyLine(0).Width+iconContext.HiliteSize, this.PropertyLine(0).Cap, this.PropertyLine(0).Join, this.PropertyLine(0).Limit);
				graphics.RenderSolid(iconContext.HiliteOutlineColor);
			}

			if ( this.tempLine != null )
			{
				this.tempLine.DrawGeometry(graphics, iconContext);
			}
		}


		protected bool				mouseDown = false;
		protected ObjectLine		tempLine;
	}
}

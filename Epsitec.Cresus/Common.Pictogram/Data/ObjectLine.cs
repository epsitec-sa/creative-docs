using Epsitec.Common.Support;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe ObjectLine est la classe de l'objet graphique "segment de ligne".
	/// </summary>
	public class ObjectLine : AbstractObject
	{
		public ObjectLine()
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
		}

		protected override AbstractObject CreateNewObject()
		{
			return new ObjectLine();
		}

		public override void Dispose()
		{
			if ( this.ExistProperty(2) )  this.PropertyArrow(2).Changed -= new EventHandler(this.HandleChanged);
			base.Dispose();
		}


		// Nom de l'icône.
		public override string IconName
		{
			get { return @"file:images/line.icon"; }
		}


		// Détecte si la souris est sur l'objet.
		public override bool Detect(Drawing.Point pos)
		{
			if ( this.isHide )  return false;

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

			if ( rank == 0 )  // p1 ?
			{
				this.Handle(0).Position = pos;
			}
			else if ( rank == 1 )  // p2 ?
			{
				this.Handle(1).Position = pos;
			}
			else if ( rank == this.HandleArrowRank(0) )  // pp1 ?
			{
				double d = Drawing.Point.Distance(this.Handle(0).Position, pos);
				this.PropertyArrow(2).Length1 = d;
			}
			else if ( rank == this.HandleArrowRank(1) )  // pp2 ?
			{
				double d = Drawing.Point.Distance(this.Handle(1).Position, pos);
				this.PropertyArrow(2).Length2 = d;
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
			return ( rank >= 2 );
		}

		// Retourne la propriété modifiée en déplaçant une poignée.
		public override AbstractProperty MoveHandleProperty(int rank)
		{
			if ( rank >= this.handles.Count )  // poignée d'une propriété ?
			{
				return base.MoveHandleProperty(rank);
			}
			if ( rank >= 2 )  return this.PropertyArrow(2);
			return null;
		}


		// Déplace globalement l'objet.
		public override void MoveGlobal(GlobalModifierData initial, GlobalModifierData final, bool all)
		{
			base.MoveGlobal(initial, final, all);
			this.UpdateHandle();
		}


		// Début de la création d'un objet.
		public override void CreateMouseDown(Drawing.Point pos, IconContext iconContext)
		{
			iconContext.ConstrainFixStarting(pos);
			this.HandleAdd(pos, HandleType.Primary);
			this.HandleAdd(pos, HandleType.Primary);
		}

		// Déplacement pendant la création d'un objet.
		public override void CreateMouseMove(Drawing.Point pos, IconContext iconContext)
		{
			iconContext.ConstrainSnapPos(ref pos);
			this.Handle(1).Position = pos;
			this.dirtyBbox = true;
		}

		// Fin de la création d'un objet.
		public override void CreateMouseUp(Drawing.Point pos, IconContext iconContext)
		{
			iconContext.ConstrainSnapPos(ref pos);
			this.Handle(1).Position = pos;
			iconContext.ConstrainDelStarting();
			this.UpdateHandle();
		}

		// Indique si l'objet doit exister. Retourne false si l'objet ne peut
		// pas exister et doit être détruit.
		public override bool CreateIsExist(IconContext iconContext)
		{
			double len = Drawing.Point.Distance(this.Handle(0).Position, this.Handle(1).Position);
			return ( len > this.minimalSize );
		}


		private void HandleChanged(object sender)
		{
			this.UpdateHandle();
		}

		// Met à jour les poignées pour les profondeurs des flèches.
		protected void UpdateHandle()
		{
			if ( this.handles.Count < 2 )  return;

			Drawing.Point p1 = this.Handle(0).Position;
			Drawing.Point p2 = this.Handle(1).Position;
			Drawing.Point pp1 = Drawing.Point.Move(p1, p2, this.PropertyArrow(2).GetLength(0));
			Drawing.Point pp2 = Drawing.Point.Move(p2, p1, this.PropertyArrow(2).GetLength(1));
			int r1 = this.HandleArrowRank(0);
			int r2 = this.HandleArrowRank(1);
			int total = 2 + ((r1==-1)?0:1) + ((r2==-1)?0:1);

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
				this.Handle(r2).IsSelected = this.Handle(1).IsSelected;
			}
		}

		// Retourne le rang d'une poignée secondaire.
		// extremity = 0: poignée proche de p1 (this.Handle(0))
		// extremity = 1: poignée proche de p2 (this.Handle(2))
		protected int HandleArrowRank(int extremity)
		{
			if ( this.PropertyArrow(2).GetArrowType(extremity) == ArrowType.Right )  return -1;

			if ( extremity == 0 )
			{
				return 2;
			}
			else
			{
				if ( this.PropertyArrow(2).GetArrowType(0) == ArrowType.Right )  return 2;
				return 3;
			}
		}


		// Met à jour le rectangle englobant l'objet.
		protected override void UpdateBoundingBox()
		{
			this.bboxThin = Drawing.Rectangle.FromCorners(this.Handle(0).Position, this.Handle(1).Position);

			Drawing.Path pathStart;  bool outlineStart, surfaceStart;
			Drawing.Path pathEnd;    bool outlineEnd,   surfaceEnd;
			Drawing.Path pathLine;
			this.PathBuild(out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine);

			Drawing.Rectangle bboxStart = AbstractObject.ComputeBoundingBox(pathStart);
			Drawing.Rectangle bboxEnd   = AbstractObject.ComputeBoundingBox(pathEnd);
			Drawing.Rectangle bboxLine  = AbstractObject.ComputeBoundingBox(pathLine);

			this.PropertyLine(0).InflateBoundingBox(ref bboxLine);
			this.bboxGeom = bboxLine;

			if ( outlineStart )  this.PropertyLine(0).InflateBoundingBox(ref bboxStart);
			this.bboxGeom.MergeWith(bboxStart);

			if ( outlineEnd )  this.PropertyLine(0).InflateBoundingBox(ref bboxEnd);
			this.bboxGeom.MergeWith(bboxEnd);

			this.bboxGeom.MergeWith(this.bboxThin);
			this.bboxFull = this.bboxGeom;
		}

		// Crée les 3 chemins de l'objet.
		protected void PathBuild(out Drawing.Path pathStart, out bool outlineStart, out bool surfaceStart,
								 out Drawing.Path pathEnd,   out bool outlineEnd,   out bool surfaceEnd,
								 out Drawing.Path pathLine)
		{
			pathStart = new Drawing.Path();
			pathEnd   = new Drawing.Path();
			pathLine  = new Drawing.Path();

			Drawing.Point p1 = this.Handle(0).Position;
			Drawing.Point p2 = this.Handle(1).Position;
			double w = this.PropertyLine(0).Width;
			Drawing.CapStyle cap = this.PropertyLine(0).Cap;
			Drawing.Point pp1 = this.PropertyArrow(2).PathExtremity(pathStart, 0, w,cap, p1,p2, out outlineStart, out surfaceStart);
			Drawing.Point pp2 = this.PropertyArrow(2).PathExtremity(pathEnd,   1, w,cap, p2,p1, out outlineEnd,   out surfaceEnd);

			pathLine.MoveTo(pp1);
			pathLine.LineTo(pp2);
		}

		// Dessine l'objet.
		public override void DrawGeometry(Drawing.Graphics graphics, IconContext iconContext)
		{
			if ( this.isHide )  return;
			base.DrawGeometry(graphics, iconContext);

			if ( this.TotalHandle < 2 )  return;

			Drawing.Path pathStart;  bool outlineStart, surfaceStart;
			Drawing.Path pathEnd;    bool outlineEnd,   surfaceEnd;
			Drawing.Path pathLine;
			this.PathBuild(out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine);

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
				if ( outlineStart )
				{
					graphics.Rasterizer.AddOutline(pathStart, this.PropertyLine(0).Width+iconContext.HiliteSize, this.PropertyLine(0).Cap, this.PropertyLine(0).Join, this.PropertyLine(0).Limit);
					graphics.RenderSolid(iconContext.HiliteOutlineColor);
				}
				if ( surfaceStart )
				{
					graphics.Rasterizer.AddSurface(pathStart);
					graphics.RenderSolid(iconContext.HiliteOutlineColor);
				}

				if ( outlineEnd )
				{
					graphics.Rasterizer.AddOutline(pathEnd, this.PropertyLine(0).Width+iconContext.HiliteSize, this.PropertyLine(0).Cap, this.PropertyLine(0).Join, this.PropertyLine(0).Limit);
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
		}
	}
}

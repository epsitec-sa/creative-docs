using System.Collections.Generic;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Objects
{
	/// <summary>
	/// La classe Line est la classe de l'objet graphique "segment de ligne".
	/// </summary>
	[System.Serializable()]
	public class Line : Objects.Abstract
	{
		public Line(Document document, Objects.Abstract model) : this(document, model, false)
		{
		}

		public Line(Document document, Objects.Abstract model, bool floating) : base(document, model)
		{
			if ( this.document == null )  return;  // objet factice ?
			this.CreateProperties(model, floating);
		}

		protected override bool ExistingProperty(Properties.Type type)
		{
			if ( type == Properties.Type.Name )  return true;
			if ( type == Properties.Type.LineMode )  return true;
			if ( type == Properties.Type.LineColor )  return true;
			if ( type == Properties.Type.Arrow )  return true;
			return false;
		}

		protected override Objects.Abstract CreateNewObject(Document document, Objects.Abstract model)
		{
			return new Line(document, model);
		}

		public override void Dispose()
		{
			base.Dispose();
		}


		public override string IconUri
		{
			//	Nom de l'icône.
			get { return Misc.Icon("ObjectLine"); }
		}


		public override void MoveHandleStarting(int rank, Point pos, DrawingContext drawingContext)
		{
			//	Début du déplacement d'une poignée.
			base.MoveHandleStarting(rank, pos, drawingContext);

			if ( rank < this.handles.Count )  // poignée de l'objet ?
			{
				drawingContext.ConstrainClear();

				Handle handle = this.Handle(rank);
				if ( handle.PropertyType == Properties.Type.None )
				{
					drawingContext.ConstrainAddHV(this.Handle(0).Position, false, -1);
					drawingContext.ConstrainAddHV(this.Handle(1).Position, false, -1);
					drawingContext.ConstrainAddLine(this.Handle(0).Position, this.Handle(1).Position, false, -1);
					if ( rank == 0 || rank == 1 )
					{
						drawingContext.ConstrainAddCircle(this.Handle(rank^1).Position, this.Handle(rank).Position, false, -1);
					}
				}
				else
				{
					Properties.Abstract property = this.Property(handle.PropertyType);
					property.MoveHandleStarting(this, handle.PropertyRank, pos, drawingContext);
				}

				if ( rank == 0 )
				{
					drawingContext.MagnetFixStarting(this.Handle(1).Position);
				}
				else if ( rank == 1 )
				{
					drawingContext.MagnetFixStarting(this.Handle(0).Position);
				}
				else
				{
					drawingContext.MagnetClearStarting();
				}
			}
		}

		public override void MoveHandleProcess(int rank, Point pos, DrawingContext drawingContext)
		{
			//	Déplace une poignée.
			if ( rank >= 2 )  // poignée d'une propriété ?
			{
				base.MoveHandleProcess(rank, pos, drawingContext);
				return;
			}

			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapPos(ref pos);

			if ( rank == 0 )  // p1 ?
			{
				this.Handle(0).Position = pos;
			}
			else if ( rank == 1 )  // p2 ?
			{
				this.Handle(1).Position = pos;
			}
			this.HandlePropertiesUpdate();
			this.SetDirtyBbox();
			this.TextInfoModifLine();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}


		public override void MoveGlobalProcess(Selector selector)
		{
			//	Déplace globalement l'objet.
			base.MoveGlobalProcess(selector);
			this.HandlePropertiesUpdate();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}


		public override void CreateMouseDown(Point pos, DrawingContext drawingContext)
		{
			//	Début de la création d'un objet.
			drawingContext.ConstrainClear();
			drawingContext.ConstrainAddHV(pos, false, -1);
			if ( this.handles.Count == 0 )
			{
				this.HandleAdd(pos, HandleType.Primary);
				this.HandleAdd(pos, HandleType.Primary);
				drawingContext.MagnetFixStarting(pos);
			}
			else
			{
				this.Handle(0).Position = pos;
				this.Handle(1).Position = pos;
			}
			this.isCreating = true;
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override void CreateMouseMove(Point pos, DrawingContext drawingContext)
		{
			//	Déplacement pendant la création d'un objet.
			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapPos(ref pos);
			this.Handle(1).Position = pos;
			this.SetDirtyBbox();
			this.TextInfoModifLine();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override void CreateMouseUp(Point pos, DrawingContext drawingContext)
		{
			//	Fin de la création d'un objet.
			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapPos(ref pos);
			this.Handle(1).Position = pos;
			drawingContext.ConstrainDelStarting();
			drawingContext.MagnetClearStarting();
			this.isCreating = false;
			this.document.Modifier.TextInfoModif = "";

			this.HandlePropertiesCreate();
			this.HandlePropertiesUpdate();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override bool CreateIsExist(DrawingContext drawingContext)
		{
			//	Indique si l'objet doit exister. Retourne false si l'objet ne peut
			//	pas exister et doit être détruit.
			double len = Point.Distance(this.Handle(0).Position, this.Handle(1).Position);
			return ( len > drawingContext.MinimalSize );
		}


		public override Shape[] ShapesBuild(IPaintPort port, DrawingContext drawingContext, bool simplify)
		{
			//	Constuit les formes de l'objet.
			Path pathStart, pathEnd, pathLine;
			bool outlineStart, outlineEnd, surfaceStart, surfaceEnd;
			this.PathBuild(drawingContext,
						   out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine);

			int totalShapes = 1;
			if ( surfaceStart )  totalShapes ++;
			if ( surfaceEnd   )  totalShapes ++;
			if ( outlineStart )  totalShapes ++;
			if ( outlineEnd   )  totalShapes ++;
			
			Shape[] shapes = new Shape[totalShapes];
			int i = 0;
			
			//	Forme du chemin principal.
			shapes[i] = new Shape();
			shapes[i].Path = pathLine;
			shapes[i].SetPropertyStroke(port, this.PropertyLineMode, this.PropertyLineColor);
			i ++;

			//	Forme de la surface de départ.
			if ( surfaceStart )
			{
				shapes[i] = new Shape();
				shapes[i].Path = pathStart;
				shapes[i].SetPropertySurface(port, this.PropertyLineColor);
				shapes[i].IsMisc = true;
				i ++;
			}

			//	Forme de la surface d'arrivée.
			if ( surfaceEnd )
			{
				shapes[i] = new Shape();
				shapes[i].Path = pathEnd;
				shapes[i].SetPropertySurface(port, this.PropertyLineColor);
				shapes[i].IsMisc = true;
				i ++;
			}

			//	Forme du chemin de départ.
			if ( outlineStart )
			{
				shapes[i] = new Shape();
				shapes[i].Path = pathStart;
				shapes[i].SetPropertyStroke(port, this.PropertyLineMode, this.PropertyLineColor);
				shapes[i].IsMisc = true;
				i ++;
			}

			//	Forme du chemin d'arrivée.
			if ( outlineEnd )
			{
				shapes[i] = new Shape();
				shapes[i].Path = pathEnd;
				shapes[i].SetPropertyStroke(port, this.PropertyLineMode, this.PropertyLineColor);
				shapes[i].IsMisc = true;
				i ++;
			}

			return shapes;
		}

		protected void PathBuild(DrawingContext drawingContext,
								 out Path pathStart, out bool outlineStart, out bool surfaceStart,
								 out Path pathEnd,   out bool outlineEnd,   out bool surfaceEnd,
								 out Path pathLine)
		{
			//	Crée les 3 chemins de l'objet.
			pathStart = new Path();
			pathEnd   = new Path();
			pathLine  = new Path();

			double zoom = Properties.Abstract.DefaultZoom(drawingContext);
			pathStart.DefaultZoom = zoom;
			pathEnd.DefaultZoom = zoom;
			pathLine.DefaultZoom = zoom;

			Point p1 = this.Handle(0).Position;
			Point p2 = this.Handle(1).Position;
			double w = this.PropertyLineMode.Width;
			CapStyle cap = this.PropertyLineMode.Cap;
			Point pp1 = this.PropertyArrow.PathExtremity(pathStart, 0, w,cap, p1,p2, false, out outlineStart, out surfaceStart);
			Point pp2 = this.PropertyArrow.PathExtremity(pathEnd,   1, w,cap, p2,p1, false, out outlineEnd,   out surfaceEnd);

			pathLine.MoveTo(pp1);
			pathLine.LineTo(pp2);
		}


		public override Path GetMagnetPath()
		{
			//	Retourne le chemin géométrique de l'objet pour les constructions
			//	magnétiques.
			Path path = new Path();

			path.MoveTo(this.Handle(0).Position);
			path.LineTo(this.Handle(1).Position);

			return path;
		}

		protected override Path GetPath()
		{
			//	Retourne le chemin géométrique de l'objet.
			Path pathStart, pathEnd, pathLine;
			bool outlineStart, outlineEnd, surfaceStart, surfaceEnd;
			this.PathBuild(null,
						   out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine);

			if ( outlineStart || surfaceStart )
			{
				pathLine.Append(pathStart, 0.0);
			}

			if ( outlineEnd || surfaceEnd )
			{
				pathLine.Append(pathEnd, 0.0);
			}

			return pathLine;
		}

		public void CreateFromPoints(Point p1, Point p2)
		{
			//	Crée une ligne à partir de 2 points.
			this.HandleAdd(p1, HandleType.Primary);
			this.HandleAdd(p2, HandleType.Primary);
			this.SetDirtyBbox();
		}


		#region Serialization
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise l'objet.
			base.GetObjectData(info, context);
		}

		protected Line(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui désérialise l'objet.
		}
		#endregion
	}
}

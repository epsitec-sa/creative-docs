using System.Collections.Generic;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Objects
{
	/// <summary>
	/// La classe Free est la classe de l'objet graphique "trait à main levée".
	/// </summary>
	[System.Serializable()]
	public class Free : Objects.Abstract
	{
		public Free(Document document, Objects.Abstract model) : this(document, model, false)
		{
		}

		public Free(Document document, Objects.Abstract model, bool floating) : base(document, model)
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
			if ( type == Properties.Type.FillGradient )  return true;
			if ( type == Properties.Type.PolyClose )  return true;
			return false;
		}

		protected override Objects.Abstract CreateNewObject(Document document, Objects.Abstract model)
		{
			return new Free(document, model);
		}

		public override void Dispose()
		{
			base.Dispose();
		}


		public override string IconName
		{
			//	Nom de l'icône.
			get { return Misc.Icon("ObjectFree"); }
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
					drawingContext.ConstrainAddHV(this.Handle(0).Position);
					drawingContext.ConstrainAddHV(this.Handle(1).Position);
					drawingContext.ConstrainAddLine(this.Handle(0).Position, this.Handle(1).Position);
					if ( rank == 0 || rank == 1 )
					{
						drawingContext.ConstrainAddCircle(this.Handle(rank^1).Position, this.Handle(rank).Position);
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


		public override void MoveSelectedSegmentStarting(int rank, Point pos, DrawingContext drawingContext)
		{
			//	Début du déplacement d'une poignée d'un segment sélectionné.
			base.MoveSelectedSegmentStarting(rank, pos, drawingContext);

			this.initialPos = pos;
			SelectedSegment ss = this.selectedSegments[rank] as SelectedSegment;
			int r = ss.Rank;
			int n = ss.Rank+1;
			this.Handle(r).InitialPosition = this.Handle(r).Position;
			this.Handle(n).InitialPosition = this.Handle(n).Position;

			drawingContext.ConstrainClear();
			drawingContext.ConstrainAddHV(ss.Position);
			Point p1 = this.Handle(r).Position;
			Point p2 = this.Handle(n).Position;
			Size d = new Size(p2.X-p1.X, p2.Y-p1.Y);
			drawingContext.ConstrainAddLine(p1, p2);
			drawingContext.ConstrainAddLine(pos, new Point(pos.X-d.Height, pos.Y+d.Width));
		}

		public override void MoveSelectedSegmentProcess(int rank, Point pos, DrawingContext drawingContext)
		{
			//	Déplace une poignée d'un segment sélectionné.
			this.document.Notifier.NotifyArea(this.BoundingBox);

			drawingContext.SnapPos(ref pos);
			Point move = pos-this.initialPos;

			SelectedSegment ss = this.selectedSegments[rank] as SelectedSegment;
			int r = ss.Rank;
			int n = ss.Rank+1;
			this.Handle(r).Position = this.Handle(r).InitialPosition+move;
			this.Handle(n).Position = this.Handle(n).InitialPosition+move;
			SelectedSegment.Update(this.selectedSegments, this);

			this.SetDirtyBbox();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override void MoveSelectedSegmentEnding(int rank, Point pos, DrawingContext drawingContext)
		{
			//	Fin du déplacement d'une poignée d'un segment sélectionné.
			base.MoveSelectedSegmentEnding(rank, pos, drawingContext);
		}

		
		public override void MoveGlobalProcess(Selector selector)
		{
			//	Déplace globalement l'objet.
			base.MoveGlobalProcess(selector);
			this.HandlePropertiesUpdate();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}


		public override bool IsSelectedSegmentPossible
		{
			//	Indique si cet objet peut avoir des segments sélectionnés.
			get { return true; }
		}

		public override bool ShaperHandleState(string family, ref bool enable, System.Collections.ArrayList actives)
		{
			//	Donne l'état d'une commande ShaperHandle*.
			if ( family == "Add" )
			{
				enable = (this.selectedSegments != null && this.selectedSegments.Count != 0);
				return true;
			}

			if ( family == "Sub" )
			{
				enable = (this.TotalMainHandle-this.TotalShaperHandleSelected() >= 2 && this.IsShaperHandleSelected());
				return true;
			}

			return base.ShaperHandleState(family, ref enable, actives);
		}

		public override bool ShaperHandleCommand(string cmd)
		{
			//	Exécute une commande ShaperHandle*.
			if ( cmd == "ShaperHandleAdd" )
			{
				this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.ShaperHandleAdd);
				this.InsertOpletGeometry();
				SelectedSegment.InsertOpletGeometry(this.selectedSegments, this);
				this.document.Notifier.NotifyArea(this.BoundingBox);

				if ( this.selectedSegments != null )
				{
					int[] index = SelectedSegment.Sort(this.selectedSegments);
					for ( int i=0 ; i<index.Length ; i++ )
					{
						SelectedSegment ss = this.selectedSegments[index[i]] as SelectedSegment;
						this.ShaperHandleAdd(ss.Position, ss.Rank);
					}
				}
				this.SelectedSegmentClear();

				this.document.Notifier.NotifyArea(this.BoundingBox);
				this.document.Modifier.OpletQueueValidateAction();
				return true;
			}

			if ( cmd == "ShaperHandleSub" )
			{
				this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.ShaperHandleSub);
				this.InsertOpletGeometry();
				this.document.Notifier.NotifyArea(this.BoundingBox);

				for ( int i=this.TotalMainHandle-1 ; i>=0 ; i-- )
				{
					if ( !this.Handle(i).IsVisible )  continue;
					if ( this.Handle(i).IsShaperDeselected )  continue;
					this.ShaperHandleSub(i);
				}

				this.document.Notifier.NotifyArea(this.BoundingBox);
				this.document.Modifier.OpletQueueValidateAction();
				return true;
			}

			return base.ShaperHandleCommand(cmd);
		}

		protected void ShaperHandleAdd(Point pos, int rank)
		{
			//	Ajoute une poignée sans changer l'aspect.
			int next = rank+1;
			Point p = Point.Projection(this.Handle(rank).Position, this.Handle(next).Position, pos);

			Handle handle = new Handle(this.document);
			handle.Position = p;
			handle.Type = HandleType.Primary;
			handle.IsVisible = true;
			this.HandleInsert(rank+1, handle);
			this.HandlePropertiesUpdate();
		}

		protected void ShaperHandleSub(int rank)
		{
			//	Supprime une poignée sans trop changer l'aspect.
			bool starting = (this.Handle(rank).Type == HandleType.Starting);
			this.HandleDelete(rank);

			//	Il doit toujours y avoir une poignée de départ !
			if ( starting )
			{
				this.Handle(rank).Type = HandleType.Starting;
			}
			this.HandlePropertiesUpdate();
		}


		public override void CreateMouseDown(Point pos, DrawingContext drawingContext)
		{
			//	Début de la création d'un objet.
			this.HandleAdd(pos, HandleType.Primary);
			this.isCreating = true;
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override void CreateMouseMove(Point pos, DrawingContext drawingContext)
		{
			//	Déplacement pendant la création d'un objet.
			this.document.Notifier.NotifyArea(this.BoundingBox);

			if (this.TotalHandle == 1)
			{
				this.HandleAdd(pos, HandleType.Primary);
			}
			else
			{
				double len = Point.Distance(this.Handle(this.TotalHandle-1).Position, this.Handle(this.TotalHandle-2).Position);
				if (len < Free.spacing)
				{
					this.Handle(this.TotalHandle-1).Position = pos;
				}
				else
				{
					this.HandleAdd(pos, HandleType.Primary);
				}
			}

			this.SetDirtyBbox();
			this.TextInfoModifLine();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override void CreateMouseUp(Point pos, DrawingContext drawingContext)
		{
			//	Fin de la création d'un objet.
#if false
			this.document.Notifier.NotifyArea(this.BoundingBox);
			
			if (this.TotalHandle > 2)
			{
				this.HandleDelete(this.TotalHandle-1);
				this.Handle(this.TotalHandle-1).Position = pos;
				this.SetDirtyBbox();
				this.document.Notifier.NotifyArea(this.BoundingBox);
			}
#endif

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
			return this.TotalMainHandle >= 2;
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

			int totalShapes = 2;
			if ( surfaceStart )  totalShapes ++;
			if ( surfaceEnd   )  totalShapes ++;
			if ( outlineStart )  totalShapes ++;
			if ( outlineEnd   )  totalShapes ++;
			
			Shape[] shapes = new Shape[totalShapes];
			int i = 0;
			
			//	Forme de la surface principale.
			shapes[i] = new Shape();
			shapes[i].Path = pathLine;
			shapes[i].SetPropertySurface(port, this.PropertyFillGradient);
			i ++;

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

			int total = this.TotalMainHandle;
			if (total <= 2)
			{
				for (int i=0; i<total; i++)
				{
					Point pos = this.Handle(i).Position;

					if (i == 0)
					{
						pathLine.MoveTo(pos);
					}
					else
					{
						pathLine.LineTo(pos);
					}
				}
			}
			else
			{
				if (this.PropertyPolyClose.BoolValue)
				{
					total++;
				}

				for (int i=0; i<total; i++)
				{
					if (i == 0)
					{
						Point p = this.Handle(i).Position;
						pathLine.MoveTo(p);
					}
					else
					{
						Point s1, s2;

						if (i == 1 && !this.PropertyPolyClose.BoolValue)
						{
#if true
							s1 = this.GetCyclingHandlePosition(i-1);
#else
							s1 = Point.Scale(this.GetCyclingHandlePosition(i), this.GetCyclingHandlePosition(i-1), Free.pressure);
#endif
						}
						else
						{
							s1 = this.ComputeSecondary(this.GetCyclingHandlePosition(i), this.GetCyclingHandlePosition(i-1), this.GetCyclingHandlePosition(i-2));
						}

						if (i == total-1 && !this.PropertyPolyClose.BoolValue)
						{
#if true
							s2 = this.GetCyclingHandlePosition(i);
#else
							s2 = Point.Scale(this.GetCyclingHandlePosition(i-1), this.GetCyclingHandlePosition(i), Free.pressure);
#endif
						}
						else
						{
							s2 = this.ComputeSecondary(this.GetCyclingHandlePosition(i-1), this.GetCyclingHandlePosition(i), this.GetCyclingHandlePosition(i+1));
						}
						
						pathLine.CurveTo(s1, s2, this.GetCyclingHandlePosition(i));
					}
				}
			}

			outlineStart = false;
			surfaceStart = false;
			outlineEnd = false;
			surfaceEnd = false;
		}

		private Point GetCyclingHandlePosition(int rank)
		{
			int total = this.TotalMainHandle;

			if (rank < 0)
			{
				rank = total+rank;
			}

			if (rank >= total)
			{
				rank = rank-total;
			}

			return this.Handle(rank).Position;
		}

		private Point ComputeSecondary(Point p1, Point p, Point p2)
		{
			return Point.Move(p, p+(p1-p2), Point.Distance(p,p1)*Free.pressure);
		}


		public override Path GetMagnetPath()
		{
			//	Retourne le chemin géométrique de l'objet pour les constructions
			//	magnétiques.
			Path pathStart, pathEnd, pathLine;
			bool outlineStart, outlineEnd, surfaceStart, surfaceEnd;
			this.PathBuild(null,
						   out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine);

			return pathLine;
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

		protected Free(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui désérialise l'objet.
		}
		#endregion


		private static readonly double	spacing = 200;
		private static readonly double	pressure = 0.33;

		protected Point					initialPos;
	}
}

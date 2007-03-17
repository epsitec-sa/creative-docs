using System.Collections.Generic;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Objects
{
	/// <summary>
	/// La classe Free est la classe de l'objet graphique "trait � main lev�e".
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
			if ( type == Properties.Type.Tension )  return true;
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
			//	Nom de l'ic�ne.
			get { return Misc.Icon("ObjectFree"); }
		}


		public override void MoveHandleStarting(int rank, Point pos, DrawingContext drawingContext)
		{
			//	D�but du d�placement d'une poign�e.
			base.MoveHandleStarting(rank, pos, drawingContext);

			if ( rank < this.handles.Count )  // poign�e de l'objet ?
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
			//	D�place une poign�e.
			if ( rank >= this.TotalMainHandle )  // poign�e d'une propri�t� ?
			{
				base.MoveHandleProcess(rank, pos, drawingContext);
				return;
			}

			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapPos(ref pos);

			this.Handle(rank).Position = pos;

			this.HandlePropertiesUpdate();
			this.SetDirtyBbox();
			this.TextInfoModif(pos, rank);
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}


		public override void MoveSelectedSegmentStarting(int rank, Point pos, DrawingContext drawingContext)
		{
			//	D�but du d�placement d'une poign�e d'un segment s�lectionn�.
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
			//	D�place une poign�e d'un segment s�lectionn�.
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
			//	Fin du d�placement d'une poign�e d'un segment s�lectionn�.
			base.MoveSelectedSegmentEnding(rank, pos, drawingContext);
		}

		
		public override void MoveGlobalProcess(Selector selector)
		{
			//	D�place globalement l'objet.
			base.MoveGlobalProcess(selector);
			this.HandlePropertiesUpdate();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}


		public override bool IsSelectedSegmentPossible
		{
			//	Indique si cet objet peut avoir des segments s�lectionn�s.
			get { return true; }
		}

		public override bool ShaperHandleState(string family, ref bool enable, System.Collections.ArrayList actives)
		{
			//	Donne l'�tat d'une commande ShaperHandle*.
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
			//	Ex�cute une commande ShaperHandle*.
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
			//	Ajoute une poign�e sans changer l'aspect.
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
			//	Supprime une poign�e sans trop changer l'aspect.
			bool starting = (this.Handle(rank).Type == HandleType.Starting);
			this.HandleDelete(rank);

			//	Il doit toujours y avoir une poign�e de d�part !
			if ( starting )
			{
				this.Handle(rank).Type = HandleType.Starting;
			}
			this.HandlePropertiesUpdate();
		}


		public override void CreateMouseDown(Point pos, DrawingContext drawingContext)
		{
			//	D�but de la cr�ation d'un objet.
			this.InitSpacing(drawingContext);
			this.ChangePropertyPolyClose(false);
			drawingContext.SnapPos(ref pos);
			this.HandleAdd(pos, HandleType.Starting);
			this.Handle(0).IsVisible = true;
			this.isCreating = true;
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override void CreateMouseMove(Point pos, DrawingContext drawingContext)
		{
			//	D�placement pendant la cr�ation d'un objet.
			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapPos(ref pos);

			if (this.TotalHandle == 1)
			{
				this.HandleAdd(pos, HandleType.Primary);
			}
			else
			{
				double len = Point.Distance(this.Handle(this.TotalHandle-1).Position, this.Handle(this.TotalHandle-2).Position);
				if (len < this.spacing)
				{
					this.Handle(this.TotalHandle-1).Position = pos;
				}
				else
				{
					this.HandleAdd(pos, HandleType.Primary);
				}
			}

			double startingLen = Point.Distance(this.Handle(0).Position, pos);
			if ( startingLen <= drawingContext.CloseMargin && this.TotalHandle > 2)
			{
				this.Handle(0).Type = HandleType.Ending;
			}
			else
			{
				this.Handle(0).Type = HandleType.Starting;
			}

			this.SetDirtyBbox();
			this.TextInfoModif(pos, this.TotalHandle-1);
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override void CreateMouseUp(Point pos, DrawingContext drawingContext)
		{
			//	Fin de la cr�ation d'un objet.
			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapPos(ref pos);

			int total = this.TotalHandle;
			if (total > 2)
			{
				Point p0 = this.Handle(total-1).Position;
				Point p1 = this.Handle(total-2).Position;
				Point p2 = this.Handle(total-3).Position;

				double d01 = Point.Distance(p0, p1);
				double d12 = Point.Distance(p1, p2);

				if (d01 < this.spacing*0.2)  // dernier point proche de l'avant-dernier ?
				{
					this.HandleDelete(total-1);
					this.Handle(total-2).Position = pos;
				}
				else if (d01 < this.spacing*0.8)  // dernier point � mi-chemin d'une nouvelle position ?
				{
					this.Handle(total-2).Position = Point.Move(p2, p1, (d01+d12)*0.5);
				}
				// Si le dernier point est proche d'une nouvelle position, on le laisse tel quel.

				this.SetDirtyBbox();
				this.document.Notifier.NotifyArea(this.BoundingBox);
			}

			total = this.TotalHandle;
			double len = Point.Distance(this.Handle(0).Position, pos);
			if (len <= drawingContext.CloseMargin && total > 2)  // trait ferm� ?
			{
				this.HandleDelete(total-1);
				this.ChangePropertyPolyClose(true);
			}
			this.Handle(0).Type = HandleType.Starting;
			this.Handle(0).IsVisible = false;
			
			this.isCreating = false;
			this.document.Modifier.TextInfoModif = "";

			this.HandlePropertiesCreate();
			this.HandlePropertiesUpdate();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override void CreateProcessMessage(Common.Widgets.Message message, Point pos)
		{
			//	Gestion du clavier pendant la cr�ation d'un objet.
			if (message.MessageType == Common.Widgets.MessageType.KeyDown)
			{
				double spacing = Free.radiusSpacing;

				if (message.KeyCode == Epsitec.Common.Widgets.KeyCode.ArrowUp   ||
					message.KeyCode == Epsitec.Common.Widgets.KeyCode.ArrowRight||
					message.KeyCode == Epsitec.Common.Widgets.KeyCode.ShiftKey  )
				{
					spacing += 20;
				}

				if (message.KeyCode == Epsitec.Common.Widgets.KeyCode.ArrowDown ||
					message.KeyCode == Epsitec.Common.Widgets.KeyCode.ArrowLeft ||
					message.KeyCode == Epsitec.Common.Widgets.KeyCode.ControlKey)
				{
					spacing -= 20;
				}

				if (message.KeyCode == Epsitec.Common.Widgets.KeyCode.Multiply||
					message.KeyCode == Epsitec.Common.Widgets.KeyCode.Decimal )
				{
					spacing = 200;
				}

				spacing = System.Math.Max(spacing, 20);
				spacing = System.Math.Min(spacing, 400);

				if (Free.radiusSpacing != spacing)
				{
					this.document.Notifier.NotifyArea(this.BoundingBox);

					Free.radiusSpacing = spacing;
					this.spacing = Free.radiusSpacing/this.zoom;
					
					this.SetDirtyBbox();
					this.document.Notifier.NotifyArea(this.BoundingBox);
				}
			}
		}

		public override bool CreateIsExist(DrawingContext drawingContext)
		{
			//	Indique si l'objet doit exister. Retourne false si l'objet ne peut
			//	pas exister et doit �tre d�truit.
			if (this.TotalMainHandle > 2)
			{
				return true;
			}

			if (this.TotalMainHandle == 2)
			{
				double len = Point.Distance(this.Handle(0).Position, this.Handle(1).Position);
				return ( len > drawingContext.MinimalSize );
			}

			return false;
		}

		private void InitSpacing(DrawingContext drawingContext)
		{
			//	Calcule l'espacement, en fonction du zoom actuel, afin d'obtenir une distance visible
			//	toujours identique.
			this.zoom = drawingContext.Zoom;
			this.spacing = Free.radiusSpacing/this.zoom;
		}


		protected void TextInfoModif(Point mouse, int rank)
		{
			//	Texte des informations de modification.
			string text = string.Format("Sommet={0}/{1}", rank+1, this.TotalMainHandle);
			this.document.Modifier.TextInfoModif = text;
		}


		public override Shape[] ShapesBuild(IPaintPort port, DrawingContext drawingContext, bool simplify)
		{
			//	Constuit les formes de l'objet.
			Path pathStart, pathEnd, pathLine;
			bool outlineStart, outlineEnd, surfaceStart, surfaceEnd;
			this.PathBuild(drawingContext,
						   out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine, simplify);

			int totalShapes = 2;
			if ( surfaceStart )  totalShapes ++;
			if ( surfaceEnd   )  totalShapes ++;
			if ( outlineStart )  totalShapes ++;
			if ( outlineEnd   )  totalShapes ++;
			
			int total = this.TotalMainHandle;
			bool support = false;
			if ( this.isCreating && total >= 2 )
			{
				support = true;
				totalShapes += 2;
			}

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

			//	Forme de la surface de d�part.
			if ( surfaceStart )
			{
				shapes[i] = new Shape();
				shapes[i].Path = pathStart;
				shapes[i].SetPropertySurface(port, this.PropertyLineColor);
				shapes[i].IsMisc = true;
				i ++;
			}

			//	Forme de la surface d'arriv�e.
			if ( surfaceEnd )
			{
				shapes[i] = new Shape();
				shapes[i].Path = pathEnd;
				shapes[i].SetPropertySurface(port, this.PropertyLineColor);
				shapes[i].IsMisc = true;
				i ++;
			}

			//	Forme du chemin de d�part.
			if ( outlineStart )
			{
				shapes[i] = new Shape();
				shapes[i].Path = pathStart;
				shapes[i].SetPropertyStroke(port, this.PropertyLineMode, this.PropertyLineColor);
				shapes[i].IsMisc = true;
				i ++;
			}

			//	Forme du chemin d'arriv�e.
			if ( outlineEnd )
			{
				shapes[i] = new Shape();
				shapes[i].Path = pathEnd;
				shapes[i].SetPropertyStroke(port, this.PropertyLineMode, this.PropertyLineColor);
				shapes[i].IsMisc = true;
				i ++;
			}

			//	Forme des traits de support pour les poign�es secondaires.
			if ( support )
			{
				Point center = this.Handle(total-2).Position;

				Path pathSupport = new Path();
				pathSupport.AppendCircle(center, this.spacing);

				pathSupport.MoveTo(center+new Point(-this.spacing*0.2, 0));
				pathSupport.LineTo(center+new Point( this.spacing*0.2, 0));

				pathSupport.MoveTo(center+new Point(0, -this.spacing*0.2));
				pathSupport.LineTo(center+new Point(0,  this.spacing*0.2));

				shapes[i] = new Shape();
				shapes[i].Path = pathSupport;
				shapes[i].SetPropertyStroke(port, this.PropertyLineMode, this.PropertyLineColor);
				shapes[i].Aspect = Aspect.Support;
				shapes[i].IsVisible = true;
				i ++;

				Path pathBox = new Path();
				pathBox.AppendRectangle(center-new Point(this.spacing, this.spacing), new Size(this.spacing*2, this.spacing*2));

				shapes[i] = new Shape();
				shapes[i].Path = pathBox;
				shapes[i].Type = Type.Surface;
				shapes[i].Aspect = Aspect.InvisibleBox;
				i ++;
			}

			return shapes;
		}

		protected void PathBuild(DrawingContext drawingContext,
								 out Path pathStart, out bool outlineStart, out bool surfaceStart,
								 out Path pathEnd,   out bool outlineEnd,   out bool surfaceEnd,
								 out Path pathLine, bool simplify)
		{
			//	Cr�e les 3 chemins de l'objet.
			pathStart = new Path();
			pathEnd   = new Path();
			pathLine  = new Path();

			double zoom = Properties.Abstract.DefaultZoom(drawingContext);
			pathStart.DefaultZoom = zoom;
			pathEnd.DefaultZoom = zoom;
			pathLine.DefaultZoom = zoom;

			int total = this.TotalMainHandle;
			if ( total < 2 )
			{
				outlineStart = false;
				surfaceStart = false;
				outlineEnd   = false;
				surfaceEnd   = false;
				return;
			}

			Point e1, e2;
			double w = this.PropertyLineMode.Width;
			CapStyle cap = this.PropertyLineMode.Cap;
			e1 = this.Handle(0).Position;
			e2 = this.Handle(1).Position;
			this.vp1 = this.PropertyArrow.PathExtremity(pathStart, 0, w,cap, e1,e2, simplify, out outlineStart, out surfaceStart);
			e1 = this.Handle(total-1).Position;
			e2 = this.Handle(total-2).Position;
			this.vp2 = this.PropertyArrow.PathExtremity(pathEnd,   1, w,cap, e1,e2, simplify, out outlineEnd,   out surfaceEnd);

			double tension = this.PropertyTension.TensionValue * 0.5;

			if (total <= 2)
			{
				for (int i=0; i<total; i++)
				{
					Point pos = this.GetCyclingHandlePosition(i);

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
						Point p = this.GetCyclingHandlePosition(i);
						pathLine.MoveTo(p);
					}
					else
					{
						Point s1, s2;

						if (i == 1 && !this.PropertyPolyClose.BoolValue)
						{
							s1 = this.GetCyclingHandlePosition(i-1);
						}
						else
						{
							s1 = this.ComputeSecondary(this.GetCyclingHandlePosition(i), this.GetCyclingHandlePosition(i-1), this.GetCyclingHandlePosition(i-2), tension);
						}

						if (i == total-1 && !this.PropertyPolyClose.BoolValue)
						{
							s2 = this.GetCyclingHandlePosition(i);
						}
						else
						{
							s2 = this.ComputeSecondary(this.GetCyclingHandlePosition(i-1), this.GetCyclingHandlePosition(i), this.GetCyclingHandlePosition(i+1), tension);
						}
						
						pathLine.CurveTo(s1, s2, this.GetCyclingHandlePosition(i));
					}
				}
			}
		}

		private Point GetCyclingHandlePosition(int rank)
		{
			//	Retourne la position d'un poign�e, en acceptant les rangs n�gatifs ou d�passant le
			//	nombre maximal de poign�es. Tient compte des positions virtuelles vp1/vp2 selon la
			//	propri�t� Arrow.
			int total = this.TotalMainHandle;

			if (rank < 0)
			{
				rank = total+rank;
			}

			if (rank >= total)
			{
				rank = rank-total;
			}

			if (rank == 0)
			{
				return this.vp1;
			}
			else if (rank == total-1)
			{
				return this.vp2;
			}
			else
			{
				return this.Handle(rank).Position;
			}
		}

		private Point ComputeSecondary(Point p1, Point p, Point p2, double tension)
		{
			//	Calcule un point secondaire s1 permettant d'obtenir une jolie courbe.
			//	   p
			//	   o----o p2
			//	  /|   /
			//	 o |  /      le segment s1-p est // � p1-p2
			//	s1 | /
			//	   |/
			//	p1 o

			return Point.Move(p, p+(p1-p2), Point.Distance(p,p1)*tension);
		}


		public override Path GetMagnetPath()
		{
			//	Retourne le chemin g�om�trique de l'objet pour les constructions
			//	magn�tiques.
			Path pathStart, pathEnd, pathLine;
			bool outlineStart, outlineEnd, surfaceStart, surfaceEnd;
			this.PathBuild(null,
						   out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine, true);

			return pathLine;
		}

		protected override Path GetPath()
		{
			//	Retourne le chemin g�om�trique de l'objet.
			Path pathStart, pathEnd, pathLine;
			bool outlineStart, outlineEnd, surfaceStart, surfaceEnd;
			this.PathBuild(null,
						   out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine, false);

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


		#region Serialization
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	S�rialise l'objet.
			base.GetObjectData(info, context);
		}

		protected Free(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui d�s�rialise l'objet.
		}
		#endregion


		protected static double			radiusSpacing = 200;

		protected double				zoom;
		protected double				spacing;
		protected Point					initialPos;
		protected Point					vp1, vp2;
	}
}

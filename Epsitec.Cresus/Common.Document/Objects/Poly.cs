using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Objects
{
	/// <summary>
	/// La classe Poly est la classe de l'objet graphique "polygone".
	/// </summary>
	[System.Serializable()]
	public class Poly : Objects.Abstract
	{
		public Poly(Document document, Objects.Abstract model) : base(document, model)
		{
			if ( this.document == null )  return;  // objet factice ?
			this.CreateProperties(model, false);
		}

		protected override bool ExistingProperty(Properties.Type type)
		{
			if ( type == Properties.Type.Name )  return true;
			if ( type == Properties.Type.LineMode )  return true;
			if ( type == Properties.Type.LineColor )  return true;
			if ( type == Properties.Type.Arrow )  return true;
			if ( type == Properties.Type.FillGradient )  return true;
			if ( type == Properties.Type.PolyClose )  return true;
			if ( type == Properties.Type.Corner )  return true;
			return false;
		}

		protected override Objects.Abstract CreateNewObject(Document document, Objects.Abstract model)
		{
			return new Poly(document, model);
		}

		public override void Dispose()
		{
			base.Dispose();
		}


		public override string IconName
		{
			//	Nom de l'ic�ne.
			get { return Misc.Icon("ObjectPoly"); }
		}


		public override void MoveHandleStarting(int rank, Point pos, DrawingContext drawingContext)
		{
			//	D�but du d�placement d'une poign�e.
			base.MoveHandleStarting(rank, pos, drawingContext);

			if ( rank < this.handles.Count )  // poign�e de l'objet ?
			{
				drawingContext.ConstrainFlush();
				Handle handle = this.Handle(rank);

				if ( this.TotalMainHandle == 2 )
				{
					if ( handle.PropertyType == Properties.Type.None )
					{
						drawingContext.ConstrainAddHV(this.Handle(0).Position);
						drawingContext.ConstrainAddHV(this.Handle(1).Position);
						drawingContext.ConstrainAddLine(this.Handle(0).Position, this.Handle(1).Position);
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
				else
				{
					if ( handle.PropertyType == Properties.Type.None )
					{
						drawingContext.ConstrainAddRect(this.Handle(this.NextRank(rank)).Position, this.Handle(this.PrevRank(rank)).Position);
						drawingContext.ConstrainAddLine(this.Handle(rank).Position, this.Handle(this.NextRank(rank)).Position);
						drawingContext.ConstrainAddLine(this.Handle(rank).Position, this.Handle(this.PrevRank(rank)).Position);
						drawingContext.ConstrainAddHV(this.Handle(rank).Position);
						drawingContext.ConstrainAddCircle(this.Handle(this.NextRank(rank)).Position, this.Handle(rank).Position);
						drawingContext.ConstrainAddCircle(this.Handle(this.PrevRank(rank)).Position, this.Handle(rank).Position);
					}
					else
					{
						Properties.Abstract property = this.Property(handle.PropertyType);
						property.MoveHandleStarting(this, handle.PropertyRank, pos, drawingContext);
					}

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
			int n = this.NextRank(ss.Rank);
			this.Handle(r).InitialPosition = this.Handle(r).Position;
			this.Handle(n).InitialPosition = this.Handle(n).Position;

			drawingContext.ConstrainFlush();
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
			int n = this.NextRank(ss.Rank);
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

			if ( family == "Continue" )
			{
				if ( this.IsShaperHandleSelected() && !this.PropertyPolyClose.BoolValue )
				{
					int total = this.TotalMainHandle;
					for ( int i=0 ; i<total ; i++ )
					{
						if ( !this.Handle(i).IsVisible )  continue;
						if ( this.Handle(i).IsShaperDeselected )  continue;
						if ( this.Handle(i).Type == HandleType.Starting ||
							 this.Handle(this.NextRank(i)).Type == HandleType.Starting )
						{
							enable = true;
						}
					}
				}
				return true;
			}

			if ( family == "Sub" )
			{
				enable = (this.TotalMainHandle > 2 && this.IsShaperHandleSelected());
				return true;
			}

			if ( family == "Poly" )
			{
				if ( this.IsShaperHandleSelected() )
				{
					int total = this.TotalMainHandle;
					for ( int i=0 ; i<total ; i++ )
					{
						if ( !this.Handle(i).IsVisible )  continue;
						if ( this.Handle(i).IsShaperDeselected )  continue;

						HandleConstrainType type = this.Handle(i).ConstrainType;
						if ( type == HandleConstrainType.Simply )  Abstract.ShaperHandleStateAdd(actives, "Simply"); 
						else                                       Abstract.ShaperHandleStateAdd(actives, "Corner"); 
						enable = true;
					}
				}
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

			if ( cmd == "ShaperHandleContinue" )
			{
				this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.ShaperHandleContinue);
				this.InsertOpletGeometry();
				this.document.Notifier.NotifyArea(this.BoundingBox);

				for ( int i=this.TotalMainHandle-1 ; i>=0 ; i-- )
				{
					if ( !this.Handle(i).IsVisible )  continue;
					if ( this.Handle(i).IsShaperDeselected )  continue;
					if ( this.Handle(i).Type == HandleType.Starting ||
						this.Handle(this.NextRank(i)).Type == HandleType.Starting )
					{
						this.ShaperHandleContinue(i);
					}
				}

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

			if ( cmd == "ShaperHandleSimply" )
			{
				this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.ShaperHandleSimply);
				this.InsertOpletGeometry();
				this.document.Notifier.NotifyArea(this.BoundingBox);

				int total = this.TotalMainHandle;
				for ( int i=0 ; i<total ; i++ )
				{
					if ( !this.Handle(i).IsVisible )  continue;
					if ( this.Handle(i).IsShaperDeselected )  continue;
					this.Handle(i).ConstrainType = HandleConstrainType.Simply;
				}

				this.document.Notifier.NotifyArea(this.BoundingBox);
				this.document.Modifier.OpletQueueValidateAction();
				return true;
			}

			if ( cmd == "ShaperHandleCorner" )
			{
				this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.ShaperHandleCorner);
				this.InsertOpletGeometry();
				this.document.Notifier.NotifyArea(this.BoundingBox);

				int total = this.TotalMainHandle;
				for ( int i=0 ; i<total ; i++ )
				{
					if ( !this.Handle(i).IsVisible )  continue;
					if ( this.Handle(i).IsShaperDeselected )  continue;
					this.Handle(i).ConstrainType = HandleConstrainType.Symmetric;
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
			int next = this.NextRank(rank);
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

		protected void ShaperHandleContinue(int rank)
		{
			//	Prolonge la ligne.
			HandleType type = this.Handle(rank).Type;
			this.Handle(rank).Type = HandleType.Primary;

			int ins, prev1, prev2;
			if ( type == HandleType.Starting )  // ins�re au d�but ?
			{
				ins   = rank;
				prev1 = rank;
				prev2 = rank+1;
			}
			else	// ins�re � la fin ?
			{
				ins   = rank+1;
				prev1 = rank;
				prev2 = rank-1;
			}

			double d = 20.0/this.document.Modifier.ActiveViewer.DrawingContext.ScaleX;
			Point pos = Point.Move(this.Handle(prev1).Position, this.Handle(prev2).Position, -d);

			Handle handle = new Handle(this.document);
			handle.Position = pos;
			handle.Type = type;
			handle.IsVisible = true;
			this.HandleInsert(ins, handle);
			this.HandlePropertiesUpdate();
		}


		protected int DetectOutline(Point pos)
		{
			//	D�tecte si la souris est sur le pourtour de l'objet.
			//	Retourne le rang de la poign�e de d�part, ou -1
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;

			Shape[] shapes = this.ShapesBuild(null, context, false);
			return context.Drawer.DetectOutline(pos, context, shapes);
		}


		public override void CreateMouseDown(Point pos, DrawingContext drawingContext)
		{
			//	D�but de la cr�ation d'un objet.
			drawingContext.SnapPos(ref pos);

			if ( this.TotalHandle == 0 )
			{
				drawingContext.ConstrainFlush();
				drawingContext.ConstrainAddHV(pos);
				this.ChangePropertyPolyClose(false);
				this.HandleAdd(pos, HandleType.Starting);
				this.HandleAdd(pos, HandleType.Primary);
				this.Handle(0).IsVisible = true;
				this.Handle(1).IsVisible = true;
			}
			else
			{
				double len = Point.Distance(pos, this.Handle(this.TotalHandle-1).Position);
				if ( len > drawingContext.CloseMargin )
				{
					this.HandleAdd(pos, HandleType.Primary);
					this.Handle(this.TotalHandle-1).IsVisible = true;
				}
			}

			this.mouseDown = true;
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override void CreateMouseMove(Point pos, DrawingContext drawingContext)
		{
			//	D�placement pendant la cr�ation d'un objet.
			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapPos(ref pos);

			this.isCreating = true;

			int rank = this.TotalHandle-1;
			if ( rank > 0 )
			{
				double len = Point.Distance(this.Handle(0).Position, pos);
				if ( len <= drawingContext.CloseMargin )
				{
					this.Handle(0).Type = HandleType.Ending;
				}
				else
				{
					this.Handle(0).Type = HandleType.Starting;
				}
			}

			this.AdditionalMove(pos, drawingContext);

			if ( this.mouseDown )
			{
				this.Handle(rank).Position = pos;
			}
			this.SetDirtyBbox();
			this.TextInfoModif(pos, rank);
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override void CreateMouseUp(Point pos, DrawingContext drawingContext)
		{
			//	Fin de la cr�ation d'un objet.
			if ( this.TotalHandle == 2 )
			{
				double len = Point.Distance(this.Handle(0).Position, this.Handle(1).Position);
				if ( len < drawingContext.MinimalSize )
				{
					this.HandleDelete(1);
				}
			}

			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapPos(ref pos);
			int rank = this.TotalHandle-1;
			this.Handle(rank).Position = pos;
			drawingContext.ConstrainDelStarting();
			drawingContext.ConstrainFlush();
			drawingContext.ConstrainAddHV(pos);
			drawingContext.MagnetClearStarting();
			this.mouseDown = false;
			this.AdditionalCreate(pos, drawingContext);
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override bool CreateIsEnding(DrawingContext drawingContext)
		{
			//	Indique si la cr�ation de l'objet est termin�e.
			if ( this.TotalHandle < 2 )  return false;

			int rank = this.TotalHandle-1;
			double len = Point.Distance(this.Handle(0).Position, this.Handle(rank).Position);
			if ( len > drawingContext.CloseMargin )  return false;  // pas fini

			this.isCreating = false;
			this.document.Modifier.TextInfoModif = "";
			this.HandleDelete(rank);
			this.ChangePropertyPolyClose(true);

			this.AdditionalDelete();
			this.Handle(0).Type = HandleType.Starting;
			this.Deselect();
			drawingContext.ConstrainDelStarting();

			this.HandlePropertiesCreate();
			this.HandlePropertiesUpdate();
			return true;
		}

		public override bool CreateIsExist(DrawingContext drawingContext)
		{
			//	Indique si l'objet doit exister. Retourne false si l'objet ne peut
			//	pas exister et doit �tre d�truit.
			return ( this.TotalHandle >= 2 );
		}

		public override bool CreateEnding(DrawingContext drawingContext)
		{
			//	Termine la cr�ation de l'objet. Retourne false si l'objet ne peut
			//	pas exister et doit �tre d�truit.
			this.document.Notifier.NotifyArea(this.BoundingBox);
			this.isCreating = false;
			this.document.Modifier.TextInfoModif = "";

			if ( this.TotalHandle < 2 )  return false;

			this.AdditionalDelete();
			this.Handle(0).Type = HandleType.Starting;
			this.Deselect();
			drawingContext.ConstrainDelStarting();

			this.HandlePropertiesCreate();
			this.HandlePropertiesUpdate();
			return true;
		}

		protected void TextInfoModif(Point mouse, int rank)
		{
			//	Texte des informations de modification.
			if ( this.isCreating )
			{
				Point p1, p2;
				if ( this.mouseDown )
				{
					if ( this.TotalHandle < 2 )  return;
					p1 = this.Handle(this.TotalHandle-2).Position;
					p2 = this.Handle(this.TotalHandle-1).Position;
				}
				else
				{
					if ( this.TotalHandle < 1 )  return;
					p1 = this.Handle(this.TotalHandle-1).Position;
					p2 = mouse;
				}
				double len = Point.Distance(p1, p2);
				double angle = Point.ComputeAngleDeg(p1, p2);
				string text = string.Format(Res.Strings.Object.Poly.Info1, this.document.Modifier.RealToString(len), this.document.Modifier.AngleToString(angle));
				this.document.Modifier.TextInfoModif = text;
			}
			else
			{
				int prev = rank-1;
				int next = rank+1;
				if ( prev < 0 )  prev = this.TotalMainHandle-1;
				if ( next >= this.TotalMainHandle )  next = 0;

				Point p1 = this.Handle(prev).Position;
				Point p2 = this.Handle(rank).Position;
				Point p3 = this.Handle(next).Position;

				double len1 = Point.Distance(p1, p2);
				double len2 = Point.Distance(p2, p3);
				double angle1 = Point.ComputeAngleDeg(p1, p2);
				double angle2 = Point.ComputeAngleDeg(p3, p2);
				string text = string.Format(Res.Strings.Object.Poly.Info2, this.document.Modifier.RealToString(len1), this.document.Modifier.RealToString(len2), this.document.Modifier.AngleToString(angle1), this.document.Modifier.AngleToString(angle2));
				this.document.Modifier.TextInfoModif = text;
			}
		}

		public override bool CreateAction(int rank, out string cmd, out string name, out string text)
		{
			//	Retourne un bouton d'action pendant la cr�ation.
			switch ( rank )
			{
				case 0:
					cmd  = "Object";
					name = "CreateEnding";
					text = Abstract.CreateAction("CreateEnding", "CreateCloseNo", Res.Strings.Object.Button.CreateEnding);
					return true;

				case 1:
					cmd  = "Object";
					name = "CreateCloseEnding";
					text = Abstract.CreateAction("CreateEnding", "CreateCloseYes", Res.Strings.Object.Button.CreateEnding);
					return true;

				case 2:
					cmd  = "";
					name = "";
					text = "";
					return true;

				case 3:
					cmd  = "Object";
					name = "CreateAndSelect";
					text = Abstract.CreateAction("CreateAndSelect", "CreateCloseNo", Res.Strings.Object.Button.CreateAndSelect);
					return true;

				case 4:
					cmd  = "Object";
					name = "CreateCloseAndSelect";
					text = Abstract.CreateAction("CreateAndSelect", "CreateCloseYes", Res.Strings.Object.Button.CreateAndSelect);
					return true;

				case 5:
					cmd  = "";
					name = "";
					text = "";
					return true;

				case 6:
					cmd  = "Object";
					name = "CreateAndShaper";
					text = Abstract.CreateAction("CreateAndShaper", "CreateCloseNo", Res.Strings.Object.Button.CreateAndShaper);
					return true;

				case 7:
					cmd  = "Object";
					name = "CreateCloseAndShaper";
					text = Abstract.CreateAction("CreateAndShaper", "CreateCloseYes", Res.Strings.Object.Button.CreateAndShaper);
					return true;
			}

			return base.CreateAction(rank, out cmd, out name, out text);
		}

		protected void AdditionalCreate(Point pos, DrawingContext drawingContext)
		{
			//	Cr�e l'objet temporaire pour montrer le nouveau segment.
			this.additionalLineExist = true;
			this.additionalLineP1 = pos;
			this.additionalLineP2 = pos;
		}

		protected void AdditionalMove(Point pos, DrawingContext drawingContext)
		{
			//	D�place l'objet temporaire pour montrer le nouveau segment.
			this.additionalLineP2 = pos;
		}

		protected void AdditionalDelete()
		{
			//	D�truit l'objet temporaire pour montrer le nouveau segment.
			this.additionalLineExist = false;
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
			
			Path pathAdditional = null;
			if ( this.additionalLineExist )
			{
				pathAdditional = new Path();
				pathAdditional.MoveTo(this.additionalLineP1);
				pathAdditional.LineTo(this.additionalLineP2);
				totalShapes ++;
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

			//	Forme de la ligne temporaire.
			if ( this.additionalLineExist )
			{
				shapes[i] = new Shape();
				shapes[i].Path = pathAdditional;
				shapes[i].SetPropertyStroke(port, this.PropertyLineMode, this.PropertyLineColor);
				shapes[i].Aspect = Aspect.Additional;
				i ++;
			}

			return shapes;
		}

		protected void PathBuild(DrawingContext drawingContext,
								 out Path pathStart, out bool outlineStart, out bool surfaceStart,
								 out Path pathEnd,   out bool outlineEnd,   out bool surfaceEnd,
								 out Path pathLine, bool simplify)
		{
			//	Cr�e les chemins de l'objet.
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

			Point p1, p2, pp1, pp2, s;
			double w = this.PropertyLineMode.Width;
			CapStyle cap = this.PropertyLineMode.Cap;
			p1 = this.Handle(0).Position;
			p2 = this.Handle(1).Position;
			pp1 = this.PropertyArrow.PathExtremity(pathStart, 0, w,cap, p1,p2, simplify, out outlineStart, out surfaceStart);
			p1 = this.Handle(total-1).Position;
			p2 = this.Handle(total-2).Position;
			pp2 = this.PropertyArrow.PathExtremity(pathEnd,   1, w,cap, p1,p2, simplify, out outlineEnd,   out surfaceEnd);

			bool close = ( this.PropertyPolyClose.BoolValue && total > 2 );
			Properties.Corner corner = this.PropertyCorner;
			if ( corner.CornerType == Properties.CornerType.Right || simplify )  // coins droits ?
			{
				int first = 0;
				for ( int i=0 ; i<total ; i++ )
				{
					p1 = this.Handle(i).Position;

					if ( i == 0 )  // premier point ?
					{
						pathLine.MoveTo(pp1);
					}
					else if ( this.Handle(i).Type == HandleType.Starting )  // premier point ?
					{
						if ( close )
						{
							pathLine.LineTo(pp1);
							pathLine.Close();
						}
						first = i;
						pp1 = this.Handle(i).Position;
						pathLine.MoveTo(pp1);
					}
					else if ( i < total-1 )  // point interm�diaire ?
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
			else	// coins sp�ciaux ?
			{
				int first = 0;
				for ( int i=0 ; i<total ; i++ )
				{
					p1 = this.Handle(i).Position;
					int prev = this.PrevRank(i);
					int next = this.NextRank(i);
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
							this.PathCorner(pathLine, p1,s,p2, corner, simply, true);
						}
					}
					else if ( this.Handle(i).Type == HandleType.Starting )  // premier point ?
					{
						if ( close )
						{
							pathLine.Close();
						}
						first = i;
						pp1 = this.Handle(i).Position;
						if ( outlineStart || surfaceStart || !close )
						{
							pathLine.MoveTo(pp1);
						}
						else
						{
							p1 = this.Handle(prev).Position;
							s  = this.Handle(i).Position;
							p2 = this.Handle(next).Position;
							this.PathCorner(pathLine, p1,s,p2, corner, simply, true);
						}
					}
					else if ( i < total-1 )  // point interm�diaire ?
					{
						if ( i > next && !close )
						{
							pathLine.LineTo(this.Handle(i).Position);
						}
						else
						{
							p1 = this.Handle(prev).Position;
							s  = this.Handle(i).Position;
							p2 = this.Handle(next).Position;
							this.PathCorner(pathLine, p1,s,p2, corner, simply, false);
						}
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
							this.PathCorner(pathLine, p1,s,p2, corner, simply, false);
						}
					}
				}
				if ( close )
				{
					pathLine.Close();
				}
			}
		}

		protected int PrevRank(int rank)
		{
			//	Cherche le rang pr�c�dent, en tenant compte
			//	des ensembles Starting-Primary(s).
			if ( rank == 0 || this.Handle(rank).Type == HandleType.Starting )
			{
				do
				{
					rank ++;
				}
				while ( rank < this.TotalMainHandle && this.Handle(rank).Type != HandleType.Starting );
			}
			rank --;
			return rank;
		}

		protected int NextRank(int rank)
		{
			//	Cherche le rang suivant, en tenant compte
			//	des ensembles Starting-Primary(s).
			rank ++;
			if ( rank >= this.TotalMainHandle || this.Handle(rank).Type == HandleType.Starting )
			{
				do
				{
					rank --;
				}
				while ( rank > 0 && this.Handle(rank).Type != HandleType.Starting );
			}
			return rank;
		}

		protected void PathCorner(Path path, Point p1, Point s, Point p2, Properties.Corner corner, bool simply, bool first)
		{
			//	Cr�e le chemin d'un coin.
			if ( simply )
			{
				if ( first )  path.MoveTo(s);
				else          path.LineTo(s);
			}
			else
			{
				double l1 = Point.Distance(p1, s);
				double l2 = Point.Distance(p2, s);
				double radius = System.Math.Min(corner.Radius, System.Math.Min(l1,l2)/2);
				Point c1 = Point.Move(s, p1, radius);
				Point c2 = Point.Move(s, p2, radius);
				if ( first )  path.MoveTo(c1);
				else          path.LineTo(c1);
				corner.PathCorner(path, c1,s,c2, radius);
			}
		}


		#region CreateFromPath
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

		public bool CreateFromPath(Path path, int subPath)
		{
			//	Cr�e un polygone � partir d'un chemin quelconque.
			PathElement[] elements;
			Point[] points;
			path.GetElements(out elements, out points);
			if ( elements.Length > 1000 )  return false;

			int firstHandle = this.TotalMainHandle;
			Point start = new Point(0, 0);
			Point current = new Point(0, 0);
			Point p1 = new Point(0, 0);
			Point p2 = new Point(0, 0);
			Point p3 = new Point(0, 0);
			bool close = false;
			bool bDo = false;
			int subRank = -1;
			int i = 0;
			while ( i < elements.Length )
			{
				switch ( elements[i] & PathElement.MaskCommand )
				{
					case PathElement.MoveTo:
						subRank ++;
						current = points[i++];
						firstHandle = this.TotalMainHandle;
						if ( subPath == -1 || subPath == subRank )
						{
							this.HandleAdd(current, HandleType.Starting);
							bDo = true;
						}
						start = current;
						break;

					case PathElement.LineTo:
						p1 = points[i++];
						if ( subPath == -1 || subPath == subRank )
						{
							if ( Geometry.Compare(p1, start) )
							{
								close = true;
								firstHandle = this.TotalMainHandle;
							}
							else
							{
								this.HandleAdd(p1, HandleType.Primary);
								bDo = true;
							}
						}
						current = p1;
						break;

					case PathElement.Curve3:
						p1 = points[i];
						p2 = points[i++];
						p3 = points[i++];
						p1 = Point.Scale(current, p1, 2.0/3.0);
						p2 = Point.Scale(p3,      p2, 2.0/3.0);
						if ( subPath == -1 || subPath == subRank )
						{
							if ( Geometry.Compare(p3, start) )
							{
								close = true;
								firstHandle = this.TotalMainHandle;
							}
							else
							{
								this.HandleAdd(p3, HandleType.Primary);
								bDo = true;
							}
						}
						current = p3;
						break;

					case PathElement.Curve4:
						p1 = points[i++];
						p2 = points[i++];
						p3 = points[i++];
						if ( subPath == -1 || subPath == subRank )
						{
							if ( Geometry.Compare(p3, start) )
							{
								close = true;
								firstHandle = this.TotalMainHandle;
							}
							else
							{
								this.HandleAdd(p3, HandleType.Primary);
								bDo = true;
							}
						}
						current = p3;
						break;

					default:
						if ( (elements[i] & PathElement.FlagClose) != 0 )
						{
							close = true;
						}
						i ++;
						break;
				}
			}
			this.PropertyPolyClose.BoolValue = close;

			return bDo;
		}

		public void CreateFinalise()
		{
			//	Finalise la cr�ation d'un polygone.
			this.HandlePropertiesCreate();  // cr�e les poign�es des propri�t�s
			this.Select(false);
			this.Select(true);  // pour s�lectionner toutes les poign�es
		}
		#endregion


		#region Serialization
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	S�rialise l'objet.
			base.GetObjectData(info, context);
		}

		protected Poly(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui d�s�rialise l'objet.
		}
		#endregion

		
		protected bool				mouseDown = false;
		protected bool				additionalLineExist = false;
		protected Point				additionalLineP1;
		protected Point				additionalLineP2;
		protected Point				initialPos;
	}
}

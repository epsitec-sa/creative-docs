using System.Collections.Generic;
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
			if ( type == Properties.Type.Frame )  return true;
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


		public override string IconUri
		{
			//	Nom de l'icône.
			get { return Misc.Icon("ObjectPoly"); }
		}


		public override void MoveHandleStarting(int rank, Point pos, DrawingContext drawingContext)
		{
			//	Début du déplacement d'une poignée.
			base.MoveHandleStarting(rank, pos, drawingContext);

			if ( rank < this.handles.Count )  // poignée de l'objet ?
			{
				drawingContext.ConstrainClear();
				Handle handle = this.Handle(rank);

				if ( this.TotalMainHandle == 2 )
				{
					if ( handle.PropertyType == Properties.Type.None )
					{
						drawingContext.ConstrainAddHV(this.Handle(0).Position, false, 0);
						drawingContext.ConstrainAddHV(this.Handle(1).Position, false, 1);
						drawingContext.ConstrainAddLine(this.Handle(0).Position, this.Handle(1).Position, false, -1);
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
						drawingContext.ConstrainAddHV(this.Handle(this.NextRank(rank)).Position, false, this.NextRank(rank));
						drawingContext.ConstrainAddHV(this.Handle(this.PrevRank(rank)).Position, false, this.PrevRank(rank));
						drawingContext.ConstrainAddCircle(this.Handle(this.NextRank(rank)).Position, this.Handle(rank).Position, false, this.NextRank(rank));
						drawingContext.ConstrainAddCircle(this.Handle(this.PrevRank(rank)).Position, this.Handle(rank).Position, false, this.PrevRank(rank));

						drawingContext.ConstrainAddLine(this.Handle(rank).Position, this.Handle(this.NextRank(rank)).Position, false, rank);
						drawingContext.ConstrainAddLine(this.Handle(rank).Position, this.Handle(this.PrevRank(rank)).Position, false, rank);
						drawingContext.ConstrainAddHV(this.Handle(rank).Position, false, rank);
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
			//	Déplace une poignée.
			if ( rank >= this.TotalMainHandle )  // poignée d'une propriété ?
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
			//	Début du déplacement d'une poignée d'un segment sélectionné.
			base.MoveSelectedSegmentStarting(rank, pos, drawingContext);

			this.initialPos = pos;
			SelectedSegment ss = this.selectedSegments[rank] as SelectedSegment;
			int r = ss.Rank;
			int n = this.NextRank(ss.Rank);
			this.Handle(r).InitialPosition = this.Handle(r).Position;
			this.Handle(n).InitialPosition = this.Handle(n).Position;

			drawingContext.ConstrainClear();
			drawingContext.ConstrainAddHV(ss.Position, false, -1);
			Point p1 = this.Handle(r).Position;
			Point p2 = this.Handle(n).Position;
			Size d = new Size(p2.X-p1.X, p2.Y-p1.Y);
			drawingContext.ConstrainAddLine(p1, p2, false, -1);
			drawingContext.ConstrainAddLine(pos, new Point(pos.X-d.Height, pos.Y+d.Width), false, -1);
		}

		public override void MoveSelectedSegmentProcess(int rank, Point pos, DrawingContext drawingContext)
		{
			//	Déplace une poignée d'un segment sélectionné.
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
				enable = (this.TotalMainHandle-this.TotalShaperHandleSelected() >= 2 && this.IsShaperHandleSelected());
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
			//	Ajoute une poignée sans changer l'aspect.
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

		protected void ShaperHandleContinue(int rank)
		{
			//	Prolonge la ligne.
			HandleType type = this.Handle(rank).Type;
			this.Handle(rank).Type = HandleType.Primary;

			int ins, prev1, prev2;
			if ( type == HandleType.Starting )  // insère au début ?
			{
				ins   = rank;
				prev1 = rank;
				prev2 = rank+1;
			}
			else	// insère à la fin ?
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


		public override void CreateMouseDown(Point pos, DrawingContext drawingContext)
		{
			//	Début de la création d'un objet.
			this.document.Notifier.NotifyArea(this.BoundingBox);  // pour effacer les résidus de l'ancienne flèche

			drawingContext.SnapPos(ref pos);

			if ( this.TotalHandle == 0 )
			{
				drawingContext.ConstrainClear();
				drawingContext.ConstrainAddHV(pos, false, 0);
				this.ChangePropertyPolyClose(false);
				this.HandleAdd(pos, HandleType.Starting);
				this.HandleAdd(pos, HandleType.Primary);
				this.Handle(0).IsVisible = true;
				this.Handle(1).IsVisible = true;
			}
			else
			{
				double len = Point.Distance(pos, this.Handle(this.TotalMainHandle-1).Position);
				if ( len > drawingContext.CloseMargin )
				{
					this.HandleAdd(pos, HandleType.Primary);
					this.Handle(this.TotalMainHandle-1).IsVisible = true;
				}
			}

			this.mouseDown = true;
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override void CreateMouseMove(Point pos, DrawingContext drawingContext)
		{
			//	Déplacement pendant la création d'un objet.
			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapPos(ref pos);

			this.isCreating = true;

			int rank = this.TotalMainHandle-1;
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
			//	Fin de la création d'un objet.
			if ( this.TotalMainHandle == 2 )
			{
				double len = Point.Distance(this.Handle(0).Position, this.Handle(1).Position);
				if ( len < drawingContext.MinimalSize )
				{
					this.HandleDelete(1);
				}
			}

			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapPos(ref pos);
			int rank = this.TotalMainHandle-1;
			this.Handle(rank).Position = pos;
			drawingContext.ConstrainDelStarting();
			drawingContext.ConstrainClear();
			drawingContext.ConstrainAddHV(pos, false, rank);
			drawingContext.MagnetClearStarting();
			this.mouseDown = false;
			this.AdditionalCreate(pos, drawingContext);
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override bool CreateIsEnding(DrawingContext drawingContext)
		{
			//	Indique si la création de l'objet est terminée.
			if ( this.TotalMainHandle < 3 )  return false;

			int rank = this.TotalMainHandle-1;
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
			//	pas exister et doit être détruit.
			return ( this.TotalMainHandle >= 2 );
		}

		public override bool CreateEnding(DrawingContext drawingContext)
		{
			//	Termine la création de l'objet. Retourne false si l'objet ne peut
			//	pas exister et doit être détruit.
			this.document.Notifier.NotifyArea(this.BoundingBox);
			this.isCreating = false;
			this.document.Modifier.TextInfoModif = "";

			if ( this.TotalMainHandle < 2 )  return false;

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
					if ( this.TotalMainHandle < 2 )  return;
					p1 = this.Handle(this.TotalMainHandle-2).Position;
					p2 = this.Handle(this.TotalMainHandle-1).Position;
				}
				else
				{
					if ( this.TotalMainHandle < 1 )  return;
					p1 = this.Handle(this.TotalMainHandle-1).Position;
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
			//	Retourne un bouton d'action pendant la création.
			switch ( rank )
			{
				case 0:
					cmd  = "Object";
					name = "CreateEnding";
					text = Abstract.CreateAction("CreateCloseNo", Res.Strings.Object.Button.CreateEndingOpen);
					return true;

				case 1:
					cmd  = "Object";
					name = "CreateCloseEnding";
					text = Abstract.CreateAction("CreateCloseYes", Res.Strings.Object.Button.CreateEndingClose);
					return true;

				case 2:
					cmd  = "";
					name = "";
					text = "";
					return true;

				case 3:
					cmd  = "Object";
					name = "CreateAndSelect";
					text = Abstract.CreateAction("CreateAndSelect", Res.Strings.Object.Button.CreateAndSelect);
					return true;

				case 4:
					cmd  = "Object";
					name = "CreateAndShaper";
					text = Abstract.CreateAction("CreateAndShaper", Res.Strings.Object.Button.CreateAndShaper);
					return true;
			}

			return base.CreateAction(rank, out cmd, out name, out text);
		}

		protected void AdditionalCreate(Point pos, DrawingContext drawingContext)
		{
			//	Crée l'objet temporaire pour montrer le nouveau segment.
			this.additionalLineExist = true;
			this.additionalLineP1 = pos;
			this.additionalLineP2 = pos;
		}

		protected void AdditionalMove(Point pos, DrawingContext drawingContext)
		{
			//	Déplace l'objet temporaire pour montrer le nouveau segment.
			this.additionalLineP2 = pos;
		}

		protected void AdditionalDelete()
		{
			//	Détruit l'objet temporaire pour montrer le nouveau segment.
			this.additionalLineExist = false;
		}

		
		public override Shape[] ShapesBuild(IPaintPort port, DrawingContext drawingContext, bool simplify)
		{
			//	Constuit les formes de l'objet.
			var frame = this.PropertyFrame;

			Path pathStart, pathEnd, pathLine;
			bool outlineStart, outlineEnd, surfaceStart, surfaceEnd;
			this.PathBuild (drawingContext,
						   out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd, out outlineEnd, out surfaceEnd,
						   out pathLine, simplify, false);

			Path pathAdditional = null;
			if (this.additionalLineExist)
			{
				pathAdditional = new Path ();
				pathAdditional.MoveTo (this.additionalLineP1);
				pathAdditional.LineTo (this.additionalLineP2);
			}

			var shapes = new List<Shape> ();
			var objectShapes = new List<Shape> ();

			//	Forme de la surface principale.
			{
				var shape = new Shape ();
				shape.Path = pathLine;
				shape.SetPropertySurface (port, this.PropertyFillGradient);
				objectShapes.Add (shape);
			}

			//	Forme du chemin principal.
			{
				var shape = new Shape ();
				shape.Path = pathLine;
				shape.SetPropertyStroke (port, this.PropertyLineMode, this.PropertyLineColor);
				objectShapes.Add (shape);
			}

			//	Forme de la surface de départ.
			if (surfaceStart)
			{
				var shape = new Shape ();
				shape.Path = pathStart;
				shape.SetPropertySurface (port, this.PropertyLineColor);
				shape.IsMisc = true;
				objectShapes.Add (shape);
			}

			//	Forme de la surface d'arrivée.
			if (surfaceEnd)
			{
				var shape = new Shape ();
				shape.Path = pathEnd;
				shape.SetPropertySurface (port, this.PropertyLineColor);
				shape.IsMisc = true;
				objectShapes.Add (shape);
			}

			//	Forme du chemin de départ.
			if (outlineStart)
			{
				var shape = new Shape ();
				shape.Path = pathStart;
				shape.SetPropertyStroke (port, this.PropertyLineMode, this.PropertyLineColor);
				shape.IsMisc = true;
				objectShapes.Add (shape);
			}

			//	Forme du chemin d'arrivée.
			if (outlineEnd)
			{
				var shape = new Shape ();
				shape.Path = pathEnd;
				shape.SetPropertyStroke (port, this.PropertyLineMode, this.PropertyLineColor);
				shape.IsMisc = true;
				objectShapes.Add (shape);
			}

			if (!simplify && (frame == null || frame.FrameType == Properties.FrameType.None))  // pas de cadre ?
			{
				shapes.AddRange (objectShapes);
			}
			else  // cadre ?
			{
				frame.AddShapes (shapes, objectShapes, port, drawingContext, this.GetPolygons (), this.PropertyCorner);
			}

			//	Forme de la ligne temporaire.
			if (this.additionalLineExist)
			{
				var shape = new Shape ();
				shape.Path = pathAdditional;
				shape.SetPropertyStroke (port, this.PropertyLineMode, this.PropertyLineColor);
				shape.Aspect = Aspect.Additional;
				shapes.Add (shape);
			}

			return shapes.ToArray ();
		}

		protected void PathBuild(DrawingContext drawingContext,
								 out Path pathStart, out bool outlineStart, out bool surfaceStart,
								 out Path pathEnd,   out bool outlineEnd,   out bool surfaceEnd,
								 out Path pathLine, bool simplify, bool forShaper)
		{
			//	Crée les chemins de l'objet.
			//	Le mode forShaper génère des segments nuls lorsque la ligne est ouverte, pour ne pas
			//	perturber le compte des segments avec Geometry.PathExtract et Geometry.DetectOutlineRank.
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
						if (close)  // fermé ?
						{
							pathLine.LineTo(pp1);
							pathLine.Close();
						}
						else if (forShaper)  // ouvert et modeleur ?
						{
							pathLine.MoveTo(pp1);
							pathLine.LineTo(pp1);  // met un segment nul
						}
						first = i;
						pp1 = this.Handle(i).Position;
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
				if (close)  // fermé ?
				{
					pathLine.LineTo(pp1);
					pathLine.Close();
				}
				else if (forShaper)  // ouvert et modeleur ?
				{
					pathLine.MoveTo(pp1);
					pathLine.LineTo(pp1);  // met un segment nul
				}
			}
			else	// coins spéciaux ?
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
						if (close)  // fermé ?
						{
							pathLine.Close();
						}
						else if (forShaper)  // ouvert et modeleur ?
						{
							pathLine.MoveTo(p1);
							pathLine.LineTo(p1);  // met un segment nul
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
					else if ( i < total-1 )  // point intermédiaire ?
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
				if (close)  // fermé ?
				{
					pathLine.Close();
				}
				else if (forShaper)  // ouvert et modeleur ?
				{
					pathLine.MoveTo(p1);
					pathLine.LineTo(p1);  // met un segment nul
				}
			}
		}

		private List<Polygon> GetPolygons()
		{
			var polygons = new List<Polygon> ();
			Polygon polygon = null;

			int total = this.TotalMainHandle;
			for (int i=0; i<total; i++)
			{
				if (this.Handle(i).Type == HandleType.Starting)
				{
					polygon = new Polygon ();
					polygons.Add (polygon);
				}

				if (polygon != null)
				{
					var p = this.Handle (i).Position;
					polygon.Points.Add (p);
				}
			}

			return polygons;
		}

		protected int PrevRank(int rank)
		{
			//	Cherche le rang précédent, en tenant compte
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
			//	Crée le chemin d'un coin.
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


		public override Path GetMagnetPath()
		{
			//	Retourne le chemin géométrique de l'objet pour les constructions
			//	magnétiques.
			Path pathStart, pathEnd, pathLine;
			bool outlineStart, outlineEnd, surfaceStart, surfaceEnd;
			this.PathBuild(null,
						   out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine, true, false);

			return pathLine;
		}

		public override Path GetShaperPath()
		{
			//	Retourne le chemin géométrique de l'objet pour le modeleur.
			Path pathStart, pathEnd, pathLine;
			bool outlineStart, outlineEnd, surfaceStart, surfaceEnd;
			this.PathBuild(null,
						   out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine, true, true);

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
						   out pathLine, false, false);

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

		public void CreateFinalise()
		{
			//	Finalise la création d'un polygone.
			this.HandlePropertiesCreate();  // crée les poignées des propriétés
			this.Select(false);
			this.Select(true);  // pour sélectionner toutes les poignées
		}


		#region Serialization
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise l'objet.
			base.GetObjectData(info, context);
		}

		protected Poly(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui désérialise l'objet.
		}
		#endregion

		
		protected bool				mouseDown = false;
		protected bool				additionalLineExist = false;
		protected Point				additionalLineP1;
		protected Point				additionalLineP2;
		protected Point				initialPos;
	}
}

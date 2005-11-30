using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Objects
{
	/// <summary>
	/// La classe Bezier est la classe de l'objet graphique "courbes de Bézier".
	/// </summary>
	[System.Serializable()]
	public class Bezier : Objects.Abstract
	{
		public Bezier(Document document, Objects.Abstract model) : base(document, model)
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
			return false;
		}

		protected override Objects.Abstract CreateNewObject(Document document, Objects.Abstract model)
		{
			return new Bezier(document, model);
		}

		public override void Dispose()
		{
			base.Dispose();
		}


		// Nom de l'icône.
		public override string IconName
		{
			get { return Misc.Icon("ObjectBezier"); }
		}


		// Détecte si la souris est sur le pourtour de l'objet.
		// Retourne le rang de la poignée de départ, ou -1
		protected int DetectOutline(Point pos)
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;

			Shape[] shapes = this.ShapesBuild(null, context, false);
			int rank = context.Drawer.DetectOutline(pos, context, shapes);
			if ( rank != -1 )  rank *= 3;
			return rank;
		}

		// Déplace tout l'objet.
		public override void MoveAllProcess(Point move)
		{
			this.document.Notifier.NotifyArea(this.BoundingBox);

			bool allHandle = !this.IsSelected;
			int total = this.TotalMainHandle;
			for ( int i=0 ; i<total ; i+=3 )
			{
				if ( allHandle || this.Handle(i+1).IsVisible )
				{
					this.Handle(i+0).Position += move;
					this.Handle(i+1).Position += move;
					this.Handle(i+2).Position += move;
				}
			}
			this.HandlePropertiesUpdate();
			this.SetDirtyBbox();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// Sélectionne toutes les poignées de l'objet dans un rectangle.
		public override void Select(Drawing.Rectangle rect)
		{
			this.InsertOpletSelection();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);

			bool shaper = this.document.Modifier.IsToolShaper;

			int sel = 0;
			int total = this.TotalMainHandle;
			for ( int i=0 ; i<total ; i+=3 )
			{
				if ( rect.Contains(this.Handle(i+1).Position) )
				{
					this.Handle(i+0).Modify(true, false, false);
					this.Handle(i+1).Modify(true, false, false);
					this.Handle(i+2).Modify(true, false, false);
					sel += 3;
				}
				else
				{
					if ( shaper )
					{
						this.Handle(i+0).Modify(true, false, true);
						this.Handle(i+1).Modify(true, false, true);
						this.Handle(i+2).Modify(true, false, true);
					}
					else
					{
						this.Handle(i+0).Modify(false, false, false);
						this.Handle(i+1).Modify(false, false, false);
						this.Handle(i+2).Modify(false, false, false);
					}
				}
			}
			this.selected = ( sel > 0 );
			this.edited = false;
			this.globalSelected = false;
			this.allSelected = (sel == total);
			this.HandlePropertiesUpdate();
			this.SplitProperties();

			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);
			this.document.Notifier.NotifySelectionChanged();
		}


		// Indique si cet objet peut avoir des segments sélectionnés.
		public override bool IsSelectedSegmentPossible
		{
			get { return true; }
		}

		// Donne l'état d'une commande ShaperHandle*.
		public override bool ShaperHandleState(string family, ref bool enable, System.Collections.ArrayList actives)
		{
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
					for ( int i=0 ; i<total ; i+=3 )
					{
						if ( !this.Handle(i+1).IsVisible )  continue;
						if ( this.Handle(i+1).IsShaperDeselected )  continue;
						if ( this.Handle(i+1).Type == HandleType.Starting ||
							 this.Handle(this.NextRank(i)+1).Type == HandleType.Starting )
						{
							enable = true;
						}
					}
				}
				return true;
			}

			if ( family == "Sub" )
			{
				enable = (this.TotalMainHandle > 2*3 && this.IsShaperHandleSelected());
				return true;
			}

			if ( family == "Segment" )
			{
				if ( this.selectedSegments != null && this.selectedSegments.Count != 0 )
				{
					enable = true;
					for ( int i=0 ; i<this.selectedSegments.Count ; i++ )
					{
						SelectedSegment ss = this.selectedSegments[i] as SelectedSegment;
						int rank = ss.Rank*3+2;
						string state = (this.Handle(rank).Type == HandleType.Hide) ? "ToLine" : "ToCurve";
						{
							Abstract.ShaperHandleStateAdd(actives, state);
						}
					}
				}
				return true;
			}

			if ( family == "Curve" )
			{
				if ( this.IsShaperHandleSelected() )
				{
					int total = this.TotalMainHandle;
					for ( int i=0 ; i<total ; i+=3 )
					{
						if ( !this.Handle(i+1).IsVisible )  continue;
						if ( this.Handle(i+1).IsShaperDeselected )  continue;
						if ( this.Handle(i).Type != HandleType.Hide && this.Handle(i+2).Type != HandleType.Hide )
						{
							HandleConstrainType type = this.Handle(i+1).ConstrainType;
							if ( type == HandleConstrainType.Symmetric )  Abstract.ShaperHandleStateAdd(actives, "Sym"); 
							if ( type == HandleConstrainType.Smooth    )  Abstract.ShaperHandleStateAdd(actives, "Smooth"); 
							if ( type == HandleConstrainType.Corner    )  Abstract.ShaperHandleStateAdd(actives, "Dis"); 
							enable = true;
						}
					}
				}
				return true;
			}

			if ( family == "CurveLine" )
			{
				if ( this.IsShaperHandleSelected() )
				{
					int total = this.TotalMainHandle;
					for ( int i=0 ; i<total ; i+=3 )
					{
						if ( !this.Handle(i+1).IsVisible )  continue;
						if ( this.Handle(i+1).IsShaperDeselected )  continue;
						if ( (this.Handle(i).Type == HandleType.Hide) != (this.Handle(i+2).Type == HandleType.Hide) )
						{
							HandleConstrainType type = this.Handle(i+1).ConstrainType;
							if ( type == HandleConstrainType.Smooth )  Abstract.ShaperHandleStateAdd(actives, "Inline"); 
							else                                       Abstract.ShaperHandleStateAdd(actives, "Free"); 
							enable = true;
						}
					}
				}
				return true;
			}

			return base.ShaperHandleState(family, ref enable, actives);
		}

		// Exécute une commande ShaperHandle*.
		public override bool ShaperHandleCommand(string cmd)
		{
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
						this.ShaperHandleAdd(ss.Position, ss.Rank*3);
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

				for ( int i=this.TotalMainHandle-3 ; i>=0 ; i-=3 )
				{
					if ( !this.Handle(i+1).IsVisible )  continue;
					if ( this.Handle(i+1).IsShaperDeselected )  continue;
					if ( this.Handle(i+1).Type == HandleType.Starting ||
						 this.Handle(this.NextRank(i)+1).Type == HandleType.Starting )
					{
						this.ShaperHandleContinue(i+1);
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

				for ( int i=this.TotalMainHandle-3 ; i>=0 ; i-=3 )
				{
					if ( !this.Handle(i+1).IsVisible )  continue;
					if ( this.Handle(i+1).IsShaperDeselected )  continue;
					this.ShaperHandleSub(i+1);
				}

				this.document.Notifier.NotifyArea(this.BoundingBox);
				this.document.Modifier.OpletQueueValidateAction();
				return true;
			}

			if ( cmd == "ShaperHandleToLine" )
			{
				this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.ShaperHandleToLine);
				this.InsertOpletGeometry();
				SelectedSegment.InsertOpletGeometry(this.selectedSegments, this);
				this.document.Notifier.NotifyArea(this.BoundingBox);

				if ( this.selectedSegments != null )
				{
					for ( int i=0 ; i<this.selectedSegments.Count ; i++ )
					{
						SelectedSegment ss = this.selectedSegments[i] as SelectedSegment;
						int rank = ss.Rank*3;
						this.ShaperHandleToLine(rank);
					}
				}
				this.SelectedSegmentClear();

				this.document.Notifier.NotifyArea(this.BoundingBox);
				this.document.Modifier.OpletQueueValidateAction();
				return true;
			}

			if ( cmd == "ShaperHandleToCurve" )
			{
				this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.ShaperHandleToCurve);
				this.InsertOpletGeometry();
				SelectedSegment.InsertOpletGeometry(this.selectedSegments, this);
				this.document.Notifier.NotifyArea(this.BoundingBox);

				if ( this.selectedSegments != null )
				{
					for ( int i=0 ; i<this.selectedSegments.Count ; i++ )
					{
						SelectedSegment ss = this.selectedSegments[i] as SelectedSegment;
						int rank = ss.Rank*3;
						this.ShaperHandleToCurve(rank);
					}
				}
				this.SelectedSegmentClear();

				this.document.Notifier.NotifyArea(this.BoundingBox);
				this.document.Modifier.OpletQueueValidateAction();
				return true;
			}

			if ( cmd == "ShaperHandleSym" )
			{
				this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.ShaperHandleSym);
				this.InsertOpletGeometry();
				this.document.Notifier.NotifyArea(this.BoundingBox);

				int total = this.TotalMainHandle;
				for ( int i=0 ; i<total ; i+=3 )
				{
					if ( !this.Handle(i+1).IsVisible )  continue;
					if ( this.Handle(i+1).IsShaperDeselected )  continue;
					if ( this.Handle(i).Type != HandleType.Hide && this.Handle(i+2).Type != HandleType.Hide )
					{
						this.ShaperHandleSym(i+1);
					}
				}

				this.document.Modifier.OpletQueueValidateAction();
				this.document.Notifier.NotifyArea(this.BoundingBox);
				return true;
			}

			if ( cmd == "ShaperHandleSmooth" )
			{
				this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.ShaperHandleSmooth);
				this.InsertOpletGeometry();
				this.document.Notifier.NotifyArea(this.BoundingBox);

				int total = this.TotalMainHandle;
				for ( int i=0 ; i<total ; i+=3 )
				{
					if ( !this.Handle(i+1).IsVisible )  continue;
					if ( this.Handle(i+1).IsShaperDeselected )  continue;
					if ( this.Handle(i).Type != HandleType.Hide && this.Handle(i+2).Type != HandleType.Hide )
					{
						this.ShaperHandleSmooth(i+1);
					}
				}

				this.document.Notifier.NotifyArea(this.BoundingBox);
				this.document.Modifier.OpletQueueValidateAction();
				return true;
			}

			if ( cmd == "ShaperHandleDis" )
			{
				this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.ShaperHandleDis);
				this.InsertOpletGeometry();
				this.document.Notifier.NotifyArea(this.BoundingBox);

				int total = this.TotalMainHandle;
				for ( int i=0 ; i<total ; i+=3 )
				{
					if ( !this.Handle(i+1).IsVisible )  continue;
					if ( this.Handle(i+1).IsShaperDeselected )  continue;
					if ( this.Handle(i).Type != HandleType.Hide && this.Handle(i+2).Type != HandleType.Hide )
					{
						this.ShaperHandleCorner(i+1);
					}
				}

				this.document.Notifier.NotifyArea(this.BoundingBox);
				this.document.Modifier.OpletQueueValidateAction();
				return true;
			}

			if ( cmd == "ShaperHandleInline" )
			{
				this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.ShaperHandleInline);
				this.InsertOpletGeometry();
				this.document.Notifier.NotifyArea(this.BoundingBox);

				int total = this.TotalMainHandle;
				for ( int i=0 ; i<total ; i+=3 )
				{
					if ( !this.Handle(i+1).IsVisible )  continue;
					if ( this.Handle(i+1).IsShaperDeselected )  continue;
					if ( (this.Handle(i).Type == HandleType.Hide) != (this.Handle(i+2).Type == HandleType.Hide) )
					{
						this.ShaperHandleSmooth(i+1);
					}
				}

				this.document.Notifier.NotifyArea(this.BoundingBox);
				this.document.Modifier.OpletQueueValidateAction();
				return true;
			}

			if ( cmd == "ShaperHandleFree" )
			{
				this.document.Modifier.OpletQueueBeginAction(Res.Strings.Action.ShaperHandleFree);
				this.InsertOpletGeometry();
				this.document.Notifier.NotifyArea(this.BoundingBox);

				int total = this.TotalMainHandle;
				for ( int i=0 ; i<total ; i+=3 )
				{
					if ( !this.Handle(i+1).IsVisible )  continue;
					if ( this.Handle(i+1).IsShaperDeselected )  continue;
					if ( (this.Handle(i).Type == HandleType.Hide) != (this.Handle(i+2).Type == HandleType.Hide) )
					{
						this.ShaperHandleCorner(i+1);
					}
				}

				this.document.Notifier.NotifyArea(this.BoundingBox);
				this.document.Modifier.OpletQueueValidateAction();
				return true;
			}

			return base.ShaperHandleCommand(cmd);
		}

		// Passe le point en mode symétrique.
		protected void ShaperHandleSym(int rank)
		{
			this.Handle(rank).ConstrainType = HandleConstrainType.Symmetric;
			this.MoveSecondary(rank, rank-1, rank+1, this.Handle(rank-1).Position);
			this.SetDirtyBbox();
		}

		// Passe le point en mode lisse.
		protected void ShaperHandleSmooth(int rank)
		{
			this.Handle(rank).ConstrainType = HandleConstrainType.Smooth;

			if ( this.Handle(rank-1).Type == HandleType.Hide || this.Handle(rank+1).Type == HandleType.Hide )
			{
				this.MovePrimary(rank, this.Handle(rank).Position);
			}
			else
			{
				this.MoveSecondary(rank, rank-1, rank+1, this.Handle(rank-1).Position);
			}
			this.SetDirtyBbox();
		}

		// Passe le point en mode anguleux.
		protected void ShaperHandleCorner(int rank)
		{
			this.Handle(rank).ConstrainType = HandleConstrainType.Corner;
		}

		// Prolonge la courbe.
		protected void ShaperHandleContinue(int rank)
		{
			HandleType type = this.Handle(rank).Type;
			this.Handle(rank).Type = HandleType.Primary;

			int prev1, prev2, sec1a, sec1b, ins1, ins2, ins3;
			if ( type == HandleType.Starting )  // insère au début ?
			{
				sec1a = rank-1;
				prev1 = rank;
				sec1b = rank+1;
				prev2 = rank+3;

				ins1  = rank-1;
				ins2  = rank;
				ins3  = rank+1;
			}
			else	// insère à la fin ?
			{
				sec1a = rank+1;
				prev1 = rank;
				sec1b = rank-1;
				prev2 = rank-3;

				ins1  = rank+2;
				ins2  = rank+2;
				ins3  = rank+2;
			}

			Handle handle;
			if ( this.Handle(sec1b).Type == HandleType.Hide )
			{
				double d = 20.0/this.document.Modifier.ActiveViewer.DrawingContext.ScaleX;
				Point pos = Point.Move(this.Handle(prev1).Position, this.Handle(prev2).Position, -d);
				Point p1 = pos+(this.Handle(sec1a).Position-this.Handle(prev1).Position);

				this.Handle(sec1a).Type = HandleType.Hide;
				this.Handle(sec1a).Position = this.Handle(prev1).Position;

				handle = new Handle(this.document);
				handle.Position = p1;
				handle.Type = HandleType.Bezier;
				handle.IsVisible = true;
				this.HandleInsert(ins1, handle);

				handle = new Handle(this.document);
				handle.Position = pos;
				handle.Type = type;
				handle.IsVisible = true;
				this.HandleInsert(ins2, handle);

				handle = new Handle(this.document);
				handle.Position = pos;
				handle.Type = HandleType.Hide;
				handle.IsVisible = true;
				this.HandleInsert(ins3, handle);
			}
			else
			{
				Point sec = this.Handle(sec1a).Position;
				if ( this.Handle(prev1).Position == sec )
				{
					double d = 20.0/this.document.Modifier.ActiveViewer.DrawingContext.ScaleX;
					sec = Point.Move(this.Handle(prev1).Position, this.Handle(prev2).Position, -d);
				}
				Point pos = Point.Scale(this.Handle(prev1).Position, sec, 3.0);
				Point s1  = Point.Scale(this.Handle(prev1).Position, sec, 4.0);
				Point s2  = Point.Scale(this.Handle(prev1).Position, sec, 2.0);

				this.Handle(sec1a).Position = sec;

				handle = new Handle(this.document);
				handle.Position = s1;
				handle.Type = HandleType.Bezier;
				handle.IsVisible = true;
				this.HandleInsert(ins1, handle);

				handle = new Handle(this.document);
				handle.Position = pos;
				handle.Type = type;
				handle.ConstrainType = HandleConstrainType.Smooth;
				handle.IsVisible = true;
				this.HandleInsert(ins2, handle);

				handle = new Handle(this.document);
				handle.Position = s2;
				handle.Type = HandleType.Bezier;
				handle.IsVisible = true;
				this.HandleInsert(ins3, handle);
			}

			this.HandlePropertiesUpdate();
		}

		// Ajoute une poignée sans changer l'aspect de la courbe.
		protected void ShaperHandleAdd(Point pos, int rank)
		{
			for ( int i=0 ; i<3 ; i++ )
			{
				Handle handle = new Handle(this.document);
				handle.Position = pos;
				handle.Type = (i==1) ? HandleType.Primary : HandleType.Bezier;
				handle.IsVisible = true;
				this.HandleInsert(rank+3, handle);
			}

			int prev = rank+0;
			int curr = this.NextRank(rank);
			int next = this.NextRank(curr);

			if ( this.Handle(prev+2).Type == HandleType.Hide && this.Handle(next+0).Type == HandleType.Hide )
			{
				pos = Point.Projection(this.Handle(prev+1).Position, this.Handle(next+1).Position, pos);
				this.Handle(curr+0).Position = pos;
				this.Handle(curr+1).Position = pos;
				this.Handle(curr+2).Position = pos;
				this.Handle(curr+0).Type = HandleType.Hide;
				this.Handle(curr+2).Type = HandleType.Hide;
			}
			else
			{
				double t = Point.FindBezierParameter(this.Handle(prev+1).Position, this.Handle(prev+2).Position, this.Handle(next+0).Position, this.Handle(next+1).Position, pos);
				this.Handle(curr+1).Position = Point.FromBezier(this.Handle(prev+1).Position, this.Handle(prev+2).Position, this.Handle(next+0).Position, this.Handle(next+1).Position, t);
				pos = Point.Scale(this.Handle(prev+2).Position, this.Handle(next+0).Position, t);
				this.Handle(prev+2).Position = Point.Scale(this.Handle(prev+1).Position, this.Handle(prev+2).Position, t);
				this.Handle(next+0).Position = Point.Scale(this.Handle(next+1).Position, this.Handle(next+0).Position, 1-t);
				this.Handle(curr+0).Position = Point.Scale(this.Handle(prev+2).Position, pos, t);
				this.Handle(curr+2).Position = Point.Scale(this.Handle(next+0).Position, pos, 1-t);

				this.Handle(curr+1).ConstrainType = HandleConstrainType.Smooth;
				if ( this.Handle(prev+1).ConstrainType == HandleConstrainType.Symmetric )  this.Handle(prev+1).ConstrainType = HandleConstrainType.Smooth;
				if ( this.Handle(next+1).ConstrainType == HandleConstrainType.Symmetric )  this.Handle(next+1).ConstrainType = HandleConstrainType.Smooth;
			}
			this.SetDirtyBbox();
			this.HandlePropertiesUpdate();
		}

		// Supprime une poignée sans trop changer l'aspect de la courbe.
		protected void ShaperHandleSub(int rank)
		{
			bool starting = (this.Handle(rank).Type == HandleType.Starting);

			this.HandleDelete(rank-1);
			this.HandleDelete(rank-1);
			this.HandleDelete(rank-1);

			// Il doit toujours y avoir une poignée de départ !
			if ( starting )
			{
				this.Handle(rank).Type = HandleType.Starting;
			}

			int prev = this.PrevRank(rank-1);
			int next = this.NextRank(prev);

			if ( this.Handle(prev+2).Type == HandleType.Hide || this.Handle(next+0).Type == HandleType.Hide )
			{
				if ( this.Handle(prev+2).Type != this.Handle(next+0).Type )
				{
					this.ShaperHandleToCurve(prev);
				}
			}
			this.SetDirtyBbox();
			this.HandlePropertiesUpdate();
		}

		// Conversion d'un segement en ligne droite.
		protected void ShaperHandleToLine(int rank)
		{
			int next = this.NextRank(rank);
			this.Handle(rank+2).Position = this.Handle(rank+1).Position;
			this.Handle(next+0).Position = this.Handle(next+1).Position;
			this.Handle(rank+2).Type = HandleType.Hide;
			this.Handle(next+0).Type = HandleType.Hide;
			this.Handle(rank+1).ConstrainType = HandleConstrainType.Corner;
			this.Handle(next+1).ConstrainType = HandleConstrainType.Corner;
			this.SetDirtyBbox();
			this.HandlePropertiesUpdate();
		}

		// Conversion d'un segement en courbe.
		protected void ShaperHandleToCurve(int rank)
		{
			int next = this.NextRank(rank);
			this.Handle(rank+2).Position = Point.Scale(this.Handle(rank+1).Position, this.Handle(next+1).Position, 0.25);
			this.Handle(next+0).Position = Point.Scale(this.Handle(next+1).Position, this.Handle(rank+1).Position, 0.25);
			this.Handle(rank+2).Type = HandleType.Bezier;
			this.Handle(next+0).Type = HandleType.Bezier;
			this.Handle(rank+1).ConstrainType = HandleConstrainType.Corner;
			this.Handle(next+1).ConstrainType = HandleConstrainType.Corner;
			this.SetDirtyBbox();
			this.HandlePropertiesUpdate();
		}


		// Indique si au moins une poignée est sélectionnée par le modeleur.
		public override bool IsShaperHandleSelected()
		{
			int total = this.TotalMainHandle;
			for ( int i=0 ; i<total ; i+=3 )
			{
				Handle handle = this.Handle(i+1);
				if ( !handle.IsVisible )  continue;

				if ( !handle.IsShaperDeselected )  return true;
			}
			return false;
		}

		// Retourne la liste des positions des poignées sélectionnées par le modeleur.
		public override System.Collections.ArrayList MoveSelectedHandles()
		{
			this.InsertOpletGeometry();

			System.Collections.ArrayList startingPos = new System.Collections.ArrayList();
			int total = this.TotalMainHandle;
			for ( int i=0 ; i<total ; i+=3 )
			{
				if ( !this.Handle(i+1).IsVisible )  continue;

				if ( !this.Handle(i+1).IsShaperDeselected )
				{
					startingPos.Add(this.Handle(i+0).Position);
					startingPos.Add(this.Handle(i+1).Position);
					startingPos.Add(this.Handle(i+2).Position);
				}
			}
			if ( startingPos.Count == 0 )  return null;

			if ( this.selectedSegments != null )
			{
				SelectedSegment.InsertOpletGeometry(this.selectedSegments, this);
			}

			return startingPos;
		}

		// Déplace toutes les poignées sélectionnées par le modeleur.
		public override void MoveSelectedHandles(System.Collections.ArrayList startingPos, Point move)
		{
			this.document.Notifier.NotifyArea(this.BoundingBox);

			int s = 0;
			int total = this.TotalMainHandle;
			for ( int i=0 ; i<total ; i+=3 )
			{
				if ( !this.Handle(i+1).IsVisible )  continue;

				if ( !this.Handle(i+1).IsShaperDeselected )
				{
					this.Handle(i+0).Position = ((Point)startingPos[s++]) + move;
					this.Handle(i+1).Position = ((Point)startingPos[s++]) + move;
					this.Handle(i+2).Position = ((Point)startingPos[s++]) + move;
				}
			}

			if ( this.selectedSegments != null )
			{
				SelectedSegment.Update(this.selectedSegments, this);
			}

			this.SetDirtyBbox();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}


		// Adapte le point secondaire s'il est en mode "en ligne".
		protected void AdaptPrimaryLine(int rankPrimary, int rankSecondary, out int rankExtremity)
		{
			if ( rankSecondary > rankPrimary )
			{
				rankExtremity = this.PrevRank(rankPrimary-1)+1;
			}
			else
			{
				rankExtremity = this.NextRank(rankPrimary-1)+1;
			}

			if ( this.Handle(rankPrimary).ConstrainType != HandleConstrainType.Smooth )  return;
			int rankOpposite = rankPrimary - (rankSecondary-rankPrimary);
			if ( this.Handle(rankOpposite).Type != HandleType.Hide )  return;

			double dist = Point.Distance(this.Handle(rankPrimary).Position, this.Handle(rankSecondary).Position);
			Point pos = new Point();
			pos = Point.Move(this.Handle(rankPrimary).Position, this.Handle(rankExtremity).Position, dist);
			pos = Point.Symmetry(this.Handle(rankPrimary).Position, pos);
			this.Handle(rankSecondary).Position = pos;
			this.SetDirtyBbox();
			this.HandlePropertiesUpdate();
		}

		// Cherche le rang du groupe "sps" précédent, en tenant compte
		// des ensembles Starting-Primary(s).
		protected int PrevRank(int rank)
		{
			System.Diagnostics.Debug.Assert(rank%3 == 0);
			if ( rank == 0 || this.Handle(rank+1).Type == HandleType.Starting )
			{
				do
				{
					rank += 3;
				}
				while ( rank < this.TotalMainHandle && this.Handle(rank+1).Type != HandleType.Starting );
			}
			rank -= 3;
			return rank;
		}

		// Cherche le rang du groupe "sps" suivant, en tenant compte
		// des ensembles Starting-Primary(s).
		protected int NextRank(int rank)
		{
			System.Diagnostics.Debug.Assert(rank%3 == 0);
			rank += 3;
			if ( rank >= this.TotalMainHandle || this.Handle(rank+1).Type == HandleType.Starting )
			{
				do
				{
					rank -= 3;
				}
				while ( rank > 0 && this.Handle(rank+1).Type != HandleType.Starting );
			}
			return rank;
		}

		// Déplace une poignée primaire selon les contraintes.
		protected void MovePrimary(int rank, Point pos)
		{
			Point move = pos-this.Handle(rank).Position;
			this.Handle(rank).Position = pos;
			this.Handle(rank-1).Position += move;
			this.Handle(rank+1).Position += move;
			if ( this.Handle(rank-1).Type == HandleType.Hide )  this.Handle(rank-1).Position = pos;
			if ( this.Handle(rank+1).Type == HandleType.Hide )  this.Handle(rank+1).Position = pos;

			int rankExtremity;
			this.AdaptPrimaryLine(rank, rank-1, out rankExtremity);
			this.AdaptPrimaryLine(rankExtremity, rankExtremity+1, out rankExtremity);
			this.AdaptPrimaryLine(rank, rank+1, out rankExtremity);
			this.AdaptPrimaryLine(rankExtremity, rankExtremity-1, out rankExtremity);
			this.SetDirtyBbox();
		}

		// Déplace une poignée secondaire selon les contraintes.
		protected void MoveSecondary(int rankPrimary, int rankSecondary, int rankOpposite, Point pos)
		{
			HandleConstrainType type = this.Handle(rankPrimary).ConstrainType;

			this.Handle(rankSecondary).Position = pos;

			if ( this.Handle(rankOpposite).Type == HandleType.Hide )  // droite?
			{
				if ( type == HandleConstrainType.Smooth )
				{
					if ( rankOpposite > rankSecondary )
					{
						rankOpposite += 2;
						if ( rankOpposite >= this.TotalMainHandle )  rankOpposite = 1;
					}
					else
					{
						rankOpposite -= 2;
						if ( rankOpposite < 0 )  rankOpposite = this.TotalMainHandle-2;
					}
					this.Handle(rankSecondary).Position = Point.Projection(this.Handle(rankPrimary).Position, this.Handle(rankOpposite).Position, pos);
				}
			}
			else	// courbe ?
			{
				if ( type == HandleConstrainType.Symmetric )
				{
					this.Handle(rankOpposite).Position = Point.Symmetry(this.Handle(rankPrimary).Position, this.Handle(rankSecondary).Position);
				}

				if ( type == HandleConstrainType.Smooth )
				{
					double dist = Point.Distance(this.Handle(rankPrimary).Position, this.Handle(rankOpposite).Position);
					Point p = Point.Move(this.Handle(rankPrimary).Position, this.Handle(rankSecondary).Position, dist);
					this.Handle(rankOpposite).Position = Point.Symmetry(this.Handle(rankPrimary).Position, p);
				}
			}
		}

		// Début du déplacement une poignée.
		public override void MoveHandleStarting(int rank, Point pos, DrawingContext drawingContext)
		{
			base.MoveHandleStarting(rank, pos, drawingContext);

			if ( rank < this.handles.Count )  // poignée de l'objet ?
			{
				drawingContext.ConstrainFlush();

				Handle handle = this.Handle(rank);
				if ( handle.PropertyType == Properties.Type.None )
				{
					int prev = this.PrevRank(rank/3*3);
					int next = this.NextRank(rank/3*3);

					if ( rank%3 == 1 )  // poigné principale ?
					{
						if ( this.Handle(rank-1).Type == HandleType.Hide )
						{
							drawingContext.ConstrainAddLine(this.Handle(rank).Position, this.Handle(prev+1).Position);
							drawingContext.ConstrainAddHV(this.Handle(prev+1).Position);
							drawingContext.ConstrainAddCircle(this.Handle(prev+1).Position, this.Handle(rank).Position);
						}

						if ( this.Handle(rank+1).Type == HandleType.Hide )
						{
							drawingContext.ConstrainAddLine(this.Handle(rank).Position, this.Handle(next+1).Position);
							drawingContext.ConstrainAddHV(this.Handle(next+1).Position);
							drawingContext.ConstrainAddCircle(this.Handle(next+1).Position, this.Handle(rank).Position);
						}

						drawingContext.ConstrainAddHV(this.Handle(rank).Position);
					}
					else	// poignée secondaire ?
					{
						pos = this.Handle((rank/3)*3+1).Position;
						drawingContext.ConstrainAddLine(this.Handle(rank).Position, pos);
						drawingContext.ConstrainAddHV(pos);

						if ( rank%3 == 0 )
						{
							if ( this.Handle(rank+2).Type == HandleType.Hide )
							{
								drawingContext.ConstrainAddLine(this.Handle(rank+1).Position, this.Handle(next+1).Position);
							}
							drawingContext.ConstrainAddCircle(this.Handle(rank+1).Position, this.Handle(rank).Position);
						}

						if ( rank%3 == 2 )
						{
							if ( this.Handle(rank-2).Type == HandleType.Hide )
							{
								drawingContext.ConstrainAddLine(this.Handle(rank-1).Position, this.Handle(prev+1).Position);
							}
							drawingContext.ConstrainAddCircle(this.Handle(rank-1).Position, this.Handle(rank).Position);
						}
					}
				}
				else
				{
					Properties.Abstract property = this.Property(handle.PropertyType);
					property.MoveHandleStarting(this, handle.PropertyRank, pos, drawingContext);
				}

				drawingContext.MagnetClearStarting();
			}
		}

		// Déplace une poignée.
		public override void MoveHandleProcess(int rank, Point pos, DrawingContext drawingContext)
		{
			if ( rank >= this.TotalMainHandle )  // poignée d'une propriété ?
			{
				base.MoveHandleProcess(rank, pos, drawingContext);
				return;
			}

			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapPos(ref pos);

			if ( this.Handle(rank).Type == HandleType.Primary  ||  // principale ?
				 this.Handle(rank).Type == HandleType.Starting )
			{
				this.MovePrimary(rank, pos);
			}
			else if ( this.Handle(rank).Type == HandleType.Bezier )  // secondaire ?
			{
				if ( rank%3 == 0 )  // poignée secondaire ?
				{
					this.MoveSecondary(rank+1, rank, rank+2, pos);
				}
				if ( rank%3 == 2 )  // poignée secondaire ?
				{
					this.MoveSecondary(rank-1, rank, rank-2, pos);
				}
			}

			this.HandlePropertiesUpdate();
			this.SetDirtyBbox();
			this.TextInfoModif(pos, rank);
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}


		// Début du déplacement d'une poignée d'un segment sélectionné.
		public override void MoveSelectedSegmentStarting(int rank, Point pos, DrawingContext drawingContext)
		{
			base.MoveSelectedSegmentStarting(rank, pos, drawingContext);

			this.initialPos = pos;
			SelectedSegment ss = this.selectedSegments[rank] as SelectedSegment;
			int rp1 = ss.Rank*3+1;
			int rs1 = ss.Rank*3+2;
			int rs2 = this.NextRank(ss.Rank*3)+0;
			int rp2 = this.NextRank(ss.Rank*3)+1;

			if ( (this.Handle(rs1).Type == HandleType.Hide) )  // droite ?
			{
				this.Handle(rp1).InitialPosition = this.Handle(rp1).Position;
				this.Handle(rp2).InitialPosition = this.Handle(rp2).Position;

				drawingContext.ConstrainFlush();
				drawingContext.ConstrainAddHV(ss.Position);
				Point p1 = this.Handle(rp1).Position;
				Point p2 = this.Handle(rp2).Position;
				Size d = new Size(p2.X-p1.X, p2.Y-p1.Y);
				drawingContext.ConstrainAddLine(p1, p2);
				drawingContext.ConstrainAddLine(pos, new Point(pos.X-d.Height, pos.Y+d.Width));
			}
			else	// courbe ?
			{
				this.Handle(rs1).InitialPosition = this.Handle(rs1).Position;
				this.Handle(rs2).InitialPosition = this.Handle(rs2).Position;

				drawingContext.ConstrainFlush();
				drawingContext.ConstrainAddHV(ss.Position);
				double a1 = Point.ComputeAngleDeg(this.Handle(rp1).Position, this.Handle(rs1).Position);
				double a2 = Point.ComputeAngleDeg(this.Handle(rp2).Position, this.Handle(rs2).Position);
				Point c = ss.Position;
				Point p = Transform.RotatePointDeg(c, (a1+a2)/2, new Point(c.X+100, c.Y));
				drawingContext.ConstrainAddLine(c, p);
			}
		}

		// Déplace une poignée d'un segment sélectionné.
		public override void MoveSelectedSegmentProcess(int rank, Point pos, DrawingContext drawingContext)
		{
			this.document.Notifier.NotifyArea(this.BoundingBox);

			drawingContext.SnapPos(ref pos);
			Point move = pos-this.initialPos;

			SelectedSegment ss = this.selectedSegments[rank] as SelectedSegment;
			int rp1 = ss.Rank*3+1;
			int rs1 = ss.Rank*3+2;
			int rs2 = this.NextRank(ss.Rank*3)+0;
			int rp2 = this.NextRank(ss.Rank*3)+1;

			if ( (this.Handle(rs1).Type == HandleType.Hide) )  // droite ?
			{
				this.MovePrimary(rp1, this.Handle(rp1).InitialPosition+move);
				this.MovePrimary(rp2, this.Handle(rp2).InitialPosition+move);
			}
			else	// courbe ?
			{
				this.MoveSecondary(rs1-1, rs1, rs1-2, this.Handle(rs1).InitialPosition+move);
				this.MoveSecondary(rs2+1, rs2, rs2+2, this.Handle(rs2).InitialPosition+move);
			}

			SelectedSegment.Update(this.selectedSegments, this);

			this.SetDirtyBbox();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// Fin du déplacement d'une poignée d'un segment sélectionné.
		public override void MoveSelectedSegmentEnding(int rank, Point pos, DrawingContext drawingContext)
		{
			base.MoveSelectedSegmentEnding(rank, pos, drawingContext);
		}

		
		// Déplace globalement l'objet.
		public override void MoveGlobalProcess(Selector selector)
		{
			base.MoveGlobalProcess(selector);
			this.HandlePropertiesUpdate();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// Aligne l'objet sur la grille.
		public override void AlignGrid(DrawingContext drawingContext)
		{
			this.InsertOpletGeometry();
			this.document.Notifier.NotifyArea(this.BoundingBox);

			int total = this.handles.Count;
			for ( int i=0 ; i<total ; i+=3 )
			{
				if ( this.Handle(i+1).IsVisible )
				{
					Point pos = this.Handle(i+1).Position;
					drawingContext.SnapGridForce(ref pos);
					Point move = pos-this.Handle(i+1).Position;
					this.Handle(i+0).Position += move;
					this.Handle(i+1).Position += move;
					this.Handle(i+2).Position += move;
				}
			}

			this.HandlePropertiesUpdate();
			this.SetDirtyBbox();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}


		// Début de la création d'un objet.
		public override void CreateMouseDown(Point pos, DrawingContext drawingContext)
		{
			drawingContext.ConstrainFlush();
			drawingContext.ConstrainAddHV(pos);

			this.isCreating = true;
			bool ignore = false;

			if ( this.TotalHandle == 0 )
			{
				this.lastPoint = false;
				this.ChangePropertyPolyClose(false);
			}
			else
			{
				double len = Point.Distance(pos, this.Handle(1).Position);
				if ( len <= drawingContext.CloseMargin )
				{
					if ( this.TotalHandle == 3 )
					{
						ignore = true;
					}
					else
					{
						pos = this.Handle(1).Position;
						this.lastPoint = true;
						this.ChangePropertyPolyClose(true);
					}
				}
				else
				{
					len = Point.Distance(pos, this.Handle(this.TotalHandle-2).Position);
					if ( len <= drawingContext.CloseMargin )
					{
						ignore = true;
					}
				}
			}

			if ( !this.lastPoint && !ignore )
			{
				if ( this.TotalHandle == 0 )
				{
					this.HandleAdd(pos, HandleType.Bezier);
					this.HandleAdd(pos, HandleType.Starting, HandleConstrainType.Smooth);
					this.HandleAdd(pos, HandleType.Bezier);
				}
				else
				{
					this.HandleAdd(pos, HandleType.Bezier);
					this.HandleAdd(pos, HandleType.Primary, HandleConstrainType.Smooth);
					this.HandleAdd(pos, HandleType.Bezier);

					int rank = this.TotalHandle-6;
					if ( this.Handle(rank).Type == HandleType.Hide )
					{
						this.Handle(rank+2).Position = Point.Scale(this.Handle(rank+1).Position, pos, 0.5);
					}
				}
				this.Handle(this.TotalHandle-2).IsVisible = true;
			}

			this.mouseDown = true;
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// Déplacement pendant la création d'un objet.
		public override void CreateMouseMove(Point pos, DrawingContext drawingContext)
		{
			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapPos(ref pos);

			if ( this.mouseDown )
			{
				int rank;
				if ( this.lastPoint )
				{
					rank = 1;
				}
				else
				{
					rank = this.TotalHandle-2;
				}
				this.Handle(rank+1).Position = pos;
				this.Handle(rank-1).Position = Point.Symmetry(this.Handle(rank).Position, pos);
				this.SetDirtyBbox();
				this.TextInfoModif(pos, rank);
			}
			else
			{
				if ( this.TotalHandle > 0 )
				{
					double len = Point.Distance(pos, this.Handle(1).Position);
					if ( len <= drawingContext.CloseMargin )
					{
						this.Handle(1).Type = HandleType.Ending;
					}
					else
					{
						this.Handle(1).Type = HandleType.Starting;
					}
				}
				this.document.Modifier.TextInfoModif = "";
			}

			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// Fin de la création d'un objet.
		public override void CreateMouseUp(Point pos, DrawingContext drawingContext)
		{
			this.document.Notifier.NotifyArea(this.BoundingBox);

			int rank = this.TotalHandle;
			double len = Point.Distance(this.Handle(rank-1).Position, this.Handle(rank-2).Position);
			if ( rank <= 3 )
			{
				if ( len <= drawingContext.MinimalSize )
				{
					rank -= 3;
					this.Handle(rank+0).Position = this.Handle(rank+1).Position;
					this.Handle(rank+2).Position = this.Handle(rank+1).Position;
					this.Handle(rank+0).Type = HandleType.Hide;
					this.Handle(rank+2).Type = HandleType.Hide;
					this.Handle(rank+1).ConstrainType = HandleConstrainType.Corner;
				}
			}
			else
			{
				if ( len <= drawingContext.MinimalSize )
				{
					this.ShaperHandleToLine(rank-6);
				}
			}

			drawingContext.ConstrainDelStarting();
			drawingContext.MagnetClearStarting();
			this.mouseDown = false;
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// Indique si la création de l'objet est terminée.
		public override bool CreateIsEnding(DrawingContext drawingContext)
		{
			if ( this.lastPoint )
			{
				this.Handle(1).Type = HandleType.Starting;
				this.Deselect();
				this.HandlePropertiesCreate();
				this.HandlePropertiesUpdate();
				this.isCreating = false;
				this.document.Modifier.TextInfoModif = "";
				return true;
			}
			else
			{
				return false;
			}
		}

		// Indique si l'objet doit exister. Retourne false si l'objet ne peut
		// pas exister et doit être détruit.
		public override bool CreateIsExist(DrawingContext drawingContext)
		{
			return true;
		}

		// Termine la création de l'objet. Retourne false si l'objet ne peut
		// pas exister et doit être détruit.
		public override bool CreateEnding(DrawingContext drawingContext)
		{
			this.isCreating = false;
			this.document.Modifier.TextInfoModif = "";

			int total = this.TotalHandle;
			if ( total <= 3 )  return false;

			this.Handle(1).Type = HandleType.Starting;
			this.Deselect();
			this.HandlePropertiesCreate();
			this.HandlePropertiesUpdate();
			return true;
		}

		// Texte des informations de modification.
		protected void TextInfoModif(Point mouse, int rank)
		{
			int r1, r2;
			if ( this.isCreating )
			{
				r1 = this.TotalHandle-2;
				r2 = this.TotalHandle-1;
			}
			else
			{
				if ( rank%3 == 0 )
				{
					r1 = rank;
					r2 = rank+1;
				}
				else if ( rank%3 == 2 )
				{
					r1 = rank;
					r2 = rank-1;
				}
				else
				{
					this.document.Modifier.TextInfoModif = "";
					return;
				}
			}

			Point p1 = this.Handle(r1).Position;
			Point p2 = this.Handle(r2).Position;
			double len = Point.Distance(p1, p2);
			double angle = Point.ComputeAngleDeg(p1, p2);
			string text = string.Format(Res.Strings.Object.Bezier.Info, this.document.Modifier.RealToString(len), this.document.Modifier.AngleToString(angle));
			this.document.Modifier.TextInfoModif = text;
		}

		// Retourne un bouton d'action pendant la création.
		public override bool CreateAction(int rank, out string cmd, out string name, out string text)
		{
			if ( rank == 0 )
			{
				cmd  = "Object";
				name = "CreateEnding";
				text = string.Format("     {0}  {1}", Misc.Image("CreateEnding"), Res.Strings.Object.Button.CreateEnding);
				return true;
			}
			if ( rank == 1 )
			{
				cmd  = "Object";
				name = "CreateAndSelect";
				text = string.Format("     {0}  {1}", Misc.Image("CreateAndSelect"), Res.Strings.Object.Button.CreateAndSelect);
				return true;
			}
			if ( rank == 2 )
			{
				cmd  = "Object";
				name = "CreateAndShaper";
				text = string.Format("     {0}  {1}", Misc.Image("CreateAndShaper"), Res.Strings.Object.Button.CreateAndShaper);
				return true;
			}
			return base.CreateAction(rank, out cmd, out name, out text);
		}

		
		// Constuit les formes de l'objet.
		public override Shape[] ShapesBuild(IPaintPort port, DrawingContext drawingContext, bool simplify)
		{
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
			
			bool support = false;
			if ( (this.IsSelected || this.isCreating) &&
				 drawingContext != null && drawingContext.IsActive &&
				 !this.IsGlobalSelected )
			{
				support = true;
				totalShapes ++;
			}

			Shape[] shapes = new Shape[totalShapes];
			int i = 0;
			
			// Forme de la surface principale.
			shapes[i] = new Shape();
			shapes[i].Path = pathLine;
			shapes[i].SetPropertySurface(port, this.PropertyFillGradient);
			i ++;

			// Forme du chemin principal.
			shapes[i] = new Shape();
			shapes[i].Path = pathLine;
			shapes[i].SetPropertyStroke(port, this.PropertyLineMode, this.PropertyLineColor);
			i ++;

			// Forme de la surface de départ.
			if ( surfaceStart )
			{
				shapes[i] = new Shape();
				shapes[i].Path = pathStart;
				shapes[i].SetPropertySurface(port, this.PropertyLineColor);
				shapes[i].IsMisc = true;
				i ++;
			}

			// Forme de la surface d'arrivée.
			if ( surfaceEnd )
			{
				shapes[i] = new Shape();
				shapes[i].Path = pathEnd;
				shapes[i].SetPropertySurface(port, this.PropertyLineColor);
				shapes[i].IsMisc = true;
				i ++;
			}

			// Forme du chemin de départ.
			if ( outlineStart )
			{
				shapes[i] = new Shape();
				shapes[i].Path = pathStart;
				shapes[i].SetPropertyStroke(port, this.PropertyLineMode, this.PropertyLineColor);
				shapes[i].IsMisc = true;
				i ++;
			}

			// Forme du chemin d'arrivée.
			if ( outlineEnd )
			{
				shapes[i] = new Shape();
				shapes[i].Path = pathEnd;
				shapes[i].SetPropertyStroke(port, this.PropertyLineMode, this.PropertyLineColor);
				shapes[i].IsMisc = true;
				i ++;
			}

			// Forme des traits de support pour les poignées secondaires.
			if ( support )
			{
				Path pathSupport = new Path();
				int total = this.TotalMainHandle;
				for ( int j=0 ; j<total ; j+=3 )
				{
					if ( !this.Handle(j+1).IsVisible )  continue;
					pathSupport.MoveTo(this.Handle(j+1).Position);
					pathSupport.LineTo(this.Handle(j+0).Position);
					pathSupport.MoveTo(this.Handle(j+1).Position);
					pathSupport.LineTo(this.Handle(j+2).Position);
				}

				shapes[i] = new Shape();
				shapes[i].Path = pathSupport;
				shapes[i].SetPropertyStroke(port, this.PropertyLineMode, this.PropertyLineColor);
				shapes[i].Aspect = Aspect.Support;
				shapes[i].IsVisible = true;
				i ++;
			}

			return shapes;
		}

		// Calcule les points de la droite à une extrémité de la courbe.
		public void ComputeExtremity(bool start, out Point a, out Point b)
		{
			int r1 = start ? 1 : this.TotalMainHandle-2;
			int r2 = start ? 2 : this.TotalMainHandle-3;

			if ( this.Handle(r2).Type == HandleType.Hide )
			{
				r2 = start ? 4 : this.TotalMainHandle-5;
			}

			a = this.Handle(r1).Position;
			b = this.Handle(r2).Position;
		}

		// Crée les chemins de l'objet.
		protected void PathBuild(DrawingContext drawingContext,
								 out Path pathStart, out bool outlineStart, out bool surfaceStart,
								 out Path pathEnd,   out bool outlineEnd,   out bool surfaceEnd,
								 out Path pathLine, bool simplify)
		{
			pathStart = new Path();
			pathEnd   = new Path();
			pathLine  = new Path();

			double zoom = Properties.Abstract.DefaultZoom(drawingContext);
			pathStart.DefaultZoom = zoom;
			pathEnd.DefaultZoom = zoom;
			pathLine.DefaultZoom = zoom;

			int total = this.TotalMainHandle;
			if ( total < 6 )
			{
				outlineStart = false;
				surfaceStart = false;
				outlineEnd   = false;
				surfaceEnd   = false;
				return;
			}

			Point p1, p2, pp1, pp2;
			double w = this.PropertyLineMode.Width;
			CapStyle cap = this.PropertyLineMode.Cap;
			this.ComputeExtremity(true, out p1, out p2);
			pp1 = this.PropertyArrow.PathExtremity(pathStart, 0, w,cap, p1,p2, simplify, out outlineStart, out surfaceStart);
			this.ComputeExtremity(false, out p1, out p2);
			pp2 = this.PropertyArrow.PathExtremity(pathEnd,   1, w,cap, p1,p2, simplify, out outlineEnd,   out surfaceEnd);

			int first = 0;
			for ( int i=0 ; i<total ; i+=3 )
			{
				if ( i == 0 )  // premier point ?
				{
					pathLine.MoveTo(pp1);
				}
				else if ( this.Handle(i+1).Type == HandleType.Starting )  // premier point ?
				{
					if ( this.PropertyPolyClose.BoolValue )  // fermé ?
					{
						this.PathPutSegment(pathLine, i-1, first, pp1);
						pathLine.Close();
					}
					first = i;
					pp1 = this.Handle(i+1).Position;
					pathLine.MoveTo(pp1);
				}
				else if ( i < total-3 )  // point intermédiaire ?
				{
					this.PathPutSegment(pathLine, i-1, i, this.Handle(i+1).Position);
				}
				else	// dernier point ?
				{
					this.PathPutSegment(pathLine, i-1, i, pp2);
				}
			}
			if ( this.PropertyPolyClose.BoolValue )  // fermé ?
			{
				this.PathPutSegment(pathLine, total-1, first, pp1);
				pathLine.Close();
			}
		}

		// Ajoute un segment de courbe ou de droite dans le chemin.
		protected void PathPutSegment(Path path, int rankS1, int rankS2, Point p2)
		{
			if ( this.Handle(rankS1).Type == HandleType.Hide &&
				 this.Handle(rankS2).Type == HandleType.Hide )
			{
				path.LineTo(p2);
			}
			else
			{
				path.CurveTo(this.Handle(rankS1).Position, this.Handle(rankS2).Position, p2);
			}
		}


		#region CreateFromPath
		// Retourne le chemin géométrique de l'objet pour les constructions
		// magnétiques.
		public override Path GetMagnetPath()
		{
			Path pathStart, pathEnd, pathLine;
			bool outlineStart, outlineEnd, surfaceStart, surfaceEnd;
			this.PathBuild(null,
						   out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine, true);

			return pathLine;
		}

		// Retourne le chemin géométrique de l'objet.
		public override Path GetPath(int rank)
		{
			if ( rank > 0 )  return null;
			Path pathStart, pathEnd, pathLine;
			bool outlineStart, outlineEnd, surfaceStart, surfaceEnd;
			this.PathBuild(null,
						   out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine, false);
			return pathLine;
		}

		// Crée une courbe de Bézier à partir d'un chemin quelconque.
		public bool CreateFromPath(Path path, int subPath)
		{
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
							this.HandleAdd(current, HandleType.Bezier);
							this.HandleAdd(current, HandleType.Starting);
							this.HandleAdd(current, HandleType.Bezier);
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
								this.PathAdjust(firstHandle);
								firstHandle = this.TotalMainHandle;
							}
							else
							{
								this.HandleAdd(p1, HandleType.Bezier);
								this.HandleAdd(p1, HandleType.Primary);
								this.HandleAdd(p1, HandleType.Bezier);
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
								this.Handle(this.TotalMainHandle-1).Position = p1;
								this.Handle(firstHandle).Position = p2;
								this.PathAdjust(firstHandle);
								firstHandle = this.TotalMainHandle;
							}
							else
							{
								this.Handle(this.TotalMainHandle-1).Position = p1;
								this.HandleAdd(p2, HandleType.Bezier);
								this.HandleAdd(p3, HandleType.Primary);
								this.HandleAdd(p3, HandleType.Bezier);
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
								this.Handle(this.TotalMainHandle-1).Position = p1;
								this.Handle(firstHandle).Position = p2;
								this.PathAdjust(firstHandle);
								firstHandle = this.TotalMainHandle;
							}
							else
							{
								this.Handle(this.TotalMainHandle-1).Position = p1;
								this.HandleAdd(p2, HandleType.Bezier);
								this.HandleAdd(p3, HandleType.Primary);
								this.HandleAdd(p3, HandleType.Bezier);
								bDo = true;
							}
						}
						current = p3;
						break;

					default:
						if ( (elements[i] & PathElement.FlagClose) != 0 )
						{
							close = true;
							this.PathAdjust(firstHandle);
						}
						i ++;
						break;
				}
			}
			this.PathAdjust(firstHandle);
			this.PropertyPolyClose.BoolValue = close;

			return bDo;
		}

		// Ajuste le chemin.
		protected void PathAdjust(int firstHandle)
		{
			// Transforme les courbes en droite si nécessaire.
			int total = this.TotalMainHandle;
			for ( int i=firstHandle ; i<total ; i+= 3 )
			{
				if ( this.Handle(i+0).Position == this.Handle(i+1).Position )
				{
					this.Handle(i+0).Type = HandleType.Hide;
				}

				if ( this.Handle(i+2).Position == this.Handle(i+1).Position )
				{
					this.Handle(i+2).Type = HandleType.Hide;
				}
			}

			// Mets les bonnes contraintes aux points principaux.
			for ( int i=firstHandle ; i<total ; i+= 3 )
			{
				this.Handle(i+1).ConstrainType = this.ComputeConstrain(firstHandle, i);
			}
		}

		// Devine le type de contrainte.
		protected HandleConstrainType ComputeConstrain(int firstHandle, int rank)
		{
			Point p = this.Handle(rank+1).Position;

			Point s1 = this.Handle(rank+0).Position;
			if ( this.Handle(rank+0).Type == HandleType.Hide )  // droite ?
			{
				int r = rank-2;
				if ( r < 0 )  r = this.TotalMainHandle-2;
				s1 = this.Handle(r).Position;
			}

			Point s2 = this.Handle(rank+2).Position;
			if ( this.Handle(rank+2).Type == HandleType.Hide )  // droite ?
			{
				int r = rank+4;
				if ( r > this.TotalMainHandle )  r = firstHandle+1;
				s2 = this.Handle(r).Position;
			}

			return Bezier.ComputeConstrain(s1, p, s2);
		}

		// Devine le type de contrainte en fonction d'un point principal et
		// des 2 points secondaires de part et d'autre.
		protected static HandleConstrainType ComputeConstrain(Point s1, Point p, Point s2)
		{
			double dx = System.Math.Abs((p.X-s1.X)-(s2.X-p.X));
			double dy = System.Math.Abs((p.Y-s1.Y)-(s2.Y-p.Y));
			if ( dx < 0.0001 && dy < 0.0001 )
			{
				return HandleConstrainType.Symmetric;
			}
			
			double a1 = Point.ComputeAngleDeg(p, s1);
			double a2 = Point.ComputeAngleDeg(p, s2);
			if ( System.Math.Abs(System.Math.Abs(a1-a2)      ) < 0.1 ||
				 System.Math.Abs(System.Math.Abs(a1-a2)-180.0) < 0.1 )
			{
				return HandleConstrainType.Smooth;
			}

			return HandleConstrainType.Corner;
		}

		// Crée une courbe de Bézier à partir de 4 points.
		public void CreateFromPoints(Point p1, Point s1, Point s2, Point p2)
		{
			this.HandleAdd(p1, HandleType.Hide);
			this.HandleAdd(p1, HandleType.Starting);
			this.HandleAdd(s1, HandleType.Bezier);

			this.HandleAdd(s2, HandleType.Bezier);
			this.HandleAdd(p2, HandleType.Primary);
			this.HandleAdd(p2, HandleType.Hide);

			this.SetDirtyBbox();
		}

		// Finalise la création d'une courbe de Bézier.
		public void CreateFinalise()
		{
			this.HandlePropertiesCreate();  // crée les poignées des propriétés
			this.Select(false);
			this.Select(true);  // pour sélectionner toutes les poignées
		}
		#endregion


		#region Serialization
		// Sérialise l'objet.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		// Constructeur qui désérialise l'objet.
		protected Bezier(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
		#endregion

		
		protected bool				mouseDown = false;
		protected bool				lastPoint;
		protected Point				initialPos;
	}
}

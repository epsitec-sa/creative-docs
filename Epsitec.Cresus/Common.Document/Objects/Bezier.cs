using System.Collections.Generic;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Objects
{
	/// <summary>
	/// La classe Bezier est la classe de l'objet graphique "courbes de B�zier".
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
			if ( type == Properties.Type.Frame )  return true;
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


		public override string IconUri
		{
			//	Nom de l'ic�ne.
			get { return Misc.Icon("ObjectBezier"); }
		}


		public override void MoveAllProcess(Point move)
		{
			//	D�place tout l'objet.
			this.document.Notifier.NotifyArea(this.BoundingBox);

			bool allHandle = !this.IsSelected;
			bool global = true;
			int total = this.TotalMainHandle;
			for ( int i=0 ; i<total ; i+=3 )
			{
				if ( allHandle || this.Handle(i+1).IsVisible )
				{
					this.Handle(i+0).Position += move;
					this.Handle(i+1).Position += move;
					this.Handle(i+2).Position += move;
				}
				else
				{
					global = false;
				}
			}

			if ( global )
			{
				this.MoveBbox(move);
			}
			else
			{
				this.SetDirtyBbox();
			}

			this.HandlePropertiesUpdate();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override void Select(Drawing.Rectangle rect)
		{
			//	S�lectionne toutes les poign�es de l'objet dans un rectangle.
			this.InsertOpletSelection();
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer, this.BoundingBox);

			bool shaper = this.document.Modifier.IsToolShaper;

			int sel = 0;
			int total = this.TotalMainHandle;
			for ( int i=0 ; i<total ; i+=3 )
			{
				if ( rect.Contains(this.Handle(i+1).Position) )
				{
					this.Handle(i+0).Modify(true, false, false, false);
					this.Handle(i+1).Modify(true, false, false, false);
					this.Handle(i+2).Modify(true, false, false, false);
					sel += 3;
				}
				else
				{
					if ( shaper )
					{
						this.Handle(i+0).Modify(true, false, false, true);
						this.Handle(i+1).Modify(true, false, false, true);
						this.Handle(i+2).Modify(true, false, false, true);
					}
					else
					{
						this.Handle(i+0).Modify(false, false, false, false);
						this.Handle(i+1).Modify(false, false, false, false);
						this.Handle(i+2).Modify(false, false, false, false);
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
				enable = (this.TotalMainHandle/3-this.TotalShaperHandleSelected() >= 2 && this.IsShaperHandleSelected());
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

		protected void ShaperHandleSym(int rank)
		{
			//	Passe le point en mode sym�trique.
			this.Handle(rank).ConstrainType = HandleConstrainType.Symmetric;
			this.MoveSecondary(rank, rank-1, rank+1, this.Handle(rank-1).Position);
			this.SetDirtyBbox();
		}

		protected void ShaperHandleSmooth(int rank)
		{
			//	Passe le point en mode lisse.
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

		protected void ShaperHandleCorner(int rank)
		{
			//	Passe le point en mode anguleux.
			this.Handle(rank).ConstrainType = HandleConstrainType.Corner;
		}

		protected void ShaperHandleContinue(int rank)
		{
			//	Prolonge la courbe.
			HandleType type = this.Handle(rank).Type;
			this.Handle(rank).Type = HandleType.Primary;

			int prev1, prev2, sec1a, sec1b, ins1, ins2, ins3;
			if ( type == HandleType.Starting )  // ins�re au d�but ?
			{
				sec1a = rank-1;
				prev1 = rank;
				sec1b = rank+1;
				prev2 = rank+3;

				ins1  = rank-1;
				ins2  = rank;
				ins3  = rank+1;
			}
			else	// ins�re � la fin ?
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

		protected void ShaperHandleAdd(Point pos, int rank)
		{
			//	Ajoute une poign�e sans changer l'aspect de la courbe.
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

		protected void ShaperHandleSub(int rank)
		{
			//	Supprime une poign�e sans trop changer l'aspect de la courbe.
			bool starting = (this.Handle(rank).Type == HandleType.Starting);

			this.HandleDelete(rank-1);
			this.HandleDelete(rank-1);
			this.HandleDelete(rank-1);

			//	Il doit toujours y avoir une poign�e de d�part !
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

		protected void ShaperHandleToLine(int rank)
		{
			//	Conversion d'un segement en ligne droite.
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

		protected void ShaperHandleToCurve(int rank)
		{
			//	Conversion d'un segement en courbe.
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


		public override bool IsShaperHandleSelected()
		{
			//	Indique si au moins une poign�e est s�lectionn�e par le modeleur.
			int total = this.TotalMainHandle;
			for ( int i=0 ; i<total ; i+=3 )
			{
				Handle handle = this.Handle(i+1);
				if ( !handle.IsVisible )  continue;

				if ( !handle.IsShaperDeselected )  return true;
			}
			return false;
		}

		public override int TotalShaperHandleSelected()
		{
			//	Donne le nombre de poign�es s�lectionn�es par le modeleur.
			int count = 0;
			int total = this.TotalMainHandle;
			for ( int i=0 ; i<total ; i+=3 )
			{
				Handle handle = this.Handle(i+1);
				if ( !handle.IsVisible )  continue;

				if ( !handle.IsShaperDeselected )  count ++;
			}
			return count;
		}

		protected void AdaptPrimaryLine(int rankPrimary, int rankSecondary, out int rankExtremity)
		{
			//	Adapte le point secondaire s'il est en mode "en ligne".
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

		protected int PrevRank(int rank)
		{
			//	Cherche le rang du groupe "sps" pr�c�dent, en tenant compte
			//	des ensembles Starting-Primary(s).
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

		protected int NextRank(int rank)
		{
			//	Cherche le rang du groupe "sps" suivant, en tenant compte
			//	des ensembles Starting-Primary(s).
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

		protected void MovePrimary(int rank, Point pos)
		{
			//	D�place une poign�e primaire selon les contraintes.
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

		protected void MoveSecondary(int rankPrimary, int rankSecondary, int rankOpposite, Point pos)
		{
			//	D�place une poign�e secondaire selon les contraintes.
			HandleConstrainType type = this.Handle(rankPrimary).ConstrainType;

			this.Handle(rankSecondary).Position = pos;

			if ( this.Handle(rankOpposite).Type == HandleType.Hide )  // droite?
			{
				if ( type == HandleConstrainType.Smooth )
				{
					if ( rankOpposite > rankSecondary )
					{
						rankOpposite = this.NextRank(rankPrimary-1);
					}
					else
					{
						rankOpposite = this.PrevRank(rankPrimary-1);
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

		public override void MoveHandleStarting(int rank, Point pos, DrawingContext drawingContext)
		{
			//	D�but du d�placement une poign�e.
			base.MoveHandleStarting(rank, pos, drawingContext);

			if ( rank < this.handles.Count )  // poign�e de l'objet ?
			{
				drawingContext.ConstrainClear();

				Handle handle = this.Handle(rank);
				if ( handle.PropertyType == Properties.Type.None )
				{
					int prev = this.PrevRank(rank/3*3);
					int next = this.NextRank(rank/3*3);

					if ( rank%3 == 1 )  // poign� principale ?
					{
						if ( this.Handle(rank-1).Type == HandleType.Hide )
						{
							drawingContext.ConstrainAddLine(this.Handle(rank).Position, this.Handle(prev+1).Position, false, -1);
							drawingContext.ConstrainAddHV(this.Handle(prev+1).Position, false, prev+1);
							drawingContext.ConstrainAddCircle(this.Handle(prev+1).Position, this.Handle(rank).Position, false, prev+1);
						}

						if ( this.Handle(rank+1).Type == HandleType.Hide )
						{
							drawingContext.ConstrainAddLine(this.Handle(rank).Position, this.Handle(next+1).Position, false, -1);
							drawingContext.ConstrainAddHV(this.Handle(next+1).Position, false, next+1);
							drawingContext.ConstrainAddCircle(this.Handle(next+1).Position, this.Handle(rank).Position, false, next+1);
						}

						drawingContext.ConstrainAddHV(this.Handle(rank).Position, false, rank);
					}
					else	// poign�e secondaire ?
					{
						pos = this.Handle((rank/3)*3+1).Position;
						drawingContext.ConstrainAddLine(this.Handle(rank).Position, pos, false, rank);
						drawingContext.ConstrainAddHV(pos, false, rank);

						if ( rank%3 == 0 )
						{
							if ( this.Handle(rank+2).Type == HandleType.Hide )
							{
								drawingContext.ConstrainAddLine(this.Handle(rank+1).Position, this.Handle(next+1).Position, false, -1);
							}
							drawingContext.ConstrainAddCircle(this.Handle(rank+1).Position, this.Handle(rank).Position, false, rank+1);
						}

						if ( rank%3 == 2 )
						{
							if ( this.Handle(rank-2).Type == HandleType.Hide )
							{
								drawingContext.ConstrainAddLine(this.Handle(rank-1).Position, this.Handle(prev+1).Position, false, -1);
							}
							drawingContext.ConstrainAddCircle(this.Handle(rank-1).Position, this.Handle(rank).Position, false, rank-1);
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

			if ( this.Handle(rank).Type == HandleType.Primary  ||  // principale ?
				 this.Handle(rank).Type == HandleType.Starting )
			{
				this.MovePrimary(rank, pos);
			}
			else if ( this.Handle(rank).Type == HandleType.Bezier )  // secondaire ?
			{
				if ( rank%3 == 0 )  // poign�e secondaire ?
				{
					this.MoveSecondary(rank+1, rank, rank+2, pos);
				}
				if ( rank%3 == 2 )  // poign�e secondaire ?
				{
					this.MoveSecondary(rank-1, rank, rank-2, pos);
				}
			}

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
			int rp1 = ss.Rank*3+1;
			int rs1 = ss.Rank*3+2;
			int rs2 = this.NextRank(ss.Rank*3)+0;
			int rp2 = this.NextRank(ss.Rank*3)+1;

			if ( (this.Handle(rs1).Type == HandleType.Hide) )  // droite ?
			{
				this.Handle(rp1).InitialPosition = this.Handle(rp1).Position;
				this.Handle(rp2).InitialPosition = this.Handle(rp2).Position;

				drawingContext.ConstrainClear();
				drawingContext.ConstrainAddHV(ss.Position, false, -1);
				Point p1 = this.Handle(rp1).Position;
				Point p2 = this.Handle(rp2).Position;
				Size d = new Size(p2.X-p1.X, p2.Y-p1.Y);
				drawingContext.ConstrainAddLine(p1, p2, false, -1);
				drawingContext.ConstrainAddLine(pos, new Point(pos.X-d.Height, pos.Y+d.Width), false, -1);
			}
			else	// courbe ?
			{
				this.Handle(rs1).InitialPosition = this.Handle(rs1).Position;
				this.Handle(rs2).InitialPosition = this.Handle(rs2).Position;

				drawingContext.ConstrainClear();
				drawingContext.ConstrainAddHV(ss.Position, false, -1);
				double a1 = Point.ComputeAngleDeg(this.Handle(rp1).Position, this.Handle(rs1).Position);
				double a2 = Point.ComputeAngleDeg(this.Handle(rp2).Position, this.Handle(rs2).Position);
				Point c = ss.Position;
				Point p = Transform.RotatePointDeg(c, (a1+a2)/2, new Point(c.X+100, c.Y));
				drawingContext.ConstrainAddLine(c, p, false, -1);
			}
		}

		public override void MoveSelectedSegmentProcess(int rank, Point pos, DrawingContext drawingContext)
		{
			//	D�place une poign�e d'un segment s�lectionn�.
			if (rank >= this.selectedSegments.Count)
			{
				return;
			}

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
				if ( drawingContext.IsCtrl )  // contraintes ?
				{
					double d1 = Point.Distance(this.Handle(rp1).Position, this.Handle(rs1).InitialPosition+move);
					Point s1 = Point.Move(this.Handle(rp1).Position, this.Handle(rs1).InitialPosition, d1);
					this.MoveSecondary(rs1-1, rs1, rs1-2, s1);

					double d2 = Point.Distance(this.Handle(rp2).Position, this.Handle(rs2).InitialPosition+move);
					Point s2 = Point.Move(this.Handle(rp2).Position, this.Handle(rs2).InitialPosition, d2);
					this.MoveSecondary(rs2+1, rs2, rs2+2, s2);
				}
				else
				{
					this.MoveSecondary(rs1-1, rs1, rs1-2, this.Handle(rs1).InitialPosition+move);
					this.MoveSecondary(rs2+1, rs2, rs2+2, this.Handle(rs2).InitialPosition+move);
				}
			}

			SelectedSegment.Update(this.selectedSegments, this);

			this.SetDirtyBbox();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override void MoveSelectedSegmentEnding(int rank, Point pos, DrawingContext drawingContext)
		{
			//	Fin du d�placement d'une poign�e d'un segment s�lectionn�.
			base.MoveSelectedSegmentEnding(rank, pos, drawingContext);
		}

		
		public override void MoveSelectedHandlesStarting(Point mouse, DrawingContext drawingContext)
		{
			//	Retourne la liste des positions des poign�es s�lectionn�es par le modeleur.
			drawingContext.SnapPos(ref mouse);
			this.moveSelectedHandleStart = mouse;

			this.moveSelectedHandleList = new System.Collections.ArrayList();
			int total = this.TotalMainHandle;
			for ( int i=0 ; i<total ; i+=3 )
			{
				if ( !this.Handle(i+1).IsVisible )  continue;

				if ( !this.Handle(i+1).IsShaperDeselected )
				{
					this.MoveSelectedHandlesAdd(i+0);
					this.MoveSelectedHandlesAdd(i+1);
					this.MoveSelectedHandlesAdd(i+2);
				}
			}

			if ( this.selectedSegments != null && this.selectedSegments.Count != 0 )
			{
				foreach ( SelectedSegment ss in this.selectedSegments )
				{
					this.MoveSelectedHandlesAdd((ss.Rank+0)*3+0);
					this.MoveSelectedHandlesAdd((ss.Rank+0)*3+1);
					this.MoveSelectedHandlesAdd((ss.Rank+0)*3+2);
					this.MoveSelectedHandlesAdd((ss.Rank+1)*3+0);
					this.MoveSelectedHandlesAdd((ss.Rank+1)*3+1);
					this.MoveSelectedHandlesAdd((ss.Rank+1)*3+2);
				}
			}

			if ( this.moveSelectedHandleList.Count == 0 )
			{
				this.moveSelectedHandleList = null;
				return;
			}

			this.InsertOpletGeometry();

			if ( this.selectedSegments != null )
			{
				SelectedSegment.InsertOpletGeometry(this.selectedSegments, this);
			}
		}


		public override void MoveGlobalProcess(Selector selector)
		{
			//	D�place globalement l'objet.
			base.MoveGlobalProcess(selector);
			this.HandlePropertiesUpdate();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override void AlignGrid(DrawingContext drawingContext)
		{
			//	Aligne l'objet sur la grille.
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


		public override void CreateMouseDown(Point pos, DrawingContext drawingContext)
		{
			//	D�but de la cr�ation d'un objet.
			this.document.Notifier.NotifyArea(this.BoundingBox);  // pour effacer les r�sidus de l'ancienne fl�che

			drawingContext.ConstrainClear();
			drawingContext.ConstrainAddHV(pos, false, 0);

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
					if ( this.TotalMainHandle == 3 )
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
					len = Point.Distance(pos, this.Handle(this.TotalMainHandle-2).Position);
					if ( len <= drawingContext.CloseMargin )
					{
						ignore = true;
					}
				}
			}

			if ( !this.lastPoint && !ignore )
			{
				if ( this.TotalMainHandle == 0 )
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

					int rank = this.TotalMainHandle-6;
					if ( this.Handle(rank).Type == HandleType.Hide )
					{
						this.Handle(rank+2).Position = Point.Scale(this.Handle(rank+1).Position, pos, 0.5);
					}
				}
				this.Handle(this.TotalMainHandle-2).IsVisible = true;
			}

			this.mouseDown = true;
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override void CreateMouseMove(Point pos, DrawingContext drawingContext)
		{
			//	D�placement pendant la cr�ation d'un objet.
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
					rank = this.TotalMainHandle-2;
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

		public override void CreateMouseUp(Point pos, DrawingContext drawingContext)
		{
			//	Fin de la cr�ation d'un objet.
			this.document.Notifier.NotifyArea(this.BoundingBox);

			int rank = this.TotalMainHandle;
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

		public override bool CreateIsEnding(DrawingContext drawingContext)
		{
			//	Indique si la cr�ation de l'objet est termin�e.
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

		public override bool CreateIsExist(DrawingContext drawingContext)
		{
			//	Indique si l'objet doit exister. Retourne false si l'objet ne peut
			//	pas exister et doit �tre d�truit.
			return true;
		}

		public override bool CreateEnding(DrawingContext drawingContext)
		{
			//	Termine la cr�ation de l'objet. Retourne false si l'objet ne peut
			//	pas exister et doit �tre d�truit.
			this.isCreating = false;
			this.document.Modifier.TextInfoModif = "";

			int total = this.TotalMainHandle;
			if ( total <= 3 )  return false;

			this.Handle(1).Type = HandleType.Starting;
			this.Deselect();
			this.HandlePropertiesCreate();
			this.HandlePropertiesUpdate();
			return true;
		}

		protected void TextInfoModif(Point mouse, int rank)
		{
			//	Texte des informations de modification.
			int r1, r2;
			if ( this.isCreating )
			{
				r1 = this.TotalMainHandle-2;
				r2 = this.TotalMainHandle-1;
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

		public override bool CreateAction(int rank, out string cmd, out string name, out string text)
		{
			//	Retourne un bouton d'action pendant la cr�ation.
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

			//	Forme de la surface de d�part.
			if (surfaceStart)
			{
				var shape = new Shape ();
				shape.Path = pathStart;
				shape.SetPropertySurface (port, this.PropertyLineColor);
				shape.IsMisc = true;
				objectShapes.Add (shape);
			}

			//	Forme de la surface d'arriv�e.
			if (surfaceEnd)
			{
				var shape = new Shape ();
				shape.Path = pathEnd;
				shape.SetPropertySurface (port, this.PropertyLineColor);
				shape.IsMisc = true;
				objectShapes.Add (shape);
			}

			//	Forme du chemin de d�part.
			if (outlineStart)
			{
				var shape = new Shape ();
				shape.Path = pathStart;
				shape.SetPropertyStroke (port, this.PropertyLineMode, this.PropertyLineColor);
				shape.IsMisc = true;
				objectShapes.Add (shape);
			}

			//	Forme du chemin d'arriv�e.
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
				var polygons = Geometry.PathToPolygons (pathLine);
				frame.AddShapes (this, shapes, objectShapes, port, drawingContext, polygons, null);
			}

			//	Forme des traits de support pour les poign�es secondaires.
			if ((this.IsSelected || this.isCreating) &&
				 drawingContext != null && drawingContext.IsActive &&
				 !this.IsGlobalSelected)
			{
				Path pathSupport = new Path ();
				int total = this.TotalMainHandle;
				for (int j=0; j<total; j+=3)
				{
					if (!this.Handle (j+1).IsVisible)
					{
						continue;
					}

					pathSupport.MoveTo (this.Handle (j+1).Position);
					pathSupport.LineTo (this.Handle (j+0).Position);
					pathSupport.MoveTo (this.Handle (j+1).Position);
					pathSupport.LineTo (this.Handle (j+2).Position);
				}

				var shape = new Shape ();
				shape.Path = pathSupport;
				shape.SetPropertyStroke (port, this.PropertyLineMode, this.PropertyLineColor);
				shape.Aspect = Aspect.Support;
				shape.IsVisible = true;
				shapes.Add (shape);
			}

			return shapes.ToArray ();
		}

		public void ComputeExtremity(bool start, out Point a, out Point b)
		{
			//	Calcule les points de la droite � une extr�mit� de la courbe.
			int r1 = start ? 1 : this.TotalMainHandle-2;
			int r2 = start ? 2 : this.TotalMainHandle-3;

			if ( this.Handle(r2).Type == HandleType.Hide )
			{
				r2 = start ? 4 : this.TotalMainHandle-5;
			}

			a = this.Handle(r1).Position;
			b = this.Handle(r2).Position;
		}

		protected void PathBuild(DrawingContext drawingContext,
								 out Path pathStart, out bool outlineStart, out bool surfaceStart,
								 out Path pathEnd,   out bool outlineEnd,   out bool surfaceEnd,
								 out Path pathLine, bool simplify, bool forShaper)
		{
			//	Cr�e les chemins de l'objet.
			//	Le mode forShaper g�n�re des segments nuls lorsque la ligne est ouverte, pour ne pas
			//	perturber le compte des segments avec Geometry.PathExtract et Geometry.DetectOutlineRank.
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
					if (this.PropertyPolyClose.BoolValue)  // ferm� ?
					{
						this.PathPutSegment(pathLine, i-1, first, pp1);
						pathLine.Close();
					}
					else if (forShaper)  // ouvert et modeleur ?
					{
						pathLine.MoveTo(pp1);
						pathLine.LineTo(pp1);  // met un segment nul
					}
					first = i;
					pp1 = this.Handle(i+1).Position;
					pathLine.MoveTo(pp1);
				}
				else if ( i < total-3 )  // point interm�diaire ?
				{
					this.PathPutSegment(pathLine, i-1, i, this.Handle(i+1).Position);
				}
				else	// dernier point ?
				{
					this.PathPutSegment(pathLine, i-1, i, pp2);
				}
			}
			if (this.PropertyPolyClose.BoolValue)  // ferm� ?
			{
				this.PathPutSegment(pathLine, total-1, first, pp1);
				pathLine.Close();
			}
			else if (forShaper)  // ouvert et modeleur ?
			{
				pathLine.MoveTo(pp1);
				pathLine.LineTo(pp1);  // met un segment nul
			}
		}

		protected void PathPutSegment(Path path, int rankS1, int rankS2, Point p2)
		{
			//	Ajoute un segment de courbe ou de droite dans le chemin.
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


		public override Path GetMagnetPath()
		{
			//	Retourne le chemin g�om�trique de l'objet pour les constructions
			//	magn�tiques.
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
			//	Retourne le chemin g�om�trique de l'objet pour le modeleur.
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
			//	Retourne le chemin g�om�trique de l'objet.
			Path pathStart, pathEnd, pathLine;
			bool outlineStart, outlineEnd, surfaceStart, surfaceEnd;
			this.PathBuild(null,
						   out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine, false, false);
			return pathLine;
		}

		public void CreateFromPoints(Point p1, Point s1, Point s2, Point p2)
		{
			//	Cr�e une courbe de B�zier � partir de 4 points.
			this.HandleAdd(p1, HandleType.Hide);
			this.HandleAdd(p1, HandleType.Starting);
			this.HandleAdd(s1, HandleType.Bezier);

			this.HandleAdd(s2, HandleType.Bezier);
			this.HandleAdd(p2, HandleType.Primary);
			this.HandleAdd(p2, HandleType.Hide);

			this.SetDirtyBbox();
		}

		public void CreateFinalise()
		{
			//	Finalise la cr�ation d'une courbe de B�zier.
			this.HandlePropertiesCreate();  // cr�e les poign�es des propri�t�s
			this.Select(false);
			this.Select(true);  // pour s�lectionner toutes les poign�es
		}


		#region Serialization
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	S�rialise l'objet.
			base.GetObjectData(info, context);
		}

		protected Bezier(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui d�s�rialise l'objet.
		}
		#endregion

		
		protected bool				mouseDown = false;
		protected bool				lastPoint;
		protected Point				initialPos;
	}
}

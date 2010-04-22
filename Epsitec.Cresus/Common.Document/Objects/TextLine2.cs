using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Text;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Objects
{
	/// <summary>
	/// La classe TextLine2 est la classe de l'objet graphique "texte simple".
	/// </summary>
	[System.Serializable()]
	public class TextLine2 : Objects.AbstractText, Text.ITextRenderer
	{
		public TextLine2(Document document, Objects.Abstract model) : base(document, model)
		{
		}

		protected override bool ExistingProperty(Properties.Type type)
		{
			if ( type == Properties.Type.Name )  return true;
			if ( type == Properties.Type.LineMode )  return true;
			if ( type == Properties.Type.LineColor )  return true;
			return false;
		}

		protected override Objects.Abstract CreateNewObject(Document document, Objects.Abstract model)
		{
			//	Crée une instance de l'objet.
			return new TextLine2(document, model);
		}

		protected override void Initialize()
		{
			this.textFrame = new Text.SingleLineTextFrame();
			base.Initialize();
		}
		
		protected override void InitializeInternals()
		{
			if ( this.textFrame == null )
			{
				this.textFrame = new Text.SingleLineTextFrame();
			}

			base.InitializeInternals();
		}


		public override string IconUri
		{
			//	Nom de l'icône.
			get { return Misc.Icon("ObjectTextLine"); }
		}

		
		public override int CreationLastHandle
		{
			//	Retourne le rang de la dernière poignée créée.
			//	A la création, une ligne de texte est toujours une droite, mais chacune des deux poignées
			//	créée est en fait constituée de 3 poignées (secondaire-primaire-secondaire). Il faut donc
			//	retourner le rang de la dernière poignée principale.
			get
			{
				return this.TotalMainHandle-2;
			}
		}


		public override bool Detect(Drawing.Rectangle rect, bool all)
		{
			//	Détecte si l'objet est dans un rectangle.
			//	all = true  -> toutes les poignées doivent être dans le rectangle
			//	all = false -> une seule poignée doit être dans le rectangle
			if ( this.isHide )  return false;

			if ( all )
			{
				return rect.Contains(this.BoundingBoxThin);
			}
			else
			{
				return base.Detect(rect, all);
			}
		}

		protected int DetectOutline(Point pos)
		{
			//	Détecte si la souris est sur le pourtour de l'objet.
			//	Retourne le rank de la poignée de départ, ou -1
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;

			Shape[] shapes = this.ShapesBuild(null, context, false);
			int rank = context.Drawer.DetectOutline(pos, context, shapes);
			if ( rank != -1 )  rank *= 3;
			return rank;
		}


		public override void MoveAllProcess(Point move)
		{
			//	Déplace tout l'objet.
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
			this.UpdateGeometry();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override void Select(Drawing.Rectangle rect)
		{
			//	Sélectionne toutes les poignées de l'objet dans un rectangle.
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
				if ( this.IsShaperHandleSelected() )
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
			//	Passe le point en mode symétrique.
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
			//	Ajoute une poignée sans changer l'aspect de la courbe.
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
		}

		protected void ShaperHandleSub(int rank)
		{
			//	Supprime une poignée sans changer l'aspect de la courbe.
			bool starting = (this.Handle(rank).Type == HandleType.Starting);

			this.HandleDelete(rank-1);
			this.HandleDelete(rank-1);
			this.HandleDelete(rank-1);

			//	Il doit toujours y avoir une poignée de départ !
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
			//	Indique si au moins une poignée est sélectionnée par le modeleur.
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
			//	Donne le nombre de poignées sélectionnées par le modeleur.
			int count = 0;
			int total = this.TotalHandle;
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
			//	Cherche le rang du groupe "sps" précédent, en tenant compte
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
			//	Déplace une poignée primaire selon les contraintes.
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
			//	Déplace une poignée secondaire selon les contraintes.
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
			//	Début du déplacement une poignée.
			base.MoveHandleStarting(rank, pos, drawingContext);

			if ( rank < this.handles.Count )  // poignée de l'objet ?
			{
				drawingContext.ConstrainClear();

				Handle handle = this.Handle(rank);
				if ( handle.PropertyType == Properties.Type.None )
				{
					int prev = this.PrevRank(rank/3*3);
					int next = this.NextRank(rank/3*3);

					if ( rank%3 == 1 )  // poigné principale ?
					{
						if ( this.Handle(rank-1).Type == HandleType.Hide )
						{
							drawingContext.ConstrainAddLine(this.Handle(rank).Position, this.Handle(prev+1).Position, false, -1);
							drawingContext.ConstrainAddHV(this.Handle(prev+1).Position, false, prev+1);
						}

						if ( this.Handle(rank+1).Type == HandleType.Hide )
						{
							drawingContext.ConstrainAddLine(this.Handle(rank).Position, this.Handle(next+1).Position, false, -1);
							drawingContext.ConstrainAddHV(this.Handle(next+1).Position, false, next+1);
						}

						drawingContext.ConstrainAddHV(this.Handle(rank).Position, false, rank);
					}
					else	// poignée secondaire ?
					{
						pos = this.Handle((rank/3)*3+1).Position;
						drawingContext.ConstrainAddLine(this.Handle(rank).Position, pos, false, rank);
						drawingContext.ConstrainAddHV(pos, false, rank);

						if ( rank%3 == 0 && this.Handle(rank+2).Type == HandleType.Hide )
						{
							drawingContext.ConstrainAddLine(this.Handle(rank+1).Position, this.Handle(next+1).Position, false, -1);
						}

						if ( rank%3 == 2 && this.Handle(rank-2).Type == HandleType.Hide )
						{
							drawingContext.ConstrainAddLine(this.Handle(rank-1).Position, this.Handle(prev+1).Position, false, -1);
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
			//	Déplace une poignée.
			if ( rank >= this.TotalMainHandle )  // poignée d'une propriété ?
			{
				base.MoveHandleProcess(rank, pos, drawingContext);
				return;
			}

			this.document.Notifier.NotifyArea(this.BoundingBox);
			this.textFlow.NotifyAreaFlow();
			drawingContext.SnapPos(ref pos);

			if ( this.Handle(rank).Type == HandleType.Starting ||
				 this.Handle(rank).Type == HandleType.Primary  )  // principale ?
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

			this.UpdateGeometry();
			this.SetDirtyBbox();
			this.TextInfoModifLine();
			this.document.Notifier.NotifyArea(this.BoundingBox);
			this.textFlow.NotifyAreaFlow();
		}


		public override void MoveSelectedSegmentStarting(int rank, Point pos, DrawingContext drawingContext)
		{
			//	Début du déplacement d'une poignée d'un segment sélectionné.
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
			}
			else	// courbe ?
			{
				this.Handle(rs1).InitialPosition = this.Handle(rs1).Position;
				this.Handle(rs2).InitialPosition = this.Handle(rs2).Position;
			}
		}

		public override void MoveSelectedSegmentProcess(int rank, Point pos, DrawingContext drawingContext)
		{
			//	Déplace une poignée d'un segment sélectionné.
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

			this.UpdateGeometry();
			this.SetDirtyBbox();
			this.document.Notifier.NotifyArea(this.BoundingBox);
			this.textFlow.NotifyAreaFlow();
		}

		public override void MoveSelectedSegmentEnding(int rank, Point pos, DrawingContext drawingContext)
		{
			//	Fin du déplacement d'une poignée d'un segment sélectionné.
			base.MoveSelectedSegmentEnding(rank, pos, drawingContext);
		}

		
		public override void MoveSelectedHandlesStarting(Point mouse, DrawingContext drawingContext)
		{
			//	Retourne la liste des positions des poignées sélectionnées par le modeleur.
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
			//	Déplace globalement l'objet.
			base.MoveGlobalProcess(selector);
			this.HandlePropertiesUpdate();
			this.UpdateGeometry();
			this.textFlow.NotifyAreaFlow();
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

		
		#region ForSamples
		public void CreateForSample()
		{
			//	Crée un objet pour un échantillon.
			Point pos = new Point(0, 0);

			this.HandleAdd(pos, HandleType.Primary);  // rang = 0
			this.HandleAdd(pos, HandleType.Primary);  // rang = 1
			this.HandleAdd(pos, HandleType.Primary);  // rang = 2

			this.HandleAdd(pos, HandleType.Primary);  // rang = 3
			this.HandleAdd(pos, HandleType.Primary);  // rang = 4
			this.HandleAdd(pos, HandleType.Primary);  // rang = 5
		}

		public void RectangleToSample(Point p1, Point p2)
		{
			//	Spécifie les dimensions pour un échantillon.
			this.Handle(0).Position = p1;
			this.Handle(1).Position = p1;
			this.Handle(2).Position = p1;

			this.Handle(3).Position = p2;
			this.Handle(4).Position = p2;
			this.Handle(5).Position = p2;

			this.UpdateGeometry();
		}
		#endregion

		public override void CreateMouseDown(Point pos, DrawingContext drawingContext)
		{
			//	Début de la création d'un objet.
			drawingContext.ConstrainClear();
			drawingContext.ConstrainAddHV(pos, false, 0);

			this.HandleAdd(pos, HandleType.Hide);
			this.HandleAdd(pos, HandleType.Starting);
			this.HandleAdd(pos, HandleType.Hide);

			this.HandleAdd(pos, HandleType.Hide);
			this.HandleAdd(pos, HandleType.Primary);
			this.HandleAdd(pos, HandleType.Hide);

			this.Handle(1).IsVisible = true;
			this.Handle(4).IsVisible = true;

			this.isCreating = true;
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override void CreateMouseMove(Point pos, DrawingContext drawingContext)
		{
			//	Déplacement pendant la création d'un objet.
			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapPos(ref pos);
			this.Handle(3).Position = pos;
			this.Handle(4).Position = pos;
			this.Handle(5).Position = pos;
			this.UpdateGeometry();
			this.SetDirtyBbox();
			this.TextInfoModifLine();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override void CreateMouseUp(Point pos, DrawingContext drawingContext)
		{
			//	Fin de la création d'un objet.
			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapPos(ref pos);
			this.Handle(3).Position = pos;
			this.Handle(4).Position = pos;
			this.Handle(5).Position = pos;
			drawingContext.ConstrainDelStarting();
			drawingContext.MagnetClearStarting();

			if ( !this.CreateIsExist(drawingContext) )  // clic (presque) sans bouger ?
			{
				Drawing.Rectangle box = this.document.Modifier.ActiveViewer.GuidesSearchBox(pos);

				if ( !box.IsEmpty )
				{
					pos = this.Handle(0).Position;
					pos.X = box.Left;
					this.Handle(0).Position = pos;
					this.Handle(1).Position = pos;
					this.Handle(2).Position = pos;

					pos = this.Handle(3).Position;
					pos.X = box.Right;
					this.Handle(3).Position = pos;
					this.Handle(4).Position = pos;
					this.Handle(5).Position = pos;
				}
			}
			this.UpdateGeometry();
			this.SetDirtyBbox();

			this.isCreating = false;
			this.document.Modifier.TextInfoModif = "";
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override bool CreateIsExist(DrawingContext drawingContext)
		{
			//	Indique si l'objet doit exister. Retourne false si l'objet ne peut
			//	pas exister et doit être détruit.
			//?this.Deselect();
			double len = Point.Distance(this.Handle(1).Position, this.Handle(4).Position);
			return ( len > drawingContext.MinimalSize );
		}

		public override bool EditAfterCreation()
		{
			//	Indique s'il faut sélectionner l'objet après sa création.
			return true;
		}


		public override Drawing.Rectangle RealBoundingBox()
		{
			//	Retourne la bounding réelle, en fonction des caractères contenus.
			this.mergingBoundingBox = Drawing.Rectangle.Empty;
			this.DrawText(null, null, InternalOperation.RealBoundingBox);
			return this.mergingBoundingBox;
		}
		
		protected Path RealSelectPath()
		{
			//	Retourne le chemin de tous les caractères sélectionnés.
			this.realSelectPath = new Path();
			this.DrawText(null, null, InternalOperation.RealSelectPath);
			return this.realSelectPath;
		}
		

		public override Shape[] ShapesBuild(IPaintPort port, DrawingContext drawingContext, bool simplify)
		{
			//	Constuit les formes de l'objet.
			Path path = this.PathBuild();

			bool flowHandles = this.edited && drawingContext != null && drawingContext.VisibleHandles && !drawingContext.IsBitmap;

			int totalShapes = 3;
			if ( flowHandles )  totalShapes += 2;

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
			
			//	Traits de la courbe.
			shapes[i] = new Shape();
			shapes[i].Path = path;
			shapes[i].SetPropertyStroke(port, this.PropertyLineMode, this.PropertyLineColor);
			i ++;

			//	Caractères du texte.
			shapes[i] = new Shape();
			shapes[i].SetTextObject(this);
			i ++;

			if ( flowHandles )
			{
				shapes[i] = new Shape();
				shapes[i].Path = this.PathFlowHandlesStroke(port, drawingContext);
				shapes[i].SetPropertyStroke(port, this.PropertyLineMode, this.PropertyLineColor);
				shapes[i].Aspect = Aspect.Support;
				shapes[i].IsVisible = true;
				i ++;

				shapes[i] = new Shape();
				shapes[i].Path = this.PathFlowHandlesSurface(port, drawingContext);
				shapes[i].SetPropertySurface(port, this.PropertyLineColor);
				shapes[i].Aspect = Aspect.Support;
				shapes[i].IsVisible = true;
				i ++;
			}

			//	Rectangles des caractères seulement pour la détection.
			shapes[i] = new Shape();
			shapes[i].Path = this.RealSelectPath();
			shapes[i].Type = Type.Surface;
			shapes[i].Aspect = Aspect.OnlyDetect;
			i ++;

			//	Forme des traits de support pour les poignées secondaires.
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

#if false
			bool flowHandles = this.edited && drawingContext != null;

			Path pathLine = this.PathBuild();
			Path pathHilite = null;
			Path pathSupport = null;
			Path pathBbox = this.RealSelectPath();

			if ( this.IsHilite &&
				 drawingContext != null &&
				 drawingContext.IsActive &&
				 !this.edited )
			{
				pathHilite = pathBbox;
			}

			if ( this.IsSelected &&
				 drawingContext != null &&
				 drawingContext.IsActive &&
				 !this.IsGlobalSelected &&
				 !this.edited )
			{
				pathSupport = new Path();
				int total = this.TotalMainHandle;
				for ( int j=0 ; j<total ; j+=3 )
				{
					if ( !this.Handle(j+1).IsVisible )  continue;
					pathSupport.MoveTo(this.Handle(j+1).Position);
					pathSupport.LineTo(this.Handle(j+0).Position);
					pathSupport.MoveTo(this.Handle(j+1).Position);
					pathSupport.LineTo(this.Handle(j+2).Position);
				}
			}

			int totalShapes = 3;
			if ( pathHilite  != null )  totalShapes ++;
			if ( pathSupport != null )  totalShapes ++;
			if ( flowHandles         )  totalShapes += 2;

			Shape[] shapes = new Shape[totalShapes];
			int i = 0;
			
			//	Rectangles des caractères survolés.
			if ( pathHilite != null )
			{
				shapes[i] = new Shape();
				shapes[i].Path = pathHilite;
				shapes[i].Type = Type.Surface;
				shapes[i].Aspect = Aspect.Hilited;
				i ++;
			}
			
			//	Chemin pointillé.
			shapes[i] = new Shape();
			shapes[i].Path = pathLine;
			shapes[i].Type = Type.Stroke;
			i ++;

			//	Caractères du texte.
			shapes[i] = new Shape();
			shapes[i].SetTextObject(this);
			i ++;

			//	Traits de support si chemin courbe.
			if ( pathSupport != null )
			{
				shapes[i] = new Shape();
				shapes[i].Path = pathSupport;
				shapes[i].Type = Type.Stroke;
				shapes[i].Aspect = Aspect.Support;
				i ++;
			}

			if ( flowHandles )
			{
				shapes[i] = new Shape();
				shapes[i].Path = this.PathFlowHandlesStroke(port, drawingContext);
				shapes[i].SetPropertyStroke(port, this.PropertyLineMode, this.PropertyLineColor);
				shapes[i].Aspect = Aspect.Support;
				shapes[i].IsVisible = true;
				i ++;

				shapes[i] = new Shape();
				shapes[i].Path = this.PathFlowHandlesSurface(port, drawingContext);
				shapes[i].SetPropertySurface(port, this.PropertyLineColor);
				shapes[i].Aspect = Aspect.Support;
				shapes[i].IsVisible = true;
				i ++;
			}

			//	Rectangles des caractères pour bbox et détection.
			shapes[i] = new Shape();
			shapes[i].Path = pathBbox;
			shapes[i].Type = Type.Surface;
			shapes[i].Aspect = Aspect.InvisibleBox;
			i ++;

			return shapes;
#endif
		}

		protected Path PathBuild()
		{
			//	Crée le chemin de l'objet.
			Path path = new Path();

			int total = this.TotalMainHandle;
			if ( total < 6 )  return path;

			for ( int i=0 ; i<total ; i+=3 )
			{
				if ( i == 0 )  // premier point ?
				{
					path.MoveTo(this.Handle(i+1).Position);
				}
				else
				{
					path.CurveTo(this.Handle(i-1).Position, this.Handle(i).Position, this.Handle(i+1).Position);
				}
			}
			return path;
		}

		protected double Transform(Point position)
		{
			//	Transforme une position x;y en position le long de la courbe (2D -> 1D).
			double length = 0.0;
			double min = 1000000;
			double best = double.NaN;
			Point lastProjection = Point.Zero;
			int i = 0;
			do
			{
				Point p1 = this.Handle(i+1).Position;
				Point s1 = this.Handle(i+2).Position;
				Point s2 = this.Handle(i+3).Position;
				Point p2 = this.Handle(i+4).Position;

				if ( this.Handle(i+2).Type == HandleType.Hide )  // droite ?
				{
					Point p = Point.Projection(p1, p2, position);
					if ( TextLine2.Contains(p, p1, p2) )
					{
						double d = Point.Distance(p, position);
						if ( d < min )
						{
							min = d;
							best = length + Point.Distance(p1, p);
						}
					}
					else if ( !lastProjection.IsZero )
					{
						Point pp = Point.Projection(p1, p2, lastProjection);
						if ( TextLine2.Contains(pp, p1, p2) )
						{
							double d = Point.Distance(pp, lastProjection);
							if ( d < min )
							{
								min = d;
								best = length + Point.Distance(p1, pp);
							}
						}
					}
					lastProjection = p;

					length += Point.Distance(p1,p2);
				}
				else	// courbe ?
				{
					Point pos = p1;
					int total = (int)(1.0/TextLine2.step);
					for ( int rank=1 ; rank<=total ; rank ++ )
					{
						double t = TextLine2.step*rank;
						Point next = Point.FromBezier(p1,s1,s2,p2, t);

						Point p = Point.Projection(pos, next, position);
						if ( TextLine2.Contains(p, pos, next) )
						{
							double d = Point.Distance(p, position);
							if ( d < min )
							{
								min = d;
								best = length + Point.Distance(pos, p);
							}
						}
						else if ( !lastProjection.IsZero )
						{
							Point pp = Point.Projection(pos, next, lastProjection);
							if ( TextLine2.Contains(pp, pos, next) )
							{
								double d = Point.Distance(pp, lastProjection);
								if ( d < min )
								{
									min = d;
									best = length + Point.Distance(pos, pp);
								}
							}
						}
						lastProjection = p;

						length += Point.Distance(pos, next);
						pos = next;
					}
				}
				i += 3;  // courbe suivante
			}
			while ( i < this.TotalMainHandle-3 );

			if ( !double.IsNaN(best) && best == 0.0 )
			{
				best = 0.001;  // TODO: améliore grandement la sélection entre 2 TextLine2 !
			}

			return best;
		}

		protected static bool Contains(Point p, Point p1, Point p2)
		{
			//	Retourne true si p est le long de p1-p2.
			return
			(
				p.X >= System.Math.Min(p1.X, p2.X) &&
				p.X <= System.Math.Max(p1.X, p2.X) &&
				p.Y >= System.Math.Min(p1.Y, p2.Y) &&
				p.Y <= System.Math.Max(p1.Y, p2.Y)
			);
		}

		protected void Transform(double position, out Point posXY, out double angle)
		{
			//	Transforme une position le long de la courbe en position x;y (1D -> 2D).
			if ( position <= 0 )  // au début ?
			{
				Point p1 = this.Handle(1).Position;
				Point p2 = this.Handle(2).Position;
				if ( this.Handle(2).Type == HandleType.Hide )  // droite ?
				{
					p2 = this.Handle(4).Position;
				}
				posXY = p1;
				angle = Point.ComputeAngleDeg(p1, p2);
				return;
			}

			double length = 0.0;
			int i = 0;
			do
			{
				Point p1 = this.Handle(i+1).Position;
				Point s1 = this.Handle(i+2).Position;
				Point s2 = this.Handle(i+3).Position;
				Point p2 = this.Handle(i+4).Position;

				if ( this.Handle(i+2).Type == HandleType.Hide )  // droite ?
				{
					double last = length;
					length += Point.Distance(p1, p2);

					if ( position <= length )
					{
						posXY = Point.Move(p1, p2, position-last);
						angle = Point.ComputeAngleDeg(p1, p2);
						return;
					}
				}
				else	// courbe ?
				{
					Point pos = p1;
					int total = (int)(1.0/TextLine2.step);
					for ( int rank=1 ; rank<=total ; rank ++ )
					{
						double t = TextLine2.step*rank;
						Point next = Point.FromBezier(p1,s1,s2,p2, t);
						double last = length;
						length += Point.Distance(pos, next);

						if ( position <= length )
						{
							posXY = Point.Move(pos, next, position-last);
							angle = Point.ComputeAngleDeg(pos, next);
							return;
						}

						pos = next;
					}
				}
				i += 3;  // courbe suivante
			}
			while ( i < this.TotalMainHandle-3 );

			if ( true )  // à la fin ou même plus loin ?
			{
				int total = this.TotalMainHandle;
				Point p1 = this.Handle(total-2).Position;
				Point p2 = this.Handle(total-3).Position;
				if ( this.Handle(total-3).Type == HandleType.Hide )  // droite ?
				{
					p2 = this.Handle(total-5).Position;
				}
				posXY = Point.Move(p1, p2, length-position);
				angle = Point.ComputeAngleDeg(p2, p1);
			}
		}

		protected double GetLength
		{
			//	Retourne la longueur totale d'une courbe multiple.
			get
			{
				if ( this.isDirtyLength )
				{
					this.isDirtyLength = false;
					this.length = 0.0;
					int i = 0;
					do
					{
						Point p1 = this.Handle(i+1).Position;
						Point s1 = this.Handle(i+2).Position;
						Point s2 = this.Handle(i+3).Position;
						Point p2 = this.Handle(i+4).Position;

						if ( this.Handle(i+2).Type == HandleType.Hide )  // droite ?
						{
							this.length += Point.Distance(p1,p2);
						}
						else	// courbe ?
						{
							Point pos = p1;
							int total = (int)(1.0/TextLine2.step);
							for ( int rank=1 ; rank<=total ; rank ++ )
							{
								double t = TextLine2.step*rank;
								Point next = Point.FromBezier(p1,s1,s2,p2, t);
								this.length += Point.Distance(pos, next);
								pos = next;
							}
						}
						i += 3;  // courbe suivante
					}
					while ( i < this.TotalMainHandle-3 );
				}

				return this.length;
			}
		}

		public override double WidthForHRuler
		{
			//	Donne la largeur à utiliser pour la règle horizontale.
			get
			{
				return this.GetLength;
			}
		}


		#region FlowHandles
		protected override void CornersFlowPrev(out Point p1, out Point p2, out Point p3, out Point p4, DrawingContext drawingContext)
		{
			//	Calcules les 4 coins de la poignée "pavé précédent".
			Point pp1 = this.Handle(1).Position;
			Point pp2 = this.Handle(2).Position;
			if ( this.Handle(2).Type == HandleType.Hide )  // droite ?
			{
				pp2 = this.Handle(4).Position;
			}
			double d = Abstract.EditFlowHandleSize/drawingContext.ScaleX;

			p2 = pp1;
			p1 = Point.Move(pp1, pp2, -d);
			p4 = p2 + new Point(p1.Y-p2.Y, p2.X-p1.X);
			p3 = p4 + (p1-p2);
		}

		protected override void CornersFlowNext(out Point p1, out Point p2, out Point p3, out Point p4, DrawingContext drawingContext)
		{
			//	Calcules les 4 coins de la poignée "pavé suivant".
			int total = this.TotalMainHandle;
			Point pp1 = this.Handle(total-2).Position;
			Point pp2 = this.Handle(total-3).Position;
			if ( this.Handle(total-3).Type == HandleType.Hide )  // droite ?
			{
				pp2 = this.Handle(total-5).Position;
			}
			double d = Abstract.EditFlowHandleSize/drawingContext.ScaleX;

			p1 = pp1;
			p2 = Point.Move(pp1, pp2, -d);
			p3 = p1 + new Point(p1.Y-p2.Y, p2.X-p1.X);
			p4 = p3 + (p2-p1);
		}
		#endregion

		
		protected override void UpdateTextFrame()
		{
			//	Met à jour le TextFrame en fonction des dimensions du pavé.
			Text.SingleLineTextFrame frame = this.textFrame as Text.SingleLineTextFrame;
			double width = this.GetLength;

			if ( frame.Width != width )
			{
				frame.Width = width;
				this.textFlow.TextStory.NotifyTextChanged();
			}
		}
		
		public override Drawing.Point ConvertInTextFrame(Drawing.Point pos)
		{
			//	Calcule la coordonnée transformée dans le texte frame.
			double lin = this.Transform(pos);
			if ( !double.IsNaN(lin) )
			{
				Point curve;
				double angle;
				this.Transform(lin, out curve, out angle);
				pos = new Point(lin, 0);
			}
			
			return pos;
		}


		protected override void DrawText(IPaintPort port, DrawingContext drawingContext, InternalOperation op)
		{
			//	Effectue une opération quelconque sur le texte du pavé.
			this.internalOperation = op;

			if ( this.internalOperation == InternalOperation.Painting )
			{
				this.cursorBox = Drawing.Rectangle.Empty;
				this.selectBox = Drawing.Rectangle.Empty;
			}

			this.port = port;
			this.graphics = port as Graphics;
			this.drawingContext = drawingContext;

			this.isActive = true;
			if ( this.document.Modifier != null && this.document.Modifier.ActiveViewer != null )
			{
				this.isActive = (this.document.Modifier.ActiveViewer.DrawingContext == drawingContext &&
								 this.document.Modifier.ActiveViewer.IsFocused);
			}

			if (this.textFlow.HasActiveTextBox && this.graphics != null && this.internalOperation == InternalOperation.Painting)
			{
				Text.ITextFrame frame;
				double cx, cy, ascender, descender, cursorAngle;
				this.textFlow.TextNavigator.GetCursorGeometry (out frame, out cx, out cy, out ascender, out descender, out cursorAngle);
				cursorAngle *= 180.0/System.Math.PI;  // en degrés
				cursorAngle -= 90.0;

				if (frame == this.textFrame)
				{
					Point center;
					double angle;
					this.Transform (cx, out center, out angle);
					// cursorAngle permet de pencher le curseur sur de l'italique.
					Point c1 = Drawing.Transform.RotatePointDeg (center, angle+cursorAngle, new Point (center.X, center.Y+ascender));
					Point c2 = Drawing.Transform.RotatePointDeg (center, angle+cursorAngle, new Point (center.X, center.Y+descender));

					if (!this.textFlow.TextNavigator.HasRealSelection)
					{
						this.graphics.LineWidth = 3.0/drawingContext.ScaleX;
						this.graphics.AddLine (c1, c2);
						this.graphics.RenderSolid (Color.FromAlphaRgb (1, 1, 1, 1));
					}
				}
			}
			
			this.textFlow.TextStory.TextContext.ShowControlCharacters = this.textFlow.HasActiveTextBox && this.drawingContext != null && this.drawingContext.TextShowControlCharacters;
			this.textFlow.TextFitter.RenderTextFrame(this.textFrame, this);

			if ( this.textFlow.HasActiveTextBox && this.graphics != null && this.internalOperation == InternalOperation.Painting )
			{
				//	Peint le curseur uniquement si l'objet est en édition, qu'il n'y a pas
				//	de sélection et que l'on est en train d'afficher à l'écran.
				Text.ITextFrame frame;
				double cx, cy, ascender, descender, cursorAngle;
				this.textFlow.TextNavigator.GetCursorGeometry(out frame, out cx, out cy, out ascender, out descender, out cursorAngle);
				cursorAngle *= 180.0/System.Math.PI;  // en degrés
				cursorAngle -= 90.0;
			
				if ( frame == this.textFrame )
				{
					Point center;
					double angle;
					this.Transform(cx, out center, out angle);
					// cursorAngle permet de pencher le curseur sur de l'italique.
					Point c1 = Drawing.Transform.RotatePointDeg(center, angle+cursorAngle, new Point(center.X, center.Y+ascender));
					Point c2 = Drawing.Transform.RotatePointDeg(center, angle+cursorAngle, new Point(center.X, center.Y+descender));

					if ( !this.textFlow.TextNavigator.HasRealSelection )
					{
						this.graphics.LineWidth = 1.0/drawingContext.ScaleX;
						this.graphics.AddLine(c1, c2);
						this.graphics.RenderSolid(DrawingContext.ColorCursorEdit(this.isActive));
					}

					this.ComputeAutoScroll(c1, c2);
					this.cursorBox.MergeWith(c1);
					this.cursorBox.MergeWith(c2);
					this.selectBox.MergeWith(c1);
					this.selectBox.MergeWith(c2);
				}
			}

			this.port = null;
			this.graphics = null;
			this.drawingContext = null;
		}


		#region ITextRenderer Members
		public bool IsFrameAreaVisible(Text.ITextFrame frame, double x, double y, double width, double height)
		{
			return true;
		}
		
		public void RenderStartParagraph(Text.Layout.Context context)
		{
		}
		
		public void RenderStartLine(Text.Layout.Context context)
		{
			context.DisableSimpleRendering();
		}
		
		public void RenderTab(Text.Layout.Context layout, string tag, double tabOrigin, double tabStop, ulong tabCode, bool isTabDefined, bool isTabAuto)
		{
			if ( this.graphics == null )  return;
			if ( this.drawingContext == null )  return;
			if ( this.drawingContext.TextShowControlCharacters == false )  return;
			if ( this.textFlow.HasActiveTextBox == false )  return;
			if ( isTabAuto )  return;

			double x1 = tabOrigin;
			double x2 = tabStop;
			double y  = layout.LineBaseY + layout.LineAscender*0.3;
			double a  = System.Math.Min(layout.LineAscender*0.3, (x2-x1)*0.5);

			Point p1, p2;
			double a1, a2;
			this.Transform(x1, out p1, out a1);
			this.Transform(x2, out p2, out a2);

			p1 = Drawing.Transform.RotatePointDeg(p1, a1, new Point(p1.X, p1.Y+y));
			p2 = Drawing.Transform.RotatePointDeg(p2, a2, new Point(p2.X, p2.Y+y));

			double aa = Point.ComputeAngleDeg(p2, p1);
			Point p2a = Drawing.Transform.RotatePointDeg(p2, aa+30, new Point(p2.X+a, p2.Y));
			Point p2b = Drawing.Transform.RotatePointDeg(p2, aa-30, new Point(p2.X+a, p2.Y));

			Color color = isTabDefined ? Drawing.Color.FromBrightness(0.8) : DrawingContext.ColorTabZombie;
			
			if ( (tabCode & this.markerSelected) != 0 )  // tabulateur sélectionné ?
			{
				Drawing.Rectangle rect = new Drawing.Rectangle(x1, layout.LineY1, x2-x1, layout.LineY2-layout.LineY1);
				graphics.Align(ref rect);
				
				this.graphics.AddFilledRectangle(rect);
				this.graphics.RenderSolid(DrawingContext.ColorSelectEdit(this.isActive));

				if ( isTabDefined )  color = Drawing.Color.FromBrightness(0.5);
			}
			
			this.graphics.LineWidth = 1.0/this.drawingContext.ScaleX;
			this.graphics.AddLine(p1, p2);
			this.graphics.AddLine(p2, p2a);
			this.graphics.AddLine(p2, p2b);
			this.graphics.RenderSolid(color);
		}
		
		public void Render(Text.Layout.Context layout, Epsitec.Common.OpenType.Font font, double size, string color, Text.Layout.TextToGlyphMapping mapping, ushort[] glyphs, double[] x, double[] y, double[] sx, double[] sy, bool isLastRun)
		{
			if ( this.internalOperation == InternalOperation.Painting )
			{
				System.Diagnostics.Debug.Assert(mapping != null);
				Text.ITextFrame frame = layout.Frame;

				//	Vérifions d'abord que le mapping du texte vers les glyphes est
				//	correct et correspond à quelque chose de valide :
				Drawing.Font drawingFont = Drawing.Font.GetFont(font);
				System.Collections.ArrayList selRectList = null;
				int offset = 0;

				int[]    cArray;  // unicodes
				ushort[] gArray;  // glyphes
				ulong[]  tArray;  // textes

				SpaceType[] iArray = new SpaceType[glyphs.Length];
				int ii = 0;
				bool isSpace = false;

				while ( mapping.GetNextMapping(out cArray, out gArray, out tArray) )
				{
					int numChars  = cArray.Length;
					int numGlyphs = gArray.Length;
					System.Diagnostics.Debug.Assert(numChars == 1 || numGlyphs == 1);

					if ( numChars == 1 && numGlyphs == 1 )
					{
						int code = cArray[0];
						if ( code == 0x20 || code == 0xA0 || code == 0x202F || (code >= 0x2000 && code <= 0x200F) )  // espace ?
						{
							isSpace = true;  // contient au moins un espace
							if ( code == 0xA0 || code == 0x2007 || code == 0x200D || code == 0x202F || code == 0x2060 )
							{
								iArray[ii++] = SpaceType.NoBreakSpace;  // espace insécable
							}
							else
							{
								iArray[ii++] = SpaceType.BreakSpace;  // espace sécable
							}
						}
						else if ( code == 0x0C )  // saut ?
						{
							isSpace = true;  // contient au moins un espace

							Text.Properties.BreakProperty prop;
							this.document.TextContext.GetBreak(tArray[0], out prop);
							if ( prop.ParagraphStartMode == Text.Properties.ParagraphStartMode.NewFrame )
							{
								iArray[ii++] = SpaceType.NewFrame;
							}
							else
							{
								iArray[ii++] = SpaceType.NewPage;
							}
						}
						else
						{
							iArray[ii++] = SpaceType.None;  // pas un espace
						}
					}
					else
					{
						for ( int i=0 ; i<numGlyphs ; i++ )
						{
							iArray[ii++] = SpaceType.None;  // pas un espace
						}
					}

					double x1 = x[offset+0];
					double x2 = x[offset+numGlyphs];
					double yy = y[offset];
					offset += numGlyphs;

					for ( int i=0 ; i<numChars ; i++ )
					{
						if ( (tArray[i] & this.markerSelected) != 0 )
						{
							double ww = (x2-x1)/numChars;
							double xx = x1 + ww*i;
							this.MarkSel(drawingFont, size, ref selRectList, xx, ww, yy);
						}
					}
				}

				if (this.textFlow.HasActiveTextBox && selRectList != null && this.graphics != null && !this.drawingContext.IsBitmap)
				{
					//	Dessine les rectangles correspondant à la sélection (un par caractère).
					foreach ( Drawing.Rectangle rect in selRectList )
					{
						Point p1, p2;
						double a1, a2;
						this.Transform(rect.Left,  out p1, out a1);
						this.Transform(rect.Right, out p2, out a2);

						Point c1 = Drawing.Transform.RotatePointDeg(p1, a1, new Point(p1.X, p1.Y+rect.Bottom));
						Point c2 = Drawing.Transform.RotatePointDeg(p1, a1, new Point(p1.X, p1.Y+rect.Top));
						Point c3 = Drawing.Transform.RotatePointDeg(p2, a2, new Point(p2.X, p2.Y+rect.Top));
						Point c4 = Drawing.Transform.RotatePointDeg(p2, a2, new Point(p2.X, p2.Y+rect.Bottom));

						Path path = new Path();
						path.MoveTo(c1);
						path.LineTo(c2);
						path.LineTo(c3);
						path.LineTo(c4);
						path.Close();
						this.graphics.Rasterizer.AddSurface(path);

						this.selectBox.MergeWith(c1);
						this.selectBox.MergeWith(c2);
						this.selectBox.MergeWith(c3);
						this.selectBox.MergeWith(c4);
					}
					this.graphics.RenderSolid(DrawingContext.ColorSelectEdit(this.isActive));
				}

				//	Dessine le texte.
				this.RenderText(font, size, glyphs, iArray, x, y, sx, sy, RichColor.Parse(color), isSpace);
			}
			
			if ( this.internalOperation == InternalOperation.GetPath )
			{
				this.RenderText(font, size, glyphs, null, x, y, sx, sy, RichColor.Empty, false);
			}

			if ( this.internalOperation == InternalOperation.CharactersTable )
			{
				int[]    cArray;
				ushort[] gArray;
				ulong[]  tArray;
				while ( mapping.GetNextMapping(out cArray, out gArray, out tArray) )
				{
					int numChars  = cArray.Length;
					int numGlyphs = gArray.Length;
					System.Diagnostics.Debug.Assert(numChars == 1 || numGlyphs == 1);

					for ( int i=0 ; i<numGlyphs ; i++ )
					{
						if ( gArray[i] >= 0xffff )  continue;

						PDF.CharacterList cl;
						if ( numChars == 1 )
						{
							if ( i == 1 )  // TODO: césure gérée de façon catastrophique !
							{
								cl = new PDF.CharacterList(gArray[i], (int)'-', font);
							}
							else
							{
								cl = new PDF.CharacterList(gArray[i], cArray[0], font);
							}
						}
						else
						{
							cl = new PDF.CharacterList(gArray[i], cArray, font);
						}

						if ( !this.charactersTable.ContainsKey(cl) )
						{
							this.charactersTable.Add(cl, null);
						}
					}
				}
			}

			if ( this.internalOperation == InternalOperation.RealBoundingBox )
			{
				this.RenderText(font, size, glyphs, null, x, y, sx, sy, RichColor.Empty, false);
			}

			if ( this.internalOperation == InternalOperation.RealSelectPath )
			{
				this.RenderText(font, size, glyphs, null, x, y, sx, sy, RichColor.Empty, false);
			}
		}

		protected void MarkSel(Drawing.Font drawingFont, double size, ref System.Collections.ArrayList selRectList, double x, double w, double y)
		{
			//	Marque un caractère sélectionné.
			if ( this.graphics == null )  return;

			double ascender  = drawingFont.Ascender*size;
			double descender = drawingFont.Descender*size;

			Drawing.Rectangle rect = new Drawing.Rectangle(x, y+descender, w, ascender-descender);
			graphics.Align(ref rect);

			if ( selRectList == null )
			{
				selRectList = new System.Collections.ArrayList();
			}

			selRectList.Add(rect);
		}

		protected void RenderText(Epsitec.Common.OpenType.Font font, double size, ushort[] glyphs, SpaceType[] insecs, double[] x, double[] y, double[] sx, double[] sy, RichColor color, bool isSpace)
		{
			//	Effectue le rendu des caractères.
			if ( this.internalOperation == InternalOperation.Painting )
			{
				if ( this.graphics != null )  // affichage sur écran ?
				{
					Drawing.Font drawingFont = Drawing.Font.GetFont(font);
					if ( drawingFont != null )
					{
						// Dessine les caractères.
						for ( int i=0 ; i<glyphs.Length ; i++ )
						{
							if ( glyphs[i] < 0xffff )
							{
								double width = x[i+1] - x[i];  // largeur du glyph

								Point pos;
								double angle;
								this.Transform(x[i]+width/2, out pos, out angle);
								pos = Drawing.Transform.RotatePointDeg(pos, angle, new Point(pos.X, pos.Y+y[i]));

								Transform initial = this.graphics.Transform;
								this.graphics.TranslateTransform(pos.X, pos.Y);
								this.graphics.RotateTransformDeg(angle, 0, 0);
								this.graphics.Rasterizer.AddGlyph(drawingFont, glyphs[i], -width/2, 0, size, (sx == null) ? 1.0 : sx[i], (sy == null) ? 1.0 : sy[i]);
								this.graphics.Transform = initial;
							}
						}

						if ( this.textFlow.HasActiveTextBox && isSpace && insecs != null &&
							 this.drawingContext != null && this.drawingContext.TextShowControlCharacters )
						{
							for ( int i=0 ; i<glyphs.Length ; i++ )
							{
								double width = font.GetGlyphWidth(glyphs[i], size);
								double oy = font.GetAscender(size)*0.3;

								Point pos;
								double angle;
								this.Transform(x[i], out pos, out angle);
								pos = Drawing.Transform.RotatePointDeg(pos, angle, new Point(pos.X, pos.Y+y[i]));

								Transform initial = this.graphics.Transform;
								this.graphics.TranslateTransform(pos.X, pos.Y);
								this.graphics.RotateTransformDeg(angle, 0, 0);

								if ( insecs[i] == SpaceType.BreakSpace )  // espace sécable ?
								{
									this.graphics.AddFilledCircle(width/2, oy, size*0.05);
								}

								if ( insecs[i] == SpaceType.NoBreakSpace )  // espace insécable ?
								{
									this.graphics.AddCircle(width/2, oy, size*0.08);
								}

								if ( insecs[i] == SpaceType.NewFrame ||
									 insecs[i] == SpaceType.NewPage  )  // saut ?
								{
									Text.SingleLineTextFrame frame = this.textFrame as Text.SingleLineTextFrame;
									Point p1 = new Point(0,           oy);
									Point p2 = new Point(frame.Width, oy);
									Path path = Path.FromLine(p1, p2);

									double w    = (insecs[i] == SpaceType.NewFrame) ? 0.8 : 0.5;
									double dash = (insecs[i] == SpaceType.NewFrame) ? 0.0 : 8.0;
									double gap  = (insecs[i] == SpaceType.NewFrame) ? 3.0 : 2.0;
									Drawer.DrawPathDash(this.graphics, this.drawingContext, path, w, dash, gap, color.Basic);
								}

								this.graphics.Transform = initial;
							}
						}
			
						this.graphics.RenderSolid(color.Basic);
					}
				}
				else if ( this.port is Printing.PrintPort )  // impression ?
				{
					Printing.PrintPort printPort = port as Printing.PrintPort;
					printPort.RichColor = color;
					Drawing.Font drawingFont = Drawing.Font.GetFont(font);
					
					for ( int i=0 ; i<glyphs.Length ; i++ )
					{
						if ( glyphs[i] < 0xffff )
						{
							double width = x[i+1] - x[i];  // largeur du glyph

							Point pos;
							double angle;
							this.Transform(x[i]+width/2, out pos, out angle);
							pos = Drawing.Transform.RotatePointDeg(pos, angle, new Point(pos.X, pos.Y+y[i]));

							Transform initial = printPort.Transform;
							printPort.TranslateTransform(pos.X, pos.Y);
							printPort.RotateTransformDeg(angle, 0, 0);

							ushort[] oneGlyph = new ushort[1];
							double[] oneX = new double[1];
							double[] oneY = new double[1];
							double[] oneSx = new double[1];
							double[] oneSy = new double[1];

							oneGlyph[0] = glyphs[i];
							oneX[0] = -width/2;
							oneY[0] = 0;
							oneSx[0] = (sx == null) ? 1.0 : sx[i];
							oneSy[0] = (sy == null) ? 1.0 : sy[i];

							printPort.PaintGlyphs(drawingFont, size, oneGlyph, oneX, oneY, oneSx, oneSy);

							printPort.Transform = initial;
						}
					}
				}
				else if ( this.port is PDF.Port )  // exportation PDF ?
				{
					PDF.Port pdfPort = port as PDF.Port;
					pdfPort.RichColor = color;
					Drawing.Font drawingFont = Drawing.Font.GetFont(font);
					
					for ( int i=0 ; i<glyphs.Length ; i++ )
					{
						if ( glyphs[i] < 0xffff )
						{
							double width = x[i+1] - x[i];  // largeur du glyph

							Point pos;
							double angle;
							this.Transform(x[i]+width/2, out pos, out angle);
							pos = Drawing.Transform.RotatePointDeg(pos, angle, new Point(pos.X, pos.Y+y[i]));

							Transform initial = pdfPort.Transform;
							pdfPort.TranslateTransform(pos.X, pos.Y);
							pdfPort.RotateTransformDeg(angle, 0, 0);

							ushort[] oneGlyph = new ushort[1];
							double[] oneX = new double[1];
							double[] oneY = new double[1];
							double[] oneSx = new double[1];
							double[] oneSy = new double[1];

							oneGlyph[0] = glyphs[i];
							oneX[0] = -width/2;
							oneY[0] = 0;
							oneSx[0] = (sx == null) ? 1.0 : sx[i];
							oneSy[0] = (sy == null) ? 1.0 : sy[i];

							pdfPort.PaintGlyphs(drawingFont, size, oneGlyph, oneX, oneY, oneSx, oneSy);

							pdfPort.Transform = initial;
						}
					}
				}
			}

			if ( this.internalOperation == InternalOperation.GetPath )
			{
				Drawing.Font drawingFont = Drawing.Font.GetFont(font);
					
				for ( int i=0 ; i<glyphs.Length ; i++ )
				{
					if ( glyphs[i] < 0xffff )
					{
						double width = x[i+1] - x[i];  // largeur du glyph

						Point pos;
						double angle;
						this.Transform(x[i]+width/2, out pos, out angle);
						pos = Drawing.Transform.RotatePointDeg(pos, angle, new Point(pos.X, pos.Y+y[i]));

						Transform initial = this.graphics.Transform;
						this.graphics.TranslateTransform(pos.X, pos.Y);
						this.graphics.RotateTransformDeg(angle, 0, 0);

						ushort[] oneGlyph = new ushort[1];
						double[] oneX = new double[1];
						double[] oneY = new double[1];
						double[] oneSx = new double[1];
						double[] oneSy = new double[1];

						oneGlyph[0] = glyphs[i];
						oneX[0] = -width/2;
						oneY[0] = 0;
						oneSx[0] = (sx == null) ? 1.0 : sx[i];
						oneSy[0] = (sy == null) ? 1.0 : sy[i];

						this.graphics.PaintGlyphs(drawingFont, size, oneGlyph, oneX, oneY, oneSx, oneSy);

						this.graphics.Transform = initial;
					}
				}
			}

			if ( this.internalOperation == InternalOperation.RealBoundingBox )
			{
				Drawing.Font drawingFont = Drawing.Font.GetFont(font);
				if ( drawingFont != null )
				{
					double ascender  = drawingFont.Ascender*size;
					double descender = drawingFont.Descender*size;

					for ( int i=0 ; i<glyphs.Length ; i++ )
					{
						Point p1, p2;
						double a1, a2;
						this.Transform(x[i+0], out p1, out a1);
						this.Transform(x[i+1], out p2, out a2);
						p1 = Drawing.Transform.RotatePointDeg(p1, a1, new Point(p1.X, p1.Y+y[i]));
						p2 = Drawing.Transform.RotatePointDeg(p2, a2, new Point(p2.X, p2.Y+y[i]));
						
						Drawing.Rectangle bounds;

						if ( glyphs[i] < 0xffff )
						{
							bounds = drawingFont.GetGlyphBounds(glyphs[i], size);
						}
						else
						{
							bounds = drawingFont.GetCharBounds((int)Unicode.Code.EndOfText);
							bounds.Scale(size);
						}

						if ( sx != null )  bounds.Scale(sx[i], 1.0);
						if ( sy != null )  bounds.Scale(1.0, sy[i]);

						Point c1 = Drawing.Transform.RotatePointDeg(p1, a1, p1+bounds.BottomLeft);
						Point c2 = Drawing.Transform.RotatePointDeg(p1, a1, p1+bounds.TopLeft);
						Point c3 = Drawing.Transform.RotatePointDeg(p1, a1, p1+bounds.BottomRight);
						Point c4 = Drawing.Transform.RotatePointDeg(p1, a1, p1+bounds.TopRight);

						this.mergingBoundingBox.MergeWith(c1);
						this.mergingBoundingBox.MergeWith(c2);
						this.mergingBoundingBox.MergeWith(c3);
						this.mergingBoundingBox.MergeWith(c4);

						c1 = Drawing.Transform.RotatePointDeg(p1, a1, new Point(p1.X, p1.Y+descender));
						c2 = Drawing.Transform.RotatePointDeg(p1, a1, new Point(p1.X, p1.Y+ascender));
						c3 = Drawing.Transform.RotatePointDeg(p2, a2, new Point(p2.X, p2.Y+descender));
						c4 = Drawing.Transform.RotatePointDeg(p2, a2, new Point(p2.X, p2.Y+ascender));

						this.mergingBoundingBox.MergeWith(c1);
						this.mergingBoundingBox.MergeWith(c2);
						this.mergingBoundingBox.MergeWith(c3);
						this.mergingBoundingBox.MergeWith(c4);
					}
				}
			}

			if ( this.internalOperation == InternalOperation.RealSelectPath )
			{
				Drawing.Font drawingFont = Drawing.Font.GetFont(font);
				if ( drawingFont != null )
				{
					double ascender  = drawingFont.Ascender*size;
					double descender = drawingFont.Descender*size;

					for ( int i=0 ; i<glyphs.Length ; i++ )
					{
						if ( glyphs[i] < 0xffff )
						{
							Point p1, p2;
							double a1, a2;
							this.Transform(x[i+0], out p1, out a1);
							this.Transform(x[i+1], out p2, out a2);
							p1 = Drawing.Transform.RotatePointDeg(p1, a1, new Point(p1.X, p1.Y+y[i]));
							p2 = Drawing.Transform.RotatePointDeg(p2, a2, new Point(p2.X, p2.Y+y[i]));
							
							Point c1 = Drawing.Transform.RotatePointDeg(p1, a1, new Point(p1.X, p1.Y+descender));
							Point c2 = Drawing.Transform.RotatePointDeg(p1, a1, new Point(p1.X, p1.Y+ascender));
							Point c3 = Drawing.Transform.RotatePointDeg(p2, a2, new Point(p2.X, p2.Y+descender));
							Point c4 = Drawing.Transform.RotatePointDeg(p2, a2, new Point(p2.X, p2.Y+ascender));

							this.realSelectPath.MoveTo(c1);
							this.realSelectPath.LineTo(c2);
							this.realSelectPath.LineTo(c4);
							this.realSelectPath.LineTo(c3);
							this.realSelectPath.Close();
						}
					}
				}
			}
		}

		public void Render(Text.Layout.Context layout, Text.IGlyphRenderer glyphRenderer, string color, double x, double y, bool isLastRun)
		{
			glyphRenderer.RenderGlyph(layout.Frame, x, y);
		}
		
		public void RenderEndLine(Text.Layout.Context context)
		{
		}
		
		public void RenderEndParagraph(Text.Layout.Context context)
		{
			if ( this.internalOperation != InternalOperation.Painting )  return;

			Text.Layout.XlineRecord[] records = context.XlineRecords;
			if ( records.Length == 0 )  return;

			System.Collections.ArrayList process = new System.Collections.ArrayList();
			
			for ( int lineStart=0 ; lineStart<records.Length ; )
			{
				bool found;
				
				do
				{
					Text.Properties.AbstractXlineProperty xline = null;
					Text.Properties.FontColorProperty color = null;
					
					Text.Layout.XlineRecord starting = null;
					Text.Layout.XlineRecord ending   = null;

					found = false;
					
					for ( int i=lineStart ; i<records.Length ; i++ )
					{
						if ( records[i].Type == Text.Layout.XlineRecord.RecordType.LineEnd )
						{
							if ( starting != null )
							{
								System.Diagnostics.Debug.Assert(xline != null);
								
								ending = records[i];
								found  = true;
								this.RenderXline(context, xline, starting, ending);  // dessine le trait
								process.Add(new XlineInfo(xline, color));  // le trait est fait
							}
							break;
						}
						
						ending = records[i];
						
						for ( int j=0 ; j<records[i].Xlines.Length ; j++ )
						{
							if ( xline == null )  // cherche le début ?
							{
								if ( TextLine2.XlineContains(process, records[i].Xlines[j], records[i].TextColor) )  continue;

								xline    = records[i].Xlines[j];
								color    = records[i].TextColor;
								starting = records[i];
								ending   = null;  // la fin ne peut pas être dans ce record
								break;
							}
							else if ( starting == null )	// cherche un autre début ?
							{
								if ( !Text.Property.CompareEqualContents(xline, records[i].Xlines[j]) ||
									 !Text.Property.CompareEqualContents(color, records[i].TextColor) )
								{
									continue;
								}
								
								starting = records[i];
								ending   = null;  // la fin ne peut pas être dans ce record
								break;
							}
							else	// cherche la fin ?
							{
								if ( Text.Property.CompareEqualContents(xline, records[i].Xlines[j]) &&
									 Text.Property.CompareEqualContents(color, records[i].TextColor) )
								{
									ending = null;  // la fin ne peut pas être dans ce record
									break;
								}
							}
						}
						
						if ( starting != null && ending != null )  // fin trouvée ?
						{
							System.Diagnostics.Debug.Assert(xline != null);
							
							this.RenderXline(context, xline, starting, ending);  // dessine le trait
							process.Add(new XlineInfo(xline, color));  // le trait est fait
							
							//	Cherche encore d'autres occurrences de la même propriété dans
							//	la même ligne...
							
							starting = null;
							ending   = null;
							found    = true;
						}
					}
				}
				while ( found );
				
				//	Saute les enregistrements de la ligne courante et reprend tout depuis
				//	le début de la ligne suivante:
				
				while ( lineStart<records.Length )
				{
					if ( records[lineStart++].Type == Text.Layout.XlineRecord.RecordType.LineEnd )  break;
				}
				
				process.Clear();
			}
		}

		protected void RenderXline(Text.Layout.Context context, Text.Properties.AbstractXlineProperty xline, Text.Layout.XlineRecord starting, Text.Layout.XlineRecord ending)
		{
			//	Dessine un trait souligné, surligné ou biffé.
			if ( ending.X <= starting.X )  return;
			
			double y = starting.Y;

			if ( xline.WellKnownType == Text.Properties.WellKnownType.Underline )
			{
				y -= xline.Position;
			}
			if ( xline.WellKnownType == Text.Properties.WellKnownType.Overline )
			{
				y += context.LineAscender;
				y -= xline.Position;
			}
			if ( xline.WellKnownType == Text.Properties.WellKnownType.Strikeout )
			{
				y += xline.Position;
			}

			Point p1, p2;
			double a1, a2;
			this.Transform(starting.X, out p1, out a1);
			this.Transform(ending.X,   out p2, out a2);

			Point c1 = Drawing.Transform.RotatePointDeg(p1, a1, new Point(p1.X, p1.Y+y-xline.Thickness/2));
			Point c2 = Drawing.Transform.RotatePointDeg(p1, a1, new Point(p1.X, p1.Y+y+xline.Thickness/2));
			Point c3 = Drawing.Transform.RotatePointDeg(p2, a2, new Point(p2.X, p2.Y+y-xline.Thickness/2));
			Point c4 = Drawing.Transform.RotatePointDeg(p2, a2, new Point(p2.X, p2.Y+y+xline.Thickness/2));

			Path path = new Path();
			path.MoveTo(c1);
			path.LineTo(c2);
			path.LineTo(c4);
			path.LineTo(c3);
			path.Close();

			string color = xline.DrawStyle;
			if ( color == null )  // couleur par défaut (comme le texte) ?
			{
				color = starting.TextColor.TextColor;
			}
			this.port.RichColor = RichColor.Parse(color);

			this.port.PaintSurface(path);
		}
		#endregion


		public override void UpdateGeometry()
		{
			//	Met à jour après un changement de géométrie de l'objet.
			this.isDirtyLength = true;
			base.UpdateGeometry();
		}


		public override Path GetMagnetPath()
		{
			//	Retourne le chemin géométrique de l'objet pour les constructions
			//	magnétiques.
			Path path = this.PathBuild();
			return path;
		}

		public override Path GetShaperPath()
		{
			//	Retourne le chemin géométrique de l'objet pour le modeleur.
			Path path = this.PathBuild();
			return path;
		}

		public void CreateFinalise()
		{
			//	Finalise la création d'une courbe de Bézier.
			this.HandlePropertiesCreate();  // crée les poignées des propriétés
			this.Select(false);
			this.Select(true);  // pour sélectionner toutes les poignées
		}


		#region Serialization
		protected TextLine2(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui désérialise l'objet.
		}
		#endregion


		protected Point							initialPos;
		protected Path							realSelectPath;
		protected bool							isDirtyLength = true;
		protected double						length;

		protected static readonly double		step = 1.0/100.0;  // une courbe de Bézier est décomposée en 100 segments
	}
}

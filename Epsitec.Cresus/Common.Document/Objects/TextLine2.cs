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
	public class TextLine2 : Objects.Abstract
	{
		public TextLine2(Document document, Objects.Abstract model) : base(document, model)
		{
			if ( this.document == null )  return;  // objet factice ?
			this.CreateProperties(model, false);
			this.Initialise();
		}

		protected void Initialise()
		{
			this.textFrame = new Text.SimpleTextFrame();
			
			this.NewTextFlow();
			this.InitialiseInternals();
		}

		protected void InitialiseInternals()
		{
			if ( this.textFrame == null )
			{
				this.textFrame = new Text.SimpleTextFrame();
			}

			System.Diagnostics.Debug.Assert(this.textFlow != null);
			
			this.markerSelected = this.document.TextContext.Markers.Selected;

			this.cursorBox = Drawing.Rectangle.Empty;
			this.selectBox = Drawing.Rectangle.Empty;
		}

		public override void NewTextFlow()
		{
			//	Crée un nouveau TextFlow pour l'objet.
			TextFlow flow = new TextFlow(this.document);
			this.document.TextFlows.Add(flow);
			this.TextFlow = flow;
			flow.Add(this, null, true);
		}

		public override TextFlow TextFlow
		{
			//	TextFlow associé à l'objet.
			get
			{
				return this.textFlow;
			}

			set
			{
				if ( this.textFlow != value )
				{
					this.InsertOpletTextFlow();
					this.textFlow = value;

					this.UpdateTextLayout();
					this.textFlow.NotifyAreaFlow();
				}
			}
		}

		public override Text.ITextFrame TextFrame
		{
			//	Donne le TextFrame associé à l'objet.
			get
			{
				return this.textFrame;
			}
		}

		protected override bool ExistingProperty(Properties.Type type)
		{
			if ( type == Properties.Type.Name )  return true;
			return false;
		}

		protected override Objects.Abstract CreateNewObject(Document document, Objects.Abstract model)
		{
			return new TextLine2(document, model);
		}

		public override void Dispose()
		{
			if ( this.textFlow != null )
			{
				this.textFlow.Remove(this);  // objet seul dans son propre flux

				if ( this.textFlow.Count == 1 )  // est-on le dernier et seul utilisateur ?
				{
					this.document.TextFlows.Remove(this.textFlow);
				}
			}

			base.Dispose();
		}


		public override string IconName
		{
			//	Nom de l'icône.
			get { return Misc.Icon("ObjectTextLine"); }
		}

		protected TextNavigator2 MetaNavigator
		{
			//	MetaNavigator associé au TextFlow.
			get
			{
				if ( this.textFlow == null )
				{
					return null;
				}
				else
				{
					return this.textFlow.MetaNavigator;
				}
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

		public override DetectEditType DetectEdit(Point pos)
		{
			//	Détecte si la souris est sur l'objet pour l'éditer.
			if ( this.isHide )  return DetectEditType.Out;

			Drawing.Rectangle bbox = this.BoundingBox;
			if ( !bbox.Contains(pos) )  return DetectEditType.Out;

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			Shape[] shapes = this.ShapesBuild(null, context, false);
			if ( context.Drawer.DetectOutline(pos, context, shapes) != -1 )  return DetectEditType.Body;

			//?if ( this.DetectTextCurve(pos) )  return DetectEditType.Body;
			return DetectEditType.Out;
		}


		public override void MoveAllProcess(Point move)
		{
			//	Déplace tout l'objet.
			this.document.Notifier.NotifyArea(this.BoundingBox);

			bool allHandle = !this.IsSelected;
			int total = this.handles.Count;
			for ( int i=0 ; i<total ; i+=3 )
			{
				if ( allHandle || this.Handle(i+1).IsVisible )
				{
					this.Handle(i+0).Position += move;
					this.Handle(i+1).Position += move;
					this.Handle(i+2).Position += move;
				}
			}

			this.SetDirtyBbox();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override void MoveGlobalProcess(Selector selector)
		{
			//	Déplace globalement l'objet.
			base.MoveGlobalProcess(selector);
			this.UpdateGeometry();
			this.textFlow.NotifyAreaFlow();
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
			int curr = rank+3;
			int next = rank+6;
			if ( next >= this.handles.Count )  next = 0;

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
			this.HandleDelete(rank-1);
			this.HandleDelete(rank-1);
			this.HandleDelete(rank-1);

			int prev = rank-4;
			if ( prev < 0 )  prev = this.handles.Count-3;
			int next = rank-1;
			if ( next >= this.handles.Count )  next = 0;

			if ( this.Handle(prev+2).Type == HandleType.Hide || this.Handle(next+0).Type == HandleType.Hide )
			{
				if ( this.Handle(prev+2).Type != this.Handle(next+0).Type )
				{
					this.ShaperHandleToCurve(prev);
				}
			}
			this.SetDirtyBbox();
		}

		protected void ShaperHandleToLine(int rank)
		{
			//	Conversion d'un segement en ligne droite.
			int next = rank+3;
			if ( next >= this.handles.Count )  next = 0;
			this.Handle(rank+2).Position = this.Handle(rank+1).Position;
			this.Handle(next+0).Position = this.Handle(next+1).Position;
			this.Handle(rank+2).Type = HandleType.Hide;
			this.Handle(next+0).Type = HandleType.Hide;
			this.Handle(rank+1).ConstrainType = HandleConstrainType.Corner;
			this.Handle(next+1).ConstrainType = HandleConstrainType.Corner;
			this.SetDirtyBbox();
		}

		protected void ShaperHandleToCurve(int rank)
		{
			//	Conversion d'un segement en courbe.
			int next = rank+3;
			if ( next >= this.handles.Count )  next = 0;
			this.Handle(rank+2).Position = Point.Scale(this.Handle(rank+1).Position, this.Handle(next+1).Position, 0.25);
			this.Handle(next+0).Position = Point.Scale(this.Handle(next+1).Position, this.Handle(rank+1).Position, 0.25);
			this.Handle(rank+2).Type = HandleType.Bezier;
			this.Handle(next+0).Type = HandleType.Bezier;
			this.Handle(rank+1).ConstrainType = HandleConstrainType.Corner;
			this.Handle(next+1).ConstrainType = HandleConstrainType.Corner;
			this.SetDirtyBbox();
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
			rankExtremity = rankPrimary - (rankSecondary-rankPrimary)*3;
			if ( rankExtremity < 0 )  rankExtremity = this.handles.Count-2;
			if ( rankExtremity >= this.handles.Count )  rankExtremity = 1;

			if ( this.Handle(rankPrimary).ConstrainType != HandleConstrainType.Smooth )  return;
			int rankOpposite = rankPrimary - (rankSecondary-rankPrimary);
			if ( this.Handle(rankOpposite).Type != HandleType.Hide )  return;

			double dist = Point.Distance(this.Handle(rankPrimary).Position, this.Handle(rankSecondary).Position);
			Point pos = new Point();
			pos = Point.Move(this.Handle(rankPrimary).Position, this.Handle(rankExtremity).Position, dist);
			pos = Point.Symmetry(this.Handle(rankPrimary).Position, pos);
			this.Handle(rankSecondary).Position = pos;
			this.SetDirtyBbox();
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
						}

						if ( this.Handle(rank+1).Type == HandleType.Hide )
						{
							drawingContext.ConstrainAddLine(this.Handle(rank).Position, this.Handle(next+1).Position);
							drawingContext.ConstrainAddHV(this.Handle(next+1).Position);
						}

						drawingContext.ConstrainAddHV(this.Handle(rank).Position);
					}
					else	// poignée secondaire ?
					{
						pos = this.Handle((rank/3)*3+1).Position;
						drawingContext.ConstrainAddLine(this.Handle(rank).Position, pos);
						drawingContext.ConstrainAddHV(pos);

						if ( rank%3 == 0 && this.Handle(rank+2).Type == HandleType.Hide )
						{
							drawingContext.ConstrainAddLine(this.Handle(rank+1).Position, this.Handle(next+1).Position);
						}

						if ( rank%3 == 2 && this.Handle(rank-2).Type == HandleType.Hide )
						{
							drawingContext.ConstrainAddLine(this.Handle(rank-1).Position, this.Handle(prev+1).Position);
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
			if ( rank >= this.handles.Count )  // poignée d'une propriété ?
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

			this.SetDirtyBbox();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override void MoveSelectedSegmentEnding(int rank, Point pos, DrawingContext drawingContext)
		{
			//	Fin du déplacement d'une poignée d'un segment sélectionné.
			base.MoveSelectedSegmentEnding(rank, pos, drawingContext);
		}

		
		public override void CreateMouseDown(Point pos, DrawingContext drawingContext)
		{
			//	Début de la création d'un objet.
			drawingContext.ConstrainFlush();
			drawingContext.ConstrainAddHV(pos);

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
			this.isCreating = false;
			this.document.Modifier.TextInfoModif = "";
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		public override bool CreateIsExist(DrawingContext drawingContext)
		{
			//	Indique si l'objet doit exister. Retourne false si l'objet ne peut
			//	pas exister et doit être détruit.
			this.Deselect();
			double len = Point.Distance(this.Handle(1).Position, this.Handle(4).Position);
			return ( len > drawingContext.MinimalSize );
		}

		public override bool EditAfterCreation()
		{
			//	Indique s'il faut sélectionner l'objet après sa création.
			return true;
		}

		public override void FillFontFaceList(System.Collections.ArrayList list)
		{
			//	Ajoute toutes les fontes utilisées par l'objet dans une liste.
			//?this.textLayout.FillFontFaceList(list);
		}

		public override void FillOneCharList(IPaintPort port, DrawingContext drawingContext, System.Collections.Hashtable table)
		{
			//	Ajoute tous les caractères utilisés par l'objet dans une table.
			this.charactersTable = table;
			this.DrawText(port, drawingContext, InternalOperation.CharactersTable);
			this.charactersTable = null;
		}

		public override bool IsEditable
		{
			//	Indique si un objet est éditable.
			get { return true; }
		}


		public override void CloneObject(Objects.Abstract src)
		{
			//	Reprend toutes les caractéristiques d'un objet.
			base.CloneObject(src);

			TextLine2 srcText = src as TextLine2;
			System.Diagnostics.Debug.Assert(srcText != null);

			if ( srcText.textFlow.Count == 1 ||  // objet solitaire ?
				 srcText.document != this.document )
			{
				this.textFlow.MergeWith(srcText.textFlow);  // copie le texte
			}
			else	// objet d'une chaîne ?
			{
				srcText.textFlow.Add(this, srcText, true);  // met dans la chaîne
			}

			this.UpdateGeometry();
		}

		
		public override void PutCommands(System.Collections.ArrayList list)
		{
			//	Met les commandes pour l'objet dans une liste.
			base.PutCommands(list);

			if ( this.document.Modifier.Tool == "ToolEdit" )
			{
				bool sel = (this.textFlow.TextNavigator.SelectionCount != 0);
				if ( sel )
				{
					this.PutCommands(list, "Cut");
					this.PutCommands(list, "Copy");
					this.PutCommands(list, "Paste");
					this.PutCommands(list, "");
					this.PutCommands(list, "FontQuick1");
					this.PutCommands(list, "FontQuick2");
					this.PutCommands(list, "FontQuick3");
					this.PutCommands(list, "FontQuick4");
					this.PutCommands(list, "");
					this.PutCommands(list, "FontBold");
					this.PutCommands(list, "FontItalic");
					this.PutCommands(list, "FontUnderlined");
					this.PutCommands(list, "");
					this.PutCommands(list, "FontSubscript");
					this.PutCommands(list, "FontSuperscript");
					this.PutCommands(list, "");
					this.PutCommands(list, "FontSizeMinus");
					this.PutCommands(list, "FontSizePlus");
					this.PutCommands(list, "");
					this.PutCommands(list, "FontClear");
					this.PutCommands(list, "");
				}
				else
				{
					this.PutCommands(list, "Paste");
				}
			}
		}

		
		public override bool EditProcessMessage(Message message, Point pos)
		{
			//	Gestion d'un événement pendant l'édition.
			if ( message.IsKeyType )
			{
				this.document.Modifier.ActiveViewer.CloseMiniBar(false);
			}

			if ( message.Type == MessageType.KeyDown   ||
				 message.Type == MessageType.KeyPress  ||
				 message.Type == MessageType.MouseDown )
			{
				this.autoScrollOneShot = true;
			}

			if ( message.Type == MessageType.KeyPress )
			{
				if ( this.EditProcessKeyPress(message) )  return true;
			}

			if ( message.Type == MessageType.KeyDown )
			{
				if ( message.KeyCode == KeyCode.Return ||
					 message.KeyCode == KeyCode.Tab    )  return false;
			}

			Point ppos;
			ITextFrame frame;
			
			if ( this.IsInTextFrame(pos, out ppos) )
			{
				frame = this.textFrame;
			}
			else
			{
				//	Si la souris n'est pas dans notre texte frame, on utilise le text
				//	frame correspondant à sa position (s'il y en a un).
				
				frame = this.textFlow.FindTextFrame(pos, out ppos);
				
				if ( frame == null )  frame = this.textFrame;
			}
			
			if ( !this.MetaNavigator.ProcessMessage(message, ppos, frame) )  return false;
			
			if ( message.Type == MessageType.MouseDown )
			{
				this.document.Modifier.ActiveViewer.CloseMiniBar(false);
			}

			return true;
		}

		protected bool EditProcessKeyPress(Message message)
		{
			//	Gestion des événements clavier.
			if ( message.IsCtrlPressed )
			{
				switch ( message.KeyCode )
				{
					case KeyCode.AlphaX:  return this.EditCut();
					case KeyCode.AlphaC:  return this.EditCopy();
					case KeyCode.AlphaV:  return this.EditPaste();
					case KeyCode.AlphaA:  return this.EditSelectAll();
				}
			}
			return false;
		}

		#region CopyPaste
		public override bool EditCut()
		{
			this.EditCopy();
			this.MetaNavigator.DeleteSelection();
			return true;
		}
		
		public override bool EditCopy()
		{
			string[] texts = this.textFlow.TextNavigator.GetSelectedTexts();
			if ( texts == null || texts.Length == 0 )  return false;

			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			foreach ( string part in texts )
			{
				builder.Append(part);
			}
			string text = builder.ToString();

			Support.Clipboard.WriteData data = new Support.Clipboard.WriteData();
			data.WriteHtmlFragment(text);
			data.WriteTextLayout(text);
			Support.Clipboard.SetData(data);
			return true;
		}
		
		public override bool EditPaste()
		{
			Support.Clipboard.ReadData data = Support.Clipboard.GetData();
			string text = data.ReadTextLayout();
			if ( text == null )
			{
				text = data.ReadText();
				if ( text != null )
				{
					text = text.Replace("\r\n", "\u2029");		//	ParagraphSeparator
					text = text.Replace("\n", "\u2028");		//	LineSeparator
					text = text.Replace("\r", "\u2028");		//	LineSeparator
				}
			}
			if ( text == null )  return false;
			this.MetaNavigator.Insert(text);
			this.textFlow.NotifyAreaFlow();
			return true;
		}

		public override bool EditSelectAll()
		{
			this.MetaNavigator.SelectAll();
			return true;
		}
		#endregion

		public override bool EditInsertText(string text, string fontFace, string fontStyle)
		{
			//	Insère un texte dans le pavé en édition.
			this.MetaNavigator.EndSelection();
			this.document.Modifier.OpletQueueBeginActionNoMerge(Res.Strings.Action.Text.Glyphs.Insert);

			if ( fontFace == "" )
			{
				this.MetaNavigator.Insert(text);
			}
			else
			{
				for ( int i=0 ; i<text.Length ; i++ )
				{
					OpenType.Font font = TextContext.GetFont(fontFace, fontStyle);
					Text.Unicode.Code code = (Text.Unicode.Code) text[i];
					int glyph = font.GetGlyphIndex(text[i]);
					Text.Properties.OpenTypeProperty otp = new Text.Properties.OpenTypeProperty(fontFace, fontStyle, glyph);
					this.MetaNavigator.Insert(code, otp);
				}
			}

			this.document.Modifier.OpletQueueValidateAction();
			this.textFlow.NotifyAreaFlow();
			return true;
		}

		public override bool EditInsertText(Unicode.Code code)
		{
			//	Insère un texte dans le pavé en édition.
			this.MetaNavigator.EndSelection();
			this.document.Modifier.OpletQueueBeginActionNoMerge(Res.Strings.Action.Text.Glyphs.Insert);

			this.MetaNavigator.Insert(code);

			this.document.Modifier.OpletQueueValidateAction();
			this.textFlow.NotifyAreaFlow();
			return true;
		}

		public override bool EditInsertText(Text.Properties.BreakProperty brk)
		{
			//	Insère un texte dans le pavé en édition.
			this.MetaNavigator.EndSelection();
			this.document.Modifier.OpletQueueBeginActionNoMerge(Res.Strings.Action.Text.Glyphs.Insert);

			this.MetaNavigator.Insert(brk);

			this.document.Modifier.OpletQueueValidateAction();
			this.textFlow.NotifyAreaFlow();
			return true;
		}

		public override bool EditInsertGlyph(int code, int glyph, string fontFace, string fontStyle)
		{
			//	Insère un glyphe dans le pavé en édition.
			this.MetaNavigator.EndSelection();
			this.document.Modifier.OpletQueueBeginActionNoMerge(Res.Strings.Action.Text.Glyphs.Alternate);

			if ( fontFace == "" )
			{
				string text = code.ToString();
				this.MetaNavigator.Insert(text);
			}
			else
			{
				OpenType.Font font = TextContext.GetFont(fontFace, fontStyle);
				Text.Properties.OpenTypeProperty otp = new Text.Properties.OpenTypeProperty(fontFace, fontStyle, glyph);
				this.MetaNavigator.Insert((Text.Unicode.Code)code, otp);
				this.MetaNavigator.SelectInsertedCharacter();
			}

			this.document.Modifier.OpletQueueValidateAction();
			this.textFlow.NotifyAreaFlow();
			return true;
		}

		public override bool EditGetSelectedGlyph(out int code, out int glyph, out OpenType.Font font)
		{
			//	Retourne le glyphe du caractère sélectionné.
			code = 0;
			glyph = 0;
			font = null;

			int n = this.textFlow.TextNavigator.SelectionCount;
			if ( n != 1 )  return false;

			ulong[] sel = this.textFlow.TextNavigator.GetRawSelection(0);
			if ( sel.Length != 1 )  return false;

			code = Unicode.Bits.GetCode(sel[0]);

			Text.Properties.OpenTypeProperty otp;
			this.document.TextContext.GetOpenType(sel[0], out otp);

			string face = this.document.TextWrapper.Defined.FontFace;
			if ( face == null )
			{
				face = this.document.TextWrapper.Active.FontFace;
				if ( face == null )
				{
					face = "";
				}
			}

			string style = this.document.TextWrapper.Defined.FontStyle;
			if ( style == null )
			{
				style = this.document.TextWrapper.Active.FontStyle;
				if ( style == null )
				{
					style = "";
				}
			}

			font = TextContext.GetFont(face, style);

			if ( otp == null )
			{
				glyph = font.GetGlyphIndex(code);
			}
			else
			{
				glyph = otp.GlyphIndex;
			}

			return true;
		}


		protected override void UpdatePageNumber()
		{
			//	Met à jour le TextFrame en fonction du numéro de la page.
			this.textFrame.PageNumber = this.pageNumber+1;
		}


		#region TextFormat
		public override System.Collections.ArrayList CreateTextPanels(string filter)
		{
			//	Crée tous les panneaux pour l'édition.
			System.Collections.ArrayList list = new System.Collections.ArrayList();

			if ( TextPanels.Abstract.IsFilterShow("Justif", filter) )
			{
				TextPanels.Justif justif = new TextPanels.Justif(this.document);
				list.Add(justif);
			}

			if ( TextPanels.Abstract.IsFilterShow("Leading", filter) )
			{
				TextPanels.Leading leading = new TextPanels.Leading(this.document);
				list.Add(leading);
			}

			if ( TextPanels.Abstract.IsFilterShow("Margins", filter) )
			{
				TextPanels.Margins margins = new TextPanels.Margins(this.document);
				list.Add(margins);
			}

			if ( TextPanels.Abstract.IsFilterShow("Spaces", filter) )
			{
				TextPanels.Spaces spaces = new TextPanels.Spaces(this.document);
				list.Add(spaces);
			}

			if ( TextPanels.Abstract.IsFilterShow("Keep", filter) )
			{
				TextPanels.Keep keep = new TextPanels.Keep(this.document);
				list.Add(keep);
			}

			if ( TextPanels.Abstract.IsFilterShow("Font", filter) )
			{
				TextPanels.Font font = new TextPanels.Font(this.document);
				list.Add(font);
			}

			if ( TextPanels.Abstract.IsFilterShow("Xline", filter) )
			{
				TextPanels.Xline xline = new TextPanels.Xline(this.document);
				list.Add(xline);
			}

			if ( TextPanels.Abstract.IsFilterShow("Xscript", filter) )
			{
				TextPanels.Xscript xscript = new TextPanels.Xscript(this.document);
				list.Add(xscript);
			}

			if ( TextPanels.Abstract.IsFilterShow("Box", filter) )
			{
				TextPanels.Box box = new TextPanels.Box(this.document);
				list.Add(box);
			}

			if ( TextPanels.Abstract.IsFilterShow("Language", filter) )
			{
				TextPanels.Language language = new TextPanels.Language(this.document);
				list.Add(language);
			}

			return list;
		}

		protected Text.Property[] GetTextProperties(bool accumulated)
		{
			//	Donne la liste des propriétés.
			if ( accumulated )
			{
				return this.textFlow.TextNavigator.AccumulatedTextProperties;
			}
			else
			{
				return this.textFlow.TextNavigator.TextProperties;
			}
		}

		protected bool IsExistingStyle(string name)
		{
			//	Indique l'existance d'un style.
			Text.TextStyle[] styles = this.textFlow.TextNavigator.TextStyles;
			foreach ( Text.TextStyle style in styles )
			{
				if ( style.Name == name )  return true;
			}
			return false;
		}

		protected override void EditWrappersAttach()
		{
			//	Attache l'objet au différents wrappers.
			this.document.Wrappers.WrappersAttach(this.textFlow);
		}

		protected override void UpdateTextRulers()
		{
			//	Met à jour les règles pour le texte en édition.
			if ( this.edited )
			{
				this.textFlow.UpdateTextRulers();
			}
		}
		#endregion

		public override Drawing.Rectangle EditCursorBox
		{
			//	Donne la zone contenant le curseur d'édition.
			get
			{
				return this.cursorBox;
			}
		}

		public override Drawing.Rectangle EditSelectBox
		{
			//	Donne la zone contenant le texte sélectionné.
			get
			{
				return this.selectBox;
			}
		}

		public override void EditMouseDownMessage(Point pos)
		{
			//	Gestion d'un événement pendant l'édition.
			//?int rank = this.DetectTextCurveRank(pos);
			//?pos = this.RankToLinearPos(rank);
			//?if ( pos == Point.Empty )  return;
			//?this.textNavigator.MouseDownMessage(pos);
		}

		
		protected Drawing.Rectangle FullBoundingBox()
		{
			//	Calcule la bbox qui englobe l'objet et les poignées secondaires.
			Drawing.Rectangle bbox = Drawing.Rectangle.Empty;
			int total = this.TotalHandle;
			for ( int i=0 ; i<total ; i++ )
			{
				bbox.MergeWith(this.Handle(i).Position);
			}
			return bbox;
		}

		public override Shape[] ShapesBuild(IPaintPort port, DrawingContext drawingContext, bool simplify)
		{
			//	Constuit les formes de l'objet.
			Path pathLine = this.PathBuild();
			Path pathHilite = null;
			Path pathSupport = null;
			Path pathBbox = null;

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

			//	Rectangles des caractères pour bbox et détection.
			shapes[i] = new Shape();
			shapes[i].Path = pathBbox;
			shapes[i].Type = Type.Surface;
			shapes[i].Aspect = Aspect.InvisibleBox;
			i ++;

			return shapes;
		}

		protected Path PathBuild()
		{
			//	Crée le chemin de l'objet.
			Path path = new Path();

			int total = this.handles.Count;
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

		protected double GetLength()
		{
			//	Retourne la longueur totale d'une courbe multiple.
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
						length += Point.Distance(pos, next);
						pos = next;
					}
				}
				i += 3;  // courbe suivante
			}
			while ( i < this.handles.Count-3 );

			return length;
		}


		protected void UpdateTextFrame()
		{
			//	Met à jour le TextFrame en fonction des dimensions du pavé.
			double width  = this.GetLength();
			double height = 1000000;
			
			if ( this.textFrame.Width   != width  ||
				 this.textFrame.Height  != height )
			{
				this.textFrame.OriginX = 0;
				this.textFrame.OriginY = 0;
				this.textFrame.Width   = width;
				this.textFrame.Height  = height;
				
				this.textFlow.TextStory.NotifyTextChanged();
			}
		}
		
		public override bool IsInTextFrame(Drawing.Point pos, out Drawing.Point ppos)
		{
			//	Détermine si un point se trouve dans le texte frame.
			if ( this.transform == null )
			{
				ppos = Drawing.Point.Empty;
				return false;
			}
			
			ppos = this.transform.TransformInverse(pos);
			
			if ( ppos.X < 0 || ppos.Y < 0 || ppos.X > this.textFrame.Width || ppos.Y > this.textFrame.Height )
			{
				return false;
			}
			else
			{
				return true;
			}
		}
		
		public override void DrawText(IPaintPort port, DrawingContext drawingContext)
		{
			//	Dessine le texte du pavé.
			this.DrawText(port, drawingContext, InternalOperation.Painting);
		}

		protected void DrawText(IPaintPort port, DrawingContext drawingContext, InternalOperation op)
		{
			//	Effectue une opération quelconque sur le texte du pavé.
			this.internalOperation = op;

			if ( this.internalOperation == InternalOperation.Painting )
			{
				this.cursorBox = Drawing.Rectangle.Empty;
				this.selectBox = Drawing.Rectangle.Empty;
			}
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
		
		public void RenderTab(Text.Layout.Context layout, string tag, double tabOrigin, double tabStop, ulong tabCode, bool isTabDefined)
		{
			if ( this.graphics == null )  return;
			if ( this.drawingContext == null )  return;
			if ( this.drawingContext.TextShowControlCharacters == false )  return;
			if ( this.textFlow.HasActiveTextBox == false )  return;

			double x1 = tabOrigin;
			double x2 = tabStop;
			double y  = layout.LineBaseY + layout.LineAscender*0.3;
			double a  = System.Math.Min(layout.LineAscender*0.3, (x2-x1)*0.5);

			Point p1 = new Point(x1, y);
			Point p2 = new Point(x2, y);
			graphics.Align(ref p1);
			graphics.Align(ref p2);
			double adjust = 0.5/this.drawingContext.ScaleX;
			p1.X += adjust;  p1.Y += adjust;
			p2.X -= adjust;  p2.Y += adjust;
			if ( p1.X >= p2.X )  return;

			Point p2a = new Point(p2.X-a, p2.Y-a*0.75);
			Point p2b = new Point(p2.X-a, p2.Y+a*0.75);

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
				int  offset = 0;
				bool isInSelection = false;
				double selX = 0;

				System.Collections.ArrayList selRectList = null;

				double x1 = 0;
				double x2 = 0;

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

					x1 = x[offset+0];
					x2 = x[offset+numGlyphs];

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

					for ( int i=0 ; i<numChars ; i++ )
					{
						if ( (tArray[i] & this.markerSelected) != 0 )
						{
							//	Le caractère considéré est sélectionné.
							if ( isInSelection == false )
							{
								//	C'est le premier caractère d'une tranche. Il faut mémoriser son début :
								double xx = x1 + ((x2 - x1) * i) / numChars;
								isInSelection = true;
								selX = xx;
							}
						}
						else
						{
							if ( isInSelection )
							{
								//	Nous avons quitté une tranche sélectionnée. Il faut mémoriser sa fin :
								double xx = x1 + ((x2 - x1) * i) / numChars;
								isInSelection = false;

								if ( xx > selX )
								{
									this.MarkSel(layout, ref selRectList, xx, selX);
								}
							}
						}
					}

					offset += numGlyphs;
				}

				if ( isInSelection )
				{
					//	Nous avons quitté une tranche sélectionnée. Il faut mémoriser sa fin :
					double xx = x2;
					isInSelection = false;

					if ( xx > selX )
					{
						this.MarkSel(layout, ref selRectList, xx, selX);
					}
				}

				if ( this.textFlow.HasActiveTextBox && selRectList != null && this.graphics != null )
				{
					//	Dessine les rectangles correspondant à la sélection.
					foreach ( Drawing.Rectangle rect in selRectList )
					{
						this.graphics.AddFilledRectangle(rect);

						Point c1 = this.transform.TransformDirect(rect.BottomLeft);
						Point c2 = this.transform.TransformDirect(rect.TopRight);
						this.selectBox.MergeWith(c1);
						this.selectBox.MergeWith(c2);
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
		}

		protected void MarkSel(Text.Layout.Context layout, ref System.Collections.ArrayList selRectList, double x, double selX)
		{
			//	Marque une tranche sélectionnée.
			if ( this.graphics == null )  return;

			double dx = x - selX;
			double dy = layout.LineY2 - layout.LineY1;
			Drawing.Rectangle rect = new Drawing.Rectangle(selX, layout.LineY1, dx, dy);
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
						if ( sy == null )
						{
							this.graphics.Rasterizer.AddGlyphs(drawingFont, size, glyphs, x, y, sx);
						}
						else
						{
							for ( int i=0 ; i<glyphs.Length ; i++ )
							{
								if ( glyphs[i] < 0xffff )
								{
									this.graphics.Rasterizer.AddGlyph(drawingFont, glyphs[i], x[i], y[i], size, sx == null ? 1.0 : sx[i], sy == null ? 1.0 : sy[i]);
								}
							}
						}

						if ( this.textFlow.HasActiveTextBox && isSpace && insecs != null &&
							this.drawingContext != null && this.drawingContext.TextShowControlCharacters )
						{
							for ( int i=0 ; i<glyphs.Length ; i++ )
							{
								double width = font.GetGlyphWidth(glyphs[i], size);
								double oy = font.GetAscender(size)*0.3;

								if ( insecs[i] == SpaceType.BreakSpace )  // espace sécable ?
								{
									this.graphics.AddFilledCircle(x[i]+width/2, y[i]+oy, size*0.05);
								}

								if ( insecs[i] == SpaceType.NoBreakSpace )  // espace insécable ?
								{
									this.graphics.AddCircle(x[i]+width/2, y[i]+oy, size*0.08);
								}

								if ( insecs[i] == SpaceType.NewFrame ||
									insecs[i] == SpaceType.NewPage  )  // saut ?
								{
									Point p1 = new Point(x[i],                 y[i]+oy);
									Point p2 = new Point(this.textFrame.Width, y[i]+oy);
									Path path = Path.FromLine(p1, p2);

									double w    = (insecs[i] == SpaceType.NewFrame) ? 0.8 : 0.5;
									double dash = (insecs[i] == SpaceType.NewFrame) ? 0.0 : 8.0;
									double gap  = (insecs[i] == SpaceType.NewFrame) ? 3.0 : 2.0;
									Drawer.DrawPathDash(this.graphics, this.drawingContext, path, w, dash, gap, color.Basic);
								}
							}
						}
					}
			
					this.graphics.RenderSolid(color.Basic);
				}
				else if ( this.port is Printing.PrintPort )  // impression ?
				{
					Printing.PrintPort printPort = port as Printing.PrintPort;
					Drawing.Font drawingFont = Drawing.Font.GetFont(font);
					printPort.RichColor = color;
					printPort.PaintGlyphs(drawingFont, size, glyphs, x, y, sx, sy);
				}
				else if ( this.port is PDF.Port )  // exportation PDF ?
				{
					PDF.Port pdfPort = port as PDF.Port;
					Drawing.Font drawingFont = Drawing.Font.GetFont(font);
					pdfPort.RichColor = color;
					pdfPort.PaintGlyphs(drawingFont, size, glyphs, x, y, sx, sy);
				}
			}

			if ( this.internalOperation == InternalOperation.GetPath )
			{
				Drawing.Font drawingFont = Drawing.Font.GetFont(font);
				this.graphics.PaintGlyphs(drawingFont, size, glyphs, x, y, sx, sy);
			}

			if ( this.internalOperation == InternalOperation.RealBoundingBox )
			{
				Drawing.Font drawingFont = Drawing.Font.GetFont(font);
				if ( drawingFont != null )
				{
					for ( int i=0 ; i<glyphs.Length ; i++ )
					{
						if ( glyphs[i] < 0xffff )
						{
							Drawing.Rectangle bounds = drawingFont.GetGlyphBounds(glyphs[i], size);

							if ( sx != null )  bounds.Scale(sx[i], 1.0);
							if ( sy != null )  bounds.Scale(1.0, sy[i]);

							bounds.Offset(x[i], y[i]);

							this.mergingBoundingBox.MergeWith(this.transform.TransformDirect(bounds.BottomLeft));
							this.mergingBoundingBox.MergeWith(this.transform.TransformDirect(bounds.BottomRight));
							this.mergingBoundingBox.MergeWith(this.transform.TransformDirect(bounds.TopLeft));
							this.mergingBoundingBox.MergeWith(this.transform.TransformDirect(bounds.TopRight));
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

			Path path = Path.FromRectangle(starting.X, y-xline.Thickness/2, ending.X-starting.X, xline.Thickness);

			string color = xline.DrawStyle;
			if ( color == null )  // couleur par défaut (comme le texte) ?
			{
				color = starting.TextColor.TextColor;
			}
			this.port.RichColor = RichColor.Parse(color);

			this.port.PaintSurface(path);
		}

		protected static bool XlineContains(System.Collections.ArrayList process, Text.Properties.AbstractXlineProperty xline, Text.Properties.FontColorProperty color)
		{
			//	Cherche si une propriété Xline est déjà dans une liste.
			foreach ( XlineInfo existing in process )
			{
				if ( Text.Property.CompareEqualContents(existing.Xline, xline) &&
					Text.Property.CompareEqualContents(existing.Color, color) )
				{
					return true;
				}
			}
			return false;
		}
		
		private class XlineInfo
		{
			public XlineInfo(Text.Properties.AbstractXlineProperty xline, Text.Properties.FontColorProperty color)
			{
				this.xline = xline;
				this.color = color;
			}
			
			
			public Text.Properties.AbstractXlineProperty Xline
			{
				get
				{
					return this.xline;
				}
			}
			
			public Text.Properties.FontColorProperty Color
			{
				get
				{
					return this.color;
				}
			}
			
			
			Text.Properties.AbstractXlineProperty	xline;
			Text.Properties.FontColorProperty		color;
		}
		#endregion


		#region Serialization
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise l'objet.
			base.GetObjectData(info, context);
			info.AddValue("TextFlow", this.textFlow);
		}

		protected TextLine2(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui désérialise l'objet.
			this.textFlow = (TextFlow) info.GetValue("TextFlow", typeof(TextFlow));
		}

		public override void ReadCheckWarnings(Font.FaceInfo[] fonts, System.Collections.ArrayList warnings)
		{
			//	Vérifie si tous les fichiers existent.
			//?Common.Document.Objects.Abstract.ReadCheckFonts(fonts, warnings, this.textLayout);
		}
		
		public override void ReadFinalize()
		{
			base.ReadFinalize ();
			
			this.InitialiseInternals();
		}
		
		public override void ReadFinalizeFlowReady(TextFlow flow)
		{
			System.Diagnostics.Debug.Assert(this.textFlow == flow);
			
			this.UpdateTextFrame();
		}
		#endregion

		
		public void UpdateGeometry()
		{
			//	Met à jour après un changement de géométrie de l'objet.
			this.UpdateTextFrame();
			this.UpdateTextLayout();
		}

		public override void UpdateTextLayout()
		{
			//	Met à jour le texte suite à une modification du conteneur.
			if ( this.edited )
			{
				this.textFlow.UpdateTextLayout();
			}
		}

		protected override void SetEdited(bool state)
		{
			//	Modifie le mode d'édition. Il faut obligatoirement utiliser cet appel
			//	pour modifier this.edited !
			if ( this.edited == state )  return;

			this.edited = state;

			if ( this.document.HRuler != null )
			{
				this.document.HRuler.Edited = this.edited;
				this.document.VRuler.Edited = this.edited;
			}

			if ( this.edited )
			{
				if ( this.document.HRuler != null )
				{
					this.document.HRuler.EditObject = this;
					this.document.VRuler.EditObject = this;
					this.document.HRuler.WrappersAttach();
					this.document.VRuler.WrappersAttach();
				}
				this.EditWrappersAttach();  // attache l'objet aux différents wrappers
				
				this.textFlow.ActiveTextBox = this;
			}
			else
			{
				if ( this.document.HRuler != null )
				{
					this.document.HRuler.EditObject = null;
					this.document.VRuler.EditObject = null;
					this.document.HRuler.WrappersDetach();
					this.document.VRuler.WrappersDetach();
				}
				this.document.Wrappers.WrappersDetach();
				
				if ( this.textFlow.ActiveTextBox == this )
				{
					this.textFlow.ActiveTextBox = null;
				}
			}

			this.UpdateTextRulers();

			//	Redessine tout, à cause des "poignées" du flux qui peuvent apparaître
			//	ou disparaître.
			this.document.Notifier.NotifyArea(this.document.Modifier.ActiveViewer);
		}


		public override Path GetMagnetPath()
		{
			//	Retourne le chemin géométrique de l'objet pour les constructions
			//	magnétiques.
			Path path = this.PathBuild();
			return path;
		}


		#region OpletTextFlow
		protected void InsertOpletTextFlow()
		{
			//	Ajoute un oplet pour mémoriser le flux.
			if ( this.textFlow == null )  return;  // création de l'objet ?
			if ( !this.document.Modifier.OpletQueueEnable )  return;
			OpletTextFlow oplet = new OpletTextFlow(this);
			this.document.Modifier.OpletQueue.Insert(oplet);
		}

		//	Mémorise le flux de l'objet.
		protected class OpletTextFlow : AbstractOplet
		{
			public OpletTextFlow(Objects.TextLine2 host)
			{
				System.Diagnostics.Debug.Assert(host.textFlow != null);
				this.host = host;
				this.textFlow = host.textFlow;
			}

			protected void Swap()
			{
				System.Diagnostics.Debug.Assert(host.textFlow != null);
				System.Diagnostics.Debug.Assert(this.textFlow != null);

				TextFlow temp = host.textFlow;
				host.textFlow = this.textFlow;
				this.textFlow = temp;

				host.UpdateTextLayout();
			}

			public override IOplet Undo()
			{
				this.Swap();
				return this;
			}

			public override IOplet Redo()
			{
				this.Swap();
				return this;
			}

			protected Objects.TextLine2				host;
			protected TextFlow						textFlow;
		}
		#endregion

		
		protected bool							isActive;
		protected ulong							markerSelected;
		protected TextFlow						textFlow;
		protected Text.SimpleTextFrame			textFrame;
		protected IPaintPort					port;
		protected Graphics						graphics;
		protected DrawingContext				drawingContext;
		protected Transform						transform;
		protected Drawing.Rectangle				redrawArea;
		protected Drawing.Rectangle				cursorBox;
		protected Drawing.Rectangle				selectBox;
		protected InternalOperation				internalOperation = InternalOperation.Painting;
		protected System.Collections.Hashtable	charactersTable = null;
		protected Drawing.Rectangle				mergingBoundingBox;

		protected int							advanceRank;
		protected int							advanceIndex;
		protected double						advanceBzt;
		protected double						advanceWidth;
		protected Point							advanceP1;
		protected Point							advanceP2;
		protected Point							advanceLastTop;
		protected Point							advanceLastBottom;
		protected bool							advanceCheckEnd;
		protected double						advanceFactor;
		protected double						advanceMaxAscender;
		protected Point							initialPos;

		protected static readonly double		step = 0.01;
	}
}

using System.Collections.Generic;
using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using System.Runtime.Serialization;

namespace Epsitec.Common.Document.Objects
{
	/// <summary>
	/// La classe TextLine est la classe de l'objet graphique "texte simple".
	/// </summary>
	[System.Serializable()]
	public class TextLine : Objects.Abstract
	{
		public TextLine(Document document, Objects.Abstract model) : base(document, model)
		{
			if ( this.document == null )  return;  // objet factice ?
			this.CreateProperties(model, false);
			this.Initialize();
		}

		protected void Initialize()
		{
			this.textLayout = new TextLayout();
			this.textNavigator = new TextNavigator(null, this.textLayout);
			this.textNavigator.StyleChanged += this.HandleTextChanged;
			this.textNavigator.TextInserted += this.HandleTextChanged;
			this.textNavigator.TextDeleted  += this.HandleTextChanged;
			this.textLayout.BreakMode = TextBreakMode.Hyphenate;
			this.textLayout.LayoutSize = new Size(1000000, 1000000);
			this.textLayout.Alignment = ContentAlignment.BottomLeft;
			if ( this.document.Modifier != null )
			{
				this.textNavigator.OpletQueue = this.document.Modifier.OpletQueue;
			}
			this.cursorBox = Drawing.Rectangle.Empty;
			this.selectBox = Drawing.Rectangle.Empty;
		}

		protected override bool ExistingProperty(Properties.Type type)
		{
			if ( type == Properties.Type.Name )  return true;
			if ( type == Properties.Type.TextLine )  return true;
			if ( type == Properties.Type.TextFont )  return true;
			return false;
		}

		protected override Objects.Abstract CreateNewObject(Document document, Objects.Abstract model)
		{
			return new TextLine(document, model);
		}

		public override void Dispose()
		{
			if ( this.document != null )
			{
				this.textNavigator.StyleChanged -= this.HandleTextChanged;
				this.textNavigator.TextInserted -= this.HandleTextChanged;
				this.textNavigator.TextDeleted  -= this.HandleTextChanged;
			}

			base.Dispose();
		}


		public override string IconUri
		{
			//	Nom de l'icône.
			get { return Misc.Icon("ObjectTextLine"); }
		}


		public string Content
		{
			get
			{
				return this.textLayout.Text;
			}

			set
			{
				this.textLayout.Text = value;
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

			if ( this.DetectTextCurve(pos) )  return DetectEditType.Body;
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
			if ( rank >= this.handles.Count )  // poignée d'une propriété ?
			{
				base.MoveHandleProcess(rank, pos, drawingContext);
				return;
			}

			this.document.Notifier.NotifyArea(this.BoundingBox);
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

		public override void FillFontFaceList(List<OpenType.FontName> list)
		{
			//	Ajoute toutes les fontes utilisées par l'objet dans une liste.
			this.textLayout.FillFontFaceList(list);
		}

		public override void FillOneCharList(IPaintPort port, DrawingContext drawingContext, System.Collections.Hashtable table)
		{
			//	Ajoute tous les caractères utilisés par l'objet dans une table.
			this.textLayout.DefaultFont      = this.PropertyTextFont.GetFont();
			this.textLayout.DefaultFontSize  = this.PropertyTextFont.FontSize;
			this.textLayout.DefaultRichColor = this.PropertyTextFont.FontColor;
			TextLayout.OneCharStructure[] fix = this.textLayout.ComputeStructure();

			foreach ( TextLayout.OneCharStructure oneChar in fix )
			{
				PDF.CharacterList cl = new PDF.CharacterList(oneChar);
				if ( !table.ContainsKey(cl) )
				{
					table.Add(cl, null);
				}
			}
		}

		public override bool IsEditable
		{
			//	Indique si un objet est éditable.
			get { return false; }
		}


		public override void CloneObject(Objects.Abstract src)
		{
			//	Reprend toutes les caractéristiques d'un objet.
			base.CloneObject(src);

			TextLine obj = src as TextLine;
			this.textLayout.Text = obj.textLayout.Text;
			obj.textNavigator.Context.CopyTo(this.textNavigator.Context);
		}

		
		#region TextFormat
		public bool EditBold()
		{
			//	Met en gras pendant l'édition.
			this.textNavigator.SelectionBold = !this.textNavigator.SelectionBold;
			return true;
		}

		public bool EditItalic()
		{
			//	Met en italique pendant l'édition.
			this.textNavigator.SelectionItalic = !this.textNavigator.SelectionItalic;
			return true;
		}

		public bool EditUnderline()
		{
			//	Souligne pendant l'édition.
			this.textNavigator.SelectionUnderline = !this.textNavigator.SelectionUnderline;
			return true;
		}
		#endregion

		
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
			Path pathBbox = this.HiliteTextCurve();

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
					int total = (int)(1.0/TextLine.step);
					for ( int rank=1 ; rank<=total ; rank ++ )
					{
						double t = TextLine.step*rank;
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

		protected bool Advance(double width, bool checkEnd, ref int i, ref double t, ref Point pos)
		{
			//	Avance le long d'une courbe multiple.
			//	La courbe est fragmentée en 100 morceaux (TextLine.step = 0.01)
			//	considérés chacuns comme des lignes droites.
			//	Retourne false lorsqu'on arrive à la fin.
			//	Le mode checkEnd = true ne teste pas l'arrivée à la fin, ce qui est
			//	utile en mode JustifHorizontal.Right pour être certain de caser le
			//	dernier caractère. Sans cela, des erreurs d'arrondi font qu'il est
			//	parfois considéré comme hors du tracé.
			if ( i >= this.handles.Count-3 )  return false;

			while ( true )
			{
				Point p1 = this.Handle(i+1).Position;
				Point s1 = this.Handle(i+2).Position;
				Point s2 = this.Handle(i+3).Position;
				Point p2 = this.Handle(i+4).Position;

				if ( this.Handle(i+2).Type == HandleType.Hide )  // droite ?
				{
					double d = Point.Distance(p1,p2);
					double t2 = (t*d+width)/d;
					if ( t2 <= 1.0 )
					{
						t = t2;
						pos = Point.Scale(p1,p2, t);
						return true;
					}
					width -= (1.0-t)*d;
				}
				else	// courbe ?
				{
					pos = Point.FromBezier(p1,s1,s2,p2, t);
					double t1 = t;
					double t2 = t;
					double l1 = 0.0;
					double l2 = 0.0;
					Point next1, next2;
					next1 = pos;
					while ( true )
					{
						t2 = System.Math.Min(t2+TextLine.step, 1.0);
						next2 = Point.FromBezier(p1,s1,s2,p2, t2);  // segment suivant
						l2 += Point.Distance(next1, next2);
						if ( l2 >= width )  // a-t-on trop avancé ?
						{
							t = t1+(t2-t1)*(width-l1)/(l2-l1);  // approximation linéaire
							pos = Point.Move(next1, next2, width-l1);
							return true;
						}
						if ( t2 >= 1.0 )  break;  // fin de cette portion de courbe ?
						t1 = t2;
						l1 = l2;
						next1 = next2;
					}
					width -= l2;
				}

				i += 3;  // portion de courbe suivante
				if ( i >= this.handles.Count-3 )  // dernière portion dépassée ?
				{
					if ( checkEnd )  return false;
					pos = p2;
					return true;
				}
				t = 0.0;
			}
		}

		protected double AdvanceString(Font font, string text)
		{
			//	Retourne la largeur d'un caractère.
			System.Diagnostics.Debug.Assert(text.Length == 1);
			if ( text[0] == TextLayout.CodeEndOfText )
			{
				return (this.advanceCharArray.Length <= 1) ? 1.0 : 0.000001;
			}
			else
			{
				return font.GetTextAdvance(text);
			}
		}

		protected Drawing.Rectangle AdvanceBounds(Font font, string text)
		{
			//	Retourne la bbox d'un caractère.
			System.Diagnostics.Debug.Assert(text.Length == 1);
			if ( text[0] == TextLayout.CodeEndOfText )
			{
				return new Drawing.Rectangle(0, font.Descender, 0.000001, font.Ascender-font.Descender);
			}
			else
			{
				return font.GetGlyphBounds(font.GetGlyphIndex(text[0]));
			}
		}

		protected bool AdvanceInit()
		{
			//	Initialise l'avance le long des caractères du texte.
			Properties.TextLine justif = this.PropertyTextLine;

			this.textLayout.DefaultFont      = this.PropertyTextFont.GetFont();
			this.textLayout.DefaultFontSize  = this.PropertyTextFont.FontSize;
			this.textLayout.DefaultRichColor = this.PropertyTextFont.FontColor;
			this.advanceCharArray = this.textLayout.ComputeStructure();

			double width = 0;
			this.advanceMaxAscender = 0;
			for ( int i=0 ; i<this.advanceCharArray.Length ; i++ )
			{
				string s = new string(this.advanceCharArray[i].Character, 1);
				width += this.AdvanceString(this.advanceCharArray[i].Font, s)*this.advanceCharArray[i].FontSize;
				width += justif.Add*this.advanceCharArray[i].FontSize;

				this.advanceMaxAscender = System.Math.Max(this.advanceMaxAscender, this.advanceCharArray[i].Font.Ascender*this.advanceCharArray[i].FontSize);
			}

			this.advanceRank = 0;
			this.advanceIndex = 0;
			this.advanceBzt = 0.0;
			this.advanceP1 = this.Handle(1).Position;
			this.advanceP2 = this.advanceP1;
			this.advanceLastTop    = Point.Zero;
			this.advanceLastBottom = Point.Zero;
			this.advanceCheckEnd = true;
			this.advanceFactor = 1.0;

			if ( justif.Horizontal == Properties.JustifHorizontal.Center )
			{
				double length = this.GetLength();
				width = System.Math.Max((length-width)/2, 0.0);
				this.Advance(width, true, ref this.advanceIndex, ref this.advanceBzt, ref this.advanceP1);
				this.advanceCheckEnd = (width == 0.0);
			}
			if ( justif.Horizontal == Properties.JustifHorizontal.Right )
			{
				double length = this.GetLength();
				width = System.Math.Max(length-width, 0.0);
				this.Advance(width, true, ref this.advanceIndex, ref this.advanceBzt, ref this.advanceP1);
				this.advanceCheckEnd = (width == 0.0);
			}
			if ( justif.Horizontal == Properties.JustifHorizontal.Stretch )
			{
				double length = this.GetLength();
				this.advanceFactor = length/width;
				this.advanceCheckEnd = false;
			}

			return true;
		}

		protected bool AdvanceNext(out string character,
								   out Font font,
								   out double fontSize,
								   out RichColor fontColor,
								   out Point pos,
								   out Point ptl,
								   out Point pbl,
								   out Point ptr,
								   out Point pbr,
								   out double angle)
		{
			//	Avance sur le prochain caractère du texte.
			Properties.TextLine justif = this.PropertyTextLine;

			character = "";
			font = null;
			fontSize = 0;
			fontColor = RichColor.Empty;
			pos = new Point();
			ptl = new Point();
			pbl = new Point();
			ptr = new Point();
			pbr = new Point();
			angle = 0;

			int i = this.advanceRank++;
			if ( i >= this.advanceCharArray.Length )  return false;

			character = new string(this.advanceCharArray[i].Character, 1);
			font = this.advanceCharArray[i].Font;
			fontSize = this.advanceCharArray[i].FontSize * this.advanceFactor;
			fontColor = this.advanceCharArray[i].FontColor;

			this.advanceWidth = this.AdvanceString(font, character)*fontSize;
			this.advanceWidth += justif.Add*fontSize;
			if ( !this.Advance(this.advanceWidth, this.advanceCheckEnd, ref this.advanceIndex, ref this.advanceBzt, ref this.advanceP2) )
			{
				return false;
			}

			pos = this.advanceP1;
			if ( justif.Offset > 0.0 )
			{
				pos = Point.Move(this.advanceP1, this.advanceP2, this.advanceMaxAscender*justif.Offset);
				pos = Transform.RotatePointDeg(this.advanceP1, -90, pos);
			}

			angle = Point.ComputeAngleDeg(this.advanceP1, this.advanceP2);

			Drawing.Rectangle gb = this.AdvanceBounds(font, character);
			gb.Top    = font.Ascender;
			gb.Bottom = font.Descender;
			gb.Scale(fontSize);
			gb.Offset(pos);
			pbl = Transform.RotatePointDeg(pos, angle, gb.BottomLeft);
			pbr = Transform.RotatePointDeg(pos, angle, gb.BottomRight);
			ptl = Transform.RotatePointDeg(pos, angle, gb.TopLeft);
			ptr = Transform.RotatePointDeg(pos, angle, gb.TopRight);

			if ( !this.advanceLastTop.IsZero )
			{
				ptl = Point.Projection(this.advanceLastTop, this.advanceLastBottom, ptl);
				pbl = Point.Projection(this.advanceLastTop, this.advanceLastBottom, pbl);
			}

			this.advanceP1 = this.advanceP2;
			this.advanceLastTop    = ptr;
			this.advanceLastBottom = pbr;

			return true;
		}

		protected Point RankToLinearPos(int rank)
		{
			//	Conversion d'un rank dans le texte en une position linéaire, c'est-à-dire
			//	une position qui suppose que le texte est droit.
			Point lp = Point.Zero;
			if ( rank == -1 || !this.AdvanceInit() )  return lp;

			lp.X = 0.00001;  // pour feinter Point.IsZero !
			lp.Y = 0.00001;
			string	character;
			Font	font;
			double	fontSize;
			RichColor fontColor;
			Point	pos, ptl, pbl, ptr, pbr;
			double	angle;
			while ( this.AdvanceNext(out character, out font, out fontSize, out fontColor, out pos, out ptl, out pbl, out ptr, out pbr, out angle) )
			{
				if ( rank == this.advanceRank-1 )  return lp;
				lp.X += this.advanceWidth / this.advanceFactor;
			}
			return lp;
		}

		protected bool DetectTextCurve(Point mouse)
		{
			//	Détecte si la souris est sur un caractère du texte le long de la courbe multiple.
			if ( !this.AdvanceInit() )  return false;

			string	character;
			Font	font;
			double	fontSize;
			RichColor fontColor;
			Point	pos, ptl, pbl, ptr, pbr;
			double	angle;
			while ( this.AdvanceNext(out character, out font, out fontSize, out fontColor, out pos, out ptl, out pbl, out ptr, out pbr, out angle) )
			{
				InsideSurface inside = new InsideSurface(mouse, 4);
				inside.AddLine(pbl, pbr);
				inside.AddLine(pbr, ptr);
				inside.AddLine(ptr, ptl);
				inside.AddLine(ptl, pbl);
				if ( inside.IsInside() )  return true;
			}
			return false;
		}

		protected int DetectTextCurveRank(Point mouse)
		{
			//	Détecte sur quel caractère est la souris le long de la courbe multiple.
			if ( !this.AdvanceInit() )  return -1;

			string	character;
			Font	font;
			double	fontSize;
			RichColor fontColor;
			Point	pos, ptl, pbl, ptr, pbr;
			double	angle;
			while ( this.AdvanceNext(out character, out font, out fontSize, out fontColor, out pos, out ptl, out pbl, out ptr, out pbr, out angle) )
			{
				Point ptm = (ptl+ptr)/2;
				Point pbm = (pbl+pbr)/2;

				InsideSurface inside = new InsideSurface(mouse, 4);
				inside.AddLine(pbl, pbm);
				inside.AddLine(pbm, ptm);
				inside.AddLine(ptm, ptl);
				inside.AddLine(ptl, pbl);
				if ( inside.IsInside() )  return this.advanceRank-1;  // dans la moitié gauche ?

				inside = new InsideSurface(mouse, 4);
				inside.AddLine(pbm, pbr);
				inside.AddLine(pbr, ptr);
				inside.AddLine(ptr, ptm);
				inside.AddLine(ptm, pbm);
				if ( inside.IsInside() )  return this.advanceRank;  // dans la moitié droite ?
			}
			return -1;
		}

		protected void BboxTextCurve(ref Drawing.Rectangle bbox)
		{
			//	Calcule la bbox du texte le long de la courbe multiple.
			if ( !this.AdvanceInit() )  return;

			string	character;
			Font	font;
			double	fontSize;
			RichColor fontColor;
			Point	pos, ptl, pbl, ptr, pbr;
			double	angle;
			while ( this.AdvanceNext(out character, out font, out fontSize, out fontColor, out pos, out ptl, out pbl, out ptr, out pbr, out angle) )
			{
				bbox.MergeWith(pbl);
				bbox.MergeWith(pbr);
				bbox.MergeWith(ptl);
				bbox.MergeWith(ptr);
			}
		}

		protected Path HiliteTextCurve()
		{
			//	Donne le chemin du texte le long de la courbe multiple.
			if ( !this.AdvanceInit() )  return null;

			Path path = new Path();

			string	character;
			Font	font;
			double	fontSize;
			RichColor fontColor;
			Point	pos, ptl, pbl, ptr, pbr;
			double	angle;
			while ( this.AdvanceNext(out character, out font, out fontSize, out fontColor, out pos, out ptl, out pbl, out ptr, out pbr, out angle) )
			{
				path.MoveTo(pbl);
				path.LineTo(pbr);
				path.LineTo(ptr);
				path.LineTo(ptl);
				path.Close();
			}

			return path;
		}

		protected Path OneRealPathCurve(int rank)
		{
			//	Construit le chemin réel d'un seul caractère.
			if ( !this.AdvanceInit() )  return null;

			string	character;
			Font	font;
			double	fontSize;
			RichColor fontColor;
			Point	pos, ptl, pbl, ptr, pbr;
			double	angle;

			int i = 0;
			while ( this.AdvanceNext(out character, out font, out fontSize, out fontColor, out pos, out ptl, out pbl, out ptr, out pbr, out angle) )
			{
				if ( i == rank && character[0] != TextLayout.CodeEndOfText )
				{
					int glyph = font.GetGlyphIndex(character[0]);

					Transform transform = Transform.Identity;
					transform = transform.Scale (fontSize);
					transform = transform.RotateDeg (angle);
					transform = transform.Translate (pos);

					Path path = new Path();
					path.Append(font, glyph, transform);
					return path;
				}

				i ++;
			}
			return null;
		}

		public override void DrawText(IPaintPort port, DrawingContext drawingContext)
		{
			//	Dessine le texte le long de la courbe multiple.
			this.cursorBox = Drawing.Rectangle.Empty;
			this.selectBox = Drawing.Rectangle.Empty;

			if ( !this.AdvanceInit() )  return;

			this.textLayout.DrawingScale = drawingContext.ScaleX;

			int cursorFrom = System.Math.Min(this.textNavigator.Context.CursorFrom, this.textNavigator.Context.CursorTo);
			int cursorTo   = System.Math.Max(this.textNavigator.Context.CursorFrom, this.textNavigator.Context.CursorTo);
			Point lastTop    = Point.Zero;
			Point lastBottom = Point.Zero;

			string	character;
			Font	font;
			double	fontSize;
			RichColor fontColor;
			Point	pos, ptl, pbl, ptr, pbr;
			double	angle;
			Point   c1 = new Point(0,0);
			Point   c2 = new Point(0,0);

			bool active = true;
			if ( this.document.Modifier != null )
			{
				active = (this.document.Modifier.ActiveViewer.DrawingContext == drawingContext &&
						  this.document.Modifier.ActiveViewer.IsFocused);
			}

			while ( this.AdvanceNext(out character, out font, out fontSize, out fontColor, out pos, out ptl, out pbl, out ptr, out pbr, out angle) )
			{
				int rank = this.advanceRank-1;

				if ( port is Graphics &&
					 this.edited &&
					 cursorFrom != cursorTo && rank >= cursorFrom && rank < cursorTo )
				{
					Graphics graphics = port as Graphics;
					Path path = new Path();
					path.MoveTo(pbl);
					path.LineTo(pbr);
					path.LineTo(ptr);
					path.LineTo(ptl);
					path.Close();
					graphics.Rasterizer.AddSurface(path);
					graphics.RenderSolid(DrawingContext.ColorSelectEdit(active));

					this.selectBox.MergeWith(pbl);
					this.selectBox.MergeWith(pbr);
					this.selectBox.MergeWith(ptr);
					this.selectBox.MergeWith(ptl);
				}

				if ( character[0] != TextLayout.CodeEndOfText )
				{
					Transform ot = port.Transform;
					port.RotateTransformDeg(angle, pos.X, pos.Y);

					port.RichColor = fontColor;
					port.PaintText(pos.X, pos.Y, character, font, fontSize);
					
					port.Transform = ot;
				}

				if ( port is Graphics &&
					 this.edited &&
					 rank == this.textNavigator.Context.CursorTo )
				{
					Graphics graphics = port as Graphics;
					graphics.LineWidth = 1.0/drawingContext.ScaleX;
					graphics.AddLine(ptl, pbl);
					graphics.RenderSolid(DrawingContext.ColorCursorEdit(active));
					c1 = ptl;
					c2 = pbl;
				}
				
				lastTop    = ptr;
				lastBottom = pbr;
			}

			if ( port is Graphics &&
				 this.edited &&
				 this.advanceRank-1 == this.textNavigator.Context.CursorTo )
			{
				Graphics graphics = port as Graphics;
				graphics.LineWidth = 1.0/drawingContext.ScaleX;
				graphics.AddLine(lastTop, lastBottom);
				graphics.RenderSolid(DrawingContext.ColorCursorEdit(active));
				c1 = lastTop;
				c2 = lastBottom;
			}

			if ( c1.X != 0.0 || c1.Y != 0.0 || c2.X != 0.0 || c2.Y != 0.0 )
			{
				this.ComputeAutoScroll(c1, c2);
				this.cursorBox.MergeWith(c1);
				this.cursorBox.MergeWith(c2);
				this.selectBox.MergeWith(c1);
				this.selectBox.MergeWith(c2);
			}
		}


		private void HandleTextChanged(object sender)
		{
			this.SetDirtyBbox();
		}


		public override Path GetMagnetPath()
		{
			//	Retourne le chemin géométrique de l'objet pour les constructions
			//	magnétiques.
			Path path = this.PathBuild();
			return path;
		}


		#region Serialization
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			//	Sérialise l'objet.
			base.GetObjectData(info, context);
			info.AddValue("Text", this.textLayout.Text);
		}

		protected TextLine(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			//	Constructeur qui désérialise l'objet.
			this.Initialize();
			this.textLayout.Text = info.GetString("Text");
		}

		public override void ReadCheckWarnings(System.Collections.ArrayList warnings)
		{
			//	Vérifie si tous les fichiers existent.
			Common.Document.Objects.Abstract.ReadCheckFonts(warnings, this.textLayout);
		}
		#endregion

		
		protected TextLayout				textLayout;
		protected TextNavigator				textNavigator;

		protected Common.Widgets.TextLayout.OneCharStructure[] advanceCharArray;
		protected int						advanceRank;
		protected int						advanceIndex;
		protected double					advanceBzt;
		protected double					advanceWidth;
		protected Point						advanceP1;
		protected Point						advanceP2;
		protected Point						advanceLastTop;
		protected Point						advanceLastBottom;
		protected bool						advanceCheckEnd;
		protected double					advanceFactor;
		protected double					advanceMaxAscender;
		protected Drawing.Rectangle			cursorBox;
		protected Drawing.Rectangle			selectBox;
		protected Point						initialPos;

		protected static readonly double	step = 0.01;
	}
}

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


		// Nom de l'ic�ne.
		public override string IconName
		{
			get { return Misc.Icon("ObjectBezier"); }
		}


		// D�tecte si la souris est sur le pourtour de l'objet.
		// Retourne le rang de la poign�e de d�part, ou -1
		protected int DetectOutline(Point pos)
		{
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;

			Shape[] shapes = this.ShapesBuild(null, context, false);
			int rank = context.Drawer.DetectOutline(pos, context, shapes);
			if ( rank != -1 )  rank *= 3;
			return rank;
		}

		// D�place tout l'objet.
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

		// S�lectionne toutes les poign�es de l'objet dans un rectangle.
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


		// Donne le contenu du menu contextuel.
		public override void ContextMenu(System.Collections.ArrayList list, Point pos, int handleRank)
		{
			ContextMenuItem item;

			if ( handleRank == -1 )  // sur un segment ?
			{
				int rank = this.DetectOutline(pos);
				if ( rank == -1 )  return;

				item = new ContextMenuItem();
				list.Add(item);  // s�parateur

				if ( this.Handle(rank+2).Type == HandleType.Hide )
				{
					item = new ContextMenuItem();
					item.Command = "Object";
					item.Name = "Curve";
					item.Icon = Misc.Icon("ToCurve");
					item.Text = Res.Strings.Object.Bezier.Menu.ToCurve;
					list.Add(item);
				}
				else
				{
					item = new ContextMenuItem();
					item.Command = "Object";
					item.Name = "Line";
					item.Icon = Misc.Icon("ToLine");
					item.Text = Res.Strings.Object.Bezier.Menu.ToLine;
					list.Add(item);
				}

				item = new ContextMenuItem();
				item.Command = "Object";
				item.Name = "HandleAdd";
				item.Icon = Misc.Icon("Add");
				item.Text = Res.Strings.Object.Bezier.Menu.HandleAdd;
				list.Add(item);
			}
			else	// sur un point ?
			{
				if ( handleRank%3 == 1 )  // poign�e principale ?
				{
					if ( this.Handle(handleRank-1).Type != HandleType.Hide && this.Handle(handleRank+1).Type != HandleType.Hide )
					{
						item = new ContextMenuItem();
						list.Add(item);  // s�parateur

						HandleConstrainType type = this.Handle(handleRank).ConstrainType;

						item = new ContextMenuItem();
						item.Command = "Object";
						item.Name = "HandleSym";
						item.IconActiveNo = Misc.Icon("RadioNo");
						item.IconActiveYes = Misc.Icon("RadioYes");
						item.Active = ( type == HandleConstrainType.Symmetric );
						item.Text = Res.Strings.Object.Bezier.Menu.HandleSym;
						list.Add(item);

						item = new ContextMenuItem();
						item.Command = "Object";
						item.Name = "HandleSmooth";
						item.IconActiveNo = Misc.Icon("RadioNo");
						item.IconActiveYes = Misc.Icon("RadioYes");
						item.Active = ( type == HandleConstrainType.Smooth );
						item.Text = Res.Strings.Object.Bezier.Menu.HandleSmooth;
						list.Add(item);

						item = new ContextMenuItem();
						item.Command = "Object";
						item.Name = "HandleCorner";
						item.IconActiveNo = Misc.Icon("RadioNo");
						item.IconActiveYes = Misc.Icon("RadioYes");
						item.Active = ( type == HandleConstrainType.Corner );
						item.Text = Res.Strings.Object.Bezier.Menu.HandleCorner;
						list.Add(item);
					}
					else if ( this.Handle(handleRank-1).Type != HandleType.Hide || this.Handle(handleRank+1).Type != HandleType.Hide )
					{
						item = new ContextMenuItem();
						list.Add(item);  // s�parateur

						HandleConstrainType type = this.Handle(handleRank).ConstrainType;

						item = new ContextMenuItem();
						item.Command = "Object";
						item.Name = "HandleSmooth";
						item.IconActiveNo = Misc.Icon("RadioNo");
						item.IconActiveYes = Misc.Icon("RadioYes");
						item.Active = ( type == HandleConstrainType.Smooth );
						item.Text = Res.Strings.Object.Bezier.Menu.HandleLine;
						list.Add(item);

						item = new ContextMenuItem();
						item.Command = "Object";
						item.Name = "HandleCorner";
						item.IconActiveNo = Misc.Icon("RadioNo");
						item.IconActiveYes = Misc.Icon("RadioYes");
						item.Active = ( type != HandleConstrainType.Smooth );
						item.Text = Res.Strings.Object.Bezier.Menu.HandleFree;
						list.Add(item);
					}

					bool sep = false;

					if ( !this.PropertyPolyClose.BoolValue &&
						 (this.Handle(handleRank).Type == HandleType.Starting ||
						  this.Handle(this.NextRank(handleRank-1)+1).Type == HandleType.Starting) )
					{
						item = new ContextMenuItem();
						list.Add(item);  // s�parateur
						sep = true;

						item = new ContextMenuItem();
						item.Command = "Object";
						item.Name = "HandleContinue";
						item.Icon = Misc.Icon("Add");
						item.Text = Res.Strings.Object.Bezier.Menu.HandleContinue;
						list.Add(item);
					}

					if ( this.TotalMainHandle >= 3*3 )
					{
						if ( !sep )
						{
							item = new ContextMenuItem();
							list.Add(item);  // s�parateur
						}

						item = new ContextMenuItem();
						item.Command = "Object";
						item.Name = "HandleDelete";
						item.Icon = Misc.Icon("Sub");
						item.Text = Res.Strings.Object.Bezier.Menu.HandleDelete;
						list.Add(item);
					}
				}
			}
		}

		// Ex�cute une commande du menu contextuel.
		public override void ContextCommand(string cmd, Point pos, int handleRank)
		{
			int rank = this.DetectOutline(pos);

			if ( cmd == "Line" )
			{
				if ( rank == -1 )  return;
				this.ContextToLine(rank);
			}

			if ( cmd == "Curve" )
			{
				if ( rank == -1 )  return;
				this.ContextToCurve(rank);
			}

			if ( cmd == "HandleAdd" )
			{
				if ( rank == -1 )  return;
				this.ContextAddHandle(pos, rank);
			}

			if ( cmd == "HandleContinue" )
			{
				this.ContextContinueHandle(handleRank);
			}

			if ( cmd == "HandleSym" )
			{
				this.ContextSym(handleRank);
			}

			if ( cmd == "HandleSmooth" )
			{
				this.ContextSmooth(handleRank);
			}

			if ( cmd == "HandleCorner" )
			{
				this.ContextCorner(handleRank);
			}

			if ( cmd == "HandleDelete" )
			{
				this.ContextSubHandle(pos, handleRank);
			}
		}

		// Passe le point en mode sym�trique.
		protected void ContextSym(int rank)
		{
			this.Handle(rank).ConstrainType = HandleConstrainType.Symmetric;
			this.MoveSecondary(rank, rank-1, rank+1, this.Handle(rank-1).Position);
			this.SetDirtyBbox();
		}

		// Passe le point en mode lisse.
		protected void ContextSmooth(int rank)
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
		protected void ContextCorner(int rank)
		{
			this.Handle(rank).ConstrainType = HandleConstrainType.Corner;
		}

		// Prolonge la courbe.
		protected void ContextContinueHandle(int rank)
		{
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

		// Ajoute une poign�e sans changer l'aspect de la courbe.
		protected void ContextAddHandle(Point pos, int rank)
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

		// Supprime une poign�e sans changer l'aspect de la courbe.
		protected void ContextSubHandle(Point pos, int rank)
		{
			bool starting = (this.Handle(rank).Type == HandleType.Starting);

			this.HandleDelete(rank-1);
			this.HandleDelete(rank-1);
			this.HandleDelete(rank-1);

			// Il doit toujours y avoir une poign�e de d�part !
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
					this.ContextToCurve(prev);
				}
			}
			this.SetDirtyBbox();
			this.HandlePropertiesUpdate();
		}

		// Conversion d'un segement en ligne droite.
		protected void ContextToLine(int rank)
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
		protected void ContextToCurve(int rank)
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


		// Indique si au moins une poign�e est s�lectionn�e par le modeleur.
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

		// Retourne la liste des positions des poign�es s�lectionn�es par le modeleur.
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

		// D�place toutes les poign�es s�lectionn�es par le modeleur.
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

		// Cherche le rang du groupe "sps" pr�c�dent, en tenant compte
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

		// D�place une poign�e primaire selon les contraintes.
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

		// D�place une poign�e secondaire selon les contraintes.
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

		// D�but du d�placement une poign�e.
		public override void MoveHandleStarting(int rank, Point pos, DrawingContext drawingContext)
		{
			base.MoveHandleStarting(rank, pos, drawingContext);

			if ( rank < this.handles.Count )  // poign�e de l'objet ?
			{
				drawingContext.ConstrainFlush();

				Handle handle = this.Handle(rank);
				if ( handle.PropertyType == Properties.Type.None )
				{
					int prev = this.PrevRank(rank/3*3);
					int next = this.NextRank(rank/3*3);

					if ( rank%3 == 1 )  // poign� principale ?
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
					else	// poign�e secondaire ?
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

		// D�place une poign�e.
		public override void MoveHandleProcess(int rank, Point pos, DrawingContext drawingContext)
		{
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


		// D�place globalement l'objet.
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


		// D�but de la cr�ation d'un objet.
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
					this.HandleAdd(pos, HandleType.Starting);
					this.HandleAdd(pos, HandleType.Bezier);
				}
				else
				{
					this.HandleAdd(pos, HandleType.Bezier);
					this.HandleAdd(pos, HandleType.Primary);
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

		// D�placement pendant la cr�ation d'un objet.
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

		// Fin de la cr�ation d'un objet.
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
					this.ContextToLine(rank-6);
				}
			}

			drawingContext.ConstrainDelStarting();
			drawingContext.MagnetClearStarting();
			this.mouseDown = false;
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// Indique si la cr�ation de l'objet est termin�e.
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
		// pas exister et doit �tre d�truit.
		public override bool CreateIsExist(DrawingContext drawingContext)
		{
			return true;
		}

		// Termine la cr�ation de l'objet. Retourne false si l'objet ne peut
		// pas exister et doit �tre d�truit.
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

		// Retourne un bouton d'action pendant la cr�ation.
		public override bool CreateAction(int rank, out string cmd, out string name, out string text)
		{
			if ( rank == 0 )
			{
				cmd  = "Object";
				name = "CreateEnding";
				text = Res.Strings.Object.Bezier.Button.CreateEnding;
				return true;
			}
			if ( rank == 1 )
			{
				cmd  = "Object";
				name = "CreateAndSelect";
				text = Res.Strings.Object.Bezier.Button.CreateAndSelect;
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

			// Forme de la surface de d�part.
			if ( surfaceStart )
			{
				shapes[i] = new Shape();
				shapes[i].Path = pathStart;
				shapes[i].SetPropertySurface(port, this.PropertyLineColor);
				shapes[i].IsMisc = true;
				i ++;
			}

			// Forme de la surface d'arriv�e.
			if ( surfaceEnd )
			{
				shapes[i] = new Shape();
				shapes[i].Path = pathEnd;
				shapes[i].SetPropertySurface(port, this.PropertyLineColor);
				shapes[i].IsMisc = true;
				i ++;
			}

			// Forme du chemin de d�part.
			if ( outlineStart )
			{
				shapes[i] = new Shape();
				shapes[i].Path = pathStart;
				shapes[i].SetPropertyStroke(port, this.PropertyLineMode, this.PropertyLineColor);
				shapes[i].IsMisc = true;
				i ++;
			}

			// Forme du chemin d'arriv�e.
			if ( outlineEnd )
			{
				shapes[i] = new Shape();
				shapes[i].Path = pathEnd;
				shapes[i].SetPropertyStroke(port, this.PropertyLineMode, this.PropertyLineColor);
				shapes[i].IsMisc = true;
				i ++;
			}

			// Forme des traits de support pour les poign�es secondaires.
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

		// Calcule les points de la droite � une extr�mit� de la courbe.
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

		// Cr�e les chemins de l'objet.
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
					if ( this.PropertyPolyClose.BoolValue )  // ferm� ?
					{
						this.PathPutSegment(pathLine, i-1, first, pp1);
						pathLine.Close();
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
			if ( this.PropertyPolyClose.BoolValue )  // ferm� ?
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
		// Retourne le chemin g�om�trique de l'objet pour les constructions
		// magn�tiques.
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

		// Retourne le chemin g�om�trique de l'objet.
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

		// Cr�e une courbe de B�zier � partir d'un chemin quelconque.
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
			// Transforme les courbes en droite si n�cessaire.
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

		// Cr�e une courbe de B�zier � partir de 4 points.
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

		// Finalise la cr�ation d'une courbe de B�zier.
		public void CreateFinalise()
		{
			this.HandlePropertiesCreate();  // cr�e les poign�es des propri�t�s
			this.Select(false);
			this.Select(true);  // pour s�lectionner toutes les poign�es
		}
		#endregion


		#region Serialization
		// S�rialise l'objet.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
		}

		// Constructeur qui d�s�rialise l'objet.
		protected Bezier(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
		#endregion

		
		protected bool				mouseDown = false;
		protected bool				lastPoint;
	}
}

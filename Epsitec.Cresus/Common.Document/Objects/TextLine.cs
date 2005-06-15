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
			this.Initialise();
		}

		protected void Initialise()
		{
			this.textLayout = new TextLayout();
			this.textNavigator = new TextNavigator(this.textLayout);
			this.textNavigator.StyleChanged += new EventHandler(this.HandleTextChanged);
			this.textNavigator.TextInserted += new EventHandler(this.HandleTextChanged);
			this.textNavigator.TextDeleted  += new EventHandler(this.HandleTextChanged);
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
				this.textNavigator.StyleChanged -= new EventHandler(this.HandleTextChanged);
				this.textNavigator.TextInserted -= new EventHandler(this.HandleTextChanged);
				this.textNavigator.TextDeleted  -= new EventHandler(this.HandleTextChanged);
			}

			base.Dispose();
		}


		// Nom de l'icône.
		public override string IconName
		{
			get { return "manifest:Epsitec.App.DocumentEditor.Images.TextLine.icon"; }
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

		
		// Détecte si la souris est sur l'objet.
		public override bool Detect(Point pos)
		{
			if ( this.isHide )  return false;

			Drawing.Rectangle bbox = this.BoundingBox;
			if ( !bbox.Contains(pos) )  return false;

			Path path = this.PathBuild();
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			if ( Geometry.DetectOutline(path, context.MinimalWidth, pos) )  return true;
			return this.DetectTextCurve(pos);
		}

		// Détecte si l'objet est dans un rectangle.
		// all = true  -> toutes les poignées doivent être dans le rectangle
		// all = false -> une seule poignée doit être dans le rectangle
		public override bool Detect(Drawing.Rectangle rect, bool all)
		{
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

		// Détecte si la souris est sur le pourtour de l'objet.
		// Retourne le rank de la poignée de départ, ou -1
		protected int DetectOutline(Point pos)
		{
			Path path = this.PathBuild();
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			int rank = Geometry.DetectOutlineRank(path, context.MinimalWidth, pos);
			if ( rank != -1 )  rank *= 3;
			return rank;
		}

		// Détecte si la souris est sur l'objet pour l'éditer.
		public override bool DetectEdit(Point pos)
		{
			if ( this.isHide )  return false;

			Drawing.Rectangle bbox = this.BoundingBox;
			if ( !bbox.Contains(pos) )  return false;

			Path path = this.PathBuild();
			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			if ( Geometry.DetectOutline(path, context.MinimalWidth, pos) )  return true;
			return this.DetectTextCurve(pos);
		}


		// Déplace tout l'objet.
		public override void MoveAllProcess(Point move)
		{
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


		// Donne le contenu du menu contextuel.
		public override void ContextMenu(System.Collections.ArrayList list, Point pos, int handleRank)
		{
			ContextMenuItem item;

			if ( handleRank == -1 )  // sur un segment ?
			{
				int rank = this.DetectOutline(pos);
				if ( rank == -1 )  return;

				item = new ContextMenuItem();
				list.Add(item);  // séparateur

				if ( this.Handle(rank+2).Type == HandleType.Hide )
				{
					item = new ContextMenuItem();
					item.Command = "Object";
					item.Name = "Curve";
					item.Icon = "manifest:Epsitec.App.DocumentEditor.Images.ToCurve.icon";
					item.Text = Res.Strings.Object.Bezier.Menu.ToCurve;
					list.Add(item);
				}
				else
				{
					item = new ContextMenuItem();
					item.Command = "Object";
					item.Name = "Line";
					item.Icon = "manifest:Epsitec.App.DocumentEditor.Images.ToLine.icon";
					item.Text = Res.Strings.Object.Bezier.Menu.ToLine;
					list.Add(item);
				}

				item = new ContextMenuItem();
				item.Command = "Object";
				item.Name = "HandleAdd";
				item.Icon = "manifest:Epsitec.App.DocumentEditor.Images.Add.icon";
				item.Text = Res.Strings.Object.Bezier.Menu.HandleAdd;
				list.Add(item);
			}
			else	// sur un point ?
			{
				if ( handleRank%3 == 1 )  // poignée principale ?
				{
					if ( this.Handle(handleRank-1).Type != HandleType.Hide && this.Handle(handleRank+1).Type != HandleType.Hide )
					{
						item = new ContextMenuItem();
						list.Add(item);  // séparateur

						HandleConstrainType type = this.Handle(handleRank).ConstrainType;

						item = new ContextMenuItem();
						item.Command = "Object";
						item.Name = "HandleSym";
						item.IconActiveNo = "manifest:Epsitec.App.DocumentEditor.Images.RadioNo.icon";
						item.IconActiveYes = "manifest:Epsitec.App.DocumentEditor.Images.RadioYes.icon";
						item.Active = ( type == HandleConstrainType.Symmetric );
						item.Text = Res.Strings.Object.Bezier.Menu.HandleSym;
						list.Add(item);

						item = new ContextMenuItem();
						item.Command = "Object";
						item.Name = "HandleSmooth";
						item.IconActiveNo = "manifest:Epsitec.App.DocumentEditor.Images.RadioNo.icon";
						item.IconActiveYes = "manifest:Epsitec.App.DocumentEditor.Images.RadioYes.icon";
						item.Active = ( type == HandleConstrainType.Smooth );
						item.Text = Res.Strings.Object.Bezier.Menu.HandleSmooth;
						list.Add(item);

						item = new ContextMenuItem();
						item.Command = "Object";
						item.Name = "HandleCorner";
						item.IconActiveNo = "manifest:Epsitec.App.DocumentEditor.Images.RadioNo.icon";
						item.IconActiveYes = "manifest:Epsitec.App.DocumentEditor.Images.RadioYes.icon";
						item.Active = ( type == HandleConstrainType.Corner );
						item.Text = Res.Strings.Object.Bezier.Menu.HandleCorner;
						list.Add(item);
					}
					else if ( this.Handle(handleRank-1).Type != HandleType.Hide || this.Handle(handleRank+1).Type != HandleType.Hide )
					{
						item = new ContextMenuItem();
						list.Add(item);  // séparateur

						HandleConstrainType type = this.Handle(handleRank).ConstrainType;

						item = new ContextMenuItem();
						item.Command = "Object";
						item.Name = "HandleSmooth";
						item.IconActiveNo = "manifest:Epsitec.App.DocumentEditor.Images.RadioNo.icon";
						item.IconActiveYes = "manifest:Epsitec.App.DocumentEditor.Images.RadioYes.icon";
						item.Active = ( type == HandleConstrainType.Smooth );
						item.Text = Res.Strings.Object.Bezier.Menu.HandleLine;
						list.Add(item);

						item = new ContextMenuItem();
						item.Command = "Object";
						item.Name = "HandleCorner";
						item.IconActiveNo = "manifest:Epsitec.App.DocumentEditor.Images.RadioNo.icon";
						item.IconActiveYes = "manifest:Epsitec.App.DocumentEditor.Images.RadioYes.icon";
						item.Active = ( type != HandleConstrainType.Smooth );
						item.Text = Res.Strings.Object.Bezier.Menu.HandleFree;
						list.Add(item);
					}

					bool sep = false;

					if ( this.Handle(handleRank).Type == HandleType.Starting ||
						 this.Handle(this.NextRank(handleRank-1)+1).Type == HandleType.Starting )
					{
						item = new ContextMenuItem();
						list.Add(item);  // séparateur
						sep = true;

						item = new ContextMenuItem();
						item.Command = "Object";
						item.Name = "HandleContinue";
						item.Icon = "manifest:Epsitec.App.DocumentEditor.Images.Add.icon";
						item.Text = Res.Strings.Object.Bezier.Menu.HandleContinue;
						list.Add(item);
					}

					if ( this.TotalMainHandle >= 3*3 )
					{
						if ( !sep )
						{
							item = new ContextMenuItem();
							list.Add(item);  // séparateur
						}

						item = new ContextMenuItem();
						item.Command = "Object";
						item.Name = "HandleDelete";
						item.Icon = "manifest:Epsitec.App.DocumentEditor.Images.Sub.icon";
						item.Text = Res.Strings.Object.Bezier.Menu.HandleDelete;
						list.Add(item);
					}
				}
			}
		}

		// Exécute une commande du menu contextuel.
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

		// Passe le point en mode symétrique.
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

		// Ajoute une poignée sans changer l'aspect de la courbe.
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

		// Supprime une poignée sans changer l'aspect de la courbe.
		protected void ContextSubHandle(Point pos, int rank)
		{
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
					this.ContextToCurve(prev);
				}
			}
			this.SetDirtyBbox();
		}

		// Conversion d'un segement en ligne droite.
		protected void ContextToLine(int rank)
		{
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

		// Conversion d'un segement en courbe.
		protected void ContextToCurve(int rank)
		{
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


		// Adapte le point secondaire s'il est en mode "en ligne".
		protected void AdaptPrimaryLine(int rankPrimary, int rankSecondary, out int rankExtremity)
		{
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
						if ( rankOpposite >= this.handles.Count )  rankOpposite = 1;
					}
					else
					{
						rankOpposite -= 2;
						if ( rankOpposite < 0 )  rankOpposite = this.handles.Count-2;
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

		// Déplace une poignée.
		public override void MoveHandleProcess(int rank, Point pos, DrawingContext drawingContext)
		{
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


		// Début de la création d'un objet.
		public override void CreateMouseDown(Point pos, DrawingContext drawingContext)
		{
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

		// Déplacement pendant la création d'un objet.
		public override void CreateMouseMove(Point pos, DrawingContext drawingContext)
		{
			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapPos(ref pos);
			this.Handle(3).Position = pos;
			this.Handle(4).Position = pos;
			this.Handle(5).Position = pos;
			this.SetDirtyBbox();
			this.TextInfoModifLine();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// Fin de la création d'un objet.
		public override void CreateMouseUp(Point pos, DrawingContext drawingContext)
		{
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

		// Indique si l'objet doit exister. Retourne false si l'objet ne peut
		// pas exister et doit être détruit.
		public override bool CreateIsExist(DrawingContext drawingContext)
		{
			this.Deselect();
			double len = Point.Distance(this.Handle(1).Position, this.Handle(4).Position);
			return ( len > drawingContext.MinimalSize );
		}

		// Indique s'il faut sélectionner l'objet après sa création.
		public override bool EditAfterCreation()
		{
			return true;
		}

		// Ajoute toutes les fontes utilisées par l'objet dans une liste.
		public override void FillFontFaceList(System.Collections.ArrayList list)
		{
			this.textLayout.FillFontFaceList(list);
		}

		// Indique si un objet est éditable.
		public override bool IsEditable
		{
			get { return true; }
		}

		// Lie l'objet éditable à une règle.
		public override bool EditRulerLink(TextRuler ruler, DrawingContext drawingContext)
		{
			ruler.ListCapability = false;
			ruler.TabCapability = false;
			ruler.AttachToText(this.textNavigator);
			return true;
		}


		// Reprend toutes les caractéristiques d'un objet.
		public override void CloneObject(Objects.Abstract src)
		{
			base.CloneObject(src);

			TextLine obj = src as TextLine;
			this.textLayout.Text = obj.textLayout.Text;
			obj.textNavigator.Context.CopyTo(this.textNavigator.Context);
		}

		
		// Gestion d'un événement pendant l'édition.
		public override bool EditProcessMessage(Message message, Point pos)
		{
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

			if ( message.IsMouseType )
			{
				int rank = this.DetectTextCurveRank(pos);
				pos = this.RankToLinearPos(rank);
				if ( pos == Point.Empty )  return false;
			}

			if ( message.Type == MessageType.KeyDown )
			{
				if ( message.KeyCode == KeyCode.Return ||
					 message.KeyCode == KeyCode.Tab    )  return false;
			}

			if ( !this.textNavigator.ProcessMessage(message, pos) )  return false;

			this.SetDirtyBbox();
			return true;
		}

		// Gestion des événements clavier.
		protected bool EditProcessKeyPress(Message message)
		{
			if ( message.IsCtrlPressed )
			{
				switch ( message.KeyCode )
				{
					case KeyCode.AlphaX:  return this.EditCut();
					case KeyCode.AlphaC:  return this.EditCopy();
					case KeyCode.AlphaV:  return this.EditPaste();
					case KeyCode.AlphaB:  return this.EditBold();
					case KeyCode.AlphaI:  return this.EditItalic();
					case KeyCode.AlphaU:  return this.EditUnderlined();
					case KeyCode.AlphaA:  return this.EditSelectAll();
				}
			}
			return false;
		}

		#region CopyPaste
		public override bool EditCut()
		{
			string text = this.textNavigator.Selection;
			if ( text == "" )  return false;
			Support.Clipboard.WriteData data = new Support.Clipboard.WriteData();
			data.WriteHtmlFragment(text);
			data.WriteTextLayout(text);
			Support.Clipboard.SetData(data);
			this.textNavigator.Selection = "";
			this.document.Notifier.NotifyArea(this.BoundingBox);
			return true;
		}
		
		public override bool EditCopy()
		{
			string text = this.textNavigator.Selection;
			Support.Clipboard.WriteData data = new Support.Clipboard.WriteData();
			if ( text == "" )  return false;
			data.WriteHtmlFragment(text);
			data.WriteTextLayout(text);
			Support.Clipboard.SetData(data);
			return true;
		}
		
		public override bool EditPaste()
		{
			Support.Clipboard.ReadData data = Support.Clipboard.GetData();
			string html = data.ReadTextLayout();
			if ( html == null )
			{
				html = data.ReadHtmlFragment();
				if ( html != null )
				{
					html = Support.Clipboard.ConvertHtmlToSimpleXml(html);
				}
				else
				{
					html = TextLayout.ConvertToTaggedText(data.ReadText());
				}
			}
			if ( html == null )  return false;
			this.textNavigator.Selection = html;
			this.document.Notifier.NotifyArea(this.BoundingBox);
			return true;
		}

		public override bool EditSelectAll()
		{
			this.textLayout.SelectAll(this.textNavigator.Context);
			return true;
		}
		#endregion

		// Insère un glyphe dans le pavé en édition.
		public override bool EditInsertGlyph(string text)
		{
			this.textNavigator.Selection = text;
			this.document.Notifier.NotifyArea(this.BoundingBox);
			return true;
		}

		// Donne la fonte actullement utilisée.
		public override string EditGetFontName()
		{
			return this.textNavigator.SelectionFontName;
		}

		#region TextFormat
		// Met en gras pendant l'édition.
		public override bool EditBold()
		{
			this.textNavigator.SelectionBold = !this.textNavigator.SelectionBold;
			return true;
		}

		// Met en italique pendant l'édition.
		public override bool EditItalic()
		{
			this.textNavigator.SelectionItalic = !this.textNavigator.SelectionItalic;
			return true;
		}

		// Souligne pendant l'édition.
		public override bool EditUnderlined()
		{
			this.textNavigator.SelectionUnderlined = !this.textNavigator.SelectionUnderlined;
			return true;
		}
		#endregion

		// Donne la zone contenant le curseur d'édition.
		public override Drawing.Rectangle EditCursorBox
		{
			get
			{
				return this.cursorBox;
			}
		}

		// Donne la zone contenant le texte sélectionné.
		public override Drawing.Rectangle EditSelectBox
		{
			get
			{
				return this.selectBox;
			}
		}

		// Gestion d'un événement pendant l'édition.
		public override void EditMouseDownMessage(Point pos)
		{
			int rank = this.DetectTextCurveRank(pos);
			pos = this.RankToLinearPos(rank);
			if ( pos == Point.Empty )  return;

			this.textNavigator.MouseDownMessage(pos);
		}

		
		// Met à jour le rectangle englobant l'objet.
		protected override void UpdateBoundingBox()
		{
			Path path = this.PathBuild();
			this.bboxThin = Geometry.ComputeBoundingBox(path);
			this.BboxTextCurve(ref this.bboxThin);

			this.bboxGeom = this.bboxThin;
			this.bboxFull = this.bboxThin;
			this.bboxFull.MergeWith(this.FullBoundingBox());
		}

		// Calcule la bbox qui englobe l'objet et les poignées secondaires.
		protected Drawing.Rectangle FullBoundingBox()
		{
			Drawing.Rectangle bbox = Drawing.Rectangle.Empty;
			int total = this.TotalHandle;
			for ( int i=0 ; i<total ; i++ )
			{
				bbox.MergeWith(this.Handle(i).Position);
			}
			return bbox;
		}

		// Crée le chemin de l'objet.
		protected Path PathBuild()
		{
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

		// Retourne la longueur totale d'une courbe multiple.
		protected double GetLength()
		{
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

		// Avance le long d'une courbe multiple.
		// La courbe est fragmentée en 100 morceaux (TextLine.step = 0.01)
		// considérés chacuns comme des lignes droites.
		// Retourne false lorsqu'on arrive à la fin.
		// Le mode checkEnd = true ne teste pas l'arrivée à la fin, ce qui est
		// utile en mode JustifHorizontal.Right pour être certain de caser le
		// dernier caractère. Sans cela, des erreurs d'arrondi font qu'il est
		// parfois considéré comme hors du tracé.
		protected bool Advance(double width, bool checkEnd, ref int i, ref double t, ref Point pos)
		{
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

		// Retourne la largeur d'un caractère.
		protected double AdvanceString(Font font, string text)
		{
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

		// Retourne la bbox d'un caractère.
		protected Drawing.Rectangle AdvanceBounds(Font font, string text)
		{
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

		// Initialise l'avance le long des caractères du texte.
		protected bool AdvanceInit()
		{
			Properties.TextLine justif = this.PropertyTextLine;

			this.textLayout.DefaultFont     = this.PropertyTextFont.GetFont();
			this.textLayout.DefaultFontSize = this.PropertyTextFont.FontSize;
			this.textLayout.DefaultColor    = this.PropertyTextFont.FontColor;
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
			this.advanceLastTop    = Point.Empty;
			this.advanceLastBottom = Point.Empty;
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

		// Avance sur le prochain caractère du texte.
		protected bool AdvanceNext(out string character,
								   out Font font,
								   out double fontSize,
								   out Color fontColor,
								   out Point pos,
								   out Point ptl,
								   out Point pbl,
								   out Point ptr,
								   out Point pbr,
								   out double angle)
		{
			Properties.TextLine justif = this.PropertyTextLine;

			character = "";
			font = null;
			fontSize = 0;
			fontColor = Color.Empty;
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

			if ( !this.advanceLastTop.IsEmpty )
			{
				ptl = Point.Projection(this.advanceLastTop, this.advanceLastBottom, ptl);
				pbl = Point.Projection(this.advanceLastTop, this.advanceLastBottom, pbl);
			}

			this.advanceP1 = this.advanceP2;
			this.advanceLastTop    = ptr;
			this.advanceLastBottom = pbr;

			return true;
		}

		// Conversion d'un rank dans le texte en une position linéaire, c'est-à-dire
		// une position qui suppose que le texte est droit.
		protected Point RankToLinearPos(int rank)
		{
			Point lp = Point.Empty;
			if ( rank == -1 || !this.AdvanceInit() )  return lp;

			lp.X = 0.00001;  // pour feinter Point.IsEmpty !
			lp.Y = 0.00001;
			string	character;
			Font	font;
			double	fontSize;
			Color	fontColor;
			Point	pos, ptl, pbl, ptr, pbr;
			double	angle;
			while ( this.AdvanceNext(out character, out font, out fontSize, out fontColor, out pos, out ptl, out pbl, out ptr, out pbr, out angle) )
			{
				if ( rank == this.advanceRank-1 )  return lp;
				lp.X += this.advanceWidth / this.advanceFactor;
			}
			return lp;
		}

		// Détecte si la souris est sur un caractère du texte le long de la courbe multiple.
		protected bool DetectTextCurve(Point mouse)
		{
			if ( !this.AdvanceInit() )  return false;

			string	character;
			Font	font;
			double	fontSize;
			Color	fontColor;
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

		// Détecte sur quel caractère est la souris le long de la courbe multiple.
		protected int DetectTextCurveRank(Point mouse)
		{
			if ( !this.AdvanceInit() )  return -1;

			string	character;
			Font	font;
			double	fontSize;
			Color	fontColor;
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

		// Calcule la bbox du texte le long de la courbe multiple.
		protected void BboxTextCurve(ref Drawing.Rectangle bbox)
		{
			if ( !this.AdvanceInit() )  return;

			string	character;
			Font	font;
			double	fontSize;
			Color	fontColor;
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

		// Dessine le texte le long de la courbe multiple.
		protected void HiliteTextCurve(Graphics graphics, DrawingContext drawingContext)
		{
			if ( !this.AdvanceInit() )  return;

			string	character;
			Font	font;
			double	fontSize;
			Color	fontColor;
			Point	pos, ptl, pbl, ptr, pbr;
			double	angle;
			while ( this.AdvanceNext(out character, out font, out fontSize, out fontColor, out pos, out ptl, out pbl, out ptr, out pbr, out angle) )
			{
				Path path = new Path();
				path.MoveTo(pbl);
				path.LineTo(pbr);
				path.LineTo(ptr);
				path.LineTo(ptl);
				path.Close();
				graphics.Rasterizer.AddSurface(path);
			}
			graphics.RenderSolid(drawingContext.HiliteSurfaceColor);
		}

		// Construit le chemin réel d'un seul caractère.
		protected Path OneRealPathCurve(int rank)
		{
			if ( !this.AdvanceInit() )  return null;

			string	character;
			Font	font;
			double	fontSize;
			Color	fontColor;
			Point	pos, ptl, pbl, ptr, pbr;
			double	angle;

			int i = 0;
			while ( this.AdvanceNext(out character, out font, out fontSize, out fontColor, out pos, out ptl, out pbl, out ptr, out pbr, out angle) )
			{
				if ( i == rank && character[0] != TextLayout.CodeEndOfText )
				{
					int glyph = font.GetGlyphIndex(character[0]);

					Transform transform = new Transform();
					transform.Scale(fontSize);
					transform.RotateDeg(angle);
					transform.Translate(pos);

					Path path = new Path();
					path.Append(font, glyph, transform);
					return path;
				}

				i ++;
			}
			return null;
		}

		// Dessine le texte le long de la courbe multiple.
		protected void DrawTextCurve(IPaintPort port, DrawingContext drawingContext)
		{
			this.cursorBox = Drawing.Rectangle.Empty;
			this.selectBox = Drawing.Rectangle.Empty;

			if ( !this.AdvanceInit() )  return;

			this.textLayout.DrawingScale = drawingContext.ScaleX;

			int cursorFrom = System.Math.Min(this.textNavigator.Context.CursorFrom, this.textNavigator.Context.CursorTo);
			int cursorTo   = System.Math.Max(this.textNavigator.Context.CursorFrom, this.textNavigator.Context.CursorTo);
			Point lastTop    = Point.Empty;
			Point lastBottom = Point.Empty;

			string	character;
			Font	font;
			double	fontSize;
			Color	fontColor;
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

					if ( port is Graphics )
					{
						Graphics graphics = port as Graphics;
						graphics.AddText(pos.X, pos.Y, character, font, fontSize);
						graphics.RenderSolid(drawingContext.AdaptColor(fontColor));
					}
					
					if ( port is Common.Printing.PrintPort )
					{
						port.Color = drawingContext.AdaptColor(fontColor);
						port.PaintText(pos.X, pos.Y, character, font, fontSize);
					}
					
					if ( port is PDFPort )
					{
						port.Color = drawingContext.AdaptColor(fontColor);
						port.PaintText(pos.X, pos.Y, character, font, fontSize);
					}
					
					port.Transform = ot;
				}

				if ( port is Graphics &&
					 this.edited &&
					 rank == this.textNavigator.Context.CursorTo )
				{
					if ( active )
					{
						Graphics graphics = port as Graphics;
						graphics.LineWidth = 1.0/drawingContext.ScaleX;
						graphics.AddLine(ptl, pbl);
						graphics.RenderSolid(DrawingContext.ColorCursorEdit);
					}
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
				if ( active )
				{
					Graphics graphics = port as Graphics;
					graphics.LineWidth = 1.0/drawingContext.ScaleX;
					graphics.AddLine(lastTop, lastBottom);
					graphics.RenderSolid(DrawingContext.ColorCursorEdit);
				}
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

		// Dessine l'objet.
		public override void DrawGeometry(Graphics graphics, DrawingContext drawingContext)
		{
			base.DrawGeometry(graphics, drawingContext);

			int total = this.TotalHandle;
			if ( total < 6 )  return;

			Path path = this.PathBuild();

			if ( this.IsHilite && drawingContext.IsActive && !this.edited )
			{
				this.HiliteTextCurve(graphics, drawingContext);
			}

			if ( this.edited && drawingContext.IsActive )  // en cours d'édition ?
			{
				graphics.Rasterizer.AddOutline(path, 2.0/drawingContext.ScaleX);
				graphics.RenderSolid(DrawingContext.ColorFrameEdit);
			}
			else
			{
				if ( this.IsHilite && drawingContext.IsActive )
				{
					graphics.Rasterizer.AddOutline(path, drawingContext.HiliteSize);
					graphics.RenderSolid(drawingContext.HiliteOutlineColor);
				}
			}

			this.DrawTextCurve(graphics, drawingContext);  // dessine le texte

			if ( (this.IsSelected || this.textLayout.Text == "") && drawingContext.IsActive )
			{
				graphics.Rasterizer.AddOutline(path, 1.0/drawingContext.ScaleX);
				graphics.RenderSolid(Color.FromBrightness(0.6));
			}

			if ( this.IsSelected && drawingContext.IsActive && !this.IsGlobalSelected && !this.edited )
			{
				double initialWidth = graphics.LineWidth;
				graphics.LineWidth = 1.0/drawingContext.ScaleX;

				for ( int i=0 ; i<total ; i+=3 )
				{
					graphics.AddLine(this.Handle(i+0).Position, this.Handle(i+1).Position);
					graphics.AddLine(this.Handle(i+1).Position, this.Handle(i+2).Position);
					graphics.RenderSolid(Color.FromBrightness(0.6));
				}

				graphics.LineWidth = initialWidth;
			}
		}


		// Imprime l'objet.
		public override void PrintGeometry(Printing.PrintPort port, DrawingContext drawingContext)
		{
			base.PrintGeometry(port, drawingContext);

			if ( this.TotalHandle < 6 )  return;

			this.DrawTextCurve(port, drawingContext);  // dessine le texte
		}

		// Exporte en PDF la géométrie de l'objet.
		public override void ExportPDF(PDFPort port, DrawingContext drawingContext)
		{
			if ( this.TotalHandle < 6 )  return;

			this.DrawTextCurve(port, drawingContext);  // dessine le texte
		}

		
		private void HandleTextChanged(object sender)
		{
			this.SetDirtyBbox();
		}


		// Retourne le chemin géométrique de l'objet pour les constructions
		// magnétiques.
		public override Path GetMagnetPath()
		{
			Path path = this.PathBuild();
			return path;
		}

		// Retourne le chemin géométrique de l'objet.
		public override Path GetPath(int rank)
		{
			return this.OneRealPathCurve(rank);
		}


		#region Serialization
		// Sérialise l'objet.
		public override void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			base.GetObjectData(info, context);
			info.AddValue("Text", this.textLayout.Text);
		}

		// Constructeur qui désérialise l'objet.
		protected TextLine(SerializationInfo info, StreamingContext context) : base(info, context)
		{
			this.Initialise();
			this.textLayout.Text = info.GetString("Text");
		}

		// Vérifie si tous les fichiers existent.
		public override void ReadCheckWarnings(Font.FaceInfo[] fonts, System.Collections.ArrayList warnings)
		{
			Common.Document.Objects.Abstract.ReadCheckFonts(fonts, warnings, this.textLayout);
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

		protected static readonly double	step = 0.01;
	}
}

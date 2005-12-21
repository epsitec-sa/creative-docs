using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using System.Xml.Serialization;

namespace Epsitec.Common.Pictogram.Data
{
	/// <summary>
	/// La classe ObjectTextLine est la classe de l'objet graphique "texte simple".
	/// </summary>
	public class ObjectTextLine : AbstractObject
	{
		public ObjectTextLine()
		{
			PropertyName name = new PropertyName();
			name.Type = PropertyType.Name;
			this.AddProperty(name);

			PropertyTextLine textLine = new PropertyTextLine();
			textLine.Type = PropertyType.TextLine;
			this.AddProperty(textLine);

			PropertyFont font = new PropertyFont();
			font.Type = PropertyType.TextFont;
			this.AddProperty(font);

			this.textLayout = new TextLayout();
			this.textNavigator = new TextNavigator(this.textLayout);
			this.textNavigator.StyleChanged += new EventHandler(this.HandleTextChanged);
			this.textNavigator.TextInserted += new EventHandler(this.HandleTextChanged);
			this.textNavigator.TextDeleted  += new EventHandler(this.HandleTextChanged);
			this.textLayout.BreakMode = Drawing.TextBreakMode.Hyphenate;
			this.textLayout.LayoutSize = new Drawing.Size(1000000, 1000000);
			this.textLayout.Alignment = Drawing.ContentAlignment.BottomLeft;
			this.textNavigator.OpletQueue = Widgets.IconEditor.OpletQueue;
		}

		protected override AbstractObject CreateNewObject()
		{
			return new ObjectTextLine();
		}

		public override void Dispose()
		{
			this.textNavigator.StyleChanged -= new EventHandler(this.HandleTextChanged);
			this.textNavigator.TextInserted -= new EventHandler(this.HandleTextChanged);
			this.textNavigator.TextDeleted  -= new EventHandler(this.HandleTextChanged);
			base.Dispose();
		}


		public override string IconName
		{
			//	Nom de l'ic�ne.
			get { return @"file:images/textline.icon"; }
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

		
		public override bool Detect(Drawing.Point pos)
		{
			//	D�tecte si la souris est sur l'objet.
			if ( this.isHide )  return false;

			Drawing.Rectangle bbox = this.BoundingBox;
			if ( !bbox.Contains(pos) )  return false;

			Drawing.Path path = this.PathBuild();
			if ( AbstractObject.DetectOutline(path, this.minimalWidth, pos) )  return true;
			return this.DetectTextCurve(pos);
		}

		public override bool Detect(Drawing.Rectangle rect, bool all)
		{
			//	D�tecte si l'objet est dans un rectangle.
			//	all = true  -> toutes les poign�es doivent �tre dans le rectangle
			//	all = false -> une seule poign�e doit �tre dans le rectangle
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

		protected int DetectOutline(Drawing.Point pos)
		{
			//	D�tecte si la souris est sur le pourtour de l'objet.
			//	Retourne le rank de la poign�e de d�part, ou -1
			Drawing.Path path = this.PathBuild();
			int rank = AbstractObject.DetectOutlineRank(path, this.minimalWidth, pos);
			if ( rank != -1 )  rank *= 3;
			return rank;
		}

		public override bool DetectEdit(Drawing.Point pos)
		{
			//	D�tecte si la souris est sur l'objet pour l'�diter.
			if ( this.isHide )  return false;

			Drawing.Rectangle bbox = this.BoundingBox;
			if ( !bbox.Contains(pos) )  return false;

			Drawing.Path path = this.PathBuild();
			if ( AbstractObject.DetectOutline(path, this.minimalWidth, pos) )  return true;
			return this.DetectTextCurve(pos);
		}


		public override void MoveAll(Drawing.Point move, bool all)
		{
			//	D�place tout l'objet.
			int total = this.handles.Count;
			for ( int i=0 ; i<total ; i+=3 )
			{
				if ( all || this.Handle(i+1).IsSelected )
				{
					this.Handle(i+0).Position += move;
					this.Handle(i+1).Position += move;
					this.Handle(i+2).Position += move;
				}
			}
			this.dirtyBbox = true;
		}


		public override void ContextMenu(System.Collections.ArrayList list, Drawing.Point pos, int handleRank)
		{
			//	Donne le contenu du menu contextuel.
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
					item.Icon = @"file:images/tocurve.icon";
					item.Text = "Courbe";
					list.Add(item);
				}
				else
				{
					item = new ContextMenuItem();
					item.Command = "Object";
					item.Name = "Line";
					item.Icon = @"file:images/toline.icon";
					item.Text = "Droit";
					list.Add(item);
				}

				item = new ContextMenuItem();
				item.Command = "Object";
				item.Name = "HandleAdd";
				item.Icon = @"file:images/add.icon";
				item.Text = "Ajouter un point";
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
						item.IconActiveNo = @"file:images/activeno.icon";
						item.IconActiveYes = @"file:images/activeyes.icon";
						item.Active = ( type == HandleConstrainType.Symmetric );
						item.Text = "Symetrique";
						list.Add(item);

						item = new ContextMenuItem();
						item.Command = "Object";
						item.Name = "HandleSmooth";
						item.IconActiveNo = @"file:images/activeno.icon";
						item.IconActiveYes = @"file:images/activeyes.icon";
						item.Active = ( type == HandleConstrainType.Smooth );
						item.Text = "Lisse";
						list.Add(item);

						item = new ContextMenuItem();
						item.Command = "Object";
						item.Name = "HandleCorner";
						item.IconActiveNo = @"file:images/activeno.icon";
						item.IconActiveYes = @"file:images/activeyes.icon";
						item.Active = ( type == HandleConstrainType.Corner );
						item.Text = "Anguleux";
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
						item.IconActiveNo = @"file:images/activeno.icon";
						item.IconActiveYes = @"file:images/activeyes.icon";
						item.Active = ( type == HandleConstrainType.Smooth );
						item.Text = "En ligne";
						list.Add(item);

						item = new ContextMenuItem();
						item.Command = "Object";
						item.Name = "HandleCorner";
						item.IconActiveNo = @"file:images/activeno.icon";
						item.IconActiveYes = @"file:images/activeyes.icon";
						item.Active = ( type != HandleConstrainType.Smooth );
						item.Text = "Libre";
						list.Add(item);
					}

					if ( this.handles.Count >= 3*3 )
					{
						item = new ContextMenuItem();
						list.Add(item);  // s�parateur

						item = new ContextMenuItem();
						item.Command = "Object";
						item.Name = "HandleDelete";
						item.Icon = @"file:images/sub.icon";
						item.Text = "Enlever le point";
						list.Add(item);
					}
				}
			}
		}

		public override void ContextCommand(string cmd, Drawing.Point pos, int handleRank)
		{
			//	Ex�cute une commande du menu contextuel.
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

		protected void ContextSym(int rank)
		{
			//	Passe le point en mode sym�trique.
			this.Handle(rank).ConstrainType = HandleConstrainType.Symmetric;
			this.MoveSecondary(rank, rank-1, rank+1, this.Handle(rank-1).Position);
			this.dirtyBbox = true;
		}

		protected void ContextSmooth(int rank)
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
			this.dirtyBbox = true;
		}

		protected void ContextCorner(int rank)
		{
			//	Passe le point en mode anguleux.
			this.Handle(rank).ConstrainType = HandleConstrainType.Corner;
		}

		protected void ContextAddHandle(Drawing.Point pos, int rank)
		{
			//	Ajoute une poign�e sans changer l'aspect de la courbe.
			for ( int i=0 ; i<3 ; i++ )
			{
				Handle handle = new Handle();
				handle.Position = pos;
				handle.Type = (i==1) ? HandleType.Primary : HandleType.Bezier;
				handle.IsSelected = true;
				this.HandleInsert(rank+3, handle);
			}

			int prev = rank+0;
			int curr = rank+3;
			int next = rank+6;
			if ( next >= this.handles.Count )  next = 0;

			if ( this.Handle(prev+2).Type == HandleType.Hide && this.Handle(next+0).Type == HandleType.Hide )
			{
				pos = Drawing.Point.Projection(this.Handle(prev+1).Position, this.Handle(next+1).Position, pos);
				this.Handle(curr+0).Position = pos;
				this.Handle(curr+1).Position = pos;
				this.Handle(curr+2).Position = pos;
				this.Handle(curr+0).Type = HandleType.Hide;
				this.Handle(curr+2).Type = HandleType.Hide;
			}
			else
			{
				double t = Drawing.Point.FindBezierParameter(this.Handle(prev+1).Position, this.Handle(prev+2).Position, this.Handle(next+0).Position, this.Handle(next+1).Position, pos);
				this.Handle(curr+1).Position = Drawing.Point.FromBezier(this.Handle(prev+1).Position, this.Handle(prev+2).Position, this.Handle(next+0).Position, this.Handle(next+1).Position, t);
				pos = Drawing.Point.Scale(this.Handle(prev+2).Position, this.Handle(next+0).Position, t);
				this.Handle(prev+2).Position = Drawing.Point.Scale(this.Handle(prev+1).Position, this.Handle(prev+2).Position, t);
				this.Handle(next+0).Position = Drawing.Point.Scale(this.Handle(next+1).Position, this.Handle(next+0).Position, 1-t);
				this.Handle(curr+0).Position = Drawing.Point.Scale(this.Handle(prev+2).Position, pos, t);
				this.Handle(curr+2).Position = Drawing.Point.Scale(this.Handle(next+0).Position, pos, 1-t);

				this.Handle(curr+1).ConstrainType = HandleConstrainType.Smooth;
				if ( this.Handle(prev+1).ConstrainType == HandleConstrainType.Symmetric )  this.Handle(prev+1).ConstrainType = HandleConstrainType.Smooth;
				if ( this.Handle(next+1).ConstrainType == HandleConstrainType.Symmetric )  this.Handle(next+1).ConstrainType = HandleConstrainType.Smooth;
			}
			this.dirtyBbox = true;
		}

		protected void ContextSubHandle(Drawing.Point pos, int rank)
		{
			//	Supprime une poign�e sans changer l'aspect de la courbe.
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
			this.dirtyBbox = true;
		}

		protected void ContextToLine(int rank)
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
			this.dirtyBbox = true;
		}

		protected void ContextToCurve(int rank)
		{
			//	Conversion d'un segement en courbe.
			int next = rank+3;
			if ( next >= this.handles.Count )  next = 0;
			this.Handle(rank+2).Position = Drawing.Point.Scale(this.Handle(rank+1).Position, this.Handle(next+1).Position, 0.25);
			this.Handle(next+0).Position = Drawing.Point.Scale(this.Handle(next+1).Position, this.Handle(rank+1).Position, 0.25);
			this.Handle(rank+2).Type = HandleType.Bezier;
			this.Handle(next+0).Type = HandleType.Bezier;
			this.Handle(rank+1).ConstrainType = HandleConstrainType.Corner;
			this.Handle(next+1).ConstrainType = HandleConstrainType.Corner;
			this.dirtyBbox = true;
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

			double dist = Drawing.Point.Distance(this.Handle(rankPrimary).Position, this.Handle(rankSecondary).Position);
			Drawing.Point pos = new Drawing.Point();
			pos = Drawing.Point.Move(this.Handle(rankPrimary).Position, this.Handle(rankExtremity).Position, dist);
			pos = Drawing.Point.Symmetry(this.Handle(rankPrimary).Position, pos);
			this.Handle(rankSecondary).Position = pos;
			this.dirtyBbox = true;
		}

		protected void MovePrimary(int rank, Drawing.Point pos)
		{
			//	D�place une poign�e primaire selon les contraintes.
			Drawing.Point move = pos-this.Handle(rank).Position;
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
			this.dirtyBbox = true;
		}

		protected void MoveSecondary(int rankPrimary, int rankSecondary, int rankOpposite, Drawing.Point pos)
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
						rankOpposite += 2;
						if ( rankOpposite >= this.handles.Count )  rankOpposite = 1;
					}
					else
					{
						rankOpposite -= 2;
						if ( rankOpposite < 0 )  rankOpposite = this.handles.Count-2;
					}
					this.Handle(rankSecondary).Position = Drawing.Point.Projection(this.Handle(rankPrimary).Position, this.Handle(rankOpposite).Position, pos);
				}
			}
			else	// courbe ?
			{
				if ( type == HandleConstrainType.Symmetric )
				{
					this.Handle(rankOpposite).Position = Drawing.Point.Symmetry(this.Handle(rankPrimary).Position, this.Handle(rankSecondary).Position);
				}

				if ( type == HandleConstrainType.Smooth )
				{
					double dist = Drawing.Point.Distance(this.Handle(rankPrimary).Position, this.Handle(rankOpposite).Position);
					Drawing.Point p = Drawing.Point.Move(this.Handle(rankPrimary).Position, this.Handle(rankSecondary).Position, dist);
					this.Handle(rankOpposite).Position = Drawing.Point.Symmetry(this.Handle(rankPrimary).Position, p);
				}
			}
		}

		public override void MoveHandleStarting(int rank, Drawing.Point pos, IconContext iconContext)
		{
			//	D�but du d�placement une poign�e.
			if ( rank < this.TotalHandle )
			{
				pos = this.Handle((rank/3)*3+1).Position;
				iconContext.ConstrainFixStarting(pos);
			}
		}

		public override void MoveHandleProcess(int rank, Drawing.Point pos, IconContext iconContext)
		{
			//	D�place une poign�e.
			if ( rank >= this.handles.Count )  // poign�e d'une propri�t� ?
			{
				base.MoveHandleProcess(rank, pos, iconContext);
				return;
			}

			iconContext.ConstrainSnapPos(ref pos);
			iconContext.SnapGrid(ref pos);

			if ( this.Handle(rank).Type == HandleType.Starting ||
				 this.Handle(rank).Type == HandleType.Primary  )  // principale ?
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
			this.dirtyBbox = true;
		}


		public override void CreateMouseDown(Drawing.Point pos, IconContext iconContext)
		{
			//	D�but de la cr�ation d'un objet.
			iconContext.ConstrainFixStarting(pos);

			this.HandleAdd(pos, HandleType.Hide);
			this.HandleAdd(pos, HandleType.Starting);
			this.HandleAdd(pos, HandleType.Hide);

			this.HandleAdd(pos, HandleType.Hide);
			this.HandleAdd(pos, HandleType.Primary);
			this.HandleAdd(pos, HandleType.Hide);

			this.Handle(1).IsSelected = true;
			this.Handle(4).IsSelected = true;
		}

		public override void CreateMouseMove(Drawing.Point pos, IconContext iconContext)
		{
			//	D�placement pendant la cr�ation d'un objet.
			iconContext.ConstrainSnapPos(ref pos);
			iconContext.SnapGrid(ref pos);
			this.Handle(3).Position = pos;
			this.Handle(4).Position = pos;
			this.Handle(5).Position = pos;
			this.dirtyBbox = true;
		}

		public override void CreateMouseUp(Drawing.Point pos, IconContext iconContext)
		{
			//	Fin de la cr�ation d'un objet.
			iconContext.ConstrainSnapPos(ref pos);
			iconContext.SnapGrid(ref pos);
			this.Handle(3).Position = pos;
			this.Handle(4).Position = pos;
			this.Handle(5).Position = pos;
			iconContext.ConstrainDelStarting();
		}

		public override bool CreateIsExist(IconContext iconContext)
		{
			//	Indique si l'objet doit exister. Retourne false si l'objet ne peut
			//	pas exister et doit �tre d�truit.
			this.Deselect();
			double len = Drawing.Point.Distance(this.Handle(1).Position, this.Handle(4).Position);
			return ( len > this.minimalSize );
		}

		public override bool EditAfterCreation()
		{
			//	Indique s'il faut s�lectionner l'objet apr�s sa cr�ation.
			return true;
		}

		public override bool IsEditable()
		{
			//	Indique si un objet est �ditable.
			return true;
		}

		public override bool EditRulerLink(TextRuler ruler, IconContext iconContext)
		{
			//	Lie l'objet �ditable � une r�gle.
			ruler.ListCapability = false;
			ruler.TabCapability = false;
			ruler.AttachToText(this.textNavigator);
			return true;
		}


		public override void CloneObject(AbstractObject src)
		{
			//	Reprend toutes les caract�ristiques d'un objet.
			base.CloneObject(src);

			ObjectTextLine obj = src as ObjectTextLine;
			this.textLayout.Text = obj.textLayout.Text;
			obj.textNavigator.Context.CopyTo(this.textNavigator.Context);
		}

		
		public override bool EditProcessMessage(Message message, Drawing.Point pos)
		{
			//	Gestion d'un �v�nement pendant l'�dition.
			if ( message.IsMouseType )
			{
				int rank = this.DetectTextCurveRank(pos);
				pos = this.RankToLinearPos(rank);
				if ( pos == Drawing.Point.Empty )  return false;
			}

			if ( message.Type == MessageType.KeyDown )
			{
				if ( message.KeyCode == KeyCode.Return ||
					 message.KeyCode == KeyCode.Tab    )  return false;
			}

			if ( !this.textNavigator.ProcessMessage(message, pos) )  return false;

			this.dirtyBbox = true;
			return true;
		}

		public override void EditMouseDownMessage(Drawing.Point pos)
		{
			//	Gestion d'un �v�nement pendant l'�dition.
			int rank = this.DetectTextCurveRank(pos);
			pos = this.RankToLinearPos(rank);
			if ( pos == Drawing.Point.Empty )  return;

			this.textNavigator.MouseDownMessage(pos);
		}

		
		protected override void UpdateBoundingBox()
		{
			//	Met � jour le rectangle englobant l'objet.
			Drawing.Path path = this.PathBuild();
			this.bboxThin = AbstractObject.ComputeBoundingBox(path);
			this.BboxTextCurve(ref this.bboxThin);

			this.bboxGeom = this.bboxThin;
			this.bboxFull = this.bboxThin;
			this.bboxFull.MergeWith(this.FullBoundingBox());
		}

		protected Drawing.Rectangle FullBoundingBox()
		{
			//	Calcule la bbox qui englobe l'objet et les poign�es secondaires.
			Drawing.Rectangle bbox = Drawing.Rectangle.Empty;
			int total = this.TotalHandle;
			for ( int i=0 ; i<total ; i++ )
			{
				bbox.MergeWith(this.Handle(i).Position);
			}
			return bbox;
		}

		protected Drawing.Path PathBuild()
		{
			//	Cr�e le chemin de l'objet.
			Drawing.Path path = new Drawing.Path();

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
				Drawing.Point p1 = this.Handle(i+1).Position;
				Drawing.Point s1 = this.Handle(i+2).Position;
				Drawing.Point s2 = this.Handle(i+3).Position;
				Drawing.Point p2 = this.Handle(i+4).Position;

				if ( this.Handle(i+2).Type == HandleType.Hide )  // droite ?
				{
					length += Drawing.Point.Distance(p1,p2);
				}
				else	// courbe ?
				{
					Drawing.Point pos = p1;
					int total = (int)(1.0/ObjectTextLine.step);
					for ( int rank=1 ; rank<=total ; rank ++ )
					{
						double t = ObjectTextLine.step*rank;
						Drawing.Point next = Drawing.Point.FromBezier(p1,s1,s2,p2, t);
						length += Drawing.Point.Distance(pos, next);
						pos = next;
					}
				}
				i += 3;  // courbe suivante
			}
			while ( i < this.handles.Count-3 );

			return length;
		}

		protected bool Advance(double width, bool checkEnd, ref int i, ref double t, ref Drawing.Point pos)
		{
			//	Avance le long d'une courbe multiple.
			//	La courbe est fragment�e en 100 morceaux (ObjectTextLine.step = 0.01)
			//	consid�r�s chacuns comme des lignes droites.
			//	Retourne false lorsqu'on arrive � la fin.
			//	Le mode checkEnd = true ne teste pas l'arriv�e � la fin, ce qui est
			//	utile en mode JustifHorizontal.Right pour �tre certain de caser le
			//	dernier caract�re. Sans cela, des erreurs d'arrondi font qu'il est
			//	parfois consid�r� comme hors du trac�.
			if ( i >= this.handles.Count-3 )  return false;

			while ( true )
			{
				Drawing.Point p1 = this.Handle(i+1).Position;
				Drawing.Point s1 = this.Handle(i+2).Position;
				Drawing.Point s2 = this.Handle(i+3).Position;
				Drawing.Point p2 = this.Handle(i+4).Position;

				if ( this.Handle(i+2).Type == HandleType.Hide )  // droite ?
				{
					double d = Drawing.Point.Distance(p1,p2);
					double t2 = (t*d+width)/d;
					if ( t2 <= 1.0 )
					{
						t = t2;
						pos = Drawing.Point.Scale(p1,p2, t);
						return true;
					}
					width -= (1.0-t)*d;
				}
				else	// courbe ?
				{
					pos = Drawing.Point.FromBezier(p1,s1,s2,p2, t);
					double t1 = t;
					double t2 = t;
					double l1 = 0.0;
					double l2 = 0.0;
					Drawing.Point next1, next2;
					next1 = pos;
					while ( true )
					{
						t2 = System.Math.Min(t2+ObjectTextLine.step, 1.0);
						next2 = Drawing.Point.FromBezier(p1,s1,s2,p2, t2);  // segment suivant
						l2 += Drawing.Point.Distance(next1, next2);
						if ( l2 >= width )  // a-t-on trop avanc� ?
						{
							t = t1+(t2-t1)*(width-l1)/(l2-l1);  // approximation lin�aire
							pos = Drawing.Point.Move(next1, next2, width-l1);
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
				if ( i >= this.handles.Count-3 )  // derni�re portion d�pass�e ?
				{
					if ( checkEnd )  return false;
					pos = p2;
					return true;
				}
				t = 0.0;
			}
		}

		protected static double AdvanceString(Drawing.Font font, string text)
		{
			//	Retourne la largeur d'un caract�re.
			System.Diagnostics.Debug.Assert(text.Length == 1);
			if ( text[0] == TextLayout.CodeEndOfText )
			{
				return 0.000001;
			}
			else
			{
				return font.GetTextAdvance(text);
			}
		}

		protected static Drawing.Rectangle AdvanceBounds(Drawing.Font font, string text)
		{
			//	Retourne la bbox d'un caract�re.
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
			//	Initialise l'avance le long des caract�res du texte.
			PropertyTextLine justif = this.PropertyTextLine(1);

			this.textLayout.DefaultFont     = this.PropertyFont(2).GetFont();
			this.textLayout.DefaultFontSize = this.PropertyFont(2).FontSize;
			this.textLayout.DefaultColor    = this.PropertyFont(2).FontColor;
			this.advanceCharArray = this.textLayout.ComputeStructure();

			double width = 0;
			this.advanceMaxAscender = 0;
			for ( int i=0 ; i<this.advanceCharArray.Length ; i++ )
			{
				string s = new string(this.advanceCharArray[i].Character, 1);
				width += ObjectTextLine.AdvanceString(this.advanceCharArray[i].Font, s)*this.advanceCharArray[i].FontSize;
				width += justif.Add*this.advanceCharArray[i].FontSize;

				this.advanceMaxAscender = System.Math.Max(this.advanceMaxAscender, this.advanceCharArray[i].Font.Ascender*this.advanceCharArray[i].FontSize);
			}

			this.advanceRank = 0;
			this.advanceIndex = 0;
			this.advanceBzt = 0.0;
			this.advanceP1 = this.Handle(1).Position;
			this.advanceP2 = this.advanceP1;
			this.advanceLastTop    = Drawing.Point.Empty;
			this.advanceLastBottom = Drawing.Point.Empty;
			this.advanceCheckEnd = true;
			this.advanceFactor = 1.0;

			if ( justif.Horizontal == JustifHorizontal.Center )
			{
				double length = this.GetLength();
				width = System.Math.Max((length-width)/2, 0.0);
				this.Advance(width, true, ref this.advanceIndex, ref this.advanceBzt, ref this.advanceP1);
				this.advanceCheckEnd = (width == 0.0);
			}
			if ( justif.Horizontal == JustifHorizontal.Right )
			{
				double length = this.GetLength();
				width = System.Math.Max(length-width, 0.0);
				this.Advance(width, true, ref this.advanceIndex, ref this.advanceBzt, ref this.advanceP1);
				this.advanceCheckEnd = (width == 0.0);
			}
			if ( justif.Horizontal == JustifHorizontal.Stretch )
			{
				double length = this.GetLength();
				this.advanceFactor = length/width;
				this.advanceCheckEnd = false;
			}

			return true;
		}

		protected bool AdvanceNext(out string character,
								   out Drawing.Font font,
								   out double fontSize,
								   out Drawing.Color fontColor,
								   out Drawing.Point pos,
								   out Drawing.Point ptl,
								   out Drawing.Point pbl,
								   out Drawing.Point ptr,
								   out Drawing.Point pbr,
								   out double angle)
		{
			//	Avance sur le prochain caract�re du texte.
			PropertyTextLine justif = this.PropertyTextLine(1);

			character = "";
			font = null;
			fontSize = 0;
			fontColor = Drawing.Color.Empty;
			pos = new Drawing.Point();
			ptl = new Drawing.Point();
			pbl = new Drawing.Point();
			ptr = new Drawing.Point();
			pbr = new Drawing.Point();
			angle = 0;

			int i = this.advanceRank++;
			if ( i >= this.advanceCharArray.Length )  return false;

			character = new string(this.advanceCharArray[i].Character, 1);
			font = this.advanceCharArray[i].Font;
			fontSize = this.advanceCharArray[i].FontSize * this.advanceFactor;
			fontColor = this.advanceCharArray[i].FontColor;

			this.advanceWidth = ObjectTextLine.AdvanceString(font, character)*fontSize;
			this.advanceWidth += justif.Add*fontSize;
			if ( !this.Advance(this.advanceWidth, this.advanceCheckEnd, ref this.advanceIndex, ref this.advanceBzt, ref this.advanceP2) )
			{
				return false;
			}

			pos = this.advanceP1;
			if ( justif.Offset > 0.0 )
			{
				pos = Drawing.Point.Move(this.advanceP1, this.advanceP2, this.advanceMaxAscender*justif.Offset);
				pos = Drawing.Transform.RotatePointDeg(this.advanceP1, -90, pos);
			}

			angle = Drawing.Point.ComputeAngleDeg(this.advanceP1, this.advanceP2);

			Drawing.Rectangle gb = ObjectTextLine.AdvanceBounds(font, character);
			gb.Top    = font.Ascender;
			gb.Bottom = font.Descender;
			gb.Scale(fontSize);
			gb.Offset(pos);
			pbl = Drawing.Transform.RotatePointDeg(pos, angle, gb.BottomLeft);
			pbr = Drawing.Transform.RotatePointDeg(pos, angle, gb.BottomRight);
			ptl = Drawing.Transform.RotatePointDeg(pos, angle, gb.TopLeft);
			ptr = Drawing.Transform.RotatePointDeg(pos, angle, gb.TopRight);

			if ( !this.advanceLastTop.IsEmpty )
			{
				ptl = Drawing.Point.Projection(this.advanceLastTop, this.advanceLastBottom, ptl);
				pbl = Drawing.Point.Projection(this.advanceLastTop, this.advanceLastBottom, pbl);
			}

			this.advanceP1 = this.advanceP2;
			this.advanceLastTop    = ptr;
			this.advanceLastBottom = pbr;

			return true;
		}

		protected Drawing.Point RankToLinearPos(int rank)
		{
			//	Conversion d'un rank dans le texte en une position lin�aire, c'est-�-dire
			//	une position qui suppose que le texte est droit.
			Drawing.Point lp = Drawing.Point.Empty;
			if ( rank == -1 || !this.AdvanceInit() )  return lp;

			lp.X = 0.00001;  // pour feinter Drawing.Point.IsEmpty !
			lp.Y = 0.00001;
			string			character;
			Drawing.Font	font;
			double			fontSize;
			Drawing.Color	fontColor;
			Drawing.Point	pos, ptl, pbl, ptr, pbr;
			double			angle;
			while ( this.AdvanceNext(out character, out font, out fontSize, out fontColor, out pos, out ptl, out pbl, out ptr, out pbr, out angle) )
			{
				if ( rank == this.advanceRank-1 )  return lp;
				lp.X += this.advanceWidth / this.advanceFactor;
			}
			return lp;
		}

		protected bool DetectTextCurve(Drawing.Point mouse)
		{
			//	D�tecte si la souris est sur un caract�re du texte le long de la courbe multiple.
			if ( !this.AdvanceInit() )  return false;

			string			character;
			Drawing.Font	font;
			double			fontSize;
			Drawing.Color	fontColor;
			Drawing.Point	pos, ptl, pbl, ptr, pbr;
			double			angle;
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

		protected int DetectTextCurveRank(Drawing.Point mouse)
		{
			//	D�tecte sur quel caract�re est la souris le long de la courbe multiple.
			if ( !this.AdvanceInit() )  return -1;

			string			character;
			Drawing.Font	font;
			double			fontSize;
			Drawing.Color	fontColor;
			Drawing.Point	pos, ptl, pbl, ptr, pbr;
			double			angle;
			while ( this.AdvanceNext(out character, out font, out fontSize, out fontColor, out pos, out ptl, out pbl, out ptr, out pbr, out angle) )
			{
				Drawing.Point ptm = (ptl+ptr)/2;
				Drawing.Point pbm = (pbl+pbr)/2;

				InsideSurface inside = new InsideSurface(mouse, 4);
				inside.AddLine(pbl, pbm);
				inside.AddLine(pbm, ptm);
				inside.AddLine(ptm, ptl);
				inside.AddLine(ptl, pbl);
				if ( inside.IsInside() )  return this.advanceRank-1;  // dans la moiti� gauche ?

				inside = new InsideSurface(mouse, 4);
				inside.AddLine(pbm, pbr);
				inside.AddLine(pbr, ptr);
				inside.AddLine(ptr, ptm);
				inside.AddLine(ptm, pbm);
				if ( inside.IsInside() )  return this.advanceRank;  // dans la moiti� droite ?
			}
			return -1;
		}

		protected void BboxTextCurve(ref Drawing.Rectangle bbox)
		{
			//	Calcule la bbox du texte le long de la courbe multiple.
			if ( !this.AdvanceInit() )  return;

			string			character;
			Drawing.Font	font;
			double			fontSize;
			Drawing.Color	fontColor;
			Drawing.Point	pos, ptl, pbl, ptr, pbr;
			double			angle;
			while ( this.AdvanceNext(out character, out font, out fontSize, out fontColor, out pos, out ptl, out pbl, out ptr, out pbr, out angle) )
			{
				bbox.MergeWith(pbl);
				bbox.MergeWith(pbr);
				bbox.MergeWith(ptl);
				bbox.MergeWith(ptr);
			}
		}

		protected void HiliteTextCurve(Drawing.Graphics graphics, IconContext iconContext)
		{
			//	Dessine le texte le long de la courbe multiple.
			if ( !this.AdvanceInit() )  return;

			string			character;
			Drawing.Font	font;
			double			fontSize;
			Drawing.Color	fontColor;
			Drawing.Point	pos, ptl, pbl, ptr, pbr;
			double			angle;
			while ( this.AdvanceNext(out character, out font, out fontSize, out fontColor, out pos, out ptl, out pbl, out ptr, out pbr, out angle) )
			{
				Drawing.Path path = new Drawing.Path();
				path.MoveTo(pbl);
				path.LineTo(pbr);
				path.LineTo(ptr);
				path.LineTo(ptl);
				path.Close();
				graphics.Rasterizer.AddSurface(path);
			}
			graphics.RenderSolid(iconContext.HiliteSurfaceColor);
		}

		protected void DrawTextCurve(Drawing.IPaintPort port, IconContext iconContext)
		{
			//	Dessine le texte le long de la courbe multiple.
			if ( !this.AdvanceInit() )  return;

			this.textLayout.DrawingScale = iconContext.ScaleX;

			int cursorFrom = System.Math.Min(this.textNavigator.Context.CursorFrom, this.textNavigator.Context.CursorTo);
			int cursorTo   = System.Math.Max(this.textNavigator.Context.CursorFrom, this.textNavigator.Context.CursorTo);
			Drawing.Point lastTop    = Drawing.Point.Empty;
			Drawing.Point lastBottom = Drawing.Point.Empty;

			string			character;
			Drawing.Font	font;
			double			fontSize;
			Drawing.Color	fontColor;
			Drawing.Point	pos, ptl, pbl, ptr, pbr;
			double			angle;
			while ( this.AdvanceNext(out character, out font, out fontSize, out fontColor, out pos, out ptl, out pbl, out ptr, out pbr, out angle) )
			{
				int rank = this.advanceRank-1;

				if ( port is Drawing.Graphics &&
					 iconContext.IsFocused &&
					 this.edited &&
					 cursorFrom != cursorTo && rank >= cursorFrom && rank < cursorTo )
				{
					Drawing.Graphics graphics = port as Drawing.Graphics;
					Drawing.Path path = new Drawing.Path();
					path.MoveTo(pbl);
					path.LineTo(pbr);
					path.LineTo(ptr);
					path.LineTo(ptl);
					path.Close();
					graphics.Rasterizer.AddSurface(path);
					graphics.RenderSolid(IconContext.ColorSelectEdit);
				}

				if ( character[0] != TextLayout.CodeEndOfText )
				{
					Drawing.Transform ot = port.Transform;
					port.RotateTransformDeg(angle, pos.X, pos.Y);
					if ( port is Drawing.Graphics )
					{
						Drawing.Graphics graphics = port as Drawing.Graphics;
						graphics.AddText(pos.X, pos.Y, character, font, fontSize);
						graphics.RenderSolid(iconContext.AdaptColor(fontColor));
					}
					else
					{
						port.Color = iconContext.AdaptColor(fontColor);
						port.PaintText(pos.X, pos.Y, character, font, fontSize);
					}
					port.Transform = ot;
				}

				if ( port is Drawing.Graphics &&
					 iconContext.IsFocused &&
					 this.edited &&
					 rank == this.textNavigator.Context.CursorTo )
				{
					Drawing.Graphics graphics = port as Drawing.Graphics;
					graphics.LineWidth = 1.0/iconContext.ScaleX;
					graphics.AddLine(ptl, pbl);
					graphics.RenderSolid(IconContext.ColorFrameEdit);
				}
				
				lastTop    = ptr;
				lastBottom = pbr;
			}

			if ( port is Drawing.Graphics &&
				 iconContext.IsFocused &&
				 this.edited &&
				 this.advanceRank-1 == this.textNavigator.Context.CursorTo )
			{
				Drawing.Graphics graphics = port as Drawing.Graphics;
				graphics.LineWidth = 1.0/iconContext.ScaleX;
				graphics.AddLine(lastTop, lastBottom);
				graphics.RenderSolid(IconContext.ColorFrameEdit);
			}
		}

		public override void DrawGeometry(Drawing.Graphics graphics, IconContext iconContext, IconObjects iconObjects)
		{
			//	Dessine l'objet.
			if ( base.IsFullHide(iconContext) )  return;
			base.DrawGeometry(graphics, iconContext, iconObjects);

			int total = this.TotalHandle;
			if ( total < 6 )  return;

			Drawing.Path path = this.PathBuild();

			if ( this.IsHilite && iconContext.IsEditable && !this.edited )
			{
				this.HiliteTextCurve(graphics, iconContext);
			}

			if ( this.edited && iconContext.IsEditable )  // en cours d'�dition ?
			{
				graphics.Rasterizer.AddOutline(path, 2.0/iconContext.ScaleX);
				graphics.RenderSolid(IconContext.ColorFrameEdit);
			}
			else
			{
				if ( this.IsHilite && iconContext.IsEditable )
				{
					graphics.Rasterizer.AddOutline(path, iconContext.HiliteSize);
					graphics.RenderSolid(iconContext.HiliteOutlineColor);
				}
			}

			this.DrawTextCurve(graphics, iconContext);  // dessine le texte

			if ( (this.IsSelected() || this.textLayout.Text == "") && iconContext.IsEditable )
			{
				graphics.Rasterizer.AddOutline(path, 1.0/iconContext.ScaleX);
				graphics.RenderSolid(Drawing.Color.FromBrightness(0.6));
			}

			if ( this.IsSelected() && iconContext.IsEditable && !this.IsGlobalSelected() && !this.edited )
			{
				double initialWidth = graphics.LineWidth;
				graphics.LineWidth = 1.0/iconContext.ScaleX;

				for ( int i=0 ; i<total ; i+=3 )
				{
					graphics.AddLine(this.Handle(i+0).Position, this.Handle(i+1).Position);
					graphics.AddLine(this.Handle(i+1).Position, this.Handle(i+2).Position);
					graphics.RenderSolid(Drawing.Color.FromBrightness(0.6));
				}

				graphics.LineWidth = initialWidth;
			}
		}


		public override void PrintGeometry(Printing.PrintPort port, IconContext iconContext, IconObjects iconObjects)
		{
			//	Imprime l'objet.
			base.PrintGeometry(port, iconContext, iconObjects);

			int total = this.TotalHandle;
			if ( total < 6 )  return;

			this.DrawTextCurve(port, iconContext);  // dessine le texte
		}

		
		private void HandleTextChanged(object sender)
		{
			this.dirtyBbox = true;
		}


		protected TextLayout				textLayout;
		protected TextNavigator				textNavigator;

		protected Common.Widgets.TextLayout.OneCharStructure[] advanceCharArray;
		protected int						advanceRank;
		protected int						advanceIndex;
		protected double					advanceBzt;
		protected double					advanceWidth;
		protected Drawing.Point				advanceP1;
		protected Drawing.Point				advanceP2;
		protected Drawing.Point				advanceLastTop;
		protected Drawing.Point				advanceLastBottom;
		protected bool						advanceCheckEnd;
		protected double					advanceFactor;
		protected double					advanceMaxAscender;

		protected static readonly double	step = 0.01;
	}
}

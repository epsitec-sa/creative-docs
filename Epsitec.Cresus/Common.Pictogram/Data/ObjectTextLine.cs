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

			PropertyString textString = new PropertyString();
			textString.Type = PropertyType.TextString;
			this.AddProperty(textString);

			PropertyFont textFont = new PropertyFont();
			textFont.Type = PropertyType.TextFont;
			this.AddProperty(textFont);

			PropertyTextLine textLine = new PropertyTextLine();
			textLine.Type = PropertyType.TextLine;
			this.AddProperty(textLine);
		}

		protected override AbstractObject CreateNewObject()
		{
			return new ObjectTextLine();
		}


		// Nom de l'icône.
		public override string IconName
		{
			get { return @"file:images/textline.icon"; }
		}


		// Détecte si la souris est sur l'objet.
		public override bool Detect(Drawing.Point pos)
		{
			if ( this.isHide )  return false;

			Drawing.Rectangle bbox = this.BoundingBox;
			if ( !bbox.Contains(pos) )  return false;

			Drawing.Path path = this.PathBuild();
			if ( AbstractObject.DetectOutline(path, this.minimalWidth, pos) )  return true;
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
		protected int DetectOutline(Drawing.Point pos)
		{
			Drawing.Path path = this.PathBuild();
			int rank = AbstractObject.DetectOutlineRank(path, this.minimalWidth, pos);
			if ( rank != -1 )  rank *= 3;
			return rank;
		}

		// Détecte si la souris est sur l'objet pour l'éditer.
		public override bool DetectEdit(Drawing.Point pos)
		{
			if ( this.isHide )  return false;

			Drawing.Rectangle bbox = this.BoundingBox;
			if ( !bbox.Contains(pos) )  return false;

			Drawing.Path path = this.PathBuild();
			return AbstractObject.DetectOutline(path, this.minimalWidth, pos);
		}


		// Déplace tout l'objet.
		public override void MoveAll(Drawing.Point move, bool all)
		{
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


		// Donne le contenu du menu contextuel.
		public override void ContextMenu(System.Collections.ArrayList list, Drawing.Point pos, int handleRank)
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
						list.Add(item);  // séparateur

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
						list.Add(item);  // séparateur

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

		// Exécute une commande du menu contextuel.
		public override void ContextCommand(string cmd, Drawing.Point pos, int handleRank)
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
			this.dirtyBbox = true;
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
			this.dirtyBbox = true;
		}

		// Passe le point en mode anguleux.
		protected void ContextCorner(int rank)
		{
			this.Handle(rank).ConstrainType = HandleConstrainType.Corner;
		}

		// Ajoute une poignée sans changer l'aspect de la courbe.
		protected void ContextAddHandle(Drawing.Point pos, int rank)
		{
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
				double t = Drawing.Point.Bezier(this.Handle(prev+1).Position, this.Handle(prev+2).Position, this.Handle(next+0).Position, this.Handle(next+1).Position, pos);
				this.Handle(curr+1).Position = Drawing.Point.Bezier(this.Handle(prev+1).Position, this.Handle(prev+2).Position, this.Handle(next+0).Position, this.Handle(next+1).Position, t);
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

		// Supprime une poignée sans changer l'aspect de la courbe.
		protected void ContextSubHandle(Drawing.Point pos, int rank)
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
			this.dirtyBbox = true;
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
			this.dirtyBbox = true;
		}

		// Conversion d'un segement en courbe.
		protected void ContextToCurve(int rank)
		{
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


		// Adapte le point secondaire s'il est en mode "en ligne".
		protected void AdaptPrimaryLine(int rankPrimary, int rankSecondary, out int rankExtremity)
		{
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

		// Déplace une poignée primaire selon les contraintes.
		protected void MovePrimary(int rank, Drawing.Point pos)
		{
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

		// Déplace une poignée secondaire selon les contraintes.
		protected void MoveSecondary(int rankPrimary, int rankSecondary, int rankOpposite, Drawing.Point pos)
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

		// Début du déplacement une poignée.
		public override void MoveHandleStarting(int rank, Drawing.Point pos, IconContext iconContext)
		{
			if ( rank < this.TotalHandle )
			{
				pos = this.Handle((rank/3)*3+1).Position;
				iconContext.ConstrainFixStarting(pos);
			}
		}

		// Déplace une poignée.
		public override void MoveHandleProcess(int rank, Drawing.Point pos, IconContext iconContext)
		{
			if ( rank >= this.handles.Count )  // poignée d'une propriété ?
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
				if ( rank%3 == 0 )  // poignée secondaire ?
				{
					this.MoveSecondary(rank+1, rank, rank+2, pos);
				}
				if ( rank%3 == 2 )  // poignée secondaire ?
				{
					this.MoveSecondary(rank-1, rank, rank-2, pos);
				}
			}
			this.dirtyBbox = true;
		}


		// Début de la création d'un objet.
		public override void CreateMouseDown(Drawing.Point pos, IconContext iconContext)
		{
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

		// Déplacement pendant la création d'un objet.
		public override void CreateMouseMove(Drawing.Point pos, IconContext iconContext)
		{
			iconContext.ConstrainSnapPos(ref pos);
			iconContext.SnapGrid(ref pos);
			this.Handle(3).Position = pos;
			this.Handle(4).Position = pos;
			this.Handle(5).Position = pos;
			this.dirtyBbox = true;
		}

		// Fin de la création d'un objet.
		public override void CreateMouseUp(Drawing.Point pos, IconContext iconContext)
		{
			iconContext.ConstrainSnapPos(ref pos);
			iconContext.SnapGrid(ref pos);
			this.Handle(3).Position = pos;
			this.Handle(4).Position = pos;
			this.Handle(5).Position = pos;
			iconContext.ConstrainDelStarting();
		}

		// Indique si l'objet doit exister. Retourne false si l'objet ne peut
		// pas exister et doit être détruit.
		public override bool CreateIsExist(IconContext iconContext)
		{
			this.Deselect();
			double len = Drawing.Point.Distance(this.Handle(1).Position, this.Handle(4).Position);
			return ( len > this.minimalSize );
		}

		// Indique s'il faut sélectionner l'objet après sa création.
		public override bool EditAfterCreation()
		{
			return true;
		}

		// Indique si un objet est éditable.
		public override bool IsEditable()
		{
			return true;
		}


		// Met à jour le rectangle englobant l'objet.
		protected override void UpdateBoundingBox()
		{
			Drawing.Path path = this.PathBuild();
			this.bboxThin = AbstractObject.ComputeBoundingBox(path);
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
		protected Drawing.Path PathBuild()
		{
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

		// Retourne la longueur totale d'une courbe multiple.
		protected double GetLength()
		{
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
						Drawing.Point next = Drawing.Point.Bezier(p1,s1,s2,p2, t);
						length += Drawing.Point.Distance(pos, next);
						pos = next;
					}
				}
				i += 3;  // courbe suivante
			}
			while ( i < this.handles.Count-3 );

			return length;
		}

		// Avance le long d'une courbe multiple.
		// La courbe est fragmentée en 100 morceaux (ObjectTextLine.step = 0.01)
		// considérés chacuns comme des lignes droites.
		// Retourne false lorsqu'on arrive à la fin.
		// Le mode checkEnd = true ne teste pas l'arrivée à la fin, ce qui est
		// utile en mode JustifHorizontal.Right pour être certain de caser le
		// dernier caractère. Sans cela, des erreurs d'arrondi font qu'il est
		// parfois considéré comme hors du tracé.
		protected bool Advance(double width, bool checkEnd, ref int i, ref double t, ref Drawing.Point pos)
		{
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
					pos = Drawing.Point.Bezier(p1,s1,s2,p2, t);
					double t1 = t;
					double t2 = t;
					double l1 = 0.0;
					double l2 = 0.0;
					Drawing.Point next1, next2;
					next1 = pos;
					while ( true )
					{
						t2 = System.Math.Min(t2+ObjectTextLine.step, 1.0);
						next2 = Drawing.Point.Bezier(p1,s1,s2,p2, t2);  // segment suivant
						l2 += Drawing.Point.Distance(next1, next2);
						if ( l2 >= width )  // a-t-on trop avancé ?
						{
							t = t1+(t2-t1)*(width-l1)/(l2-l1);  // approximation linéaire
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
				if ( i >= this.handles.Count-3 )  // dernière portion dépassée ?
				{
					if ( checkEnd )  return false;
					pos = p2;
					return true;
				}
				t = 0.0;
			}
		}

		// Détecte si la souris est sur un caractère du texte le long de la courbe multiple.
		protected bool DetectTextCurve(Drawing.Point mouse)
		{
			Drawing.Font font = this.PropertyFont(2).GetFont();
			double fs = this.PropertyFont(2).FontSize;

			string text = this.PropertyString(1).String;
			if ( text == "" )  return false;
			PropertyTextLine justif = this.PropertyTextLine(3);

			int index = 0;
			double bzt = 0.0;
			Drawing.Point p1 = this.Handle(1).Position;
			Drawing.Point p2 = p1;
			Drawing.Point pos;

			bool checkEnd = true;
			if ( justif.Horizontal == JustifHorizontal.Center )
			{
				double length = this.GetLength();
				double width = font.GetTextAdvance(text)*fs + justif.Add*fs*text.Length;
				width = System.Math.Max((length-width)/2, 0.0);
				this.Advance(width, true, ref index, ref bzt, ref p1);
				checkEnd = (width == 0.0);
			}
			if ( justif.Horizontal == JustifHorizontal.Right )
			{
				double length = this.GetLength();
				double width = font.GetTextAdvance(text)*fs + justif.Add*fs*text.Length;
				width = System.Math.Max(length-width, 0.0);
				this.Advance(width, true, ref index, ref bzt, ref p1);
				checkEnd = (width == 0.0);
			}
			if ( justif.Horizontal == JustifHorizontal.Stretch )
			{
				double length = this.GetLength();
				double width = font.GetTextAdvance(text)*fs + justif.Add*fs*text.Length;
				fs *= length/width;
				checkEnd = false;
			}

			Drawing.Point lastTop    = new Drawing.Point(0,0);
			Drawing.Point lastBottom = new Drawing.Point(0,0);
			for ( int i=0 ; i<text.Length ; i++ )
			{
				double width = font.GetCharAdvance(text[i])*fs + justif.Add*fs;
				if ( !this.Advance(width, checkEnd, ref index, ref bzt, ref p2) )  break;

				pos = p1;
				if ( justif.Offset > 0.0 )
				{
					pos = Drawing.Point.Move(p1, p2, font.Ascender*fs*justif.Offset);
					pos = Drawing.Transform.RotatePoint(p1, -System.Math.PI/2, pos);
				}

				double angle = Drawing.Point.ComputeAngle(p1, p2);

				Drawing.Rectangle gb = font.GetGlyphBounds(font.GetGlyphIndex(text[i]));
				gb.Top    = font.Ascender;
				gb.Bottom = font.Descender;
				gb.Scale(fs);
				gb.Offset(pos);
				Drawing.Point pbl = Drawing.Transform.RotatePoint(pos, angle, gb.BottomLeft);
				Drawing.Point pbr = Drawing.Transform.RotatePoint(pos, angle, gb.BottomRight);
				Drawing.Point ptl = Drawing.Transform.RotatePoint(pos, angle, gb.TopLeft);
				Drawing.Point ptr = Drawing.Transform.RotatePoint(pos, angle, gb.TopRight);

				if ( i > 0 )
				{
					ptl = lastTop;
					pbl = lastBottom;
				}

				InsideSurface inside = new InsideSurface(mouse, 4);
				inside.AddLine(pbl, pbr);
				inside.AddLine(pbr, ptr);
				inside.AddLine(ptr, ptl);
				inside.AddLine(ptl, pbl);
				if ( inside.IsInside() )  return true;

				lastTop    = ptr;
				lastBottom = pbr;
				p1 = p2;
			}
			return false;
		}

		// Calcule la bbox du texte le long de la courbe multiple.
		protected void BboxTextCurve(ref Drawing.Rectangle bbox)
		{
			Drawing.Font font = this.PropertyFont(2).GetFont();
			double fs = this.PropertyFont(2).FontSize;

			string text = this.PropertyString(1).String;
			if ( text == "" )  return;
			PropertyTextLine justif = this.PropertyTextLine(3);

			int index = 0;
			double bzt = 0.0;
			Drawing.Point p1 = this.Handle(1).Position;
			Drawing.Point p2 = p1;
			Drawing.Point pos;

			bool checkEnd = true;
			if ( justif.Horizontal == JustifHorizontal.Center )
			{
				double length = this.GetLength();
				double width = font.GetTextAdvance(text)*fs + justif.Add*fs*text.Length;
				width = System.Math.Max((length-width)/2, 0.0);
				this.Advance(width, true, ref index, ref bzt, ref p1);
				checkEnd = (width == 0.0);
			}
			if ( justif.Horizontal == JustifHorizontal.Right )
			{
				double length = this.GetLength();
				double width = font.GetTextAdvance(text)*fs + justif.Add*fs*text.Length;
				width = System.Math.Max(length-width, 0.0);
				this.Advance(width, true, ref index, ref bzt, ref p1);
				checkEnd = (width == 0.0);
			}
			if ( justif.Horizontal == JustifHorizontal.Stretch )
			{
				double length = this.GetLength();
				double width = font.GetTextAdvance(text)*fs + justif.Add*fs*text.Length;
				fs *= length/width;
				checkEnd = false;
			}

			Drawing.Point lastTop    = new Drawing.Point(0,0);
			Drawing.Point lastBottom = new Drawing.Point(0,0);
			for ( int i=0 ; i<text.Length ; i++ )
			{
				double width = font.GetCharAdvance(text[i])*fs + justif.Add*fs;
				if ( !this.Advance(width, checkEnd, ref index, ref bzt, ref p2) )  break;

				pos = p1;
				if ( justif.Offset > 0.0 )
				{
					pos = Drawing.Point.Move(p1, p2, font.Ascender*fs*justif.Offset);
					pos = Drawing.Transform.RotatePoint(p1, -System.Math.PI/2, pos);
				}

				double angle = Drawing.Point.ComputeAngle(p1, p2);

				Drawing.Rectangle gb = font.GetGlyphBounds(font.GetGlyphIndex(text[i]));
				gb.Top    = font.Ascender;
				gb.Bottom = font.Descender;
				gb.Scale(fs);
				gb.Offset(pos);
				Drawing.Point pbl = Drawing.Transform.RotatePoint(pos, angle, gb.BottomLeft);
				Drawing.Point pbr = Drawing.Transform.RotatePoint(pos, angle, gb.BottomRight);
				Drawing.Point ptl = Drawing.Transform.RotatePoint(pos, angle, gb.TopLeft);
				Drawing.Point ptr = Drawing.Transform.RotatePoint(pos, angle, gb.TopRight);

				if ( i > 0 )
				{
					ptl = lastTop;
					pbl = lastBottom;
				}

				bbox.MergeWith(pbl);
				bbox.MergeWith(pbr);
				bbox.MergeWith(ptl);
				bbox.MergeWith(ptr);

				lastTop    = ptr;
				lastBottom = pbr;
				p1 = p2;
			}
		}

		// Dessine le texte le long de la courbe multiple.
		protected void HiliteTextCurve(Drawing.Graphics graphics, IconContext iconContext)
		{
			Drawing.Font font = this.PropertyFont(2).GetFont();
			double fs = this.PropertyFont(2).FontSize;

			string text = this.PropertyString(1).String;
			if ( text == "" )  return;
			PropertyTextLine justif = this.PropertyTextLine(3);

			int index = 0;
			double bzt = 0.0;
			Drawing.Point p1 = this.Handle(1).Position;
			Drawing.Point p2 = p1;
			Drawing.Point pos;

			bool checkEnd = true;
			if ( justif.Horizontal == JustifHorizontal.Center )
			{
				double length = this.GetLength();
				double width = font.GetTextAdvance(text)*fs + justif.Add*fs*text.Length;
				width = System.Math.Max((length-width)/2, 0.0);
				this.Advance(width, true, ref index, ref bzt, ref p1);
				checkEnd = (width == 0.0);
			}
			if ( justif.Horizontal == JustifHorizontal.Right )
			{
				double length = this.GetLength();
				double width = font.GetTextAdvance(text)*fs + justif.Add*fs*text.Length;
				width = System.Math.Max(length-width, 0.0);
				this.Advance(width, true, ref index, ref bzt, ref p1);
				checkEnd = (width == 0.0);
			}
			if ( justif.Horizontal == JustifHorizontal.Stretch )
			{
				double length = this.GetLength();
				double width = font.GetTextAdvance(text)*fs + justif.Add*fs*text.Length;
				fs *= length/width;
				checkEnd = false;
			}

			Drawing.Point lastTop    = new Drawing.Point(0,0);
			Drawing.Point lastBottom = new Drawing.Point(0,0);
			for ( int i=0 ; i<text.Length ; i++ )
			{
				double width = font.GetCharAdvance(text[i])*fs + justif.Add*fs;
				if ( !this.Advance(width, checkEnd, ref index, ref bzt, ref p2) )  break;

				pos = p1;
				if ( justif.Offset > 0.0 )
				{
					pos = Drawing.Point.Move(p1, p2, font.Ascender*fs*justif.Offset);
					pos = Drawing.Transform.RotatePoint(p1, -System.Math.PI/2, pos);
				}

				double angle = Drawing.Point.ComputeAngle(p1, p2);

				Drawing.Rectangle gb = font.GetGlyphBounds(font.GetGlyphIndex(text[i]));
				gb.Top    = font.Ascender;
				gb.Bottom = font.Descender;
				gb.Scale(fs);
				gb.Offset(pos);
				Drawing.Point pbl = Drawing.Transform.RotatePoint(pos, angle, gb.BottomLeft);
				Drawing.Point pbr = Drawing.Transform.RotatePoint(pos, angle, gb.BottomRight);
				Drawing.Point ptl = Drawing.Transform.RotatePoint(pos, angle, gb.TopLeft);
				Drawing.Point ptr = Drawing.Transform.RotatePoint(pos, angle, gb.TopRight);

				if ( i > 0 )
				{
					ptl = lastTop;
					pbl = lastBottom;
				}

				Drawing.Path path = new Drawing.Path();
				path.MoveTo(pbl);
				path.LineTo(pbr);
				path.LineTo(ptr);
				path.LineTo(ptl);
				path.Close();
				graphics.Rasterizer.AddSurface(path);

				lastTop    = ptr;
				lastBottom = pbr;
				p1 = p2;
			}
			graphics.RenderSolid(iconContext.HiliteOutlineColor);
		}

		// Dessine le texte le long de la courbe multiple.
		protected void DrawTextCurve(Drawing.Graphics graphics, IconContext iconContext)
		{
			Drawing.Font font = this.PropertyFont(2).GetFont();
			double fs = this.PropertyFont(2).FontSize;

			string text = this.PropertyString(1).String;
			if ( text == "" )  return;
			PropertyTextLine justif = this.PropertyTextLine(3);

			int index = 0;
			double bzt = 0.0;
			Drawing.Point p1 = this.Handle(1).Position;
			Drawing.Point p2 = p1;
			Drawing.Point pos;

			bool checkEnd = true;
			if ( justif.Horizontal == JustifHorizontal.Center )
			{
				double length = this.GetLength();
				double width = font.GetTextAdvance(text)*fs + justif.Add*fs*text.Length;
				width = System.Math.Max((length-width)/2, 0.0);
				this.Advance(width, true, ref index, ref bzt, ref p1);
				checkEnd = (width == 0.0);
			}
			if ( justif.Horizontal == JustifHorizontal.Right )
			{
				double length = this.GetLength();
				double width = font.GetTextAdvance(text)*fs + justif.Add*fs*text.Length;
				width = System.Math.Max(length-width, 0.0);
				this.Advance(width, true, ref index, ref bzt, ref p1);
				checkEnd = (width == 0.0);
			}
			if ( justif.Horizontal == JustifHorizontal.Stretch )
			{
				double length = this.GetLength();
				double width = font.GetTextAdvance(text)*fs + justif.Add*fs*text.Length;
				fs *= length/width;
				checkEnd = false;
			}

			for ( int i=0 ; i<text.Length ; i++ )
			{
				double width = font.GetCharAdvance(text[i])*fs + justif.Add*fs;
				if ( !this.Advance(width, checkEnd, ref index, ref bzt, ref p2) )  break;

				Drawing.Transform ot = graphics.SaveTransform();

				pos = p1;
				if ( justif.Offset > 0.0 )
				{
					pos = Drawing.Point.Move(p1, p2, font.Ascender*fs*justif.Offset);
					pos = Drawing.Transform.RotatePoint(p1, -System.Math.PI/2, pos);
				}

				double angle = Drawing.Point.ComputeAngle(p1, p2);
				angle *= 180.0/System.Math.PI;  // radians -> degrés
				graphics.RotateTransform(angle, pos.X, pos.Y);

				graphics.AddText(pos.X, pos.Y, text.Substring(i,1), font, fs);

				graphics.Transform = ot;
				p1 = p2;
			}
			graphics.RenderSolid(iconContext.AdaptColor(this.PropertyFont(2).FontColor));
		}

		// Dessine l'objet.
		public override void DrawGeometry(Drawing.Graphics graphics, IconContext iconContext, IconObjects iconObjects)
		{
			if ( base.IsFullHide(iconContext) )  return;
			base.DrawGeometry(graphics, iconContext, iconObjects);

			int total = this.TotalHandle;
			if ( total < 6 )  return;

			Drawing.Path path = this.PathBuild();

			if ( this.IsHilite && iconContext.IsEditable )
			{
				this.HiliteTextCurve(graphics, iconContext);
			}

			if ( this.edited && iconContext.IsEditable )  // en cours d'édition ?
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

			if ( (this.IsSelected() || this.PropertyString(1).String == "") && iconContext.IsEditable )
			{
				graphics.Rasterizer.AddOutline(path, 1.0/iconContext.ScaleX);
				graphics.RenderSolid(Drawing.Color.FromBrightness(0.6));
			}

			if ( this.IsSelected() && iconContext.IsEditable && !this.IsGlobalSelected() )
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


		protected static readonly double				step = 0.01;
	}
}

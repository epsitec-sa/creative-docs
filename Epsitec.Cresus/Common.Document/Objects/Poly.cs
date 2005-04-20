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


		// Nom de l'icône.
		public override string IconName
		{
			get { return "manifest:Epsitec.App.DocumentEditor.Images.Poly.icon"; }
		}


		// Détecte si la souris est sur l'objet.
		public override bool Detect(Point pos)
		{
			if ( this.isHide )  return false;

			Drawing.Rectangle bbox = this.BoundingBox;
			if ( !bbox.Contains(pos) )  return false;

			Path pathStart;  bool outlineStart, surfaceStart;
			Path pathEnd;    bool outlineEnd,   surfaceEnd;
			Path pathLine;
			this.PathBuild(null,
						   out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine, false);

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			double width = System.Math.Max(this.PropertyLineMode.Width/2, context.MinimalWidth);
			
			if (                 Geometry.DetectOutline(pathLine,  width, pos) )  return true;
			if ( outlineStart && Geometry.DetectOutline(pathStart, width, pos) )  return true;
			if ( outlineEnd   && Geometry.DetectOutline(pathEnd,   width, pos) )  return true;

			if ( surfaceStart && Geometry.DetectSurface(pathStart, pos) )  return true;
			if ( surfaceEnd   && Geometry.DetectSurface(pathEnd,   pos) )  return true;

			if ( this.PropertyFillGradient.IsVisible() )
			{
				pathLine.Close();
				if ( Geometry.DetectSurface(pathLine, pos) )  return true;
			}

			return false;
		}


		// Début du déplacement d'une poignée.
		public override void MoveHandleStarting(int rank, Point pos, DrawingContext drawingContext)
		{
			base.MoveHandleStarting(rank, pos, drawingContext);

			if ( rank < this.handles.Count )  // poignée de l'objet ?
			{
				drawingContext.ConstrainFlush();
				Handle handle = this.Handle(rank);

				if ( this.TotalMainHandle == 2 )
				{
					if ( handle.PropertyType == Properties.Type.None )
					{
						drawingContext.ConstrainAddHV(this.Handle(0).Position);
						drawingContext.ConstrainAddHV(this.Handle(1).Position);
						drawingContext.ConstrainAddLine(this.Handle(0).Position, this.Handle(1).Position);
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
						drawingContext.ConstrainAddRect(this.Handle(this.NextRank(rank)).Position, this.Handle(this.PrevRank(rank)).Position);
						drawingContext.ConstrainAddLine(this.Handle(rank).Position, this.Handle(this.NextRank(rank)).Position);
						drawingContext.ConstrainAddLine(this.Handle(rank).Position, this.Handle(this.PrevRank(rank)).Position);
						drawingContext.ConstrainAddHV(this.Handle(rank).Position);
						drawingContext.ConstrainAddCircle(this.Handle(this.NextRank(rank)).Position, this.Handle(rank).Position);
						drawingContext.ConstrainAddCircle(this.Handle(this.PrevRank(rank)).Position, this.Handle(rank).Position);
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

			this.Handle(rank).Position = pos;

			this.HandlePropertiesUpdate();
			this.SetDirtyBbox();
			this.TextInfoModif(pos, rank);
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}


		// Déplace globalement l'objet.
		public override void MoveGlobalProcess(Selector selector)
		{
			base.MoveGlobalProcess(selector);
			this.HandlePropertiesUpdate();
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}


		// Donne le contenu du menu contextuel.
		public override void ContextMenu(System.Collections.ArrayList list, Point pos, int handleRank)
		{
			ContextMenuItem item;

			if ( handleRank == -1 )
			{
				if ( this.DetectOutline(pos) == -1 )  return;

				item = new ContextMenuItem();
				list.Add(item);  // séparateur

				item = new ContextMenuItem();
				item.Command = "Object";
				item.Name = "HandleAdd";
				item.Icon = "manifest:Epsitec.App.DocumentEditor.Images.Add.icon";
				item.Text = Res.Strings.Object.Poly.Menu.HandleAdd;
				list.Add(item);
			}
			else
			{
				if ( this.TotalMainHandle > 2 )
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
					item.Text = Res.Strings.Object.Poly.Menu.HandleSym;
					list.Add(item);

					item = new ContextMenuItem();
					item.Command = "Object";
					item.Name = "HandleSimply";
					item.IconActiveNo = "manifest:Epsitec.App.DocumentEditor.Images.RadioNo.icon";
					item.IconActiveYes = "manifest:Epsitec.App.DocumentEditor.Images.RadioYes.icon";
					item.Active = ( type == HandleConstrainType.Simply );
					item.Text = Res.Strings.Object.Poly.Menu.HandleSimply;
					list.Add(item);

					item = new ContextMenuItem();
					list.Add(item);  // séparateur

					if ( !this.PropertyPolyClose.BoolValue &&
						 (this.Handle(handleRank).Type == HandleType.Starting ||
						  this.Handle(this.NextRank(handleRank)).Type == HandleType.Starting) )
					{
						item = new ContextMenuItem();
						item.Command = "Object";
						item.Name = "HandleContinue";
						item.Icon = "manifest:Epsitec.App.DocumentEditor.Images.Add.icon";
						item.Text = Res.Strings.Object.Poly.Menu.HandleContinue;
						list.Add(item);
					}

					item = new ContextMenuItem();
					item.Command = "Object";
					item.Name = "HandleDelete";
					item.Icon = "manifest:Epsitec.App.DocumentEditor.Images.Sub.icon";
					item.Text = Res.Strings.Object.Poly.Menu.HandleDelete;
					list.Add(item);
				}
			}
		}

		// Exécute une commande du menu contextuel.
		public override void ContextCommand(string cmd, Point pos, int handleRank)
		{
			if ( cmd == "HandleAdd" )
			{
				int rank = this.DetectOutline(pos);
				if ( rank == -1 )  return;

				int next = this.NextRank(rank);
				Point p = Point.Projection(this.Handle(rank).Position, this.Handle(next).Position, pos);

				Handle handle = new Handle(this.document);
				handle.Position = p;
				handle.Type = HandleType.Primary;
				handle.IsVisible = true;
				this.HandleInsert(rank+1, handle);
				this.HandlePropertiesUpdate();
			}

			if ( cmd == "HandleContinue" )
			{
				HandleType type = this.Handle(handleRank).Type;
				this.Handle(handleRank).Type = HandleType.Primary;

				int ins, prev1, prev2;
				if ( type == HandleType.Starting )  // insère au début ?
				{
					ins   = handleRank;
					prev1 = handleRank;
					prev2 = handleRank+1;
				}
				else	// insère à la fin ?
				{
					ins   = handleRank+1;
					prev1 = handleRank;
					prev2 = handleRank-1;
				}

				double d = 20.0/this.document.Modifier.ActiveViewer.DrawingContext.ScaleX;
				pos = Point.Move(this.Handle(prev1).Position, this.Handle(prev2).Position, -d);

				Handle handle = new Handle(this.document);
				handle.Position = pos;
				handle.Type = type;
				handle.IsVisible = true;
				this.HandleInsert(ins, handle);
				this.HandlePropertiesUpdate();
			}

			if ( cmd == "HandleDelete" )
			{
				bool starting = (this.Handle(handleRank).Type == HandleType.Starting);
				this.HandleDelete(handleRank);

				// Il doit toujours y avoir une poignée de départ !
				if ( starting )
				{
					this.Handle(handleRank).Type = HandleType.Starting;
				}
				this.HandlePropertiesUpdate();
			}

			if ( cmd == "HandleSym" )
			{
				this.Handle(handleRank).ConstrainType = HandleConstrainType.Symmetric;
			}

			if ( cmd == "HandleSimply" )
			{
				this.Handle(handleRank).ConstrainType = HandleConstrainType.Simply;
			}
		}

		// Détecte si la souris est sur le pourtour de l'objet.
		// Retourne le rank de la poignée de départ, ou -1
		protected int DetectOutline(Point pos)
		{
			this.document.Modifier.OpletQueueEnable = false;
			Properties.Corner corner = this.PropertyCorner;
			Properties.CornerType type = corner.CornerType;
			corner.CornerType = Properties.CornerType.Right;

			Path pathStart;  bool outlineStart, surfaceStart;
			Path pathEnd;    bool outlineEnd,   surfaceEnd;
			Path pathLine;
			this.PathBuild(null,
						   out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine, false);

			corner.CornerType = type;
			this.document.Modifier.OpletQueueEnable = true;

			DrawingContext context = this.document.Modifier.ActiveViewer.DrawingContext;
			double width = System.Math.Max(this.PropertyLineMode.Width/2, context.MinimalWidth);
			return Geometry.DetectOutlineRank(pathLine, width, pos);
		}


		// Début de la création d'un objet.
		public override void CreateMouseDown(Point pos, DrawingContext drawingContext)
		{
			drawingContext.SnapPos(ref pos);

			if ( this.TotalHandle == 0 )
			{
				drawingContext.ConstrainFlush();
				drawingContext.ConstrainAddHV(pos);
				this.ChangePropertyPolyClose(false);
				this.HandleAdd(pos, HandleType.Starting);
				this.HandleAdd(pos, HandleType.Primary);
				this.Handle(0).IsVisible = true;
				this.Handle(1).IsVisible = true;
			}
			else
			{
				double len = Point.Distance(pos, this.Handle(this.TotalHandle-1).Position);
				if ( len > drawingContext.CloseMargin )
				{
					this.HandleAdd(pos, HandleType.Primary);
					this.Handle(this.TotalHandle-1).IsVisible = true;
				}
			}

			this.mouseDown = true;
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// Déplacement pendant la création d'un objet.
		public override void CreateMouseMove(Point pos, DrawingContext drawingContext)
		{
			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapPos(ref pos);

			this.isCreating = true;

			int rank = this.TotalHandle-1;
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

			this.TempMove(pos, drawingContext);

			if ( this.mouseDown )
			{
				this.Handle(rank).Position = pos;
			}
			this.SetDirtyBbox();
			this.TextInfoModif(pos, rank);
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// Fin de la création d'un objet.
		public override void CreateMouseUp(Point pos, DrawingContext drawingContext)
		{
			if ( this.TotalHandle == 2 )
			{
				double len = Point.Distance(this.Handle(0).Position, this.Handle(1).Position);
				if ( len < drawingContext.MinimalSize )
				{
					this.HandleDelete(1);
				}
			}

			this.document.Notifier.NotifyArea(this.BoundingBox);
			drawingContext.SnapPos(ref pos);
			int rank = this.TotalHandle-1;
			this.Handle(rank).Position = pos;
			drawingContext.ConstrainDelStarting();
			drawingContext.ConstrainFlush();
			drawingContext.ConstrainAddHV(pos);
			drawingContext.MagnetClearStarting();
			this.mouseDown = false;
			this.TempCreate(pos, drawingContext);
			this.document.Notifier.NotifyArea(this.BoundingBox);
		}

		// Indique si la création de l'objet est terminée.
		public override bool CreateIsEnding(DrawingContext drawingContext)
		{
			if ( this.TotalHandle < 2 )  return false;

			int rank = this.TotalHandle-1;
			double len = Point.Distance(this.Handle(0).Position, this.Handle(rank).Position);
			if ( len > drawingContext.CloseMargin )  return false;  // pas fini

			this.isCreating = false;
			this.document.Modifier.TextInfoModif = "";
			this.HandleDelete(rank);
			this.ChangePropertyPolyClose(true);

			this.TempDelete();
			this.Handle(0).Type = HandleType.Starting;
			this.Deselect();
			drawingContext.ConstrainDelStarting();

			this.HandlePropertiesCreate();
			this.HandlePropertiesUpdate();
			return true;
		}

		// Indique si l'objet doit exister. Retourne false si l'objet ne peut
		// pas exister et doit être détruit.
		public override bool CreateIsExist(DrawingContext drawingContext)
		{
			return ( this.TotalHandle >= 2 );
		}

		// Termine la création de l'objet. Retourne false si l'objet ne peut
		// pas exister et doit être détruit.
		public override bool CreateEnding(DrawingContext drawingContext)
		{
			this.document.Notifier.NotifyArea(this.BoundingBox);
			this.isCreating = false;
			this.document.Modifier.TextInfoModif = "";

			if ( this.TotalHandle < 2 )  return false;

			this.TempDelete();
			this.Handle(0).Type = HandleType.Starting;
			this.Deselect();
			drawingContext.ConstrainDelStarting();

			this.HandlePropertiesCreate();
			this.HandlePropertiesUpdate();
			return true;
		}

		// Texte des informations de modification.
		protected void TextInfoModif(Point mouse, int rank)
		{
			if ( this.isCreating )
			{
				Point p1, p2;
				if ( this.mouseDown )
				{
					if ( this.TotalHandle < 2 )  return;
					p1 = this.Handle(this.TotalHandle-2).Position;
					p2 = this.Handle(this.TotalHandle-1).Position;
				}
				else
				{
					if ( this.TotalHandle < 1 )  return;
					p1 = this.Handle(this.TotalHandle-1).Position;
					p2 = mouse;
				}
				double len = Point.Distance(p1, p2);
				double angle = Point.ComputeAngleDeg(p1, p2);
				string text = string.Format(Res.Strings.Object.Poly.Info1, this.document.Modifier.RealToString(len), angle.ToString("F1"));
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

		// Retourne un bouton d'action pendant la création.
		public override bool CreateAction(int rank, out string cmd, out string name, out string text)
		{
			if ( rank == 0 )
			{
				cmd  = "Object";
				name = "CreateEnding";
				text = Res.Strings.Object.Poly.Button.CreateEnding;
				return true;
			}
			if ( rank == 1 )
			{
				cmd  = "Object";
				name = "CreateAndSelect";
				text = Res.Strings.Object.Poly.Button.CreateAndSelect;
				return true;
			}
			return base.CreateAction(rank, out cmd, out name, out text);
		}

		// Crée l'objet temporaire pour montrer le nouveau segment.
		protected void TempCreate(Point pos, DrawingContext drawingContext)
		{
			this.tempLineExist = true;
			this.tempLineP1 = pos;
			this.tempLineP2 = pos;
		}

		// Déplace l'objet temporaire pour montrer le nouveau segment.
		protected void TempMove(Point pos, DrawingContext drawingContext)
		{
			this.tempLineP2 = pos;
		}

		// Détruit l'objet temporaire pour montrer le nouveau segment.
		protected void TempDelete()
		{
			this.tempLineExist = false;
		}

		
		// Met à jour le rectangle englobant l'objet.
		protected override void UpdateBoundingBox()
		{
			Path pathStart;  bool outlineStart, surfaceStart;
			Path pathEnd;    bool outlineEnd,   surfaceEnd;
			Path pathLine;
			this.PathBuild(null,
						   out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine, false);

			Path pathTemp = null;
			if ( this.tempLineExist )
			{
				pathTemp = new Path();
				pathTemp.MoveTo(this.tempLineP1);
				pathTemp.LineTo(this.tempLineP2);
			}

			Path[] paths = new Path[4];
			paths[0] = pathLine;
			paths[1] = pathStart;
			paths[2] = pathEnd;
			paths[3] = pathTemp;

			bool[] lineModes = new bool[4];
			lineModes[0] = true;
			lineModes[1] = outlineStart;
			lineModes[2] = outlineEnd;
			lineModes[3] = true;

			bool[] lineColors = new bool[4];
			lineColors[0] = true;
			lineColors[1] = surfaceStart;
			lineColors[2] = surfaceEnd;
			lineColors[3] = true;

			bool[] fillGradients = new bool[4];
			fillGradients[0] = true;
			fillGradients[1] = false;
			fillGradients[2] = false;
			fillGradients[3] = false;

			this.ComputeBoundingBox(paths, lineModes, lineColors, fillGradients);

			for ( int i=0 ; i<this.TotalMainHandle ; i++ )
			{
				this.InflateBoundingBox(this.Handle(i).Position, true);
			}
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
						if ( close )
						{
							pathLine.LineTo(pp1);
							pathLine.Close();
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
				if ( close )
				{
					pathLine.LineTo(pp1);
					pathLine.Close();
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
						if ( close )
						{
							pathLine.Close();
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
				if ( close )
				{
					pathLine.Close();
				}
			}
		}

		// Cherche le rang précédent, en tenant compte
		// des ensembles Starting-Primary(s).
		protected int PrevRank(int rank)
		{
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

		// Cherche le rang suivant, en tenant compte
		// des ensembles Starting-Primary(s).
		protected int NextRank(int rank)
		{
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

		// Crée le chemin d'un coin.
		protected void PathCorner(Path path, Point p1, Point s, Point p2, Properties.Corner corner, bool simply, bool first)
		{
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

		// Dessine l'objet.
		public override void DrawGeometry(Graphics graphics, DrawingContext drawingContext)
		{
			base.DrawGeometry(graphics, drawingContext);

			if ( this.TotalHandle < 1 )  return;

			Path pathStart;  bool outlineStart, surfaceStart;
			Path pathEnd;    bool outlineEnd,   surfaceEnd;
			Path pathLine;
			this.PathBuild(drawingContext,
						   out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine, false);

			this.surfaceAnchor.LineUse = false;
			this.PropertyFillGradient.RenderSurface(graphics, drawingContext, pathLine, this.surfaceAnchor);

			if ( outlineStart )
			{
				this.surfaceAnchor.LineUse = true;
				this.PropertyLineMode.DrawPath(graphics, drawingContext, pathStart, this.PropertyLineColor, this.surfaceAnchor);
			}
			if ( surfaceStart )
			{
				this.surfaceAnchor.LineUse = false;
				this.PropertyLineColor.RenderSurface(graphics, drawingContext, pathStart, this.surfaceAnchor);
			}

			if ( outlineEnd )
			{
				this.surfaceAnchor.LineUse = true;
				this.PropertyLineMode.DrawPath(graphics, drawingContext, pathEnd, this.PropertyLineColor, this.surfaceAnchor);
			}
			if ( surfaceEnd )
			{
				this.surfaceAnchor.LineUse = false;
				this.PropertyLineColor.RenderSurface(graphics, drawingContext, pathEnd, this.surfaceAnchor);
			}

			this.surfaceAnchor.LineUse = true;
			this.PropertyLineMode.DrawPath(graphics, drawingContext, pathLine, this.PropertyLineColor, this.surfaceAnchor);

			if ( this.IsHilite && drawingContext.IsActive )
			{
				if ( this.PropertyFillGradient.IsVisible() )
				{
					graphics.Rasterizer.AddSurface(pathLine);
					graphics.RenderSolid(drawingContext.HiliteSurfaceColor);
				}

				if ( outlineStart )
				{
					this.PropertyLineMode.AddOutline(graphics, pathStart, drawingContext.HiliteSize);
					graphics.RenderSolid(drawingContext.HiliteOutlineColor);
				}
				if ( surfaceStart )
				{
					graphics.Rasterizer.AddSurface(pathStart);
					graphics.RenderSolid(drawingContext.HiliteOutlineColor);
				}

				if ( outlineEnd )
				{
					this.PropertyLineMode.AddOutline(graphics, pathEnd, drawingContext.HiliteSize);
					graphics.RenderSolid(drawingContext.HiliteOutlineColor);
				}
				if ( surfaceEnd )
				{
					graphics.Rasterizer.AddSurface(pathEnd);
					graphics.RenderSolid(drawingContext.HiliteOutlineColor);
				}

				this.PropertyLineMode.AddOutline(graphics, pathLine, drawingContext.HiliteSize);
				graphics.RenderSolid(drawingContext.HiliteOutlineColor);
			}

			if ( this.tempLineExist )
			{
				Path pathTemp = new Path();
				pathTemp.MoveTo(this.tempLineP1);
				pathTemp.LineTo(this.tempLineP2);
				Color color = this.PropertyLineColor.Color1;
				color.A = 0.2;
				this.PropertyLineMode.DrawPath(graphics, drawingContext, pathTemp, color);
			}

			if ( this.IsDrawDash(drawingContext) )
			{
				this.PropertyLineMode.DrawPathDash(graphics, drawingContext, pathLine, this.PropertyLineColor);

				if ( outlineStart )
				{
					this.PropertyLineMode.DrawPathDash(graphics, drawingContext, pathStart, this.PropertyLineColor);
				}

				if ( outlineEnd )
				{
					this.PropertyLineMode.DrawPathDash(graphics, drawingContext, pathEnd, this.PropertyLineColor);
				}
			}
		}

		// Imprime l'objet.
		public override void PrintGeometry(Printing.PrintPort port, DrawingContext drawingContext)
		{
			base.PrintGeometry(port, drawingContext);

			if ( this.TotalHandle < 1 )  return;

			Path pathStart;  bool outlineStart, surfaceStart;
			Path pathEnd;    bool outlineEnd,   surfaceEnd;
			Path pathLine;
			this.PathBuild(drawingContext,
						   out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine, false);

			if ( this.PropertyFillGradient.PaintColor(port, drawingContext) )
			{
				port.PaintSurface(pathLine);
			}

			if ( this.PropertyLineColor.PaintColor(port, drawingContext) )
			{
				if ( outlineStart )
				{
					this.PropertyLineMode.PaintOutline(port, drawingContext, pathStart);
				}
				if ( surfaceStart )
				{
					port.PaintSurface(pathStart);
				}

				if ( outlineEnd )
				{
					this.PropertyLineMode.PaintOutline(port, drawingContext, pathEnd);
				}
				if ( surfaceEnd )
				{
					port.PaintSurface(pathEnd);
				}

				this.PropertyLineMode.PaintOutline(port, drawingContext, pathLine);
			}
		}


		#region CreateFromPath
		// Retourne le chemin géométrique de l'objet pour les constructions
		// magnétiques.
		public override Path GetMagnetPath()
		{
			Path pathStart;  bool outlineStart, surfaceStart;
			Path pathEnd;    bool outlineEnd,   surfaceEnd;
			Path pathLine;
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
			Path pathStart;  bool outlineStart, surfaceStart;
			Path pathEnd;    bool outlineEnd,   surfaceEnd;
			Path pathLine;
			this.PathBuild(null,
						   out pathStart, out outlineStart, out surfaceStart,
						   out pathEnd,   out outlineEnd,   out surfaceEnd,
						   out pathLine, false);

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

		// Crée un polygone à partir d'un chemin quelconque.
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
							this.HandleAdd(current, HandleType.Starting);
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
								firstHandle = this.TotalMainHandle;
							}
							else
							{
								this.HandleAdd(p1, HandleType.Primary);
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
								firstHandle = this.TotalMainHandle;
							}
							else
							{
								this.HandleAdd(p3, HandleType.Primary);
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
								firstHandle = this.TotalMainHandle;
							}
							else
							{
								this.HandleAdd(p3, HandleType.Primary);
								bDo = true;
							}
						}
						current = p3;
						break;

					default:
						if ( (elements[i] & PathElement.FlagClose) != 0 )
						{
							close = true;
						}
						i ++;
						break;
				}
			}
			this.PropertyPolyClose.BoolValue = close;

			return bDo;
		}

		// Finalise la création d'un polygone.
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
		protected Poly(SerializationInfo info, StreamingContext context) : base(info, context)
		{
		}
		#endregion

		
		protected bool				mouseDown = false;
		protected bool				tempLineExist = false;
		protected Point				tempLineP1;
		protected Point				tempLineP2;
	}
}

using Epsitec.Common.Widgets;

namespace Epsitec.Common.Pictogram.Data
{
	public enum ConstrainType
	{
		None,			// aucune
		Normal,			// horizontal, vertical et 45 degrés
		Square,			// uniquement 45 degrés
		Line,			// uniquement horizontal et vertical
		Scale,			// mise à l'échelle
		Rotate,			// rotation
	}

	/// <summary>
	/// La classe IconContext contient le "device contexte" pour les icônes.
	/// </summary>
	public class IconContext
	{
		public IconContext()
		{
		}

		public IAdorner Adorner
		{
			get { return this.adorner; }
			set { this.adorner = value; }
		}

		public Drawing.GlyphPaintStyle GlyphPaintStyle
		{
			get { return this.glyphPaintStyle; }
			set { this.glyphPaintStyle = value; }
		}

		public Drawing.Color UniqueColor
		{
			get { return this.uniqueColor; }
			set { this.uniqueColor = value; }
		}

		public double ScaleX
		{
			get { return this.scaleX; }
			set { this.scaleX = value; }
		}

		public double ScaleY
		{
			get { return this.scaleY; }
			set { this.scaleY = value; }
		}

		public double Zoom
		{
			get { return this.zoom; }
			set { this.zoom = value; }
		}

		public double OriginX
		{
			get { return this.originX; }
			set { this.originX = value; }
		}

		public double OriginY
		{
			get { return this.originY; }
			set { this.originY = value; }
		}


		// Mode "aperçu avant impression".
		public bool PreviewActive
		{
			get { return this.previewActive; }
			set { this.previewActive = value; }
		}

		// Présence de la grille magnétique.
		public bool GridActive
		{
			get { return this.gridActive; }
			set { this.gridActive = value; }
		}

		// Pas de la grille magnétique.
		public Drawing.Point GridStep
		{
			get { return this.gridStep; }
			set { this.gridStep = value; }
		}

		// Force un point sur la grille magnétique.
		public void SnapGrid(ref Drawing.Point pos)
		{
			if ( !this.gridActive )  return;
			pos = pos.GridAlign(new Drawing.Point(this.gridStep.X/2, this.gridStep.Y/2), this.gridStep);
		}

		// Force un point sur la grille magnétique.
		public void SnapGrid(Drawing.Point origin, ref Drawing.Point pos)
		{
			if ( !this.gridActive )  return;
			pos -= origin;
			pos = pos.GridAlign(new Drawing.Point(0, 0), this.gridStep);
			pos += origin;
		}


		// Mode caché à moitié (estomper).
		public bool HideHalfActive
		{
			get { return this.hideHalfActive; }
			set { this.hideHalfActive = value; }
		}


		// Indique si l'objet Drawer est éditable.
		public bool IsEditable
		{
			get { return this.isEditable; }
			set { this.isEditable = value; }
		}

		// Indique si l'icône est estompée.
		public bool IsDimmed
		{
			get { return this.isDimmed; }
			set { this.isDimmed = value; }
		}

		// Indique s'il faut afficher les bbox.
		public bool IsDrawBoxThin
		{
			get { return this.isDrawBoxThin; }
			set { this.isDrawBoxThin = value; }
		}

		// Indique s'il faut afficher les bbox.
		public bool IsDrawBoxGeom
		{
			get { return this.isDrawBoxGeom; }
			set { this.isDrawBoxGeom = value; }
		}

		// Indique s'il faut afficher les bbox.
		public bool IsDrawBoxFull
		{
			get { return this.isDrawBoxFull; }
			set { this.isDrawBoxFull = value; }
		}

		// Taille minimale que doit avoir un objet à sa création.
		public double MinimalSize
		{
			get { return this.minimalSize/this.scaleX; }
		}

		// Epaisseur minimale d'un objet pour la détection du coutour.
		public double MinimalWidth
		{
			get { return this.minimalWidth/this.scaleX; }
		}

		// Marge pour fermer un polygone.
		public double CloseMargin
		{
			get { return this.closeMargin/this.scaleX; }
		}

		// Taille supplémentaire lorsqu'un objet est survolé par la souris.
		public double HiliteSize
		{
			get { return this.hiliteSize/this.scaleX; }
		}

		// Taille d'une poignée.
		public double HandleSize
		{
			get { return this.handleSize/this.scaleX; }
		}

		// Marge à ajouter à la bbox lors du dessin, pour résoudre le cas des poignées
		// qui débordent d'un objet avec un trait mince, et du mode Hilite qui augmente
		// l'épaisseur lors du survol de la souris.
		public double SelectMarginSize
		{
			get { return System.Math.Max(this.handleSize+4, this.hiliteSize)/this.scaleX/2; }
		}

		// Adapte une couleur en fonction de l'état de l'icône.
		public Drawing.Color AdaptColor(Drawing.Color color)
		{
			if ( this.modifyColor != null )
			{
				this.modifyColor(ref color);
			}

			if ( this.adorner != null )
			{
				this.adorner.AdaptPictogramColor(ref color, this.glyphPaintStyle, this.uniqueColor);
			}

			if ( this.isDimmed )  // estompé (hors groupe) ?
			{
				double alpha = color.A;
				double intensity = color.GetBrightness();
				intensity = 0.5+(intensity-0.5)*0.05;  // diminue le contraste
				intensity = System.Math.Min(intensity+0.1, 1.0);  // augmente l'intensité
				color = Drawing.Color.FromBrightness(intensity);
				color.A = alpha*0.2;  // très transparent
			}

			return color;
		}

		public delegate void ModifyColor(ref Drawing.Color color);
		public ModifyColor modifyColor;

		// Couleur lorsqu'un objet est survolé par la souris.
		public Drawing.Color HiliteOutlineColor
		{
			get
			{
				IAdorner adorner = Epsitec.Common.Widgets.Adorner.Factory.Active;
				return Drawing.Color.FromColor(adorner.ColorCaption, 0.8);
			}
		}

		// Couleur lorsqu'un objet est survolé par la souris.
		public Drawing.Color HiliteSurfaceColor
		{
			get
			{
				IAdorner adorner = Epsitec.Common.Widgets.Adorner.Factory.Active;
				return Drawing.Color.FromColor(adorner.ColorCaption, 0.4);
			}
		}


		// Indique si la touche Ctrl est pressés.
		public bool IsCtrl
		{
			get { return this.isCtrl; }
			set { this.isCtrl = value; }
		}

		// Fixe le point initial pour les contraintes.
		public void ConstrainFixStarting(Drawing.Point pos)
		{
			this.constrainStarting = pos;
			this.constrainOrigin = pos;
			this.constrainType = ConstrainType.Normal;
		}

		// Fixe le point initial pour les contraintes.
		public void ConstrainFixStarting(Drawing.Point pos, ConstrainType type)
		{
			this.constrainStarting = pos;
			this.constrainOrigin = pos;
			this.constrainType = type;
		}

		// Fixe le point initial pour les contraintes.
		public void ConstrainFixStarting(Drawing.Point origin, Drawing.Point pos, ConstrainType type)
		{
			this.constrainStarting = pos;
			this.constrainOrigin = origin;
			this.constrainType = type;
		}

		// Retourne une position éventuellement contrainte.
		public void ConstrainSnapPos(ref Drawing.Point pos)
		{
			if ( this.constrainType == ConstrainType.None || !this.isCtrl )  return;

			if ( this.constrainType == ConstrainType.Normal )
			{
				double angle = Drawing.Point.ComputeAngleRad(this.constrainStarting, pos);
				double dist = Drawing.Point.Distance(pos, this.constrainStarting);
				angle = System.Math.Floor((angle+System.Math.PI/8)/(System.Math.PI/4))*(System.Math.PI/4);
				pos = Drawing.Transform.RotatePointRad(this.constrainStarting, angle, this.constrainStarting+new Drawing.Point(dist,0));
			}

			if ( this.constrainType == ConstrainType.Square )
			{
				double angle = Drawing.Point.ComputeAngleRad(this.constrainStarting, pos);
				double dist = Drawing.Point.Distance(pos, this.constrainStarting);
				angle += System.Math.PI/4;
				angle = System.Math.Floor((angle+System.Math.PI/4)/(System.Math.PI/2))*(System.Math.PI/2);
				angle -= System.Math.PI/4;
				pos = Drawing.Transform.RotatePointRad(this.constrainStarting, angle, this.constrainStarting+new Drawing.Point(dist,0));
			}

			if ( this.constrainType == ConstrainType.Line )
			{
				if ( System.Math.Abs(pos.X-this.constrainStarting.X) < System.Math.Abs(pos.Y-this.constrainStarting.Y) )
				{
					pos.X = this.constrainStarting.X;
				}
				else
				{
					pos.Y = this.constrainStarting.Y;
				}
			}

			if ( this.constrainType == ConstrainType.Scale )
			{
				double dist = Drawing.Point.Distance(this.constrainStarting, pos);
				dist = System.Math.Min(dist/4, 10.0/this.scaleX);
				Drawing.Point proj = Drawing.Point.Projection(this.constrainStarting, this.constrainOrigin, pos);
				if ( Drawing.Point.Distance(proj, pos) < dist )
				{
					pos = proj;
				}
				else
				{
					if ( System.Math.Abs(pos.X-this.constrainStarting.X) < System.Math.Abs(pos.Y-this.constrainStarting.Y) )
					{
						pos.X = this.constrainStarting.X;
					}
					else
					{
						pos.Y = this.constrainStarting.Y;
					}
				}
			}
		}

		// Enlève le point initial pour les contraintes.
		public void ConstrainDelStarting()
		{
			this.constrainType = ConstrainType.None;
		}

		// Dessine les contraintes.
		public void DrawConstrain(Drawing.Graphics graphics, Drawing.Size size)
		{
			if ( this.constrainType == ConstrainType.None || !this.isCtrl )  return;

			graphics.LineWidth = 1.0/this.scaleX;
			Drawing.Point pos = this.constrainStarting;
			ConstrainType type = this.constrainType;
			double max = System.Math.Max(size.Width, size.Height);

			if ( type == ConstrainType.Normal || type == ConstrainType.Line || type == ConstrainType.Scale )
			{
				graphics.AddLine(pos.X, -size.Height, pos.X, size.Height);
				graphics.AddLine(-size.Width, pos.Y, size.Width, pos.Y);
				graphics.RenderSolid(IconContext.ColorConstrain);
			}

			if ( type == ConstrainType.Normal || type == ConstrainType.Square )
			{
				Drawing.Point p1 = Drawing.Transform.RotatePointRad(pos, System.Math.PI*0.25, pos+new Drawing.Point(max,0));
				Drawing.Point p2 = Drawing.Transform.RotatePointRad(pos, System.Math.PI*1.25, pos+new Drawing.Point(max,0));
				graphics.AddLine(p1, p2);

				p1 = Drawing.Transform.RotatePointRad(pos, System.Math.PI*0.75, pos+new Drawing.Point(max,0));
				p2 = Drawing.Transform.RotatePointRad(pos, System.Math.PI*1.75, pos+new Drawing.Point(max,0));
				graphics.AddLine(p1, p2);

				graphics.RenderSolid(IconContext.ColorConstrain);
			}

			if ( this.constrainType == ConstrainType.Scale )
			{
				Drawing.Point p1 = Drawing.Point.Move(this.constrainStarting, this.constrainOrigin, max);
				Drawing.Point p2 = Drawing.Point.Move(this.constrainOrigin, this.constrainStarting, max);
				graphics.AddLine(p1, p2);
				graphics.RenderSolid(IconContext.ColorConstrain);
			}
		}


		// Retourne la couleur pour indiquer une sélection multiple.
		static public Drawing.Color ColorMulti
		{
			get { return Drawing.Color.FromARGB(1.0, 1.0, 0.0, 0.0); }  // rouge
		}

		// Retourne la couleur pour indiquer un style.
		static public Drawing.Color ColorStyle
		{
			get { return Drawing.Color.FromARGB(1.0, 0.0, 0.5, 1.0); }  // bleu
		}

		// Retourne la couleur pour indiquer un style.
		static public Drawing.Color ColorStyleBack
		{
			get { return Drawing.Color.FromARGB(0.15, 0.0, 0.5, 1.0); }  // bleu
		}

		// Retourne la couleur du pourtour d'une poignée.
		static public Drawing.Color ColorHandleOutline
		{
			get { return Drawing.Color.FromARGB(1.0, 0.0, 0.0, 0.0); }  // noir
		}

		// Retourne la couleur d'une poignée principale.
		static public Drawing.Color ColorHandleMain
		{
			get { return Drawing.Color.FromARGB(1.0, 1.0, 0.0, 0.0); }  // rouge
		}

		// Retourne la couleur d'une poignée de début/fin.
		static public Drawing.Color ColorHandleStart
		{
			get { return Drawing.Color.FromARGB(1.0, 0.0, 1.0, 0.0); }  // vert
		}

		// Retourne la couleur d'une poignée de propriété.
		static public Drawing.Color ColorHandleProperty
		{
			get { return Drawing.Color.FromARGB(1.0, 0.0, 1.0, 1.0); }  // cyan
		}

		// Retourne la couleur d'une poignée de sélection globale.
		static public Drawing.Color ColorHandleGlobal
		{
			get { return Drawing.Color.FromARGB(1.0, 1.0, 1.0, 1.0); }  // blanc
		}

		// Retourne la couleur pour dessiner une contrainte.
		static public Drawing.Color ColorConstrain
		{
			get { return Drawing.Color.FromARGB(0.5, 1.0, 0.0, 0.0); }  // rouge
		}

		// Retourne la couleur du cadre pendant l'édition.
		static public Drawing.Color ColorFrameEdit
		{
			get { return Drawing.Color.FromARGB(1.0, 1.0, 0.0, 0.0); }  // rouge
		}

		// Retourne la couleur des sélections pendant l'édition.
		static public Drawing.Color ColorSelectEdit
		{
			get { return Drawing.Color.FromARGB(1.0, 1.0, 1.0, 0.0); }  // jaune
		}


		protected IAdorner					adorner;
		protected Drawing.GlyphPaintStyle	glyphPaintStyle;
		protected Drawing.Color				uniqueColor;
		protected double					scaleX = 1;
		protected double					scaleY = 1;
		protected double					zoom = 1;
		protected double					originX = 0;
		protected double					originY = 0;
		protected bool						previewActive = false;
		protected bool						gridActive = false;
		protected Drawing.Point				gridStep = new Drawing.Point(1, 1);
		protected bool						hideHalfActive = true;
		protected bool						isEditable = false;
		protected bool						isDimmed = false;
		protected bool						isDrawBoxThin = false;
		protected bool						isDrawBoxGeom = false;
		protected bool						isDrawBoxFull = false;
		protected double					minimalSize = 3;
		protected double					minimalWidth = 5;
		protected double					closeMargin = 10;
		protected double					hiliteSize = 6;
		protected double					handleSize = 10;
		protected bool						isCtrl = false;
		protected Drawing.Point				constrainStarting;
		protected Drawing.Point				constrainOrigin;
		protected ConstrainType				constrainType;
	}
}

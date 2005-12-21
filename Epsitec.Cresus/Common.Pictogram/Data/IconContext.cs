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


		public bool IsFocused
		{
			//	Indique si Drawer a le focus.
			get { return this.isFocused; }
			set { this.isFocused = value; }
		}

		public bool PreviewActive
		{
			//	Mode "aperçu avant impression".
			get { return this.previewActive; }
			set { this.previewActive = value; }
		}

		public bool GridActive
		{
			//	Présence de la grille magnétique.
			get { return this.gridActive; }
			set { this.gridActive = value; }
		}

		public Drawing.Point GridStep
		{
			//	Pas de la grille magnétique.
			get { return this.gridStep; }
			set { this.gridStep = value; }
		}

		public void SnapGrid(ref Drawing.Point pos)
		{
			//	Force un point sur la grille magnétique.
			if ( !this.gridActive )  return;
			pos = Drawing.Point.GridAlign(pos, new Drawing.Point(this.gridStep.X/2, this.gridStep.Y/2), this.gridStep);
		}

		public void SnapGrid(Drawing.Point origin, ref Drawing.Point pos)
		{
			//	Force un point sur la grille magnétique.
			if ( !this.gridActive )  return;
			pos -= origin;
			pos = Drawing.Point.GridAlign(pos, new Drawing.Point(0, 0), this.gridStep);
			pos += origin;
		}


		public bool HideHalfActive
		{
			//	Mode caché à moitié (estomper).
			get { return this.hideHalfActive; }
			set { this.hideHalfActive = value; }
		}


		public bool IsEditable
		{
			//	Indique si l'objet Drawer est éditable.
			get { return this.isEditable; }
			set { this.isEditable = value; }
		}

		public bool IsDimmed
		{
			//	Indique si l'icône est estompée.
			get { return this.isDimmed; }
			set { this.isDimmed = value; }
		}

		public bool IsDrawBoxThin
		{
			//	Indique s'il faut afficher les bbox.
			get { return this.isDrawBoxThin; }
			set { this.isDrawBoxThin = value; }
		}

		public bool IsDrawBoxGeom
		{
			//	Indique s'il faut afficher les bbox.
			get { return this.isDrawBoxGeom; }
			set { this.isDrawBoxGeom = value; }
		}

		public bool IsDrawBoxFull
		{
			//	Indique s'il faut afficher les bbox.
			get { return this.isDrawBoxFull; }
			set { this.isDrawBoxFull = value; }
		}

		public double MinimalSize
		{
			//	Taille minimale que doit avoir un objet à sa création.
			get { return this.minimalSize/this.scaleX; }
		}

		public double MinimalWidth
		{
			//	Epaisseur minimale d'un objet pour la détection du coutour.
			get { return this.minimalWidth/this.scaleX; }
		}

		public double CloseMargin
		{
			//	Marge pour fermer un polygone.
			get { return this.closeMargin/this.scaleX; }
		}

		public double HiliteSize
		{
			//	Taille supplémentaire lorsqu'un objet est survolé par la souris.
			get { return this.hiliteSize/this.scaleX; }
		}

		public double HandleSize
		{
			//	Taille d'une poignée.
			get { return this.handleSize/this.scaleX; }
		}

		public double SelectMarginSize
		{
			//	Marge à ajouter à la bbox lors du dessin, pour résoudre le cas des poignées
			//	qui débordent d'un objet avec un trait mince, et du mode Hilite qui augmente
			//	l'épaisseur lors du survol de la souris.
			get { return System.Math.Max(this.handleSize+4, this.hiliteSize)/this.scaleX/2; }
		}

		public Drawing.Color AdaptColor(Drawing.Color color)
		{
			//	Adapte une couleur en fonction de l'état de l'icône.
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

		public Drawing.Color HiliteOutlineColor
		{
			//	Couleur lorsqu'un objet est survolé par la souris.
			get
			{
				IAdorner adorner = Epsitec.Common.Widgets.Adorner.Factory.Active;
				return Drawing.Color.FromColor(adorner.ColorCaption, 0.8);
			}
		}

		public Drawing.Color HiliteSurfaceColor
		{
			//	Couleur lorsqu'un objet est survolé par la souris.
			get
			{
				IAdorner adorner = Epsitec.Common.Widgets.Adorner.Factory.Active;
				return Drawing.Color.FromColor(adorner.ColorCaption, 0.4);
			}
		}


		public bool IsCtrl
		{
			//	Indique si la touche Ctrl est pressés.
			get { return this.isCtrl; }
			set { this.isCtrl = value; }
		}

		public void ConstrainFixStarting(Drawing.Point pos)
		{
			//	Fixe le point initial pour les contraintes.
			this.constrainStarting = pos;
			this.constrainOrigin = pos;
			this.constrainType = ConstrainType.Normal;
		}

		public void ConstrainFixStarting(Drawing.Point pos, ConstrainType type)
		{
			//	Fixe le point initial pour les contraintes.
			this.constrainStarting = pos;
			this.constrainOrigin = pos;
			this.constrainType = type;
		}

		public void ConstrainFixStarting(Drawing.Point origin, Drawing.Point pos, ConstrainType type)
		{
			//	Fixe le point initial pour les contraintes.
			this.constrainStarting = pos;
			this.constrainOrigin = origin;
			this.constrainType = type;
		}

		public void ConstrainSnapPos(ref Drawing.Point pos)
		{
			//	Retourne une position éventuellement contrainte.
			if ( this.constrainType == ConstrainType.None || !this.isCtrl )  return;

			if ( this.constrainType == ConstrainType.Normal )
			{
				double angle = Drawing.Point.ComputeAngleDeg(this.constrainStarting, pos);
				double dist = Drawing.Point.Distance(pos, this.constrainStarting);
				angle = System.Math.Floor((angle+45)/90)*90;
				pos = Drawing.Transform.RotatePointDeg(this.constrainStarting, angle, this.constrainStarting+new Drawing.Point(dist,0));
			}

			if ( this.constrainType == ConstrainType.Square )
			{
				double angle = Drawing.Point.ComputeAngleDeg(this.constrainStarting, pos);
				double dist = Drawing.Point.Distance(pos, this.constrainStarting);
				angle += 90;
				angle = System.Math.Floor((angle+90)/180)*180;
				angle -= 90;
				pos = Drawing.Transform.RotatePointDeg(this.constrainStarting, angle, this.constrainStarting+new Drawing.Point(dist,0));
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

		public bool ConstrainDelStarting()
		{
			//	Enlève le point initial pour les contraintes.
			if ( this.constrainType == ConstrainType.None )  return false;
			this.constrainType = ConstrainType.None;
			return true;
		}

		public void DrawConstrain(Drawing.Graphics graphics, Drawing.Size size)
		{
			//	Dessine les contraintes.
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
				Drawing.Point p1 = Drawing.Transform.RotatePointDeg(pos, 180.0*0.25, pos+new Drawing.Point(max,0));
				Drawing.Point p2 = Drawing.Transform.RotatePointDeg(pos, 180.0*1.25, pos+new Drawing.Point(max,0));
				graphics.AddLine(p1, p2);

				p1 = Drawing.Transform.RotatePointDeg(pos, 180.0*0.75, pos+new Drawing.Point(max,0));
				p2 = Drawing.Transform.RotatePointDeg(pos, 180.0*1.75, pos+new Drawing.Point(max,0));
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


		static public Drawing.Color ColorMulti
		{
			//	Retourne la couleur pour indiquer une sélection multiple.
			get { return Drawing.Color.FromARGB(1.0, 1.0, 0.0, 0.0); }  // rouge
		}

		static public Drawing.Color ColorStyle
		{
			//	Retourne la couleur pour indiquer un style.
			get { return Drawing.Color.FromARGB(1.0, 0.0, 0.5, 1.0); }  // bleu
		}

		static public Drawing.Color ColorStyleBack
		{
			//	Retourne la couleur pour indiquer un style.
			get { return Drawing.Color.FromARGB(0.15, 0.0, 0.5, 1.0); }  // bleu
		}

		static public Drawing.Color ColorHandleOutline
		{
			//	Retourne la couleur du pourtour d'une poignée.
			get { return Drawing.Color.FromARGB(1.0, 0.0, 0.0, 0.0); }  // noir
		}

		static public Drawing.Color ColorHandleMain
		{
			//	Retourne la couleur d'une poignée principale.
			get { return Drawing.Color.FromARGB(1.0, 1.0, 0.0, 0.0); }  // rouge
		}

		static public Drawing.Color ColorHandleStart
		{
			//	Retourne la couleur d'une poignée de début/fin.
			get { return Drawing.Color.FromARGB(1.0, 0.0, 1.0, 0.0); }  // vert
		}

		static public Drawing.Color ColorHandleProperty
		{
			//	Retourne la couleur d'une poignée de propriété.
			get { return Drawing.Color.FromARGB(1.0, 0.0, 1.0, 1.0); }  // cyan
		}

		static public Drawing.Color ColorHandleGlobal
		{
			//	Retourne la couleur d'une poignée de sélection globale.
			get { return Drawing.Color.FromARGB(1.0, 1.0, 1.0, 1.0); }  // blanc
		}

		static public Drawing.Color ColorConstrain
		{
			//	Retourne la couleur pour dessiner une contrainte.
			get { return Drawing.Color.FromARGB(0.5, 1.0, 0.0, 0.0); }  // rouge
		}

		static public Drawing.Color ColorFrameEdit
		{
			//	Retourne la couleur du cadre pendant l'édition.
			get { return Drawing.Color.FromARGB(1.0, 1.0, 0.0, 0.0); }  // rouge
		}

		static public Drawing.Color ColorSelectEdit
		{
			//	Retourne la couleur des sélections pendant l'édition.
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
		protected bool						isFocused = false;
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

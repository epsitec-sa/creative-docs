//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// The <c>Tile</c> class paints a widget with an arrow; the logic which defines
	/// how and where the arrow should be painted can be overridden by the derived
	/// classes.
	/// </summary>
	public class Tile : FrameBox
	{
		public Tile()
		{
			this.directArrow = new TileArrow ();
			this.reverseArrow = new TileArrow ();
		}

		/// <summary>
		/// Gets or sets a value indicating whether this tile is read only.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this tile is read only; otherwise, <c>false</c>.
		/// </value>
		public bool IsReadOnly
		{
			get;
			set;
		}


		/// <summary>
		/// Détermine le côté sur lequel s'affiche la flèche. Si la flèche n'est pas dessinée, le côté
		/// correspondant aura un vide.
		/// </summary>
		/// <value>Position de la flèche.</value>
		public Direction ArrowDirection
		{
			get
			{
				return this.arrowDirection;
			}
			set
			{
				if (this.arrowDirection != value)
				{
					this.arrowDirection = value;
					this.Invalidate ();
				}
			}
		}

		public virtual TileArrowMode ArrowMode
		{
			get
			{
				return this.arrowMode;
			}
			set
			{
				if (this.arrowMode != value)
				{
					this.arrowMode = value;
					this.Invalidate ();
				}
			}
		}

		public virtual TileArrow DirectArrow
		{
			get
			{
				return this.directArrow;
			}
		}

		public virtual TileArrow ReverseArrow
		{
			get
			{
				return this.reverseArrow;
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			switch (this.ArrowMode)
			{
				case TileArrowMode.None:
				case TileArrowMode.VisibleDirect:
					this.DirectArrow.Paint (graphics, this.Client.Bounds, this.ArrowMode, this.ArrowDirection);
					break;

				case TileArrowMode.VisibleReverse:
					this.DirectArrow.Paint (graphics, this.Client.Bounds, TileArrowMode.None, this.ArrowDirection);
					break;
			}
		}

		protected override void PaintForegroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			switch (this.ArrowMode)
			{
				case TileArrowMode.VisibleReverse:
					this.ReverseArrow.Paint (graphics, this.Client.Bounds, this.ArrowMode, this.ArrowDirection);
					break;
			}
		}

		#region Colors

		public static Color BorderColor
		{
			get
			{
				IAdorner adorner = Common.Widgets.Adorners.Factory.Active;
				return adorner.ColorBorder;
			}
		}

		public static Color SurfaceSummaryColor
		{
			get
			{
				// TODO: Adapter aux autres adorners
				return Color.FromHexa ("ffffff");
			}
		}

		public static Color SurfaceEditingColor
		{
			get
			{
				// TODO: Adapter aux autres adorners
				return Color.FromHexa ("eef6ff");
			}
		}

		public static Color SurfaceSelectedGroupingColor
		{
			get
			{
				// TODO: Adapter aux autres adorners
				return Color.FromHexa ("d8e8fe");
			}
		}

		public static Color SurfaceSelectedContainerColor
		{
			get
			{
				// TODO: Adapter aux autres adorners
				return Color.FromHexa ("c6defe");
			}
		}

		public static Color SurfaceHilitedColor
		{
			get
			{
				// TODO: Adapter aux autres adorners
				return Color.FromHexa ("ffeec2");  // orange pâle
			}
		}

		public static Color ThicknessHilitedColor
		{
			get
			{
				// TODO: Adapter aux autres adorners
				return Color.FromHexa ("ffc83c");  // orange
			}
		}

		#endregion

		private readonly TileArrow directArrow;
		private readonly TileArrow reverseArrow;
		
		private Direction arrowDirection;
		private TileArrowMode arrowMode;
	}
}

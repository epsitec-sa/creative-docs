//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets
{
	/// <summary>
	/// Ce widget est un conteneur générique, qui peut être sélectionné. L'un de ses côté est
	/// alors une flèche (qui déborde de son Client.Bounds) qui pointe vers son enfant.
	/// </summary>
	public class ContainerTile : ArrowedTile
	{
		public ContainerTile()
		{
		}

		public ContainerTile(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}


		/// <summary>
		/// Gets or sets the parent widget GroupingTile.
		/// </summary>
		/// <value>The GroupingTile widget.</value>
		public GroupingTile ParentGroupingTile
		{
			get;
			set;
		}


		public bool ArrowEnabled
		{
			get
			{
				return this.arrowEnabled;
			}
			set
			{
				this.arrowEnabled = value;
			}
		}

		/// <summary>
		/// Détermine si le widget est sensible au survol de la souris.
		/// </summary>
		/// <value><c>true</c> if [entered sensitivity]; otherwise, <c>false</c>.</value>
		public bool EnteredSensitivity
		{
			get
			{
				return this.enteredSensitivity;
			}
			set
			{
				this.enteredSensitivity = value;
			}
		}


		protected override void OnSelected()
		{
			base.OnSelected ();

			if (this.ParentGroupingTile != null)
			{
				this.ParentGroupingTile.Invalidate ();
			}
		}

		protected override void OnDeselected()
		{
			base.OnDeselected ();

			if (this.ParentGroupingTile != null)
			{
				this.ParentGroupingTile.Invalidate ();
			}
		}


		protected override void OnEntered(MessageEventArgs e)
		{
			base.OnEntered (e);

			if (this.ParentGroupingTile != null)
			{
				this.ParentGroupingTile.Invalidate ();
			}
		}

		protected override void OnExited(MessageEventArgs e)
		{
			base.OnExited (e);

			if (this.ParentGroupingTile != null)
			{
				this.ParentGroupingTile.Invalidate ();
			}
		}


		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			PaintingArrowMode mode = this.GetPaintingArrowMode ();
			Color thicknessColor = this.GetThicknessColor ();
			Color outlineColor = this.GetOutlineColor ();
			Color surfaceColor = this.GetSurfaceColor ();

			this.PaintArrow (graphics, clipRect, mode, thicknessColor, outlineColor, surfaceColor);
		}

		protected override void PaintForegroundImplementation(Graphics graphics, Rectangle clipRect)
		{
		}


		private PaintingArrowMode GetPaintingArrowMode()
		{
			return Widgets.PaintingArrowMode.None;
		}

		private Color GetSurfaceColor()
		{
			if (this.IsEditing)
			{
				return ArrowedTile.BackgroundEditingColor;
			}
			else
			{
				if (this.enteredSensitivity && this.IsEntered && !this.IsSoloContainer)
				{
					return ArrowedTile.BackgroundOutlineHilitedColor;
				}

				if (this.IsSelected && !this.IsSoloContainer)
				{
					return ArrowedTile.BackgroundSelectedContainerColor;
				}
			}

			return Color.Empty;
		}

		private Color GetOutlineColor()
		{
			if (this.IsEditing)
			{
				return Color.Empty;
			}
			else
			{
				if (this.enteredSensitivity && this.IsEntered && !this.IsSoloContainer)
				{
					return ArrowedTile.BorderColor;
				}

				if (this.IsSelected && !this.IsSoloContainer)
				{
					return ArrowedTile.BorderColor;
				}
			}

			return Color.Empty;
		}

		private Color GetThicknessColor()
		{
			return Color.Empty;
		}


#if false
		private bool HasArrow
		{
			get
			{
				//?return this.ArrowEnabled && (this.IsSelected || this.IsEntered) && !this.HasRevertedArrow;
				return false;
			}
		}

		private bool HasRevertedArrow
		{
			get
			{
				return this.ArrowEnabled && this.IsSelected && this.HasMouseHilite;
			}
		}

		private bool HasMouseHilite
		{
			get
			{
				return this.IsEntered && !this.IsSoloContainer && this.enteredSensitivity;
			}
		}
#endif

		private bool ShowSelection
		{
			get
			{
				return this.IsSelected && !this.IsSoloContainer;
			}
		}

		private bool IsSoloContainer
		{
			get
			{
				return this.ParentGroupingTile != null && this.ParentGroupingTile.ChildrenTiles.Count <= 1;
			}
		}


		private bool arrowEnabled;
		private bool enteredSensitivity;
	}
}

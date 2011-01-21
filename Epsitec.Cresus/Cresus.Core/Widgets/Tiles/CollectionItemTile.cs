//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Widgets.Tiles
{
	/// <summary>
	/// The <c>CollectionItemTile</c> is a specialized <see cref="SummaryTile"/> which
	/// provides insertion and removal buttons.
	/// </summary>
	public class CollectionItemTile : SummaryTile
	{
		public CollectionItemTile()
		{
			this.EnableRemoveButtons = true;

			this.CreateUI ();
		}


		public bool EnableRemoveButtons
		{
			get;
			set;
		}

		public override bool IsDragAndDropEnabled
		{
			get
			{
				return true;
			}
		}

		protected override void OnEntered(MessageEventArgs e)
		{
			this.SetButtonVisibility (this.EnableRemoveButtons);
			
			base.OnEntered (e);
		}

		protected override void OnExited(MessageEventArgs e)
		{
			this.SetButtonVisibility (false);
			
			base.OnExited (e);
		}

		protected override TileArrowMode GetPaintingArrowMode()
		{
			if (this.IsCompact)
			{
				if (this.IsEntered || this.Hilited || this.IsSelected)
				{
					return TileArrowMode.Hilited;
				}
			}
			
			return base.GetPaintingArrowMode ();
		}


		private void CreateUI()
		{
			this.CreateRemoveButton ();
		}

		private void CreateRemoveButton()
		{
			this.buttonRemove = new GlyphButton
			{
				Parent        = this,
				ButtonStyle   = Common.Widgets.ButtonStyle.Normal,
				GlyphShape    = Common.Widgets.GlyphShape.Minus,
				Anchor        = AnchorStyles.Right | AnchorStyles.Bottom,
				PreferredSize = new Size (CollectionItemTile.simpleButtonSize, CollectionItemTile.simpleButtonSize),
				Margins       = this.ContainerPadding,
				Visibility    = false,
			};

			this.buttonRemove.Clicked += delegate
			{
				this.OnRemoveClicked ();
			};
		}

		
		private void SetButtonVisibility(bool visibility)
		{
			this.buttonRemove.Visibility = visibility;
		}


		protected virtual void OnRemoveClicked()
		{
			var handler = (EventHandler) this.GetUserEventHandler (CollectionItemTile.RemoveClickedEvent);

			if (handler != null)
			{
				handler (this);
			}
		}

		
		public event EventHandler RemoveClicked
		{
			add
			{
				this.AddUserEventHandler (CollectionItemTile.RemoveClickedEvent, value);
			}
			remove
			{
				this.RemoveUserEventHandler (CollectionItemTile.RemoveClickedEvent, value);
			}
		}


		private const string RemoveClickedEvent = "RemoveClicked";
		

		private static readonly double simpleButtonSize = 16;

		private GlyphButton buttonRemove;
	}
}

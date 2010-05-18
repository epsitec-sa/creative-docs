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
			this.CreateUI ();
		}

		
		protected override void OnEntered(MessageEventArgs e)
		{
			base.OnEntered (e);

			this.buttonAdd.Visibility = true;
			this.buttonRemove.Visibility = true;
		}

		protected override void OnExited(MessageEventArgs e)
		{
			base.OnExited (e);

			this.buttonAdd.Visibility = false;
			this.buttonRemove.Visibility = false;
		}


		private void CreateUI()
		{
			this.CreateAddButton ();
			this.CreateRemoveButton ();
		}

		private void CreateAddButton()
		{
			this.buttonAdd = new GlyphButton
			{
				Parent = this,
				ButtonStyle = Common.Widgets.ButtonStyle.Normal,
				GlyphShape = Common.Widgets.GlyphShape.Plus,
				Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
				PreferredSize = new Size (CollectionItemTile.simpleButtonSize, CollectionItemTile.simpleButtonSize),
				Margins = new Margins (0, TileArrow.Breadth+CollectionItemTile.simpleButtonSize-1, 0, 0),
				Visibility = false,
			};

			//	Parfois, lorsque la touche Tab est pressée, ce handler est appelé suite à un Widget.SimulateClicked(),
			//	ce qui ne devrait pas arriver. Pour palier à cette erreur, je teste la visibilité du widget. En effet,
			//	un vrai événement ne peut pas subvenir si le bouton est caché.
			//	TODO: Pour Pierre, à corriger !
			this.buttonAdd.Clicked +=
				delegate
				{
					this.OnAddClicked ();
				};
		}

		private void CreateRemoveButton()
		{
			this.buttonRemove = new GlyphButton
			{
				Parent = this,
				ButtonStyle = Common.Widgets.ButtonStyle.Normal,
				GlyphShape = Common.Widgets.GlyphShape.Minus,
				Anchor = AnchorStyles.Right | AnchorStyles.Bottom,
				PreferredSize = new Size (CollectionItemTile.simpleButtonSize, CollectionItemTile.simpleButtonSize),
				Margins = new Margins (0, TileArrow.Breadth, 0, 0),
				Visibility = false,
			};

			this.buttonRemove.Clicked +=
				delegate
				{
					this.OnRemoveClicked ();
				};
		}



		protected virtual void OnAddClicked()
		{
			var handler = (EventHandler) this.GetUserEventHandler (CollectionItemTile.AddClickedEvent);

			if (handler != null)
			{
				handler (this);
			}
		}

		protected virtual void OnRemoveClicked()
		{
			var handler = (EventHandler) this.GetUserEventHandler (CollectionItemTile.RemoveClickedEvent);

			if (handler != null)
			{
				handler (this);
			}
		}

		
		public event EventHandler AddClicked
		{
			add
			{
				this.AddUserEventHandler (CollectionItemTile.AddClickedEvent, value);
			}
			remove
			{
				this.RemoveUserEventHandler (CollectionItemTile.AddClickedEvent, value);
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


		private const string AddClickedEvent	= "AddClicked";
		private const string RemoveClickedEvent = "RemoveClicked";
		

		private static readonly double simpleButtonSize = 16;

		private GlyphButton buttonAdd;
		private GlyphButton buttonRemove;
	}
}

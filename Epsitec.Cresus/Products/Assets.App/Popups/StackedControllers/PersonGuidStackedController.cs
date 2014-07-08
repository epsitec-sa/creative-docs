//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups.StackedControllers
{
	public class PersonGuidStackedController : AbstractStackedController
	{
		public PersonGuidStackedController(DataAccessor accessor, StackedControllerDescription description)
			: base (accessor, description)
		{
			var primary      = this.accessor.GetNodeGetter (BaseType.Persons);
			var secondary    = new SortableNodeGetter (primary, this.accessor, BaseType.Persons);
			this.nodeGetter = new SorterNodeGetter (secondary);

			secondary.SetParams (null, SortingInstructions.Default);
			this.nodeGetter.SetParams (SortingInstructions.Default);

			this.dataFiller = new SinglePersonsTreeTableFiller (this.accessor, this.nodeGetter);
		}


		public Guid								Value
		{
			//	Retourne le Guid de l'objet actuellement sélectionné.
			get
			{
				int sel = this.visibleSelectedRow;
				if (sel != -1 && sel < this.nodeGetter.Count)
				{
					return this.nodeGetter[sel].Guid;
				}
				else
				{
					return Guid.Empty;
				}
			}
			//	Sélectionne l'objet ayant un Guid donné. Si la ligne correspondante
			//	est cachée, on est assez malin pour sélectionner la prochaine ligne
			//	visible, vers le haut.
			set
			{
				this.visibleSelectedRow = this.nodeGetter.GetNodes ().Select (x => x.Guid).ToList ().IndexOf (value);
				this.UpdateController ();
			}
		}


		public override int						RequiredHeight
		{
			get
			{
				return this.description.Height;
			}
		}


		public override void CreateUI(Widget parent, int labelWidth, ref int tabIndex)
		{
			this.CreateLabel (parent, labelWidth);
			var controllerFrame = this.CreateControllerFrame (parent);

			this.controller = new NavigationTreeTableController ();
			this.controller.CreateUI (controllerFrame, headerHeight: 0, footerHeight: 0);
			this.controller.AllowsMovement = false;

			TreeTableFiller<SortableNode>.FillColumns (this.controller, this.dataFiller, "Controller.Stacked.Persons");
			this.UpdateController ();

			//	Connexion des événements.
			this.controller.RowClicked += delegate (object sender, int row, int column)
			{
				this.visibleSelectedRow = this.controller.TopVisibleRow + row;

				var node = this.nodeGetter[this.visibleSelectedRow];
				this.Value = node.Guid;

				this.UpdateController ();
				this.OnValueChanged ();
			};

			this.controller.ContentChanged += delegate (object sender, bool crop)
			{
				this.UpdateController (crop);
			};
		}


		private void UpdateController(bool crop = true)
		{
			if (this.controller != null)
			{
				TreeTableFiller<SortableNode>.FillContent (this.controller, this.dataFiller, this.visibleSelectedRow, crop);
			}
		}


		private readonly SorterNodeGetter				nodeGetter;
		private readonly SinglePersonsTreeTableFiller	dataFiller;

		private NavigationTreeTableController			controller;
		private int										visibleSelectedRow;
	}
}
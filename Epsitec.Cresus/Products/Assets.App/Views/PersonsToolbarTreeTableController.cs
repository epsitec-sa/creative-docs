﻿//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Popups;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Views
{
	public class PersonsToolbarTreeTableController : AbstractToolbarTreeTableController<SortableNode>, IDirty
	{
		public PersonsToolbarTreeTableController(DataAccessor accessor)
			: base (accessor)
		{
			this.hasFilter         = false;
			this.hasTreeOperations = false;

			var primary = this.accessor.GetNodeGetter (BaseType.Persons);
			this.secondaryGetter = new SortableNodeGetter (primary, this.accessor, BaseType.Persons);
			this.nodeGetter = new SorterNodeGetter (this.secondaryGetter);

			this.title = "Personnes";
		}


		#region IDirty Members
		public bool InUse
		{
			get;
			set;
		}

		public bool DirtyData
		{
			get;
			set;
		}
		#endregion


		public override void UpdateData()
		{
			this.secondaryGetter.SetParams (null, this.sortingInstructions);
			(this.nodeGetter as SorterNodeGetter).SetParams (this.sortingInstructions);

			this.UpdateController ();
			this.UpdateToolbar ();
		}


		public override Guid SelectedGuid
		{
			//	Retourne le Guid de l'objet actuellement sélectionné.
			get
			{
				int sel = this.VisibleSelectedRow;
				if (sel != -1 && sel < this.nodeGetter.Count)
				{
					return this.nodeGetter[sel].Guid;
				}
				else
				{
					return Guid.Empty;
				}
			}
			//	Sélectionne l'objet ayant un Guid donné.
			set
			{
				this.VisibleSelectedRow = this.nodeGetter.Nodes.ToList ().FindIndex (x => x.Guid == value);
			}
		}


		protected override void CreateNodeFiller()
		{
			this.dataFiller = new PersonsTreeTableFiller (this.accessor, this.nodeGetter);
			TreeTableFiller<SortableNode>.FillColumns (this.controller, this.dataFiller);

			this.controller.AddSortedColumn (0);
		}


		protected override void OnDeselect()
		{
			this.VisibleSelectedRow = -1;
		}

		protected override void OnNew()
		{
			var target = this.toolbar.GetTarget (ToolbarCommand.New);
			this.ShowCreatePopup (target);
		}

		protected override void OnDelete()
		{
			var target = this.toolbar.GetTarget (ToolbarCommand.Delete);

			if (target != null)
			{
				var popup = new YesNoPopup
				{
					Question = "Voulez-vous supprimer la personne sélectionnée ?",
				};

				popup.Create (target, leftOrRight: true);

				popup.ButtonClicked += delegate (object sender, string name)
				{
					if (name == "yes")
					{
						this.accessor.RemoveObject (BaseType.Persons, this.SelectedGuid);
						this.UpdateData ();
						this.OnUpdateAfterDelete ();
					}
				};
			}
		}


		private void ShowCreatePopup(Widget target)
		{
			var popup = new CreatePersonPopup (this.accessor);

			popup.Create (target, leftOrRight: true);

			popup.ButtonClicked += delegate (object sender, string name)
			{
				if (name == "create")
				{
					this.CreateObject (popup.ObjectName, popup.ObjectModel);
				}
			};
		}

		private void CreateObject(string name, Guid model)
		{
			var date = this.accessor.Mandat.StartDate;
			var guid = this.accessor.CreateObject (BaseType.Persons, date, name, Guid.Empty);
			var obj = this.accessor.GetObject (BaseType.Persons, guid);
			System.Diagnostics.Debug.Assert (obj != null);

			if (!model.IsEmpty)
			{
				var objModel = this.accessor.GetObject (BaseType.Persons, model);
				this.accessor.CopyObject (obj, objModel, null);
			}

			this.UpdateData ();

			this.SelectedGuid = guid;
			this.OnUpdateAfterCreate (guid, EventType.Input, Timestamp.Now);  // Timestamp quelconque !
		}


		private SortableNodeGetter secondaryGetter;
	}
}

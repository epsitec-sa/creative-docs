//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Support;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodeGetters;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class MethodsPopup : AbstractPopup
	{
		private MethodsPopup(DataAccessor accessor, Guid selectedGuid)
		{
			this.accessor = accessor;

			this.controller = new NavigationTreeTableController (this.accessor);
			this.filterController = new SimpleFilterController ();

			var primary     = this.accessor.GetNodeGetter (BaseType.Methods);
			var secondary   = new SortableNodeGetter (primary, this.accessor, BaseType.Methods);
			this.nodeGetter = new SorterNodeGetter (secondary);

			secondary.SetParams (null, this.SortingInstructions);
			this.UpdateGetter ();

			this.visibleSelectedRow = this.nodeGetter.GetNodes ().ToList ().FindIndex (x => x.Guid == selectedGuid);

			this.dataFiller = new MethodsTreeTableFiller (this.accessor, this.nodeGetter);
		}


		protected override Size					DialogSize
		{
			get
			{
				return this.GetSize ();
			}
		}

		protected override void CreateUI()
		{
			this.CreateTitle (Res.Strings.Popup.Methods.Title.ToString ());
			this.CreateCloseButton ();

			var frame = new FrameBox
			{
				Parent = this.mainFrameBox,
				Dock   = DockStyle.Fill,
			};

			this.controller.CreateUI (frame, headerHeight: MethodsPopup.headerHeight, footerHeight: 0);
			this.controller.AllowsMovement = false;
			this.controller.AllowsSorting  = false;

			TreeTableFiller<SortableNode>.FillColumns (this.controller, this.dataFiller, "Popup.Methods");
			this.CreateFilterUI (this.mainFrameBox);
			this.UpdateController ();

			//	Connexion des événements.
			this.controller.ContentChanged += delegate (object sender, bool crop)
			{
				this.UpdateController (crop);
			};

			this.controller.RowClicked += delegate (object sender, int row, int column)
			{
				this.visibleSelectedRow = this.controller.TopVisibleRow + row;
				this.UpdateController ();

				var node = this.nodeGetter[this.visibleSelectedRow];
				this.OnNavigate (node.Guid);
				this.ClosePopup ();
			};
		}

		private void CreateFilterUI(Widget parent)
		{
			//	Crée la partie inférieure permettant la saisie d'un filtre.
			this.filterController.CreateUI (parent);

			this.filterController.FilterChanged += delegate
			{
				this.UpdateAfterTextChanged ();
			};
		}


		private void UpdateAfterTextChanged()
		{
			var guid = this.SelectedGuid;

			this.UpdateGetter ();

			if (this.nodeGetter.Count == 1)
			{
				//	S'il n'y a plus qu'un seul contact filtré, c'est lui qui est
				//	sélectionné d'office. Ainsi, au clavier, on peut faire:
				//	"dupond" Return (s'il n'y a d'un seul Dupond dans les personnes)
				this.visibleSelectedRow = 0;
			}
			else
			{
				this.visibleSelectedRow = this.nodeGetter.SearchIndex (guid);
			}

			this.UpdateController ();
		}

		private Guid							SelectedGuid
		{
			get
			{
				if (this.visibleSelectedRow == -1)
				{
					return Guid.Empty;
				}
				else
				{
					var node = this.nodeGetter[this.visibleSelectedRow];
					return node.Guid;
				}
			}
		}


		[Command (Res.CommandIds.Popup.TreeTable.Prev)]
		private void DoPrev()
		{
			//	Appelé lorsque l'utilisateur presse sur "flèche en haut".
			if (this.visibleSelectedRow == -1)
			{
				this.visibleSelectedRow = this.nodeGetter.Count-1;
			}
			else if (this.visibleSelectedRow > 0)
			{
				this.visibleSelectedRow--;
			}

			this.UpdateController ();
		}

		[Command (Res.CommandIds.Popup.TreeTable.Next)]
		private void DoNext()
		{
			//	Appelé lorsque l'utilisateur presse sur "flèche en bas".
			if (this.visibleSelectedRow == -1)
			{
				this.visibleSelectedRow = 0;
			}
			else if (this.visibleSelectedRow < this.nodeGetter.Count-1)
			{
				this.visibleSelectedRow++;
			}

			this.UpdateController ();
		}


		protected override bool					IsAcceptEnable
		{
			get
			{
				return this.visibleSelectedRow != -1;
			}
		}


		private Size GetSize()
		{
			var parent = this.GetParent ();

			//	On calcule une hauteur adaptée au contenu, mais qui ne dépasse
			//	évidement pas la hauteur de la fenêtre principale.
			double h = parent.ActualHeight
					 - MethodsPopup.headerHeight
					 - AbstractScroller.DefaultBreadth;

			//	Utilise au maximum les 4/10 de la hauteur.
			int max = (int) (h*0.4) / MethodsPopup.rowHeight;

			int rows = System.Math.Min (this.nodeGetter.Count, max);
			rows = System.Math.Max (rows, 3);

			int dx = MethodsPopup.popupWidth
				   + (int) AbstractScroller.DefaultBreadth;

			int dy = AbstractPopup.titleHeight
				   + MethodsPopup.headerHeight
				   + rows * MethodsPopup.rowHeight
				   + (int) AbstractScroller.DefaultBreadth
				   + AbstractFilterController.height;

			return new Size (dx, dy);
		}


		private void UpdateGetter()
		{
			this.nodeGetter.SetParams (this.SortingInstructions, this.Filter);
		}

		private bool Filter(Guid guid)
		{
			var exp = this.accessor.GetObject (BaseType.Methods, guid);

			if (this.filterController.HasFilter)
			{
				foreach (var field in DataAccessor.MethodFields)
				{
					var text = ObjectProperties.GetObjectPropertyString (exp, null, field);
					if (this.filterController.IsMatching (text))
					{
						return true;  // visible
					}
				}

				return false;  // caché
			}

			return true;  // visible
		}

		private void UpdateController(bool crop = true)
		{
			TreeTableFiller<SortableNode>.FillContent (this.controller, this.dataFiller, this.visibleSelectedRow, crop);
		}

		private SortingInstructions SortingInstructions
		{
			get
			{
				var field = this.accessor.GetMainStringField (BaseType.Methods);
				return new SortingInstructions (field, SortedType.Ascending, ObjectField.Unknown, SortedType.None);
			}
		}


		#region Events handler
		private void OnNavigate(Guid guid)
		{
			this.Navigate.Raise (this, guid);
		}

		public event EventHandler<Guid> Navigate;
		#endregion


		#region Helpers
		public static void Show(Widget target, DataAccessor accessor, Guid selectedGuid, System.Action<Guid> action)
		{
			//	Affiche le popup pour choisir une expression.
			var popup = new MethodsPopup (accessor, selectedGuid);

			popup.Create (target, leftOrRight: true);

			popup.Navigate += delegate (object sender, Guid guid)
			{
				action (guid);
			};

			popup.Closed += delegate (object sender, ReasonClosure raison)
			{
				if (raison == ReasonClosure.AcceptKey)
				{
					action (popup.SelectedGuid);
				}
			};
		}
		#endregion


		private const int headerHeight  = 22;
		private const int rowHeight     = 18;
		private const int popupWidth    = MethodsTreeTableFiller.totalWidth;

		private readonly DataAccessor					accessor;
		private readonly NavigationTreeTableController	controller;
		private readonly SimpleFilterController			filterController;
		private readonly SorterNodeGetter				nodeGetter;
		private readonly MethodsTreeTableFiller			dataFiller;

		private int										visibleSelectedRow;
	}
}
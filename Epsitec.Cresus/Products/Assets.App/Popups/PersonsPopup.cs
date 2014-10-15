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
	public class PersonsPopup : AbstractPopup
	{
		public PersonsPopup(DataAccessor accessor, Guid selectedGuid)
		{
			this.accessor = accessor;

			this.controller = new NavigationTreeTableController ();

			var primary     = this.accessor.GetNodeGetter (BaseType.Persons);
			var secondary   = new SortableNodeGetter (primary, this.accessor, BaseType.Persons);
			this.nodeGetter = new SorterNodeGetter (secondary);

			secondary.SetParams (null, this.SortingInstructions);
			this.UpdateGetter ();

			this.visibleSelectedRow = this.nodeGetter.GetNodes ().ToList ().FindIndex (x => x.Guid == selectedGuid);

			this.dataFiller = new PersonsTreeTableFiller (this.accessor, this.nodeGetter);
		}


		protected override Size DialogSize
		{
			get
			{
				return this.GetSize ();
			}
		}

		protected override void CreateUI()
		{
			this.CreateTitle (Res.Strings.Popup.Persons.Title.ToString ());
			this.CreateCloseButton ();

			var frame = new FrameBox
			{
				Parent = this.mainFrameBox,
				Dock   = DockStyle.Fill,
			};

			this.controller.CreateUI (frame, headerHeight: PersonsPopup.headerHeight, footerHeight: 0);
			this.controller.AllowsMovement = false;
			this.controller.AllowsSorting  = false;

			TreeTableFiller<SortableNode>.FillColumns (this.controller, this.dataFiller, "Popup.Persons");
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
			var footer = new FrameBox
			{
				Parent          = parent,
				PreferredHeight = PersonsPopup.filterMargins + PersonsPopup.filterHeight + PersonsPopup.filterMargins,
				Dock            = DockStyle.Bottom,
				Padding         = new Margins (PersonsPopup.filterMargins),
				BackColor       = ColorManager.WindowBackgroundColor,
			};

			var text = Res.Strings.Popup.Accounts.Filter.ToString ();

			new StaticText
			{
				Parent           = footer,
				Text             = text,
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleRight,
				PreferredWidth   = 10 + text.GetTextWidth (),
				Margins          = new Margins (0, 10, 0, 0),
				Dock             = DockStyle.Left,
			};

			this.filterField = new TextField
			{
				Parent           = footer,
				Dock             = DockStyle.Fill,
			};

			var clearButton = new IconButton
			{
				Parent        = footer,
				IconUri       = Misc.GetResourceIconUri ("Field.Delete"),
				AutoFocus     = false,
				Dock          = DockStyle.Right,
				PreferredSize = new Size (PersonsPopup.filterHeight, PersonsPopup.filterHeight),
				Margins       = new Margins (2, 0, 0, 0),
				Enable        = false,
			};

			//	Connexions des événements.
			this.filterField.TextChanged += delegate
			{
				this.UpdateAfterTextChanged ();
				clearButton.Enable = !string.IsNullOrEmpty (this.filterField.Text);
			};

			clearButton.Clicked += delegate
			{
				this.filterField.Text = null;
			};

			this.filterField.Focus ();
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

		private Guid SelectedGuid
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


		[Command (Res.CommandIds.PersonsPopup.Prev)]
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

		[Command (Res.CommandIds.PersonsPopup.Next)]
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


		protected override bool IsAcceptEnable
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
					 - PersonsPopup.headerHeight
					 - AbstractScroller.DefaultBreadth;

			//	Utilise au maximum les 4/10 de la hauteur.
			int max = (int) (h*0.4) / PersonsPopup.rowHeight;

			int rows = System.Math.Min (this.nodeGetter.Count, max);
			rows = System.Math.Max (rows, 3);

			int dx = PersonsPopup.popupWidth
				   + (int) AbstractScroller.DefaultBreadth;

			int dy = AbstractPopup.titleHeight
				   + PersonsPopup.headerHeight
				   + rows * PersonsPopup.rowHeight
				   + (int) AbstractScroller.DefaultBreadth
				   + PersonsPopup.filterMargins*2
				   + PersonsPopup.filterHeight;

			return new Size (dx, dy);
		}


		private void UpdateGetter()
		{
			if (this.filterField == null || string.IsNullOrEmpty (this.filterField.Text))
			{
				this.searchEngine = null;
			}
			else
			{
				var definition = SearchDefinition.Default.FromPattern (this.filterField.Text);
				this.searchEngine = new SearchEngine (definition);
			}

			this.nodeGetter.SetParams (this.SortingInstructions, this.Filter);
		}

		private bool Filter(Guid guid)
		{
			var person = this.accessor.GetObject (BaseType.Persons, guid);

			if (this.searchEngine != null)
			{
				foreach (var userField in this.accessor.GlobalSettings.GetUserFields (BaseType.PersonsUserFields))
				{
					if (userField.Type == FieldType.String)
					{
						var text = ObjectProperties.GetObjectPropertyString (person, null, userField.Field);
						if (this.searchEngine.IsMatching (text))
						{
							return true;  // visible
						}
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
				var field = this.accessor.GetMainStringField (BaseType.Persons);
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
			//	Affiche le popup pour choisir une personne.
			var popup = new PersonsPopup (accessor, selectedGuid);

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
		private const int filterMargins = 5;
		private const int filterHeight  = 20;
		private const int popupWidth    = 500;

		private readonly DataAccessor					accessor;
		private readonly NavigationTreeTableController	controller;
		private readonly SorterNodeGetter				nodeGetter;
		private readonly PersonsTreeTableFiller			dataFiller;

		private int										visibleSelectedRow;
		private TextField								filterField;
		private SearchEngine							searchEngine;
	}
}
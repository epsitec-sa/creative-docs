//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Assets.App.Helpers;
using Epsitec.Cresus.Assets.App.Views;
using Epsitec.Cresus.Assets.App.Widgets;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.DataFillers;
using Epsitec.Cresus.Assets.Server.NodesGetter;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.App.Popups
{
	public class CreateObjectPopup : AbstractPopup
	{
		public CreateObjectPopup(DataAccessor accessor, BaseType baseType, Guid selectedGuid)
		{
			this.accessor = accessor;
			this.baseType = baseType;

			this.controller = new NavigationTreeTableController();

			//	GuidNode -> ParentPositionNode -> LevelNode -> TreeNode
			var primaryNodesGetter = this.accessor.GetNodesGetter (this.baseType);
			this.nodesGetter = new TreeNodesGetter (this.accessor, this.baseType, primaryNodesGetter);
			this.nodesGetter.UpdateData ();

			//	On cherche l'objet de regroupement qui sera sélectionné.
			var groupGuid = Guid.Empty;

			var obj = this.accessor.GetObject (this.baseType, selectedGuid);
			if (obj != null)
			{
				var grouping = ObjectCalculator.GetObjectPropertyInt (obj, null, ObjectField.Regroupement);

				if (grouping.HasValue && grouping.Value == 1)
				{
					//	Si l'objet sélectionné est un objet de regroupement, on le prend.
					groupGuid = selectedGuid;
				}
				else
				{
					//	Si l'objet sélectionné n'est pas un objet de regroupement,
					//	on prend son parent.
					groupGuid = ObjectCalculator.GetObjectPropertyGuid (obj, null, ObjectField.Parent);
				}
			}

			this.visibleSelectedRow = this.nodesGetter.Nodes.ToList().FindIndex(x => x.Guid == groupGuid);
			this.UpdateSelectedRow ();

			this.dataFiller = new SingleObjectsTreeTableFiller (this.accessor, this.baseType, this.nodesGetter);

			//	Connexion des événements.
			this.controller.ContentChanged += delegate (object sender, bool crop)
			{
				this.UpdateController (crop);
			};

			this.controller.RowClicked += delegate (object sender, int row)
			{
				this.visibleSelectedRow = this.controller.TopVisibleRow + row;
				this.UpdateController ();
				this.UpdateSelectedRow ();
			};

			this.ObjectDate = new Timestamp (new System.DateTime (System.DateTime.Now.Year, 1, 1), 0).Date;
		}


		public System.DateTime?					ObjectDate;
		public string							ObjectName;
		public bool								ObjectGrouping;
		public Guid								ObjectParent;


		protected override Size DialogSize
		{
			get
			{
				return new Size (CreateObjectPopup.PopupWidth, CreateObjectPopup.PopupHeight);
			}
		}

		protected override void CreateUI()
		{
			this.CreateTitle (this.mainFrameBox);
			this.CreateCloseButton ();

			var line1 = this.CreateFrame (CreateObjectPopup.Margin, 337, CreateObjectPopup.PopupWidth-CreateObjectPopup.Margin*2, CreateObjectPopup.LineHeight);
			var line2 = this.CreateFrame (CreateObjectPopup.Margin, 310, CreateObjectPopup.PopupWidth-CreateObjectPopup.Margin*2, CreateObjectPopup.LineHeight);
			var line3 = this.CreateFrame (CreateObjectPopup.Margin, 283, CreateObjectPopup.PopupWidth-CreateObjectPopup.Margin*2, CreateObjectPopup.LineHeight);
			var line4 = this.CreateFrame (CreateObjectPopup.Margin,  60, CreateObjectPopup.PopupWidth-CreateObjectPopup.Margin*2, 210);
			var line5 = this.CreateFrame (CreateObjectPopup.Margin,  20, CreateObjectPopup.PopupWidth-CreateObjectPopup.Margin*2, 24);

			this.CreateDate      (line1);
			this.CreateName      (line2);
			this.CreateGrouping  (line3);
			this.CreateTreeTable (line4);
			this.CreateButtons   (line5);

			this.UpdateButtons ();
			this.textField.Focus ();
		}

		private void CreateTitle(Widget parent)
		{
			new StaticText
			{
				Parent           = parent,
				Text             = this.Title,
				ContentAlignment = ContentAlignment.MiddleCenter,
				Dock             = DockStyle.Top,
				PreferredHeight  = CreateObjectPopup.TitleHeight - 4,
				BackColor        = ColorManager.SelectionColor,
			};

			new StaticText
			{
				Parent           = parent,
				Dock             = DockStyle.Top,
				PreferredHeight  = 4,
				BackColor        = ColorManager.SelectionColor,
			};
		}

		private void CreateDate(Widget parent)
		{
			new StaticText
			{
				Parent           = parent,
				Text             = "Date d'entrée",
				ContentAlignment = ContentAlignment.MiddleRight,
				Dock             = DockStyle.Left,
				PreferredWidth   = CreateObjectPopup.Indent,
				Margins          = new Margins (0, 10, 0, 0),
			};

			var frame = new FrameBox
			{
				Parent           = parent,
				Dock             = DockStyle.Fill,
				BackColor        = ColorManager.WindowBackgroundColor,
			};

			var dateController = new DateFieldController
			{
				Label      = null,
				LabelWidth = 0,
				Value      = this.ObjectDate,
			};

			dateController.HideAdditionalButtons = true;
			dateController.CreateUI (frame);
			dateController.SetFocus ();

			dateController.ValueEdited += delegate
			{
				this.ObjectDate = dateController.Value;
				this.UpdateButtons ();
			};
		}

		private void CreateName(Widget parent)
		{
			new StaticText
			{
				Parent           = parent,
				Text             = "Nom",
				ContentAlignment = ContentAlignment.MiddleRight,
				Dock             = DockStyle.Left,
				PreferredWidth   = CreateObjectPopup.Indent,
				Margins          = new Margins (0, 10, 0, 0),
			};

			var frame = new FrameBox
			{
				Parent           = parent,
				Dock             = DockStyle.Fill,
				BackColor        = ColorManager.WindowBackgroundColor,
				Padding          = new Margins (2),
			};

			this.textField = new TextField
			{
				Parent           = frame,
				Dock             = DockStyle.Fill,
			};

			this.textField.TextChanged += delegate
			{
				this.ObjectName = this.textField.Text;
				this.UpdateButtons ();
			};
		}

		private void CreateGrouping(Widget parent)
		{
			this.groupingButton = new CheckButton
			{
				Parent           = parent,
				Text             = GroupingLabel,
				AutoFocus        = false,
				Dock             = DockStyle.Fill,
				Margins          = new Margins (CreateObjectPopup.Indent + 10, 0, 0, 0),
			};

			this.groupingButton.ActiveStateChanged += delegate
			{
				this.ObjectGrouping = this.groupingButton.ActiveState == ActiveState.Yes;
				this.lastGrouping = this.ObjectGrouping;
				this.UpdateButtons ();
			};
		}

		private void CreateTreeTable(Widget parent)
		{
			new StaticText
			{
				Parent           = parent,
				Text             = "Dans le groupe",
				ContentAlignment = ContentAlignment.TopRight,
				Dock             = DockStyle.Left,
				PreferredWidth   = CreateObjectPopup.Indent,
				Margins          = new Margins (0, 10, 0, 0),
			};

			var frame = new FrameBox
			{
				Parent           = parent,
				Dock             = DockStyle.Fill,
			};

			this.controller.CreateUI (frame, headerHeight: 0, footerHeight: 0);
			this.controller.AllowsMovement = false;

			TreeTableFiller<TreeNode>.FillColumns (this.dataFiller, this.controller);
			this.UpdateController ();

			this.controller.RowClicked += delegate (object sender, int row)
			{
				var node = this.nodesGetter[row];
				this.ObjectParent = node.Guid;
				this.UpdateButtons ();
			};
		}

		private void CreateButtons(Widget parent)
		{
			this.createButton = new Button
			{
				Parent        = parent,
				Name          = "create",
				ButtonStyle   = ButtonStyle.Icon,
				AutoFocus     = false,
				Dock          = DockStyle.Left,
				PreferredSize = new Size (CreateObjectPopup.PopupWidth/2 - CreateObjectPopup.Margin - 5 + 60, parent.PreferredHeight),
				Margins       = new Margins (0, 5, 0, 0),
			};

			this.cancelButton = new Button
			{
				Parent        = parent,
				Name          = "cancel",
				Text          = "Annuler",
				ButtonStyle   = ButtonStyle.Icon,
				AutoFocus     = false,
				Dock          = DockStyle.Left,
				PreferredSize = new Size (CreateObjectPopup.PopupWidth/2 - CreateObjectPopup.Margin - 5 - 60, parent.PreferredHeight),
				Margins       = new Margins (5, 0, 0, 0),
			};

			this.createButton.Clicked += this.HandleButtonClicked;
			this.cancelButton.Clicked += this.HandleButtonClicked;
		}

		private void HandleButtonClicked(object sender, MessageEventArgs e)
		{
			var button = sender as Button;

			this.ClosePopup ();
			this.OnButtonClicked (button.Name);
		}


		private void UpdateController(bool crop = true)
		{
			this.controller.RowsCount = this.nodesGetter.Count;

			int visibleCount = this.controller.VisibleRowsCount;
			int rowsCount    = this.controller.RowsCount;
			int count        = System.Math.Min (visibleCount, rowsCount);
			int firstRow     = this.controller.TopVisibleRow;
			int selection    = this.visibleSelectedRow;

			if (selection != -1)
			{
				//	La sélection ne peut pas dépasser le nombre maximal de lignes.
				selection = System.Math.Min (selection, rowsCount-1);

				//	Si la sélection est hors de la zone visible, on choisit un autre cadrage.
				if (crop && (selection < firstRow || selection >= firstRow+count))
				{
					firstRow = this.controller.GetTopVisibleRow (selection);
				}

				if (this.controller.TopVisibleRow != firstRow)
				{
					this.controller.TopVisibleRow = firstRow;
				}

				selection -= this.controller.TopVisibleRow;
			}

			TreeTableFiller<TreeNode>.FillContent (this.dataFiller, this.controller, firstRow, count, selection);
		}

		private void UpdateSelectedRow()
		{
			if (this.visibleSelectedRow != -1)
			{
				var node = this.nodesGetter[this.visibleSelectedRow];
				this.ObjectParent = node.Guid;

				var next = this.nodesGetter[this.visibleSelectedRow+1];
				this.groupingEnable = next.Level <= node.Level;
			}
		}

		private void UpdateButtons()
		{
			//-this.groupingButton.Enable = this.groupingEnable;
			//-
			//-if (this.groupingEnable)
			//-{
			//-	this.groupingButton.ActiveState = this.lastGrouping ? ActiveState.Yes : ActiveState.No;
			//-	this.ObjectGrouping = this.lastGrouping;
			//-}
			//-else
			//-{
			//-	var g = this.lastGrouping;
			//-	this.groupingButton.ActiveState = ActiveState.Yes;
			//-	this.ObjectGrouping = true;
			//-	this.lastGrouping = g;
			//-}

			this.createButton.Text = this.GetCreateButtonText (this.ObjectGrouping);

			this.createButton.Enable = this.ObjectDate.HasValue &&
									   !string.IsNullOrEmpty (this.ObjectName) &&
									   !this.ObjectParent.IsEmpty;
		}


		private string Title
		{
			get
			{
				switch (this.baseType)
				{
					case BaseType.Objects:
						return "Création d'un nouveal objet";

					case BaseType.Categories:
						return "Création d'une nouvelle catégorie";

					case BaseType.Groups:
						return "Création d'un nouveau groupe";

					default:
						return null;
				}
			}
		}

		private string GroupingLabel
		{
			get
			{
				switch (this.baseType)
				{
					case BaseType.Objects:
						return "Objet de regroupement";

					case BaseType.Categories:
						return "Catégorie de regroupement";

					case BaseType.Groups:
						return "Groupe de regroupement";

					default:
						return null;
				}
			}
		}

		private string GetCreateButtonText(bool grouping)
		{
			switch (this.baseType)
			{
				case BaseType.Objects:
					if (grouping)
					{
						return "Créer un objet de regroupement";
					}
					else
					{
						return "Créer un objet final";
					}

				case BaseType.Categories:
					if (grouping)
					{
						return "Créer une catégorie de regroupement";
					}
					else
					{
						return "Créer une catégorie finale";
					}

				case BaseType.Groups:
					if (grouping)
					{
						return "Créer un groupe de regroupement";
					}
					else
					{
						return "Créer un groupe final";
					}

				default:
					return null;
			}
		}


		private static readonly int TitleHeight = 24;
		private static readonly int LineHeight  = 2+17+2;
		private static readonly int Indent      = 80;
		private static readonly int PopupWidth  = 330;
		private static readonly int PopupHeight = 400;
		private static readonly int Margin      = 20;

		private readonly DataAccessor					accessor;
		private readonly BaseType						baseType;
		private readonly NavigationTreeTableController	controller;
		private readonly TreeNodesGetter				nodesGetter;
		private readonly SingleObjectsTreeTableFiller	dataFiller;

		private TextField								textField;
		private CheckButton								groupingButton;
		private Button									createButton;
		private Button									cancelButton;
		private int										visibleSelectedRow;
		private bool									groupingEnable;
		private bool									lastGrouping;
	}
}
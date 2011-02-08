//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Drawing;
using Epsitec.Common.Dialogs;

using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Widgets;

using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.DocumentOptionsEditor
{
	public class SelectController
	{
		public SelectController(Core.Business.BusinessContext businessContext, DocumentOptionsEntity documentOptionsEntity, Dictionary<string, string> options)
		{
			this.businessContext       = businessContext;
			this.documentOptionsEntity = documentOptionsEntity;
			this.options               = options;

			this.allOptions   = DocumentOption.GetAllDocumentOptions ().Where (x => !x.IsTitle).ToList ();
			this.titleOptions = DocumentOption.GetAllDocumentOptions ().Where (x =>  x.IsTitle).ToList ();
		}


		public void CreateUI(Widget parent)
		{
			this.table = new CellTable
			{
				Parent = parent,
				PreferredWidth = 310,
				Dock = DockStyle.Left,
				Margins = new Margins (0, 10, 0, 0),
				StyleH = CellArrayStyles.Separator | CellArrayStyles.Header,
				StyleV = CellArrayStyles.ScrollNorm | CellArrayStyles.Separator | CellArrayStyles.SelectLine,
			};

			var rightFrame = new FrameBox
			{
				Parent = parent,
				PreferredWidth = 200,
				Dock = DockStyle.Left,
				Margins = new Margins (0, 10, 0, 0),
			};

			{
				var title = new StaticText
				{
					Parent = rightFrame,
					Text = "Types de documents :",
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, 2),
				};

				var box = new FrameBox
				{
					Parent = rightFrame,
					PreferredHeight = 230,
					DrawFullFrame = true,
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, 10),
					Padding = new Margins (10),
				};

				this.typesText = new StaticText
				{
					Parent = box,
					ContentAlignment = Common.Drawing.ContentAlignment.TopLeft,
					TextBreakMode = Common.Drawing.TextBreakMode.Hyphenate,
					Dock = DockStyle.Fill,
				};
			}

			{
				var title = new StaticText
				{
					Parent = rightFrame,
					Text = "Description :",
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 10, 2),
				};

				var box = new FrameBox
				{
					Parent = rightFrame,
					PreferredHeight = 80,
					DrawFullFrame = true,
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 0, 10),
					Padding = new Margins (10),
				};

				this.descriptionText = new StaticText
				{
					Parent = box,
					ContentAlignment = Common.Drawing.ContentAlignment.TopLeft,
					TextBreakMode = Common.Drawing.TextBreakMode.Hyphenate,
					Dock = DockStyle.Fill,
				};
			}

			{
				var buttonsBox = new FrameBox
				{
					Parent = rightFrame,
					PreferredWidth = 200,
					DrawFullFrame = true,
					Dock = DockStyle.Fill,
					Margins = new Margins (0, 0, 10, 0),
					Padding = new Margins (10),
				};

				this.noButton = new RadioButton
				{
					Parent = buttonsBox,
					Text = "N'utilise pas",
					AutoToggle = false,
					Dock = DockStyle.Top,
				};

				this.yesButton = new RadioButton
				{
					Parent = buttonsBox,
					Text = "Utilise",
					AutoToggle = false,
					Dock = DockStyle.Top,
				};
			}

			//	Conexion des événements.
			this.table.SelectionChanged += delegate
			{
				this.UpdateWidgets ();
			};

			this.noButton.Clicked += delegate
			{
				this.ActionUse (false);
			};

			this.yesButton.Clicked += delegate
			{
				this.ActionUse (true);
			};

			this.UpdateTable ();
			this.UpdateWidgets ();
		}


		public void Update()
		{
		}


		private void ActionUse(bool value)
		{
			int sel = this.table.SelectedRow;

			if (sel != -1)
			{
				this.SetUsedOption (sel, value);
				this.UpdateTable ();
				this.UpdateWidgets ();
			}
		}


		private void UpdateTable()
		{
			int rows = allOptions.Count;
			this.table.SetArraySize (2, rows);

			this.table.SetWidthColumn (0, 240);
			this.table.SetWidthColumn (1, 50);

			this.table.SetHeaderTextH (0, "Description");
			this.table.SetHeaderTextH (1, "Utilisé");

			ContentAlignment[] alignments =
			{
				ContentAlignment.MiddleLeft,
				ContentAlignment.MiddleCenter,
			};

			for (int row=0; row<rows; row++)
			{
				this.table.FillRow (row, alignments);
				this.table.UpdateRow (row, this.GetRowTexts (row));
			}
		}

		private string[] GetRowTexts(int row)
		{
			var result = new string[2];

			result[0] = this.allOptions[row].Description;
			result[1] = this.GetUsedOption (row) ? "<b>Oui</b>" : "Non";

			return result;
		}

		private void UpdateWidgets()
		{
			int sel = this.table.SelectedRow;

			if (sel == -1)
			{
				this.typesText.Text       = null;
				this.descriptionText.Text = null;

				this.noButton.Enable  = false;
				this.yesButton.Enable = false;
			}
			else
			{
				this.typesText.Text       = this.allOptions[sel].DocumentTypeDescription;
				this.descriptionText.Text = this.GetFullDescription (sel);

				this.noButton.Enable  = true;
				this.yesButton.Enable = true;

				bool state = this.GetUsedOption (sel);
				this.noButton.ActiveState  = !state ? ActiveState.Yes : ActiveState.No;
				this.yesButton.ActiveState =  state ? ActiveState.Yes : ActiveState.No;
			}
		}

		private string GetFullDescription(int row)
		{
			string title = "";
			string group = this.allOptions[row].Group;

			if (!string.IsNullOrEmpty (group))
			{
				title = this.titleOptions.Where (x => x.Group == group).Select (x => x.Title).FirstOrDefault ();
				title = string.Concat (title, "<br/>");
			}

			return string.Concat (title, this.allOptions[row].Description);
		}


		private void SetUsedOption(int row, bool value)
		{
			string name = this.allOptions[row].Name;

			if (value)  // utilise l'option ?
			{
				if (!this.GetUsedOption (row))
				{
					this.options.Add (name, this.allOptions[row].DefaultValue);
					this.SetDirty ();
				}
			}
			else  // n'utilise pas l'option ?
			{
				if (this.GetUsedOption (row))
				{
					this.options.Remove (name);
					this.SetDirty ();
				}
			}
		}

		private bool GetUsedOption(int row)
		{
			string name = this.allOptions[row].Name;
			return this.options.ContainsKey (name);
		}


		private void SetDirty()
		{
			this.businessContext.NotifyExternalChanges ();
		}


		private readonly Core.Business.BusinessContext		businessContext;
		private readonly DocumentOptionsEntity				documentOptionsEntity;
		private readonly Dictionary<string, string>			options;
		private readonly List<DocumentOption>				allOptions;
		private readonly List<DocumentOption>				titleOptions;

		private CellTable									table;
		private StaticText									typesText;
		private StaticText									descriptionText;
		private RadioButton									noButton;
		private RadioButton									yesButton;
	}
}

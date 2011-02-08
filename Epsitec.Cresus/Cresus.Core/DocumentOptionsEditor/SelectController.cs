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
				StyleV = CellArrayStyles.ScrollNorm | CellArrayStyles.Separator | CellArrayStyles.SelectLine | CellArrayStyles.SelectMulti,
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
					PreferredHeight = 200,
					DrawFullFrame = true,
					Dock = DockStyle.Top,
					Padding = new Margins (10, 10, 1, 1),
				};

				this.documentTypesText = new StaticText
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
					Margins = new Margins (0, 0, 5, 2),
				};

				var box = new FrameBox
				{
					Parent = rightFrame,
					PreferredHeight = 62,
					DrawFullFrame = true,
					Dock = DockStyle.Top,
					Padding = new Margins (10, 10, 1, 1),
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
				var title = new StaticText
				{
					Parent = rightFrame,
					Text = "Type de la valeur :",
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 5, 2),
				};

				var box = new FrameBox
				{
					Parent = rightFrame,
					PreferredHeight = 18,
					DrawFullFrame = true,
					Dock = DockStyle.Top,
					Padding = new Margins (10, 10, 1, 1),
				};

				this.valueTypeText = new StaticText
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

				this.noneButton = new Button
				{
					Parent = buttonsBox,
					Text = "N'utilise aucun",
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 5, 0),
				};

				this.allButton = new Button
				{
					Parent = buttonsBox,
					Text = "Utilise tout",
					Dock = DockStyle.Top,
					Margins = new Margins (0, 0, 1, 0),
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

			this.noneButton.Clicked += delegate
			{
				this.ActionAll (false);
			};

			this.allButton.Clicked += delegate
			{
				this.ActionAll (true);
			};

			this.UpdateTable ();
			this.UpdateWidgets ();
		}


		public void Update()
		{
		}


		private void ActionUse(bool value)
		{
			for (int row = 0; row < this.allOptions.Count; row++)
			{
				if (table.IsCellSelected (row, 0))
				{
					this.SetUsedOption (row, value);
				}
			}

			this.UpdateTable ();
			this.UpdateWidgets ();
		}

		private void ActionAll(bool value)
		{
			for (int sel = 0; sel < this.allOptions.Count; sel++)
			{
				this.SetUsedOption (sel, value);
			}

			this.UpdateTable ();
			this.UpdateWidgets ();
		}


		private void UpdateTable()
		{
			int rows = this.allOptions.Count;
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
			//?result[1] = this.GetUsedOption (row) ? "<b>Oui</b>" : "Non";
			result[1] = this.GetUsedOption (row) ? "Oui" : "";

			return result;
		}

		private void UpdateWidgets()
		{
			int count = 0;
			int usedCount = 0;
			int selCount = 0;
			int sel = -1;

			for (int row = 0; row < this.allOptions.Count; row++)
			{
				bool state = this.GetUsedOption (row);

				if (state)
				{
					selCount++;
				}

				if (table.IsCellSelected (row, 0))
				{
					if (state)
					{
						usedCount++;
					}

					count++;

					if (sel == -1)
					{
						sel = row;
					}
				}
			}

			bool diff = (count > 1 && usedCount != 0 && count != usedCount);

			if (sel == -1 || count == 0)
			{
				this.documentTypesText.Text = null;
				this.descriptionText.Text   = null;
				this.valueTypeText.Text     = null;
			}
			else if (count > 1)
			{
				this.documentTypesText.FormattedText = Misc.Italic ("Sélection multiple");
				this.descriptionText.FormattedText   = Misc.Italic ("Sélection multiple");
				this.valueTypeText.FormattedText     = Misc.Italic ("Sélection multiple");
			}
			else
			{
				this.documentTypesText.Text = this.allOptions[sel].DocumentTypeDescription;
				this.descriptionText.Text   = this.GetFullDescription (sel);
				this.valueTypeText.Text     = this.GetValueTypeDescription (sel);
			}

			if (count == 0)
			{
				this.noButton.Enable  = false;
				this.yesButton.Enable = false;
			}
			else
			{
				this.noButton.Enable  = true;
				this.yesButton.Enable = true;
			}

			if (sel == -1 || diff)
			{
				this.noButton.ActiveState  = ActiveState.No;
				this.yesButton.ActiveState = ActiveState.No;
			}
			else
			{
				bool state = this.GetUsedOption (sel);
				this.noButton.ActiveState  = !state ? ActiveState.Yes : ActiveState.No;
				this.yesButton.ActiveState =  state ? ActiveState.Yes : ActiveState.No;
			}

			this.noneButton.Enable = (selCount != 0);
			this.allButton.Enable  = (selCount != this.allOptions.Count);
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

		private string GetValueTypeDescription(int row)
		{
			switch (this.allOptions[row].Type)
			{
				case DocumentOptionValueType.Boolean:
					return "Vrai ou faux";

				case DocumentOptionValueType.Distance:
					return "Distance en mm";
			}

			return null;
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
		private StaticText									documentTypesText;
		private StaticText									descriptionText;
		private StaticText									valueTypeText;
		private RadioButton									noButton;
		private RadioButton									yesButton;
		private Button										noneButton;
		private Button										allButton;
	}
}

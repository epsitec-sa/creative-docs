//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Debug;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.IO;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Printers;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.Cresus.Core.Dialogs
{
	/// <summary>
	/// Dialogue pour choisir les imprimantes à utiliser.
	/// </summary>
	class PrinterListDialog : AbstractDialog
	{
		public PrinterListDialog(CoreApplication application)
		{
			this.application = application;

			this.printerList = PrinterListDialog.GetPrinterSettings ();
		}


		protected override Window CreateWindow()
		{
			this.window = new Window ();

			this.SetupWindow ();
			this.SetupWidgets ();
			this.UpdateWidgets ();

			this.window.AdjustWindowSize ();

			return this.window;
		}

		private void SetupWindow()
		{
			this.OwnerWindow = this.application.Window;
			this.window.Icon = this.application.Window.Icon;
			this.window.Text = "Choix des imprimantes disponibles";
			this.window.MakeFixedSizeWindow ();
			this.window.ClientSize = new Size (600, 400);

			window.WindowCloseClicked += delegate
			{
				this.OnDialogClosed ();
				this.CloseDialog ();
			};
		}

		private void SetupWidgets()
		{
			var frame = new FrameBox
			{
				Parent = this.window.Root,
				Dock = DockStyle.Fill,
				Margins = new Margins (10, 10, 10, 0),
			};

			var leftFrame = new FrameBox
			{
				Parent = frame,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 0, 0, 0),
			};

			var rightFrame = new FrameBox
			{
				Parent = frame,
				PreferredWidth = 300,
				Dock = DockStyle.Right,
				Margins = new Margins (10, 0, 0, 0),
			};

			//	Rempli le panneau de gauche.
			var toolbar = new FrameBox
			{
				Parent = leftFrame,
				Dock = DockStyle.Top,
				PreferredHeight = 23,
				Margins = new Margins (0, 0, 0, 2),
			};

			this.addButton = new GlyphButton
			{
				Parent = toolbar,
				GlyphShape = Common.Widgets.GlyphShape.Plus,
				PreferredWidth = 23*2-1,
				Dock = DockStyle.Left,
				Margins = new Margins (0, 0, 0, 0),
			};

			this.removeButton = new GlyphButton
			{
				Parent = toolbar,
				GlyphShape = Common.Widgets.GlyphShape.Minus,
				PreferredWidth = 23,
				Dock = DockStyle.Left,
				Margins = new Margins (2, 0, 0, 0),
			};

			this.moveUpButton = new GlyphButton
			{
				Parent = toolbar,
				GlyphShape = Common.Widgets.GlyphShape.ArrowUp,
				PreferredWidth = 23,
				Dock = DockStyle.Left,
				Margins = new Margins (12, 0, 0, 0),
			};

			this.moveDownButton = new GlyphButton
			{
				Parent = toolbar,
				GlyphShape = Common.Widgets.GlyphShape.ArrowDown,
				PreferredWidth = 23,
				Dock = DockStyle.Left,
				Margins = new Margins (2, 0, 0, 0),
			};

			this.scrollList = new ScrollList
			{
				Parent = leftFrame,
				Dock = DockStyle.Fill,
			};

			//	Rempli le panneau de droite.
			this.logicalLabel = new StaticText
			{
				Parent = rightFrame,
				Text = "Dénomination de l'imprimante :",
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 23+2, UIBuilder.MarginUnderLabel),
			};

			this.logicalField = new TextFieldEx
			{
				DefocusAction = Common.Widgets.DefocusAction.AcceptEdition,
				Parent = rightFrame,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 5),
				TabIndex = 1,
			};

			this.commentLabel = new StaticText
			{
				Parent = rightFrame,
				Text = "Description :",
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderLabel),
			};

			this.commentField = new TextFieldEx
			{
				DefocusAction = Common.Widgets.DefocusAction.AcceptEdition,
				Parent = rightFrame,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 25),
				TabIndex = 2,
			};

			this.physicalLabel = new StaticText
			{
				Parent = rightFrame,
				Text = "Choix de l'imprimante physique :",
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderLabel),
			};

			this.physicalField = new TextFieldCombo
			{
				IsReadOnly = true,
				Parent = rightFrame,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 5),
				TabIndex = 3,
			};

			this.trayLabel = new StaticText
			{
				Parent = rightFrame,
				Text = "Choix du bac de l'imprimante :",
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, UIBuilder.MarginUnderLabel),
			};

			this.trayField = new TextFieldCombo
			{
				IsReadOnly = true,
				Parent = rightFrame,
				Dock = DockStyle.Top,
				Margins = new Margins (0, 0, 0, 25),
				TabIndex = 4,
			};

			//	Rempli le pied de page.
			var footer = new FrameBox
			{
				Parent = this.window.Root,
				PreferredHeight = 20,
				Dock = DockStyle.Bottom,
				Margins = new Margins (10, 10, 10, 10),
			};

			this.errorInfo = new StaticText
			{
				Parent = footer,
				ContentAlignment = Common.Drawing.ContentAlignment.MiddleCenter,
				Dock = DockStyle.Fill,
				Margins = new Margins (0, 10, 0, 0),
			};

			this.cancelButton = new Button ()
			{
				Parent = footer,
				Text = "Annuler",
				Dock = DockStyle.Right,
				Margins = new Margins (10, 0, 0, 0),
				TabIndex = 101,
			};

			this.acceptButton = new Button ()
			{
				Parent = footer,
				Text = "D'accord",
				Dock = DockStyle.Right,
				TabIndex = 100,
			};

			//	Initialise les tooltips.
			ToolTip.Default.SetToolTip (this.addButton,      "Ajoute une nouvelle impriante");
			ToolTip.Default.SetToolTip (this.removeButton,   "Supprime l'imprimante");
			ToolTip.Default.SetToolTip (this.moveUpButton,   "Montre l'imprimante dans la liste");
			ToolTip.Default.SetToolTip (this.moveDownButton, "Descend l'imprimante dans la liste");

			//	Connection des événements.
			this.addButton.Clicked += delegate
			{
				this.ActionAddPrinter ();
			};

			this.removeButton.Clicked += delegate
			{
				this.ActionRemovePrinter ();
			};

			this.moveUpButton.Clicked += delegate
			{
				this.ActionMoveUpPrinter ();
			};

			this.moveDownButton.Clicked += delegate
			{
				this.ActionMoveDownPrinter ();
			};

			this.scrollList.SelectedItemChanged += delegate
			{
				this.ActionSelectPrinter ();
			};

			this.logicalField.AcceptingEdition += delegate
			{
				this.ActionLogicalChanged ();
			};

			this.commentField.AcceptingEdition += delegate
			{
				this.ActionCommentChanged ();
			};

			this.physicalField.SelectedItemChanged += delegate
			{
				this.ActionPhysicalChanged ();
			};

			this.trayField.SelectedItemChanged += delegate
			{
				this.ActionTrayChanged ();
			};

			this.acceptButton.Clicked += delegate
			{
				PrinterListDialog.SetPrinterSettings (this.printerList);

				this.Result = DialogResult.Accept;
				this.CloseDialog ();
			};

			this.cancelButton.Clicked += delegate
			{
				this.Result = DialogResult.Cancel;
				this.CloseDialog ();
			};

			this.UpdateScrollList ();
			this.UpdatePhysicalField ();
			this.UpdateWidgets ();
		}

		private void UpdateWidgets()
		{
			int sel = this.SelectedIndex;

			this.addButton.Enable = true;
			this.removeButton.Enable = sel != -1;
			this.moveUpButton.Enable = sel > 0;
			this.moveDownButton.Enable = sel != -1 && sel < this.printerList.Count-1;

			this.logicalLabel.Enable = sel != -1;
			this.logicalField.Enable = sel != -1;

			this.commentLabel.Enable = sel != -1;
			this.commentField.Enable = sel != -1;

			this.physicalLabel.Enable = sel != -1;
			this.physicalField.Enable = sel != -1;
			
			this.trayLabel.Enable = sel != -1;
			this.trayField.Enable = sel != -1;

			if (sel == -1)
			{
				this.logicalField.Text  = null;
				this.commentField.Text  = null;
				this.physicalField.Text = null;
				this.trayField.Text     = null;
			}
			else
			{
				Printer printer = this.SelectedPrinter;

				bool printerChanged = (this.physicalField.Text != printer.PhysicalName);

				this.logicalField.Text  = printer.LogicalName;
				this.commentField.Text  = printer.Comment;
				this.physicalField.Text = printer.PhysicalName;
				this.trayField.Text     = printer.Tray;

				if (printerChanged)
				{
					this.UpdateTrayField ();
				}
			}

			string error = this.GetError ();

			if (string.IsNullOrEmpty (error))
			{
				this.errorInfo.Text = null;
				this.errorInfo.BackColor = Color.Empty;

				this.acceptButton.Enable = true;
			}
			else
			{
				this.errorInfo.Text = error;
				this.errorInfo.BackColor = Color.FromName ("Gold");

				this.acceptButton.Enable = false;
			}
		}

		private void UpdateScrollList(int? sel=null)
		{
			if (!sel.HasValue)
			{
				sel = this.SelectedIndex;
			}

			this.ignoreChange = true;

			this.scrollList.Items.Clear ();

			foreach (var printer in this.printerList)
			{
				this.scrollList.Items.Add (printer.NiceDescription);
			}

			this.ignoreChange = false;

			if (sel.HasValue)
			{
				this.SelectedIndex = sel.Value;
				this.scrollList.ShowSelected (ScrollShowMode.Extremity);
			}
		}

		private void UpdatePhysicalField()
		{
			List<string> physicalNames = PrinterSettings.InstalledPrinters.ToList ();

			this.physicalField.Items.Clear ();
			foreach (var physicalName in physicalNames)
			{
				if (!string.IsNullOrWhiteSpace (physicalName))
				{
					this.physicalField.Items.Add (physicalName);
				}
			}
		}

		private void UpdateTrayField()
		{
			Printer printer = this.SelectedPrinter;
			List<string> trayNames = PrinterListDialog.GetTrayList(printer);

			this.trayField.Items.Clear ();
			foreach (var trayName in trayNames)
			{
				if (!string.IsNullOrWhiteSpace (trayName))
				{
					this.trayField.Items.Add (trayName);
				}
			}
		}


		private void ActionAddPrinter()
		{
			int sel = this.SelectedIndex;

			if (sel == -1)
			{
				sel = this.printerList.Count;  // insère à la fin
			}
			else
			{
				sel++;  // insère après la ligne sélectionnée
			}

			Printer printer = new Printer (this.DefaultLogicalName, null, null, false, 0, 0, null);

			this.printerList.Insert (sel, printer);

			this.UpdateScrollList (sel);
			this.UpdateWidgets ();

			this.logicalField.SelectAll ();
			this.logicalField.Focus ();
		}

		private void ActionRemovePrinter()
		{
			int sel = this.SelectedIndex;

			this.printerList.RemoveAt (sel);

			if (sel >= this.printerList.Count)
			{
				sel = this.printerList.Count-1;
			}

			this.UpdateScrollList (sel);
			this.UpdateWidgets ();
		}

		private void ActionMoveUpPrinter()
		{
			int sel = this.SelectedIndex;

			var t = this.printerList[sel];
			this.printerList.RemoveAt (sel);
			this.printerList.Insert (sel-1, t);

			this.UpdateScrollList (sel-1);
			this.UpdateWidgets ();
		}

		private void ActionMoveDownPrinter()
		{
			int sel = this.SelectedIndex;

			var t = this.printerList[sel];
			this.printerList.RemoveAt (sel);
			this.printerList.Insert (sel+1, t);

			this.UpdateScrollList (sel+1);
			this.UpdateWidgets ();
		}

		private void ActionSelectPrinter()
		{
			if (this.ignoreChange)
			{
				return;
			}

			this.UpdateWidgets ();
		}

		private void ActionLogicalChanged()
		{
			if (this.ignoreChange)
			{
				return;
			}

			int sel = this.SelectedIndex;

			if (this.printerList[sel].LogicalName != this.logicalField.Text)
			{
				this.printerList[sel].LogicalName = this.logicalField.Text;
				this.UpdateScrollList (sel);
				this.UpdateWidgets ();
			}
		}

		private void ActionCommentChanged()
		{
			if (this.ignoreChange)
			{
				return;
			}

			int sel = this.SelectedIndex;

			if (this.printerList[sel].Comment != this.commentField.Text)
			{
				this.printerList[sel].Comment = this.commentField.Text;
				this.UpdateScrollList (sel);
				this.UpdateWidgets ();
			}
		}

		private void ActionPhysicalChanged()
		{
			if (this.ignoreChange)
			{
				return;
			}

			int sel = this.SelectedIndex;

			if (this.printerList[sel].PhysicalName != this.physicalField.Text)
			{
				this.printerList[sel].PhysicalName = this.physicalField.Text;
				this.UpdateScrollList (sel);
				this.UpdateWidgets ();
			}
		}

		private void ActionTrayChanged()
		{
			if (this.ignoreChange)
			{
				return;
			}

			int sel = this.SelectedIndex;

			if (this.printerList[sel].Tray != this.trayField.Text)
			{
				this.printerList[sel].Tray = this.trayField.Text;
				this.UpdateScrollList (sel);
				this.UpdateWidgets ();
			}
		}


		private static List<string> GetTrayList(Printer printer)
		{
			List<string> trayNames = new List<string> ();

			if (printer != null)
			{
				PrinterSettings settings = PrinterSettings.FindPrinter (printer.PhysicalName);

				if (settings != null)
				{
					System.Array.ForEach (settings.PaperSources, paperSource => trayNames.Add (paperSource.Name));
				}

				if (!trayNames.Contains (printer.Tray) && printer.Tray != "")
				{
					trayNames.Add (printer.Tray);
				}
			}

			return trayNames;
		}


		private string DefaultLogicalName
		{
			get
			{
				return string.Format ("Imprimante {0}", (this.printerList.Count+1).ToString ());
			}
		}


		private string GetError()
		{
			for (int i = 0; i < this.printerList.Count; i++)
			{
				if (string.IsNullOrWhiteSpace (this.printerList[i].LogicalName))
				{
					return string.Format ("<b>Rang {0}</b>: L'imprimante n'est pas nommée.", (i+1).ToString ());
				}

				if (string.IsNullOrWhiteSpace (this.printerList[i].PhysicalName))
				{
					return string.Format ("<b>{0}</b>: Il faut choisir l'imprimante physique.", this.printerList[i].LogicalName);
				}

				if (string.IsNullOrWhiteSpace (this.printerList[i].Tray))
				{
					return string.Format ("<b>{0}</b>: Il faut choisir le bac.", this.printerList[i].LogicalName);
				}

				for (int j = 0; j < this.printerList.Count; j++)
				{
					if (j != i && this.printerList[j].LogicalName == this.printerList[i].LogicalName)
					{
						return string.Format ("<b>{0}</b>: Ces deux imprimantes ont la même dénomination.", this.printerList[i].LogicalName);
					}
				}
			}

			return null;
		}


		private Printer SelectedPrinter
		{
			get
			{
				int sel = this.SelectedIndex;

				if (sel == -1)
				{
					return null;
				}
				else
				{
					return this.printerList[sel];
				}
			}
		}

		private int SelectedIndex
		{
			get
			{
				return this.scrollList.SelectedItemIndex;
			}
			set
			{
				this.ignoreChange = true;
				this.scrollList.SelectedItemIndex = value;
				this.ignoreChange = false;
			}
		}

	
		#region Settings
		public static List<Printer> GetPrinterSettings()
		{
			List<Printer> list = new List<Printer> ();

			Dictionary<string, string> settings = CoreApplication.ExtractSettings ("Printer");

			foreach (var setting in settings.Values)
			{
				Printer printer = new Printer ();
				printer.SetSerializableContent (setting);

				list.Add (printer);
			}

			return list;
		}

		private static void SetPrinterSettings(List<Printer> list)
		{
			Dictionary<string, string> settings = new Dictionary<string,string>();
			int index = 0;

			foreach (var printer in list)
			{
				if (!string.IsNullOrWhiteSpace (printer.LogicalName) &&
					!string.IsNullOrWhiteSpace (printer.PhysicalName))
				{
					string key = string.Concat ("Printer", (index++).ToString (CultureInfo.InvariantCulture));
					settings.Add (key, printer.GetSerializableContent ());
				}
			}

			CoreApplication.MergeSettings ("Printer", settings);
		}
		#endregion


		private readonly CoreApplication				application;

		private Window									window;
		private GlyphButton								addButton;
		private GlyphButton								removeButton;
		private GlyphButton								moveUpButton;
		private GlyphButton								moveDownButton;
		private ScrollList								scrollList;
		private StaticText								logicalLabel;
		private StaticText								commentLabel;
		private StaticText								physicalLabel;
		private StaticText								trayLabel;
		private TextFieldEx								logicalField;
		private TextFieldEx								commentField;
		private TextFieldCombo							physicalField;
		private TextFieldCombo							trayField;
		private StaticText								errorInfo;
		private Button									acceptButton;
		private Button									cancelButton;
		private List<Printer>							printerList;
		private bool									ignoreChange;
	}
}

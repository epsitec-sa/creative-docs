//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Debug;
using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.Printing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Validators;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.App.BanquePiguet
{

	/// <summary>
	/// The <c>PrinterManagerDialog</c> class is a dialog which lets the user configure the
	/// <see cref="Printer"/>s used to print the bvs.
	/// </summary>
	class PrinterManagerDialog : AbstractDialog
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="PrinterManagerDialog"/> class.
		/// </summary>
		/// <param name="application">The <see cref="Application"/> creating this instance.</param>
		public PrinterManagerDialog(Application application)
		{
			this.Application = application;
			this.Printers = new List<Printer>(Printer.Load ());
		}

		/// <summary>
		/// Gets or sets the <see cref="Application"/> who created this instance.
		/// </summary>
		/// <value>The <see cref="Application"/> who created this instance.</value>
		protected Application Application
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets list of configured <see cref="Printer"/>s.
		/// </summary>
		/// <value>The list of configured <see cref="Printer"/>s.</value>
		protected IList<Printer> Printers
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the <see cref="CellTable"/> displaying the <see cref="Printer"/>s.
		/// </summary>
		/// <value>The <see cref="CellTable"/> displaying the <see cref="Printer"/>s.</value>
		protected CellTable PrintersCellTable
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the <see cref="Button"/> used to save the configuration.
		/// </summary>
		/// <value>The <see cref="Button"/> used to save the configuration.</value>
		protected Button SaveButton
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the <see cref="Button"/> used to close the <see cref="PrinterManagerDialog"/>.
		/// </summary>
		/// <value>The <see cref="Button"/> used to close the <see cref="PrinterManagerDialog"/>.</value>
		protected Button CancelButton
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the <see cref="Button"/> used to add a new <see cref="Printer"/>.
		/// </summary>
		/// <value>The <see cref="Button"/> used to add a new <see cref="Printer"/>.</value>
		protected Button AddPrinterButton
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the <see cref="Button"/> used to remove a <see cref="Printer"/>.
		/// </summary>
		/// <value>The <see cref="Button"/> used to remove a <see cref="Printer"/>.</value>
		protected Button RemovePrinterButton
		{
			get;
			set;
		}

		/// <summary>
		/// Gets the <see cref="AbstractTextField"/>s contained in PrintersCellTable.
		/// </summary>
		/// <value>The <see cref="AbstractTextField"/>s contained in PrintersCellTable.</value>
		/// <remarks>
		/// This property assumes that <see cref="PrinterManagerDialog.PrintersCellTable"/> is properly
		/// initialized. Do not call it before or while it is initialized.
		/// </remarks>
		protected IEnumerable<AbstractTextField> TextFieldCells
		{
			get
			{
				for (int i = 0; i < this.PrintersCellTable.Rows ; i++)
				{
					for (int j = 1; j < this.PrintersCellTable.Columns; j++)
					{
						yield return (AbstractTextField) this.PrintersCellTable[j, i].Children[0];
					}
				}
			}
		}

		/// <summary>
		/// Creates a <see cref="Window"/> for the current dialog.
		/// </summary>
		/// <returns>The created <see cref="Window"/>.</returns>
		protected override Window CreateWindow()
		{
			Window window = new Window ();

			this.SetupWindow (window);
			this.SetupButtons (window);
			this.SetupPrintersCellTable (window);
			this.SetupEvents (window);
			
			return window;
		}


		/// <summary>
		/// Sets up the properties of the <see cref="PrinterManagerDialog.Window"/> property of
		/// this instance.
		/// </summary>
		/// <param name="window">The <see cref="Window"/> to setup.</param>
		/// <remarks>
		/// This method is called at the initialization of this instance.
		/// </remarks>
		protected void SetupWindow(Window window)
		{
			this.OwnerWindow = this.Application.Window;
			window.Icon = this.Application.Window.Icon;
			window.Text = "Configuration des imprimantes";
			window.WindowSize = new Size (800, 400);
		}

		/// <summary>
		/// Sets up <see cref="PrinterManagerDialog.PrintersCellTable"/> according to
		/// <see cref="PrinterManagerDialog.Printers"/>. If <see cref="PrinterManagerDialog.PrintersCellTable"/>
		/// is allready initialized, that instance is removed and replaced by a new one. In addition, the
		/// <see cref="Event"/>s used to update <see cref="PrinterManagerDialog.Printers"/> and the
		/// <see cref="IValidator"/>s are also set up.
		/// </summary>
		/// <param name="window">The <see cref="Window"/> of the dialog.</param>
		/// <remarks>
		/// This method is called at the initialization of this instance and when
		/// a <see cref="Printer"/> is added or removed.
		/// </remarks>
		protected void SetupPrintersCellTable(Window window)
		{
			if (this.PrintersCellTable != null)
			{
				this.PrintersCellTable.Dispose ();
			}

			this.PrintersCellTable = new CellTable ()
			{
				Dock = DockStyle.Fill,
				Margins = new Margins (10, 10, 10, 10),
				Parent = window.Root,

			};

			this.PrintersCellTable.StyleH  = CellArrayStyles.None;
			this.PrintersCellTable.StyleH |= CellArrayStyles.Header;
			this.PrintersCellTable.StyleH |= CellArrayStyles.Separator;
			this.PrintersCellTable.StyleH |= CellArrayStyles.Mobile;
			this.PrintersCellTable.StyleH |= CellArrayStyles.Stretch;

			this.PrintersCellTable.StyleV  = CellArrayStyles.None;
			this.PrintersCellTable.StyleV |= CellArrayStyles.ScrollNorm;
			this.PrintersCellTable.StyleV |= CellArrayStyles.Separator;
			this.PrintersCellTable.StyleV |= CellArrayStyles.SelectLine;

			this.PrintersCellTable.SetArraySize (7, this.Printers.Count);
			this.PrintersCellTable.SetWidthColumn (0, 15);
			this.PrintersCellTable.SetWidthColumn (1, 200);
			this.PrintersCellTable.SetWidthColumn (2, 200);
			this.PrintersCellTable.SetWidthColumn (3, 150);
			this.PrintersCellTable.SetWidthColumn (4, 75);
			this.PrintersCellTable.SetWidthColumn (5, 75);
			this.PrintersCellTable.SetWidthColumn (6, 250);

			this.PrintersCellTable.SetHeaderTextH (1, "Nom");
			this.PrintersCellTable.SetHeaderTextH (2, "Bac");
			this.PrintersCellTable.SetHeaderTextH (3, "Orientation");
			this.PrintersCellTable.SetHeaderTextH (4, "Pos. horiz.");
			this.PrintersCellTable.SetHeaderTextH (5, "Pos. vert.");
			this.PrintersCellTable.SetHeaderTextH (6, "Commentaire");

			for (int i = 0; i < this.Printers.Count; i++)
			{
				Printer printer = this.Printers[i];

				TextFieldCombo nameTextField = new TextFieldCombo ()
				{
					Dock = DockStyle.Fill,
					Text = FormattedText.Escape (printer.Name),
				};

				this.PopulateNameTextField (printer, nameTextField);

				TextFieldCombo trayTextField = new TextFieldCombo ()
				{
					Dock = DockStyle.Fill,
					Text = FormattedText.Escape (printer.Tray),
				};

				this.PopulateTraysTextField (printer, trayTextField);

				TextFieldCombo orientationTextField = new TextFieldCombo ()
				{
					Dock = DockStyle.Fill,
					IsReadOnly = true,
					Text = printer.Horizontal ? "Horizontal" : "Vertical",
				};

				orientationTextField.Items.Add ("Horizontal");
				orientationTextField.Items.Add ("Vertical");

				TextFieldUpDown xOffsetTextField = new TextFieldUpDown ()
				{
					Dock = DockStyle.Fill,
					MaxValue = decimal.MaxValue,
					MinValue = decimal.MinValue,
					Resolution = 0.1M,
					Step = 0.1M,
					Text = printer.XOffset.ToString (System.Globalization.CultureInfo.InvariantCulture),
				};

				TextFieldUpDown yOffsetTextField = new TextFieldUpDown ()
				{
					Dock = DockStyle.Fill,
					MaxValue = decimal.MaxValue,
					MinValue = decimal.MinValue,
					Resolution = 0.1M,
					Step = 0.1M,
					Text = printer.YOffset.ToString (System.Globalization.CultureInfo.InvariantCulture),
				};

				TextField commentTextField = new TextField ()
				{
					Dock = DockStyle.Fill,
					Text = FormattedText.Escape (printer.Comment),
				};

				nameTextField.TextChanged += (sender) =>
				{
					printer.Name = FormattedText.Unescape (nameTextField.Text);
					this.PopulateTraysTextField (printer, trayTextField);
				};

				trayTextField.TextChanged += (sender) => printer.Tray = FormattedText.Unescape (trayTextField.Text);

				orientationTextField.TextChanged += (sender) => printer.Horizontal = (FormattedText.Unescape (orientationTextField.Text) == "Horizontal");

				xOffsetTextField.TextChanged += (sender) =>
				{
					string text = FormattedText.Unescape (xOffsetTextField.Text);
					double result;

					if (double.TryParse (text, NumberStyles.Number, CultureInfo.InvariantCulture, out result))
					{
						printer.XOffset = result;
					}
				};

				yOffsetTextField.TextChanged += (sender) =>
				{
					string text = FormattedText.Unescape (yOffsetTextField.Text);
					double result;

					if (double.TryParse (text, NumberStyles.Number, CultureInfo.InvariantCulture, out result))
					{
						printer.YOffset = result;
					}
				};

				commentTextField.TextChanged += (sender) =>
				{
					printer.Comment = FormattedText.Unescape (commentTextField.Text);
				};

				new PredicateValidator (nameTextField, () => this.CheckNameTextField(nameTextField)).Validate ();
				
				this.PrintersCellTable[1, i].Insert(nameTextField);
				this.PrintersCellTable[2, i].Insert (trayTextField);
				this.PrintersCellTable[3, i].Insert (orientationTextField);
				this.PrintersCellTable[4, i].Insert (xOffsetTextField);
				this.PrintersCellTable[5, i].Insert (yOffsetTextField);
				this.PrintersCellTable[6, i].Insert (commentTextField);
			}
			
			this.PrintersCellTable.FinalSelectionChanged += (sender) => this.CheckRemovePrinterEnabled ();
			this.CheckRemovePrinterEnabled ();

			this.TextFieldCells.ToList ().ForEach (c => c.TextChanged += (sender) => this.CheckSaveEnabled (sender as Widget));
			this.CheckSaveEnabled (null);
		}

		/// <summary>
		/// Populates the items of <paramref name="nameTextField"/> with the installed printers and
		/// the name <paramref name="printer"/> of and sets its current value to the name of
		/// <paramref name="printer"/>.
		/// </summary>
		/// <param name="printer">The <see cref="Printer"/> corresponding to nameTextField.</param>
		/// <param name="nameTextField">The <c>nameTextField</c> to populate.</param>
		/// <remarks>
		/// This method is called whenever a <c>nameTextField</c> is initialized.
		/// </remarks>
		protected void PopulateNameTextField(Printer printer, TextFieldCombo nameTextField)
		{
			List<string> printerNames = PrinterSettings.InstalledPrinters.ToList ();

			if (!printerNames.Contains (printer.Name) && printer.Name != "")
			{
				printerNames.Add (printer.Name);
			}

			printerNames.ForEach (printerName => nameTextField.Items.Add (FormattedText.Escape (printerName)));
		}

		/// <summary>
		/// Populates the items of <paramref name="trayTextField"/> with the trays of the installed
		/// printer corresponding to the name of <paramref name="printer"/> and with the tray of
		/// <paramref name="printer"/> and sets its current value to the tray of <paramref name="printer"/>.
		/// </summary>
		/// <param name="printer">The <see cref="Printer"/>.</param>
		/// <param name="trayTextField">The <c>trayTextField</c> field.</param>
		/// <remarks>
		/// This method is called whenever a  <c>trayTextField</c> is initialized or when the value of the
		/// corresponding <c>nameTextField</c> changes.
		/// </remarks>
		protected void PopulateTraysTextField(Printer printer, TextFieldCombo trayTextField)
		{
			if (trayTextField.Items.Count > 0)
			{
				trayTextField.Items.Clear ();
			}

			List<string> trayNames = new List<string> ();

			PrinterSettings settings = PrinterSettings.FindPrinter (printer.Name);
			
			if (settings != null)
			{
				System.Array.ForEach (settings.PaperSources, paperSource => trayNames.Add (paperSource.Name));
			}

			if (!trayNames.Contains (printer.Tray) && printer.Tray != "")
			{
				trayNames.Add (printer.Tray);
			}

			trayNames.ForEach (trayName => trayTextField.Items.Add (FormattedText.Escape (trayName)));
		}

		/// <summary>
		/// Sets up the <see cref="Button"/>s of this instance.
		/// </summary>
		/// <param name="window">The <see cref="Window"/> of the dialog.</param>
		/// <remarks>
		/// This method is called at the initialization of this instance.
		/// </remarks>
		protected void SetupButtons(Window window)
		{
			FrameBox buttonsFrameBox = new FrameBox ()
			{
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				Dock = DockStyle.Bottom,
				Parent = window.Root,
			};

			this.AddPrinterButton = new Button ()
			{
				Dock = DockStyle.Left,
				Margins = new Margins (10, 10, 0, 10),
				Parent = buttonsFrameBox,
				TabIndex = 1,
				Text = "Ajouter",
			};

			this.RemovePrinterButton = new Button ()
			{
				Dock = DockStyle.Left,
				Margins = new Margins (0, 10, 0, 10),
				Parent = buttonsFrameBox,
				TabIndex = 2,
				Text = "Supprimer",
			};

			this.CancelButton = new Button ()
			{
				Dock = DockStyle.Right,
				Margins = new Margins (10, 10, 0, 10),
				Parent = buttonsFrameBox,
				TabIndex = 4,
				Text = "Annuler",
			};

			this.SaveButton = new Button ()
			{
				Dock = DockStyle.Right,
				Margins = new Margins (0, 0, 0, 10),
				Parent = buttonsFrameBox,
				TabIndex = 3,
				Text = "Enregistrer",
			};
			this.SaveButton.Focus ();
		}

		/// <summary>
		/// Sets up the <see cref="Event"/>s of this instance.
		/// </summary>
		/// <param name="window">The <see cref="Window"/> of the dialog.</param>
		/// <remarks>
		/// This method is called at the initialization of this instance. Note that the <see cref="Event"/>s
		/// related to <see cref="PrinterManagerDialog.PrintersCellTable"/> are not set up here, as they are
		/// set up in <see cref="PrinterManagerDialog.SetupPrintersCellTable"/>.
		/// </remarks>
		protected void SetupEvents(Window window)
		{
			this.AddPrinterButton.Clicked += (sender, e) => this.AddPrinter ();
			this.RemovePrinterButton.Clicked += (sender, e) => this.RemovePrinter ();
			this.SaveButton.Clicked += (sender, e) => this.Save ();
			this.CancelButton.Clicked += (sender, e) => this.CloseDialog ();
		}

		/// <summary>
		/// Checks that the text of <paramref name="nameTextField"/> is valid.
		/// </summary>
		/// <param name="nameTextField">The <see cref="PrinterManagerDialog.nameTextField"/> whose text to check.</param>
		/// <returns>A <see cref="bool"/> indicating whether the text of <paramref name="nameTextField"/> is valid or not.</returns>
		protected bool CheckNameTextField(TextFieldCombo nameTextField)
		{
			string text = FormattedText.Unescape (nameTextField.Text);

			bool empty = (text.Trim ().Length == 0);
			bool duplicate = this.Printers.Count (printer => printer.Name == text) > 1;
			
			return !empty && !duplicate;
		}


		/// <summary>
		/// Enables or disables <see cref="PrinterManagerDialog.SaveButton"/> according to the validity
		/// of the <see cref="AbstractTextField"/>s contained in <see cref="PrinterManagerDialog.PrintersCellTable"/>.
		/// </summary>
		/// <param name="sender">The <see cref="widget"/> whose text modification fired the event.</param>
		/// <remarks>
		/// This method is called whenver the text changes in on the the <see cref="AbstractTextField"/>s
		/// contained in <see cref="PrinterManagerDialog.PrintersCellTable"/>.
		/// </remarks>
		protected void CheckSaveEnabled(Widget sender)
		{
			if (sender != null && sender.Validator != null)
			{
				sender.Validator.MakeDirty (true);
			}
			
			this.SaveButton.Enable = this.TextFieldCells.All (c => c.IsValid);
		}

		/// <summary>
		/// Enables or disables <see cref="PrinterManagerDialog.RemovePrinterButton"/> according to
		/// what is selected in <see cref="PrinterManagerDialog.PrintersCellTable"/>.
		/// </summary>
		/// <remarks>
		/// This method is called whenever the selection changes in <see cref="PrinterManagerDialog.PrintersCellTable"/>.
		/// </remarks>
		protected void CheckRemovePrinterEnabled()
		{
			this.RemovePrinterButton.Enable  = (this.PrintersCellTable.SelectedRow >= 0);
		}

		/// <summary>
		/// Saves the content of <see cref="PrinterManagerDialog.Printers"/> and exits the
		/// <see cref="PrinterManagerDialog"/>.
		/// </summary>
		/// <remarks>
		/// This method is called whenever <see cref="PrinterManagerDialog.SaveButton"/> is clicked.
		/// </remarks>
		protected void Save()
		{
			try
			{
				Printer.Save (this.Printers);
			}
			catch (System.Exception e)
			{
				MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, "Une erreur s'est produite lors de la sauvegarde des imprimantes").OpenDialog ();
				ErrorLogger.LogException (new System.Exception ("An error occured while saving the printers.", e));
			}

			this.CloseDialog ();
		}

		/// <summary>
		/// Adds a new empty <see cref="Printer"/> to <see cref="PrinterManagerDialog.Printers"/> and
		/// resets <see cref="PrinterManagerDialog.PrintersCellTable"/>.
		/// </summary>
		/// <remarks>
		/// This method is called whenver the <see cref="PrinterManagerDialog.AddPrinterButton"/> is clicked.
		/// </remarks>
		protected void AddPrinter()
		{
			this.Printers.Add (new Printer ("", "", true, 0, 0, ""));
			this.SetupPrintersCellTable (this.DialogWindow);
		}

		/// <summary>
		/// Removes the <see cref="Printer"/> selected in <see cref="PrinterManagerDialog.PrintersCellTable"/>
		/// from Printers and resets <see cref="PrinterManagerDialog.PrintersCellTable"/>.
		/// </summary>
		/// <remarks>
		/// This method is called whenver the <see cref="PrinterManagerDialog.RemovePrinterButton"/> is clicked.
		/// </remarks>
		protected void RemovePrinter()
		{
			int selectedRow = this.PrintersCellTable.SelectedRow;

			if (selectedRow >= 0)
			{
				this.Printers.RemoveAt (selectedRow);
				this.SetupPrintersCellTable (this.DialogWindow);
			}
		}
	}

}

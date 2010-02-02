//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common;
using Epsitec.Common.Drawing;
using Epsitec.Common.Printing;
using Epsitec.Common.Support;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Validators;

using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace Epsitec.App.BanquePiguet
{

	class PrintersManager : Window
	{

		public PrintersManager(Application application)
		{
			try
			{
				this.Application = application;
				this.Printers = Printer.Load ();
				this.SetupWindow ();
				this.SetupButtons ();
				this.SetupPrintersCellTable ();
				this.SetupEvents ();
				this.AdjustWindowSize ();
			}
			catch (System.Exception e)
			{
				Tools.Error (new System.Exception ("An error occured while initializing PrintersManager.", e));
			}
		}

		protected Application Application
		{
			get;
			set;
		}

		protected List<Printer> Printers
		{
			get;
			set;
		}

		protected CellTable PrintersCellTable
		{
			get;
			set;
		}

		protected Button SaveButton
		{
			get;
			set;
		}

		protected Button CancelButton
		{
			get;
			set;
		}

		protected Button AddPrinterButton
		{
			get;
			set;
		}

		protected Button RemovePrinterButton
		{
			get;
			set;
		}

		protected void SetupWindow()
		{
			this.Text = "Configuration des imprimantes";
		}

		protected IEnumerable<AbstractTextField> TextFieldCells
		{
			get
			{
				if (this.PrintersCellTable != null)
				{
					for (int i = 0; i < this.Printers.Count; i++)
					{
						for (int j = 1; j < 5; j++)
						{
							yield return (AbstractTextField) this.PrintersCellTable[j, i].Children[0];
						}
					}
				}
			}
		}

		protected void SetupPrintersCellTable()
		{
			if (this.PrintersCellTable != null)
			{
				this.PrintersCellTable.Dispose ();
			}

			this.PrintersCellTable = new CellTable ()
			{
				Dock = DockStyle.Fill,
				Margins = new Margins (10, 10, 10, 10),
				Parent = this.Root,

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

			this.PrintersCellTable.SetArraySize (5, this.Printers.Count);
			this.PrintersCellTable.SetWidthColumn (0, 25);

			this.PrintersCellTable.SetHeaderTextH (1, "Nom");
			this.PrintersCellTable.SetHeaderTextH (2, "Bac");
			this.PrintersCellTable.SetHeaderTextH (3, "Décalage x");
			this.PrintersCellTable.SetHeaderTextH (4, "Décalage y");

			for (int i = 0; i < this.Printers.Count; i++)
			{
				Printer printer = this.Printers[i];
				
				TextFieldCombo nameTextField = new TextFieldCombo ()
				{
					Dock = DockStyle.Fill,
					Text = printer.Name,
					
				};

				this.PopulateNameTextField (printer, nameTextField);

				TextFieldCombo trayTextField = new TextFieldCombo ()
				{
					Dock = DockStyle.Fill,
					Text = printer.Tray,
				};

				this.PopulateTraysTextField (printer, trayTextField);

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

				nameTextField.TextChanged += (sender) =>
				{
					printer.Name = FormattedText.Unescape (nameTextField.Text);
					this.PopulateTraysTextField (printer, trayTextField);
				};

				trayTextField.TextChanged += (sender) => printer.Tray = FormattedText.Unescape (trayTextField.Text);				

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

				System.Console.WriteLine (i + " PRINTER: " + printer.Name + " VALIDATOR START");

				//new PredicateValidator (nameTextField, () => { Console.WriteLine(printer.Name + " NAME: " + (FormattedText.Unescape (nameTextField.Text).Trim ().Length > 0)); return FormattedText.Unescape (nameTextField.Text).Trim ().Length > 0;});//.Validate ();
				//new PredicateValidator (trayTextField, () => { Console.WriteLine(printer.Name + " TRAY: " + (FormattedText.Unescape (trayTextField.Text).Trim ().Length > 0)); return FormattedText.Unescape (trayTextField.Text).Trim ().Length > 0;});//.Validate ();

				new PredicateValidator (nameTextField, () => { System.Console.WriteLine(">" + printer.Name + "< NAME: " + (false)); return false;}).Validate ();
				new PredicateValidator (trayTextField, () => { System.Console.WriteLine(">" + printer.Name + "< TRAY: " + (false)); return false;}).Validate ();

				System.Console.WriteLine (i + " PRINTER: " + printer.Name + " VALIDATOR END");

				this.PrintersCellTable[1, i].Insert(nameTextField);
				this.PrintersCellTable[2, i].Insert (trayTextField);
				this.PrintersCellTable[3, i].Insert (xOffsetTextField);
				this.PrintersCellTable[4, i].Insert (yOffsetTextField);
			}

			this.PrintersCellTable.FinalSelectionChanged += (sender) => this.CheckRemovePrinterValidity ();
			this.CheckRemovePrinterValidity ();

			this.TextFieldCells.ToList ().ForEach (c => c.TextChanged += (sender) => this.CheckSaveValidity ());
			this.CheckSaveValidity ();
		}

		protected void PopulateNameTextField(Printer printer, TextFieldCombo nameTextField)
		{
			List<string> printerNames = PrinterSettings.InstalledPrinters.ToList ();

			if (!printerNames.Contains (printer.Name) && printer.Name != "")
			{
				printerNames.Add (printer.Name);
			}

			printerNames.ForEach (printerName => nameTextField.Items.Add (printerName));
		}

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

			trayNames.ForEach (trayName => trayTextField.Items.Add (trayName));
		}

		protected void SetupButtons()
		{
			FrameBox buttonsFrameBox = new FrameBox ()
			{
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				Dock = DockStyle.Bottom,
				Parent = this.Root,
			};

			this.CancelButton = new Button ()
			{
				Dock = DockStyle.Right,
				Margins = new Margins (10, 10, 10, 10),
				Parent = buttonsFrameBox,
				Text = "Annuler",
			};

			this.SaveButton = new Button ()
			{
				Dock = DockStyle.Right,
				Margins = new Margins (10, 0, 10, 10),
				Parent = buttonsFrameBox,
				Text = "Sauvegarder",
			};

			this.AddPrinterButton = new Button ()
			{
				Dock = DockStyle.Left,
				Margins = new Margins (10, 10, 10, 10),
				Parent = buttonsFrameBox,
				Text = "Ajouter",
			};

			this.RemovePrinterButton = new Button ()
			{
				Dock = DockStyle.Left,
				Margins = new Margins (0, 0, 10, 10),
				Parent = buttonsFrameBox,
				Text = "Supprimer",
			};

		}

		protected void SetupEvents()
		{
			this.AddPrinterButton.Clicked += (sender, e) => this.AddPrinter ();
			this.RemovePrinterButton.Clicked += (sender, e) => this.RemovePrinter ();
			this.SaveButton.Clicked += (sender, e) => this.Save ();
			this.CancelButton.Clicked += (sender, e) => this.Cancel ();
			this.WindowClosed += (sender) => this.Cancel();
		}

		protected void CheckSaveValidity()
		{
			this.SaveButton.Enable = this.TextFieldCells.All (c => c.IsValid);
		}

		protected void CheckRemovePrinterValidity()
		{
			this.RemovePrinterButton.Enable  = (this.PrintersCellTable.SelectedRow >= 0);
		}

		protected void Save()
		{
			try
			{
				Printer.Save (this.Printers);
			}
			catch (System.Exception e)
			{
				Tools.Error (new System.Exception ("An error occured while saving the printers.", e));
			}

			this.Exit ();
		}

		protected void Cancel()
		{
			this.Exit ();
		}

		protected void AddPrinter()
		{
			this.Printers.Add (new Printer ("", "", 0, 0));
			this.SetupPrintersCellTable ();
		}

		protected void RemovePrinter()
		{
			int selectedRow = this.PrintersCellTable.SelectedRow;

			if (selectedRow >= 0)
			{
				this.Printers.RemoveAt (selectedRow);
			}

			this.SetupPrintersCellTable ();
		}

		protected void Exit()
		{
			this.Application.DisplayPrintersManager (false);
		}

	}

}

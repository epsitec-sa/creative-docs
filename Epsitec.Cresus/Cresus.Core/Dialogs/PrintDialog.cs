//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

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
	/// The <c>PrintDialog</c> class is a dialog which lets the user choose on which printer
	/// he wants to print the bv and how much copies does he want to print.
	/// </summary>
	class PrintDialog : AbstractDialog
	{
		public PrintDialog(Application application, Printers.AbstractEntityPrinter entityPrinter, IEnumerable<AbstractEntity> entities, List<Printer> printers)
		{
			this.Application = application;
			this.entityPrinter = entityPrinter;
			this.entities = entities;
			this.Printers = printers;
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
		/// Gets or sets the list of <see cref="Printer"/>s available to the user.
		/// </summary>
		/// <value>The list of <see cref="Printer"/>s available to the user.</value>
		protected List<Printer> Printers
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the <see cref="Button"/> used to print the bv.
		/// </summary>
		/// <value>The <see cref="Button"/> used to prints the bv.</value>
		protected Button PrintButton
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the <see cref="Button"/> used to close the window.
		/// </summary>
		/// <value>The <see cref="Button"/> used to close the window.</value>
		protected Button CancelButton
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the <see cref="TextFieldUpDown"/> used for the number of copies.
		/// </summary>
		/// <value>The <see cref="TextFieldUpDown"/> used for the number of copies.</value>
		protected TextFieldUpDown NbCopiesTextField
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the <see cref="TextFieldCombo"/> used for the printer selection.
		/// </summary>
		/// <value>The <see cref="TextFieldCombo"/> used for the printer selection.</value>
		protected TextFieldCombo PrinterTextField
		{
			get;
			set;
		}

		/// <summary>
		/// Creates a <see cref="Window"/> for the current dialog.
		/// </summary>
		/// <returns>The created <see cref="Window"/>.</returns>
		protected override Window CreateWindow()
		{
			Window window = new Window ();

			this.SetupWindow (window);
			this.SetupWidgets (window);
			this.SetupEvents (window);

			window.AdjustWindowSize ();

			return window;
		}

		/// <summary>
		/// Sets up the properties of the <see cref="PrintDialog.window"/> property of this
		/// instance.
		/// </summary>
		/// <param name="window">The <see cref="Window"/> to setup.</param>
		/// <remarks>
		/// This method is called at the initialization of this instance.
		/// </remarks>
		protected void SetupWindow(Window window)
		{
			this.OwnerWindow = this.Application.Window;
			window.Icon = this.Application.Window.Icon;
			window.Text = "Imprimer";
			window.WindowSize = new Size (100, 100);
			window.MakeFixedSizeWindow ();
		}

		/// <summary>
		/// Sets up the <see cref="Widget"/>s of the <see cref="PrintDialog"/>, such as the
		/// <see cref="Button"/>s and <see cref="TexfField"/>s.
		/// </summary>
		/// <param name="window">The <see cref="Window"/> to setup.</param>
		/// <remarks>
		/// This method is called at the initialization of this instance.
		/// </remarks>
		protected void SetupWidgets(Window window)
		{
			FrameBox frameBox = new FrameBox ()
			{
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				Dock = DockStyle.Top,
				Parent = window.Root
			};

			GroupBox printerGroupBox = new GroupBox ()
			{
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				Dock = DockStyle.Stacked,
				Margins = new Margins (10, 10, 10, 10),
				Padding = new Margins (5, 5, 0, 5),
				Parent = frameBox,
				Text = "Imprimante",
				TabIndex = 1,
			};

			this.PrinterTextField = new TextFieldCombo ()
			{
				Dock = DockStyle.Stacked,
				Parent = printerGroupBox,
				PreferredWidth = 200,
				TabIndex = 1,
				IsReadOnly = true,
			};

			this.Printers.ForEach (printer => this.PrinterTextField.Items.Add (printer.Name));

			string preferredPrinter;
			bool preferredPrinterDefined = Settings.Load ().TryGetValue ("preferredPrinter", out preferredPrinter);

			if (preferredPrinterDefined && this.Printers.Any (printer => printer.Name == preferredPrinter))
			{
				this.PrinterTextField.Text = FormattedText.Escape (preferredPrinter);
			}
			else
			{
				this.PrinterTextField.Text = FormattedText.Escape (this.Printers[0].Name);
			}

			GroupBox nbPagesGroupBox = new GroupBox ()
			{
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				Dock = DockStyle.Stacked,
				Margins = new Margins (10, 10, 0, 10),
				Padding = new Margins (5, 5, 0, 5),
				Parent = frameBox,
				Text = "Nombre de copies",
				TabIndex = 2,
			};

			this.NbCopiesTextField = new TextFieldUpDown ()
			{
				Dock = DockStyle.Stacked,
				Parent = nbPagesGroupBox,
				PreferredWidth = 200,
				Resolution = 1,
				Step = 1,
				TabIndex = 1,
				Text = "1",
			};

			this.UpdateNbPagesRange ();

			FrameBox buttonsFrameBox = new FrameBox ()
			{
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				Dock = DockStyle.Stacked,
				Parent = frameBox,
				TabIndex = 3,
			};

			this.PrintButton = new Button ()
			{
				Dock = DockStyle.StackFill,
				Margins = new Margins (10, 10, 0, 10),
				Parent = buttonsFrameBox,
				Text = "Imprimer",
				TabIndex = 1,
			};

			this.CancelButton = new Button ()
			{
				Dock = DockStyle.StackFill,
				Margins = new Margins (0, 10, 0, 10),
				Parent = buttonsFrameBox,
				Text = "Annuler",
				TabIndex = 2,
			};
		}

		/// <summary>
		/// Sets up the <see cref="Event"/>s of this instance.
		/// </summary>
		/// <param name="window">The <see cref="Window"/> to setup.</param>
		/// <remarks>
		/// This method is called at the initialization of this instance.
		/// </remarks>
		protected void SetupEvents(Window window)
		{
			this.PrinterTextField.TextChanged += (sender) => this.UpdateNbPagesRange ();
			this.PrinterTextField.TextChanged += (sender) => this.CheckPrintEnabled ();
			this.NbCopiesTextField.TextChanged += (sender) => this.CheckPrintEnabled ();
			this.CancelButton.Clicked += (sender, e) => this.CloseDialog ();
			this.PrintButton.Clicked += (sender, e) => this.Print ();
		}

		/// <summary>
		/// Updates the range of <see cref="PrintDialog.NbCopiesTextField"/> based on the value
		/// of <see cref="PrintDialog.PrinterTextField"/>.
		/// </summary>
		/// <remarks>
		/// This method is called whenever the text of <see cref="PrintDialog.PrinterTextField"/> changes.
		/// </remarks>
		protected void UpdateNbPagesRange()
		{
			PrinterSettings printer = PrinterSettings.FindPrinter(FormattedText.Unescape (this.PrinterTextField.Text));

			this.NbCopiesTextField.MinValue = 1;
			this.NbCopiesTextField.MaxValue = printer.MaximumCopies;
		}

		/// <summary>
		/// Enables or disables <see cref="PrintDialog.PrintButton"/> according to the validity of
		/// <see cref="PrintDialog.PrinterTextField"/> and <see cref="PrintDialog.NbCopiesTextField"/>.
		/// </summary>
		/// <remarks>
		/// This method is called whenever the text of <see cref="PrintDialog.PrinterTextField"/>
		/// and <see cref="PrintDialog.NbCopiesTextField"/> changes.
		/// </remarks>
		protected void CheckPrintEnabled()
		{
			this.PrintButton.Enable = this.NbCopiesTextField.IsValid;	
		}

		protected void Print()
		{
			Printer printer = this.Printers.Find (p => p.Name == FormattedText.Unescape (this.PrinterTextField.Text));
			PrinterSettings printerSettings = PrinterSettings.FindPrinter (printer.Name);
			
			bool checkTray = printerSettings.PaperSources.Any (tray => (tray.Name == printer.Tray));

			if (checkTray)
			{
				try
				{
					foreach (var entity in this.entities)
					{
						this.PrintEntities (this.entityPrinter, entity);
					}

					this.LogPrint ();
				}
				catch (System.Exception e)
				{
					MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, "Une erreur s'est produite lors de l'impression").OpenDialog ();
					ErrorLogger.LogException (new System.Exception ("An error occured while printing.", e));
				}
				finally
				{
					Settings settings = Settings.Load ();
					settings["preferredPrinter"] = FormattedText.Unescape (this.PrinterTextField.Text);
					settings.Save ();
					this.CloseDialog ();
				}
			}
			else
			{
				string message = string.Format ("Le bac ({0}) de l'imprimante séléctionnée ({1}) n'existe pas.", printer.Tray, printer.Name);
				MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, message).OpenDialog ();
			}
		}


		protected void PrintEntities(Printers.AbstractEntityPrinter entityPrinter, AbstractEntity entity)
		{
			Printer printer = this.Printers.Find (p => p.Name == FormattedText.Unescape (this.PrinterTextField.Text));
			PrintDocument printDocument = new PrintDocument();

#if true
			printDocument.DocumentName = entityPrinter.JobName;
			printDocument.SelectPrinter(printer.Name);
			printDocument.PrinterSettings.Copies = int.Parse (FormattedText.Unescape (this.NbCopiesTextField.Text), CultureInfo.InvariantCulture);
			printDocument.DefaultPageSettings.Margins = new Margins (0, 0, 0, 0);
			printDocument.DefaultPageSettings.PaperSource = System.Array.Find (printDocument.PrinterSettings.PaperSources, paperSource => paperSource.Name == printer.Tray);

			Size size = entityPrinter.PageSize;
			double height = size.Height;
			double width  = size.Width;

			double xOffset = printer.XOffset;
			double yOffset = printer.YOffset;

			if (printer.Horizontal)
			{
				Transform transform = Transform.Identity;
				PrintPort.PrintSinglePage (painter => entityPrinter.Print (painter, new Rectangle (xOffset, -yOffset, width, height)), printDocument, transform);
			}
			else
			{
				Transform transform = Transform.Identity.RotateDeg (90);
				PrintPort.PrintSinglePage (painter => entityPrinter.Print (painter, new Rectangle (-yOffset, -xOffset, width, height)), printDocument, transform);
			}
#endif
		}

		protected void LogPrint()
		{
		}


		private IEnumerable<AbstractEntity> entities;
		private Printers.AbstractEntityPrinter entityPrinter;
	}
}

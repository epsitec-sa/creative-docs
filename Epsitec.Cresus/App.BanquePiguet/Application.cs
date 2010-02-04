//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.Printing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Validators;

using System.Collections.Generic;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using System;

namespace Epsitec.App.BanquePiguet
{

	/// <summary>
	/// The Application class displays a form which lets the user type some values of a bv, print them and
	/// configure the printers to use.
	/// </summary>
	class Application : Epsitec.Common.Widgets.Application
	{

		/// <summary>
		/// Initializes a new instance of the Application class.
		/// </summary>
		/// <param name="adminMode">Tell if the application is launched in admin mode.</param>
		public Application(bool adminMode)
		{
			this.AdminMode = adminMode;
			this.SetupWindow ();
			this.SetupForm ();
			this.SetupBvWidget ();
			this.SetupEvents ();
			this.SetupValidators ();
			this.CheckPrintButtonEnbled ();
			this.Window.AdjustWindowSize ();

#warning Remove me at the end of the tests.
			
			this.BenefeciaryIbanTextField.Text = FormattedText.Escape ("CH01 1234 5678 9012 3456 7");
			this.BeneficiaryAddressTextField.Text = FormattedText.Escape ("Monsieur Alfred DUPOND\nRue de la tarte 85 bis\n7894 Tombouctou\nCocagne Land");
			this.ReasonTextField.Text = FormattedText.Escape ("0123456789\n0123456789\n0123456789");
			
			this.DisplayPrintersManager (true);
			
			//this.DisplayPrintDialog (true);
		}

		/// <summary>
		/// Gets the short window title.
		/// </summary>
		/// <value>The short window title.</value>
		public override string ShortWindowTitle
		{
			get
			{
				return "Banque Piguet";
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the application is launched in admin mode.
		/// </summary>
		/// <value><c>A bool indicating whether the application is launched in admin mode.</value>
		public bool AdminMode
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets or sets the TextField used for the beneficiary iban.
		/// </summary>
		/// <value>The TextField used for the beneficiary iban.</value>
		protected TextField BenefeciaryIbanTextField
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the TextFieldMulti used for the beneficiary address.
		/// </summary>
		/// <value>The TextFieldMulti used for the beneficiary address.</value>
		protected TextFieldMulti BeneficiaryAddressTextField
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the BvWidget associated with this instance.
		/// </summary>
		/// <value>The BvWidget associated with this instance.</value>
		protected BvWidget BvWidget
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the button used to display PrintDialog.
		/// </summary>
		/// <value>The button used to print display PrintDialog.</value>
		protected Button PrintButton
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the button used to display PrintersManager.
		/// </summary>
		/// <value>The button used to display PrintersManager.</value>
		protected Button OptionsButton
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the TextFieldMulti used for the reason of the transfer.
		/// </summary>
		/// <value>The TextFieldMulti used for the reason of the transfer.</value>
		protected TextFieldMulti ReasonTextField
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the PrintersManager associated with this instance.
		/// </summary>
		/// <value>The PrintersManager associated with this instance.</value>
		protected PrintersManager PrintersManager
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the PrintDialog associated with this instance.
		/// </summary>
		/// <value>The PrintDialog associated with this instance.</value>
		protected PrintDialog PrintDialog
		{
			get;
			set;
		}

		/// <summary>
		/// Sets up the window property of this instance.
		/// </summary>
		/// <remarks>
		/// This method is called at the initialization of this instance.
		/// </remarks>
		protected void SetupWindow()
		{
			this.Window = new Window ()
			{
				Text = this.ShortWindowTitle,
			};

			this.Window.SetNativeIconFromManifest (System.Reflection.Assembly.GetExecutingAssembly (), "Epsitec.App.BanquePiguet.Resources.app.ico");
			this.Window.MakeFixedSizeWindow ();
		}

		/// <summary>
		/// Sets up the form containing the TextFields and the Buttons.
		/// </summary>
		/// <remarks>
		/// This method is called at the initializatin of this instance.
		/// </remarks>
		protected void SetupForm()
		{

			FrameBox formFrameBox = new FrameBox ()
			{
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				Dock = DockStyle.Left,
				Parent = this.Window.Root
			};

			GroupBox beneficiaryIbanGroupBox = new GroupBox ()
			{
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				Dock = DockStyle.Stacked,
				Margins = new Margins (10, 10, 10, 10),
				Padding = new Margins (5, 5, 0, 5),
				Parent = formFrameBox,
				Text = "N° IBAN du bénéficiaire",
				TabIndex = 1,
			};

			this.BenefeciaryIbanTextField = new TextField ()
			{
				Dock = DockStyle.Stacked,
				Parent = beneficiaryIbanGroupBox,
				PreferredWidth = 200,
				TabIndex = 1,
			};
			this.BenefeciaryIbanTextField.Focus ();
			
			GroupBox beneficiaryAddressGroupBox = new GroupBox ()
			{
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				Dock = DockStyle.Stacked,
				Margins = new Margins (10, 10, 0, 10),
				Padding = new Margins (5, 5, 0, 5),
				Parent = formFrameBox,
				Text = "Nom et adresse du bénéficiaire",
				TabIndex = 2,
			};

			this.BeneficiaryAddressTextField = new TextFieldMulti ()
			{
				Dock = DockStyle.Stacked,
				Parent = beneficiaryAddressGroupBox,
				PreferredHeight = 65,
				PreferredWidth = 200,
				ScrollerVisibility = false,
				TabIndex = 2,
			};

			GroupBox reasonGroupBox = new GroupBox ()
			{
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				Dock = DockStyle.Stacked,
				Margins = new Margins (10, 10, 0, 10),
				Padding = new Margins (5, 5, 0, 5),
				Parent = formFrameBox,
				Text = "Motif du versement",
				TabIndex = 3,
			};

			this.ReasonTextField = new TextFieldMulti ()
			{
				Dock = DockStyle.Stacked,
				Parent = reasonGroupBox,
				PreferredHeight = 52,
				PreferredWidth = 200,
				ScrollerVisibility = false,
				TabIndex = 3,
			};

			FrameBox buttonsFrameBox = new FrameBox ()
			{
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				Dock = DockStyle.Stacked,
				Parent = formFrameBox,
				TabIndex = 4,
			};

			this.PrintButton = new Button ()
			{
				Dock = DockStyle.StackFill,
				Margins = new Margins (10, 10, 0, 10),
				Parent = buttonsFrameBox,
				Text = "Imprimer",
				TabIndex = 1,
			};

			if (this.AdminMode)
			{
				this.OptionsButton = new Button ()
				{
					Dock = DockStyle.StackFill,
					Margins = new Margins (0, 10, 0, 10),
					Parent = buttonsFrameBox,
					Text = "Options",
					TabIndex = 2,
				};
			}
		}

		/// <summary>
		/// Sets up the BvWidget associated with this instance.
		/// </summary>
		/// <remarks>
		/// This method is called at the initialization of this instance.
		/// </remarks>
		/// <exception cref="System.Exception">If the definition of the bv values is not valid</exception>
		protected void SetupBvWidget()
		{
			this.BvWidget = new BvWidget ()
			{
				Dock = DockStyle.Left,
				Margins = new Margins (10, 10, 10, 10),
				Parent = this.Window.Root,
				PreferredSize = new Size (525, 265),
			};

			XElement xBvValues;

			using (XmlReader xmlReader = XmlReader.Create (Tools.GetResourceStream("BvValues.xml")))
			{
				xBvValues = XElement.Load (xmlReader);
			}

			List<string> values = new List<string> ();

			foreach (XElement value in xBvValues.Elements("value"))
			{
				string name = (string) value.Element ("name");
				string text = (string) value.Element ("text");
				
				if (text.Contains ("\\n"))
				{
					string[] lines = text.Replace("\\n", "\n").Split ('\n');
					text = lines.Aggregate ((a, b) => string.Format ("{0}\n{1}", a, b));
				}

				switch (name)
				{
					case "BankAddress":
						this.BvWidget.BankAddress = text;
						break;
					case "BankAccount":
						this.BvWidget.BankAccount = text;
						break;
					case "LayoutCode":
						this.BvWidget.LayoutCode = text;
						break;
					case "ReferenceClientNumber":
						this.BvWidget.ReferenceClientNumber = text;
						break;
					case "ClearingConstant":
						this.BvWidget.ClearingConstant = text;
						break;
					case "ClearingBank":
						this.BvWidget.ClearingBank = text;
						break;
					case "ClearingBankKey":
						this.BvWidget.ClearingBankKey = text;
						break;
					case "CcpNumber":
						this.BvWidget.CcpNumber = text;
						break;
					default:
						throw new System.Exception (string.Format ("Unknown value: {0}", name));
				}

				if (values.Contains (name))
				{
					throw new System.Exception (String.Format ("A bv value is defined more than once: {0}", name));
				}
				else
				{
					values.Add (name);
				}
			}

			if (values.Count < 8)
			{
				throw new System.Exception ("Some bv values are missing.");
			}

		}

		/// <summary>
		/// Sets up the events related to the Buttons or to the TextFields.
		/// </summary>
		/// <remarks>
		/// This method is called at the initialization of this instance.
		/// </remarks>
		protected void SetupEvents()
		{
			this.BenefeciaryIbanTextField.TextChanged += (sender) =>
			{
				if (this.CheckBeneficiaryIban ())
				{
					this.BvWidget.BeneficiaryIban = FormattedText.Unescape (this.BenefeciaryIbanTextField.Text);
				}

				this.CheckPrintButtonEnbled ();
			};

			this.BeneficiaryAddressTextField.TextChanged += (sender) =>
			{
				if (this.CheckBeneficiaryAddress ())
				{
					this.BvWidget.BeneficiaryAddress = FormattedText.Unescape (this.BeneficiaryAddressTextField.Text);
				}

				this.CheckPrintButtonEnbled ();
			};

			this.ReasonTextField.TextChanged += (sender) =>
			{
				if (this.CheckReason ())
				{
					this.BvWidget.Reason = FormattedText.Unescape(this.ReasonTextField.Text);
				}

				this.CheckPrintButtonEnbled ();
			};

			if (this.AdminMode)
			{
				this.OptionsButton.Clicked += (sender, e) => this.DisplayPrintersManager (true);
			}

			this.PrintButton.Clicked += (sender, e) => this.DisplayPrintDialog (true);
		}

		/// <summary>
		/// Sets up the validators of the TextFields.
		/// </summary>
		/// <remarks>
		/// This method is called at the initialization of this instance.
		/// </remarks>
		protected void SetupValidators()
		{
			new PredicateValidator (
				this.BenefeciaryIbanTextField,
				() => this.CheckBeneficiaryIban ()
			);
			
			new PredicateValidator (
				this.BeneficiaryAddressTextField,
				() => this.CheckBeneficiaryAddress ()
			);
			
			new PredicateValidator (
				this.ReasonTextField,
				() => this.CheckReason ());
		}

		/// <summary>
		/// Checks the text of BenefeciaryIbanTextField is valid.
		/// </summary>
		/// <returns>A bool indicating if the text of BenefeciaryIbanTextField is valid or not.</returns>
		protected bool CheckBeneficiaryIban()
		{
			string text = FormattedText.Unescape (this.BenefeciaryIbanTextField.Text);
			string iban = BvHelper.BuildNormalizedIban (text);
			return BvHelper.CheckBeneficiaryIban (iban);
		}

		/// <summary>
		/// Checks the text of BeneficiaryAddressTextField is valid.
		/// </summary>
		/// <returns>A bool indicating if the text of BeneficiaryAddressTextField is valid or not.</returns>
		protected bool CheckBeneficiaryAddress()
		{
			string address = FormattedText.Unescape (this.BeneficiaryAddressTextField.Text);
			return BvHelper.CheckBeneficiaryAddress (address);
		}

		/// <summary>
		/// Checks the text of ReasonTextField is valid.
		/// </summary>
		/// <returns>A bool indicating if the text of ReasonTextField is valid or not.</returns>
		protected bool CheckReason()
		{
			string reason = FormattedText.Unescape (this.ReasonTextField.Text);
			return BvHelper.CheckReason (reason);
		}

		/// <summary>
		/// Enables or disables the PrintButton based on the validity of the TextFields.
		/// </summary>
		/// <remarks>
		/// This method is called whenever the text of one of the TextField changes.
		/// </remarks>
		protected void CheckPrintButtonEnbled()
		{
			this.PrintButton.Enable =  BvHelper.CheckBv (this.BvWidget)
									&& this.CheckBeneficiaryIban ()
									&& this.CheckBeneficiaryAddress ()
									&& this.CheckReason ();
		}

		/// <summary>
		/// Displays or hides the PrintersManager associated to this instance.
		/// </summary>
		/// <param name="display">A bool indicating whether to display or hide the PrintersManager.</param>
		public void DisplayPrintersManager(bool display)
		{
			if (display)
			{
				this.PrintersManager = new PrintersManager (this);
				this.PrintersManager.Show ();
				this.Window.IsFrozen = true;
			}
			else
			{
				this.PrintersManager.Dispose ();
				this.PrintersManager = null;
				this.Window.IsFrozen = false;
			}
		}

		/// <summary>
		/// Displays or hides the PrintDialog associated to this instance.
		/// </summary>
		/// <param name="display">A bool indicating whether to display or hide the PrintDialog.</param>
		public void DisplayPrintDialog(bool display)
		{
			if (display)
			{
				List<Printer> printers = new List<Printer>
				(
					from printer in Printer.Load ()
					where PrinterSettings.InstalledPrinters.Contains (printer.Name)
					select printer
				);

				bool checkPrintersList = (printers.Count > 0);
				bool checkPrintersTrays = printers.All (printer =>
				{
					PaperSource[] trays = PrinterSettings.FindPrinter (printer.Name).PaperSources;
					return trays.Any (tray => (tray.Name == printer.Tray));  
				});

				if (!checkPrintersList)
				{
					MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, "Aucune imprimante n'est configurée pour cet ordinateur.").OpenDialog ();
				}
				else if (!checkPrintersTrays)
				{
					string printerName = printers.Find (printer =>
					{
						PaperSource[] trays = PrinterSettings.FindPrinter (printer.Name).PaperSources;
						return !trays.Any (tray => (tray.Name == printer.Tray));  
					}).Name ;

					string message = String.Format ("Le bac d'une imprimante est mal configuré: {0}", printerName);
					
					MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, message).OpenDialog ();
				}
				else
				{
					this.PrintDialog = new PrintDialog (this, this.BvWidget, printers);
					this.PrintDialog.Show ();
					this.Window.IsFrozen = true;
				}
			}
			else
			{
				this.PrintDialog.Dispose ();
				this.PrintDialog = null;
				this.Window.IsFrozen = false;
			}
		}

	}	

}

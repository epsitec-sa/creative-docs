﻿//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Dialogs;
using Epsitec.Common.Drawing;
using Epsitec.Common.Printing;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Common.Widgets.Validators;

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.Linq;

namespace Epsitec.App.BanquePiguet
{

	/// <summary>
	/// The <c>Application</c> class displays a form which lets the user type some values of a
	/// bv, print them and configure the printers to use.
	/// </summary>
	class Application : Epsitec.Common.Widgets.Application
	{

		/// <summary>
		/// Initializes a new instance of the <see cref="Application"/> class.
		/// </summary>
		protected Application()
			: this (true)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="Application"/> class.
		/// </summary>
		/// <param name="adminMode">Tell if the <see cref="Application"/> is launched in admin mode.</param>
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
		}

		/// <summary>
		/// Gets the short window title.
		/// </summary>
		/// <value>The short window title.</value>
		public override string ShortWindowTitle
		{
			get
			{
				return "Banque Piguet — Impression de BV";
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether the <see cref="Application"/> is launched in
		/// admin mode.
		/// </summary>
		/// <value>A <see cref="bool"/> indicating whether the <see cref="Application"/> is launched in admin mode.</value>
		public bool AdminMode
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets or sets the <see cref="TextField"/> used for the beneficiary iban.
		/// </summary>
		/// <value>The <see cref="TextField"/> used for the beneficiary iban.</value>
		protected TextField BenefeciaryIbanTextField
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the <see cref="StaticText"/> used to display errors relative to the
		/// beneficiary iban.
		/// </summary>
		/// <value>The <see cref="StaticText"/> used for the errors of the beneficiary iban.</value>
		protected StaticText BeneficiaryIbanErrorStaticText
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the <see cref="TextFieldMulti"/> used for the beneficiary address.
		/// </summary>
		/// <value>The <see cref="TextFieldMulti"/> used for the beneficiary address.</value>
		protected TextFieldMulti BeneficiaryAddressTextField
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the <see cref="StaticText"/> used to display errors relative to the
		/// beneficiary address.
		/// </summary>
		/// <value>The <see cref="StaticText"/> used for the errors of the beneficiary address.</value>
		protected StaticText BeneficiaryAddressErrorStaticText
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the <see cref="TextFieldMulti"/> used for the reason of the transfer.
		/// </summary>
		/// <value>The <see cref="TextFieldMulti"/> used for the reason of the transfer.</value>
		protected TextFieldMulti ReasonTextField
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the <see cref="StaticText"/> used to display errors relative to the
		/// reason.
		/// </summary>
		/// <value>The <see cref="StaticText"/> used for the errors of the reason.</value>
		protected StaticText ReasonErrorStaticText
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the <see cref="BvWidget"/> associated with this instance.
		/// </summary>
		/// <value>The <see cref="BvWidget"/> associated with this instance.</value>
		protected BvWidget BvWidget
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the <see cref="Button"/> used to display <see cref="PrintDialog"/>.
		/// </summary>
		/// <value>The <see cref="Button"/> used to print display <see cref="PrintDialog"/>.</value>
		protected Button PrintButton
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the <see cref="Button"/> used to display <see cref="PrinterManagerDialog"/>.
		/// </summary>
		/// <value>The <see cref="Button"/> used to display <see cref="PrinterManagerDialog"/>.</value>
		protected Button OptionsButton
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the <see cref="PrinterManagerDialog"/> associated with this instance.
		/// </summary>
		/// <value>The <see cref="PrinterManagerDialog"/> associated with this instance.</value>
		protected PrinterManagerDialog PrintersManager
		{
			get;
			set;
		}

		/// <summary>
		/// Sets up the <see cref="Application.Window"/> property of this instance.
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

			this.Window.SetNativeIconFromManifest (System.Reflection.Assembly.GetExecutingAssembly (), "Epsitec.App.BanquePiguet.Resources.app.ico", 16, 16);
			this.Window.SetNativeIconFromManifest (System.Reflection.Assembly.GetExecutingAssembly (), "Epsitec.App.BanquePiguet.Resources.app.ico", 32, 32);
			this.Window.MakeFixedSizeWindow ();
		}

		/// <summary>
		/// Sets up the form containing the <see cref="TextField"/>s and the <see cref="Button"/>s.
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
				Padding = new Margins (5, 5, 0, 0),
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

			this.BeneficiaryIbanErrorStaticText = new StaticText ()
			{
				Dock = DockStyle.Stacked,
				Parent = beneficiaryIbanGroupBox,
				PreferredWidth = 200,
			};

			GroupBox beneficiaryAddressGroupBox = new GroupBox ()
			{
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				Dock = DockStyle.Stacked,
				Margins = new Margins (10, 10, 0, 10),
				Padding = new Margins (5, 5, 0, 0),
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

			this.BeneficiaryAddressErrorStaticText = new StaticText ()
			{
				Dock = DockStyle.Stacked,
				Parent = beneficiaryAddressGroupBox,
				PreferredWidth = 200,
			};

			GroupBox reasonGroupBox = new GroupBox ()
			{
				ContainerLayoutMode = ContainerLayoutMode.VerticalFlow,
				Dock = DockStyle.Stacked,
				Margins = new Margins (10, 10, 0, 10),
				Padding = new Margins (5, 5, 0, 0),
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

			this.ReasonErrorStaticText = new StaticText ()
			{
				Dock = DockStyle.Stacked,
				Parent = reasonGroupBox,
				PreferredWidth = 200,
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
		/// Sets up the <see cref="BvWidget"/> associated with this instance.
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
				PreferredSize = new Size (578, 292),
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
					throw new System.Exception (string.Format ("A bv value is defined more than once: {0}", name));
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
		/// Sets up the events related to the <see cref="Button"/>s or to the
		/// <see cref="TextField"/>s.
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
				BvHelper.BuildNormalizedReason (FormattedText.Unescape (this.ReasonTextField.Text));

				if (this.CheckReason ())
				{
					this.BvWidget.Reason = FormattedText.Unescape(this.ReasonTextField.Text);
				}

				this.CheckPrintButtonEnbled ();
			};

			if (this.AdminMode)
			{
				this.OptionsButton.Clicked += (sender, e) => this.ShowPrinterManagerDialog ();
			}

			this.PrintButton.Clicked += (sender, e) => this.ShowPrintDialog ();
		}

		/// <summary>
		/// Sets up the validators of the <see cref="TextField"/>s.
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
		/// Checks that the text of <see cref="Application.BenefeciaryIbanTextField"/>
		/// is valid.
		/// </summary>
		/// <returns>A <see cref="bool"/> indicating if the text of <see cref="Application.BenefeciaryIbanTextField"/> is valid or not.</returns>
		protected bool CheckBeneficiaryIban()
		{
			string text = FormattedText.Unescape (this.BenefeciaryIbanTextField.Text);
			string iban = BvHelper.BuildNormalizedIban (text);
			string ibanNoSpace = iban.Replace (" ", "");

			bool validLength = ibanNoSpace.Length == 21;
			bool noLetters = validLength ? Regex.IsMatch (ibanNoSpace.Substring (ibanNoSpace.Length - 12, 12), "^[0-9]*$") : true;
			bool validIban = BvHelper.CheckBeneficiaryIban (iban);

			if (!validLength)
			{
				this.BeneficiaryIbanErrorStaticText.Text = "L'iban doit contenir 21 caractères.";
			}
			else if (!noLetters)
			{
				this.BeneficiaryIbanErrorStaticText.Text = "L'iban doit se terminer par 12 chiffres.";
			}
			else if (!validIban)
			{
				this.BeneficiaryIbanErrorStaticText.Text = "L'iban est invalide.";
			}
			else
			{
				this.BeneficiaryIbanErrorStaticText.Text = "L'iban est valide.";
			}

			return validIban && validLength && noLetters;
		}

		/// <summary>
		/// Checks the text of <see cref="Application.BeneficiaryAddressTextField"/>
		/// is valid.
		/// </summary>
		/// <returns>A <see cref="bool"/> indicating if the text of <see cref="Application.BeneficiaryAddressTextField"/> is valid or not.</returns>
		protected bool CheckBeneficiaryAddress()
		{
			string address = FormattedText.Unescape (this.BeneficiaryAddressTextField.Text);
			bool valid = BvHelper.CheckBeneficiaryAddress (address);

			string[] lines = address.Split ('\n');

			if (address.Length == 0)
			{
				this.BeneficiaryAddressErrorStaticText.Text = "L'adresse doit être remplie.";
			}
			else if (lines.Count() > 4)
			{
				this.BeneficiaryAddressErrorStaticText.Text = "L'adresse doit avoir moins de 5 lignes.";
			}
			else if (System.Array.Exists(lines, line => line.Length > 27))
			{
				this.BeneficiaryAddressErrorStaticText.Text = "Une ligne doit avoir moins de 28 lettres.";
			}
			else
			{
				this.BeneficiaryAddressErrorStaticText.Text = "L'adresse est valide.";
			}

			return valid;
		}

		/// <summary>
		/// Checks the text of <see cref="Application.ReasonTextField"/>
		/// is valid.
		/// </summary>
		/// <returns>A <see cref="bool"/> indicating if the text of <see cref="Application.ReasonTextField"/> is valid or not.</returns>
		protected bool CheckReason()
		{
			string text = FormattedText.Unescape (this.ReasonTextField.Text);
			string reason = BvHelper.BuildNormalizedReason (text);
			bool valid = BvHelper.CheckReason (reason);

			string[] lines = text.Split('\n');

			if (!valid && lines.Count () > 3)
			{
				this.ReasonErrorStaticText.Text = "La raison doit avoir moins de 3 lignes.";
			}
			else if (!valid && System.Array.Exists (lines, line => line.Length > 10))
			{
				this.ReasonErrorStaticText.Text = "Une ligne doit avoir moins de 10 lettres.";
			}
			else if (!valid)
			{
				this.ReasonErrorStaticText.Text = "La raison est invalide.";
			}
			else
			{
				this.ReasonErrorStaticText.Text = "La raison est valide.";
			}

			return valid;
		}

		/// <summary>
		/// Enables or disables <see cref="Application.PrintButton"/> based on the validity
		/// of the <see cref="TextField"/>s.
		/// </summary>
		/// <remarks>
		/// This method is called whenever the text of one of the <see cref="TextField"/> changes.
		/// </remarks>
		protected void CheckPrintButtonEnbled()
		{
			this.PrintButton.Enable =  BvHelper.CheckBv (this.BvWidget)
									&& this.CheckBeneficiaryIban ()
									&& this.CheckBeneficiaryAddress ()
									&& this.CheckReason ();
		}

		/// <summary>
		/// Shows the modal <see cref="PrinterManagerDialog"/> associated to this instance.
		/// </summary>
		public void ShowPrinterManagerDialog()
		{
			this.PrintersManager = new PrinterManagerDialog (this);
			this.PrintersManager.OpenDialog ();
		}

		/// <summary>
		/// Shows the modal <see cref="PrintDialog"/> associated to this instance.
		/// </summary>
		public void ShowPrintDialog()
		{
			List<Printer> printers = new List<Printer>
			(
				from printer in Printer.Load ()
				where PrinterSettings.InstalledPrinters.Contains (printer.Name)
				select printer
			);

			bool checkPrintersList = (printers.Count > 0);
			
			if (!checkPrintersList)
			{
				MessageDialog.CreateOk ("Erreur", DialogIcon.Warning, "Aucune imprimante n'est configurée pour cet ordinateur.").OpenDialog ();
			}
			else
			{
				PrintDialog printDialog = new PrintDialog (this, this.BvWidget, printers);
				printDialog.OpenDialog ();
			}
		}
	}	

}

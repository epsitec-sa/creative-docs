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
			this.SetupError ();
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

		public override string ApplicationIdentifier
		{
			get
			{
				return "EpBanquePiguetBV";
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
		/// Gets or sets the <see cref="TextFieldMulti"/> used for the beneficiary address.
		/// </summary>
		/// <value>The <see cref="TextFieldMulti"/> used for the beneficiary address.</value>
		protected TextFieldMulti BeneficiaryAddressTextField
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the <see cref="TextField"/> used for the amount.
		/// </summary>
		/// <value>The <see cref="TextField"/> used for the amount.</value>
		protected TextField AmountTextField
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the <see cref="TextFieldMulti"/> used for the name and address of the person
		/// who pays.
		/// </summary>
		/// <value>The <see cref="TextFieldMulti"/> used for the name and address of the person who pays.</value>
		protected TextFieldMulti PayedByTextField
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
		/// Gets or sets the <see cref="StaticText"/> used to display errors.
		/// </summary>
		/// <value>The <see cref="StaticText"/> used for the errors.</value>
		protected StaticText ErrorStaticText
		{
			get;
			set;
		}


		/// <summary>
		/// Gets or sets the <see cref="Dictionary"/> which stores the error messages.
		/// </summary>
		/// <value>The <see cref="Dictionary"/> which stores the error messages.</value>
		protected Dictionary<Widget, string> ErrorMessages
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
		/// Gets or sets the <see cref="Button"/> used to erase the bv.
		/// </summary>
		/// <value>The <see cref="Button"/> used to erase the bv.</value>
		protected Button EraseButton
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
			this.Window.MakeMinimizableFixedSizeWindow ();
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
				Parent = this.Window.Root,
			};

			new StaticText ()
			{
				Dock = DockStyle.Stacked,
				Margins = new Margins (15, 5, 5, 0),
				Parent = formFrameBox,
				Text = "N° IBAN du bénéficiaire",
			};

			this.BenefeciaryIbanTextField = new TextField ()
			{
				Dock = DockStyle.Stacked,
				Margins = new Margins (5, 5, 2, 0),
				Parent = formFrameBox,
				PreferredWidth = 200,
				TabIndex = 1,
			};
			this.BenefeciaryIbanTextField.Focus ();

			new StaticText ()
			{
				Dock = DockStyle.Stacked,
				Margins = new Margins (15, 5, 5, 0),
				Parent = formFrameBox,
				Text = "Nom et adresse du bénéficiaire",
			};

			this.BeneficiaryAddressTextField = new TextFieldMulti ()
			{
				Dock = DockStyle.Stacked,
				Margins = new Margins (5, 5, 2, 0),
				Parent = formFrameBox,
				PreferredHeight = 65,
				PreferredWidth = 200,
				ScrollerVisibility = false,
				TabIndex = 2,
			};

			new StaticText ()
			{
				Dock = DockStyle.Stacked,
				Margins = new Margins (15, 5, 5, 0),
				Parent = formFrameBox,
				Text = "Montant",
			};

			this.AmountTextField = new TextField ()
			{
				Dock = DockStyle.Stacked,
				Margins = new Margins (5, 5, 2, 0),
				Parent = formFrameBox,
				PreferredWidth = 200,
				TabIndex = 3,
			};

			new StaticText ()
			{
				Dock = DockStyle.Stacked,
				Margins = new Margins (15, 5, 5, 0),
				Parent = formFrameBox,
				Text = "Versé par",
			};

			this.PayedByTextField = new TextFieldMulti ()
			{
				Dock = DockStyle.Stacked,
				Margins = new Margins (5, 5, 2, 0),
				Parent = formFrameBox,
				PreferredHeight = 52,
				PreferredWidth = 200,
				ScrollerVisibility = false,
				TabIndex = 4,
			};

			new StaticText ()
			{
				Dock = DockStyle.Stacked,
				Margins = new Margins (15, 5, 5, 0),
				Parent = formFrameBox,
				Text = "Motif du versement",
			};

			this.ReasonTextField = new TextFieldMulti ()
			{
				Dock = DockStyle.Stacked,
				Margins = new Margins (5, 5, 2, 0),
				Parent = formFrameBox,
				PreferredHeight = 52,
				PreferredWidth = 200,
				ScrollerVisibility = false,
				TabIndex = 5,
			};

			if (this.AdminMode)
			{
				FrameBox buttonsFrameBox1 = new FrameBox ()
				{
					ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
					Dock = DockStyle.Stacked,
					Parent = formFrameBox,
					TabIndex = 6,
				};
				
				this.OptionsButton = new Button ()
				{
					Dock = DockStyle.Stacked,
					Margins = new Margins (5, 5, 5, 0),
					Parent = buttonsFrameBox1,
					PreferredWidth = 100,
					Text = "Options",
					TabIndex = 1,
				};
			}

			FrameBox buttonsFrameBox2 = new FrameBox ()
			{
				ContainerLayoutMode = ContainerLayoutMode.HorizontalFlow,
				Dock = DockStyle.Stacked,
				Parent = formFrameBox,
				TabIndex = 7,
			};

			this.PrintButton = new Button ()
			{
				Dock = DockStyle.Stacked,
				Margins = new Margins (5, 50, 5, 24),
				Parent = buttonsFrameBox2,
				PreferredWidth = 100,
				Text = "Imprimer",
				TabIndex = 2,
			};

			this.EraseButton = new Button ()
			{
				Dock = DockStyle.Stacked,
				Margins = new Margins (0, 5, 5, 24),
				Parent = buttonsFrameBox2,
				PreferredWidth = 50,
				Text = "Effacer",
				TabIndex = 3,
			};
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
				Margins = new Margins (0, 5, 5, 24),
				Parent = this.Window.Root,
				PreferredSize = AdminMode ? new Size(735, 371) : new Size (667, 337),
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
		/// Sets up the errors mechanism for the <see cref="TextField"/>s.
		/// </summary>
		protected void SetupError()
		{
			this.ErrorStaticText = new StaticText ()
			{
				Dock = DockStyle.StackEnd,
				Margins = new Margins (5, 5, 5, 5),
				Parent = this.Window.Root,
			};

			this.ErrorMessages = new Dictionary<Widget, string> ();
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
				this.BvWidget.BeneficiaryIban = FormattedText.Unescape (this.BenefeciaryIbanTextField.Text);
				this.CheckPrintButtonEnbled ();
			};

			this.BeneficiaryAddressTextField.TextChanged += (sender) =>
			{
				this.BvWidget.BeneficiaryAddress = FormattedText.Unescape (this.BeneficiaryAddressTextField.Text);
				this.CheckPrintButtonEnbled ();
			};

			this.AmountTextField.TextChanged += (sender) =>
			{
				this.BvWidget.Amount = FormattedText.Unescape (this.AmountTextField.Text);
				this.CheckPrintButtonEnbled ();
			};

			this.PayedByTextField.TextChanged += (sender) =>
			{
				this.BvWidget.PayedBy = FormattedText.Unescape (this.PayedByTextField.Text);
				this.CheckPrintButtonEnbled ();
			};

			this.ReasonTextField.TextChanged += (sender) =>
			{
				this.BvWidget.Reason = FormattedText.Unescape(this.ReasonTextField.Text);
				this.CheckPrintButtonEnbled ();
			};

			this.PrintButton.Clicked += (sender, e) => this.ShowPrintDialog ();

			this.EraseButton.Clicked += (sender, e) => this.Erase ();

			if (this.AdminMode)
			{
				this.OptionsButton.Clicked += (sender, e) => this.ShowPrinterManagerDialog ();
			}
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
				this.AmountTextField,
				() => this.CheckAmount ()
			);

			new PredicateValidator (
				this.PayedByTextField,
				() => this.CheckPayedBy ()
			);
			
			new PredicateValidator (
				this.ReasonTextField,
				() => this.CheckReason ()
			);
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
			bool valid = (validIban && validLength && noLetters);

			if (!valid)
			{
				this.ErrorMessages[this.BenefeciaryIbanTextField] = BvHelper.GetErrorMessageForBenefeciaryIban (iban);
			}
			else if (this.ErrorMessages.ContainsKey(this.BenefeciaryIbanTextField))
			{
				this.ErrorMessages.Remove (this.BenefeciaryIbanTextField);
			}

			this.UpdateErrorMessages ();
			System.Console.WriteLine (this.BvWidget.ActualHeight );
			return valid;
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

			if (!valid)
			{
				this.ErrorMessages[this.BeneficiaryAddressTextField] = BvHelper.GetErrorMessageForBeneficiaryAddress (address);
			}
			else if (this.ErrorMessages.ContainsKey (this.BeneficiaryAddressTextField))
			{
				this.ErrorMessages.Remove (this.BeneficiaryAddressTextField);
			}

			this.UpdateErrorMessages ();

			return valid;
		}

		/// <summary>
		/// Checks the text of <see cref="Application.AmountTextField"/>
		/// is valid.
		/// </summary>
		/// <returns>A <see cref="bool"/> indicating if the text of <see cref="Application.AmountTextField"/> is valid or not.</returns>
		public bool CheckAmount()
		{
			string text = FormattedText.Unescape (this.AmountTextField.Text);
			string amount = BvHelper.BuildNormalizedAmount (text);

			bool valid = BvHelper.CheckAmount (amount);

			if (!valid)
			{
				this.ErrorMessages[this.AmountTextField] = BvHelper.GetErrorMessageForAmount (amount);
			}
			else if (this.ErrorMessages.ContainsKey (this.AmountTextField))
			{
				this.ErrorMessages.Remove (this.AmountTextField);
			}

			this.UpdateErrorMessages ();

			return valid;
		}

		// <summary>
		/// Checks the text of <see cref="Application.PayedByTextField"/>
		/// is valid.
		/// </summary>
		/// <returns>A <see cref="bool"/> indicating if the text of <see cref="Application.PayedByTextField"/> is valid or not.</returns>
		public bool CheckPayedBy()
		{
			string payedBy = FormattedText.Unescape (this.PayedByTextField.Text);

			bool valid = BvHelper.CheckPayedBy (payedBy);

			if (!valid)
			{
				this.ErrorMessages[this.PayedByTextField] = BvHelper.GetErrorMessageForPayedBy (payedBy);
			}
			else if (this.ErrorMessages.ContainsKey (this.PayedByTextField))
			{
				this.ErrorMessages.Remove (this.PayedByTextField);
			}

			this.UpdateErrorMessages ();

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

			if (!valid)
			{
				this.ErrorMessages[this.ReasonTextField] = BvHelper.GetErrorMessageForReason (reason);
			}
			else if (this.ErrorMessages.ContainsKey (this.ReasonTextField))
			{
				this.ErrorMessages.Remove (this.ReasonTextField);
			}

			this.UpdateErrorMessages ();

			return valid;
		}


		/// <summary>
		/// Updates the value of <see cref="ErrorStaticText"/>.
		/// </summary>
		protected void UpdateErrorMessages()
		{
			string error = "";

			foreach (Widget widget in this.ErrorMessages.Keys)
			{
				error += this.ErrorMessages[widget] + "    ";
			}

			this.ErrorStaticText.Text = error.Trim ();
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
									&& this.CheckAmount ()
									&& this.CheckPayedBy ()
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

		/// <summary>
		/// Erases the values of the bv.
		/// </summary>
		public void Erase()
		{
			this.BenefeciaryIbanTextField.Text = "";
			this.BeneficiaryAddressTextField.Text = "";
			this.AmountTextField.Text = "";
			this.PayedByTextField.Text = "";
			this.ReasonTextField.Text = "";
		}

	}	

}

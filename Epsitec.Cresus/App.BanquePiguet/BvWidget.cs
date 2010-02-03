//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Widgets;
using Epsitec.Common.Drawing;

using System.Collections.Generic;
using System.Globalization;
using System.Reflection;
using System.Xml;
using System.Xml.Linq;

namespace Epsitec.App.BanquePiguet
{

	/// <summary>
	/// The class BvWidget represents a bv and can display it either on the screen or on paper.
	/// </summary>
	class BvWidget : Widget
	{

		/// <summary>
		/// The BvFieldId enum represents the set of the BvFields of a bv.
		/// </summary>
		protected enum BvFieldId {
			/// <summary>
			/// The address of the bank on the left part.
			/// </summary>
			BankAddress1,
			/// <summary>
			/// The address of the bank on the right part.
			/// </summary>
			BankAddress2,
			/// <summary>
			/// The iban of the beneficiary on the left part.
			/// </summary>
			BeneficiaryIban1,
			/// <summary>
			/// The iban of the beneficiary on the right part.
			/// </summary>
			BeneficiaryIban2,
			/// <summary>
			/// The address of the benficiary on the left part.
			/// </summary>
			BeneficiaryAddress1,
			/// <summary>
			/// The address of the beneficiary on the right part.
			/// </summary>
			BeneficiaryAddress2,
			/// <summary>
			/// The bank account on the left part.
			/// </summary>
			BankAccount1,
			/// <summary>
			/// The bank account on the right part.
			/// </summary>
			BankAccount2,
			/// <summary>
			/// The layout code of the bv.
			/// </summary>
			LayoutCode,
			/// <summary>
			/// The reason of the transfer.
			/// </summary>
			Reason,
			/// <summary>
			/// The reference line of the bv.
			/// </summary>
			ReferenceLine,
			/// <summary>
			/// The clearing line of the bv.
			/// </summary>
			ClearingLine,
			/// <summary>
			/// The ccp number line of the bv.
			/// </summary>
			CcpNumberLine
		}

		/// <summary>
		/// Initializes a new instance of the BvWidget class.
		/// </summary>
		public BvWidget() : this (null)
		{
		}

		/// <summary>
		/// Initializes a new instance of the BvWidget class.
		/// </summary>
		/// <param name="embedder">The embedder.</param>
		public BvWidget(Widget embedder) : base (embedder)
		{
			this.SetupAttributes ();
			this.SetupBackgroundImage ();
			this.SetupBvFields ();
		}

		/// <summary>
		/// Gets or sets the bank address.
		/// </summary>
		/// <value>The bank address.</value>
		/// <exception cref="System.ArgumentException">If the value is not valid.</exception>
		public string BankAddress
		{
			get
			{
				return this.BvFields[BvFieldId.BankAddress1].Text;
			}

			set
			{
				if (!BvHelper.CheckBankAddress (value))
				{
					throw new System.ArgumentException (string.Format ("The provided value is not valid: {0}", value));
				}

				this.BvFields[BvFieldId.BankAddress1].Text = value;
				this.BvFields[BvFieldId.BankAddress2].Text = value;

				this.Invalidate ();
			}
		}

		/// <summary>
		/// Gets or sets the beneficiary iban.
		/// </summary>
		/// <value>The beneficiary iban.</value>
		/// <exception cref="System.ArgumentException">If the value is not valid.</exception>
		public string BeneficiaryIban
		{
			get
			{
				return this.BvFields[BvFieldId.BeneficiaryIban1].Text;
			}

			set
			{
				string iban = BvHelper.BuildNormalizedIban (value);

				if (!BvHelper.CheckBeneficiaryIban (iban))
				{
					throw new System.ArgumentException (string.Format ("The provided value is not valid: {0}", value));
				}

				this.BvFields[BvFieldId.BeneficiaryIban1].Text = iban;
				this.BvFields[BvFieldId.BeneficiaryIban2].Text = iban;

				this.UpdateReferenceLine ();
				this.Invalidate ();
			}
		}

		/// <summary>
		/// Gets or sets the beneficiary address.
		/// </summary>
		/// <value>The beneficiary address.</value>
		/// <exception cref="System.ArgumentException">If the value is not valid.</exception>
		public string BeneficiaryAddress
		{
			get
			{
				return this.BvFields[BvFieldId.BeneficiaryAddress1].Text;
			}
			set
			{
				if (!BvHelper.CheckBeneficiaryAddress (value))
				{
					throw new System.ArgumentException (string.Format ("The provided value is not valid: {0}", value));
				}

				this.BvFields[BvFieldId.BeneficiaryAddress1].Text = value;
				this.BvFields[BvFieldId.BeneficiaryAddress2].Text = value;

				this.Invalidate ();
			}
		}

		/// <summary>
		/// Gets or sets the bank account.
		/// </summary>
		/// <value>The bank account.</value>
		/// <exception cref="System.ArgumentException">If the value is not valid.</exception>
		public string BankAccount
		{
			get
			{
				return this.BvFields[BvFieldId.BankAccount1].Text;
			}
			set
			{
				if (!BvHelper.CheckBankAccount (value))
				{
					throw new System.ArgumentException (string.Format ("The provided value is not valid: {0}", value));
				}

				this.BvFields[BvFieldId.BankAccount1].Text = value;
				this.BvFields[BvFieldId.BankAccount2].Text = value;

				this.Invalidate ();
			}
		}

		/// <summary>
		/// Gets or sets the layout code.
		/// </summary>
		/// <value>The layout code.</value>
		/// <exception cref="System.ArgumentException">If the value is not valid.</exception>
		public string LayoutCode
		{
			get
			{
				return this.BvFields[BvFieldId.LayoutCode].Text;
			}
			set
			{
				if (!BvHelper.CheckLayoutCode (value))
				{
					throw new System.ArgumentException (string.Format ("The provided value is not valid: {0}", value));
				}

				this.BvFields[BvFieldId.LayoutCode].Text = value;

				this.Invalidate ();
			}
		}

		/// <summary>
		/// Gets or sets the reason of the transfer.
		/// </summary>
		/// <value>The reason of the transfer.</value>
		/// <exception cref="System.ArgumentException">If the value is not valid.</exception>
		public string Reason
		{
			get
			{
				return this.BvFields[BvFieldId.Reason].Text;
			}
			set
			{
				if (!BvHelper.CheckReason (value))
				{
					throw new System.ArgumentException (string.Format ("The provided value is not valid: {0}", value));
				}

				this.BvFields[BvFieldId.Reason].Text = value;
				
				this.Invalidate ();
			}
		}

		/// <summary>
		/// Gets or sets the reference client number.
		/// </summary>
		/// <value>The reference client number.</value>
		/// <exception cref="System.ArgumentException">If the value is not valid.</exception>
		public string ReferenceClientNumber
		{
			get
			{
				return this.referenceClientNumber;
			}
			set
			{
				if (!BvHelper.CheckReferenceClientNumber (value))
				{
					throw new System.ArgumentException (string.Format ("The provided value is not valid: {0}", value));
				}

				this.referenceClientNumber = value;

				this.UpdateReferenceLine ();
			}
		}

		/// <summary>
		/// Gets or sets the clearing constant.
		/// </summary>
		/// <value>The clearing constant.</value>
		/// <exception cref="System.ArgumentException">If the value is not valid.</exception>
		public string ClearingConstant
		{
			get
			{
				return this.clearingConstant;
			}
			set
			{
				if (!BvHelper.CheckClearingConstant (value))
				{
					throw new System.ArgumentException (string.Format ("The provided value is not valid: {0}", value));
				}

				this.clearingConstant = value;

				this.UpdateClearingLine ();
			}
		}

		/// <summary>
		/// Gets or sets the clearing number of the bank.
		/// </summary>
		/// <value>The clearing number of the bank.</value>
		/// <exception cref="System.ArgumentException">If the value is not valid.</exception>
		public string ClearingBank
		{
			get
			{
				return this.clearingBank;
			}
			set
			{
				if (!BvHelper.CheckClearingBank (value))
				{
					throw new System.ArgumentException (string.Format ("The provided value is not valid: {0}", value));
				}

				this.clearingBank = value;

				this.UpdateClearingLine ();
			}
		}

		/// <summary>
		/// Gets or sets the control key of the clearing number of the bank.
		/// </summary>
		/// <value>The control key of the clearing number of the bank.</value>
		/// <exception cref="System.ArgumentException">If the value is not valid.</exception>
		public string ClearingBankKey
		{
			get
			{
				return this.clearingBankKey;
			}
			set
			{
				if (!BvHelper.CheckClearingBankKey (value))
				{
					throw new System.ArgumentException (string.Format ("The provided value is not valid: {0}", value));
				}

				this.clearingBankKey = value;

				this.UpdateClearingLine ();
			}
		}


		/// <summary>
		/// Gets or sets the CCP number of the bank.
		/// </summary>
		/// <value>The CCP number of the bank.</value>
		/// <exception cref="System.ArgumentException">If the value is not valid.</exception>
		public string CcpNumber
		{
			get
			{
				return this.ccpNumber;
			}
			set
			{
				if (!BvHelper.CheckBeneficiaryAddress (value))
				{
					throw new System.ArgumentException (string.Format ("The provided value is not valid: {0}", value));
				}

				this.ccpNumber = value;

				this.UpdateCcpNumberLine ();
			}
		}

		/// <summary>
		/// Gets or sets the size of the bv (in millimeters).
		/// </summary>
		/// <value>The size of the bv.</value>
		public Size BvSize
		{
			get;
			protected set;
		}

		/// <summary>
		/// Gets or sets the dictionnary containing the BvFields.
		/// </summary>
		/// <value>The dictionnary containing the BvFields.</value>
		protected Dictionary<BvFieldId, BvField> BvFields
		{
			get;
			set;
		}

		/// <summary>
		/// Gets or sets the background image of the bv.
		/// </summary>
		/// <value>The background image of the bv.</value>
		protected Bitmap BackgroundImage
		{
			get;
			set;
		}

		/// <summary>
		/// Prints the bv on paintPort and the bounds on the bv. This method is to use only
		/// to print it on paper as it will print the BvFields but not the background.
		/// </summary>
		/// <param name="paintPort">The IPaintPort to use.</param>
		/// <param name="bounds">The bounds of the bv.</param>
		public void Print(IPaintPort paintPort, Rectangle bounds)
		{

#warning Remove me at the end of tests.

			using (Path path = Path.FromRectangle (bounds))
			{
				paintPort.Color = Color.FromRgb (0, 0, 0);
				paintPort.LineWidth = 0.01;
				paintPort.PaintOutline (path);
			}

			this.PaintBvFields (paintPort, bounds);
		}

		/// <summary>
		/// Paints the bv on graphics. This method is to use only to paint it on screen
		/// as it will print the background image in addition to the BvFields.
		/// </summary>
		/// <param name="graphics">The graphics to use.</param>
		/// <param name="clipRect">The clip rectangle.</param>
		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			Rectangle bounds = this.Client.Bounds;

			this.PaintBackgroundImage (graphics, bounds);
			this.PaintBvFields (graphics, bounds);

		}

		/// <summary>
		/// Paints the background image of the bv.
		/// </summary>
		/// <param name="port">The port to use.</param>
		/// <param name="bounds">The bounds of the bv.</param>
		protected void PaintBackgroundImage(IPaintPort port, Rectangle bounds)
		{
			port.PaintImage (this.BackgroundImage, bounds);

			using (Path path = Path.FromRectangle (bounds))
			{
				port.Color = Color.FromRgb (0, 0, 0);
				port.PaintOutline (path);
			}
		}

		/// <summary>
		/// Paints the BvFields using paintPort and the bounds of the bv.
		/// </summary>
		/// <param name="paintPort">The IPaintPort to use.</param>
		/// <param name="bounds">The bounds.</param>
		protected void PaintBvFields(IPaintPort paintPort, Rectangle bounds)
		{

			foreach (BvFieldId fieldId in this.BvFields.Keys)
			{
				this.BvFields[fieldId].Paint (paintPort, bounds);
			}

		}

		/// <summary>
		/// Sets up the BvFields by loading them out of the resource file.
		/// </summary>
		/// <remarks>
		/// This method is called when the BvWidget is initialized.
		/// </remarks>
		/// <exception cref="System.Exception">If some Bvfields are missing.</exception>
		protected void SetupBvFields()
		{
			this.BvFields = new Dictionary<BvFieldId, BvField> ();

			XElement xBvDefinition;

			using (XmlReader xmlReader = XmlReader.Create (Tools.GetResourceStream ("BvDefinition.xml")))
			{
				xBvDefinition = XElement.Load (xmlReader);
			}

			XElement xSize = xBvDefinition.Element ("size");
			this.BvSize = new Size (
				double.Parse (xSize.Element ("width").Value, CultureInfo.InvariantCulture),
				double.Parse (xSize.Element ("height").Value, CultureInfo.InvariantCulture)
			);

			IEnumerable<XElement> xBvfields = xBvDefinition.Element ("fields").Elements ("field");

			foreach (XElement xBvField in xBvfields)
			{
				string name = (string) xBvField.Element ("name");
				BvFieldId id = (BvFieldId) System.Enum.Parse (typeof (BvFieldId), name);
				this.BvFields[id] = BvField.GetInstance (xBvField, this.BvSize);
			}


			if (this.BvFields.Count < System.Enum.GetValues (typeof (BvFieldId)).Length)
			{
				throw new System.Exception ("Some BvFields are missing.");
			}
		}

		/// <summary>
		/// Sets up the background image.
		/// </summary>
		/// <remarks>
		/// This method is called when the BvWidget is initialized.
		/// </remarks>
		protected void SetupBackgroundImage()
		{
			Assembly assembly = Assembly.GetExecutingAssembly ();
			string resource = "Epsitec.App.BanquePiguet.Resources.bv.jpg";

			this.BackgroundImage = (Bitmap) Bitmap.FromManifestResource (resource, assembly);
		}

		/// <summary>
		/// Sets up the string attributes in order to give them an empty initial value.
		/// </summary>
		/// <remarks>
		/// This method is called when the BvWidget is initialized.
		/// </remarks>
		protected void SetupAttributes()
		{
			this.referenceClientNumber = "";
			this.clearingConstant = "";
			this.clearingBank = "";
			this.clearingBankKey = "";
			this.ccpNumber = "";
		}


		/// <summary>
		/// Updates the text of the reference line based on the values of ReferenceClientNumber and BeneficiaryIban.
		/// </summary>
		/// <remarks>
		/// This method is called whenever the text of ReferenceClientNumber and BeneficiaryIban change.
		/// </remarks>
		protected void UpdateReferenceLine()
		{
			string iban = this.BeneficiaryIban;
			string reference = this.ReferenceClientNumber;

			if (BvHelper.CheckReferenceLine (iban, reference))
			{
				this.BvFields[BvFieldId.ReferenceLine].Text = BvHelper.BuildReferenceLine (iban, reference);
				this.Invalidate ();
			}
		}

		/// <summary>
		/// Updates the text of the clearing line based on the values of ClearingConstant, ClearingBank and
		/// ClearingBankKey.
		/// </summary>
		/// <remarks>
		/// This method is called whenever the text of ClearingConstant, ClearingBank and ClearingBankKey
		/// change.
		/// </remarks>
		protected void UpdateClearingLine()
		{
			string constant = this.ClearingConstant;
			string clearing = this.ClearingBank;
			string key = this.ClearingBankKey;

			if (BvHelper.CheckClearingLine(constant, clearing, key))
			{
				this.BvFields[BvFieldId.ClearingLine].Text = BvHelper.BuildClearingLine (constant, clearing, key);
				this.Invalidate ();
			}
		}

		/// <summary>
		/// Updates the text of the CCP number line based on the value of CcpNumber.
		/// </summary>
		/// <remarks>
		/// This method is called whenver the text of CcpNumber changes.
		/// </remarks>
		protected void UpdateCcpNumberLine()
		{
			string ccp = this.CcpNumber;

			if (BvHelper.CheckCcpNumber (ccp))
			{
				this.BvFields[BvFieldId.CcpNumberLine].Text = BvHelper.BuildCcpNumberLine (ccp);
				this.Invalidate ();
			}
		}


		/// <summary>
		/// This attribute backs the ReferenceClientNumber property.
		/// </summary>
		protected string referenceClientNumber;
		
		/// <summary>
		/// This attributes backs the ClearingConstant property.
		/// </summary>
		protected string clearingConstant;
		
		/// <summary>
		/// This attribute backs the ClearingBank property.
		/// </summary>
		protected string clearingBank;
		
		/// <summary>
		/// This attribute backs the ClearingBankKey property.
		/// </summary>
		protected string clearingBankKey;
		
		/// <summary>
		/// This attribute backs the CcpNumber property.
		/// </summary>
		protected string ccpNumber;

	}

}

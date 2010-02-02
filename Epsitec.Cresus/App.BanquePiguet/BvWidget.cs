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

	class BvWidget : Widget
	{

		protected enum BvFieldId {
			BankAddress1,
			BankAddress2,
			BeneficiaryIban1,
			BeneficiaryIban2,
			BeneficiaryAddress1,
			BeneficiaryAddress2,
			BankAccount1,
			BankAccount2,
			LayoutCode,
			Reason,
			ReferenceLine,
			ClearingLine,
			CcpNumberLine
		}

		public BvWidget() : this (null)
		{
		}

		public BvWidget(Widget embedder) : base (embedder)
		{
			try
			{
				this.SetupAttributes ();
				this.SetupBackgroundImage ();
				this.SetupBvFields ();
			}
			catch (System.Exception e)
			{
				Tools.Error (new System.Exception ("An error occured while creating the BvWidget.", e));
			}
		}
        
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

		public Size BvSize
		{
			get;
			protected set;
		}

		protected Dictionary<BvFieldId, BvField> BvFields
		{
			get;
			set;
		}

		protected Bitmap BackgroundImage
		{
			get;
			set;
		}

		public void Print(IPaintPort paintPort, Rectangle bounds)
		{

			// Prints the outline of the bv. This is to remove at the end of tests.
			using (Path path = Path.FromRectangle (bounds))
			{
				paintPort.Color = Color.FromRgb (0, 0, 0);
				paintPort.LineWidth = 0.01;
				paintPort.PaintOutline (path);
			}

			this.PaintBvFields (paintPort, bounds);
		}

		protected override void PaintBackgroundImplementation(Graphics graphics, Rectangle clipRect)
		{
			Rectangle bounds = this.Client.Bounds;

			this.PaintBackgroundImage (graphics, bounds);
			this.PaintBvFields (graphics, bounds);

		}

		protected void PaintBackgroundImage(IPaintPort port, Rectangle bounds)
		{
			port.PaintImage (this.BackgroundImage, bounds);

			using (Path path = Path.FromRectangle (bounds))
			{
				port.Color = Color.FromRgb (0, 0, 0);
				port.PaintOutline (path);
			}
		}

		protected void PaintBvFields(IPaintPort paintPort, Rectangle bounds)
		{

			foreach (BvFieldId fieldId in this.BvFields.Keys)
			{
				this.BvFields[fieldId].Paint (paintPort, bounds);
			}

		}

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

		protected void SetupBackgroundImage()
		{
			Assembly assembly = Assembly.GetExecutingAssembly ();
			string resource = "Epsitec.App.BanquePiguet.Resources.bv.jpg";

			this.BackgroundImage = (Bitmap) Bitmap.FromManifestResource (resource, assembly);
		}

		protected void SetupAttributes()
		{
			this.referenceClientNumber = "";
			this.clearingConstant = "";
			this.clearingBank = "";
			this.clearingBankKey = "";
			this.ccpNumber = "";
		}


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

		protected void UpdateCcpNumberLine()
		{
			string ccp = this.CcpNumber;

			if (BvHelper.CheckCcpNumber (ccp))
			{
				this.BvFields[BvFieldId.CcpNumberLine].Text = BvHelper.BuildCcpNumberLine (ccp);
				this.Invalidate ();
			}
		}
		

		protected string referenceClientNumber;
		protected string clearingConstant;
		protected string clearingBank;
		protected string clearingBankKey;
		protected string ccpNumber;

	}

}

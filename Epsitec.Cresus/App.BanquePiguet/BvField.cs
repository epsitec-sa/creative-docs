//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Drawing;

using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace Epsitec.App.BanquePiguet
{

	class BvField
	{

		static BvField()
		{
			using (Stream stream = Assembly.GetExecutingAssembly ().GetManifestResourceStream ("Epsitec.App.BanquePiguet.Resources.OCR_BB.tff"))
			{
				Font.RegisterDynamicFont (stream);
			}
		}
		
		public BvField(XElement xBvField, Size bvSize)
		{
			this.Text = "";
			this.TextRelativeHeight = double.Parse (xBvField.Element ("textHeight").Value, CultureInfo.InvariantCulture) / bvSize.Height;
			this.XRelativePosition = double.Parse (xBvField.Element ("xPosition").Value, CultureInfo.InvariantCulture) / bvSize.Width;
			this.YRelativePosition = double.Parse (xBvField.Element ("yPosition").Value, CultureInfo.InvariantCulture) / bvSize.Height;

			switch (xBvField.Element ("font").Value)
			{
				case "OCR-B1":
					this.TextFont = Font.GetFont ("OCR-B Bold", "Regular");
					break;

				case "Arial":
					this.TextFont = Font.GetFont ("Arial", "Regular");
					break;

				default:
					this.TextFont = Font.DefaultFont;
					break;
			}
		}
        
		public string Text
		{
			get;
			set;
		}

		public double TextRelativeHeight
		{
			get;
			set;
		}

		public Font TextFont
		{
			get;
			set;
		}


		public double XRelativePosition
		{
			get;
			set;
		}

		public double YRelativePosition
		{
			get;
			set;
		}

		public virtual void Paint(IPaintPort port, Rectangle bounds)
		{
			double xPosition = this.ComputeAbsoluteXPosition (bounds);
			double yPosition = this.ComputeAbsoluteYPosition (bounds);
			double textHeight = this.ComputeTextAbsoluteHeight (bounds);

			port.Color = Color.FromRgb (0, 0, 0);
			port.PaintText (xPosition, yPosition, this.Text, this.TextFont, textHeight);
		}

		protected double ComputeAbsoluteXPosition(Rectangle bounds)
		{
			return this.XRelativePosition * bounds.Width;
		}

		protected double ComputeAbsoluteYPosition(Rectangle bounds)
		{
			return this.YRelativePosition * bounds.Height;
		}

		protected double ComputeTextAbsoluteHeight(Rectangle bounds)
		{
			return this.TextRelativeHeight * bounds.Height;
		}

		public static BvField GetInstance(XElement xBvField, Size bvSize)
		{
			string type = (string) xBvField.Element ("type");

			BvField bvField;

			switch (type)
			{
				case "BvField":
					bvField = new BvField (xBvField, bvSize);
					break;
				case "BvFieldMultiLine":
					bvField = new BvFieldMultiLine (xBvField, bvSize);
					break;
				case "BvFieldMultiLineColumn":
					bvField = new BvFieldMultiLineColumn (xBvField, bvSize);
					break;
				default:
					throw new System.Exception (string.Format ("Invalid BvField type: {0}.", type));
			}

			return bvField;

		}

	}

}

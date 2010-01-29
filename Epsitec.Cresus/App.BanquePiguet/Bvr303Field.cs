//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Drawing;

using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Xml.Linq;

namespace Epsitec.App.BanquePiguet
{

	class Bvr303Field
	{

		static Bvr303Field()
		{
			using (Stream stream = Assembly.GetExecutingAssembly ().GetManifestResourceStream ("Epsitec.App.BanquePiguet.Resources.OCR_BB.tff"))
			{
				Font.RegisterDynamicFont (stream);
			}
		}
		
		public Bvr303Field(XElement xBvrField, Size bvrSize)
		{
			this.Text = "";
			this.TextRelativeHeight = Double.Parse (xBvrField.Element ("textHeight").Value, CultureInfo.InvariantCulture) / bvrSize.Height;
			this.XRelativePosition = Double.Parse (xBvrField.Element ("xPosition").Value, CultureInfo.InvariantCulture) / bvrSize.Width;
			this.YRelativePosition = Double.Parse (xBvrField.Element ("yPosition").Value, CultureInfo.InvariantCulture) / bvrSize.Height;

			switch (xBvrField.Element ("font").Value)
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

		public static Bvr303Field GetInstance(XElement xBvrField, Size bvrSize)
		{
			string type = (string) xBvrField.Element ("type");

			Bvr303Field bvrField;

			switch (type)
			{
				case "BvrField":
					bvrField = new Bvr303Field (xBvrField, bvrSize);
					break;
				case "BvrFieldMultiLine":
					bvrField = new Bvr303FieldMultiLine (xBvrField, bvrSize);
					break;
				case "BvrFieldMultiLineColumn":
					bvrField = new Bvr303FieldMultiLineColumn (xBvrField, bvrSize);
					break;
				default:
					throw new Exception (String.Format ("Invalid bvrField type: {0}.", type));
			}

			return bvrField;

		}

	}

}

//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX


using Epsitec.Common.Drawing;

using System.Xml;

namespace Epsitec.App.BanquePiguet
{

	class BvrField
	{

		public BvrField(XmlNode xmlBvrField)
		{
			/*
			 * using (var stream = System.Reflection.Assembly.GetExecutingAssembly ().GetManifestResourceStream ("Epsitec.Cresus.Graph.Resources.futuramc.ttf"))
			{
				Font.RegisterDynamicFont (stream);
			}
			 * */

			this.Text = "";
			this.TextRelativeHeight = double.Parse (xmlBvrField.SelectSingleNode ("textRelativeHeight").InnerText.Trim());
			this.TextFont = Font.DefaultFont;
			this.XRelativePosition = double.Parse (xmlBvrField.SelectSingleNode ("xRelativePosition").InnerText.Trim());
			this.YRelativePosition = double.Parse (xmlBvrField.SelectSingleNode ("yRelativePosition").InnerText.Trim());
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
			double xPosition = this.ComputeAbsoluteXPosition (this.XRelativePosition, bounds);
			double yPosition = this.ComputeAbsoluteYPosition (this.YRelativePosition, bounds);
			double textHeight = this.ComputeTextAbsoluteHeight (bounds);

			port.Color = Color.FromRgb (0, 0, 0);
			port.PaintText (xPosition, yPosition, this.Text, this.TextFont, textHeight);
		}

		protected double ComputeAbsoluteXPosition(double relativeX, Rectangle bounds)
		{
			return relativeX * bounds.Width;
		}

		protected double ComputeAbsoluteYPosition(double relativeY, Rectangle bounds)
		{
			return relativeY * bounds.Height;
		}

		protected double ComputeTextAbsoluteHeight(Rectangle bounds)
		{
			return this.TextRelativeHeight * bounds.Height;
		}

	}

}

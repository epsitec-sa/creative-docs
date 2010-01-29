//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Drawing;

using System;
using System.Globalization;
using System.Xml.Linq;

namespace Epsitec.App.BanquePiguet
{

	class Bvr303FieldMultiLine : Bvr303Field
	{

		public Bvr303FieldMultiLine(XElement xBvrField, Size bvrSize) : base (xBvrField, bvrSize)
		{ 
			this.VerticalSpace = Double.Parse (xBvrField.Element ("verticalSpace").Value, CultureInfo.InvariantCulture) / bvrSize.Height;
		}

		public double VerticalSpace
		{
			get;
			set;
		}

		public override void Paint(IPaintPort port, Rectangle bounds)
		{
			port.Color = Color.FromRgb (0, 0, 0);

			double xPosition = this.ComputeAbsoluteXPosition (bounds);
			double yPositionBase = this.ComputeAbsoluteYPosition (bounds);
			double textHeight = this.ComputeTextAbsoluteHeight (bounds);
			
			string[] ligns = this.Text.Split ('\n');

			for (int i = 0; i < ligns.Length; i++)
			{
				double yPosition = yPositionBase + this.ComputeVerticalOffset (i, bounds);
				port.PaintText (xPosition, yPosition, ligns[i], this.TextFont, textHeight);
			}
		}

		protected double ComputeVerticalOffset(int lignNumber, Rectangle bounds)
		{
			return -lignNumber * this.VerticalSpace * bounds.Height;
		}

	}

}

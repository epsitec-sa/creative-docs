//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Common.Drawing;

using System.Globalization;
using System.Xml.Linq;

namespace Epsitec.App.BanquePiguet
{

	/// <summary>
	/// The BvMultiLineColumn represents a BvField which can contain more than one line and whose
	/// characters are separated by spaces.
	/// </summary>
	class BvFieldMultiLineColumn : BvFieldMultiLine
	{

		/// <summary>
		/// Initializes a new instance of the BvFieldMultiLineColumn class.
		/// </summary>
		/// <param name="xBvField">The XElement containing the definition of the BvFieldMultiLineColumn.</param>
		/// <param name="bvSize">The size of the Bv in millimeters.</param>
		public BvFieldMultiLineColumn(XElement xBvField, Size bvSize) : base (xBvField, bvSize)
		{
			this.RelativeHorizontalSpace = double.Parse (xBvField.Element ("horizontalSpace").Value, CultureInfo.InvariantCulture) / bvSize.Width;
		}

		/// <summary>
		/// Gets or sets the relative (to the bv width) horizontal space within two characters.
		/// </summary>
		/// <value>The relative horizontal space.</value>
		public double RelativeHorizontalSpace
		{
			get;
			set;
		}

		/// <summary>
		/// Paints this instance on port within bounds.
		/// </summary>
		/// <param name="port">The port used to paint</param>
		/// <param name="bounds">The bounds of the bv.</param>
		/// <remarks>
		/// Override this method if you want to change the display of the BvField.
		/// </remarks>
		public override void Paint(IPaintPort port, Rectangle bounds)
		{
			port.Color = Color.FromRgb (0, 0, 0);

			double xPositionBase = this.ComputeAbsoluteXPosition (bounds);
			double yPositionBase = this.ComputeAbsoluteYPosition (bounds);
			double textHeight = this.ComputeTextAbsoluteHeight (bounds);

			string[] lines = this.Text.Split ('\n');

			for (int i = 0; i < lines.Length; i++)
			{
				double yPosition = yPositionBase + this.ComputeAbsoluteVerticalOffset (i, bounds);

				char[] line = lines[i].ToCharArray ();
				
				for (int j = 0; j < line.Length; j++)
				{
					double xPosition = xPositionBase + this.ComputeAbsoluteHorizontalOffset (j, bounds);
					string text = string.Format ("{0}", line[j]);

					port.PaintText (xPosition, yPosition, text, this.TextFont, textHeight);
				}
			}
		}

		/// <summary>
		/// Computes the absolute horizontal offset of the character given by its position
		/// on its line, charNumber.
		/// </summary>
		/// <param name="charNumber">The character number.</param>
		/// <param name="bounds">The bounds of the bv.</param>
		/// <returns>The absolute horizontal offset of the given character.</returns>
		/// <remarks>
		/// Character numbers starts at 0 for the first one, 1 for the one on the right, and so on.
		/// </remarks>
		private double ComputeAbsoluteHorizontalOffset(int charNumber, Rectangle bounds)
		{
			return charNumber * this.RelativeHorizontalSpace * bounds.Width;
		}

	}

}

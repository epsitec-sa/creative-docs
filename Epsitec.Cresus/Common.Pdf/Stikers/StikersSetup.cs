using System;
using System.Collections.Generic;
using System.Linq;
using Epsitec.Common.Drawing;

namespace Epsitec.Common.Pdf.Stikers
{
	public class StikersSetup
	{
		//	+------------------------------------------
		//	|     +

		public StikersSetup()
		{
			this.PageMargins   = new Margins (100.0);
			this.StikerSize    = new Size (620.0, 400.0);
			this.StikerMargins = new Margins (50.0);
			this.StikerGap     = new Size (10.0, 10.0);
			this.FontFace      = "Arial";
			this.FontStyle     = "Regular";
			this.FontSize      = 30.0;
			this.PaintFrame    = false;
		}

		public Margins PageMargins
		{
			set;
			get;
		}

		public Size StikerSize
		{
			set;
			get;
		}

		public Margins StikerMargins
		{
			set;
			get;
		}

		public Size StikerGap
		{
			set;
			get;
		}

		public string FontFace
		{
			set;
			get;
		}

		public string FontStyle
		{
			set;
			get;
		}

		public double FontSize
		{
			set;
			get;
		}

		public bool PaintFrame
		{
			set;
			get;
		}
	}
}

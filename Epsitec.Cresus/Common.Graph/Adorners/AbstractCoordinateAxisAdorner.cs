//	Copyright © 2009, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;

using System.Collections.Generic;

namespace Epsitec.Common.Graph.Adorners
{
	public abstract class AbstractCoordinateAxisAdorner : AbstractAdorner
	{
		public AbstractCoordinateAxisAdorner()
		{
			this.GridColor     = Color.FromBrightness (0);
			this.GridLineWidth = 0.5;
			this.VisibleGrid   = true;
			this.VisibleLabels = true;
			this.VisibleTicks  = true;
		}

		public virtual Styles.CaptionStyle Style
		{
			get
			{
				return this.style ?? CoordinateAxisAdorner.defaultStyle;
			}
			set
			{
				this.style = value;
			}
		}

		public virtual Color GridColor
		{
			get;
			set;
		}

		public virtual double GridLineWidth
		{
			get;
			set;
		}

		public virtual bool VisibleGrid
		{
			get;
			set;
		}

		public virtual bool VisibleLabels
		{
			get;
			set;
		}

		public virtual bool VisibleTicks
		{
			get;
			set;
		}

		
		protected static readonly Styles.CaptionStyle	defaultStyle = new Styles.CaptionStyle ();
		
		protected readonly double extraLength	= 12;
		protected readonly double arrowLength	= 4;
		protected readonly double arrowBreadth	= 3;
		protected readonly double tickLength	= 4;

		private Styles.CaptionStyle style;
	}
}

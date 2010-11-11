//	Copyright © 2003-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Daniel ROUX

using System.Collections.Generic;
using System;

namespace Epsitec.Common.Drawing.Serializers
{
	public abstract class AbstractSerializer
	{
		public AbstractSerializer(int resolution = 2)
		{
			this.resolution = resolution;
		}

		public int Resolution
		{
			get
			{
				return this.resolution;
			}
		}


		public string Serialize(Point p)
		{
			return string.Concat (this.Serialize (p.X), " ", this.Serialize (p.Y));
		}

		public string Serialize(double value)
		{
			double factor = System.Math.Pow (10, this.resolution);
			value = System.Math.Floor (value*factor) / factor;

			return value.ToString (System.Globalization.CultureInfo.InvariantCulture);
		}


		private readonly int resolution = 2;
	}
}

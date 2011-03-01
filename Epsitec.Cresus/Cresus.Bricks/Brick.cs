//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Bricks
{
	public abstract class Brick
	{
		protected Brick()
		{
			this.properties = new List<BrickProperty> ();
		}
		
		internal void AddProperty(BrickProperty property)
		{
			this.properties.Add (property);
		}

		internal void DebugDump(string prefix = "")
		{
			foreach (var property in this.properties)
			{
				if (property.Brick == null)
				{
					System.Diagnostics.Debug.WriteLine (prefix + property.ToString ());
				}
				else
				{
					property.Brick.DebugDump (prefix + property.Key.ToString () + ".");
				}
			}
		}

		private readonly List<BrickProperty> properties;
	}
}
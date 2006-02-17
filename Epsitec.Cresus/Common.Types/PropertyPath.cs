//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	public class PropertyPath
	{
		public PropertyPath()
		{
		}
		public PropertyPath(string path)
		{
			this.path = path;
		}
		public PropertyPath(string path, params Property[] elements)
		{
			this.path = path;
			this.elements = Copier.CopyArray (elements);
		}

		public string Path
		{
			get
			{
				return this.path;
			}
			set
			{
				this.path = value;
			}
		}
		public ReadOnlyArray<Property> Elements
		{
			get
			{
				return new ReadOnlyArray<Property> (this.elements);
			}
			set
			{
				this.elements = value.ToArray ();
			}
		}
		
		private string path;
		private Property[] elements;
	}
}

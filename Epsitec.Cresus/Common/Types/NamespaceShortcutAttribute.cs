//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	[System.AttributeUsage (System.AttributeTargets.Assembly, AllowMultiple=true)]
	public class NamespaceShortcutAttribute : System.Attribute
	{
		public NamespaceShortcutAttribute(string fullName, string shortName)
		{
			this.fullName = fullName;
			this.shortName = shortName;
		}


		public string							FullName
		{
			get
			{
				return this.fullName;
			}
		}

		public string							ShortName
		{
			get
			{
				return this.shortName;
			}
		}

		
		private readonly string					fullName;
		private readonly string					shortName;
	}
}
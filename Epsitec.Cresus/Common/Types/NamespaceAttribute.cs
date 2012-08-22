//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	[System.AttributeUsage (System.AttributeTargets.Assembly, AllowMultiple=false)]
	public class NamespaceAttribute : System.Attribute
	{
		public NamespaceAttribute(string assemblyNamespace)
		{
			this.assemblyNamespace = assemblyNamespace;
		}


		public string							AssemblyNamespace
		{
			get
			{
				return this.assemblyNamespace;
			}
		}


		private readonly string					assemblyNamespace;
	}
}

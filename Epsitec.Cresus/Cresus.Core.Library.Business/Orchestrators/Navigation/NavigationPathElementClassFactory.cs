//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.PlugIns;
using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Orchestrators.Navigation
{
	public class NavigationPathElementClassFactory : PlugInFactory<NavigationPathElement, NavigationPathElementClassAttribute, string>
	{
		public static NavigationPathElement Parse(string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				return null;
			}
			
			if ((value.StartsWith ("<")) &&
				(value.EndsWith (">")))
			{
				string id = value.Substring (1, value.Length-2).FirstToken (":");
				var template = NavigationPathElementClassFactory.CreateInstance (id);

				return template.InternalDeserialize (value.Substring (1, value.Length-2));
			}

			throw new System.FormatException ("Unexpected format");
		}
	}
}

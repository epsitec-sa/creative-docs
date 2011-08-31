//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.PlugIns;
using Epsitec.Common.Support.Extensions;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Orchestrators.Navigation
{
	/// <summary>
	/// The <c>NavigationPathElementClassFactory</c> class is used to instantiate
	/// <see cref="NavigationPathElement"/> instances based on serialized values.
	/// </summary>
	public class NavigationPathElementClassFactory : PlugInFactory<NavigationPathElement, NavigationPathElementClassAttribute, string>
	{
		/// <summary>
		/// Parses the specified value and returns a <see cref="NavigationPathElement"/>.
		/// </summary>
		/// <param name="value">The value.</param>
		/// <returns>The navigation path element.</returns>
		public static NavigationPathElement Parse(string value)
		{
			if (string.IsNullOrEmpty (value))
			{
				return null;
			}
			
			if ((value.StartsWith ("<")) &&
				(value.EndsWith (">")))
			{
				var classId  = value.Substring (1, value.Length-2).FirstToken (":");
				var template = NavigationPathElementClassFactory.GetTemplateInstance (classId);

				return template.InternalDeserialize (value.Substring (1, value.Length-2));
			}

			throw new System.FormatException ("Unexpected format");
		}
	}
}

//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.PlugIns;

using System.Collections.Generic;
using System.Xml.Linq;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>XmlNodeClassFactory</c> class is used to deserialize classes derived from
	/// <typeparamref name="TClass"/>based on an XML element. The classes must be identified
	/// using a <see cref="XmlNodeClassAttribute"/> at the <c>assembly</c> level.
	/// </summary>
	/// <typeparam name="TClass">The type of the base class of the instances created by this factory.</typeparam>
	internal class XmlNodeClassFactory<TClass> : PlugInFactory<TClass, XmlNodeClassAttribute, string>
		where TClass : IXmlNodeClass
	{
		/// <summary>
		/// Restores the class based on the XML element. The class must be declared with an
		/// <see cref="XmlNodeClassAttribute"/> attribute at the <c>assembly</c> level, and
		/// must derive from <typeparamref name="TClass"/>.
		/// </summary>
		/// <param name="xml">The XML element.</param>
		/// <returns>The class instance.</returns>
		public static TClass Restore(XElement xml)
		{
			if (xml == null)
			{
				return default (TClass);
			}

			string xmlNodeName = xml.Name.LocalName;

			return XmlNodeClassFactory<TClass>.CreateInstance (xmlNodeName, xml);
		}
	}
}
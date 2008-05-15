//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Support.PlugIns;

using System.Collections.Generic;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core.States
{
	public class StateFactory : PlugInFactory<AbstractState, StateAttribute, string>
	{
		public static AbstractState CreateState(XElement element)
		{
			string className = (string) element.Attribute ("class");
			States.AbstractState state = StateFactory.CreateInstance (className);
			return state.Deserialize (element);
		}
	}
}

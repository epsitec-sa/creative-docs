//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.PlugIns;

using System.Xml.Linq;

namespace Epsitec.Cresus.Core.States
{
	/// <summary>
	/// The <c>StateFactory</c> class manages the creation of state instances
	/// based on their (short) class name. See <see cref="StateManager"/>.
	/// </summary>
	public sealed class StateFactory : PlugInFactory<CoreState, StateAttribute, string>
	{
		/// <summary>
		/// Creates the state instance specified by the XML element. This will
		/// instanciate the proper class and initialize it based on the serialized
		/// data.
		/// </summary>
		/// <param name="manager">The state manager.</param>
		/// <param name="element">The XML element.</param>
		/// <param name="className">Name of the class.</param>
		/// <returns>
		/// The state or <c>null</c> if the element does not map to a supported class.
		/// </returns>
		public static CoreState CreateState(StateManager manager, XElement element, string className)
		{
			States.CoreState state = StateFactory.CreateInstance<StateManager> (className, manager);
			return state.Deserialize (element);
		}

		/// <summary>
		/// Gets the class name for the specified state instance. The name is
		/// defined by a <see cref="StateAttribute"/> attribute.
		/// </summary>
		/// <param name="state">The state instance.</param>
		/// <returns>The class name.</returns>
		public static string GetClassName(CoreState state)
		{
			return StateFactory.FindId (state.GetType ());
		}
	}
}

//	Copyright © 2008, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Types.Collections;
using Epsitec.Common.Widgets;

using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace Epsitec.Cresus.Core
{
	/// <summary>
	/// The <c>StateSerializationContext</c> class manages the mapping between
	/// states and tags, which can be used to serialize or deserialize XML
	/// references between the states.
	/// </summary>
	public sealed class StateSerializationContext
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="StateSerializationContext"/> class.
		/// </summary>
		/// <param name="states">The full collection of states.</param>
		public StateSerializationContext(IEnumerable<States.CoreState> states)
		{
			this.tagToState = new Dictionary<string, States.CoreState> ();
			this.stateToTag = new Dictionary<States.CoreState, string> ();

			int tag = 0;

			foreach (var state in states)
			{
				string id = tag.ToString (System.Globalization.CultureInfo.InvariantCulture);

				this.tagToState.Add (id, state);
				this.stateToTag.Add (state, id);
				
				tag++;
			}
		}

		/// <summary>
		/// Determines whether the state contains the specified tag.
		/// </summary>
		/// <param name="tag">The tag.</param>
		/// <returns>
		/// 	<c>true</c> if the state contains the specified tag; otherwise, <c>false</c>.
		/// </returns>
		public bool ContainsState(string tag)
		{
			return this.tagToState.ContainsKey (tag);
		}

		/// <summary>
		/// Gets the state for the specified tag.
		/// </summary>
		/// <param name="tag">The tag (which must be known).</param>
		/// <returns>The associated state.</returns>
		public States.CoreState GetState(string tag)
		{
			return this.tagToState[tag];
		}

		/// <summary>
		/// Gets the tag for the specified state.
		/// </summary>
		/// <param name="state">The state (which must be known).</param>
		/// <returns>The associated tag</returns>
		public string GetTag(States.CoreState state)
		{
			return this.stateToTag[state];
		}

		
		readonly Dictionary<string, States.CoreState> tagToState;
		readonly Dictionary<States.CoreState, string> stateToTag;
	}
}

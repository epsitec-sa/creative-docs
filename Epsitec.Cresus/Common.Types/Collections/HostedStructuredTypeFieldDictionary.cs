//	Copyright © 2006, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Collections
{
	/// <summary>
	/// The <c>HostedStructuredTypeFieldDictionary</c> stores <see cref="StructuredTypeField"/>
	/// instances.
	/// </summary>
	public class HostedStructuredTypeFieldDictionary : HostedDictionary<string, StructuredTypeField>
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="T:HostedStructuredTypeFieldDictionary"/> class.
		/// </summary>
		/// <param name="host">The host which must be notified.</param>
		public HostedStructuredTypeFieldDictionary(IDictionaryHost<string, StructuredTypeField> host)
			: base (host)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="T:HostedStructuredTypeFieldDictionary"/> class.
		/// </summary>
		/// <param name="insertionCallback">The insertion callback.</param>
		/// <param name="removalCallback">The removal callback.</param>
		public HostedStructuredTypeFieldDictionary(Callback insertionCallback, Callback removalCallback)
			: base (insertionCallback, removalCallback)
		{
		}

		public void Add(string id, INamedType type)
		{
			if (string.IsNullOrEmpty (id))
			{
				throw new System.ArgumentException ("Invalid field id");
			}

			if (this.ContainsKey (id))
			{
				throw new System.ArgumentException ("Duplicate definition for field '{0}'", id);
			}

			this.Add (id, new StructuredTypeField (id, type));
		}
		
		public void Add(string id, INamedType type, Support.Druid captionId)
		{
			if (string.IsNullOrEmpty (id))
			{
				throw new System.ArgumentException ("Invalid field id");
			}

			if (this.ContainsKey (id))
			{
				throw new System.ArgumentException ("Duplicate definition for field '{0}'", id);
			}

			this.Add (id, new StructuredTypeField (id, type, captionId));
		}
	}
}

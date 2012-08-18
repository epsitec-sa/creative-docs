//	Copyright © 2006-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types.Collections
{
	/// <summary>
	/// The <c>HostedStructuredTypeFieldDictionary</c> stores <see cref="StructuredTypeField"/>
	/// instances.
	/// </summary>
	public sealed class HostedStructuredTypeFieldDictionary : HostedDictionary<string, StructuredTypeField>
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

		public void Add(StructuredTypeField field)
		{
			this.Add (field.Id, field);
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
			this.Add (id, type, captionId, -1);
		}
		
		public void Add(string id, INamedType type, Support.Druid captionId, int rank)
		{
			if (string.IsNullOrEmpty (id))
			{
				throw new System.ArgumentException ("Invalid field id");
			}

			if (this.ContainsKey (id))
			{
				throw new System.ArgumentException ("Duplicate definition for field '{0}'", id);
			}

			this.Add (id, new StructuredTypeField (id, type, captionId, rank));
		}

		protected override void NotifyInsertion(string key, StructuredTypeField value)
		{
			if (key != value.Id)
			{
				throw new System.ArgumentException (string.Format ("Inserting a value with Id={0} and key={1}", value.Id, key));
			}

			base.NotifyInsertion (key, value);
		}
	}
}

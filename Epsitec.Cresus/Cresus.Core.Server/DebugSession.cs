using System.Collections;
using System.Collections.Generic;

namespace Epsitec.Cresus.Core.Server
{
	/// <summary>
	/// Ceci remplace la gestion de session en attendant que les session de Nancy soient corrigées.
	/// Ceci est un singleton et si plusieurs utilisateurs se connectent, ils utiliseront la même session!
	/// N'utiliser ceci qu'à des fins de développement/debug!
	/// </summary>
	// TODO Supprimer ce fichier!
	class DebugSession : IEnumerable<KeyValuePair<string, object>>
	{

		private static DebugSession session;
		public static DebugSession Session
		{
			get
			{
				if (session == null)
				{
					session = new DebugSession ();
				}

				return session;
			}
		}


		private readonly IDictionary<string, object> dictionary;
		private bool hasChanged;

		private DebugSession() : this (new Dictionary<string, object> (0))
		{
		}

		private DebugSession(IDictionary<string, object> dictionary)
		{
			this.dictionary = dictionary;
		}

		public int Count
		{
			get
			{
				return dictionary.Count;
			}
		}

		public void DeleteAll()
		{
			if (Count > 0)
			{
				MarkAsChanged ();
			}
			dictionary.Clear ();
		}

		public void Delete(string key)
		{
			if (dictionary.Remove (key))
			{
				MarkAsChanged ();
			}
		}

		public object this[string key]
		{
			get
			{
				return dictionary.ContainsKey (key) ? dictionary[key] : null;
			}
			set
			{
				dictionary[key] = value;
				MarkAsChanged ();
			}
		}

		public bool HasChanged
		{
			get
			{
				return this.hasChanged;
			}
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return this.GetEnumerator ();
		}
		public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
		{
			return dictionary.GetEnumerator ();
		}

		private void MarkAsChanged()
		{
			hasChanged = true;
		}
	}
}

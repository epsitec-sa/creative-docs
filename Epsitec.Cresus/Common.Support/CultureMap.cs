//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>CultureMap</c> class provides the root access to culture specific
	/// data, as it is used in the resource editor. Basically, a <c>CultureMap</c>
	/// instance represents a row in the resource list.
	/// </summary>
	public class CultureMap
	{
		internal CultureMap(IResourceAccessor owner, Druid id)
		{
			this.owner = owner;
			this.id = id;
		}

		public IResourceAccessor Owner
		{
			get
			{
				return this.owner;
			}
		}

		/// <summary>
		/// Gets the id associated with this instance.
		/// </summary>
		/// <value>The id.</value>
		public Druid Id
		{
			get
			{
				return this.id;
			}
		}

		/// <summary>
		/// Gets or sets the name associated with this instance.
		/// </summary>
		/// <value>The name.</value>
		public string Name
		{
			get
			{
				return this.name;
			}
			set
			{
				if (this.name != value)
				{
					string oldName = this.name;
					string newName = value;

					this.name = value;

					//	TODO: notify name change
				}
			}
		}

		/// <summary>
		/// Determines whether the culture for specified two letter ISO language name
		/// is defined.
		/// </summary>
		/// <param name="twoLetterISOLanguageName">The two letter ISO language name.</param>
		/// <returns>
		/// 	<c>true</c> if the culture is defined; otherwise, <c>false</c>.
		/// </returns>
		public bool IsCultureDefined(string twoLetterISOLanguageName)
		{
			if (this.map == null)
			{
				return false;
			}

			for (int i = 0; i < this.map.Length; i++)
			{
				if (this.map[i].Key == twoLetterISOLanguageName)
				{
					return true;
				}
			}

			return false;
		}

		/// <summary>
		/// Gets the enumeration of all defined cultures, even the empty ones.
		/// </summary>
		/// <returns>The enumeration of all defined cultures, using their two
		/// letter ISO language name.</returns>
		public IEnumerable<string> GetDefinedCultures()
		{
			for (int i = 0; i < this.map.Length; i++)
			{
				yield return this.map[i].Key;
			}
		}

		/// <summary>
		/// Gets the data associated with the specified two letter ISO language name.
		/// Missing data will be created on the fly.
		/// </summary>
		/// <param name="twoLetterISOLanguageName">The two letter ISO language name.</param>
		/// <returns>The structured data associated with the culture.</returns>
		public Types.StructuredData GetCultureData(string twoLetterISOLanguageName)
		{
			if ((string.IsNullOrEmpty (twoLetterISOLanguageName)) ||
				(twoLetterISOLanguageName.Length != 2))
			{
				throw new System.ArgumentException ("Invalid two letter ISO language name");
			}

			if (this.map != null)
			{
				for (int i = 0; i < this.map.Length; i++)
				{
					if (this.map[i].Key == twoLetterISOLanguageName)
					{
						return this.map[i].Value;
					}
				}
			}

			return this.owner.LoadCultureData (this, twoLetterISOLanguageName);
		}

		/// <summary>
		/// Returns a string that represents the current object.
		/// </summary>
		/// <returns>
		/// A string that represents the current object.
		/// </returns>
		public override string ToString()
		{
			return this.Name;
		}

		internal void RecordCultureData(string twoLetterISOLanguageName, Types.StructuredData data)
		{
			System.Diagnostics.Debug.Assert (data != null);
			
			if (this.map == null)
			{
				this.map = new KeyValuePair<string, Types.StructuredData>[1];
				this.map[0] = new KeyValuePair<string, Types.StructuredData> (twoLetterISOLanguageName, data);
			}
			else
			{
				int pos = this.map.Length;

				KeyValuePair<string, Types.StructuredData>[] temp = this.map;
				KeyValuePair<string, Types.StructuredData>[] copy = new KeyValuePair<string, Types.StructuredData>[pos+1];

				temp.CopyTo (copy, 0);
				copy[pos] = new KeyValuePair<string, Types.StructuredData> (twoLetterISOLanguageName, data);

				this.map = copy;
			}

			data.ValueChanged += this.HandleDataValueChanged;
		}

		private void HandleDataValueChanged(object sender, Types.DependencyPropertyChangedEventArgs e)
		{
			this.owner.NotifyItemChanged (this);
		}

		private Types.StructuredData CreateData(string twoLetterISOLanguageName)
		{
			return this.owner.DataBroker.CreateData ();
		}

		private readonly IResourceAccessor owner;
		private readonly Druid id;
		private string name;
		private KeyValuePair<string, Types.StructuredData>[] map;
	}
}

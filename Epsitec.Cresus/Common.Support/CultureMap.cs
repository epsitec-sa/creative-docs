//	Copyright © 2007-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support;
using Epsitec.Common.Types;

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
	/// <summary>
	/// The <c>CultureMap</c> class provides the root access to culture specific
	/// data, as it is used in the resource editor. Basically, a <c>CultureMap</c>
	/// instance represents a row in the resource list.
	/// </summary>
	public class CultureMap : INotifyPropertyChanged
	{
		internal CultureMap(IResourceAccessor owner, Druid id, CultureMapSource source)
		{
			this.owner = owner;
			this.id = id;
			this.source = source;
			this.originalSource = source;
		}

		/// <summary>
		/// Gets the owner of this item. This is the resource accessor which
		/// <see cref="IResourceAccessor.Collection"/> contains the item.
		/// </summary>
		/// <value>The owner.</value>
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
				if ((this.map != null) &&
					(this.map.Length == 0))
				{
					this.GetCultureData (Resources.DefaultTwoLetterISOLanguageName);
				}

				return this.name;
			}
			set
			{
				if (this.name != value)
				{
					if (this.isNameFrozen)
					{
						throw new System.InvalidOperationException (string.Format ("Trying to rename frozen item {0}.", this.name));
					}

					string oldName = this.name;
					string newName = value;

					this.name = value;

					this.OnPropertyChanged (null, new DependencyPropertyChangedEventArgs ("Name", oldName, newName));
				}
			}
		}

		/// <summary>
		/// Gets or sets the prefix (this is always empty, unless <c>Prefix</c> is
		/// overridden by a derived class).
		/// </summary>
		/// <value>The prefix.</value>
		public virtual string Prefix
		{
			get
			{
				return "";
			}
			set
			{
				throw new System.NotImplementedException ();
			}
		}

		/// <summary>
		/// Gets the full name (same output as the <see cref="ToString"/> method.
		/// </summary>
		/// <value>The full name.</value>
		public string FullName
		{
			get
			{
				return this.ToString ();
			}
		}

		/// <summary>
		/// Gets a value indicating whether this instance is a new item which
		/// has been created but not yet persisted by the resource accessor.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance is a new item; otherwise, <c>false</c>.
		/// </value>
		internal bool IsNewItem
		{
			get
			{
				return this.isNewItem;
			}
			set
			{
				this.isNewItem = value;
			}
		}

		/// <summary>
		/// Gets a value indicating whether the name is read only.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if the name is read only; otherwise, <c>false</c>.
		/// </value>
		public bool IsNameReadOnly
		{
			get
			{
				return this.isNameFrozen;
			}
		}

		/// <summary>
		/// Gets the module source of this item.
		/// </summary>
		/// <value>The module source.</value>
		public CultureMapSource Source
		{
			get
			{
				return this.source;
			}
			internal set
			{
				if (value != this.source)
				{
					System.Diagnostics.Debug.Assert (value == CultureMapSource.DynamicMerge);

					CultureMapSource oldSource = this.source;
					CultureMapSource newSource = value;

					this.source = value;

					this.OnPropertyChanged (null, new DependencyPropertyChangedEventArgs ("Source", oldSource, newSource));
				}
			}
		}

		/// <summary>
		/// Gets or sets a value indicating whether this instance is is no longer
		/// up-to-date, which means that a refresh is needed.
		/// </summary>
		/// <value>
		/// 	<c>true</c> if this instance needs refreshing; otherwise, <c>false</c>.
		/// </value>
		internal bool IsRefreshNeeded
		{
			get
			{
				return this.isRefreshNeeded;
			}
			set
			{
				this.isRefreshNeeded = value;
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
				if ((this.map[i].Key == twoLetterISOLanguageName) &&
					(this.map[i].Value != null))
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
		/// letter ISO language name; the default culture will be returned as
		/// the last one.</returns>
		public IEnumerable<string> GetDefinedCultures()
		{
			if (this.map != null)
			{
				bool hasDefault = false;

				for (int i = 0; i < this.map.Length; i++)
				{
					if (this.map[i].Value != null)
					{
						if (this.map[i].Key == Resources.DefaultTwoLetterISOLanguageName)
						{
							hasDefault = true;
						}
						else
						{
							yield return this.map[i].Key;
						}
					}
				}

				//	Make sure we return the default culture last.

				if (hasDefault)
				{
					yield return Resources.DefaultTwoLetterISOLanguageName;
				}
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
					if ((this.map[i].Key == twoLetterISOLanguageName) &&
						(this.map[i].Value != null))
					{
						return this.map[i].Value;
					}
				}
			}

			return this.owner.LoadCultureData (this, twoLetterISOLanguageName);
		}

		/// <summary>
		/// Freezes the name and makes it read only.
		/// </summary>
		public void FreezeName()
		{
			this.isNameFrozen = true;
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

		#region INotifyPropertyChanged Members

		public event EventHandler<DependencyPropertyChangedEventArgs> PropertyChanged;

		#endregion

		public void NotifyDataAdded(StructuredData data)
		{
			data.ValueChanged += this.HandleDataValueChanged;
		}
		
		public void NotifyDataRemoved(StructuredData data)
		{
			data.ValueChanged -= this.HandleDataValueChanged;
		}

		protected virtual void OnPropertyChanged(StructuredData sender, DependencyPropertyChangedEventArgs e)
		{
			if (this.owner != null)
			{
				this.owner.NotifyItemChanged (this, sender, e);

				if (this.PropertyChanged != null)
				{
					this.PropertyChanged (this, e);
				}
			}
		}

		/// <summary>
		/// Records the data associated with the specified culture.
		/// </summary>
		/// <param name="twoLetterISOLanguageName">The two letter ISO language name.</param>
		/// <param name="data">The data to record.</param>
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
				this.CheckForDuplicates (twoLetterISOLanguageName);

				bool insert = true;

				//	First, try to re-use an empty slot in the map, if there is
				//	one (the value is null, in that case) :
				
				for (int i = 0; i < this.map.Length; i++)
				{
					if (this.map[i].Key == twoLetterISOLanguageName)
					{
						this.map[i] = new KeyValuePair<string, StructuredData> (twoLetterISOLanguageName, data);
						insert = false;
					}
				}

				//	If no empty slot was found, the map must be resized to fit
				//	the needs of an extra culture :

				if (insert)
				{
					int pos = this.map.Length;

					KeyValuePair<string, Types.StructuredData>[] temp = this.map;
					KeyValuePair<string, Types.StructuredData>[] copy = new KeyValuePair<string, Types.StructuredData>[pos+1];

					temp.CopyTo (copy, 0);
					copy[pos] = new KeyValuePair<string, Types.StructuredData> (twoLetterISOLanguageName, data);

					this.map = copy;
				}
			}

			data.ValueChanged += this.HandleDataValueChanged;
		}

		/// <summary>
		/// Clears a specific culture data.
		/// </summary>
		/// <param name="twoLetterISOLanguageName">The two letter ISO language name.</param>
		public void ClearCultureData(string twoLetterISOLanguageName)
		{
			if (this.map != null)
			{
				for (int i = 0; i < this.map.Length; i++)
				{
					if (this.map[i].Key == twoLetterISOLanguageName)
					{
						this.map[i] = new KeyValuePair<string, StructuredData> (twoLetterISOLanguageName, null);
						break;
					}
				}
			}
		}

		/// <summary>
		/// Clears all culture data associated with this item.
		/// </summary>
		internal void ClearCultureData()
		{
			if (this.map != null)
			{
				for (int i = 0; i < this.map.Length; i++)
				{
					this.owner.NotifyCultureDataCleared (this, this.map[i].Key, this.map[i].Value);
				}
				
				this.map = new KeyValuePair<string,StructuredData>[0];
			}
			
			this.source = this.originalSource;
		}

		/// <summary>
		/// Checks for duplicates in the data map. If the caller tries to redefine
		/// an already known set of data, this will throw an exception in debug
		/// builds.
		/// </summary>
		/// <param name="twoLetterISOLanguageName">The two letter ISO language name.</param>
		[System.Diagnostics.Conditional ("DEBUG")]
		private void CheckForDuplicates(string twoLetterISOLanguageName)
		{
			if (this.map != null)
			{
				for (int i = 0; i < this.map.Length; i++)
				{
					if ((this.map[i].Key == twoLetterISOLanguageName) &&
						(this.map[i].Value != null))
					{
						throw new System.InvalidOperationException ("Duplicate insertion");
					}
				}
			}
		}

		private void HandleDataValueChanged(object sender, Types.DependencyPropertyChangedEventArgs e)
		{
			this.OnPropertyChanged (sender as StructuredData, e);
		}

		private readonly IResourceAccessor owner;
		private readonly Druid id;
		private string name;
		private KeyValuePair<string, Types.StructuredData>[] map;
		private bool isNewItem;
		private bool isRefreshNeeded;
		private bool isNameFrozen;
		private CultureMapSource source;
		private CultureMapSource originalSource;
	}
}

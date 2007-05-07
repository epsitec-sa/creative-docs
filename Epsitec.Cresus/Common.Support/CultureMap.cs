//	Copyright © 2007, EPSITEC SA, CH-1092 BELMONT, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Support
{
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

		public Druid Id
		{
			get
			{
				return this.id;
			}
		}

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
					this.name = value;
				}
			}
		}

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

		public Types.StructuredData GetCultureData(string twoLetterISOLanguageName)
		{
			if (this.map == null)
			{
				Types.StructuredData data = this.CreateData (twoLetterISOLanguageName);

				if (data != null)
				{
					this.map = new KeyValuePair<string, Types.StructuredData>[1];
					this.map[0] = new KeyValuePair<string, Types.StructuredData> (twoLetterISOLanguageName, data);
				}

				return data;
			}
			else
			{
				for (int i = 0; i < this.map.Length; i++)
				{
					if (this.map[i].Key == twoLetterISOLanguageName)
					{
						return this.map[i].Value;
					}
				}

				Types.StructuredData data = this.CreateData (twoLetterISOLanguageName);

				if (data != null)
				{
					int pos = this.map.Length;

					KeyValuePair<string, Types.StructuredData>[] temp = this.map;
					KeyValuePair<string, Types.StructuredData>[] copy = new KeyValuePair<string, Types.StructuredData>[pos+1];

					temp.CopyTo (copy, 0);
					copy[pos] = new KeyValuePair<string, Types.StructuredData> (twoLetterISOLanguageName, data);

					this.map = copy;
				}

				return data;
			}
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

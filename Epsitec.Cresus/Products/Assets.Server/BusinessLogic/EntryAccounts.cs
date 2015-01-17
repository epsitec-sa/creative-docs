//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public class EntryAccounts
	{
		public EntryAccounts()
		{
			this.accounts = new Dictionary<ObjectField, string> ();
		}


		public string this[ObjectField field]
		{
			get
			{
				string s;
				if (this.accounts.TryGetValue (field, out s))
				{
					return s;
				}
				else
				{
					return null;
				}
			}
			set
			{
				this.accounts[field] = value;
			}
		}


		public bool IsEmpty
		{
			get
			{
				return !this.accounts.Any ();
			}
		}


		private readonly Dictionary<ObjectField, string> accounts;
	}
}

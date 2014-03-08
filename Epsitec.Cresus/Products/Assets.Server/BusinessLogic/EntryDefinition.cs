//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.BusinessLogic
{
	public struct EntryDefinition
	{
		public EntryDefinition(Guid obj, System.DateTime date, Guid account1, Guid account2, Guid account3, Guid account4, Guid account5, Guid account6, Guid account7, Guid account8)
		{
			this.Object   = obj;
			this.Date     = date;
			this.Account1 = account1;
			this.Account2 = account2;
			this.Account3 = account3;
			this.Account4 = account4;
			this.Account5 = account5;
			this.Account6 = account6;
			this.Account7 = account7;
			this.Account8 = account8;
		}


		public bool IsEmpty
		{
			get
			{
				return this.Object.IsEmpty
					&& this.Account1.IsEmpty
					&& this.Account2.IsEmpty
					&& this.Account3.IsEmpty
					&& this.Account4.IsEmpty
					&& this.Account5.IsEmpty
					&& this.Account6.IsEmpty
					&& this.Account7.IsEmpty
					&& this.Account8.IsEmpty;
			}
		}


		public static EntryDefinition Empty = new EntryDefinition (Guid.Empty, System.DateTime.MaxValue, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty, Guid.Empty);

		public readonly Guid					Object;
		public readonly System.DateTime			Date;
		public readonly Guid					Account1;
		public readonly Guid					Account2;
		public readonly Guid					Account3;
		public readonly Guid					Account4;
		public readonly Guid					Account5;
		public readonly Guid					Account6;
		public readonly Guid					Account7;
		public readonly Guid					Account8;
	}
}

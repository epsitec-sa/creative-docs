//	Copyright © 2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Assets.Data;
using Epsitec.Cresus.Assets.Server.BusinessLogic;
using Epsitec.Cresus.Assets.Server.SimpleEngine;

namespace Epsitec.Cresus.Assets.Server.NodeGetters
{
	/// <summary>
	/// Ne retourne que les comptes qui correspondent à un filtre textuel.
	/// Les comptes centralisateurs et les comptes de TVA sont exclus d'office.
	/// </summary>
	public class AccountsFilterNodeGetter : INodeGetter<GuidNode>  // outputNodes
	{
		public AccountsFilterNodeGetter(DataAccessor accessor, INodeGetter<GuidNode> accountsNodes)
		{
			this.accessor      = accessor;
			this.accountsNodes = accountsNodes;

			this.outputNodes = new List<GuidNode> ();
		}


		public void SetParams(BaseType baseType, Timestamp? timestamp, string filter)
		{
			this.baseType  = baseType;
			this.timestamp = timestamp;
			this.filter    = filter;

			if (string.IsNullOrEmpty (this.filter))
			{
				this.preprocessFilter = null;
			}
			else
			{
				this.preprocessFilter = this.filter.ToLowerInvariant ();
			}

			this.UpdateData ();
		}


		public int Count
		{
			get
			{
				return this.outputNodes.Count;
			}
		}

		public GuidNode this[int index]
		{
			get
			{
				if (index >= 0 && index < this.outputNodes.Count)
				{
					return this.outputNodes[index];
				}
				else
				{
					return GuidNode.Empty;
				}
			}
		}

		public int SearchIndex(Guid guid)
		{
			return this.outputNodes.FindIndex (x => x.Guid == guid);
		}


		private void UpdateData()
		{
			this.outputNodes.Clear ();

			foreach (var accountNode in this.accountsNodes.GetNodes ())
			{
				var obj = this.accessor.GetObject (this.baseType, accountNode.Guid);

				var accType = (AccountType) ObjectProperties.GetObjectPropertyInt (obj, this.timestamp, ObjectField.AccountType);

				if (accType == AccountType.Normal)
				{
					if (string.IsNullOrEmpty (this.preprocessFilter))
					{
						this.outputNodes.Add (accountNode);
					}
					else
					{
						var number = ObjectProperties.GetObjectPropertyString (obj, this.timestamp, ObjectField.Number, inputValue: true);
						var name   = ObjectProperties.GetObjectPropertyString (obj, this.timestamp, ObjectField.Name);

						if (this.IsMatch (number) ||
							this.IsMatch (name))
						{
							this.outputNodes.Add (accountNode);
						}
					}
				}
			}
		}

		private bool IsMatch(string text)
		{
			if (!string.IsNullOrEmpty (text))
			{
				if (text.ToLowerInvariant ().Contains (this.preprocessFilter))
				{
					return true;
				}
			}

			return false;
		}


		private readonly DataAccessor			accessor;
		private readonly INodeGetter<GuidNode>	accountsNodes;
		private readonly List<GuidNode>			outputNodes;

		private BaseType						baseType;
		private Timestamp?						timestamp;
		private string							filter;
		private string							preprocessFilter;
	}
}

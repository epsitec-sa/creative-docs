//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Epsitec.Common.Support;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.EntitiesAccessors
{
	public class BidirectionnalConverter
	{
		public BidirectionnalConverter()
		{
			// TODO: Remplacer le dictionnaire par utre chose, qui permette 2 keys identiques
			this.content = new Dictionary<string, string> ();
		}

		public void Add(string key, string value)
		{
			this.content.Add (key, value);
		}


		public string KeyToValue(string key)
		{
			key = Misc.RemoveAccentsToLower (key);

			foreach (var pair in this.content)
			{
				if (key == Misc.RemoveAccentsToLower (pair.Key))
				{
					return pair.Value;
				}
			}

			return null;
		}

		public string ValueToKey(string value)
		{
			value = Misc.RemoveAccentsToLower (value);

			foreach (var pair in this.content)
			{
				if (value == Misc.RemoveAccentsToLower (pair.Value))
				{
					return pair.Key;
				}
			}

			return null;
		}


		public void InitializeComboWithKeys(Widgets.SuperCombo combo)
		{
			foreach (var pair in this.content)
			{
				combo.Items.Add (pair.Key);
			}
		}

		public void InitializeComboWithValues(Widgets.SuperCombo combo)
		{
			foreach (var pair in this.content)
			{
				combo.Items.Add (pair.Value);
			}
		}


		private Dictionary<string, string> content;
	}
}

//	Copyright © 2004-2008, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Responsable: Pierre ARNAUD

using System.Collections.Generic;

namespace Epsitec.Common.Types
{
	/// <summary>
	/// La class StringDict implémente un dictionnaire de clefs/valeurs de
	/// type string.
	/// </summary>
	public class StringDict : Dictionary<string, string>, IStringDict
	{
		public StringDict()
		{
		}

		public StringDict(Types.IStringDict model)
			: this ()
		{
			StringDict.Copy (model, this);
		}

		#region IStringDict Members

		public new string[] Keys
		{
			get
			{
				List<string> keys = new List<string> ();
				keys.AddRange (base.Keys);
				return keys.ToArray ();
			}
		}

		#endregion
		
		public static void Copy(Types.IStringDict model, Types.IStringDict target)
		{
			string[] keys = model.Keys;

			for (int i = 0; i < keys.Length; i++)
			{
				target.Add (keys[i], model[keys[i]]);
			}
		}
	}
}

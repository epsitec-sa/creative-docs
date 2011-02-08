//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class DocumentOptionsEntity
	{
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Name);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name);
		}

		public override EntityStatus GetEntityStatus()
		{
			using (var a = new EntityStatusAccumulator ())
			{
				a.Accumulate (this.Name.GetEntityStatus ());
				a.Accumulate (this.Description.GetEntityStatus ().TreatAsOptional ());

				return a.EntityStatus;
			}
		}


		public Dictionary<string, string> GetOptions()
		{
			// TODO: Ajouter un cache pour accélérer l'accès !
			var dict = new Dictionary<string, string> ();

			if (this.SerializedData != null)
			{
				string s = System.Text.Encoding.UTF8.GetString (this.SerializedData);

				if (!string.IsNullOrEmpty (s))
				{
					// Exemple de table obtenue: "HeaderLogo", "true", "LayoutFrameless", "false", ""
					string[] split = s.Split ('◊');

					for (int i = 0; i < split.Length-1; i+=2)
					{
						dict.Add (split[i], split[i+1]);
					}
				}
			}

			return dict;
		}

		public void SetOptions(Dictionary<string, string> options)
		{
			if (options.Count == 0)
			{
				this.SerializedData = null;
			}
			else
			{
				var builder = new System.Text.StringBuilder ();

				foreach (var pair in options)
				{
					builder.Append (pair.Key);
					builder.Append ("◊");
					builder.Append (pair.Value);
					builder.Append ("◊");
				}

				// Exemple de chaîne obtenue: "HeaderLogo◊true◊LayoutFrameless◊false◊"
				byte[] bytes = System.Text.Encoding.UTF8.GetBytes (builder.ToString ());
				this.SerializedData = bytes;
			}
		}
	}
}

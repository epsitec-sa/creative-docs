//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Helpers;
using Epsitec.Cresus.Core.Print;
using Epsitec.Cresus.Core.Print.Verbose;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class DocumentOptionsEntity
	{
		public override FormattedText GetSummary()
		{
			//	L'espace entre les <br/> est nécessaire, à cause de FormatText qui fait du zèle !
			return TextFormatter.FormatText (this.Name, FormattedText.Concat ("<br/>________________________________________<br/> <br/>", this.GetOptionsSummary ()));
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


		private FormattedText GetOptionsSummary()
		{
			var dict = this.GetOptions ();
			var all = VerboseDocumentOption.GetAll ();
			var builder = new System.Text.StringBuilder ();

			foreach (var option in all)
			{
				if (option.Option != DocumentOption.None && dict.ContainsOption (option.Option))
				{
					var description = option.Description;
					var value = dict.GetValue (option.Option);

					if (option.Type == DocumentOptionValueType.Boolean)
					{
						switch (value)
						{
							case "false":
								value = "non";
								break;

							case "true":
								value = "oui";
								break;
						}
					}

					if (option.Type == DocumentOptionValueType.Enumeration)
					{
						description = all.Where (x => x.IsTitle && x.Group == option.Group).Select (x => x.Title).FirstOrDefault ();

						for (int i = 0; i < option.Enumeration.Count (); i++)
						{
							if (option.Enumeration.ElementAt (i) == value)
							{
								value = option.EnumerationDescription.ElementAt (i);
								break;
							}
						}
					}

					if (option.Type == DocumentOptionValueType.Distance)
					{
						value = string.Concat (value, " mm");
					}

					if (string.IsNullOrEmpty (description))
					{
						description = Print.Common.DocumentOptionToString (option.Option);
					}

					builder.Append (description);
					builder.Append (" = ");
					builder.Append (value);
					builder.Append ("<br/>");
				}
			}

			return builder.ToString ();
		}

		public OptionsDictionary GetOptions()
		{
			//	Retourne le dictionnaire "option d'impression" / "valeur".
			// TODO: Ajouter un cache pour accélérer l'accès !
			var dict = new OptionsDictionary ();

			if (this.SerializedData != null)
			{
				string s = System.Text.Encoding.UTF8.GetString (this.SerializedData);

				if (!string.IsNullOrEmpty (s))
				{
					// Exemple de table obtenue: "HeaderLogo", "true", "LayoutFrameless", "false", ""
					string[] split = s.Split ('◊');

					for (int i = 0; i < split.Length-1; i+=2)
					{
						var option = Print.Common.StringToDocumentOption (split[i]);
						dict.Add (option, split[i+1]);
					}
				}
			}

			return dict;
		}

		public void SetOptions(OptionsDictionary options)
		{
			//	Spécifie le dictionnaire "option d'impression" / "valeur".
			if (options.Count == 0)
			{
				this.SerializedData = null;
			}
			else
			{
				var builder = new System.Text.StringBuilder ();

				foreach (var pair in options.ContentPair)
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

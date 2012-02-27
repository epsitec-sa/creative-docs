using Epsitec.Common.Types;

using Nancy;

using System.Collections.Generic;

using System.Text.RegularExpressions;


namespace Epsitec.Cresus.WebCore.Server
{


	/// <summary>
	/// This class provides a way to simulate collection fields in an html form. It gives us a way
	/// to name the fields that are part of a collection and then to combine them into a single
	/// field once the form is submitted.
	/// </summary>
	internal static class FormCollectionEmbedder
	{


		public static DynamicDictionary DecodeFormWithCollections(dynamic form)
		{
			var newDictionnary = new DynamicDictionary ();

			foreach (string key in form.GetDynamicMemberNames ())
			{
				string value = form[key];

				var match = FormCollectionEmbedder.regex.Match (key);

				if (match.Success)
				{
					var collectionName = match.Groups[1].Value;

					List<string> list = newDictionnary[collectionName].Value;

					if (list == null)
					{
						list = new List<string> ();
						newDictionnary[collectionName] = list;
					}

					var index = InvariantConverter.ParseInt (match.Groups[2].Value);

					// NOTE if the elements of the collection are not in the order of their index,
					// we must pre allocate items in the collection, otherwise there will be an
					// exception thrown when we'll add an element whose index is greater than the
					// number of elements in the list.

					while (list.Count < index)
					{
						list.Add (null);
					}

					list.Add (value);
				}
				else
				{
					newDictionnary[key] = value;
				}
			}

			return newDictionnary;
		}


		public static string GetFieldName(string fieldName, int index)
		{
			return fieldName + "[" + InvariantConverter.ToString (index) + "]";
		}


		// Matches patterns like xyz[123] where xyz and 123 are captured.
		private static readonly Regex regex = new Regex (@"^([^\[]+)\[(\d+)\]$");



	}


}

//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Jonas Schmid, Maintainer: -

using System.Collections.Generic;
using System.Text.RegularExpressions;
using Nancy;

namespace Epsitec.Cresus.Core.Server.NancyComponents
{
	/// <summary>
	/// This is used when we get a form with arrays, like a collection type in an entity
	/// </summary>
	public static class PostArrayHandler
	{
		public static object GetFormWithArrays(dynamic form)
		{
			var newDictionnary = new DynamicDictionary ();

			var memberNames = (IEnumerable<string>) form.GetDynamicMemberNames ();

			foreach (var key in memberNames)
			{
				string value = form[key];

				var match = PostArrayHandler.reg.Match (key);
				if (match.Success)
				{
					string arrayName = match.Groups[1].Value;

					List<string> list = newDictionnary[arrayName].Value;
					if (list == null)
					{
						list = new List<string> ();
						newDictionnary[arrayName] = list;
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

		private static readonly Regex reg = new Regex (@"^([^\[]+)\[[^\]]?\]$");
	}
}

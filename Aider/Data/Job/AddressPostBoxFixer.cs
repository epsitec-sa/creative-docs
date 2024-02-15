//	Copyright © 2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Data.ECh;
using Epsitec.Aider.Entities;
using Epsitec.Aider.Rules;
using Epsitec.Common.IO;

using Epsitec.Common.Support;
using Epsitec.Common.Support.Extensions;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Loader;

using Epsitec.Data.Platform;

using System;

using System.Collections.Generic;

using System.Diagnostics;

using System.Linq;


namespace Epsitec.Aider.Data.Job
{
	/// <summary>
	/// 
	/// </summary>
	internal static class AddressPostBoxFixer
	{
		public static void FixPostBoxes(CoreData coreData)
		{
			using (var businessContext = new BusinessContext (coreData, false))
			{
                var addresses = AddressPostBoxFixer.GetAddresses (businessContext);
                var updates = new List<AiderAddressEntity> ();

                foreach (var address in addresses)
                {
                    var postBox = address.PostBox;
                    var formatted = AiderAddressBusinessRules.ParseAndFormatPostBox (address, postBox);

                    if (formatted == postBox)
                    {
                        continue;
                    }

                    if (formatted == null)
                    {
                        var split  = postBox.SplitAfter (x => !char.IsDigit (x));
                        var text   = split.Item1.TrimEnd ();
                        var number = split.Item2;

                        if ((string.IsNullOrEmpty (address.Street)) &&
                            (string.IsNullOrEmpty (text) == false) &&
                            (string.IsNullOrEmpty (number) == false))
                        {
                            //  Information should be stored into the street field:

                            address.Street = text;
                            address.HouseNumberAndComplement = number;
                        }
                        else
                        {
                            //  Information should be stored into the additional address line field:

                            address.AddressLine1 = postBox;
                            address.PostBox = "";
                        }

                        System.Console.WriteLine ("{0} => {1}", postBox, address.GetCompactSummary ().ToSimpleText ());

                        address.PostBox = "";
                        updates.Add (address);
                    }
                    else
                    {
                        //  Information should be updated and stored into the post box field:

                        System.Console.WriteLine ("{0} => {1}", postBox, formatted);

                        address.PostBox = formatted;
                        updates.Add (address);
                    }
                }

				businessContext.SaveChanges
				(
					LockingPolicy.ReleaseLock, EntitySaveMode.IgnoreValidationErrors
				);

                System.IO.File.WriteAllLines ("post-box-fixes.txt", updates.Select (x => x.GetPostalAddress ().ToSimpleText ().Replace ('\n', '|')));
                System.IO.File.WriteAllLines ("post-box-addresses.txt", addresses.Select (x => x.GetPostalAddress ().ToSimpleText ().Replace ('\n', '|')));

                System.Console.WriteLine ("{0} updates", updates.Count);
                System.Console.ReadLine ();
			}
		}
		private static IList<AiderAddressEntity> GetAddresses(BusinessContext businessContext)
		{
			var addressExemple = new AiderAddressEntity ()
			{
			};

			var request = new Request ()
			{
				RootEntity = addressExemple
            };

			request.AddCondition
			(
				businessContext.DataContext,
                addressExemple,
				p => SqlMethods.Like (p.PostBox, "_%")
			);

			return businessContext.DataContext.GetByRequest<AiderAddressEntity> (request);
		}
	}
}

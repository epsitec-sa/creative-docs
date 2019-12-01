//	Copyright © 2019, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Aider.Entities;

using Epsitec.Common.Types;
using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Loader;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Aider.Data.Job
{
	/// <summary>
	/// Export contact summaries where comments are not empty.
	/// </summary>
	internal static class ExportCommentJob
	{
		public static void ExportComments(CoreData coreData)
		{
            using (var businessContext = new BusinessContext (coreData, false))
            {
                System.Console.WriteLine ("Extract persons with comments");
                var list = ExportCommentJob.GetContactsWithPersonComment (businessContext);

                System.IO.File.WriteAllLines ("export-person-comments.txt",
                    list.Select (c => (c.DisplayAddress + "\t" + c.Person.DisplayName + "\t" + c.Person.Comment.Text.ToSimpleText ().Replace ('\n', '|'))));

                System.Console.WriteLine ("{0} comments found", list.Count);
            }

            using (var businessContext = new BusinessContext (coreData, false))
            {
                System.Console.WriteLine ("Extract households with comments");
                var list = ExportCommentJob.GetContactsWithHouseholdComment (businessContext);

                System.IO.File.WriteAllLines ("export-household-comments.txt",
                    list.Select (c => (c.DisplayAddress + "\t" + c.Household.DisplayName + "\t" + c.Household.Comment.Text.ToSimpleText ().Replace ('\n', '|'))));

                System.Console.WriteLine ("{0} comments found", list.Count);
            }

            using (var businessContext = new BusinessContext (coreData, false))
            {
                System.Console.WriteLine ("Extract addresses with comments");
                var list = ExportCommentJob.GetContactsWithAddressComment (businessContext);

                System.IO.File.WriteAllLines ("export-address-comments.txt",
                    list.Select (c => (c.DisplayAddress + "\t" + c.DisplayName + "\t" + c.Address.Comment.Text.ToSimpleText ().Replace ('\n', '|'))));

                System.Console.WriteLine ("{0} comments found", list.Count);
            }


            System.Console.ReadLine ();
        }
        private static IList<AiderContactEntity> GetContactsWithPersonComment(BusinessContext businessContext)
        {
            var example = new AiderContactEntity ()
            {
                Person = new AiderPersonEntity ()
                {
                    Comment = new AiderCommentEntity ()
                }
            };

            var request = new Request ()
            {
                RootEntity = example
            };

            request.AddCondition
            (
                businessContext.DataContext,
                example.Person,
                person => SqlMethods.Like (SqlMethods.Convert<FormattedText, string> (person.Comment.Text), "_%")
            );

            return businessContext.DataContext.GetByRequest<AiderContactEntity> (request);
        }

        private static IList<AiderContactEntity> GetContactsWithHouseholdComment(BusinessContext businessContext)
        {
            var example = new AiderContactEntity ()
            {
                Household = new AiderHouseholdEntity ()
                {
                    Comment = new AiderCommentEntity ()
                }
            };

            var request = new Request ()
            {
                RootEntity = example
            };

            request.AddCondition
            (
                businessContext.DataContext,
                example.Household,
                household => SqlMethods.Like (SqlMethods.Convert<FormattedText, string> (household.Comment.Text), "_%")
            );

            return businessContext.DataContext.GetByRequest<AiderContactEntity> (request);
        }

        private static IList<AiderContactEntity> GetContactsWithAddressComment(BusinessContext businessContext)
        {
            var example = new AiderContactEntity ()
            {
                Address = new AiderAddressEntity ()
                {
                    Comment = new AiderCommentEntity ()
                }
            };

            var request = new Request ()
            {
                RootEntity = example
            };

            request.AddCondition
            (
                businessContext.DataContext,
                example.Address,
                address => SqlMethods.Like (SqlMethods.Convert<FormattedText, string> (address.Comment.Text), "_%")
            );

            return businessContext.DataContext.GetByRequest<AiderContactEntity> (request);
        }
    }
}

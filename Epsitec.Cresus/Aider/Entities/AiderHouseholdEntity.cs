using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;

using System.Collections.Generic;

using System.Linq;


namespace Epsitec.Aider.Entities
{


	public partial class AiderHouseholdEntity
	{


		public override FormattedText GetSummary()
		{
			var headNames = new List<string> ();

			var headName1 = this.Head1.eCH_Person.PersonOfficialName;
			var headName2 = this.Head2.eCH_Person.PersonOfficialName;

			if (headName1 != null)
			{
				headNames.Add (headName1);
			}

			if (headName2 != null)
			{
				headNames.Add (headName2);
			}

			var text = "Famille " + headNames.Distinct ().Join (" ");

			return TextFormatter.FormatText (text);
		}


		public override FormattedText GetCompactSummary()
		{
			return this.GetSummary ();
		}


		public FormattedText GetMembersTitle()
		{
			var nbMembers = this.Members.Count;

			return TextFormatter.FormatText ("Membres (", nbMembers, ")");
		}


		public FormattedText GetMembersSummary()
		{
			var members = this.Members
				   .Select (m => m.GetCompactSummary ())
				   .CreateSummarySequence (10, "...");

			return FormattedText.Join (FormattedText.FromSimpleText("\n"), members);
		}


		partial void GetMembers(ref IList<AiderPersonEntity> value)
		{
			value = this.GetMembers ().AsReadOnlyCollection ();
		}


		private HashSet<AiderPersonEntity> GetMembers()
		{
			if (this.members == null)
			{
				this.members = new HashSet<AiderPersonEntity> ();

				var businessContext = BusinessContextPool.GetCurrentContext (this);
				var dataContext = businessContext.DataContext;

				if (dataContext.IsPersistent (this))
				{
					this.members.UnionWith (dataContext.GetByExample (new AiderPersonEntity ()
					{
						Household1 = this,
					}));

					this.members.UnionWith (dataContext.GetByExample (new AiderPersonEntity ()
					{
						Household2 = this,
					}));
				}
			}

			return this.members;
		}


		public void Add(AiderPersonEntity newMember)
		{
			this.GetMembers ().Add (newMember);

			if (this.Head1.IsNull ())
			{
				this.Head1 = newMember;
			}
		}


		public void Remove(AiderPersonEntity oldMember)
		{
			this.GetMembers ().Remove (oldMember);

			if (this.Head1 == oldMember)
			{
				this.Head1 = null;
			}
			else if (this.Head2 == oldMember)
			{
				this.Head2 = null;
			}
		}


		public IEnumerable<AiderPersonEntity> GetHeads()
		{
			var head1 = this.Head1;

			if (head1.IsNotNull ())
			{
				yield return head1;
			}

			var head2 = this.Head2;

			if (head2.IsNotNull ())
			{
				yield return head2;
			}
		}


		public string GetDefaultLastname()
		{
			if (this.Head1.IsNotNull ())
			{
				return this.Head1.eCH_Person.PersonOfficialName;
			}
			else if (this.Head2.IsNotNull ())
			{
				return this.Head2.eCH_Person.PersonOfficialName;
			}
			else
			{
				return "";
			}
		}



		// This property is only meant as an in memory cache of the members of the household. It
		// will never be saved to the database.
		private HashSet<AiderPersonEntity> members;


	}


}

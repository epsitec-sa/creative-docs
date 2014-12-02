//	Copyright © 2014, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Support.Extensions;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer.Context;

using System.Linq;
using System.Collections.Generic;
using Epsitec.Aider.Data.Common;
using Epsitec.Aider.Data.Groups;
using Epsitec.Aider.Enumerations;

namespace Epsitec.Aider.Entities
{
	public partial class AiderOfficeManagementEntity
	{
		public AiderOfficeManagementEntity()
		{
		}

		
		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.OfficeMainContact.GetAddressLabelText ());
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.OfficeName);
		}


		public FormattedText GetDocumentTitleSummary()
		{
			return TextFormatter.FormatText ("Documents");
		}

		public FormattedText GetDocumentsSummary()
		{
			switch (this.Documents.Count)
			{
				case 0:
					return TextFormatter.FormatText ("Aucun");
				case 1:
					return TextFormatter.FormatText ("Un document");

				default:
					return TextFormatter.FormatText (this.Documents.Count, "documents");
			}
		}

		public FormattedText GetSettingsTitleSummary()
		{
			return TextFormatter.FormatText ("Expéditeurs");
		}

		public FormattedText GetSettingsSummary()
		{
			switch (this.OfficeSenders.Count)
			{
				case 0:
					return TextFormatter.FormatText ("Aucun");
				case 1:
					return TextFormatter.FormatText ("Un expéditeur");
				default:
					return TextFormatter.FormatText (this.OfficeSenders.Count, "expéditeurs");
			}
		}

		public static void JoinOfficeUsers(BusinessContext businessContext, AiderOfficeManagementEntity office, AiderUserEntity user)
		{
			AiderEmployeeJobEntity.CreateOfficeUser (businessContext, user.Contact.Person.Employee, office, "");
		}

		public static void LeaveOfficeUsers(BusinessContext businessContext, AiderOfficeManagementEntity office, AiderUserEntity user)
		{
			office.DeleteOfficeUserJobsForUser (businessContext, user);
		}
		
		public static void JoinOfficeManagement(BusinessContext businessContext, AiderOfficeManagementEntity office, AiderUserEntity user)
		{
			var currentOffice = user.Office;
			var currentSender = user.OfficeSender;
			var contact		  = user.Contact;

			if (currentOffice.IsNotNull () && currentOffice != office)
			{
				AiderOfficeManagementEntity.LeaveOfficeManagement (businessContext, currentOffice, user);

				//try to remap old sender settings
				var oldSender = AiderOfficeSenderEntity.Find (businessContext, contact);
				if (oldSender.IsNotNull ())
				{
					oldSender.Office = office;
					user.OfficeSender = oldSender;
					office.AddSenderInternal (oldSender);
				}
				else
				{
					//Create sender
					user.OfficeSender = AiderOfficeSenderEntity.Create (businessContext, office, user.Contact);
				}
			}
			else
			{
				//Create sender
				user.OfficeSender = AiderOfficeSenderEntity.Create (businessContext, office, user.Contact);
			}

			AiderEmployeeJobEntity.CreateOfficeManager (businessContext, user.Contact.Person.Employee, office, "");

			//Join office
			user.Office = office;
		}

		public static void LeaveOfficeManagement(BusinessContext businessContext, AiderOfficeManagementEntity office, AiderUserEntity user)
		{
			var currentOffice = user.Office;
			var currentSender = user.OfficeSender;
			var contact		  = user.Contact;

			
			if(currentSender.IsNotNull ())
			{
				office.RemoveSenderInternal (currentSender);
				AiderOfficeSenderEntity.Delete (businessContext, currentSender);
			}

			office.DeleteOfficeManagerJobsForUser (businessContext, user);
			
			//Leave parish
			user.Office = null;
		}

		public static AiderOfficeManagementEntity Find(BusinessContext businessContext, AiderGroupEntity group)
		{
			var officeExample = new AiderOfficeManagementEntity
			{
				ParishGroup = group
			};

			return businessContext.DataContext.GetByExample (officeExample).First ();
		}

		public static AiderOfficeManagementEntity Create(BusinessContext businessContext,string name,AiderGroupEntity managementGroup)
		{
			var office = businessContext.CreateAndRegisterEntity<AiderOfficeManagementEntity> ();

			office.OfficeName = name;
			office.ParishGroup = managementGroup;
			office.ParishGroupPathCache = managementGroup.Path;
			office.RefreshOfficeShortName ();
			office.Region = managementGroup.GetRootRegionCode ();
			AiderUsersGroups.CreateForGroup (businessContext, managementGroup);

			return office;
		}

		public void Delete(BusinessContext businessContext)
		{
			if(this.Documents.Any ())
			{
				throw new BusinessRuleException ("Des documents sont encore présent dans cette gestion. Suppresion annulée.");
			}
			if(this.Employees.Any ())
			{
				throw new BusinessRuleException ("Des collaborateurs sont encore présent dans cette gestion. Suppresion annulée.");
			}
			if(this.OfficeSenders.Any ())
			{
				throw new BusinessRuleException ("Des expéditeurs sont encore présent dans cette gestion. Suppresion annulée.");
			}

			businessContext.DeleteEntity (this);
		}

		public void DeleteOfficeManagerJobsForUser(BusinessContext businessContext, AiderUserEntity user)
		{
			this.DeleteOfficeJobs (businessContext, user.Contact.Person.Employee, EmployeeJobFunction.GestionnaireAIDER);
		}

		public void DeleteOfficeUserJobsForUser(BusinessContext businessContext, AiderUserEntity user)
		{		
			this.DeleteOfficeJobs (businessContext, user.Contact.Person.Employee, EmployeeJobFunction.UtilisateurAIDER);
		}

		private void DeleteOfficeJobs(BusinessContext businessContext, AiderEmployeeEntity employee, EmployeeJobFunction userFonction)
		{
			if(this.employeeJobs.Any())
			{
				var jobs = this
						.employeeJobs
						.Where (j => j.EmployeeJobFunction == userFonction
						/**/	  && j.Employee == employee
					    /**/   );

				foreach (var job in jobs)
				{
					job.Delete (businessContext);
				}
			}	
		}

		partial void GetOfficeSenders(ref IList<AiderOfficeSenderEntity> value)
		{
			value = this.GetOfficeSenders ().AsReadOnlyCollection ();
		}

		partial void GetEmployees(ref IList<AiderEmployeeEntity> value)
		{
			value = this.EmployeeJobs.Select (x => x.Employee).Distinct ().OrderBy (x => x.Person.DisplayName).AsReadOnlyCollection ();
		}

		partial void GetEmployeeJobs(ref IList<AiderEmployeeJobEntity> value)
		{
			value = this.GetVirtualCollection (ref this.employeeJobs, x => x.Office = this).AsReadOnlyCollection ();
		}

		public void RefreshOfficeShortName()
		{
			var type = this.OfficeType;
			var name = this.OfficeName;
			var text = name.ToLowerInvariant ();
			
			var parishPrefixes = new string[] { "paroisse d'", "paroisse de ", "paroisse du ", "paroisse des ", "pla " };
			var parishMatch    = parishPrefixes.FirstOrDefault (x => text.StartsWith (x));

			if (parishMatch != null)
			{
				name = name.Substring (parishMatch.Length);
				type = Enumerations.OfficeType.Parish;
			}
			else if (this.ParishGroup.IsRegion ())
			{
				type = Enumerations.OfficeType.Region;
			}
			else
			{
				foreach (var tuple in AiderOfficeManagementEntity.GetShortNameReplacementTuples ())
				{
					if (text.StartsWith (tuple.Item1))
					{
						name = tuple.Item2 + name.Substring (tuple.Item1.Length);
						type = tuple.Item3;
						break;
					}
				}
			}

			this.OfficeType = type;
			this.OfficeShortName = name.Trim ();
			this.Region = this.ParishGroup.GetRootRegionCode ();
		}

		private static IEnumerable<System.Tuple<string, string, OfficeType>> GetShortNameReplacementTuples()
		{
			yield return System.Tuple.Create ("formation et accompagnement ", "SFA ", OfficeType.RegionFA);
			yield return System.Tuple.Create ("présence et solidarité", "PS ", OfficeType.RegionPS);
			yield return System.Tuple.Create ("coordination et information", "CI ", OfficeType.RegionCI);
			yield return System.Tuple.Create ("coordination - information", "CI ", OfficeType.RegionCI);
		}


		internal void AddSenderInternal(AiderOfficeSenderEntity settings)
		{
			if (!this.GetOfficeSenders ().Any (s => s == settings))
			{
				this.GetOfficeSenders ().Add (settings);
			}
		}

		internal void RemoveSenderInternal(AiderOfficeSenderEntity settings)
		{
			this.GetOfficeSenders ().Remove (settings);
		}

		private IList<AiderOfficeSenderEntity> GetOfficeSenders()
		{
			if (this.senders == null)
			{
				this.senders = this.ExecuteWithDataContext (d => this.FindOfficeSenders (d), () => new List<AiderOfficeSenderEntity> ());
			}

			return this.senders;
		}

		private IList<AiderOfficeSenderEntity> FindOfficeSenders(DataContext dataContext)
		{
			var example = new AiderOfficeSenderEntity
			{
				Office = this
			};

			return dataContext.GetByExample (example)
							  .OrderBy (x => x.Name)
							  .ToList ();
		}


		internal void AddDocumentInternal(AiderOfficeReportEntity document)
		{
			this.GetDocuments ().Add (document);
		}

		internal void RemoveDocumentInternal(AiderOfficeReportEntity document)
		{
			this.GetDocuments ().Remove (document);
		}

		partial void GetDocuments(ref IList<AiderOfficeReportEntity> value)
		{
			value = this.GetDocuments ().AsReadOnlyCollection ();
		}

		partial void GetRegionalReferees(ref IList<AiderRefereeEntity> value)
		{
			value = this.GetVirtualCollection (ref this.regionalReferees, x => x.Group = this.ParishGroup).AsReadOnlyCollection ();
		}

		partial void GetAssociatedGroups(ref IList<AiderGroupEntity> value)
		{
			value = this.ExecuteWithDataContext (c => this.GetAssociatedGroups (c), null);
		}

		private IList<AiderGroupEntity> GetAssociatedGroups(DataContext context)
		{
			if (this.associatedGroups == null)
			{
				var groups = new HashSet<AiderGroupEntity> ();

				if (this.OfficeType == Enumerations.OfficeType.Parish)
				{
					groups.UnionWith (AiderGroupEntity.FindRegionalGroupsGloballyVisibleToParishes (context, this.ParishGroupPathCache));
					groups.UnionWith (AiderGroupEntity.FindGroupsGloballyVisibleToParishes (context));				
				}
				if (this.OfficeType == Enumerations.OfficeType.Region)
				{
					string path = this.ParishGroupPathCache;
					groups.UnionWith (AiderGroupEntity.FindAllGroupsGloballyVisibleToRegions (context, path));	
				}

				this.associatedGroups = groups.ToList();
			}

			return this.associatedGroups;
		}

		partial void GetRegionDeprecated(ref string value)
		{
			if(this.ParishGroup.Parents.Any ())
			{
				value = this.ParishGroup.Parents.Last ().Name;
			}
			else
			{
				value = this.ParishGroup.Name;
			}
			
		}

		private IList<AiderOfficeReportEntity> GetDocuments()
		{
			if (this.documents == null)
			{
				this.documents = this.ExecuteWithDataContext (d => this.FindDocuments (d), () => new List<AiderOfficeReportEntity> ());
			}

			return this.documents;
		}

		private IList<AiderOfficeReportEntity> FindDocuments(DataContext dataContext)
		{
			var example = new AiderOfficeReportEntity
			{
				Office = this
			};

			return dataContext.GetByExample (example)
							  .OrderBy (x => x.Name)
							  .ToList ();
		}
		
		
		private IList<AiderOfficeSenderEntity>	senders;
		private IList<AiderOfficeReportEntity>	documents;
		private IList<AiderEmployeeJobEntity>	employeeJobs;
		private IList<AiderRefereeEntity>		regionalReferees;
		private IList<AiderGroupEntity>			associatedGroups;
	}
}

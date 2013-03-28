//	Copyright © 2012-2013, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Marc BETTEX, Maintainer: Marc BETTEX

using Epsitec.Aider.Data;
using Epsitec.Aider.Enumerations;
using Epsitec.Aider.Override;

using Epsitec.Common.Support.Extensions;

using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Business.UserManagement;
using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer.Context;
using Epsitec.Cresus.DataLayer.Expressions;
using Epsitec.Cresus.DataLayer.Loader;

using System;

using System.Collections.Generic;

using System.Linq;
using Epsitec.Common.Support;


namespace Epsitec.Aider.Entities
{
	public partial class AiderGroupEntity
	{
		public AiderGroupEntity					Parent
		{
			get
			{
				return this.Parents.LastOrDefault ();
			}
		}

		public IList<AiderGroupEntity>			Parents
		{
			get
			{
				return this.GetParents ().AsReadOnlyCollection ();
			}
		}



		public bool IsChildOf(AiderGroupEntity group)
		{
			return AiderGroupIds.IsSameOrWithinGroup (this.Path, group.Path)
				&& this.GroupLevel > group.GroupLevel;
		}

		public bool IsRegion()
		{
			return this.GroupDef.IsNotNull () && this.GroupDef.IsRegion ();
		}

		public bool IsParish()
		{
			return this.GroupDef.IsNotNull () && this.GroupDef.IsParish ();
		}

		public bool IsNoParish()
		{
			return this.GroupDef.IsNotNull () && this.GroupDef.IsNoParish ();
		}

		public bool CanHaveSubgroups()
		{
			var definition = this.GroupDef;

			return this.GroupLevel < AiderGroupIds.MaxGroupLevel
				&& (definition.IsNull () || definition.SubgroupsAllowed);
		}

		public bool CanSubgroupsBeEdited()
		{
			var definition = this.GroupDef;

			return this.CanHaveSubgroups ()
				&& (definition.IsNull () || definition.Mutability == Mutability.Customizable);
		}

		public bool CanBeEdited()
		{
			return this.GroupDef.IsNull ();
		}

		public bool CanHaveMembers()
		{
			var definition = this.GroupDef;

			return definition.IsNull () || definition.MembersAllowed;
		}

		public bool CanBeEditedByCurrentUser()
		{
			var path = this.Path;
			var user = AiderUserManager.Current.AuthenticatedUser;
			var userParishPath = user.ParishGroupPathCache;
			var userPowerLevel = user.PowerLevel;

			if ((userPowerLevel != UserPowerLevel.None) &&
				(userPowerLevel <= UserPowerLevel.Administrator))
			{
				return true;
			}

			if (this.IsParish () || AiderGroupIds.IsWithinParish (path))
			{
				return user.EnableGroupEditionParish
					&& !string.IsNullOrEmpty (userParishPath)
					&& AiderGroupIds.IsSameOrWithinGroup (path, userParishPath);
			}
			else if (this.IsRegion () || AiderGroupIds.IsWithinRegion (path))
			{
				var userRegionPart = AiderGroupIds.GetParentPath (userParishPath);

				return user.EnableGroupEditionRegion
					&& !string.IsNullOrEmpty (userParishPath)
					&& AiderGroupIds.IsSameOrWithinGroup (path, userRegionPart);
			}
			else
			{
				return user.EnableGroupEditionCanton;
			}
		}



		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText
			(
				TextFormatter.FormatText (this.Name).ApplyBold (), "\n",
				"Membres autorisés: ", this.CanHaveMembers ().ToYesOrNo (), "\n",
				"Sous-groupes autorisés: ", this.CanHaveSubgroups ().ToYesOrNo (), "\n",
				"Sous-groupes modifiables: ", this.CanSubgroupsBeEdited ().ToYesOrNo (), "\n"
			);
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Name);
		}

		public override IEnumerable<FormattedText> GetFormattedEntityKeywords()
		{
			yield return this.Name;
		}


		public int GetDepth()
		{
			var totalDepth = this.ExecuteWithDataContext (c => this.FindDepth (c), this.FindDepth);

			return totalDepth - this.GroupLevel + 1;
		}

		public string GetHierarchicalName(AiderPersonEntity person)
		{
			return this
				.GetHierarchicalParents (person.ParishGroupPathCache)
				.Append (this)
				.Select (g => g.Name)
				.Join (", ");
		}


		public FormattedText GetParticipantsTitle()
		{
			var count = this.CountParticipants ();

			if (count == 1)
			{
				return Resources.FormattedText ("Participant");
			}
			else
			{
				return TextFormatter.FormatText ("Particpants (", count, ")");
			}
		}

		public FormattedText GetParticipantsSummary()
		{
			int count = 10;

			var participants = this.GetParticipants (count + 1)
				.Select (g => g.GetCompactSummary ())
				.CreateSummarySequence (count, "...");

			return FormattedText.Join (FormattedText.FromSimpleText ("\n"), participants);
		}

		public FormattedText GetFunctionParticipantTitle()
		{
			var count = this.CountFunctionParticipants ();

			if (count == 1)
			{
				return Resources.FormattedText ("Participant");
			}
			else
			{
				return TextFormatter.FormatText ("Particpants (", count, ")");
			}
		}

		public FormattedText GetFunctionParticipantSummary()
		{
			int count = 10;

			var groups = this.GetFuntionParticipants (count + 1)
				.Select (g => g.GetCompactSummary ())
				.CreateSummarySequence (count, "...");

			return FormattedText.Join (FormattedText.FromSimpleText ("\n"), groups);
		}



		public static string GetPath(AiderGroupEntity group)
		{
			if (group.IsNull ())
			{
				return null;
			}
			else
			{
				return group.Path;
			}
		}

		public static string GetPath(AiderGroupParticipantEntity group)
		{
			if (group.IsNull ())
			{
				return null;
			}
			else
			{
				return group.Group.Path;
			}
		}


		public static IList<AiderGroupEntity> FindRootGroups(BusinessContext businessContext)
		{
			var example = new AiderGroupEntity ()
			{
				GroupLevel = AiderGroupIds.TopLevel,
			};

			var request = Request.Create (example);
			request.AddSortClause (ValueField.Create (example, x => x.Name));

			return businessContext.DataContext.GetByRequest (request);
		}


		public static AiderGroupEntity Create(BusinessContext businessContext, AiderGroupEntity parent, AiderGroupDefEntity groupDefinition, string name, string pathPart)
		{
			var level = parent.GroupLevel + 1;
			var path = parent.Path + pathPart;

			var group = AiderGroupEntity.Create (businessContext, groupDefinition, name, level, path);

			parent.AddSubgroupInternal (group);

			return group;
		}

		public static AiderGroupEntity Create(BusinessContext businessContext, AiderGroupDefEntity groupDefinition, string name, int level, string path)
		{
			var group = businessContext.CreateAndRegisterEntity<AiderGroupEntity> ();

			group.Name = name;
			group.GroupLevel = level;
			group.Path = path;
			group.GroupDef = groupDefinition;

			return group;
		}

		public AiderGroupEntity CreateSubgroup(BusinessContext businessContext, string name)
		{
			if (!this.CanHaveSubgroups ())
			{
				throw new InvalidOperationException ("This group cannot have subgroups");
			}

			var path = this.GetNextSubgroupPath ();

			return this.CreateSubgroup (businessContext, name, path);
		}

		public AiderGroupEntity CreateSubgroup(BusinessContext businessContext, AiderGroupDefEntity definition)
		{
			var name = definition.Name;
			var path = AiderGroupIds.CreateSubgroupPathFromFullPath (this.Path, definition.PathTemplate);

			var subgroup = this.CreateSubgroup (businessContext, name, path);

			subgroup.GroupDef = definition;

			return subgroup;
		}

		public void DeleteSubgroup(BusinessContext businessContext, AiderGroupEntity subgroup)
		{
			// Recursively delete the children of the subgroup that we want to delete.

			foreach (var child in subgroup.GetSubgroups ().ToList ())
			{
				subgroup.DeleteSubgroup (businessContext, child);
			}

			this.RemoveSubgroupInternal (subgroup);

			// This might be very costly for groups which have a lot of participations.

			var participations = this.FindParticipations (businessContext);

			foreach (var participation in participations)
			{
				businessContext.DeleteEntity (participation);
			}

			if (subgroup.Comment.IsNotNull ())
			{
				businessContext.DeleteEntity (subgroup.Comment);
			}

			businessContext.DeleteEntity (subgroup);
		}

		public void Move(AiderGroupEntity newParent)
		{
			if (!newParent.CanHaveSubgroups ())
			{
				throw new InvalidOperationException ("This group cannot have subgroups");
			}

			// We start by removing this group from its current parent.

			var currentParent = this.Parent;

			if (currentParent.IsNotNull ())
			{
				currentParent.RemoveSubgroupInternal (this);
			}

			// Here we must move the current group and all its subgroups recursively. Since the
			// subgroups of a group are determined by their path only, we must update the path of
			// the current group and of its subgroups. We must also update their group level. To do
			// this, we make a single SQL request to fetch all the direct and the indirect children
			// of the current group. Then we update all their paths and their group level

			var oldPathSize = this.Path.Length;
			var newPath = newParent.GetNextSubgroupPath ();

			var deltaGroupLevel = newParent.GroupLevel + 1 - this.GroupLevel;

			var allSubgroups = this.GetAllSubgroups ();
			var allGroups = allSubgroups.Append (this);

			foreach (var group in allGroups)
			{
				var path = newPath;

				if (group.Path.Length > oldPathSize)
				{
					path += group.Path.Substring (oldPathSize);
				}

				group.Path = path;
				group.GroupLevel += deltaGroupLevel;
			}

			// Finaly, we update the in memory cache of the parents of this group and of all its
			// subgroups. Note that we sort the groups based on their path before looping on them.
			// This ensures that when we update the cache of the parents of a group, the cache of
			// the parents of its parent has already been updated.

			var pathToGroups = allGroups.ToDictionary (g => g.Path);

			this.SetParentInternal (newParent);

			foreach (var group in this.GetAllSubgroups ().OrderBy (g => g.Path))
			{
				var parentPath = AiderGroupIds.GetParentPath (group.Path);
				var parent = pathToGroups[parentPath];

				group.SetParentInternal (parent);
			}
		}

		public void ImportMembers(BusinessContext businessContext, AiderGroupEntity source, Date? startDate, FormattedText comment)
		{
			var participations = source.FindParticipations (businessContext);

			foreach (var participation in participations)
			{
				var person  = participation.Person;
				var legal   = participation.LegalPerson;
				var contact = participation.Contact;

				//	@PA: also handle legal persons

				if ((legal.IsNull ()) &&
					(person.IsNotNull ()) &&
					(person.IsNotMemberOf (this)))
				{
					var what = new Participation
					{
						Group       = this,
						Person      = person,
						LegalPerson = legal,
						Contact     = contact,
					};

					AiderGroupParticipantEntity.StartParticipation (businessContext, what, startDate, comment);
					continue;
				}


			}
		}

		public void Merge(BusinessContext businessContext, AiderGroupEntity other)
		{
			if (!other.CanHaveMembers ())
			{
				throw new InvalidOperationException ("This group cannot have members.");
			}

			if (!this.CanBeEdited ())
			{
				throw new InvalidOperationException ("This groups cannot be merged.");
			}

			if (this.GetSubgroups ().Count > 0)
			{
				throw new InvalidOperationException ("This groups cannot be merged because it has subgroups.");
			}

			var participations = this.FindParticipations (businessContext);

			foreach (var participation in participations)
			{
				if (participation.Person.IsMemberOf (other))
				{
					participation.Delete (businessContext);
				}
				else
				{
					participation.Group = other;
				}
			}

			if (this.Comment.IsNotNull ())
			{
				businessContext.DeleteEntity (this.Comment);
			}

			businessContext.DeleteEntity (this);
		}



		partial void GetSubgroups(ref IList<AiderGroupEntity> value)
		{
			value = this.GetSubgroups ().AsReadOnlyCollection ();
		}


		private string GetNextSubgroupPath()
		{
			// We look for a number that is not used yet in the subgroups.

			var usedNumbers = this.Subgroups
				.Select (g => AiderGroupIds.GetGroupNumber (g.Path))
				.ToSet ();

			int? number = null;

			for (int i = 0; i < AiderGroupIds.MaxGroupNumber; i++)
			{
				if (!usedNumbers.Contains (i))
				{
					number = i;
					break;
				}
			}

			if (!number.HasValue)
			{
				throw new Exception ("Too many subgroups.");
			}

			return AiderGroupIds.CreateCustomSubgroupPath (this.Path, number.Value);
		}


		private IList<AiderPersonEntity> GetParticipants(int count)
		{
			//	This stuff is not cached in the entity, therefore updates in memory won't modify the
			//	value returned by this method. It the entity is not persisted, it will always be an empty
			//	list.

			return this.ExecuteWithDataContext (c => this.GetParticipants (c, count),
												() => new List<AiderPersonEntity> ());
		}

		private IList<AiderPersonEntity> GetParticipants(DataContext dataContext, int count)
		{
			var request = AiderGroupParticipantEntity.CreateParticipantRequest (dataContext, this, true, true, true);

			request.Skip = 0;
			request.Take = count;

			return dataContext.GetByRequest<AiderPersonEntity> (request);
		}

		private IList<AiderPersonEntity> GetFuntionParticipants(int count)
		{
			return this.ExecuteWithDataContext (c => this.GetFunctionParticipants (c, count),
												() => new List<AiderPersonEntity> ());
		}

		private IList<AiderPersonEntity> GetFunctionParticipants(DataContext dataContext, int count)
		{
			var request = AiderGroupParticipantEntity.CreateFunctionMemberRequest (dataContext, this, true, true);

			request.Skip = 0;
			request.Take = count;

			return dataContext.GetByRequest<AiderPersonEntity> (request);
		}

		private IList<AiderGroupEntity> GetSubgroups()
		{
			if (this.subgroups == null)
			{
				this.subgroups = this.ExecuteWithDataContext (c => this.FindSubgroups (c),
															  () => new List<AiderGroupEntity> ());
			}

			return this.subgroups;
		}

		private IList<AiderGroupEntity> FindSubgroups(DataContext dataContext)
		{
			if (!this.CanHaveSubgroups ())
			{
				return new List<AiderGroupEntity> ();
			}

			var example = new AiderGroupEntity ();
			var request = Request.Create (example);

			var path  = this.Path + AiderGroupIds.SubgroupSqlWildcard;
			var level = this.GroupLevel + 1;

			request.AddCondition (dataContext, example, x => x.GroupLevel == level && SqlMethods.Like (x.Path, path));
			request.AddSortClause (ValueField.Create (example, x => x.Name));

			return dataContext.GetByRequest (request);
		}

		private IList<AiderGroupEntity> GetParents()
		{
			if (this.parents == null)
			{
				this.parents = this.ExecuteWithDataContext (c => this.FindParents (c),
															() => new List<AiderGroupEntity> ());
			}

			return this.parents;
		}

		private IList<AiderGroupEntity> FindParents(DataContext dataContext)
		{
			var groupPaths = AiderGroupIds.GetParentPaths (this.Path).ToList ();

			if (groupPaths.Count == 0)
			{
				return new List<AiderGroupEntity> ();
			}

			var example = new AiderGroupEntity ();

			var request = Request.Create (example);
			request.AddCondition (dataContext, example, g => SqlMethods.IsInSet (g.Path, groupPaths));
			request.AddSortClause (ValueField.Create (example, g => g.GroupLevel), SortOrder.Ascending);

			return dataContext.GetByRequest<AiderGroupEntity> (request);
		}


		private void AddSubgroupInternal(AiderGroupEntity group)
		{
			this.GetSubgroups ().Add (group);
		}

		private void RemoveSubgroupInternal(AiderGroupEntity group)
		{
			this.GetSubgroups ().Remove (group);
		}

		private void SetParentInternal(AiderGroupEntity newParent)
		{
			if (this.parents == null)
			{
				this.parents = new List<AiderGroupEntity> ();
			}
			else
			{
				this.parents.Clear ();
			}

			this.parents.AddRange (newParent.GetParents ().Append (newParent));
		}


		private AiderGroupEntity CreateSubgroup(BusinessContext businessContext, string name, string path)
		{
			var subgroup = businessContext.CreateAndRegisterEntity<AiderGroupEntity> ();

			subgroup.Name = name;
			subgroup.Path = path;
			subgroup.GroupLevel = this.GroupLevel + 1;

			this.AddSubgroupInternal (subgroup);
			subgroup.SetParentInternal (this);

			return subgroup;
		}




		private IEnumerable<AiderGroupParticipantEntity> FindParticipations(BusinessContext businessContext)
		{
			var example = new AiderGroupParticipantEntity ()
			{
				Group = this
			};

			return businessContext.DataContext.GetByExample (example);
		}

		private IEnumerable<AiderGroupEntity> GetHierarchicalParents(string parishPath)
		{
			var currentPath = this.Path;

			var skip = 0;

			if (AiderGroupIds.IsWithinParish (currentPath))
			{
				skip += 1;

				if (AiderGroupIds.IsWithinSameParish (currentPath, parishPath))
				{
					skip += 1;
				}
			}
			else if (AiderGroupIds.IsWithinRegion (currentPath))
			{
				if (AiderGroupIds.IsWithinSameRegion (currentPath, parishPath))
				{
					skip += 1;
				}
			}

			return this.GetParents ().Skip (skip);
		}


		private IList<AiderGroupEntity> GetAllSubgroups()
		{
			return this.ExecuteWithDataContext (c => this.FindAllSubgroups (c), this.FindAllSubgroups);
		}

		private IList<AiderGroupEntity> FindAllSubgroups(DataContext dataContext)
		{
			if (!this.CanHaveSubgroups ())
			{
				return new List<AiderGroupEntity> ();
			}

			var example = new AiderGroupEntity ();
			var request = Request.Create (example);

			request.AddCondition (dataContext, example, x => x.GroupLevel > this.GroupLevel);
			request.AddCondition (dataContext, example, x => SqlMethods.Like (x.Path, this.Path + "%"));

			return dataContext.GetByRequest (request);
		}

		private IList<AiderGroupEntity> FindAllSubgroups()
		{
			var groups = new List<AiderGroupEntity> ();

			this.FindAllSubgroups (groups);

			return groups;
		}

		private void FindAllSubgroups(IList<AiderGroupEntity> groups)
		{
			foreach (var subgroup in this.GetSubgroups ())
			{
				groups.Add (subgroup);

				subgroup.FindAllSubgroups (groups);
			}
		}

		private int FindDepth(DataContext dataContext)
		{
			// Here it would be cool to make a request that simply returns the maximum, but it can't
			// be done now. This is something that the DataLayer does not implement right now.

			var subgroups = this.FindAllSubgroups (dataContext);

			if (subgroups.Count == 0)
			{
				return this.GroupLevel;
			}

			return subgroups.Max (g => g.GroupLevel);
		}

		private int FindDepth()
		{
			var subgroups = this.GetSubgroups ();

			if (subgroups.Count == 0)
			{
				return this.GroupLevel;
			}

			return this.GetSubgroups ().Max (g => g.FindDepth ());
		}

		private int CountParticipants()
		{
			//	This stuff is not cached in the entity, therefore updates in memory won't modify the
			//	value returned by this method. If the entity is not persisted, it will always be 0.

			return this.ExecuteWithDataContext (c => this.CountParticipants (c), () => 0);
		}

		private int CountParticipants(DataContext dataContext)
		{
			var request = AiderGroupParticipantEntity.CreateParticipantRequest (dataContext, this, false, true, true);

			return dataContext.GetCount (request);
		}

		private int CountFunctionParticipants()
		{
			//	This stuff is not cached in the entity, therefore updates in memory won't modify the
			//	value returned by this method. If the entity is not persisted, it will always be 0.
			
			return this.ExecuteWithDataContext (c => this.CountFunctionParticipants (c), () => 0);
		}

		private int CountFunctionParticipants(DataContext dataContext)
		{
			var request = AiderGroupParticipantEntity.CreateFunctionMemberRequest (dataContext, this, true, false);

			return dataContext.GetCount (request);
		}




		private IList<AiderGroupEntity>			subgroups;
		private IList<AiderGroupEntity>			parents;
	}
}

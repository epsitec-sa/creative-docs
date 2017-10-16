//	Copyright © 2014-2015, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Samuel LOUP, Maintainer: Samuel LOUP

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Aider.Enumerations;
using System.Linq;
using System.Collections.Generic;
using Epsitec.Cresus.Database;
using Epsitec.Cresus.DataLayer.Context;

namespace Epsitec.Aider.Entities
{
	public partial class AiderOfficeTaskEntity
	{
		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText (this.Kind, "pour ", this.Process.GetSubject ());
		}

		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText (this.Kind, "pour ", this.Process.GetSubject ());
		}

		public FormattedText GetTaskInfo()
		{
			var info = new FormattedText ("Information:\n");
			switch (this.Kind)
			{			
				case OfficeTaskKind.EnterNewAddress:
					info = info.AppendLine (this.Process.GetSubject ());
					info = info.AppendLine ("Adresse en cours:");
					info = info.AppendLine (this.GetSourceEntity<AiderContactEntity> (this.GetDataContext ()).GetAddress ().GetSummary ());
					break;
				case OfficeTaskKind.CheckParticipation:
					var p = this.GetSourceEntity<AiderGroupParticipantEntity> (this.GetDataContext ());
					info = info.AppendLine (this.Process.GetSubject ());
					info = info.AppendLine ("Groupe concerné:");
					info = info.AppendLine (p.GetSummaryWithHierarchicalGroupName ());	
					info = info.AppendLine ("Chemin de la participation:");
					info = info.AppendLine (p.Group.GetNameParishNameWithRegion ());
					info = info.AppendLine ("\n.");
					break;
			}
			return info;
		}

		partial void SetSubject(string value)
		{
			throw new System.NotImplementedException ("Do not use this method");
		}

		partial void GetSubject(ref string value)
		{
			value = this.Process.GetSubject ().ToSimpleText ();
		}

		public FormattedText GetTaskHelp()
		{
			var help = new FormattedText ("Aide:\n");
			switch (this.Kind)
			{
				case OfficeTaskKind.EnterNewAddress:
					help = help.AppendLine ("Vous pouvez choisir d'annuler, si vous avez conservé cette");
					help = help.AppendLine ("participation par erreur.");
					break;
				case OfficeTaskKind.CheckParticipation:
					if (this.Process.Type == OfficeProcessType.PersonsOutputProcess)
					{
						help = help.AppendLine ("Si vous conservez cette participation, vous devrez");
						help = help.AppendLine ("renseigner une nouvelle adresse pour cette personne.");
					}
					else
					{
						help = help.AppendLine ("La personne à changé de paroisse. Souhaitez-vous");
						help = help.AppendLine ("tout de même conserver cette participation ?");
					}

					break;
			}
			return help;
		}

		public AiderEntity GetSourceEntity<AiderEntity>(DataContext dataContext)
			where AiderEntity : AbstractEntity
		{
			var entity = (AiderEntity) dataContext.GetPersistedEntity (this.SourceId);
			return entity;
		}

		public static AiderOfficeTaskEntity Create(
			BusinessContext businessContext, 
			OfficeTaskKind kind, 
			AiderOfficeManagementEntity office, 
			AiderOfficeProcessEntity process, 
			AbstractEntity source)
		{

			switch (kind)
			{
				case OfficeTaskKind.EnterNewAddress:
					if (source.GetType () != typeof (AiderContactEntity))
					{
						throw new BusinessRuleException ("Le type d'entité fourni ne correspond pas au genre de tâche");
					}
					break;
				case OfficeTaskKind.CheckParticipation:
					if (source.GetType () != typeof (AiderGroupParticipantEntity))
					{
						throw new BusinessRuleException ("Le type d'entité fourni ne correspond pas au genre de tâche");
					}
					break;
			}

			var task = businessContext.CreateAndRegisterEntity<AiderOfficeTaskEntity> ();
			task.Process  = process;
			task.Office   = office;
			task.Kind     = kind;
			task.IsDone   = false;
			task.SourceId = businessContext.DataContext.GetPersistedId (source);
			task.GroupPathCache = office.ParishGroupPathCache;
			return task;
		}
	}
}

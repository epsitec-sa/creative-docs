﻿//	Copyright © 2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Cresus.Core.Library;

using System.Collections.Generic;
using System.Linq;
using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Controllers;

namespace Epsitec.Cresus.Compta
{
	public class ComptaApplication : CoreInteractiveApp
	{
		public ComptaApplication()
		{
		}

		public override string					ShortWindowTitle
		{
			get
			{
				return "Crésus MCH-2";
			}
		}
		public override string					ApplicationIdentifier
		{
			get
			{
				return "Cr.MCH-2";
			}
		}

		protected override void CreateManualComponents(IList<System.Action> initializers)
		{
			initializers.Add (this.InitializeApplication);
		}

		protected override void InitializeEmptyDatabase()
		{
			var dataContext = this.Data.CreateDataContext ("hack");
			
			Hack.PopulateUsers (dataContext);

			var comptabilité = dataContext.CreateEntity<ComptabilitéEntity> ();
			var logic = new Logic (comptabilité);

			logic.ApplyRule (RuleType.Setup, comptabilité);

			dataContext.SaveChanges ();
		}

		protected override System.Xml.Linq.XDocument LoadApplicationState()
		{
			return null;
		}

		protected override void SaveApplicationState(System.Xml.Linq.XDocument doc)
		{
		}

		private void InitializeApplication()
		{
			this.businessContext = new BusinessContext (this.Data);
			var comptabilité = this.Data.GetRepository<ComptabilitéEntity> ().GetAllEntities ().FirstOrDefault ();

			var window = this.Window;
			var controller = new DocumentWindowController (this, new List<AbstractController> (), this.businessContext, comptabilité, TypeDeDocumentComptable.Journal);
			controller.SetupApplicationWindow (window);
			window.Root.BackColor = Common.Drawing.Color.FromName ("White");
		}

		private BusinessContext					businessContext;
	}
}

//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

using Epsitec.Cresus.Core.Business;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Library;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Entities
{
	public partial class WorkflowThreadEntity
	{
		public WorkflowStepEntity CreationEvent
		{
			get
			{
				WorkflowStepEntity step = this.History.FirstOrDefault ();
				DataContext dataContext = this.GetDataContext ();

				return dataContext.WrapNullEntity (step);
			}
		}

		public override FormattedText GetCompactSummary()
		{
			return TextFormatter.FormatText ("Créé le ", this.History.First ().Date);
		}

		public override FormattedText GetSummary()
		{
			return TextFormatter.FormatText ("Créé le ", this.History.First ().Date);
		}

		/// <summary>
		/// Gets the settings associated with this workflow thread. The returned collection
		/// is a copy of the actual settings (see method <see cref="SetSettings"/>).
		/// </summary>
		/// <returns>The settings.</returns>
		public SettingsCollection GetSettings()
		{
			var args = this.SerializedArgs;

			if (args.IsNotNull ())
			{
				return args.GetSettings ();
			}
			else
			{
				return new SettingsCollection ();
			}
		}

		/// <summary>
		/// Sets the settings for this workflow thread. This will overwrite any previous
		/// settings.
		/// </summary>
		/// <param name="businessContext">The business context.</param>
		/// <param name="settings">The settings.</param>
		public void SetSettings(IBusinessContext businessContext, SettingsCollection settings)
		{
			if (this.SerializedArgs.IsNull ())
			{
				if ((settings == null) ||
					(settings.Count == 0))
				{
					return;
				}

				this.SerializedArgs = businessContext.CreateEntity<XmlBlobEntity> ();
			}

			this.SerializedArgs.SetSettings (settings);
		}
	}
}

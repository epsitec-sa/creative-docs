//	Copyright © 2010-2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
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
		/// Gets the arguments associated with this workflow thread. The returned collection
		/// is a copy of the actual arguments (see method <see cref="SetArgs"/>).
		/// </summary>
		/// <returns>The arguments.</returns>
		public SettingsCollection GetArgs()
		{
			var args = this.SerializedArgs;

			if (args.IsNotNull ())
			{
				return args.GetSettingsCollection ();
			}
			else
			{
				return new SettingsCollection ();
			}
		}

		/// <summary>
		/// Sets the arguments for this workflow thread. This will overwrite any previous
		/// arguments.
		/// </summary>
		/// <param name="businessContext">The business context.</param>
		/// <param name="args">The arguments.</param>
		public void SetArgs(IBusinessContext businessContext, SettingsCollection args)
		{
			if (this.SerializedArgs.IsNull ())
			{
				if ((args == null) ||
					(args.Count == 0))
				{
					return;
				}

				this.SerializedArgs = businessContext.CreateEntity<XmlBlobEntity> ();
			}

			this.SerializedArgs.SetSettingsCollection (args);
		}
	}
}

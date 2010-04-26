//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;

namespace Epsitec.Cresus.Core.EntitiesAccessors
{
	public class AbstractAccessor
	{
		public AbstractAccessor(AbstractEntity entity, bool grouped)
		{
			System.Diagnostics.Debug.Assert (entity != null);
			this.entity = entity;
			this.grouped = grouped;
		}


		public AbstractEntity Entity
		{
			get
			{
				return this.entity;
			}
		}

		/// <summary>
		/// Indique que la tuile qui affichera l'entité est groupée.
		/// Il n'y aura donc qu'un titre pour l'ensemble du groupe.
		/// </summary>
		/// <value><c>true</c> if grouped; otherwise, <c>false</c>.</value>
		public bool Grouped
		{
			get
			{
				return this.grouped;

			}
		}


		public virtual string Icon
		{
			get
			{
				return "?";
			}
		}

		public virtual string Title
		{
			get
			{
				return "?";
			}
		}

		public virtual string Summary
		{
			get
			{
				return null;
			}
		}


		protected static string SummaryPostprocess(string summary)
		{
			summary = Misc.RemoveLastBreakLine (summary);

			if (string.IsNullOrEmpty (summary))
			{
				summary = "<i>(vide)</i>";
			}

			return summary;
		}


		private readonly AbstractEntity entity;
		private readonly bool grouped;
	}
}

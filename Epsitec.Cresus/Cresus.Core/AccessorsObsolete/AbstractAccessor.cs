//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using System.Collections.Generic;
using System.Linq;
using System.Text;

using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;
using Epsitec.Cresus.Core.Controllers;

namespace Epsitec.Cresus.Core.Accessors
{
	public abstract class AbstractAccessor
	{
		protected AbstractAccessor(object parentEntities, bool grouped)
		{
			this.parentEntities = parentEntities;
			this.grouped = grouped;
		}


		public object ParentEntities
		{
			get
			{
				return this.parentEntities;
			}
		}

		/// <summary>
		/// Indique que la tuile qui affichera l'entité est groupée avec d'autres du même type.
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

		public bool EnableAddAndRemove
		{
			get;
			set;
		}

		public ViewControllerMode ViewControllerMode
		{
			get;
			set;
		}

		
		public abstract string IconUri
		{
			get;
		}

		public abstract string Title
		{
			get;
		}

		public string Summary
		{
			get
			{
				string summary;
				
				summary = this.GetSummary ();
				summary = Misc.RemoveLastLineBreak (summary);
				
				return string.IsNullOrEmpty (summary) ? "<i>(vide)</i>" : summary;
			}
		}

		protected abstract string GetSummary();

		public virtual string RemoveQuestion
		{
			get
			{
				// TODO: Le texte se termine par "<br/> " pour contourner un bug !
				return string.Format ("<b>Voulez-vous supprimer l'élément suivant :</b><br/><br/>{0}<br/>{1}<br/> ", this.Title, this.Summary);
			}
		}


		public virtual void WidgetInitialize(Widget widget, object unspecifiedEntitie)
		{
		}


		private readonly object parentEntities;
		private readonly bool grouped;
	}
}

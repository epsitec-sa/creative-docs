//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;

using Epsitec.Cresus.Compta.Accessors;
using Epsitec.Cresus.Compta.Entities;
using Epsitec.Cresus.Compta.Helpers;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Compta.Permanents.Data
{
	/// <summary>
	/// Cette classe décrit les paramètres d'affichage permanents génériques de la comptabilité.
	/// </summary>
	public abstract class AbstractPermanents : ISettingsData
	{
		public virtual void SetComptaEntity(ComptaEntity compta)
		{
			this.comptaEntity = compta;
		}


		public virtual void Clear()
		{
		}


		public virtual AbstractPermanents CopyFrom()
		{
			return null;
		}

		public virtual void CopyTo(AbstractPermanents dst)
		{
		}

		public virtual bool CompareTo(AbstractPermanents other)
		{
			return true;
		}


		protected ComptaEntity						comptaEntity;
	}
}

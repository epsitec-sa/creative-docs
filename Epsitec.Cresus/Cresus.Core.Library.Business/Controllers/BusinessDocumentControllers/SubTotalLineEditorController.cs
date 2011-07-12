//	Copyright © 2010, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Daniel ROUX, Maintainer: Daniel ROUX

using Epsitec.Common.Drawing;
using Epsitec.Common.Widgets;
using Epsitec.Common.Types;
using Epsitec.Common.Types.Converters;
using Epsitec.Common.Support;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core;
using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.Core.Controllers;
using Epsitec.Cresus.Core.Controllers.DataAccessors;
using Epsitec.Cresus.Core.Widgets;
using Epsitec.Cresus.Core.Widgets.Tiles;
using Epsitec.Cresus.Core.Helpers;
using Epsitec.Cresus.Core.Business;

using Epsitec.Cresus.DataLayer.Context;

using System.Collections.Generic;
using System.Linq;

namespace Epsitec.Cresus.Core.Controllers.BusinessDocumentControllers
{
	public class SubTotalLineEditorController : AbstractLineEditorController
	{
		public SubTotalLineEditorController(AccessData accessData)
			: base (accessData)
		{
		}

		protected override void CreateUI(UIBuilder builder)
		{
		}

		public override FormattedText TitleTile
		{
			get
			{
				return "Sous-total";
			}
		}


		private SubTotalDocumentItemEntity Entity
		{
			get
			{
				return this.entity as SubTotalDocumentItemEntity;
			}
		}
	}
}

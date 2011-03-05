//	Copyright © 2011, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Types;
using Epsitec.Common.Support.EntityEngine;

using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Controllers.DataAccessors.DynamicFactories
{
	internal abstract class DynamicFactory
	{
		public abstract object CreateUI(EditionTile tile, UIBuilder builder);
		
		public static Caption GetInputCaption(LambdaExpression expression)
		{
			return EntityInfo.GetFieldCaption (expression);
		}

		public static string GetInputTitle(Caption caption)
		{
			if (caption == null)
			{
				return null;
			}

			if (caption.HasLabels)
			{
				return caption.DefaultLabel;
			}

			return caption.Description
				?? TextFormatter.FormatText (caption.Name).ApplyFontColor (DynamicFactory.RedColor).ToString ();
		}


		private static readonly Color RedColor = Color.FromName ("Red");
	}
}

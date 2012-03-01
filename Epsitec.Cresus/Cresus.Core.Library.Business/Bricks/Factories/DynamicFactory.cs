//	Copyright © 2011-2012, EPSITEC SA, CH-1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

using Epsitec.Common.Drawing;
using Epsitec.Common.Support.EntityEngine;
using Epsitec.Common.Types;
using Epsitec.Common.Widgets;

using Epsitec.Cresus.Core.Widgets.Tiles;

using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace Epsitec.Cresus.Core.Bricks.Factories
{
	// TODO Refractor the descendants of this class because they share a lot of their code.

	/// <summary>
	/// The <c>DynamicFactory</c> class is used to dynamically create a piece of user
	/// interface for a given entity field.
	/// </summary>
	internal abstract class DynamicFactory
	{
		/// <summary>
		/// Creates the UI to edit the underlying entity field.
		/// </summary>
		/// <param name="tile">The tile widget where the UI should be created.</param>
		/// <param name="builder">The UI builder.</param>
		/// <returns>The widget created by the <see cref="UIBuilder"/>.</returns>
		public abstract object CreateUI(FrameBox tile, UIBuilder builder);


		/// <summary>
		/// Gets the caption associated with the entity field, by analyzing a lambda
		/// expression based on a property getter.
		/// </summary>
		/// <param name="expression">The expression.</param>
		/// <returns>The <see cref="Caption"/> if one exists; otherwise, <c>null</c>.</returns>
		public static Caption GetInputCaption(LambdaExpression expression)
		{
			return EntityInfo.GetFieldCaption (expression);
		}

		/// <summary>
		/// Gets the title associated with a caption; this will either be the default label
		/// or the description; if no label and no description were defined, returns the name
		/// of the caption, with a red color.
		/// </summary>
		/// <param name="caption">The caption.</param>
		/// <returns>The title associated with the caption if the caption is not <c>null</c>; otherwise, <c>null</c>.</returns>
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

		/// <summary>
		/// </summary>
		/// <param name="caption">The caption.</param>
		public static string GetInputName(Caption caption)
		{
			if (caption == null)
			{
				throw new System.ArgumentNullException ("Caption not defined");
			}
			else
			{
				return caption.Name;
			}
		}

		private static readonly Color RedColor = Color.FromName ("Red");
	}
}

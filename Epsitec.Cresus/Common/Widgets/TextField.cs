//	Copyright © 2003-2012, EPSITEC SA, 1400 Yverdon-les-Bains, Switzerland
//	Author: Pierre ARNAUD, Maintainer: Pierre ARNAUD

[assembly: Epsitec.Common.Types.DependencyClass (typeof (Epsitec.Common.Widgets.TextField))]

namespace Epsitec.Common.Widgets
{
	/// <summary>
	/// The <c>TextField</c> class implements a simple text field.
	/// </summary>
	public class TextField : AbstractTextField
	{
		/// <summary>
		/// Initializes a new instance of the <see cref="TextField"/> class.
		/// </summary>
		public TextField()
			: this (TextFieldStyle.Normal)
		{
		}

		public TextField(TextFieldStyle style)
			: base (style)
		{
		}

		/// <summary>
		/// Initializes a new instance of the <see cref="TextField"/> class.
		/// </summary>
		/// <param name="embedder">The embedder.</param>
		public TextField(Widget embedder)
			: this ()
		{
			this.SetEmbedder (embedder);
		}
	}
}

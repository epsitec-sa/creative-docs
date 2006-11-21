using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Widget permettant d'�diter un Caption.Type.
	/// </summary>
	public class TypeEditorBinary : AbstractTypeEditor
	{
		public TypeEditorBinary()
		{
			Widget group;

			this.CreateStringLabeled(Res.Strings.Viewers.Types.Binary.Mime, this, out group, out this.fieldMime);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 0);
			this.fieldMime.PreferredWidth = 400;
			this.fieldMime.TextChanged += new EventHandler(this.HandleTextFieldChanged);
		}

		public TypeEditorBinary(Widget embedder) : this()
		{
			this.SetEmbedder(embedder);
		}
		
		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.fieldMime.TextChanged -= new EventHandler(this.HandleTextFieldChanged);
			}
			
			base.Dispose(disposing);
		}


		public override string GetSummary()
		{
			//	Retourne le texte du r�sum�.
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			this.PutSummaryInitialise();

			BinaryType type = this.AbstractType as BinaryType;
			this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.Mime, type.MimeType);

			return builder.ToString();
		}


		protected override void UpdateContent()
		{
			//	Met � jour le contenu de l'�diteur.
			BinaryType type = this.AbstractType as BinaryType;

			this.ignoreChange = true;
			this.fieldMime.Text = type.MimeType;
			this.ignoreChange = false;
		}


		private void HandleTextFieldChanged(object sender)
		{
			if (this.ignoreChange)
			{
				return;
			}

			//	[Note1] On demande le type avec un ResourceAccess.GetField.
			BinaryType type = this.AbstractType as BinaryType;

			if (sender == this.fieldMime)
			{
				type.DefineMimeType(this.fieldMime.Text);
			}

			//	[Note1] Cet appel va provoquer le ResourceAccess.SetField.
			this.OnContentChanged();
		}


		protected TextField						fieldMime;
	}
}

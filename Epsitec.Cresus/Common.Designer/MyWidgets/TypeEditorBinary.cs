using Epsitec.Common.Widgets;
using Epsitec.Common.Support;
using Epsitec.Common.Drawing;
using Epsitec.Common.Types;

namespace Epsitec.Common.Designer.MyWidgets
{
	/// <summary>
	/// Widget permettant d'éditer un Caption.Type.
	/// </summary>
	public class TypeEditorBinary : AbstractTypeEditor
	{
		public TypeEditorBinary()
		{
			FrameBox group;

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
			//	Retourne le texte du résumé.
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			this.PutSummaryInitialise();

#if true
			object value;

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceBinaryType.MimeType);
			if (!UndefinedValue.IsUndefinedValue(value))
			{
				this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.Mime, (string) value);
			}
#else
			BinaryType type = this.AbstractType as BinaryType;
			this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.Mime, type.MimeType);
#endif

			return builder.ToString();
		}


		protected override void UpdateContent()
		{
			//	Met à jour le contenu de l'éditeur.
#if true
			this.ignoreChange = true;

			object value = this.structuredData.GetValue(Support.Res.Fields.ResourceBinaryType.MimeType);
			if (UndefinedValue.IsUndefinedValue(value))
			{
				this.fieldMime.Text = "";
			}
			else
			{
				this.fieldMime.Text = (string) value;
			}
			
			this.ignoreChange = false;
#else
			BinaryType type = this.AbstractType as BinaryType;

			this.ignoreChange = true;
			this.fieldMime.Text = type.MimeType;
			this.ignoreChange = false;
#endif
		}


		private void HandleTextFieldChanged(object sender)
		{
			if (this.ignoreChange)
			{
				return;
			}

#if true
			if (sender == this.fieldMime)
			{
				this.structuredData.SetValue(Support.Res.Fields.ResourceBinaryType.MimeType, this.fieldMime.Text);
			}

			this.OnContentChanged();
			this.UpdateContent();
			this.module.AccessTypes.SetLocalDirty();
#else
			//	[Note1] On demande le type avec un ResourceAccess.GetField.
			BinaryType type = this.AbstractType as BinaryType;

			if (sender == this.fieldMime)
			{
				type.DefineMimeType(this.fieldMime.Text);
			}

			//	[Note1] Cet appel va provoquer le ResourceAccess.SetField.
			this.OnContentChanged();
#endif
		}


		protected TextField						fieldMime;
	}
}

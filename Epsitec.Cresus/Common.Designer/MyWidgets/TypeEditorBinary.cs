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
		public TypeEditorBinary(Module module)
		{
			this.module = module;
			ResetBox group;
			this.CreateStringLabeled(Res.Strings.Viewers.Types.Binary.Mime, this, out group, out this.fieldMime);
			group.Dock = DockStyle.StackBegin;
			group.Margins = new Margins(0, 0, 0, 0);
			this.fieldMime.PreferredWidth = 400;
			this.fieldMime.EditionAccepted += new EventHandler(this.HandleTextFieldChanged);
		}

		
		protected override void Dispose(bool disposing)
		{
			if ( disposing )
			{
				this.fieldMime.EditionAccepted -= new EventHandler(this.HandleTextFieldChanged);
			}
			
			base.Dispose(disposing);
		}


		public override string GetSummary()
		{
			//	Retourne le texte du résumé.
			System.Text.StringBuilder builder = new System.Text.StringBuilder();
			this.PutSummaryInitialise();

			object value;

			value = this.structuredData.GetValue(Support.Res.Fields.ResourceBinaryType.MimeType);
			if (!UndefinedValue.IsUndefinedValue(value))
			{
				this.PutSummaryValue(builder, Res.Strings.Viewers.Types.Summary.Mime, (string) value);
			}

			return builder.ToString();
		}


		public override void UpdateContent()
		{
			//	Met à jour le contenu de l'éditeur.
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
		}


		private void HandleTextFieldChanged(object sender)
		{
			if (this.ignoreChange)
			{
				return;
			}

			if (sender == this.fieldMime)
			{
				this.structuredData.SetValue(Support.Res.Fields.ResourceBinaryType.MimeType, this.fieldMime.Text);
			}

			this.OnContentChanged();
			this.UpdateContent();
			this.module.AccessTypes.SetLocalDirty();
		}


		protected TextFieldEx					fieldMime;
	}
}

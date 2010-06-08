using Epsitec.Cresus.Core.Entities;
using Epsitec.Cresus.DataLayer;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Data
{


	public class CommentRepository : Repository
	{


		public CommentRepository(DataContext dataContext)
			: base (dataContext)
		{
		}


		public IEnumerable<CommentEntity> GetCommentsByExample(CommentEntity example)
		{
			return this.GetEntitiesByExample<CommentEntity> (example);
		}


		public CommentEntity GetCommentByExample(CommentEntity example)
		{
			return this.GetEntityByExample<CommentEntity> (example);
		}


		public IEnumerable<CommentEntity> GetAllComments()
		{
			CommentEntity example = this.CreateCommentExample ();

			return this.GetCommentsByExample (example);
		}


		public CommentEntity GetCommentByText(string text)
		{
			CommentEntity example = this.CreateCommentExampleByText (text);

			return this.GetCommentByExample (example);
		}


		public CommentEntity CreateCommentExample()
		{
			return this.CreateExample<CommentEntity> ();
		}


		private CommentEntity CreateCommentExampleByText(string text)
		{
			CommentEntity example = this.CreateCommentExample ();
			example.Text = text;

			return example;
		}


	}


}

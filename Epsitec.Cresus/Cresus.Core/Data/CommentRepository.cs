using Epsitec.Cresus.Core.Entities;

using Epsitec.Cresus.DataLayer;
using Epsitec.Cresus.DataLayer.Browser;

using System.Collections.Generic;


namespace Epsitec.Cresus.Core.Data
{


	public class CommentRepository : Repository
	{


		public CommentRepository(DataContext dataContext) : base (dataContext)
		{
		}


		public IEnumerable<CommentEntity> GetCommentsByExample(CommentEntity example)
		{
			return this.GetEntitiesByExample<CommentEntity> (example);
		}


		public IEnumerable<CommentEntity> GetCommentsByExample(CommentEntity example, Request constrainer)
		{
			return this.GetEntitiesByExample<CommentEntity> (example, constrainer);
		}


		public IEnumerable<CommentEntity> GetCommentsByExample(CommentEntity example, int index, int count)
		{
			return this.GetEntitiesByExample<CommentEntity> (example, index, count);
		}


		public IEnumerable<CommentEntity> GetCommentsByExample(CommentEntity example, Request constrainer, int index, int count)
		{
			return this.GetEntitiesByExample<CommentEntity> (example, constrainer, index, count);
		}


		public IEnumerable<CommentEntity> GetAllComments()
		{
			CommentEntity example = this.CreateCommentExample ();

			return this.GetCommentsByExample (example);
		}


		public IEnumerable<CommentEntity> GetAllComments(int index, int count)
		{
			CommentEntity example = this.CreateCommentExample ();

			return this.GetCommentsByExample (example, index, count);
		}


		public IEnumerable<CommentEntity> GetCommentsByText(string text)
		{
			CommentEntity example = this.CreateCommentExampleByText (text);

			return this.GetCommentsByExample (example);
		}


		public IEnumerable<CommentEntity> GetCommentsByText(string text, int index, int count)
		{
			CommentEntity example = this.CreateCommentExampleByText (text);

			return this.GetCommentsByExample (example, index, count);
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

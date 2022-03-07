using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.DataContext;
using DAL.Entities;
using DAL.Repositories;
using Models.Comment.Output;

namespace Services.Services
{
    public class PostCommentService : GenericRepository<PostComment>, IPostCommentService
    {
        public PostCommentService(StoreContext dbContext) : base(dbContext)
        {
        }

        public Task<CommentOutput> Create()
        {
            throw new NotImplementedException();
        }
    }
    public interface IPostCommentService
    {
        public Task<CommentOutput> Create();
    }
}
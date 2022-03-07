using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL.DataContext;
using DAL.Entities;
using DAL.Entities.Identity;
using DAL.Repositories;
using Microsoft.EntityFrameworkCore;
namespace Services.Services
{
    public class ConnectionService : GenericRepository<UserConnection>, IConnectionService
    {
        StoreContext dbContext;
        public ConnectionService(StoreContext dbContext) : base(dbContext)
        {
            this.dbContext = dbContext;
        }
        public async Task<bool> newConnection(User user, string ConnectionId, string Agent)
        {
            try
            {

                var con = base.GetQuery().Where(s => s.ConnectionID.Equals(ConnectionId)).FirstOrDefault();
                if (con == null)
                {
                    con = new UserConnection
                    {
                        ConnectionID = ConnectionId,
                        UserAgent = Agent,
                        ConnecteDateTime = DateTime.Now,
                        userid = user.Id
                    };
                    await base.InsertAsync(con);
                }
                else
                {
                    con.ConnecteDateTime = DateTime.Now;
                    base.Update(con);
                }
                return await base.CompleteAsync() ? true : false;
            }
            catch
            {
                return false;
            }
        }
        public async Task<bool> delete(string ConnectionId)
        {
            var connection = base.GetQuery().Where(s => s.ConnectionID.Equals(ConnectionId)).FirstOrDefault();
            if (connection != null)
            {
                await base.DeleteAsync(connection.Id);
                return await base.CompleteAsync() ? true : false;
            }
            return false;
        }
        public async Task<List<UserConnection>> GetUserFollowersConnection(string userid)
        {
            var usersids = await dbContext.Follows.Where(i => i.FollowedUserId.Equals(userid)).Select(i => i.FollowedUser.UserConnections).ToListAsync();
            var Connections = new List<UserConnection>();
            usersids.ForEach(s => Connections.AddRange(s));
            return Connections;
        }
        public async Task<List<UserConnection>> GetUserFollowersConnection(User user) =>
         await GetUserFollowersConnection(user.Id);
        public async Task<List<UserConnection>> GetConnections(string username) =>
          await GetQuery().Where(i => i.user.NormalizedUserName.Equals(username.ToUpper())).ToListAsync();
        public async Task<List<UserConnection>> GetConnections(User user) =>
            await GetQuery().Where(i => i.user.NormalizedUserName.Equals(user.NormalizedUserName)).ToListAsync();

        public async Task<List<UserConnection>> GetPostUserConnection(int Postid) =>
           await dbContext.Posts.Where(i => i.Id == Postid).Select(s => s.User.UserConnections).FirstOrDefaultAsync();

    }
    public interface IConnectionService
    {
        public Task<List<UserConnection>> GetConnections(string username);
        public Task<List<UserConnection>> GetConnections(User user);
        public Task<List<UserConnection>> GetUserFollowersConnection(string Userid);
        public Task<List<UserConnection>> GetUserFollowersConnection(User user);
        public Task<List<UserConnection>> GetPostUserConnection(int Postid);
        public Task<bool> newConnection(User user, string ConnectionId, string Agent);
        public Task<bool> delete(string ConnectionId);
    }
}
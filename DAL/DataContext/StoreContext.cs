using DAL.Entities;
using DAL.Entities.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace DAL.DataContext
{
    public class StoreContext : IdentityDbContext<User>
    {
        public StoreContext(DbContextOptions options) : base(options) { }

        #region Dbset for tables
        public DbSet<Doctor> Doctors { get; set; }
        public DbSet<Nurse> Nurses { get; set; }
        public DbSet<Hospital> Hospitals { get; set; }
        public DbSet<Secretary> Secretaries { get; set; }
        public DbSet<AccountComment> AccountComments { get; set; }
        public DbSet<Bed> Beds { get; set; }
        public DbSet<Comment> Comments { get; set; }
        public DbSet<CommentLike> CommentLikes { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<DocumentsDoctor> DocumentsDoctors { get; set; }
        public DbSet<DocumentsNurse> DocumentsNurses { get; set; }
        public DbSet<DocumentsHospital> DocumentsHospitals { get; set; }
        public DbSet<Follow> Follows { get; set; }
        public DbSet<Like> Likes { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<PostComment> PostComments { get; set; }
        public DbSet<PostLike> PostLikes { get; set; }
        public DbSet<PostTag> PostTags { get; set; }
        public DbSet<ReserveDoctor> ReserveDoctors { get; set; }
        public DbSet<ReserveNurse> ReserveNurses { get; set; }
        public DbSet<ReserveHospital> ReserveHospitals { get; set; }
        public DbSet<SubComment> SubComments { get; set; }
        public DbSet<SubCommentLike> SubCommentLikes { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<UserTag> UserTags { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<UserRole> UserRoles { get; set; }


        #endregion

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);
            builder.Entity<User>().HasIndex(i=>i.UserName).IsUnique(); 
            builder.Entity<UserTag>().HasIndex(i=>new {i.UserId,i.TagId}).IsUnique();
            builder.Entity<Tag>().HasIndex(i=>new {i.Tagname}).IsUnique(); 
            builder.Entity<CommentLike>().HasIndex(i=>new {i.UserId,i.CommentID}).IsUnique(); 
            builder.Entity<Follow>().HasIndex(i=>new {i.UserId,i.FollowedUserId}).IsUnique(); 
            builder.Entity<PostLike>().HasIndex(i=>new {i.UserId,i.PostID}).IsUnique(); 
            builder.Entity<PostTag>().HasIndex(i=>new {i.PostId,i.TagId}).IsUnique(); 
            builder.Entity<SubCommentLike>().HasIndex(i=>new {i.SubCommentID,i.UserId}).IsUnique();
        }
 
    }

}
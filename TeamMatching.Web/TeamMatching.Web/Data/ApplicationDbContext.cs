using Microsoft.EntityFrameworkCore;
using TeamMatching.Shared.Entities;

namespace TeamMatching.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        // 엔티티들을 DbSet으로 등록 (Spring의 Repository 관리 대상과 유사)
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        public DbSet<Tag> Tags { get; set; }
        public DbSet<Application> Applications { get; set; }
        public DbSet<Team> Teams { get; set; }
        public DbSet<TeamMember> TeamMembers { get; set; }
        public DbSet<Review> Reviews { get; set; }
        public DbSet<UserTag> UserTags { get; set; }
        public DbSet<PostTag> PostTags { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // 1. UserTag 다대다 관계 설정 (UserId + TagId 복합키)
            modelBuilder.Entity<UserTag>()
                .HasKey(ut => new { ut.UserId, ut.TagId });

            modelBuilder.Entity<UserTag>()
                .HasOne(ut => ut.User)
                .WithMany(u => u.UserTags)
                .HasForeignKey(ut => ut.UserId);

            modelBuilder.Entity<UserTag>()
                .HasOne(ut => ut.Tag)
                .WithMany(t => t.UserTags)
                .HasForeignKey(ut => ut.TagId);

            // 2. PostTag 다대다 관계 설정 (PostId + TagId 복합키)
            modelBuilder.Entity<PostTag>()
                .HasKey(pt => new { pt.PostId, pt.TagId });

            modelBuilder.Entity<PostTag>()
                .HasOne(pt => pt.Post)
                .WithMany(p => p.PostTags)
                .HasForeignKey(pt => pt.PostId);

            modelBuilder.Entity<PostTag>()
                .HasOne(pt => pt.Tag)
                .WithMany(t => t.PostTags)
                .HasForeignKey(pt => pt.TagId);

            // 3. Review 유니크 제약 조건 (한 프로젝트 내 동일인 중복 평가 방지)
            modelBuilder.Entity<Review>()
                .HasIndex(r => new { r.PostId, r.ReviewerId, r.RevieweeId })
                .IsUnique();

            // 4. Review 관련 외래키 설정 (복합 관계이므로 Delete 수동 제어)
            modelBuilder.Entity<Review>()
                .HasOne(r => r.Reviewer)
                .WithMany()
                .HasForeignKey(r => r.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Reviewee)
                .WithMany()
                .HasForeignKey(r => r.RevieweeId)
                .OnDelete(DeleteBehavior.Restrict);

            // 5. TeamMember 관계 설정
            modelBuilder.Entity<TeamMember>()
                .HasOne(tm => tm.User)
                .WithMany()
                .HasForeignKey(tm => tm.UserId)
                .OnDelete(DeleteBehavior.Restrict);

            // 6. User 이메일 유니크 설정
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // 7. 초기 기술 태그 데이터 (Seeding)
            modelBuilder.Entity<Tag>().HasData(
                new Tag { Id = 1, Name = "C#" },
                new Tag { Id = 2, Name = ".NET" },
                new Tag { Id = 3, Name = "Java" },
                new Tag { Id = 4, Name = "Spring" },
                new Tag { Id = 5, Name = "Python" },
                new Tag { Id = 6, Name = "Node.js" },
                new Tag { Id = 7, Name = "React" },
                new Tag { Id = 8, Name = "Vue" },
                new Tag { Id = 9, Name = "MySQL" },
                new Tag { Id = 10, Name = "UI/UX" }
            );
        }
    }
}

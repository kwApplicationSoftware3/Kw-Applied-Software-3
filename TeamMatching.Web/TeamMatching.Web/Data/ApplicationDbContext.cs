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
        public DbSet<TeamPost> TeamPosts { get; set; }
        public DbSet<TeamPostComment> TeamPostComments { get; set; }
        public DbSet<TeamAvailability> TeamAvailabilities { get; set; }

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
                .WithMany(u => u.WrittenReviews) // 빈 괄호 안에 프로퍼티 명시
                .HasForeignKey(r => r.ReviewerId)
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Review>()
                .HasOne(r => r.Reviewee)
                .WithMany(u => u.ReceivedReviews) // 빈 괄호 안에 프로퍼티 명시
                .HasForeignKey(r => r.RevieweeId)
                .OnDelete(DeleteBehavior.Restrict);

            // 5. TeamMember 관계 설정
            modelBuilder.Entity<TeamMember>()
            .HasOne(tm => tm.User)
            .WithMany(u => u.TeamMemberships) // 명시적 연결 추가
            .HasForeignKey(tm => tm.UserId)
            .OnDelete(DeleteBehavior.Restrict);

            // 6. User 이메일 유니크 설정
            modelBuilder.Entity<User>()
                .HasIndex(u => u.Email)
                .IsUnique();

            // 7. 초기 기술 태그 데이터 (Seeding)
            modelBuilder.Entity<Tag>().HasData(
                // [프로그래밍 언어]
                new Tag { Id = 1, Name = "C" },
                new Tag { Id = 2, Name = "C++" },
                new Tag { Id = 3, Name = "C#" },
                new Tag { Id = 4, Name = "Java" },
                new Tag { Id = 5, Name = "Python" },
                new Tag { Id = 6, Name = "JavaScript" },
                new Tag { Id = 7, Name = "TypeScript" },
                new Tag { Id = 8, Name = "Go" },
                new Tag { Id = 9, Name = "Rust" },
                new Tag { Id = 10, Name = "Kotlin" },
                new Tag { Id = 11, Name = "Swift" },
                new Tag { Id = 12, Name = "Ruby" },
                new Tag { Id = 13, Name = "PHP" },
                new Tag { Id = 14, Name = "SQL" },
                new Tag { Id = 15, Name = "R" },
                new Tag { Id = 16, Name = "Dart" },
                new Tag { Id = 17, Name = "Scala" },

                // [웹/앱 프레임워크 & 라이브러리]
                new Tag { Id = 18, Name = ".NET" },
                new Tag { Id = 19, Name = "Spring" },
                new Tag { Id = 20, Name = "Spring Boot" },
                new Tag { Id = 21, Name = "Node.js" },
                new Tag { Id = 22, Name = "Express" },
                new Tag { Id = 23, Name = "Django" },
                new Tag { Id = 24, Name = "Flask" },
                new Tag { Id = 25, Name = "React" },
                new Tag { Id = 26, Name = "Vue" },
                new Tag { Id = 27, Name = "Angular" },
                new Tag { Id = 28, Name = "Svelte" },
                new Tag { Id = 29, Name = "Next.js" },
                new Tag { Id = 30, Name = "Nuxt.js" },
                new Tag { Id = 31, Name = "Flutter" },
                new Tag { Id = 32, Name = "React Native" },

                // [데이터베이스 & 인프라]
                new Tag { Id = 33, Name = "MySQL" },
                new Tag { Id = 34, Name = "PostgreSQL" },
                new Tag { Id = 35, Name = "Oracle" },
                new Tag { Id = 36, Name = "MongoDB" },
                new Tag { Id = 37, Name = "Redis" },
                new Tag { Id = 38, Name = "AWS" },
                new Tag { Id = 39, Name = "GCP" },
                new Tag { Id = 40, Name = "Firebase" },
                new Tag { Id = 41, Name = "Docker" },
                new Tag { Id = 42, Name = "Kubernetes" },
                new Tag { Id = 43, Name = "DevOps" },

                // [직무 / 분야]
                new Tag { Id = 44, Name = "프론트엔드" },
                new Tag { Id = 45, Name = "백엔드" },
                new Tag { Id = 46, Name = "모바일" },
                new Tag { Id = 47, Name = "기획" },
                new Tag { Id = 48, Name = "UI/UX" },
                new Tag { Id = 49, Name = "데이터분석" },
                new Tag { Id = 50, Name = "인공지능/AI" },
                new Tag { Id = 51, Name = "머신러닝" },
                new Tag { Id = 52, Name = "블록체인" },
                new Tag { Id = 53, Name = "보안/해킹" },
                new Tag { Id = 54, Name = "게임개발" },
                new Tag { Id = 55, Name = "알고리즘" },
                
                // [게임 엔진]
                new Tag { Id = 56, Name = "Unity" },
                new Tag { Id = 57, Name = "Unreal" },

                // [스포츠 / 액티비티]
                new Tag { Id = 58, Name = "축구" },
                new Tag { Id = 59, Name = "농구" },
                new Tag { Id = 60, Name = "야구" },
                new Tag { Id = 61, Name = "배드민턴" },
                new Tag { Id = 62, Name = "테니스" },
                new Tag { Id = 63, Name = "탁구" },
                new Tag { Id = 64, Name = "볼링" },
                new Tag { Id = 65, Name = "골프" },
                new Tag { Id = 66, Name = "수영" },
                new Tag { Id = 67, Name = "자전거/라이딩" },
                new Tag { Id = 68, Name = "헬스/피트니스" },
                new Tag { Id = 69, Name = "크로스핏" },
                new Tag { Id = 70, Name = "요가/필라테스" },
                new Tag { Id = 71, Name = "러닝/마라톤" },
                new Tag { Id = 72, Name = "등산" },
                new Tag { Id = 73, Name = "캠핑" },
                new Tag { Id = 74, Name = "클라이밍" },
                new Tag { Id = 75, Name = "e스포츠" },

                // [음악 / 밴드]
                new Tag { Id = 76, Name = "밴드" },
                new Tag { Id = 77, Name = "보컬" },
                new Tag { Id = 78, Name = "건반" },
                new Tag { Id = 79, Name = "기타(악기)" },
                new Tag { Id = 80, Name = "베이스" },
                new Tag { Id = 81, Name = "드럼" },
                new Tag { Id = 82, Name = "작곡/미디" },
                new Tag { Id = 83, Name = "클래식" },

                // [문화 / 예술 / 취미]
                new Tag { Id = 84, Name = "독서" },
                new Tag { Id = 85, Name = "어학" },
                new Tag { Id = 86, Name = "사진/출사" },
                new Tag { Id = 87, Name = "영상제작" },
                new Tag { Id = 88, Name = "맛집탐방" },
                new Tag { Id = 89, Name = "보드게임" },
                new Tag { Id = 90, Name = "방탈출" },
                new Tag { Id = 91, Name = "영화/드라마" },
                new Tag { Id = 92, Name = "전시/공연" },
                new Tag { Id = 93, Name = "댄스" },
                new Tag { Id = 94, Name = "드로잉/그림" },

                // [스터디 / 취업]
                new Tag { Id = 95, Name = "자격증" },
                new Tag { Id = 96, Name = "취업/면접" },
                new Tag { Id = 97, Name = "토익/토플" },
                new Tag { Id = 98, Name = "고시/공무원" },
                new Tag { Id = 99, Name = "창업/스타트업" }
            );

            // TeamPost와 TeamPostComment의 일대다 관계 명시
            modelBuilder.Entity<TeamPostComment>()
                .HasOne(tc => tc.TeamPost)
                .WithMany(tp => tp.TeamPostComments)
                .HasForeignKey(tc => tc.TeamPostId)
                .OnDelete(DeleteBehavior.Cascade); // 게시글 삭제 시 댓글도 삭제

            // Team과 TeamPost의 일대다 관계 명시
            modelBuilder.Entity<TeamPost>()
                .HasOne(tp => tp.Team)
                .WithMany(t => t.TeamPosts)
                .HasForeignKey(tp => tp.TeamId)
                .OnDelete(DeleteBehavior.Cascade); // 팀 삭제 시 팀 게시글도 삭제
        }
    }
}

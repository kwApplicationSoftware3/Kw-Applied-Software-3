using Microsoft.EntityFrameworkCore;

namespace TeamMatching.Web.Data
{
    public class ApplicationDbContext : DbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            
            // 향후 ERD 설계에 따라 엔티티 간의 관계(1:N, N:M), 
            // 제약 조건(MaxLength, Required), 복합 키 등을 
            // Fluent API 방식으로 이곳에 정의합니다.
        }
    }
}

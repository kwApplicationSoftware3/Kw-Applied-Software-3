using Microsoft.EntityFrameworkCore;
using TeamMatching.Shared.DTOs;
using TeamMatching.Shared.Entities;
using TeamMatching.Web.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace TeamMatching.Web.Services
{
    /// <summary>
    /// 인증 관련 비즈니스 로직 구현체
    /// </summary>
    public class PostService : IPostService
    {
        private readonly ApplicationDbContext _context;

        public PostService(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<CreatePostResponse> CreatePostAsync(CreatePostRequest request, int authorId)
        {
            try
            {
                if (authorId <= 0)
                {
                    return new CreatePostResponse { IsSuccess = false, Message = "작성자 정보가 유효하지 않습니다." };
                }
                
                // 1. 작성자 존재 확인
                var author = await _context.Users.FindAsync(authorId);
                if (author == null)
                {
                    return new CreatePostResponse { IsSuccess = false, Message = "작성자 정보를 찾을 수 없습니다." };
                }

                // 2. 게시글 엔티티 생성
                var post = new Post
                {
                    AuthorId = authorId, 
                    Title = request.Title!,
                    Content = request.Content!,
                    Category = request.Category,
                    MaxMembers = request.MaxMembers,
                };

                // 3. 선택한 태그(기술 스택) 매핑
                if (request.SelectedTagIds != null && request.SelectedTagIds.Any())
                {
                    foreach (var tagId in request.SelectedTagIds)
                    {
                        post.PostTags.Add(new PostTag { TagId = tagId });
                    }
                }

                _context.Posts.Add(post);
                await _context.SaveChangesAsync();

                return new CreatePostResponse { IsSuccess = true, Message = "게시글이 생성되었습니다." };
            }
            catch (Exception ex)
            {
                return new CreatePostResponse { IsSuccess = false, Message = $"게시글 생성 중 오류가 발생했습니다: {ex.Message}" };
            }
        }
    }
}


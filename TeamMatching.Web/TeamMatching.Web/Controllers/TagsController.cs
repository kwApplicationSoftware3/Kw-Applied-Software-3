using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TeamMatching.Shared.Entities;
using TeamMatching.Web.Data;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace TeamMatching.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")] // api/tags
    public class TagsController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public TagsController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Tag>>> GetTags()
        {
            return await _context.Tags.ToListAsync();
        }
    }
}

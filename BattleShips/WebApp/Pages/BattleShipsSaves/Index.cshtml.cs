using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Domain;

namespace WebApp.Pages_BattleShipsSaves
{
    public class IndexModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public IndexModel(DAL.AppDbContext context)
        {
            _context = context;
        }
        public IList<BattleShipsSave> BattleShipsSave { get; set; } = default!;

        public async Task OnGetAsync()
        {
            BattleShipsSave = await _context.BattleShipsSaves.ToListAsync();
        }
    }
}

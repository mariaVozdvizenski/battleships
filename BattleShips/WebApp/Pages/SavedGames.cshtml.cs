using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages
{
    public class SavedGames : PageModel
    {
        private AppDbContext _context;
        public ICollection<BattleShipsSave> BattleShipsSaves { get; set; } = default!;
        
        public SavedGames(AppDbContext context)
        {
            _context = context;
        }

        public void OnGet()
        {
            BattleShipsSaves = _context.BattleShipsSaves.ToList();
        }
    }
}
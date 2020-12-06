using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Domain;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages
{
    public class SavedGames : PageModel
    {
        private readonly AppDbContext _context;
        public ICollection<BattleShipsSave> BattleShipsSaves { get; set; } = default!;
        
        public SavedGames(AppDbContext context)
        {
            _context = context;
        }

        public IActionResult OnGet()
        {
            BattleShipsSaves = _context.BattleShipsSaves.OrderByDescending(e => e.Id).ToList();
            return Page();
        }

        public string CreateDisplayName(BattleShipsSave save)
        {
            return $"{save.SaveName} - {save.Height}x{save.Width} - {(save.Player1Turn ? "Player 1" : "Player 2")} " +
                   $"- {(save.GameType == GameType.HumanVsAi ? "Vs Computer" : "Vs Human")}";
        }

        public async Task<IActionResult> OnPostDeleteGameAsync(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var battleShipsSave = await _context.BattleShipsSaves.FindAsync(id);

            if (battleShipsSave != null)
            {
                _context.BattleShipsSaves.Remove(battleShipsSave);
                await _context.SaveChangesAsync();
            }

            return RedirectToPage();
        }
    }
}
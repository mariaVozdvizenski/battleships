using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Domain;
using GameEngine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace WebApp.Pages
{
    public class NewGameHumanVsAi : PageModel
    {
        private readonly AppDbContext _context;
        
        [BindProperty]
        public Player Player1 { get; set; } = new Player();
        
        [BindProperty]
        public Player Player2 { get; set; } = new Player();

        [Range(10, 20)] [BindProperty] public int Height { get; set; } = 10;

        [Range(10, 20)] [BindProperty] public int Width { get; set; } = 10;

        [MaxLength(50)]
        [MinLength(1)]
        [BindProperty]
        public string GameName { get; set; } = default!;

        [BindProperty]
        [Display(Name = "Ships can touch")]
        public bool ShipsCanTouch { get; set; }
 
        public NewGameHumanVsAi(AppDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            BattleShips battleShips = new BattleShips(Height, Width, Player1, Player2) 
                {ShipsCanTouch = ShipsCanTouch};
            
            battleShips.AddDefaultShipsToPlayerShipList(Player1);
            battleShips.AddDefaultShipsToPlayerShipList(Player2);
            
            battleShips.PlaceShipsAutomatically(Player1);
            battleShips.PlaceShipsAutomatically(Player2);

            battleShips.GameType = GameType.HumanVsAi;

            var save = battleShips.CreateBattleShipsSave(GameName);
            
            if (_context.BattleShipsSaves.Any(e => e.SaveName == GameName))
            {
                ModelState.AddModelError("GameName", "A save game with this name already exists.");
                return Page();
            }
            
            await _context.BattleShipsSaves.AddAsync(save);
            await _context.SaveChangesAsync();
            
            return RedirectToPage("/PlayGame", new {id = save.Id});
        }
    }
}
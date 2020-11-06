using System.Collections.Generic;
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
    public class NewGameHumanCustom : PageModel
    {
        private readonly AppDbContext _context;
        
        [BindProperty]
        public Player Player1 { get; set; } = new Player();
        
        [BindProperty]
        public Player Player2 { get; set; } = new Player();

        [Range(10, 20)]
        [BindProperty]
        public int Height { get; set; }
        
        [Range(10, 20)]
        [BindProperty]
        public int Width { get; set; }
        
        [MaxLength(50)]
        [MinLength(1)]
        [BindProperty]
        public string GameName { get; set; } = default!;

        [BindProperty] 
        public List<Ship> Player1Ships { get; set; } = default!;

        [BindProperty] 
        public List<Ship> Player2Ships { get; set; } = default!;

        [BindProperty]
        [Display(Name = "Ships can touch")]
        public bool ShipsCanTouch { get; set; }
        
        public NewGameHumanCustom(AppDbContext context)
        {
            _context = context;
        }
        
        public void OnGet()
        {
            
        }
        
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }
            
            BattleShips battleShips = new BattleShips(Height, Width, Player1, Player2);

            foreach (var player1Ship in Player1Ships)
            {
                battleShips.AddShipToPlayerShipList(Player1, player1Ship);
            }

            foreach (var player2Ship in Player2Ships)
            {
                battleShips.AddShipToPlayerShipList(Player2, player2Ship);
            }

            battleShips.ShipsCanTouch = ShipsCanTouch;
            battleShips.PlaceShipsAutomatically(Player1);
            battleShips.PlaceShipsAutomatically(Player2);
            
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
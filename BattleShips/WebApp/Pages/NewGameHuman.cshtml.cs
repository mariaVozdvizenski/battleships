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
    public class NewGameHuman : PageModel
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
        
        [MaxLength(100)]
        [MinLength(1)]
        [BindProperty]
        public string GameName { get; set; } = default!;

        public ICollection<Ship> Player1CustomShips { get; set; } = new List<Ship>();
        public ICollection<Ship> Player2CustomShips { get; set; } = new List<Ship>();

        public NewGameHuman(AppDbContext context)
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
            battleShips.AddDefaultShipsToPlayerShipList(Player1);
            battleShips.AddDefaultShipsToPlayerShipList(Player2);
            var save = battleShips.CreateBattleShipsSave(GameName);
            
            if (_context.BattleShipsSaves.Any(e => e.SaveName == GameName))
            {
                ModelState.AddModelError("GameName", "A save game with this name already exists.");
                return Page();
            }
            
            await _context.BattleShipsSaves.AddAsync(save);
            await _context.SaveChangesAsync();
            return Page();
        }
    }
}
using System;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DAL;
using Domain;
using GameEngine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;

namespace WebApp.Pages
{
    public class PlayGame : PageModel
    {
        private readonly AppDbContext _appDbContext;

        public int Id { get; set; }
        public BattleShips BattleShips { get; set; } = default!;
        
        [BindProperty]
        [Required]
        public string CoordinateString { get; set; } = default!;
        public bool HideBoards { get; set; } = false;
        
        public char[] Alphabet { get; set; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        
        public PlayGame(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task OnGetAsync(int id)
        {
            var battleShipsSave = await _appDbContext.BattleShipsSaves.FirstOrDefaultAsync(e => e.Id == id);
            Id = id;
            BattleShips = new BattleShips(battleShipsSave!.Height, battleShipsSave.Width,
                battleShipsSave.Player1JsonString,
                battleShipsSave.Player2JsonString) 
                {Player1Turn = battleShipsSave.Player1Turn};
            HideBoards = true;
            BattleShips.CheckIfGameHasFinished();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage("PlayGame", new {id});
            }
            var (row, col) = await ParseCoordinatesAsync();
            
            var battleShipsSave = await _appDbContext.BattleShipsSaves.FirstOrDefaultAsync(e => e.Id == id);
            
            BattleShips = new BattleShips(battleShipsSave!.Height, battleShipsSave.Width,
                    battleShipsSave.Player1JsonString,
                    battleShipsSave.Player2JsonString) 
                {Player1Turn = battleShipsSave.Player1Turn};
            
            BattleShips.FireAShot(BattleShips.Player1Turn ? BattleShips.Player1 : BattleShips.Player2, col, row);
            BattleShips.Player1Turn = !BattleShips.Player1Turn;
            BattleShips.UpdateBattleShipsSave(battleShipsSave);
            
            _appDbContext.BattleShipsSaves.Update(battleShipsSave);
            await _appDbContext.SaveChangesAsync();
            return RedirectToPage("PlayGame", new {id});
        }

        public async Task OnGetShowBoardsAsync(int id)
        {
            var battleShipsSave = await _appDbContext.BattleShipsSaves.FirstOrDefaultAsync(e => e.Id == id);
            BattleShips = new BattleShips(battleShipsSave!.Height, battleShipsSave.Width,
                    battleShipsSave.Player1JsonString,
                    battleShipsSave.Player2JsonString)
                {Player1Turn = battleShipsSave.Player1Turn};
            Id = id;
            BattleShips.CheckIfGameHasFinished();
        }

        private async Task<(int row, int col)> ParseCoordinatesAsync()
        {
            var coords = CoordinateString.Split(",");
            return (short.Parse(coords[0]), short.Parse(coords[1]));
        }

        public string GetPanelState(Panel panel)
        {
            return panel.PanelState switch
            {
                PanelState.Empty => "",
                PanelState.Ship => "S",
                PanelState.Submarine => "U",
                PanelState.Destroyer => "D",
                PanelState.Cruiser => "C",
                PanelState.Carrier => "A",
                PanelState.BattleShip => "B",
                PanelState.Hit => "X",
                PanelState.Miss => "M",
                _ => throw new ArgumentOutOfRangeException()
            };
        }
    }
}
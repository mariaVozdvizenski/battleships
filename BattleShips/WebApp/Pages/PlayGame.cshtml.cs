using System;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using DAL;
using Domain;
using GameEngine;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;

namespace WebApp.Pages
{
    public class PlayGame : PageModel
    {
        private readonly AppDbContext _appDbContext;
        public int Id { get; set; }
        public BattleShips BattleShips { get; set; } = default!;
        public bool ContinueOnly { get; set; } 
        
        [BindProperty]
        [Required]
        public string CoordinateString { get; set; } = default!;
        public bool HideBoards { get; set; } = false;
        public char[] Alphabet { get; set; } = "ABCDEFGHIJKLMNOPQRSTUVWXYZ".ToCharArray();
        public bool ShotHitTheTarget { get; set; }
        
        public PlayGame(AppDbContext appDbContext)
        {
            _appDbContext = appDbContext;
        }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            var battleShipsSave = await _appDbContext.BattleShipsSaves.FirstOrDefaultAsync(e => e.Id == id);
            
            BattleShips = new BattleShips(battleShipsSave);
            BattleShips.CheckIfGameHasFinished();

            if (battleShipsSave.GameType == GameType.HumanVsAi && !battleShipsSave.Player1Turn && !BattleShips.GameFinished)
            {
                return await OnPostComputerMoveAsync(id);
            }
            
            Id = id;
            
            if (battleShipsSave.GameType == GameType.HumanVsHuman)
            {
                HideBoards = true;
            }
            
            return Page();
        }

        public async Task<IActionResult> OnPostAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage("PlayGame", new {id});
            }
            var (row, col) = await ParseCoordinatesAsync();
            
            var battleShipsSave = await _appDbContext.BattleShipsSaves.FirstOrDefaultAsync(e => e.Id == id);

            BattleShips = new BattleShips(battleShipsSave);
            
            ShotHitTheTarget = BattleShips.FireAShot(BattleShips.Player1Turn ? BattleShips.Player1 : BattleShips.Player2, col, row);
            
            BattleShips.Player1Turn = !BattleShips.Player1Turn;
            BattleShips.UpdateBattleShipsSave(battleShipsSave);
            
            _appDbContext.BattleShipsSaves.Update(battleShipsSave);
            await _appDbContext.SaveChangesAsync();
            
            return RedirectToPage("PlayGame", "ShowShotResult", new {id, shotHit = ShotHitTheTarget});
        }

        public async Task<IActionResult> OnPostComputerMoveAsync(int id)
        {
            if (!ModelState.IsValid)
            {
                return RedirectToPage("PlayGame", new {id});
            }
            var battleShipsSave = await _appDbContext.BattleShipsSaves.FirstOrDefaultAsync(e => e.Id == id);
            BattleShips = new BattleShips(battleShipsSave);
            
            ShotHitTheTarget = BattleShips.FireAiShot();
            
            BattleShips.Player1Turn = !BattleShips.Player1Turn;
            BattleShips.UpdateBattleShipsSave(battleShipsSave);
            
            _appDbContext.BattleShipsSaves.Update(battleShipsSave);
            await _appDbContext.SaveChangesAsync();
            
            return RedirectToPage("PlayGame", "ShowShotResult", new {id, shotHit = ShotHitTheTarget});
        }

        public async Task OnGetShowShotResultAsync(int id, bool shotHit)
        {
            var battleShipsSave = await _appDbContext.BattleShipsSaves.FirstOrDefaultAsync(e => e.Id == id);
            BattleShips = new BattleShips(battleShipsSave);
            Id = id;
            ShotHitTheTarget = shotHit;
            ContinueOnly = true;
        }

        public async Task OnGetShowBoardsAsync(int id)
        {
            var battleShipsSave = await _appDbContext.BattleShipsSaves.FirstOrDefaultAsync(e => e.Id == id);
            BattleShips = new BattleShips(battleShipsSave);
            Id = id;
            BattleShips.CheckIfGameHasFinished();
        }

        private async Task<(int row, int col)> ParseCoordinatesAsync()
        {
            var coords = CoordinateString.Split(",");
            return (short.Parse(coords[0]), short.Parse(coords[1]));
        }
        
    }
}
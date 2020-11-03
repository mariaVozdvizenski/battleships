using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using DAL;
using Domain;

namespace WebApp.Pages_BattleShipsSaves
{
    public class DetailsModel : PageModel
    {
        private readonly DAL.AppDbContext _context;

        public DetailsModel(DAL.AppDbContext context)
        {
            _context = context;
        }

        public BattleShipsSave BattleShipsSave { get; set; } = default!;

        public async Task<IActionResult> OnGetAsync(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            BattleShipsSave = await _context.BattleShipsSaves.FirstOrDefaultAsync(m => m.Id == id);

            if (BattleShipsSave == null)
            {
                return NotFound();
            }
            return Page();
        }
    }
}

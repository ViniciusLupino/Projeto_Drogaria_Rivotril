using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ProjetoDrogas.Data;
using ProjetoDrogas.Models;

namespace ProjetoDrogas.Controllers
{
    public static class Global
    {
        public static int Quantidade;
    }
    public class ItemVendasController : Controller
    {
        private readonly Context _context;

        public ItemVendasController(Context context)
        {
            _context = context;
        }

        // GET: ItemVendas
        [Authorize(Roles = "Administrador,Operador,Gerente")]

        public async Task<IActionResult> Index(string? id)
        {
            var context = _context.ItemVendas.Include(i => i.Produtos).Include(i => i.Vendas);
            if (id != null)
            {
                ViewData["idVenda"] = id;
            }

            return View(await context.ToListAsync());
        }
        [Authorize(Roles = "Administrador,Operador")]

        // GET: ItemVendas/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.ItemVendas == null)
            {
                return NotFound();
            }

            var itemVenda = await _context.ItemVendas
                .Include(i => i.Produtos)
                .Include(i => i.Vendas)
                .FirstOrDefaultAsync(m => m.ItemVendaId == id);
            if (itemVenda == null)
            {
                return NotFound();
            }

            return View(itemVenda);
        }
        [Authorize(Roles = "Administrador,Operador")]

        // GET: ItemVendas/Create
        public IActionResult Create(string? id)
        {
            ViewData["ProdutoId"] = new SelectList(_context.Produtos, "ProdutoId", "Nome");
            ViewData["VendaId"] = new SelectList(_context.Vendas, "VendaId", "Nota");
            ViewData["Produtos"] = new SelectList(_context.Produtos, "Preco", "Preco");


            if (id != null)
            {
                ViewData["idVenda"] = id;
            }

            return View();
        }
        [Authorize(Roles = "Administrador,Operador")]

        // POST: ItemVendas/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemVendaId,VendaId,ProdutoId,Quantidade,Preco")] ItemVenda itemVenda)
        {
            if (ModelState.IsValid)
            {
                itemVenda.ItemVendaId = Guid.NewGuid();
                _context.Add(itemVenda);
                await _context.SaveChangesAsync();

                var prod = _context.Produtos.FirstOrDefault(i => i.ProdutoId == itemVenda.ProdutoId);
                prod.Quantidade -= itemVenda.Quantidade;
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "ItemVendas", new { id = itemVenda.VendaId.ToString() });
            }
            ViewData["ProdutoId"] = new SelectList(_context.Produtos, "ProdutoId", "Nome", itemVenda.ProdutoId);
            ViewData["VendaId"] = new SelectList(_context.Vendas, "VendaId", "Nota", itemVenda.VendaId);
            return View(itemVenda);
        }
        [Authorize(Roles = "Administrador,Operador")]

        // GET: ItemVendas/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.ItemVendas == null)
            {
                return NotFound();
            }

            var itemVenda = await _context.ItemVendas.FindAsync(id);
            if (itemVenda == null)
            {
                return NotFound();
            }

            Global.Quantidade = itemVenda.Quantidade;

            ViewData["ProdutoId"] = new SelectList(_context.Produtos, "ProdutoId", "Nome", itemVenda.ProdutoId);
            ViewData["VendaId"] = new SelectList(_context.Vendas, "VendaId", "Nota", itemVenda.VendaId);
            return View(itemVenda);
        }
        [Authorize(Roles = "Administrador,Operador")]

        // POST: ItemVendas/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ItemVendaId,VendaId,ProdutoId,Quantidade,Preco")] ItemVenda itemVenda)
        {
            if (id != itemVenda.ItemVendaId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(itemVenda);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemVendaExists(itemVenda.ItemVendaId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }

                //var prod = _context.Produtos.FirstOrDefault(i => i.ProdutoId == itemVenda.ProdutoId);
                //var Vveio = prod.Quantidade;
                //var Vnovo = itemVenda.Quantidade;
                //prod.Quantidade += (Vveio - Vnovo);
                var prod = _context.Produtos.FirstOrDefault(i => i.ProdutoId == itemVenda.ProdutoId);

                prod.Quantidade += (Global.Quantidade - itemVenda.Quantidade);
                
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "ItemVendas", new { id = itemVenda.VendaId.ToString() });
            }
            ViewData["ProdutoId"] = new SelectList(_context.Produtos, "ProdutoId", "Nome", itemVenda.ProdutoId);
            ViewData["VendaId"] = new SelectList(_context.Vendas, "VendaId", "Nota", itemVenda.VendaId);
            return View(itemVenda);
        }
        [Authorize(Roles = "Administrador,Operador")]

        // GET: ItemVendas/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.ItemVendas == null)
            {
                return NotFound();
            }

            var itemVenda = await _context.ItemVendas
                .Include(i => i.Produtos)
                .Include(i => i.Vendas)
                .FirstOrDefaultAsync(m => m.ItemVendaId == id);
            if (itemVenda == null)
            {
                return NotFound();
            }

            return View(itemVenda);
        }
        [Authorize(Roles = "Administrador,Operador")]

        // POST: ItemVendas/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.ItemVendas == null)
            {
                return Problem("Entity set 'Context.ItemVendas'  is null.");
            }
            var itemVenda = await _context.ItemVendas.FindAsync(id);
            if (itemVenda != null)
            {
                _context.ItemVendas.Remove(itemVenda);
            }

            var prod = _context.Produtos.FirstOrDefault(i => i.ProdutoId == itemVenda.ProdutoId);
            prod.Quantidade += itemVenda.Quantidade;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "ItemVendas", new { id = itemVenda.VendaId.ToString() });
        }
        [Authorize(Roles = "Administrador,Operador")]

        private bool ItemVendaExists(Guid id)
        {
          return (_context.ItemVendas?.Any(e => e.ItemVendaId == id)).GetValueOrDefault();
        }

    
    }
}

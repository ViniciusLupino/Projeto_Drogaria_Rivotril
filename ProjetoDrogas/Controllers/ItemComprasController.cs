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
    public static class Global1
    {
        public static int Quantidade;
    }
    public class ItemComprasController : Controller
    {
        private readonly Context _context;

        public ItemComprasController(Context context)
        {
            _context = context;
        }

        // GET: ItemCompras
        [Authorize(Roles = "Administrador,Operador,Gerente")]
        public async Task<IActionResult> Index(string? id)
        {
            var context = _context.ItemCompras.Include(i => i.Produtos).Include(i => i.Compras);
            if (id != null)
            {
                ViewData["idCompra"] = id;
            }

            return View(await context.ToListAsync());
        }
        [Authorize(Roles = "Administrador,Operador")]

        // GET: ItemCompras/Details/5
        public async Task<IActionResult> Details(Guid? id)
        {
            if (id == null || _context.ItemCompras == null)
            {
                return NotFound();
            }

            var itemCompra = await _context.ItemCompras
                .Include(i => i.Compras)
                .Include(i => i.Produtos)
                .FirstOrDefaultAsync(m => m.ItemCompraId == id);
            if (itemCompra == null)
            {
                return NotFound();
            }

            return View(itemCompra);
        }
        [Authorize(Roles = "Administrador,Operador")]

        // GET: ItemCompras/Create
        public IActionResult Create(string? id)
        {
            ViewData["CompraId"] = new SelectList(_context.Compras, "CompraId", "Nota");
            ViewData["ProdutoId"] = new SelectList(_context.Produtos, "ProdutoId", "Nome");
            ViewData["Produtos"] = new SelectList(_context.Produtos, "Preco", "Preco");


            if (id != null)
            {
                ViewData["idCompra"] = id;
            }

            return View();
        }
        [Authorize(Roles = "Administrador,Operador")]

        // POST: ItemCompras/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ItemCompraId,CompraId,ProdutoId,Quantidade,Preco")] ItemCompra itemCompra)
        {
            if (ModelState.IsValid)
            {
                itemCompra.ItemCompraId = Guid.NewGuid();
                _context.Add(itemCompra);
                await _context.SaveChangesAsync();

                var prod = _context.Produtos.FirstOrDefault(i => i.ProdutoId == itemCompra.ProdutoId);
                prod.Quantidade += itemCompra.Quantidade;
                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "ItemCompras", new { id = itemCompra.CompraId.ToString() });
            }
            ViewData["CompraId"] = new SelectList(_context.Compras, "CompraId", "Nota", itemCompra.CompraId);
            ViewData["ProdutoId"] = new SelectList(_context.Produtos, "ProdutoId", "Nome", itemCompra.ProdutoId);
            return View(itemCompra);
        }
        [Authorize(Roles = "Administrador,Operador")]

        // GET: ItemCompras/Edit/5
        public async Task<IActionResult> Edit(Guid? id)
        {
            if (id == null || _context.ItemCompras == null)
            {
                return NotFound();
            }

            var itemCompra = await _context.ItemCompras.FindAsync(id);
            if (itemCompra == null)
            {
                return NotFound();
            }

            Global1.Quantidade = itemCompra.Quantidade;

            ViewData["CompraId"] = new SelectList(_context.Compras, "CompraId", "Nota", itemCompra.CompraId);
            ViewData["ProdutoId"] = new SelectList(_context.Produtos, "ProdutoId", "Nome", itemCompra.ProdutoId);
            return View(itemCompra);
        }
        [Authorize(Roles = "Administrador,Operador")]

        // POST: ItemCompras/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Guid id, [Bind("ItemCompraId,CompraId,ProdutoId,Quantidade,Preco")] ItemCompra itemCompra)
        {
            if (id != itemCompra.ItemCompraId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(itemCompra);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ItemCompraExists(itemCompra.ItemCompraId))
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
                var prod = _context.Produtos.FirstOrDefault(i => i.ProdutoId == itemCompra.ProdutoId);

                prod.Quantidade -= (Global1.Quantidade - itemCompra.Quantidade);

                await _context.SaveChangesAsync();

                return RedirectToAction("Index", "ItemCompras", new { id = itemCompra.CompraId.ToString() });
            }
            ViewData["CompraId"] = new SelectList(_context.Compras, "CompraId", "Nota", itemCompra.CompraId);
            ViewData["ProdutoId"] = new SelectList(_context.Produtos, "ProdutoId", "Nome", itemCompra.ProdutoId);
            return View(itemCompra);
        }
        [Authorize(Roles = "Administrador,Operador")]

        // GET: ItemCompras/Delete/5
        public async Task<IActionResult> Delete(Guid? id)
        {
            if (id == null || _context.ItemCompras == null)
            {
                return NotFound();
            }

            var itemCompra = await _context.ItemCompras
                .Include(i => i.Compras)
                .Include(i => i.Produtos)
                .FirstOrDefaultAsync(m => m.ItemCompraId == id);
            if (itemCompra == null)
            {
                return NotFound();
            }

            return View(itemCompra);
        }

        // POST: ItemCompras/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Administrador,Operador")]

        public async Task<IActionResult> DeleteConfirmed(Guid id)
        {
            if (_context.ItemCompras == null)
            {
                return Problem("Entity set 'Context.ItemCompras'  is null.");
            }
            var itemCompra = await _context.ItemCompras.FindAsync(id);
            if (itemCompra != null)
            {
                _context.ItemCompras.Remove(itemCompra);
            }

            var prod = _context.Produtos.FirstOrDefault(i => i.ProdutoId == itemCompra.ProdutoId);
            prod.Quantidade -= itemCompra.Quantidade;
            await _context.SaveChangesAsync();
            return RedirectToAction("Index", "ItemCompras", new { id = itemCompra.CompraId.ToString() });
        }
        [Authorize(Roles = "Administrador,Operador")]

        private bool ItemCompraExists(Guid id)
        {
          return (_context.ItemCompras?.Any(e => e.ItemCompraId == id)).GetValueOrDefault();
        }
    }
}

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ToDoApp.Models;
using ToDo_List.Data;
using Microsoft.AspNetCore.Identity;
using ToDo_List.Models;
using Microsoft.AspNetCore.Authorization;

namespace ToDo_List.Controllers
{
    [Authorize]
    public class TodoItemController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public TodoItemController(ApplicationDbContext context, UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _context = context;
            _userManager = userManager;
            _signInManager = signInManager;
        }

        // GET: TodoItem
        public async Task<IActionResult> Index()
        {
            var userId = _userManager.GetUserId(User); // Obtén el ID del usuario autenticado
            var todoItems = await _context.TodoItems
                                           .Where(t => t.UserId == userId) // Filtra por UserId
                                           .ToListAsync();
            return View(todoItems);
        }

        // GET: TodoItem/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todoItem = await _context.TodoItems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (todoItem == null)
            {
                return NotFound();
            }

            return View(todoItem);
        }

        // GET: TodoItem/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TodoItem/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,IsCompleted,CreatedAt,DueDate")] TodoItem todoItem)
        {
            if (ModelState.IsValid)
            {
                todoItem.UserId = _userManager.GetUserId(User); // Asigna el UserId del usuario autenticado
                todoItem.CreatedAt = DateTime.Now; // Asigna la fecha de creación
                _context.Add(todoItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(todoItem);
        }

        // GET: TodoItem/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem == null || todoItem.UserId != _userManager.GetUserId(User)) // Verifica que el usuario sea el dueño
            {
                return NotFound();
            }
            return View(todoItem);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,IsCompleted,CreatedAt,DueDate")] TodoItem todoItem)
        {
            if (id != todoItem.Id)
            {
                return NotFound();
            }

            // Verifica que el usuario sea el dueño
            if (todoItem.UserId != _userManager.GetUserId(User))
            {
                return Forbid(); // O puedes redirigir o manejarlo de otra manera
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(todoItem);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TodoItemExists(todoItem.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(todoItem);
        }

        // GET: TodoItem/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var todoItem = await _context.TodoItems
                .FirstOrDefaultAsync(m => m.Id == id);
            if (todoItem == null || todoItem.UserId != _userManager.GetUserId(User)) // Verifica que el usuario sea el dueño
            {
                return NotFound();
            }

            return View(todoItem);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var todoItem = await _context.TodoItems.FindAsync(id);
            if (todoItem != null)
            {
                // Verifica que el usuario sea el dueño
                if (todoItem.UserId == _userManager.GetUserId(User))
                {
                    _context.TodoItems.Remove(todoItem);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(Index));
        }

        private bool TodoItemExists(int id)
        {
            return _context.TodoItems.Any(e => e.Id == id);
        }
    }
}

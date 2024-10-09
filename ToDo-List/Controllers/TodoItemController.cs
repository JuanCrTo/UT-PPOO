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
using Newtonsoft.Json;

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
            // Obtén el ID y correo electrónico del usuario autenticado
            var userId = _userManager.GetUserId(User);
            var userEmail = (await _userManager.GetUserAsync(User))?.Email; // Obtén el correo electrónico del usuario
            Console.WriteLine($"UserId: {userId}, Email: {userEmail}"); // Para depuración

            // Asegúrate de que el usuario esté autenticado
            if (string.IsNullOrEmpty(userId))
            {
                ModelState.AddModelError("", "El usuario no está autenticado.");
                return View(todoItem);
            }

            // Asigna el UserId del usuario autenticado
            todoItem.UserId = userId;
            todoItem.CreatedAt = DateTime.Now; // Asigna la fecha de creación

            Console.WriteLine($"ModelState IsValid: {ModelState.IsValid}");
            foreach (var state in ModelState)
            {
                Console.WriteLine($"Key: {state.Key}, Errors: {string.Join(", ", state.Value.Errors.Select(e => e.ErrorMessage))}");
            }

            Console.WriteLine($"TodoItem: {JsonConvert.SerializeObject(todoItem)}");


            if (ModelState.IsValid)
            {
                _context.Add(todoItem);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // Imprimir los errores de ModelState en la consola para debug
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage); // Para Debug en consola
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
                Console.WriteLine("El ID en la URL no coincide con el ID del objeto TodoItem");
                return NotFound();
            }

            // Obtén el ID del usuario autenticado
            var userId = _userManager.GetUserId(User);
            Console.WriteLine($"UserId autenticado: {userId}");

            // Verifica que el usuario sea el dueño
            // Busca el TodoItem en la base de datos para asegurarte de que existe y obtener el UserId
            var existingTodoItem = await _context.TodoItems.AsNoTracking().FirstOrDefaultAsync(t => t.Id == id);
            if (existingTodoItem == null)
            {
                Console.WriteLine($"No se encontró el TodoItem con ID: {id}");
                return NotFound();
            }

            Console.WriteLine($"UserId del TodoItem existente: {existingTodoItem.UserId}");


            // Verifica que el usuario autenticado sea el dueño de la tarea
            if (existingTodoItem.UserId != userId)
            {
                Console.WriteLine("El usuario autenticado no es el dueño de esta tarea.");
                return Forbid(); // El usuario no tiene acceso a esta tarea
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Asegúrate de que estás actualizando el objeto correcto
                    todoItem.UserId = existingTodoItem.UserId; // Mantén el UserId original
                    _context.Update(todoItem);
                    await _context.SaveChangesAsync();
                    Console.WriteLine("TodoItem actualizado con éxito.");
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TodoItemExists(todoItem.Id))
                    {
                        Console.WriteLine("TodoItem no existe durante la actualización.");
                        return NotFound();
                    }
                    else
                    {
                        Console.WriteLine("Error de concurrencia durante la actualización.");
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            Console.WriteLine("ModelState no es válido. No se puede actualizar.");
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

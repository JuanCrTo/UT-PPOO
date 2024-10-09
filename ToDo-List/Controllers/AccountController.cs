using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ToDo_List.Models;

namespace ToDo_List.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;

        public AccountController(UserManager<ApplicationUser> userManager, SignInManager<ApplicationUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }


        // Método para mostrar la vista de registro
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Register()
        {
            Console.WriteLine("GET Register: Renderizando la vista de registro.");
            return View();
        }

        // Método para manejar el registro
        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Register(Register model)
        {
            Console.WriteLine("POST Register: Iniciando proceso de registro.");

            // Validar el modelo
            if (ModelState.IsValid)
            {
                Console.WriteLine($"Modelo válido. Email: {model.Email}, Nombre: {model.Name}");

                var user = new ApplicationUser
                {
                    UserName = model.Email,
                    Email = model.Email,
                    Name = model.Name
                };

                // Intentar crear el usuario
                var result = await _userManager.CreateAsync(user, model.Password);

                if (result.Succeeded)
                {
                    Console.WriteLine("Registro exitoso. Iniciando sesión del usuario.");
                    await _signInManager.SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                // Si falla, se añaden los errores al modelo
                Console.WriteLine("Registro fallido. Errores:");
                foreach (var error in result.Errors)
                {
                    Console.WriteLine($"Error: {error.Description}");
                    ModelState.AddModelError(string.Empty, error.Description);
                }
            }
            else
            {
                Console.WriteLine("Modelo no válido. Errores en el modelo:");
                // Imprimir errores de validación del modelo
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Error: {error.ErrorMessage}");
                }
            }

            Console.WriteLine("Volviendo a renderizar la vista de registro con errores.");
            return View(model);
        }

        // Métodos para mostrar la vista de login y manejar el login
        [AllowAnonymous]
        [HttpGet]
        public IActionResult Login()
        {
            Console.WriteLine("GET Login: Renderizando la vista de login.");
            return View();
        }

        [AllowAnonymous]
        [HttpPost]
        public async Task<IActionResult> Login(Login model)
        {
            Console.WriteLine("POST Login: Iniciando proceso de autenticación.");

            // Validar el modelo
            if (ModelState.IsValid)
            {
                Console.WriteLine($"Modelo válido. Email: {model.Email}");

                // Intentar iniciar sesión
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, isPersistent: false, lockoutOnFailure: false);

                if (result.Succeeded)
                {
                    Console.WriteLine("Inicio de sesión exitoso. Redirigiendo a la página de inicio.");
                    return RedirectToAction("Index", "Home");
                }

                Console.WriteLine("Intento de inicio de sesión fallido.");
                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            else
            {
                Console.WriteLine("Modelo no válido. Errores en el modelo:");

                // Imprimir errores del modelo
                foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
                {
                    Console.WriteLine($"Error: {error.ErrorMessage}");
                }
            }

            Console.WriteLine("Volviendo a renderizar la vista de login con errores.");
            return View(model);
        }


        // Método para manejar el logout
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }

}

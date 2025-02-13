using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using SimpleBlog.Models;
using SimpleBlog.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.SignalR;
using SimpleBlog.Hubs;
using SimpleBlog.Services;

namespace SimpleBlog.Controllers
{
    public class PostController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<User> _userManager;
        private readonly INotificationService _notificationService;

        public PostController(ApplicationDbContext context, UserManager<User> userManager, INotificationService notificationService)
        {
            _context = context;
            _userManager = userManager;
            _notificationService = notificationService;
        }

        // Criar postagem
        public IActionResult Create()
        {
            if (!User.Identity.IsAuthenticated)
            {
                return RedirectToAction("Login", "Account"); // Redirecionar para a página de login se o usuário não estiver autenticado
            }

            // Obter o UserId do usuário autenticado
            var userId = _userManager.GetUserId(User);

            var post = new Post()
            {
                UserId = userId
            };
            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> Create(Post model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                if (user == null) return Unauthorized();
                model.UserId = user.Id;
                model.CreatedAt = DateTime.UtcNow;
                _context.Posts.Add(model);
                await _context.SaveChangesAsync();

                // Enviar notificação via SignalR
                _notificationService.SendPostCreatedNotification(model);

                return RedirectToAction("Index", "Post");
            }
            return View(model);
        }

        // Listar postagens
        public IActionResult Index()
        {
            var posts = _context.Posts.Include(p => p.User).ToList();  // Inclui o autor da postagem
            return View(posts);
        }

        // Editar postagem
        public async Task<IActionResult> Edit(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound();

            // Verifica se o usuário é o autor da postagem
            var user = await _userManager.GetUserAsync(User);
            if (user == null || post.UserId != user.Id) return Unauthorized();  // Não autorizado se não for o autor
            var userId = _userManager.GetUserId(User);
            post.UserId = userId;

            return View(post);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(int id, Post model)
        {
            if (id != model.Id) return NotFound();  // Verifica se o ID da postagem corresponde

            // Verifica se o usuário é o autor da postagem
            var user = await _userManager.GetUserAsync(User);
            if (user == null || model.UserId != user.Id) return Unauthorized();  // Não autorizado se não for o autor

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(model);  // Atualiza os dados da postagem
                    await _context.SaveChangesAsync();  // Salva as alterações no banco
                    return RedirectToAction(nameof(Index));  // Redireciona para a lista de postagens
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Posts.Any(p => p.Id == id))  // Verifica se a postagem ainda existe
                    {
                        return NotFound();
                    }
                    throw;
                }
            }
            return View(model);  // Caso haja erro de validação, retorna a view com os erros
        }

        // Excluir postagem
        public async Task<IActionResult> Delete(int id)
        {
            var post = await _context.Posts.FindAsync(id);
            if (post == null) return NotFound();

            // Verifica se o usuário é o autor da postagem
            var user = await _userManager.GetUserAsync(User);
            if (user == null || post.UserId != user.Id) return Unauthorized();  // Não autorizado se não for o autor

            _context.Posts.Remove(post);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
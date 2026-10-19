using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using TrilhaApiDesafio.Context;
using TrilhaApiDesafio.Models;

namespace TrilhaApiDesafio.Controllers
{
    public class TarefasMvcController : Controller
    {
        private readonly OrganizadorContext _context;

        public TarefasMvcController(OrganizadorContext context)
        {
            _context = context;
        }

        // GET: / ou /TarefasMvc
        public IActionResult Index(string searchTitulo, DateTime? searchData, EnumStatusTarefa? searchStatus)
        {
            var query = _context.Tarefas.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTitulo))
            {
                query = query.Where(t => t.Titulo.Contains(searchTitulo));
            }

            if (searchData.HasValue)
            {
                query = query.Where(t => t.Data.Date == searchData.Value.Date);
            }

            if (searchStatus.HasValue)
            {
                query = query.Where(t => t.Status == searchStatus.Value);
            }

            ViewBag.SearchTitulo = searchTitulo;
            ViewBag.SearchData = searchData?.ToString("yyyy-MM-dd");
            ViewBag.SearchStatus = searchStatus;

            var lista = query.OrderByDescending(t => t.Data).ToList();
            return View(lista);
        }

        // GET: /TarefasMvc/Detalhes/5
        public IActionResult Detalhes(int id)
        {
            var tarefa = _context.Tarefas.Find(id);
            if (tarefa == null)
            {
                return NotFound();
            }

            return View(tarefa);
        }

        // GET: /TarefasMvc/Criar
        public IActionResult Criar()
        {
            var tarefa = new Tarefa
            {
                Data = DateTime.Now,
                Status = EnumStatusTarefa.Pendente
            };
            return View(tarefa);
        }

        // POST: /TarefasMvc/Criar
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Criar(Tarefa tarefa)
        {
            if (string.IsNullOrWhiteSpace(tarefa.Titulo))
            {
                ModelState.AddModelError("Titulo", "O título é obrigatório.");
            }

            if (tarefa.Data == DateTime.MinValue)
            {
                ModelState.AddModelError("Data", "A data da tarefa não pode ser vazia.");
            }

            if (!ModelState.IsValid)
            {
                return View(tarefa);
            }

            _context.Tarefas.Add(tarefa);
            _context.SaveChanges();

            TempData["MensagemSucesso"] = "Tarefa cadastrada com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /TarefasMvc/Editar/5
        public IActionResult Editar(int id)
        {
            var tarefa = _context.Tarefas.Find(id);
            if (tarefa == null)
            {
                return NotFound();
            }

            return View(tarefa);
        }

        // POST: /TarefasMvc/Editar/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Editar(int id, Tarefa tarefa)
        {
            if (id != tarefa.Id)
            {
                return BadRequest();
            }

            var tarefaBanco = _context.Tarefas.Find(id);
            if (tarefaBanco == null)
            {
                return NotFound();
            }

            if (string.IsNullOrWhiteSpace(tarefa.Titulo))
            {
                ModelState.AddModelError("Titulo", "O título é obrigatório.");
            }

            if (tarefa.Data == DateTime.MinValue)
            {
                ModelState.AddModelError("Data", "A data da tarefa não pode ser vazia.");
            }

            if (!ModelState.IsValid)
            {
                return View(tarefa);
            }

            tarefaBanco.Titulo = tarefa.Titulo;
            tarefaBanco.Descricao = tarefa.Descricao;
            tarefaBanco.Data = tarefa.Data;
            tarefaBanco.Status = tarefa.Status;

            _context.Tarefas.Update(tarefaBanco);
            _context.SaveChanges();

            TempData["MensagemSucesso"] = "Tarefa atualizada com sucesso!";
            return RedirectToAction(nameof(Index));
        }

        // GET: /TarefasMvc/Deletar/5
        public IActionResult Deletar(int id)
        {
            var tarefa = _context.Tarefas.Find(id);
            if (tarefa == null)
            {
                return NotFound();
            }

            return View(tarefa);
        }

        // POST: /TarefasMvc/Deletar/5
        [HttpPost, ActionName("Deletar")]
        [ValidateAntiForgeryToken]
        public IActionResult DeletarConfirmado(int id)
        {
            var tarefaBanco = _context.Tarefas.Find(id);
            if (tarefaBanco != null)
            {
                _context.Tarefas.Remove(tarefaBanco);
                _context.SaveChanges();
                TempData["MensagemSucesso"] = "Tarefa excluída com sucesso!";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}

using Microsoft.AspNetCore.Mvc;
using TrilhaApiDesafio.Context;
using TrilhaApiDesafio.Models;

namespace TrilhaApiDesafio.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class TarefaController : ControllerBase
    {
        private readonly OrganizadorContext _context;

        public TarefaController(OrganizadorContext context)
        {
            _context = context;
        }

        [HttpGet("{id}")]
        public IActionResult ObterPorId(int id)
        {
            var tarefa = _context.Tarefas.Where(x => x.Id == id);

            if (tarefa.Any())
            {
                return Ok(tarefa);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("ObterTodos")]
        public IActionResult ObterTodos()
        {
            return Ok(_context.Tarefas);
        }

        [HttpGet("ObterPorTitulo")]
        public IActionResult ObterPorTitulo(string titulo)
        {
            var tarefas = _context.Tarefas.Where(x => x.Titulo == titulo);

            if (tarefas.Any())
            {
                return Ok(tarefas);
            }
            else
            {
                return NotFound();
            }
        }

        [HttpGet("ObterPorData")]
        public IActionResult ObterPorData(string dataStr)
        {
            if (string.IsNullOrEmpty(dataStr))
            {
                return BadRequest(new { Erro = "A data da tarefa não pode ser vazia" });
            }

            if (DateTime.TryParse(dataStr, out DateTime data))
            {
                var tarefa = _context.Tarefas.Where(x => x.Data.Date == data.Date).ToList();

                if (tarefa.Any())
                {
                    return Ok(tarefa);
                }
                else
                {
                    return NotFound();
                }
            }
            else
            {
                return BadRequest(new { Erro = "Formato de data inválido. Use o formato: yyyy-MM-dd" });
            }
        }

        [HttpGet("ObterPorStatus")]
        public IActionResult ObterPorStatus(EnumStatusTarefa status)
        {
            var tarefas = _context.Tarefas.Where(x => x.Status == status);

            if (tarefas == null)
                return NotFound();

            return Ok(tarefas);
        }

        [HttpPost]
        public IActionResult Criar(Tarefa tarefa)
        {
            if (tarefa.Data == DateTime.MinValue) // Verificar se a data da tarefa é vazia
                return BadRequest(new { Erro = "A data da tarefa não pode ser vazia" });

            if (tarefa.Data < DateTime.Now) //Verificar se a data da tarefa é menor que a data atual
                return BadRequest(new { Erro = "A data da tarefa não pode ser menor que a data atual" });

            _context.Tarefas.Add(tarefa);
            _context.SaveChanges();
            return CreatedAtAction(nameof(ObterPorId), new { id = tarefa.Id }, tarefa);
        }

        [HttpPut("{id}")]
        public IActionResult Atualizar(int id, Tarefa tarefa)
        {
            var tarefaBanco = _context.Tarefas.Find(id);

            if (tarefaBanco == null)
                return NotFound();

            if (tarefa.Data == DateTime.MinValue)
                return BadRequest(new { Erro = "A data da tarefa não pode ser vazia" });

            tarefaBanco.Titulo = tarefa.Titulo;
            tarefaBanco.Descricao = tarefa.Descricao;
            tarefaBanco.Data = tarefa.Data;
            tarefaBanco.Status = tarefa.Status;

            _context.Tarefas.Update(tarefaBanco);
            _context.SaveChanges();
            return Ok(tarefaBanco);
        }

        [HttpDelete("{id}")]
        public IActionResult Deletar(int id)
        {
            var tarefaBanco = _context.Tarefas.Find(id);

            if (tarefaBanco == null)
                return NotFound();

            _context.Tarefas.Remove(tarefaBanco);
            return NoContent();
        }
    }
}

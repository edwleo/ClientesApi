using ClientesApi.Data;
using ClientesApi.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace ClientesApi.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ClientesController : ControllerBase
    {
        //Todas las funcionalidad GET - POST - PUT - DELETE necesitan acceso a BD (ORM)
        private readonly AppDbContext _context;

        //Constructor (inicializar)
        public ClientesController(AppDbContext context) 
        { 
            _context = context;
        }

        //GET: api/clientes -> retornar la lista de clientes
        //async Task    : asíncrono (tiene latencia)
        //IEnumerable   : Define un set de datos (colección <Tipo>)
        //ActionResul   : Valor a obtener (colección definida en el modelo)
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Cliente>>> Listar([FromQuery] string? buscar)
        {
            //var clientes = await _context.Clientes.AsNoTracking().ToListAsync();
            //return Ok(clientes);
            var consulta = _context.Clientes.AsNoTracking();

            if (!string.IsNullOrWhiteSpace(buscar))
            {
                var texto = buscar.Trim();
                consulta = consulta.Where(c =>
                    c.Apellidos.Contains(texto) ||
                    c.Nombres.Contains(texto) ||
                    c.Dni.Contains(texto)
                    );
            }

            var lista = await consulta
                .OrderBy(c => c.Apellidos).ThenBy(c => c.Nombres)
                .Take(200)
                .ToListAsync();

            return Ok(lista);
        }

        //GET: api/clientes/1 -> Busca clientes por su ID
        [HttpGet("{id:int}")]
        public async Task<ActionResult<Cliente>> ObtenerPorId(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);

            if (cliente == null)
            {
                //Error 404
                return NotFound(new { mensaje = $"No existe el cliente con ID: {id}" });
            }

            //Ok (método 200)
            return Ok(cliente);
        }

        //POST: api/clientes -> registrar (no olvidar que debemos enviar en el BODY JSON {} )
        [HttpPost]
        public async Task<ActionResult<Cliente>> Registrar([FromBody] Cliente cliente)
        {
            //Evitar que el cliente envíe un ID manualmente
            cliente.Id = 0;

            //Validar que el DNI no esté repetido...
            if (await _context.Clientes.AnyAsync(c => c.Dni == cliente.Dni))
            {
                //Server Code 409 (error)
                return Conflict(new { mensaje = "Ya existe un cliente con ese DNI" });
            }

            //Procedemos al registro
            _context.Clientes.Add(cliente);
            await _context.SaveChangesAsync();

            //Server Code 201 (Save OK) - Ejecutamos en segundo plano el buscador por ID, pasamos el ID entrada, modelo
            return CreatedAtAction(nameof(ObtenerPorId), new { id = cliente.Id }, cliente);
        }

        //PUT: api/clientes/1 -> actualizar (además lleva un JSON en el BODY)
        [HttpPut("{id:int}")]
        public async Task<IActionResult> Actualizar(int id, [FromBody] Cliente cliente)
        {
            //Validación 1: Coincidencia de ID's
            if (id != cliente.Id)
            {
                //Server Code 400
                return BadRequest(new { mensaje = "El ID de la URL no coincide con el ID del cuerpo" });
            }

            //Validación 2: ¿Existe el ID?
            var existente = await _context.Clientes.FindAsync(id); //FindAsync() buscador por KEY (primary key)

            if (existente == null)
            {
                //Server Code 404
                return NotFound(new { mensaje = $"No existe cliente con id: {id}" });
            }

            //Validación 3: Conflicto DNI repetido y el ID no le pertenezca
            if (await _context.Clientes.AnyAsync(c => c.Dni == cliente.Dni && c.Id != id))
            {
                //Server Code 409 (error)
                return Conflict(new { mensaje = "Ya existe un cliente con ese DNI" });
            }

            //Procedemos a realizar la actualización...
            existente.Apellidos = cliente.Apellidos;
            existente.Nombres = cliente.Nombres;
            existente.Telefono = cliente.Telefono;
            existente.Correo = cliente.Correo;
            existente.Dni = cliente.Dni;
            existente.Direccion = cliente.Direccion;

            //Ahora confirmemos los cambios utilizando el contenedor "existente"
            await _context.SaveChangesAsync(); //está enlazado a <<<FindAsync>>>

            //Server Code 204
            return NoContent();
        }

        //DELETE: api/clientes/1 -> eliminar
        [HttpDelete("{id:int}")]
        public async Task<IActionResult> Eliminar(int id)
        {
            var cliente = await _context.Clientes.FindAsync(id);
            if (cliente == null)
            {
                //Server Code 404
                return NotFound(new { mensaje = $"No existe cliente con ID: {id}" });
            }

            _context.Clientes.Remove(cliente);
            await _context.SaveChangesAsync();

            //Server Code 204 (acción ejecutada correctamente)
            return NoContent();
        }

    }
}

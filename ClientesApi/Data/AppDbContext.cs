using ClientesApi.Models; //Modelo (definición tabla)
using Microsoft.EntityFrameworkCore; //Acceder BD

namespace ClientesApi.Data
{
    //Herencia => subClase : superClase
    public class AppDbContext : DbContext
    {
        //Constructor (inicializar)
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
        {

        }

        //ORM tendrá acceso al modelo cliente
        public DbSet<Cliente> Clientes { get; set; }  
    }
}

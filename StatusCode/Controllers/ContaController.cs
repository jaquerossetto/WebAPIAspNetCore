using System.IdentityModel.Tokens.Jwt;
using System.Text;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using StatusCode.Models;

namespace StatusCode.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ContaController : ControllerBase
    {
        private SistemaContext sistema = new SistemaContext();


        [HttpPost]
        [Route("AutenticarToken")]
        [AllowAnonymous]
        public ActionResult<dynamic> Autenticar(Credencial credencial)
        {

            var usuario = sistema.Usuario?.Where(Usuario => Usuario.Username == credencial.Username && Usuario.Senha == credencial.Senha).FirstOrDefault();

            if (usuario == null)
            {
                return NotFound(new { messenger = "Usuário ou senha incorretos." });
            }
            else
            {

                var chaveToken = GerarChaveToken();
                return Ok(new { token = chaveToken });
            }
        }

        private string GerarChaveToken()
        {
            var jwt = new JwtSecurityTokenHandler();

            var payload = new SecurityTokenDescriptor
            {
                Expires = DateTime.UtcNow.AddHours(1),
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(Encoding.ASCII.GetBytes(Ambiente.ChaveSecreta)),
                    SecurityAlgorithms.HmacSha256)
            };

            var chaveToken = jwt.CreateToken(payload);
            return jwt.WriteToken(chaveToken);
        }



        [HttpPost]
        [Route ("Cadastrar")]
        [AllowAnonymous]
        public ActionResult<Usuario> CadastrarUsuario(Usuario usuario)
        {
            sistema.Usuario?.Add(usuario);
            sistema.SaveChanges();

            if (usuario == null)
            {
                return BadRequest();
            }

            else
            {
                return Ok(usuario);
            }
        }


        [HttpPost]
        [Route ("Autenticar")]
        [AllowAnonymous]

        public ActionResult<dynamic> AutenticarUsuario(Credencial credencial)
        {
            var usuario = sistema.Usuario?.Where(Usuario => Usuario.Username == credencial.Username && Usuario.Senha == credencial.Senha).FirstOrDefault();

            if (usuario == null)
            {
                return NotFound(new { messenger = "Usuário ou senha incorretos." });
            }
            else
            {
                return Ok(credencial);
            }
        }


        [HttpGet]
        [Route("Listar")]
        [AllowAnonymous]
        public ActionResult<List<Usuario>> MostrarUsuario()
        {
            var TodosAlunos = sistema.Usuario?.ToList();
            if(TodosAlunos == null)
            {
                return NotFound();
            }
            else
            {
                return Ok(TodosAlunos);
            }
        }

    }

}
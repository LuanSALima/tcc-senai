using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tcc_3dsaa_grupoc.Models;
using PagedList;

namespace tcc_3dsaa_grupoc.Controllers
{
    public class UsuarioController : Controller
    {
        // GET: Usuario
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Cadastrar()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cadastrar(string nome_usuario, string email_usuario, string senha_usuario, string confirmar_senha_usuario)
        {
            if(ModelState.IsValid)
            {
                Usuario usuario = new Usuario();

                usuario.nome_usuario = nome_usuario;
                usuario.email_usuario = email_usuario;
                usuario.senha_usuario = senha_usuario;
                usuario.confirmar_senha_usuario = confirmar_senha_usuario;

                string resultado = usuario.CadastrarUsuario();
                TempData["Mensagem"] = resultado;

                if (resultado.Contains("sucesso"))
                {
                    return RedirectToAction("Login");
                }
                else
                {
                    return View();
                }
            }
            else
            {
                TempData["Mensagem"] = "Model State is Invalid";
                return RedirectToAction("Index", "Relato");
            }
            
        }

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(string nome_usuario, string senha_usuario)
        {

            Usuario usuario = Usuario.Logar(nome_usuario, senha_usuario);

            if (usuario != null)
            {
                Session["Usuário"] = usuario.id_usuario;
                Session["Nome_Usuário"] = usuario.nome_usuario;

                TempData["Mensagem"] = "Logado com Sucesso !";
                return RedirectToAction("Index", "Relato");
            }

            TempData["Mensagem"] = "Usuário não encontrado !";
            return View();

        }

        public ActionResult Perfil(int idUsuario = 0, int pagina = 1)
        {
            Usuario usuarioPerfil = Usuario.BuscaUsuario(idUsuario);

            if (usuarioPerfil != null)
            {
                TempData["Perfil_NomeUsuario"] = usuarioPerfil.nome_usuario;
                TempData["Perfil_IdUsuario"] = usuarioPerfil.id_usuario;

                List<Relato> listaRelatosUsuario;

                if (Session["Usuário"] != null)
                {
                    if ((int)Session["Usuário"] == idUsuario)
                    {
                        listaRelatosUsuario = Relato.ListaRelatosUsuario(idUsuario, true);
                    }
                    else
                    {
                        listaRelatosUsuario = Relato.ListaRelatosUsuario(idUsuario, false);
                    }
                }
                else
                {
                    listaRelatosUsuario = Relato.ListaRelatosUsuario(idUsuario, false);
                }

                if (listaRelatosUsuario.Count != 0)
                {
                    TempData["Perfil_SemRelatos"] = 1;
                }
                else
                {
                    TempData["Perfil_SemRelatos"] = 0;
                }
                
                return View(listaRelatosUsuario.OrderBy(relato => relato.data).ToPagedList(pagina, 10));
            }
            else
            {
                TempData["Mensagem"] = "Usuário não encontrado !";
                return RedirectToAction("Index", "Relato");
            }
            
           
        }

        public ActionResult Logout()
        {
            Session["Usuário"] = null;
            Session["Nome_Usuário"] = null;

            TempData["Mensagem"] = "Logout realizado com Sucesso !";
            return RedirectToAction("Index", "Relato");
        }
    }
}
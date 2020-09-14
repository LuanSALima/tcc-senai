using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using tcc_3dsaa_grupoc.Models;
using PagedList;

namespace tcc_3dsaa_grupoc.Controllers
{
    public class RelatoController : Controller
    {
        // GET: Relato
        public ActionResult Index(string rua = "", int pagina = 1)
        {
            if (!string.IsNullOrEmpty(rua))
            {
                List<Relato> relatosRua = Relato.ListaRelatosRua(rua);
                if (relatosRua.Count == 0)
                {
                    TempData["SemRelatos"] = 1;
                }
                else
                {
                    TempData["SemRelatos"] = 0;
                }
                return View(relatosRua.OrderBy(relato => relato.idRelato).ToPagedList(pagina, 10));
            }
            else
            {
                List<Relato> relatos = Relato.ListaRelatos();
                if (relatos.Count == 0)
                {
                    TempData["SemRelatos"] = 1;
                }
                else
                {
                    TempData["SemRelatos"] = 0;
                }
                return View(relatos.OrderBy(relato => relato.idRelato).ToPagedList(pagina, 10));
            }
            
        }

        public ActionResult Cadastrar()
        {
            if (Session["Usuário"] != null)
            {
                ViewBag.TipoRelato = new SelectList(TipoRelato.ListaTipoRelatos(), "idTipoRelato", "nomeTipoRelato");
                return View();
            }
            else
            {
                TempData["Mensagem"] = "É necessário estar logado para registrar relatos";
                return RedirectToAction("Login", "Usuario");
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Cadastrar(int idTipoRelato, string descricao, double localizacaoX, double localizacaoY, string data, string hora, bool anonimo)
        {

            if (ModelState.IsValid)
            {
                if (Session["Usuário"] != null)
                {
                    Relato relato = new Relato();

                    relato.idUsuario = (int) Session["Usuário"];
                    relato.idTipoRelato = idTipoRelato;
                    relato.descricao = descricao;
                    relato.localizacaoX = localizacaoX;
                    relato.localizacaoY = localizacaoY;
                    relato.data = data;
                    relato.hora = hora;
                    relato.anonimo = anonimo;

                    TempData["Mensagem"] = relato.Cadastrar();

                    return RedirectToAction("Index");
                }
                else
                {
                    TempData["Mensagem"] = "Usuário não encontrado";
                    return RedirectToAction("Login", "Usuario");
                }
              
                
            }
            else
            {
                TempData["Mensagem"] = "Model State is not Valid";
                return View();
            }

        }

        public ActionResult Editar(int idRelato=0)
        {
            if (Session["Usuário"] != null)
            {
                if (idRelato != 0)
                {
                    Relato relatoEncontrado = Relato.BuscaRelato(idRelato);

                    if (relatoEncontrado != null)
                    {
                        if (relatoEncontrado.idUsuario == (int)Session["Usuário"])
                        {
                            ViewBag.TipoRelato = new SelectList(TipoRelato.ListaTipoRelatos(), "idTipoRelato", "nomeTipoRelato");
                            return View(relatoEncontrado);
                        }
                        else
                        {
                            TempData["Mensagem"] = "Não é possível editar o relato de outro usuário !";
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        TempData["Mensagem"] = "Relato não encontrado !";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    TempData["Mensagem"] = "Escolha um relato para editar !";
                    return RedirectToAction("Perfil", "Usuario", new { idUsuario = (int)Session["Usuário"] });
                }

            }
            else
            {
                TempData["Mensagem"] = "É necessário estar logado para editar seus relatos !";
                return RedirectToAction("Login", "Usuario");
            }
            
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Editar(int idRelato, int idTipoRelato, string descricao, double localizacaoX, double localizacaoY, string data, string hora, bool anonimo)
        {
            //Ao inspecionar a pagina e trocar o valor do campo oculto "idRelato", ira gerar um erro no visual studio e o site ira parar de funcionar
            //o Erro que aparece aponta para a DropDownList da View Editar, mas o erro verdadeiramente está na execucao do comando MySQL, isto é,
            //O comando só edita se o idRelato e o idUsuario estiverem corretos, ao alterar o valor do campo oculto o comando não encontra o campo que
            //sera editado e por algum motivo gera um erro no dropdownlist da View Editar
            Relato relatoEscolhido = new Relato();

            if (Session["Usuário"] != null)
            {
                relatoEscolhido.idRelato = idRelato;
                relatoEscolhido.idUsuario = (int)Session["Usuário"];
                relatoEscolhido.idTipoRelato = idTipoRelato;
                relatoEscolhido.descricao = descricao;
                relatoEscolhido.localizacaoX = localizacaoX;
                relatoEscolhido.localizacaoY = localizacaoY;
                relatoEscolhido.data = data;
                relatoEscolhido.hora = hora;
                relatoEscolhido.anonimo = anonimo;

                if (relatoEscolhido.Editar())
                {
                    TempData["Mensagem"] = "Relato editado com sucesso !";
                    return RedirectToAction("Perfil", "Usuario", new { idUsuario = (int)Session["Usuário"] });
                }
                else
                {
                    TempData["Mensagem"] = "Erro ao editar o relato !";
                    return View();
                }
            }
            else
            {
                TempData["Mensagem"] = "É necessário estar logado para editar seus relatos !";
                return RedirectToAction("Login", "Usuario");
            }
            
        }

        public ActionResult Remover(int idRelato = 0)
        {

            if (Session["Usuário"] != null)
            {
                if (idRelato != 0)
                {
                    Relato relatoEncontrado = Relato.BuscaRelato(idRelato);

                    if (relatoEncontrado != null)
                    {
                        if (relatoEncontrado.idUsuario == (int)Session["Usuário"])
                        {
                            if (relatoEncontrado.Remover())
                            {
                                TempData["Mensagem"] = "Relato removido com sucesso !";
                                return RedirectToAction("Perfil", "Usuario", new { idUsuario = (int)Session["Usuário"] });
                            }
                            else
                            {
                                TempData["Mensagem"] = "Erro ao remover o relato !";
                                return RedirectToAction("Perfil", "Usuario", new { idUsuario = (int)Session["Usuário"] });
                            }
                            
                        }
                        else
                        {
                            TempData["Mensagem"] = "Não é possível remover o relato de outro usuário !";
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        TempData["Mensagem"] = "Relato não encontrado !";
                        return RedirectToAction("Index");
                    }
                }
                else
                {
                    TempData["Mensagem"] = "Escolha um relato para remover !";
                    return RedirectToAction("Perfil", "Usuario");
                }

            }
            else
            {
                TempData["Mensagem"] = "É necessário estar logado para remover algum relato !";
                return RedirectToAction("Login", "Usuario");
            }

        }
    }
}
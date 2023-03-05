using javax.annotation.processing;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using SPCPP.Model.Filters;
using SPCPP.Model.Helper;
using SPCPP.Model.Models;
using SPCPP.Model.Models.Request;
using SPCPP.Service.Interface;
using SPCPP.Service.Services;
using System.Xml.Linq;

namespace SPCPP.Web.Controllers
{
    [PaginaParaUsuarioLogado]
    public class Posgraduacao_ProfessorController : Controller
    {
        private readonly IPosgraduacaoService _posgraduacaoService;
        private readonly IPosgraduacao_ProfessorService _posgraduacao_ProfessorService;
        private readonly ISessao _sessao;

        public Posgraduacao_ProfessorController(IPosgraduacao_ProfessorService posgraduacao_ProfessorService, ISessao sessao, IPosgraduacaoService posgraduacaoService)
        {
            _posgraduacao_ProfessorService = posgraduacao_ProfessorService;
            _sessao = sessao;
            _posgraduacaoService = posgraduacaoService;
        }

        
        public IActionResult SalvarStatus(ulong professor_id , ulong posgraduacao_id , string status)
        {
            try
            {
                ProfessorCadastrado professorCastrado = _posgraduacao_ProfessorService.SalvarStatus(professor_id,posgraduacao_id,status);
                return Json(new { success = true, message = $@"Sucesso em salvar Professor: {professorCastrado.Cnome} com o Status: {professorCastrado.status}!" });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }

           
        }
        [HttpPost]
        public IActionResult UploadXml(IFormFile file, ulong id,double indiceh)
        {
            try
            {
                if (file == null)
                    throw new Exception("Insira um arquivo!");
                if (indiceh <= 0)
                    throw new Exception("Insira o Index-H");
                if (indiceh > 200)
                    throw new Exception("Index-H muito alto por favor fale com um administrador!");
                string sessaoUsuario = ControllerContext.HttpContext.Session.GetString("sessaoUsuarioLogado");
                if (string.IsNullOrEmpty(sessaoUsuario))
                    throw new Exception("Usuario não encontrado!");
                
                User usuario = JsonConvert.DeserializeObject<User>(sessaoUsuario);

                if (usuario.Perfil != Model.Enums.PerfilEnum.Docente )
                    throw new Exception("Seu tipo de Perfil não pode cadastrar em Pós Gradução!");

                Posgraduacao_Professor posgraduacao_Professor = _posgraduacao_ProfessorService.verifcarUsuarioCadastrado(usuario.Id, id);
                if (posgraduacao_Professor != null)
                    throw new Exception($"Cadastrado em {posgraduacao_Professor.DataCadastro}");

                if (file == null || !file.FileName.ToLower().Contains(".xml"))
                    throw new Exception("Arquivo Incorreto!!");
             
                XElement root = XElement.Load(file.OpenReadStream());
                SolucaoMecanica notas = _posgraduacao_ProfessorService.calcularNota(root,indiceh,usuario.Nome.ToLower(),id);
                
                if (!_posgraduacao_ProfessorService.Incluir(id, usuario, notas).Result)
                    throw new Exception("Erro ao salvar professor na Pós Graduação!");

                return Json(new { success = true, message = $"Sucesso em cadastrar-se!!! Sua nota é {notas.nota}" });
            }
            catch (Exception e)
            {
                return Json(new { success = false, message = e.Message });
            }
        }

        public IActionResult ListarProfVinculados(ulong id, int? pagina, string? nome, string Ordenar)
        {
            try
            {
                string sessaoUsuario = ControllerContext.HttpContext.Session.GetString("sessaoUsuarioLogado");
                User usuariologado = JsonConvert.DeserializeObject<User>(sessaoUsuario);

                ViewBag.Perfil = usuariologado.Perfil;


                ViewBag.Nome = nome;
                int totalpagina = Convert.ToInt32(_posgraduacaoService.GetParametro("NUMERO_PAGINATION"));
                Posgraduacao posgraduacao = _posgraduacaoService.PesquisarPorId(id);
                TempData["Posgraduacao"] = posgraduacao.nome.ToUpper();
                ViewBag.PosId = posgraduacao.id;
                List<ProfessorCadastrado> professores = new List<ProfessorCadastrado>();
                if (!string.IsNullOrEmpty(nome))
                {
                    professores = _posgraduacao_ProfessorService.PesquisarPorNome(id, nome).Result;
                    
                }
                else
                {
                    professores = _posgraduacao_ProfessorService.ListarProfVinculados(id).Result;

                }
                ViewBag.Id = id;
                ViewBag.OrdenarPg = Ordenar;
                ViewBag.CnomeParm = String.IsNullOrEmpty(Ordenar) ? "cnome" : "";
                ViewBag.EmailParm = Ordenar == "email" ? "email_desc" : "email";
                ViewBag.Data_NascParm = Ordenar == "data_nasc" ? "data_nasc_desc" : "data_nasc";
                //ViewBag.Data_exoneracaoParm = Ordenar == "data_exoneracao" ? "data_exoneracao_desc" : "data_exoneracao";
                //ViewBag.Data_saidaParm = Ordenar == "data_saida" ? "data_saida_desc" : "data_saida";
                //ViewBag.Data_aposentadoriaParm = Ordenar == "data_aposentadoria" ? "data_aposentadoria_desc" : "data_aposentadoria";
                ViewBag.Data_atualizacaoParm = Ordenar == "data_att" ? "data_att_desc" : "data_att";
                ViewBag.StatusParm = Ordenar == "status" ? "status_desc" : "status";
                ViewBag.SiapeParm = Ordenar == "siape" ? "siape_desc" : "siape";
                ViewBag.NotaParm = Ordenar == "nota" ? "nora_desc" : "nota";
                ViewBag.DataCadastroParm = Ordenar == "data_cadastro" ? "data_cadastro_desc" : "data_cadastro";


                switch (Ordenar)
                {
                    case "cnome":
                        professores = professores.OrderByDescending(s => s.Cnome).ToList();
                        break;
                    case "email":
                        professores = professores.OrderBy(s => s.Email).ToList();
                        break;
                    case "email_desc":
                        professores = professores.OrderByDescending(s => s.Email).ToList();
                        break;
                    case "data_nasc":
                        professores = professores.OrderBy(s => s.Data_nasc).ToList();
                        break;
                    case "data_nasc_desc":
                        professores = professores.OrderByDescending(s => s.Data_nasc).ToList();
                        break;
                    case "nota":
                        professores = professores.OrderBy(s => s.nota).ToList();
                        break;
                    case "nota_desc":
                        professores = professores.OrderByDescending(s => s.nota).ToList();
                        break;
                    case "data_cadastro":
                        professores = professores.OrderBy(s => s.DataCadastro).ToList();
                        break;
                    case "data_cadastro_desc":
                        professores = professores.OrderByDescending(s => s.DataCadastro).ToList();
                        break; 
                    
                    case "data_att":
                        professores = professores.OrderBy(s => s.DataAtualizacao).ToList();
                        break;
                    case "data_att_desc":
                        professores = professores.OrderByDescending(s => s.DataAtualizacao).ToList();
                        break;
                    //case "data_exoneracao":
                    //    professores = professores.OrderBy(s => s.Data_exoneracao).ToList();
                    //    break;
                    //case "data_exoneracao_desc":
                    //    professores = professores.OrderByDescending(s => s.Data_exoneracao).ToList();
                    //    break;
                    //case "data_saida":
                    //    professores = professores.OrderBy(s => s.Data_saida).ToList();
                    //    break;
                    //case "data_saida_desc":
                    //    professores = professores.OrderByDescending(s => s.Data_saida).ToList();
                    //    break;
                    //case "data_aposentadoria":
                    //    professores = professores.OrderBy(s => s.Data_aposentadoria).ToList();
                    //    break;
                    //case "data_aposentadoria_desc":
                    //    professores = professores.OrderByDescending(s => s.Data_aposentadoria).ToList();
                    //    break;
                    
 
                    case "status":
                        professores = professores.OrderBy(s => s.status).ToList();
                        break;
                    case "status_desc":
                        professores = professores.OrderByDescending(s => s.status).ToList();
                        break;
                    
                    case "siape":
                        professores = professores.OrderBy(s => s.siape).ToList();
                        break;
                    case "siape_desc":
                        professores = professores.OrderByDescending(s => s.siape).ToList();
                        break;

                    default:
                        professores = professores.OrderBy(s => s.Cnome).ToList();
                        break;
                }

                return View(PaginaList<ProfessorCadastrado>.Create(professores, pagina ?? 1, totalpagina));

            }
            catch (Exception ex)
            {
                TempData["MensagemErro"] = ex;
                return View();
            }

        }

        public async Task<JsonResult> Delete(ulong id, ulong posid)
        {
            try
            {
                return Json(new { sucesso = false, mensagem ="Desativado!" });
                bool valido = _posgraduacao_ProfessorService.deletar(id, posid).Result ;


                return Json(new { sucesso = true, valido = valido });

            }
            catch (Exception ex)
            {

                return Json(new { sucesso = false, mensagem = ex.Message });
            }

        }

    }
}

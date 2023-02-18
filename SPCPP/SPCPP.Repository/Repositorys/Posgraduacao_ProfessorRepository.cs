
using Dapper;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using SPCPP.Model.DbContexts;
using SPCPP.Model.Models;
using SPCPP.Model.Models.Request;
using SPCPP.Repository.Interface;
using System.Xml.Linq;

namespace SPCPP.Repository.Repositorys
{
    public class Posgraduacao_ProfessorRepository : GenericRepository<Posgraduacao_Professor, ApplicationDbContext>, IPosgraduacao_ProfessorRepository
    {
        private ContextSPCPP _contextSPCPP = new();
        public Posgraduacao_ProfessorRepository(ApplicationDbContext db) : base(db)
        {

        }

        public async Task<List<ProfessorCadastrado>> ListarProfVinculados(ulong posgraduacao_id)
        {

            _contextSPCPP.GetConnection();
           
            string sql = $@"select  p.user_id,p.siape , p.Cnome , p.Email ,p.Data_nasc , pp.DataCadastro, p.Carga_atual ,p.Status, pp.nota
                                        from professor p 
                                        right join posgraduacao_professor pp on p.user_id = pp.professor_id 
                                        left join  usuario u on u.id=pp.professor_id 
                                        where pp.posgraduacao_id = {posgraduacao_id};";


            List<ProfessorCadastrado> result = (await _contextSPCPP.Connection.QueryAsync<ProfessorCadastrado>(sql)).ToList();

            return result;
        }

        public async Task<List<ProfessorCadastrado>> PesquisarPorNome(ulong posgraduacao_id, string nome)
        {
            _contextSPCPP.GetConnection();
            
            string sql = $@"select  p.user_id,p.siape , p.Cnome , p.Email ,p.Data_nasc , pp.DataCadastro, p.Carga_atual ,p.Status, pp.nota
                                        from professor p 
                                        right join posgraduacao_professor pp on p.user_id = pp.professor_id 
                                        left join  usuario u on u.id=pp.professor_id 
                                        where pp.posgraduacao_id = {posgraduacao_id} and p.Cnome like '%{nome}%'";

            List<ProfessorCadastrado> result = (await _contextSPCPP.Connection.QueryAsync<ProfessorCadastrado>(sql)).ToList();

            return result;
        }

        public async Task<bool> deletar(ulong id, ulong posid)
        {
            try
            {
                string sql = $@"DELETE FROM posgraduacao_professor WHERE professor_id = {id} and posgraduacao_id= {posid};";

                _contextSPCPP.GetConnection();

                var result = await _contextSPCPP.Connection.ExecuteAsync(sql);

                return result != 0 ? true : false;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<Posgraduacao_Professor> verifcarUsuarioCadastrado(ulong professorId, ulong posgraducaoId)
        {
            try
            {
                string sql = $@"SELECT * FROM posgraduacao_professor where professor_id= {professorId} and posgraduacao_id={posgraducaoId};";

                _contextSPCPP.GetConnection();

                Posgraduacao_Professor result = (await _contextSPCPP.Connection.QueryFirstOrDefaultAsync<Posgraduacao_Professor>(sql));

                return result;

            }
            catch (Exception)
            {
                throw;
            }
        }

        #region Calcular nota
        public double calcularNota(XElement root, string nome)
        {
            string erro = string.Empty;
            double nota = 0;
            try
            {
                SolucaoMecanica solucaoMecanica = new SolucaoMecanica();
                solucaoMecanica.A1 = 0;
                solucaoMecanica.A2 = 0;
                solucaoMecanica.A3 = 0;
                solucaoMecanica.A4 = 0;

                #region GET H-INDEX

                //h-index UTILIZAR ORCID PARA DEIXAR MAIS PRECISO A PESQUISA 
                
                erro = "NOME";
                string nome_completo = root.Descendants("DADOS-GERAIS").First().Attribute("NOME-COMPLETO").Value;
                string[] name = nome_completo.Split(' ');

                string primeironome = name[0];
                string ultimonome = name[name.Count() - 1];

                if (!nome.Contains(primeironome) && !nome.Contains(ultimonome))
                    throw new Exception("O Campo nome completo do XML não condiz com o perfil logado!");

                string url = "https://www.scopus.com/results/authorNamesList.uri?sort=count-f&src=al&sid=2a8ed64d9cd0a572c1e221fa218d9633&sot=al&sdt=al&sl=45&s=AUTHLASTNAME%28ultimo_nome%29+AND+AUTHFIRST%28primeiro_nome%29&st1=ultimo_nome&st2=primeiro_nome&orcidId=&selectionPageSearch=anl&reselectAuthor=false&activeFlag=true&showDocument=false&resultsPerPage=20&offset=1&jtp=false&currentPage=1&previousSelectionCount=0&tooManySelections=false&previousResultCount=0&authSubject=LFSC&authSubject=HLSC&authSubject=PHSC&authSubject=SOSC&exactAuthorSearch=false&showFullList=false&authorPreferredName=&origin=searchauthorfreelookup&affiliationId=&txGid=0f6d3b1681be8dbfe4f429c0e2dbe0ea";
                url = url.Replace("primeiro_nome", primeironome);
                url = url.Replace("ultimo_nome", ultimonome);

                var response = CallUrl(url).Result;
                double h_index = Convert.ToDouble(response.Substring(response.IndexOf("dataCol4 alignRight\">") + 22, 3));
                



                #endregion

                #region GET A1-A4
                string query = string.Empty;
                IEnumerable<XElement> artigo_publicado = root.Descendants("ARTIGO-PUBLICADO");
                foreach (XElement artigo in artigo_publicado)
                {
                    string issn = artigo.Element("DETALHAMENTO-DO-ARTIGO").Attribute("ISSN").Value.Trim();
                    if (!string.IsNullOrEmpty(issn))
                    {
                        issn = issn.Substring(0, 4) + "-" + issn.Substring(4);
                        query = $"select estrato from publicadas_engenharias_lll_2017_2020 where issn like '%{issn}%';";
                    }
                    else
                    {
                        string titulo = artigo.Element("DETALHAMENTO-DO-ARTIGO").Attribute("TITULO-DO-PERIODICO-OU-REVISTA").Value.Trim();
                        if (!string.IsNullOrEmpty(titulo))
                        {
                            query = $"select estrato from publicadas_engenharias_lll_2017_2020 where titulo like '%{titulo}%';";
                        }
                    }
                    if (!string.IsNullOrEmpty(query))
                    {
                        _contextSPCPP.GetConnection();

                        string result = _contextSPCPP.Connection.QueryFirstOrDefault<string>(query);
                        var autores = artigo.Elements("AUTORES").Count();
                        switch (result)
                        {
                            case "A1":
                                solucaoMecanica.A1 += (1 / autores);
                                break;
                            case "A2":
                                solucaoMecanica.A2 += (1 / autores);
                                break;
                            case "A3":
                                solucaoMecanica.A3 += (1 / autores);
                                break;
                            case "A4":
                                solucaoMecanica.A4 += (1 / autores);
                                break;
                        }
                    }
                }
                #endregion


                #region GET DP
                //A quantidade de depósito de patentes(DP) é obtida no currículo LATTES referente aos últimos
                //cinco anos. O valor atribuído ao depósitos de patente DP será dividido pelo número de
                //docentes do programa coautores do respectivo pedido de patente;
                double dp = 0;
                IEnumerable<XElement> patente = root.Descendants("PATENTE");
                foreach (XElement pt in patente)
                {
                    bool deposito = pt.Element("DETALHAMENTO-DA-PATENTE").Element("HISTORICO-SITUACOES-PATENTE").Attribute("DESCRICAO-SITUACAO-PATENTE").Value.ToLower().Contains("dep");
                    bool tempo = (Convert.ToInt32(pt.Element("DADOS-BASICOS-DA-PATENTE")?.Attribute("ANO-DESENVOLVIMENTO")?.Value) >= DateTime.Now.Year - 5);

                    if (tempo && deposito != null && deposito)
                    {

                        int count = 0;
                        foreach (XElement at in pt.Elements("AUTORES"))
                            count += 1;

                        dp += (double)1 / count;
                    }
                }
                #endregion

                #region GET PC
                //O número de patentes concedidas (PC)  O valor atribuído à patente concedida PC será dividido pelo número de docentes
                //do programa coautores da respectiva patente registrada;
                double pc = 0;
                foreach (XElement pt in patente)
                {
                    bool deposito = pt.Element("DETALHAMENTO-DA-PATENTE").Element("HISTORICO-SITUACOES-PATENTE").Attribute("DESCRICAO-SITUACAO-PATENTE").Value.ToLower().Contains("concess");
                    bool tempo = (Convert.ToInt32(pt.Element("DADOS-BASICOS-DA-PATENTE")?.Attribute("ANO-DESENVOLVIMENTO")?.Value) >= DateTime.Now.Year - 5);

                    if (tempo && deposito != null && deposito)
                    {

                        int count = 0;
                        foreach (XElement at in pt.Elements("AUTORES"))
                            count += 1;

                        pc += (double)1 / count;
                    }
                }
                #endregion

                #region GET PQ
                //A bolsa de produtividade em pesquisa(PQ) é verificada no currículo LATTES.Esse parâmetro
                //é binário: para bolsista o parâmetro é atribuído o valor 1 e não bolsista 0.Caso o professor
                //tenha sido bolsista no período do último quadriênio, mas perdeu sua bolsa, será atribuído
                //valor 1 ao parâmetro PQ.
                int pq = 0;
                IEnumerable<XElement> pp = root.Descendants("PROJETO-DE-PESQUISA");
                foreach (XElement pps in pp)
                {
                    if (Convert.ToInt32(pps.Attribute("ANO-INICIO")?.Value) >= DateTime.Now.Year - 4)
                    {
                        foreach (XElement p in pps.Elements("EQUIPE-DO-PROJETO"))
                        {
                            //Console.WriteLine( p);
                            if (p.Element("INTEGRANTES-DO-PROJETO")?.Attribute("NRO-ID-CNPQ")?.Value != "")
                            {
                                pq = 1;
                                break;
                            }

                        }
                    }
                }
                #endregion

                solucaoMecanica.DP = dp;
                solucaoMecanica.PC = pc;
                solucaoMecanica.PQ = pq;
                solucaoMecanica.indiceH = h_index;

                nota = NotaMecanica(solucaoMecanica);
            }

            catch (Exception e)
            {
                if (e.Message.Contains("no elements"))
                    throw new Exception($"Atenção o arquivo XML esta errado, não contém {erro}!");
                else
                    throw new Exception($"Erro ao gerar nota, {e.Message}");
            }
            return Math.Round(nota, 2); 
        }
        private static async Task<string> CallUrl(string fullUrl)
        {
            HttpClient client = new HttpClient();
            var response = await client.GetStringAsync(fullUrl);
            return response;
        }
        public static double NotaMecanica(SolucaoMecanica sM)
        {

            double resultA1_A4 = ((sM.A1 * 1) + (sM.A2 * 0.75) + (sM.A3 * 0.625) + (sM.A4 * 0.5)) / 2.875;

            double PD = ((2 * resultA1_A4) + (sM.indiceH * 0.8) + (sM.DP * 0.2) + (sM.PC * 2) + (sM.PQ * 4)) / 9;

            return PD;
        }
        #endregion
    }
}

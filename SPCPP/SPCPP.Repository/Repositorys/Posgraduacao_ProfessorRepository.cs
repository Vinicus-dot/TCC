﻿
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
        public SolucaoMecanica calcularNota(XElement root, string nome, ulong posgraducao_id)
        {
            string erro = string.Empty;
            SolucaoMecanica solucaoMecanica = new SolucaoMecanica();
            try
            {
                solucaoMecanica.nota = 0;
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

                //if (!nome.Contains(primeironome.ToLower()) && !nome.Contains(ultimonome.ToLower()) && !nome.Contains(name[1].ToLower()))
                    //throw new Exception("O Campo nome completo do XML não condiz com o perfil logado!");

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
                        SolucaoMecanica solucaoMecanicaCautores = new SolucaoMecanica();
                        switch (result)
                        {
                            case "A1":
                                {
                                    int quantidade = 1;
                                    var autores = artigo.Elements("AUTORES");
                                    foreach(XElement at in autores)
                                    {
                                        string[] nomeCoautor = at.Attribute("NOME-COMPLETO-DO-AUTOR").Value.Replace(",", " ").Replace("'", " ").Trim().Split(" "); 
                                        query = $"select count(1) from posgraduacao_professor pp inner join professor p on pp.professor_id = p.user_id where p.Cnome like '%{nomeCoautor[0]}%' and p.Cnome like '%{nomeCoautor[1]}%' and p.Cnome like '%{nomeCoautor[nomeCoautor.Count()-1]}%' and pp.posgraduacao_id = {posgraducao_id};";
                                        int existencia = _contextSPCPP.Connection.QueryFirstOrDefault<int>(query);
                                        if (existencia != 0)
                                            quantidade += 1;

                                    }
                                    if (quantidade > 0)
                                    {
                                        foreach (XElement at in autores)
                                        {
                                            string[] nomeCoautor = at.Attribute("NOME-COMPLETO-DO-AUTOR").Value.Replace(",", " ").Replace("'", " ").Trim().Split(" ");
                                            query = $"select pp.* from posgraduacao_professor pp inner join professor p on pp.professor_id = p.user_id where p.Cnome like '%{nomeCoautor[0]}%' and p.Cnome like '%{nomeCoautor[1]}%' and p.Cnome like '%{nomeCoautor[nomeCoautor.Count() - 1]}%' and pp.posgraduacao_id = {posgraducao_id};";
                                            Posgraduacao_Professor posgraduacao_Professor = _contextSPCPP.Connection.QueryFirstOrDefault<Posgraduacao_Professor>(query);
                                            if (posgraduacao_Professor != null)
                                            {
                                                posgraduacao_Professor.A1 -= ((double)1 / quantidade);
                                                solucaoMecanicaCautores.A1 = posgraduacao_Professor.A1;
                                                solucaoMecanicaCautores.A2 = posgraduacao_Professor.A2;
                                                solucaoMecanicaCautores.A3 = posgraduacao_Professor.A3;
                                                solucaoMecanicaCautores.A4 = posgraduacao_Professor.A4;
                                                solucaoMecanicaCautores.DP = posgraduacao_Professor.DP;
                                                solucaoMecanicaCautores.PQ = posgraduacao_Professor.PQ;
                                                solucaoMecanicaCautores.PC = posgraduacao_Professor.PC;
                                                solucaoMecanicaCautores.indiceH = posgraduacao_Professor.indiceH;
                                                string nota = NotaMecanica(solucaoMecanicaCautores).ToString().Replace(",",".");
                                                query = $@"UPDATE posgraduacao_professor SET nota = {nota}, a1 = {posgraduacao_Professor.A1.ToString().Replace(",", ".")} WHERE id = {posgraduacao_Professor.id}";
                                                var salvarCoautor = _contextSPCPP.Connection.Execute(query);
                                                if (salvarCoautor == 0 )
                                                    throw new Exception("A1 - Erro ao salvar Coautor");
                                            }
                                        }
                                    }
                                    
                                    solucaoMecanica.A1 += ((double)1 / quantidade);
                                    
                                }
                                break;
                            case "A2":
                                {
                                    int quantidade = 1;
                                    var autores = artigo.Elements("AUTORES");
                                    foreach (XElement at in autores)
                                    {
                                        string[] nomeCoautor = at.Attribute("NOME-COMPLETO-DO-AUTOR").Value.Replace(",", " ").Replace("'", " ").Trim().Split(" ");
                                        query = $"select count(1) from posgraduacao_professor pp inner join professor p on pp.professor_id = p.user_id where p.Cnome like '%{nomeCoautor[0]}%' and p.Cnome like '%{nomeCoautor[1]}%' and p.Cnome like '%{nomeCoautor[nomeCoautor.Count() - 1]}%' and pp.posgraduacao_id = {posgraducao_id};";
                                        int existencia = _contextSPCPP.Connection.QueryFirstOrDefault<int>(query);
                                        if (existencia != 0)
                                            quantidade += 1;

                                    }
                                    if (quantidade > 0)
                                    {
                                        foreach (XElement at in autores)
                                        {
                                            string[] nomeCoautor = at.Attribute("NOME-COMPLETO-DO-AUTOR").Value.Replace(",", " ").Replace("'", " ").Trim().Split(" ");
                                            query = $"select pp.* from posgraduacao_professor pp inner join professor p on pp.professor_id = p.user_id where p.Cnome like '%{nomeCoautor[0]}%' and p.Cnome like '%{nomeCoautor[1]}%' and p.Cnome like '%{nomeCoautor[nomeCoautor.Count() - 1]}%' and pp.posgraduacao_id = {posgraducao_id};";
                                            Posgraduacao_Professor posgraduacao_Professor = _contextSPCPP.Connection.QueryFirstOrDefault<Posgraduacao_Professor>(query);
                                            if (posgraduacao_Professor != null)
                                            {
                                                posgraduacao_Professor.A2 -= ((double)1 / quantidade);
                                                solucaoMecanicaCautores.A1 = posgraduacao_Professor.A1;
                                                solucaoMecanicaCautores.A2 = posgraduacao_Professor.A2;
                                                solucaoMecanicaCautores.A3 = posgraduacao_Professor.A3;
                                                solucaoMecanicaCautores.A4 = posgraduacao_Professor.A4;
                                                solucaoMecanicaCautores.DP = posgraduacao_Professor.DP;
                                                solucaoMecanicaCautores.PQ = posgraduacao_Professor.PQ;
                                                solucaoMecanicaCautores.PC = posgraduacao_Professor.PC;
                                                solucaoMecanicaCautores.indiceH = posgraduacao_Professor.indiceH;
                                                string nota = NotaMecanica(solucaoMecanicaCautores).ToString().Replace(",", ".");
                                                query = $@"UPDATE posgraduacao_professor SET nota = {nota}, a2 = {posgraduacao_Professor.A2.ToString().Replace(",", ".")} WHERE id = {posgraduacao_Professor.id}";
                                                var salvarCoautor = _contextSPCPP.Connection.Execute(query);
                                                if (salvarCoautor == 0)
                                                    throw new Exception("A2 - Erro ao salvar Coautor");
                                            }
                                        }
                                    }

                                    solucaoMecanica.A2 += ((double)1 / quantidade);
                                }
                                break;
                            case "A3":
                                {
                                    int quantidade = 1;
                                    var autores = artigo.Elements("AUTORES");
                                    foreach (XElement at in autores)
                                    {
                                        string[] nomeCoautor = at.Attribute("NOME-COMPLETO-DO-AUTOR").Value.Replace(",", " ").Replace("'", " ").Trim().Split(" ");
                                        query = $"select count(1) from posgraduacao_professor pp inner join professor p on pp.professor_id = p.user_id where p.Cnome like '%{nomeCoautor[0]}%' and p.Cnome like '%{nomeCoautor[1]}%' and p.Cnome like '%{nomeCoautor[nomeCoautor.Count() - 1]}%' and pp.posgraduacao_id = {posgraducao_id};";
                                        int existencia = _contextSPCPP.Connection.QueryFirstOrDefault<int>(query);
                                        if (existencia != 0)
                                            quantidade += 1;

                                    }
                                    if (quantidade > 0)
                                    {
                                        foreach (XElement at in autores)
                                        {
                                            string[] nomeCoautor = at.Attribute("NOME-COMPLETO-DO-AUTOR").Value.Replace(",", " ").Replace("'", " ").Trim().Split(" ");
                                            query = $"select pp.* from posgraduacao_professor pp inner join professor p on pp.professor_id = p.user_id where p.Cnome like '%{nomeCoautor[0]}%' and p.Cnome like '%{nomeCoautor[1]}%' and p.Cnome like '%{nomeCoautor[nomeCoautor.Count() - 1]}%' and pp.posgraduacao_id = {posgraducao_id};";
                                            Posgraduacao_Professor posgraduacao_Professor = _contextSPCPP.Connection.QueryFirstOrDefault<Posgraduacao_Professor>(query);
                                            if (posgraduacao_Professor != null)
                                            {
                                                posgraduacao_Professor.A3 -= ((double)1 / quantidade);
                                                solucaoMecanicaCautores.A1 = posgraduacao_Professor.A1;
                                                solucaoMecanicaCautores.A2 = posgraduacao_Professor.A2;
                                                solucaoMecanicaCautores.A3 = posgraduacao_Professor.A3;
                                                solucaoMecanicaCautores.A4 = posgraduacao_Professor.A4;
                                                solucaoMecanicaCautores.DP = posgraduacao_Professor.DP;
                                                solucaoMecanicaCautores.PQ = posgraduacao_Professor.PQ;
                                                solucaoMecanicaCautores.PC = posgraduacao_Professor.PC;
                                                solucaoMecanicaCautores.indiceH = posgraduacao_Professor.indiceH;
                                                string nota = NotaMecanica(solucaoMecanicaCautores).ToString().Replace(",", ".");
                                                query = $@"UPDATE posgraduacao_professor SET nota = {nota}, a3 = {posgraduacao_Professor.A3.ToString().Replace(",", ".")} WHERE id = {posgraduacao_Professor.id}";
                                                var salvarCoautor = _contextSPCPP.Connection.Execute(query);
                                                if (salvarCoautor == 0)
                                                    throw new Exception("A3 - Erro ao salvar Coautor");
                                            }
                                        }
                                    }

                                    solucaoMecanica.A3 += ((double)1 / quantidade);
                                }
                                break;
                            case "A4":
                                {
                                    int quantidade = 1;
                                    var autores = artigo.Elements("AUTORES");
                                    foreach (XElement at in autores)
                                    {
                                        string[] nomeCoautor = at.Attribute("NOME-COMPLETO-DO-AUTOR").Value.Replace(",", " ").Replace("'", " ").Trim().Split(" ");
                                        query = $"select count(1) from posgraduacao_professor pp inner join professor p on pp.professor_id = p.user_id where p.Cnome like '%{nomeCoautor[0]}%' and p.Cnome like '%{nomeCoautor[1]}%' and p.Cnome like '%{nomeCoautor[nomeCoautor.Count() - 1]}%' and pp.posgraduacao_id = {posgraducao_id};";
                                        int existencia = _contextSPCPP.Connection.QueryFirstOrDefault<int>(query);
                                        if (existencia != 0)
                                            quantidade += 1;

                                    }
                                    if (quantidade > 0)
                                    {
                                        foreach (XElement at in autores)
                                        {
                                            string[] nomeCoautor = at.Attribute("NOME-COMPLETO-DO-AUTOR").Value.Replace(",", " ").Replace("'", " ").Trim().Split(" ");
                                            query = $"select pp.* from posgraduacao_professor pp inner join professor p on pp.professor_id = p.user_id where p.Cnome like '%{nomeCoautor[0]}%' and p.Cnome like '%{nomeCoautor[1]}%' and p.Cnome like '%{nomeCoautor[nomeCoautor.Count() - 1]}%' and pp.posgraduacao_id = {posgraducao_id};";
                                            Posgraduacao_Professor posgraduacao_Professor = _contextSPCPP.Connection.QueryFirstOrDefault<Posgraduacao_Professor>(query);
                                            if (posgraduacao_Professor != null)
                                            {
                                                posgraduacao_Professor.A4 -= ((double)1 / quantidade);
                                                solucaoMecanicaCautores.A1 = posgraduacao_Professor.A1;
                                                solucaoMecanicaCautores.A2 = posgraduacao_Professor.A2;
                                                solucaoMecanicaCautores.A3 = posgraduacao_Professor.A3;
                                                solucaoMecanicaCautores.A4 = posgraduacao_Professor.A4;
                                                solucaoMecanicaCautores.DP = posgraduacao_Professor.DP;
                                                solucaoMecanicaCautores.PQ = posgraduacao_Professor.PQ;
                                                solucaoMecanicaCautores.PC = posgraduacao_Professor.PC;
                                                solucaoMecanicaCautores.indiceH = posgraduacao_Professor.indiceH;
                                                string nota = NotaMecanica(solucaoMecanicaCautores).ToString().Replace(",", ".");
                                                query = $@"UPDATE posgraduacao_professor SET nota = {nota}, a4 = {posgraduacao_Professor.A1.ToString().Replace(",", ".")} WHERE id = {posgraduacao_Professor.id}";
                                                var salvarCoautor = _contextSPCPP.Connection.Execute(query);
                                                if (salvarCoautor == 0)
                                                    throw new Exception("A4 - Erro ao salvar Coautor");
                                            }
                                        }
                                    }

                                    solucaoMecanica.A4 += ((double)1 / quantidade);
                                }
                                break;
                        }
                    }
                }
                #endregion

                IEnumerable<XElement> patente = root.Descendants("PATENTE");
                #region GET DP
                //A quantidade de depósito de patentes(DP) é obtida no currículo LATTES referente aos últimos
                //cinco anos. O valor atribuído ao depósitos de patente DP será dividido pelo número de
                //docentes do programa coautores do respectivo pedido de patente;
                double dp = 0;
                
                foreach (XElement pt in patente)
                {
                    bool deposito = pt.Element("DETALHAMENTO-DA-PATENTE").Element("HISTORICO-SITUACOES-PATENTE").Attribute("DESCRICAO-SITUACAO-PATENTE").Value.ToLower().Contains("dep");
                    bool tempo = (Convert.ToInt32(pt.Element("DADOS-BASICOS-DA-PATENTE")?.Attribute("ANO-DESENVOLVIMENTO")?.Value) >= DateTime.Now.Year - 4);
                    tempo = true;

                    if (tempo &&  deposito)
                    {

                        int quantidade = 1;

                        var autores = pt.Elements("AUTORES");
                        foreach (XElement at in autores)
                        {
                            string[] nomeCoautor = at.Attribute("NOME-COMPLETO-DO-AUTOR").Value.Replace(",", " ").Replace("'", " ").Trim().Split(" ");
                            query = $"select count(1) from posgraduacao_professor pp inner join professor p on pp.professor_id = p.user_id where p.Cnome like '%{nomeCoautor[0]}%' and p.Cnome like '%{nomeCoautor[1]}%' and p.Cnome like '%{nomeCoautor[nomeCoautor.Count() - 1]}%' and pp.posgraduacao_id = {posgraducao_id};";
                            int existencia = _contextSPCPP.Connection.QueryFirstOrDefault<int>(query);
                            if (existencia != 0)
                                quantidade += 1;

                        }
                        if (quantidade > 0)
                        {
                            foreach (XElement at in autores)
                            {

                                string[] nomeCoautor = at.Attribute("NOME-COMPLETO-DO-AUTOR").Value.Replace(",", " ").Replace("'", " ").Trim().Split(" ");
                                
                                    query = $"select pp.* from posgraduacao_professor pp inner join professor p on pp.professor_id = p.user_id where p.Cnome like '%{nomeCoautor[0]}%' and p.Cnome like '%{nomeCoautor[1]}%' and p.Cnome like '%{nomeCoautor[nomeCoautor.Count() - 1]}%' and pp.posgraduacao_id = {posgraducao_id};";
                                    Posgraduacao_Professor posgraduacao_Professor = _contextSPCPP.Connection.QueryFirstOrDefault<Posgraduacao_Professor>(query);
                                    if (posgraduacao_Professor != null)
                                    {
                                        SolucaoMecanica solucaoMecanicaCautores = new SolucaoMecanica();

                                        posgraduacao_Professor.DP -= ((double)1 / quantidade);
                                        solucaoMecanicaCautores.A1 = posgraduacao_Professor.A1;
                                        solucaoMecanicaCautores.A2 = posgraduacao_Professor.A2;
                                        solucaoMecanicaCautores.A3 = posgraduacao_Professor.A3;
                                        solucaoMecanicaCautores.A4 = posgraduacao_Professor.A4;
                                        solucaoMecanicaCautores.DP = posgraduacao_Professor.DP;
                                        solucaoMecanicaCautores.PQ = posgraduacao_Professor.PQ;
                                        solucaoMecanicaCautores.PC = posgraduacao_Professor.PC;
                                        solucaoMecanicaCautores.indiceH = posgraduacao_Professor.indiceH;
                                        string nota = NotaMecanica(solucaoMecanicaCautores).ToString().Replace(",", ".");
                                        query = $@"UPDATE posgraduacao_professor SET nota = {nota}, dp = {posgraduacao_Professor.DP.ToString().Replace(",", ".")} WHERE id = {posgraduacao_Professor.id}";
                                        var salvarCoautor = _contextSPCPP.Connection.Execute(query);
                                        if (salvarCoautor == 0)
                                            throw new Exception("DP - Erro ao salvar Coautor");

                                    }
                                
                            }
                        }
                        
                        dp = (double)1 / quantidade;
                    }
                }
                #endregion

                #region GET PC
                //O número de patentes concedidas (PC)  O valor atribuído à patente concedida PC será dividido pelo número de docentes
                //do programa coautores da respectiva patente registrada;
                double pc = 0;
                foreach (XElement pt in patente)
                {
                    bool concedidas = pt.Element("DETALHAMENTO-DA-PATENTE").Element("HISTORICO-SITUACOES-PATENTE").Attribute("DESCRICAO-SITUACAO-PATENTE").Value.ToLower().Contains("concess");
                    bool tempo = (Convert.ToInt32(pt.Element("DADOS-BASICOS-DA-PATENTE")?.Attribute("ANO-DESENVOLVIMENTO")?.Value) >= DateTime.Now.Year - 4);
                    tempo = true;
                    if (tempo && concedidas)
                    {

                        int quantidade = 1;

                        var autores = pt.Elements("AUTORES");
                        foreach (XElement at in autores)
                        {
                            string[] nomeCoautor = at.Attribute("NOME-COMPLETO-DO-AUTOR").Value.Replace(",", " ").Replace("'", " ").Trim().Split(" ");
                            query = $"select count(1) from posgraduacao_professor pp inner join professor p on pp.professor_id = p.user_id where p.Cnome like '%{nomeCoautor[0]}%' and p.Cnome like '%{nomeCoautor[1]}%' and p.Cnome like '%{nomeCoautor[nomeCoautor.Count() - 1]}%' and pp.posgraduacao_id = {posgraducao_id};";
                            int existencia = _contextSPCPP.Connection.QueryFirstOrDefault<int>(query);
                            if (existencia != 0)
                                quantidade += 1;

                        }
                        if (quantidade > 0)
                        {
                            foreach (XElement at in autores)
                            {

                                string[] nomeCoautor = at.Attribute("NOME-COMPLETO-DO-AUTOR").Value.Replace(",", " ").Replace("'", " ").Trim().Split(" ");
                                query = $"select pp.* from posgraduacao_professor pp inner join professor p on pp.professor_id = p.user_id where p.Cnome like '%{nomeCoautor[0]}%' and p.Cnome like '%{nomeCoautor[1]}%' and p.Cnome like '%{nomeCoautor[nomeCoautor.Count() - 1]}%' and pp.posgraduacao_id = {posgraducao_id};";
                                Posgraduacao_Professor posgraduacao_Professor = _contextSPCPP.Connection.QueryFirstOrDefault<Posgraduacao_Professor>(query);
                                if (posgraduacao_Professor != null)
                                {
                                    SolucaoMecanica solucaoMecanicaCautores = new SolucaoMecanica();

                                    posgraduacao_Professor.PC -= ((double)1 / quantidade);
                                    solucaoMecanicaCautores.A1 = posgraduacao_Professor.A1;
                                    solucaoMecanicaCautores.A2 = posgraduacao_Professor.A2;
                                    solucaoMecanicaCautores.A3 = posgraduacao_Professor.A3;
                                    solucaoMecanicaCautores.A4 = posgraduacao_Professor.A4;
                                    solucaoMecanicaCautores.DP = posgraduacao_Professor.DP;
                                    solucaoMecanicaCautores.PQ = posgraduacao_Professor.PQ;
                                    solucaoMecanicaCautores.PC = posgraduacao_Professor.PC;
                                    solucaoMecanicaCautores.indiceH = posgraduacao_Professor.indiceH;
                                    string nota = NotaMecanica(solucaoMecanicaCautores).ToString().Replace(",", ".");
                                    query = $@"UPDATE posgraduacao_professor SET nota = {nota}, pc = {posgraduacao_Professor.PC.ToString().Replace(",", ".")} WHERE id = {posgraduacao_Professor.id}";
                                    var salvarCoautor = _contextSPCPP.Connection.Execute(query);
                                    if (salvarCoautor == 0)
                                        throw new Exception("PC - Erro ao salvar Coautor");

                                }
                            }
                        }

                        pc = (double)1 / quantidade;
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
                solucaoMecanica.nota = NotaMecanica(solucaoMecanica);
             
            }

            catch (Exception e)
            {
                if (e.Message.Contains("no elements"))
                    throw new Exception($"Atenção o arquivo XML esta errado, não contém {erro}!");
                else
                    throw new Exception($"Erro ao gerar nota, {e.Message}");
            }
            return solucaoMecanica; 
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

            return Math.Round(PD,2);
        }
        #endregion
    }
}

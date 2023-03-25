
using Dapper;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using SPCPP.Model.DbContexts;
using SPCPP.Model.Models;
using SPCPP.Model.Models.Request;
using SPCPP.Repository.Interface;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Linq;
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

            string sql = $@"select  p.user_id,p.siape , p.Cnome , p.Email,pp.posgraduacao_id ,p.Data_nasc , pp.DataCadastro,pp.DataAtualizacao, pp.status, pp.nota
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

            string sql = $@"select  p.user_id,p.siape , p.Cnome , p.Email ,pp.posgraduacao_id,p.Data_nasc , pp.DataCadastro,pp.DataAtualizacao , pp.status, pp.nota
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

        public UploadXML uploadXML(XElement root)
        {
            try
            {
                UploadXML uploadXML = new UploadXML();
                uploadXML.nome = root.Descendants("DADOS-GERAIS").First().Attribute("NOME-COMPLETO").Value;
                #region GET PQ
                //A bolsa de produtividade em pesquisa(PQ) é verificada no currículo LATTES.Esse parâmetro
                //é binário: para bolsista o parâmetro é atribuído o valor 1 e não bolsista 0.Caso o professor
                //tenha sido bolsista no período do último quadriênio, mas perdeu sua bolsa, será atribuído
                //valor 1 ao parâmetro PQ.
                bool pq = false;
                IEnumerable<XElement> pp = root.Descendants("DADOS-GERAIS");
                foreach (XElement dados in pp)
                {
                    bool result = false;
                    var bolsa = dados.Element("OUTRAS-INFORMACOES-RELEVANTES")?.Attribute("OUTRAS-INFORMACOES-RELEVANTES")?.Value;
                    
                    if(bolsa != null)
                        result = bolsa.ToLower().Contains("bolsista de produtividade");
                    if (result)
                    {
                        pq = true;
                        break;
                    }

                }
                uploadXML.pq = pq;
                #endregion
                
                #region GET A1-A4
                string query = string.Empty;

                IEnumerable<XElement> artigo_publicado = root.Descendants("ARTIGO-PUBLICADO");
                //List<string> issnStrings = new List<string>();
                uploadXML.artigolist = new List<Artigo>();
                
                foreach (XElement artigo in artigo_publicado)
                {

                    
                    bool ano = Convert.ToInt32(artigo.Element("DADOS-BASICOS-DO-ARTIGO").Attribute("ANO-DO-ARTIGO").Value.Trim()) >= DateTime.Now.Year - 4;

                    string issn = artigo.Element("DETALHAMENTO-DO-ARTIGO").Attribute("ISSN").Value.Trim();
                    
                    //if (ano && !string.IsNullOrEmpty(issn) && !issnStrings.Contains(issn))
                    //if (ano && !string.IsNullOrEmpty(issn) && uploadXML.artigolist.Find(x=> x.issn ==issn) == null)
                    if (ano && !string.IsNullOrEmpty(issn) )
                    {
                        Artigo artigoup = new Artigo();
                        artigoup.coautoresArtigo = new List<Autor>();

                        //issnStrings.Add(issn);
                        artigoup.issn = issn;
                        artigoup.titulo = artigo.Element("DETALHAMENTO-DO-ARTIGO").Attribute("TITULO-DO-PERIODICO-OU-REVISTA").Value.Trim();
                        artigoup.ano = artigo.Element("DADOS-BASICOS-DO-ARTIGO").Attribute("ANO-DO-ARTIGO").Value.Trim();


                        IEnumerable<XElement> autores = artigo.Elements("AUTORES");

                        foreach (XElement au in autores)
                        {
                            Autor autor = new Autor();
                            autor.nome = au.Attribute("NOME-COMPLETO-DO-AUTOR").Value.Trim();
                            autor.ordem_altoria = au.Attribute("ORDEM-DE-AUTORIA").Value.Trim();

                            if (artigoup.coautoresArtigo.Find(x => (x.nome == autor.nome) && (x.ordem_altoria == autor.ordem_altoria)) == null)
                                artigoup.coautoresArtigo.Add(autor);
                        }

                        uploadXML.artigolist.Add(artigoup);

                        
                    }

                }


                #endregion


                #region GET DP E PC
                //A quantidade de depósito de patentes(DP) é obtida no currículo LATTES referente aos últimos
                //cinco anos. O valor atribuído ao depósitos de patente DP será dividido pelo número de
                //docentes do programa coautores do respectivo pedido de patente;

                //O número de patentes concedidas (PC)  O valor atribuído à patente concedida PC será dividido pelo número de docentes
                //do programa coautores da respectiva patente registrada;

                IEnumerable<XElement> patente = root.Descendants("PATENTE");

                uploadXML.artigopatentepc = new List<Patente>();
                uploadXML.artigopatentedp = new List<Patente>();               
              
                
                foreach (XElement pt in patente)
                {
                    IEnumerable<XElement> autores = pt.Elements("AUTORES");
                    string titulo = pt.Element("DADOS-BASICOS-DA-PATENTE").Attribute("TITULO").Value;
                    string pais = pt.Element("DADOS-BASICOS-DA-PATENTE").Attribute("PAIS").Value;
                    string codigo_registro_patente = pt.Element("DETALHAMENTO-DA-PATENTE").Element("REGISTRO-OU-PATENTE").Attribute("CODIGO-DO-REGISTRO-OU-PATENTE").Value;
                    IEnumerable<XElement> historicos = pt.Element("DETALHAMENTO-DA-PATENTE").Elements("HISTORICO-SITUACOES-PATENTE");
                    foreach(XElement his in  historicos)
                    {         
                        
                        string anocomeco = his.Attribute("DATA-SITUACAO-PATENTE").Value.Trim();
                        string tipopatente = his.Attribute("DESCRICAO-SITUACAO-PATENTE").Value.Trim().ToLower();
                        string data = his.Attribute("DATA-SITUACAO-PATENTE").Value.Trim().ToLower();
                        bool tempo = false;
                        string datainfo = string.Empty;
                        if (!string.IsNullOrEmpty(data))
                        {
                            int dia = Convert.ToInt32(data.Substring(0, 2));
                            int mes = Convert.ToInt32(data.Substring(2, 2));
                            int ano = Convert.ToInt32(data.Substring(4, 4));

                            DateTime datatime = new DateTime(ano, mes, dia);
                            datainfo = $"{dia}/{mes}/{ano}";
                            TimeSpan diferenca = DateTime.Now.Subtract(datatime);
                            if (diferenca.TotalDays < 1460)
                                tempo = true;

                        }
                        else if(Convert.ToInt32(anocomeco) >= DateTime.Now.Year - 4)                     
                            tempo = true;

                        
                        if (tempo && tipopatente.Contains("dep"))
                        {
                            Patente artigopatentedp = new Patente();
                            artigopatentedp.titulo = titulo;
                            artigopatentedp.ano = datainfo;
                            artigopatentedp.pais = pais;
                            artigopatentedp.codigo_registro_patente = codigo_registro_patente;

                            artigopatentedp.coautoresPatente = new List<Autor>();
                            foreach(XElement au in autores)
                            {
                                Autor autor = new Autor();
                                autor.nome = au.Attribute("NOME-COMPLETO-DO-AUTOR").Value.Trim();
                                autor.ordem_altoria = au.Attribute("ORDEM-DE-AUTORIA").Value.Trim();

                                if (artigopatentedp.coautoresPatente.Find(x => (x.nome == autor.nome) && (x.ordem_altoria == autor.ordem_altoria)) == null)
                                    artigopatentedp.coautoresPatente.Add(autor);
                            }

                            uploadXML.artigopatentedp.Add(artigopatentedp);

                        }
                        else if(tempo && tipopatente.Contains("concess"))
                        {
                            Patente artigopatentepc = new Patente();
                            artigopatentepc.titulo = titulo;
                            artigopatentepc.ano = datainfo;
                            artigopatentepc.pais = pais;
                            artigopatentepc.codigo_registro_patente = codigo_registro_patente;
                            artigopatentepc.coautoresPatente = new List<Autor>();
                            foreach (XElement au in autores)
                            {
                                Autor autor = new Autor();
                                autor.nome = au.Attribute("NOME-COMPLETO-DO-AUTOR").Value.Trim();
                                autor.ordem_altoria = au.Attribute("ORDEM-DE-AUTORIA").Value.Trim();

                                if (artigopatentepc.coautoresPatente.Find(x => (x.nome == autor.nome) && (x.ordem_altoria == autor.ordem_altoria)) == null)
                                    artigopatentepc.coautoresPatente.Add(autor);
                            }


                            uploadXML.artigopatentepc.Add(artigopatentepc);
                        }
                       
                    }

                }    



                #endregion

                    return uploadXML;
            }
            catch(Exception)
            {
                throw;
            }
        }

        public SolucaoMecanica cadastrarProfessorPosgraduacao(double indiceh , List<string> listdpregistro, List<string> listpcregistro, List<string> listissn, UploadXML uploadXML)
        {
            try
            {
                SolucaoMecanica solucaoMecanica = new SolucaoMecanica();
                
                #region Deletando os artigos que foram deletados pelo usuario
                List<Artigo> artigolist = new List<Artigo>();      
                List<Patente> ptpc = new List<Patente>();
                List<Patente> ptdp = new List<Patente>();
                int tamanho = Math.Max(Math.Max(uploadXML.artigopatentepc.Count(), uploadXML.artigopatentedp.Count()), uploadXML.artigolist.Count());
                for (int i = 0; i < tamanho; i++ )
                {
                    if (i < uploadXML.artigolist.Count())
                        if (listissn.Find(x => (x == uploadXML.artigolist[i].issn)) == null)
                            artigolist.Add(uploadXML.artigolist[i]);

                    if (i< uploadXML.artigopatentepc.Count())
                            if (listpcregistro.Find(x => (x == uploadXML.artigopatentepc[i].codigo_registro_patente)) == null)
                                ptpc.Add(uploadXML.artigopatentepc[i]);
                  
                    if (i < uploadXML.artigopatentedp.Count())
                        if (listdpregistro.Find(x => (x == uploadXML.artigopatentedp[i].codigo_registro_patente)) == null)
                            ptdp.Add(uploadXML.artigopatentedp[i]);
                    
                }
                #endregion

                #region SET A1-A4
                    solucaoMecanica.A1 = 0;
                    solucaoMecanica.A2 = 0;
                    solucaoMecanica.A3 = 0;
                    solucaoMecanica.A4 = 0;
                     
                    foreach (Artigo artigo in artigolist)
                    {
                        string issn = artigo.issn.Trim().Substring(0, 4) + "-" + artigo.issn.Trim().Substring(4);
                        
                        string  query = $"select estrato from publicadas_engenharias_lll_2017_2020 where issn like '%{issn}%';";
                                       
                        _contextSPCPP.GetConnection();

                        string result = _contextSPCPP.Connection.QueryFirstOrDefault<string>(query);
                        SolucaoMecanica solucaoMecanicaCautores = new SolucaoMecanica();
                        switch (result)
                        {
                            case "A1":
                                {
                                    int quantidade = quantidadeAutor(artigo.coautoresArtigo,uploadXML.posgraduacao_id);

                                    if (quantidade > 1)
                                        UpdateCoautor(artigo.coautoresArtigo, uploadXML.posgraduacao_id, quantidade, "A1");
                                
                                    solucaoMecanica.A1 += ((double)1 / quantidade);

                                }
                                break;
                            case "A2":
                                {
                                    int quantidade = quantidadeAutor(artigo.coautoresArtigo, uploadXML.posgraduacao_id);

                                    if (quantidade > 1)
                                        UpdateCoautor(artigo.coautoresArtigo, uploadXML.posgraduacao_id, quantidade, "A2");

                                    solucaoMecanica.A2 += ((double)1 / quantidade);
                                }
                                break;
                            case "A3":
                                {
                                    int quantidade = quantidadeAutor(artigo.coautoresArtigo, uploadXML.posgraduacao_id);

                                    if (quantidade > 1)
                                        UpdateCoautor(artigo.coautoresArtigo, uploadXML.posgraduacao_id, quantidade, "A3");

                                    solucaoMecanica.A3 += ((double)1 / quantidade);
                                }
                                break;
                            case "A4":
                                {
                                    int quantidade = quantidadeAutor(artigo.coautoresArtigo, uploadXML.posgraduacao_id);

                                    if (quantidade > 1)
                                        UpdateCoautor(artigo.coautoresArtigo, uploadXML.posgraduacao_id, quantidade, "A4");

                                    solucaoMecanica.A4 += ((double)1 / quantidade);
                                }
                                break;
                        }
                    }
                #endregion

                #region PATENTES
                tamanho = Math.Max(ptpc.Count(), ptdp.Count());
                for (int i=0; i < tamanho; i++ )
                {
                    if(i < ptpc.Count())
                    {
                        int quantidade = quantidadeAutor(ptpc[i].coautoresPatente, uploadXML.posgraduacao_id);

                        if (quantidade > 1)
                            UpdateCoautor(ptpc[i].coautoresPatente, uploadXML.posgraduacao_id, quantidade, "PC");

                        solucaoMecanica.PC += ((double)1 / quantidade);
                    }
                    if(i < ptdp.Count())
                    {
                        int quantidade = quantidadeAutor(ptdp[i].coautoresPatente, uploadXML.posgraduacao_id);

                        if (quantidade > 1)
                            UpdateCoautor(ptdp[i].coautoresPatente, uploadXML.posgraduacao_id, quantidade, "DP");

                        solucaoMecanica.DP += ((double)1 / quantidade);
                    }
                }

                #endregion
                solucaoMecanica.indiceH = indiceh;
                solucaoMecanica.PQ = uploadXML.pq ? 1 :0;
                solucaoMecanica.nota = NotaMecanica(solucaoMecanica);
                return solucaoMecanica;

            }
            catch(Exception )
            {
                throw;
            }
            
        }

        public int quantidadeAutor(List<Autor> listaAutor, ulong posgraduacao_id)
        {
            try
            {
                int quantidade = 1;

                _contextSPCPP.GetConnection();
                foreach (Autor at in listaAutor)
                {
                    try
                    {
                        string[] nomeCoautor = at.nome.Split(" ");
                        string query = @$"select count(1) 
                                                         from posgraduacao_professor pp 
                                                         inner join professor p on pp.professor_id = p.user_id 
                                                         where p.Cnome like '%{nomeCoautor[0]}%' 
                                                         and p.Cnome like '%{nomeCoautor[1]}%' 
                                                         and p.Cnome like '%{nomeCoautor[nomeCoautor.Count() - 1]}%' 
                                                         and pp.posgraduacao_id = {posgraduacao_id};";
                        int existencia = _contextSPCPP.Connection.QueryFirstOrDefault<int>(query);
                        if (existencia != 0)
                            quantidade += 1;
                    }
                    catch (Exception e)
                    {
                        Console.Write("FALHA NOME ->>>>>>>>>>>" + at.nome);
                    }
                }
                _contextSPCPP.Close();
                return quantidade;
            }
            catch(Exception e)
            {
                throw e;
            }
        }

        public void UpdateCoautor(List<Autor> listaAutor, ulong posgraduacao_id, int quantidade, string tipo)
        {
            try { 

                SolucaoMecanica solucaoMecanicaCautores = new SolucaoMecanica();
                _contextSPCPP.GetConnection();
                foreach (Autor at in listaAutor)
                {
                    string[] nomeCoautor = at.nome.Split(" ");
                    string query = @$"select pp.* 
                                                              from posgraduacao_professor pp 
                                                              inner join professor p on pp.professor_id = p.user_id 
                                                              where p.Cnome like '%{nomeCoautor[0]}%' 
                                                              and p.Cnome like '%{nomeCoautor[1]}%' 
                                                              and p.Cnome like '%{nomeCoautor[nomeCoautor.Count() - 1]}%' 
                                                              and pp.posgraduacao_id = {posgraduacao_id};";
                    Posgraduacao_Professor posgraduacao_Professor = _contextSPCPP.Connection.QueryFirstOrDefault<Posgraduacao_Professor>(query);
                    if (posgraduacao_Professor != null)
                    {
                        if(tipo == "A1")
                            posgraduacao_Professor.A1 = Math.Abs(posgraduacao_Professor.A1 - ((double)1 / quantidade));
                        else if (tipo == "A2")
                            posgraduacao_Professor.A2 = Math.Abs(posgraduacao_Professor.A2 - ((double)1 / quantidade));
                        else if (tipo == "A3")
                            posgraduacao_Professor.A3 = Math.Abs(posgraduacao_Professor.A3 - ((double)1 / quantidade));
                        else if (tipo == "A4")
                            posgraduacao_Professor.A4 = Math.Abs(posgraduacao_Professor.A4 - ((double)1 / quantidade));
                        else if(tipo == "PC")
                            posgraduacao_Professor.PC = Math.Abs(posgraduacao_Professor.PC - ((double)1 / quantidade));
                        else if (tipo == "DP")
                            posgraduacao_Professor.DP = Math.Abs(posgraduacao_Professor.DP - ((double)1 / quantidade));

                        solucaoMecanicaCautores.A1 = posgraduacao_Professor.A1;
                        solucaoMecanicaCautores.A2 = posgraduacao_Professor.A2;
                        solucaoMecanicaCautores.A3 = posgraduacao_Professor.A3;
                        solucaoMecanicaCautores.A4 = posgraduacao_Professor.A4;
                        solucaoMecanicaCautores.DP = posgraduacao_Professor.DP;
                        solucaoMecanicaCautores.PQ = posgraduacao_Professor.PQ;
                        solucaoMecanicaCautores.PC = posgraduacao_Professor.PC;
                        solucaoMecanicaCautores.indiceH = posgraduacao_Professor.indiceH;
                        string nota = NotaMecanica(solucaoMecanicaCautores).ToString().Replace(",", ".");

                        if (tipo == "A1")
                            query = $@"UPDATE posgraduacao_professor SET DataAtualizacao = now() , nota = {nota}, a1 = {posgraduacao_Professor.A1.ToString().Replace(",", ".")} WHERE id = {posgraduacao_Professor.id}";
                        else if (tipo == "A2")
                            query = $@"UPDATE posgraduacao_professor SET DataAtualizacao = now() , nota = {nota}, a2 = {posgraduacao_Professor.A2.ToString().Replace(",", ".")} WHERE id = {posgraduacao_Professor.id}";
                        else if (tipo == "A3")
                            query = $@"UPDATE posgraduacao_professor SET DataAtualizacao = now() , nota = {nota}, a3 = {posgraduacao_Professor.A3.ToString().Replace(",", ".")} WHERE id = {posgraduacao_Professor.id}";
                        else if (tipo == "A4")
                            query = $@"UPDATE posgraduacao_professor SET DataAtualizacao = now() , nota = {nota}, a4 = {posgraduacao_Professor.A4.ToString().Replace(",", ".")} WHERE id = {posgraduacao_Professor.id}";
                        else if (tipo == "PC")
                            query = $@"UPDATE posgraduacao_professor SET DataAtualizacao = now() , nota = {nota}, pc = {posgraduacao_Professor.PC.ToString().Replace(",", ".")} WHERE id = {posgraduacao_Professor.id}";
                        else if (tipo == "DP")
                            query = $@"UPDATE posgraduacao_professor SET DataAtualizacao = now() , nota = {nota}, dp = {posgraduacao_Professor.DP.ToString().Replace(",", ".")} WHERE id = {posgraduacao_Professor.id}";

                        var salvarCoautor = _contextSPCPP.Connection.Execute(query);
                        if (salvarCoautor == 0)
                            throw new Exception($"{tipo} - Erro ao salvar Coautor");
                    }
                }
                _contextSPCPP.Close();
            }
            catch(Exception)
            {
                throw;
            }

        }

        #region Calcular nota
        public SolucaoMecanica calcularNota(XElement root, double indiceh, string nome, ulong posgraducao_id)
        {
            string erro = string.Empty;
            try
            {
               
                SolucaoMecanica solucaoMecanica = new SolucaoMecanica();

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

                if (!nome.Contains(primeironome.ToLower()) && !nome.Contains(ultimonome.ToLower()) && !nome.Contains(name[1].ToLower()))
                        throw new Exception("O Campo nome completo do XML não condiz com o perfil logado!");

                //string url = "https://www.scopus.com/results/authorNamesList.uri?sort=count-f&src=al&sid=2a8ed64d9cd0a572c1e221fa218d9633&sot=al&sdt=al&sl=45&s=AUTHLASTNAME%28ultimo_nome%29+AND+AUTHFIRST%28primeiro_nome%29&st1=ultimo_nome&st2=primeiro_nome&orcidId=&selectionPageSearch=anl&reselectAuthor=false&activeFlag=true&showDocument=false&resultsPerPage=20&offset=1&jtp=false&currentPage=1&previousSelectionCount=0&tooManySelections=false&previousResultCount=0&authSubject=LFSC&authSubject=HLSC&authSubject=PHSC&authSubject=SOSC&exactAuthorSearch=false&showFullList=false&authorPreferredName=&origin=searchauthorfreelookup&affiliationId=&txGid=0f6d3b1681be8dbfe4f429c0e2dbe0ea";
                //url = url.Replace("primeiro_nome", primeironome);
                //url = url.Replace("ultimo_nome", ultimonome);

                //var response = CallUrl(url).Result;
                //double h_index = Convert.ToDouble(response.Substring(response.IndexOf("dataCol4 alignRight\">") + 22, 3));




                #endregion

                #region GET A1-A4
                string query = string.Empty;

                IEnumerable<XElement> artigo_publicado = root.Descendants("ARTIGO-PUBLICADO");
                List<XElement> artigos_publicados_sem_repeticao = new List<XElement>();
                List<string> issnStrings = new List<string>();

                foreach(XElement artigo in artigo_publicado)
                {
                    bool ano =  Convert.ToInt32(artigo.Element("DADOS-BASICOS-DO-ARTIGO").Attribute("ANO-DO-ARTIGO").Value.Trim()) >= DateTime.Now.Year - 4;
                    
                    string issn = artigo.Element("DETALHAMENTO-DO-ARTIGO").Attribute("ISSN").Value.Trim();
                    if (ano && !string.IsNullOrEmpty(issn) && !issnStrings.Contains(issn))
                    {
                        issnStrings.Add(issn);
                        artigos_publicados_sem_repeticao.Add(artigo);
                    }
                    
                }

                foreach (XElement artigo in artigos_publicados_sem_repeticao)
                {
                    string issn = artigo.Element("DETALHAMENTO-DO-ARTIGO").Attribute("ISSN").Value.Trim();
                    if (!string.IsNullOrEmpty(issn))
                    {
                        issn = issn.Substring(0, 4) + "-" + issn.Substring(4);
                        query = $"select estrato from publicadas_engenharias_lll_2017_2020 where issn like '%{issn}%';";
                    }
                    //else
                    //{
                    //    string titulo = artigo.Element("DETALHAMENTO-DO-ARTIGO").Attribute("TITULO-DO-PERIODICO-OU-REVISTA").Value.Trim();
                    //    if (!string.IsNullOrEmpty(titulo))
                    //    {
                    //        query = $"select estrato from publicadas_engenharias_lll_2017_2020 where titulo like '%{titulo}%';";
                    //    }
                    //}
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

                                    foreach (string at in listaAutores(autores))
                                    {
                                        string[] nomeCoautor = at.Split(" ");
                                        query = $"select count(1) from posgraduacao_professor pp inner join professor p on pp.professor_id = p.user_id where p.Cnome like '%{nomeCoautor[0]}%' and p.Cnome like '%{nomeCoautor[1]}%' and p.Cnome like '%{nomeCoautor[nomeCoautor.Count() - 1]}%' and pp.posgraduacao_id = {posgraducao_id};";
                                        int existencia = _contextSPCPP.Connection.QueryFirstOrDefault<int>(query);
                                        if (existencia != 0)
                                            quantidade += 1;

                                    }
                                    if (quantidade > 1)
                                    {
                                        foreach (string at in listaAutores(autores))
                                        {
                                            string[] nomeCoautor = at.Split(" ");
                                            query = $"select pp.* from posgraduacao_professor pp inner join professor p on pp.professor_id = p.user_id where p.Cnome like '%{nomeCoautor[0]}%' and p.Cnome like '%{nomeCoautor[1]}%' and p.Cnome like '%{nomeCoautor[nomeCoautor.Count() - 1]}%' and pp.posgraduacao_id = {posgraducao_id};";
                                            Posgraduacao_Professor posgraduacao_Professor = _contextSPCPP.Connection.QueryFirstOrDefault<Posgraduacao_Professor>(query);
                                            if (posgraduacao_Professor != null)
                                            {
                                                posgraduacao_Professor.A1 = Math.Abs(posgraduacao_Professor.A1 - ((double)1 / quantidade));
                                                solucaoMecanicaCautores.A1 = posgraduacao_Professor.A1;
                                                solucaoMecanicaCautores.A2 = posgraduacao_Professor.A2;
                                                solucaoMecanicaCautores.A3 = posgraduacao_Professor.A3;
                                                solucaoMecanicaCautores.A4 = posgraduacao_Professor.A4;
                                                solucaoMecanicaCautores.DP = posgraduacao_Professor.DP;
                                                solucaoMecanicaCautores.PQ = posgraduacao_Professor.PQ;
                                                solucaoMecanicaCautores.PC = posgraduacao_Professor.PC;
                                                solucaoMecanicaCautores.indiceH = posgraduacao_Professor.indiceH;
                                                string nota = NotaMecanica(solucaoMecanicaCautores).ToString().Replace(",", ".");
                                                query = $@"UPDATE posgraduacao_professor SET DataAtualizacao = now() , nota = {nota}, a1 = {posgraduacao_Professor.A1.ToString().Replace(",", ".")} WHERE id = {posgraduacao_Professor.id}";
                                                var salvarCoautor = _contextSPCPP.Connection.Execute(query);
                                                if (salvarCoautor == 0)
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

                                    foreach (string at in listaAutores(autores))
                                    {
                                        string[] nomeCoautor = at.Split(" ");
                                        query = $"select count(1) from posgraduacao_professor pp inner join professor p on pp.professor_id = p.user_id where p.Cnome like '%{nomeCoautor[0]}%' and p.Cnome like '%{nomeCoautor[1]}%' and p.Cnome like '%{nomeCoautor[nomeCoautor.Count() - 1]}%' and pp.posgraduacao_id = {posgraducao_id};";
                                        int existencia = _contextSPCPP.Connection.QueryFirstOrDefault<int>(query);
                                        if (existencia != 0)
                                            quantidade += 1;

                                    }
                                    if (quantidade > 1)
                                    {
                                        foreach (string at in listaAutores(autores))
                                        {
                                            string[] nomeCoautor = at.Split(" ");
                                            query = $"select pp.* from posgraduacao_professor pp inner join professor p on pp.professor_id = p.user_id where p.Cnome like '%{nomeCoautor[0]}%' and p.Cnome like '%{nomeCoautor[1]}%' and p.Cnome like '%{nomeCoautor[nomeCoautor.Count() - 1]}%' and pp.posgraduacao_id = {posgraducao_id};";
                                            Posgraduacao_Professor posgraduacao_Professor = _contextSPCPP.Connection.QueryFirstOrDefault<Posgraduacao_Professor>(query);
                                            if (posgraduacao_Professor != null)
                                            {
                                                posgraduacao_Professor.A2 = Math.Abs(posgraduacao_Professor.A2 - ((double)1 / quantidade));
                                                solucaoMecanicaCautores.A1 = posgraduacao_Professor.A1;
                                                solucaoMecanicaCautores.A2 = posgraduacao_Professor.A2;
                                                solucaoMecanicaCautores.A3 = posgraduacao_Professor.A3;
                                                solucaoMecanicaCautores.A4 = posgraduacao_Professor.A4;
                                                solucaoMecanicaCautores.DP = posgraduacao_Professor.DP;
                                                solucaoMecanicaCautores.PQ = posgraduacao_Professor.PQ;
                                                solucaoMecanicaCautores.PC = posgraduacao_Professor.PC;
                                                solucaoMecanicaCautores.indiceH = posgraduacao_Professor.indiceH;
                                                string nota = NotaMecanica(solucaoMecanicaCautores).ToString().Replace(",", ".");
                                                query = $@"UPDATE posgraduacao_professor SET DataAtualizacao = now() , nota = {nota}, a2 = {posgraduacao_Professor.A2.ToString().Replace(",", ".")} WHERE id = {posgraduacao_Professor.id}";
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

                                    foreach (string at in listaAutores(autores))
                                    {
                                        string[] nomeCoautor = at.Split(" ");
                                        query = $"select count(1) from posgraduacao_professor pp inner join professor p on pp.professor_id = p.user_id where p.Cnome like '%{nomeCoautor[0]}%' and p.Cnome like '%{nomeCoautor[1]}%' and p.Cnome like '%{nomeCoautor[nomeCoautor.Count() - 1]}%' and pp.posgraduacao_id = {posgraducao_id};";
                                        int existencia = _contextSPCPP.Connection.QueryFirstOrDefault<int>(query);
                                        if (existencia != 0)
                                            quantidade += 1;

                                    }
                                    if (quantidade > 1)
                                    {
                                        foreach (string at in listaAutores(autores))
                                        {
                                            string[] nomeCoautor = at.Split(" ");
                                            query = $"select pp.* from posgraduacao_professor pp inner join professor p on pp.professor_id = p.user_id where p.Cnome like '%{nomeCoautor[0]}%' and p.Cnome like '%{nomeCoautor[1]}%' and p.Cnome like '%{nomeCoautor[nomeCoautor.Count() - 1]}%' and pp.posgraduacao_id = {posgraducao_id};";
                                            Posgraduacao_Professor posgraduacao_Professor = _contextSPCPP.Connection.QueryFirstOrDefault<Posgraduacao_Professor>(query);
                                            if (posgraduacao_Professor != null)
                                            {
                                                posgraduacao_Professor.A3 = Math.Abs(posgraduacao_Professor.A3 - ((double)1 / quantidade));
                                                solucaoMecanicaCautores.A1 = posgraduacao_Professor.A1;
                                                solucaoMecanicaCautores.A2 = posgraduacao_Professor.A2;
                                                solucaoMecanicaCautores.A3 = posgraduacao_Professor.A3;
                                                solucaoMecanicaCautores.A4 = posgraduacao_Professor.A4;
                                                solucaoMecanicaCautores.DP = posgraduacao_Professor.DP;
                                                solucaoMecanicaCautores.PQ = posgraduacao_Professor.PQ;
                                                solucaoMecanicaCautores.PC = posgraduacao_Professor.PC;
                                                solucaoMecanicaCautores.indiceH = posgraduacao_Professor.indiceH;
                                                string nota = NotaMecanica(solucaoMecanicaCautores).ToString().Replace(",", ".");
                                                query = $@"UPDATE posgraduacao_professor SET  DataAtualizacao = now() , nota = {nota}, a3 = {posgraduacao_Professor.A3.ToString().Replace(",", ".")} WHERE id = {posgraduacao_Professor.id}";
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

                                    foreach (string at in listaAutores(autores))
                                    {
                                        string[] nomeCoautor = at.Split(" ");
                                        query = $"select count(1) from posgraduacao_professor pp inner join professor p on pp.professor_id = p.user_id where p.Cnome like '%{nomeCoautor[0]}%' and p.Cnome like '%{nomeCoautor[1]}%' and p.Cnome like '%{nomeCoautor[nomeCoautor.Count() - 1]}%' and pp.posgraduacao_id = {posgraducao_id};";
                                        int existencia = _contextSPCPP.Connection.QueryFirstOrDefault<int>(query);
                                        if (existencia != 0)
                                            quantidade += 1;

                                    }
                                    if (quantidade > 1)
                                    {
                                        foreach (string at in listaAutores(autores))
                                        {
                                            string[] nomeCoautor = at.Split(" ");
                                            query = $"select pp.* from posgraduacao_professor pp inner join professor p on pp.professor_id = p.user_id where p.Cnome like '%{nomeCoautor[0]}%' and p.Cnome like '%{nomeCoautor[1]}%' and p.Cnome like '%{nomeCoautor[nomeCoautor.Count() - 1]}%' and pp.posgraduacao_id = {posgraducao_id};";
                                            Posgraduacao_Professor posgraduacao_Professor = _contextSPCPP.Connection.QueryFirstOrDefault<Posgraduacao_Professor>(query);
                                            if (posgraduacao_Professor != null)
                                            {
                                                posgraduacao_Professor.A4 = Math.Abs(posgraduacao_Professor.A4 - ((double)1 / quantidade));
                                                solucaoMecanicaCautores.A1 = posgraduacao_Professor.A1;
                                                solucaoMecanicaCautores.A2 = posgraduacao_Professor.A2;
                                                solucaoMecanicaCautores.A3 = posgraduacao_Professor.A3;
                                                solucaoMecanicaCautores.A4 = posgraduacao_Professor.A4;
                                                solucaoMecanicaCautores.DP = posgraduacao_Professor.DP;
                                                solucaoMecanicaCautores.PQ = posgraduacao_Professor.PQ;
                                                solucaoMecanicaCautores.PC = posgraduacao_Professor.PC;
                                                solucaoMecanicaCautores.indiceH = posgraduacao_Professor.indiceH;
                                                string nota = NotaMecanica(solucaoMecanicaCautores).ToString().Replace(",", ".");
                                                query = $@"UPDATE posgraduacao_professor SET DataAtualizacao = now() , nota = {nota}, a4 = {posgraduacao_Professor.A4.ToString().Replace(",", ".")} WHERE id = {posgraduacao_Professor.id}";
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
                    

                    if (tempo && deposito)
                    {

                        int quantidade = 1;

                        var autores = pt.Elements("AUTORES");

                        foreach (string at in listaAutores(autores))
                        {
                            string[] nomeCoautor = at.Split(" ");
                            query = $"select count(1) from posgraduacao_professor pp inner join professor p on pp.professor_id = p.user_id where p.Cnome like '%{nomeCoautor[0]}%' and p.Cnome like '%{nomeCoautor[1]}%' and p.Cnome like '%{nomeCoautor[nomeCoautor.Count() - 1]}%' and pp.posgraduacao_id = {posgraducao_id};";
                            int existencia = _contextSPCPP.Connection.QueryFirstOrDefault<int>(query);
                            if (existencia != 0)
                                quantidade += 1;

                        }
                        if (quantidade > 1)
                        {
                            foreach (string at in listaAutores(autores))
                            {

                                string[] nomeCoautor = at.Split(" ");

                                query = $"select pp.* from posgraduacao_professor pp inner join professor p on pp.professor_id = p.user_id where p.Cnome like '%{nomeCoautor[0]}%' and p.Cnome like '%{nomeCoautor[1]}%' and p.Cnome like '%{nomeCoautor[nomeCoautor.Count() - 1]}%' and pp.posgraduacao_id = {posgraducao_id};";
                                Posgraduacao_Professor posgraduacao_Professor = _contextSPCPP.Connection.QueryFirstOrDefault<Posgraduacao_Professor>(query);
                                if (posgraduacao_Professor != null)
                                {
                                    SolucaoMecanica solucaoMecanicaCautores = new SolucaoMecanica();

                                    posgraduacao_Professor.DP = Math.Abs (posgraduacao_Professor.DP - ((double)1 / quantidade));
                                    solucaoMecanicaCautores.A1 = posgraduacao_Professor.A1;
                                    solucaoMecanicaCautores.A2 = posgraduacao_Professor.A2;
                                    solucaoMecanicaCautores.A3 = posgraduacao_Professor.A3;
                                    solucaoMecanicaCautores.A4 = posgraduacao_Professor.A4;
                                    solucaoMecanicaCautores.DP = posgraduacao_Professor.DP;
                                    solucaoMecanicaCautores.PQ = posgraduacao_Professor.PQ;
                                    solucaoMecanicaCautores.PC = posgraduacao_Professor.PC;
                                    solucaoMecanicaCautores.indiceH = posgraduacao_Professor.indiceH;
                                    string nota = NotaMecanica(solucaoMecanicaCautores).ToString().Replace(",", ".");
                                    query = $@"UPDATE posgraduacao_professor SET DataAtualizacao = now() , nota = {nota}, dp = {posgraduacao_Professor.DP.ToString().Replace(",", ".")} WHERE id = {posgraduacao_Professor.id}";
                                    var salvarCoautor = _contextSPCPP.Connection.Execute(query);
                                    if (salvarCoautor == 0)
                                        throw new Exception("DP - Erro ao salvar Coautor");

                                }

                            }
                        }

                        dp+= (double)1 / quantidade;
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
                    
                    if (tempo && concedidas)
                    {

                        int quantidade = 1;

                        var autores = pt.Elements("AUTORES");

                        foreach (string at in listaAutores(autores))
                        {
                            string[] nomeCoautor = at.Split(" ");
                            query = $"select count(1) from posgraduacao_professor pp inner join professor p on pp.professor_id = p.user_id where p.Cnome like '%{nomeCoautor[0]}%' and p.Cnome like '%{nomeCoautor[1]}%' and p.Cnome like '%{nomeCoautor[nomeCoautor.Count() - 1]}%' and pp.posgraduacao_id = {posgraducao_id};";
                            int existencia = _contextSPCPP.Connection.QueryFirstOrDefault<int>(query);
                            if (existencia != 0)
                                quantidade += 1;

                        }
                        if (quantidade > 1)
                        {
                            foreach (string at in listaAutores(autores))
                            {

                                string[] nomeCoautor = at.Split(" ");
                                query = $"select pp.* from posgraduacao_professor pp inner join professor p on pp.professor_id = p.user_id where p.Cnome like '%{nomeCoautor[0]}%' and p.Cnome like '%{nomeCoautor[1]}%' and p.Cnome like '%{nomeCoautor[nomeCoautor.Count() - 1]}%' and pp.posgraduacao_id = {posgraducao_id};";
                                Posgraduacao_Professor posgraduacao_Professor = _contextSPCPP.Connection.QueryFirstOrDefault<Posgraduacao_Professor>(query);
                                if (posgraduacao_Professor != null)
                                {
                                    SolucaoMecanica solucaoMecanicaCautores = new SolucaoMecanica();

                                    posgraduacao_Professor.PC = Math.Abs(posgraduacao_Professor.PC - ((double)1 / quantidade));
                                    solucaoMecanicaCautores.A1 = posgraduacao_Professor.A1;
                                    solucaoMecanicaCautores.A2 = posgraduacao_Professor.A2;
                                    solucaoMecanicaCautores.A3 = posgraduacao_Professor.A3;
                                    solucaoMecanicaCautores.A4 = posgraduacao_Professor.A4;
                                    solucaoMecanicaCautores.DP = posgraduacao_Professor.DP;
                                    solucaoMecanicaCautores.PQ = posgraduacao_Professor.PQ;
                                    solucaoMecanicaCautores.PC = posgraduacao_Professor.PC;
                                    solucaoMecanicaCautores.indiceH = posgraduacao_Professor.indiceH;
                                    string nota = NotaMecanica(solucaoMecanicaCautores).ToString().Replace(",", ".");
                                    query = $@"UPDATE posgraduacao_professor SET DataAtualizacao = now() , nota = {nota}, pc = {posgraduacao_Professor.PC.ToString().Replace(",", ".")} WHERE id = {posgraduacao_Professor.id}";
                                    var salvarCoautor = _contextSPCPP.Connection.Execute(query);
                                    if (salvarCoautor == 0)
                                        throw new Exception("PC - Erro ao salvar Coautor");

                                }
                            }
                        }

                        pc+= (double)1 / quantidade;
                    }
                }
                #endregion

                #region GET PQ
                //A bolsa de produtividade em pesquisa(PQ) é verificada no currículo LATTES.Esse parâmetro
                //é binário: para bolsista o parâmetro é atribuído o valor 1 e não bolsista 0.Caso o professor
                //tenha sido bolsista no período do último quadriênio, mas perdeu sua bolsa, será atribuído
                //valor 1 ao parâmetro PQ.
                int pq = 0;
                IEnumerable<XElement> pp = root.Descendants("DADOS-GERAIS");
                foreach (XElement dados in pp)
                {
                    var result = dados.Element("OUTRAS-INFORMACOES-RELEVANTES").Attribute("OUTRAS-INFORMACOES-RELEVANTES").Value.ToLower().Contains("bolsista de produtividade");
                    if (result)
                    {
                        pq = 1;
                        break;
                    }

                }
                #endregion

                solucaoMecanica.DP = dp;
                solucaoMecanica.PC = pc;
                solucaoMecanica.PQ = pq;
                solucaoMecanica.indiceH = indiceh;
                solucaoMecanica.nota = NotaMecanica(solucaoMecanica);
                return solucaoMecanica;
            }

            catch (Exception e)
            {
                if (e.Message.Contains("no elements"))
                    throw new Exception($"Atenção o arquivo XML esta errado, não contém {erro}!");
                else
                    throw new Exception($"Erro ao gerar nota, {e.Message}");
            }
            
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

            return Math.Round(PD, 3);
        }

        public List<string> listaAutores(IEnumerable<XContainer> autores)
        {
            List<string> lista = new List<string>();
            foreach (XElement at in autores)
            {
                int flag = 0;
                string nome = at.Attribute("NOME-COMPLETO-DO-AUTOR").Value.Replace(",", " ").Replace("'", " ").Trim();
                string[] nomesplit = nome.Split(" ");
                foreach (string s in lista)
                    if (s.Contains(nomesplit[0]) && s.Contains(nomesplit[1]) && s.Contains(nomesplit[nomesplit.Count() - 1]))
                        flag = 1;
                if(flag == 0)
                     lista.Add(nome);
            }

            return lista;
        }


        #endregion

        public ProfessorCadastrado SalvarStatus(ulong professor_id, ulong posgraduacao_id, string status)
        {
            try
            {
                _contextSPCPP.GetConnection();

                string sql = $@"UPDATE posgraduacao_professor SET status = '{status}', DataAtualizacao = now() WHERE (professor_id = {professor_id} and posgraduacao_id = {posgraduacao_id});";

                _contextSPCPP.Connection.Execute(sql);

                sql = $@"select  p.user_id,p.siape , p.Cnome , p.Email ,pp.posgraduacao_id,p.Data_nasc , pp.DataCadastro, pp.status, pp.nota
		                                from professor p 
		                                right join posgraduacao_professor pp on p.user_id = pp.professor_id 
		                                left join  usuario u on u.id=pp.professor_id 
		                                where pp.posgraduacao_id = {posgraduacao_id} and pp.professor_id = {professor_id};";

                ProfessorCadastrado result = (_contextSPCPP.Connection.QueryFirstOrDefault<ProfessorCadastrado>(sql));
      
                return result;
            }
            catch(Exception)
            {
                throw;
            }
        }
    }
}

using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Script.Serialization;
using System.Web.Script.Services;
using System.Web.Services;

/// <summary>
/// Summary description for WebServiceTipoRelato
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class WebServiceTipoRelato : System.Web.Services.WebService
{

    //Conexão com o banco de dados MySql
    private static MySqlConnection conexaoBD = new MySqlConnection(ConfigurationManager.ConnectionStrings["bancoDados"].ConnectionString);

    /*
     * String de conexão sem utilizar o Connection String do Web.config
     * static MySqlConnection conexaoBD = new MySqlConnection("Server=ESN509VMYSQL;Database=3dsa_tcc_grupoc;Uid=aluno;Pwd=Senai1234;SslMode=none");
    */

    //Nome da tabela do banco de dados MySql
    private static string NOME_TABELA = "tb_tipoRelatos";

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void cadastraTipoRelato(string nome)
    {
        Context.Response.ContentType = "application/json; charset=utf-8;";

        try
        {
            MySqlCommand comando =
                new MySqlCommand("INSERT INTO " + NOME_TABELA + " VALUES (null,@nome_tipoRelato);", conexaoBD);

            comando.Parameters.AddWithValue("@nome_tipoRelato", nome);

            conexaoBD.Open();

            if (comando.ExecuteNonQuery() != 0)
            {
                Context.Response.Write(new JavaScriptSerializer().Serialize("TipoRelato Cadastrado"));
            }
            else
            {
                Context.Response.Write(new JavaScriptSerializer().Serialize("Erro ao cadastrar"));
            }
        }
        catch (Exception e)
        {
            Context.Response.Write(new JavaScriptSerializer().Serialize("Ocorreu um erro inesperado ao conectar a Base de Dados\n" + e.Message));
        }
        finally
        {
            //Fecha a conexão com o BD
            conexaoBD.Close();
        }
    }


    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void buscaTipoRelato(int idTipoRelato)
    {
        Context.Response.ContentType = "application/json; charset=utf-8;";

        try
        {
            //Passagem de qual comando será executado na conexão
            MySqlCommand comando = new MySqlCommand("SELECT id_tipoRelato, nome_tipoRelato FROM " + NOME_TABELA + " WHERE id_tipoRelato = @idTipoRelato;", conexaoBD);

            //Informando qual o valor de cada parâmetro, ou seja, de cada valor com @
            comando.Parameters.AddWithValue("@idTipoRelato", idTipoRelato);

            //Abrindo a conexão com o BD
            conexaoBD.Open();

            //Executando a pesquisa
            MySqlDataReader leitor = comando.ExecuteReader();

            //Caso a pesquisa tenha tuplas (possua um tipoRelato com o ID procurado)
            if (leitor.HasRows)
            {
                //Direciona o ponteira para esta tupla
                leitor.Read();

                //Instancia um objeto de TipoRelato com os valores retornados do BD
                TipoRelato tipoRelato = new TipoRelato
                    (leitor.GetInt32("id_tipoRelato"),
                    leitor.GetString("nome_tipoRelato")
                    );

                //Cria a resposta em JSON enviando o objeto de TipoRelato
                Context.Response.Write(new JavaScriptSerializer().Serialize(tipoRelato));
            }
            //Caso não tenha tuplas (não exista ninguem com este ID)
            else
            {
                //Cria a resposta em JSON enviando uma mensagem informando que não existe um tipoRelato com este id
                Context.Response.Write(new JavaScriptSerializer().Serialize("Não foi possível encontrar um TipoRelato com este ID"));
            }
        }
        catch (Exception e)
        {
            Context.Response.Write(new JavaScriptSerializer().Serialize("Ocorreu um erro inesperado ao conectar a Base de Dados\n" + e.Message));
        }
        finally
        {
            //Fecha a conexão com o BD
            conexaoBD.Close();
        }

    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void listaTipoRelato()
    {
        Context.Response.ContentType = "application/json; charset=utf-8;";

        try
        {
            //Passagem de qual comando será executado na conexão
            MySqlCommand comando = new MySqlCommand("SELECT id_tipoRelato, nome_tipoRelato FROM " + NOME_TABELA + ";", conexaoBD);

            //Abre a conexão com o BD
            conexaoBD.Open();

            //Executa a pesquisa
            MySqlDataReader leitor = comando.ExecuteReader();

            //Cria uma ArrayList para guardar todos os tipoRelatos encontrados
            List<TipoRelato> listaTipoRelato = new List<TipoRelato>();

            //Enquanto possuir resultados da pesquisa (cada tupla)
            while (leitor.Read())
            {
                //Instancia um objeto de TipoRelato com os valores da tupla
                TipoRelato tipoRelato = new TipoRelato(leitor.GetInt32("id_tipoRelato"), leitor.GetString("nome_tipoRelato"));
                //Adiciona este objeto na ArrayList
                listaTipoRelato.Add(tipoRelato);
            }

            //Cria a resposta em JSON enviando a lista de Tipos Relatos
            Context.Response.Write(new JavaScriptSerializer().Serialize(listaTipoRelato));
        }
        catch (Exception e)
        {
            Context.Response.Write(new JavaScriptSerializer().Serialize("Ocorreu um erro inesperado ao conectar a Base de Dados\n" + e.Message));
        }
        finally
        {
            //Fecha a conexão com o BD
            conexaoBD.Close();
        }

    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void atualizaTipoRelato(int ID_TIPORELATO, string NOME_TIPORELATO)
    {
        Context.Response.ContentType = "application/json; charset=utf-8;";

        try
        {
            MySqlCommand comando = new MySqlCommand("UPDATE " + NOME_TABELA + " SET nome_tipoRelato = @nome_tipoRelato WHERE id_tipoRelato = @id_tipoRelato;", conexaoBD);

            comando.Parameters.AddWithValue("@id_tipoRelato", ID_TIPORELATO);
            comando.Parameters.AddWithValue("@nome_tipoRelato", NOME_TIPORELATO);

            conexaoBD.Open();

            if (comando.ExecuteNonQuery() != 0) //Execução do comando e teste para verificar se alterou algo na tabela
            {
                Context.Response.Write(new JavaScriptSerializer().Serialize("Atualizado com Sucesso")); //Enviar algum conteúdo no formato configurado acima
            }
            else
            {
                Context.Response.Write(new JavaScriptSerializer().Serialize("Erro ao Atualizar")); //Enviar algum conteúdo no formato configurado acima
            }
        }
        catch (Exception e)
        {
            Context.Response.Write(new JavaScriptSerializer().Serialize("Ocorreu um erro inesperado ao conectar a Base de Dados\n" + e.Message));
        }
        finally
        {
            //Fecha a conexão com o BD
            conexaoBD.Close();
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void deletaTipoRelato(int id_tipoRelato)
    {
        Context.Response.ContentType = "application/json; charset=utf-8;";

        try
        {
            MySqlCommand comando =
            new MySqlCommand("DELETE FROM " + NOME_TABELA + " WHERE id_tipoRelato = @id_tipoRelato;", conexaoBD);
            comando.Parameters.AddWithValue("@id_tipoRelato", id_tipoRelato);

            conexaoBD.Open();

            if (comando.ExecuteNonQuery() != 0) // Se alterou algo na tabela
            {
                Context.Response.Write(new JavaScriptSerializer().Serialize("Excluido"));
            }
            else
            {
                Context.Response.Write(new JavaScriptSerializer().Serialize("Erro ao excluir"));
            }
        }
        catch (Exception e)
        {
            Context.Response.Write(new JavaScriptSerializer().Serialize("Ocorreu um erro inesperado ao conectar a Base de Dados\n" + e.Message));
        }
        finally
        {
            //Fecha a conexão com o BD
            conexaoBD.Close();
        }
    }

}

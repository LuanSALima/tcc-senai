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
/// Summary description for WebServiceRelato
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class WebServiceRelato : System.Web.Services.WebService
{
    //Conexão com o banco de dados MySql
    private static MySqlConnection conexaoBD = new MySqlConnection(ConfigurationManager.ConnectionStrings["bancoDados"].ConnectionString);

    /*
     * String de conexão sem utilizar o Connection String do Web.config
     * static MySqlConnection conexaoBD = new MySqlConnection("Server=ESN509VMYSQL;Database=3dsa_tcc_grupoc;Uid=aluno;Pwd=Senai1234;SslMode=none");
    */

    //Nome da tabela do banco de dados MySql
    private static string NOME_TABELA = "tb_relatos";

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void cadastraRelato(int idUsuario,int idTipoRelato, string descricao, string localizacao_X, string localizacao_Y, string data, string horario, int anonimo)
    {
        Context.Response.ContentType = "application/json; charset=utf-8;";

        try
        {
            MySqlCommand comando =
                new MySqlCommand("INSERT INTO " + NOME_TABELA + " VALUES (null,@fk_id_usuario,@fk_id_tipoRelato,@descricao_relato,@localizacao_x_relato,@localizacao_y_relato,null,null,null,@data_relato,@horario_relato,@anonimo_relato);", conexaoBD);

            comando.Parameters.AddWithValue("@fk_id_usuario", idUsuario);
            comando.Parameters.AddWithValue("@fk_id_tipoRelato", idTipoRelato);
            comando.Parameters.AddWithValue("@descricao_relato", descricao);
            comando.Parameters.AddWithValue("@localizacao_x_relato", localizacao_X);
            comando.Parameters.AddWithValue("@localizacao_y_relato", localizacao_Y);
            comando.Parameters.AddWithValue("@data_relato", data);
            comando.Parameters.AddWithValue("@horario_relato", horario);
            comando.Parameters.AddWithValue("@anonimo_relato", anonimo);

            conexaoBD.Open();

            if (comando.ExecuteNonQuery() != 0)
            {
                Context.Response.Write(new JavaScriptSerializer().Serialize("Relato Cadastrado"));
            }
            else
            {
                Context.Response.Write(new JavaScriptSerializer().Serialize("Erro ao cadastrar"));
            }
        }
        catch (Exception e)
        {
            Context.Response.Write(new JavaScriptSerializer().Serialize("Ocorreu um erro inesperado ao conectar a Base de Dados\n"+e.Message));
            System.Diagnostics.Debug.Write(e.StackTrace);
        }
        finally
        {
            //Fecha a conexão com o BD
            conexaoBD.Close();
        }
    }


    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void buscaRelato(int idRelato)
    {
        Context.Response.ContentType = "application/json; charset=utf-8;";

        try
        {
            //Passagem de qual comando será executado na conexão
            MySqlCommand comando = new MySqlCommand("SELECT id_relato, fk_id_usuario, fk_id_tipoRelato, descricao_relato, localizacao_x_relato, localizacao_y_relato, data_relato, horario_relato, anonimo_relato FROM " + NOME_TABELA + " WHERE id_relato = @idRelato;", conexaoBD);

            //Informando qual o valor de cada parâmetro, ou seja, de cada valor com @
            comando.Parameters.AddWithValue("@idRelato", idRelato);

            //Abrindo a conexão com o BD
            conexaoBD.Open();

            //Executando a pesquisa
            MySqlDataReader leitor = comando.ExecuteReader();

            //Caso a pesquisa tenha tuplas (possua um relato com o ID procurado)
            if (leitor.HasRows)
            {
                //Direciona o ponteira para esta tupla
                leitor.Read();

                //Instancia um objeto de Relato com os valores retornados do BD
                Relato relato = new Relato
                    (leitor.GetInt32("id_relato"),
                    leitor.GetInt32("fk_id_usuario"),
                    leitor.GetInt32("fk_id_tipoRelato"),
                    leitor.GetString("descricao_relato"),
                    leitor.GetDouble("localizacao_x_relato"),
                    leitor.GetDouble("localizacao_y_relato"),
                    leitor.GetDateTime("data_relato").ToShortDateString(),
                    leitor.GetString("horario_relato"),
                    leitor.GetInt16("anonimo_relato"));

                //Cria a resposta em JSON enviando o objeto de Relato
                Context.Response.Write(new JavaScriptSerializer().Serialize(relato));
            }
            //Caso não tenha tuplas (não exista nenhum relato com este ID)
            else
            {
                //Cria a resposta em JSON enviando uma mensagem informando que não existe um relato com este id
                Context.Response.Write(new JavaScriptSerializer().Serialize("Não foi possível encontrar um Relato com este ID"));
            }
        }
        catch (Exception e)
        {
            Context.Response.Write(new JavaScriptSerializer().Serialize("Ocorreu um erro inesperado ao conectar a Base de Dados\n"+e.Message));
        }
        finally
        {
            //Fecha a conexão com o BD
            conexaoBD.Close();
        }

    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void buscaRelatoUsuario(int idUsuario)
    {
        Context.Response.ContentType = "application/json; charset=utf-8;";

        try
        {
            //Passagem de qual comando será executado na conexão
            MySqlCommand comando = new MySqlCommand("SELECT id_relato, fk_id_usuario, tb_usuarios.nome_usuario, fk_id_tipoRelato, tb_tipoRelatos.nome_tipoRelato, descricao_relato, localizacao_x_relato, localizacao_y_relato, data_relato, horario_relato, anonimo_relato FROM tb_relatos INNER JOIN tb_usuarios ON tb_relatos.fk_id_usuario = tb_usuarios.id_usuario INNER JOIN tb_tipoRelatos ON tb_relatos.fk_id_tipoRelato = tb_tipoRelatos.id_tipoRelato WHERE fk_id_usuario = @idUsuario;", conexaoBD);

            //Informando qual o valor de cada parâmetro, ou seja, de cada valor com @
            comando.Parameters.AddWithValue("@idUsuario", idUsuario);

            //Abrindo a conexão com o BD
            conexaoBD.Open();

            //Executando a pesquisa
            MySqlDataReader leitor = comando.ExecuteReader();

            //Caso não tenha tuplas (não exista nenhum relato cadastrado por este usuario)
            if (!leitor.HasRows)
            {
                //Cria a resposta em JSON enviando uma mensagem informando que não existe um relato com este id
                Context.Response.Write(new JavaScriptSerializer().Serialize("Não possui nenhum relato cadastrado por este Usuário"));
            }
            else
            {
                //Enquanto possuir resultados da pesquisa (cada tupla)
                List<Relato> listaRelatosUsuario = new List<Relato>();

                while (leitor.Read())
                {
                    //Instancia um objeto de Relato com os valores da tupla
                    Relato relato = new Relato(leitor.GetInt32("id_relato"),
                        leitor.GetInt32("fk_id_usuario"),
                        leitor.GetString("nome_usuario"),
                        leitor.GetInt32("fk_id_tipoRelato"),
                        leitor.GetString("nome_tipoRelato"),
                        leitor.GetString("descricao_relato"),
                        leitor.GetDouble("localizacao_x_relato"),
                        leitor.GetDouble("localizacao_y_relato"),
                        leitor.GetDateTime("data_relato").ToShortDateString(),
                        leitor.GetString("horario_relato"),
                        leitor.GetInt16("anonimo_relato"));
                    //Adiciona este objeto na ArrayList
                    listaRelatosUsuario.Add(relato);
                }

                //Cria a resposta em JSON enviando a lista de Relatos
                Context.Response.Write(new JavaScriptSerializer().Serialize(listaRelatosUsuario));
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
    public void listaRelatos()
    {
        Context.Response.ContentType = "application/json; charset=utf-8;";

        try
        {
            //Passagem de qual comando será executado na conexão
            MySqlCommand comando = new MySqlCommand("SELECT id_relato, fk_id_usuario, tb_usuarios.nome_usuario, fk_id_tipoRelato, tb_tipoRelatos.nome_tipoRelato, descricao_relato, localizacao_x_relato, localizacao_y_relato, data_relato, horario_relato, anonimo_relato FROM tb_relatos INNER JOIN tb_usuarios ON tb_relatos.fk_id_usuario = tb_usuarios.id_usuario INNER JOIN tb_tipoRelatos ON tb_relatos.fk_id_tipoRelato = tb_tipoRelatos.id_tipoRelato" + ";", conexaoBD);


            //Abre a conexão com o BD
            conexaoBD.Open();

            //Executa a pesquisa
            MySqlDataReader leitor = comando.ExecuteReader();

            //Cria uma ArrayList para guardar todos os relatos encontrados
            List<Relato> listaRelatos = new List<Relato>();

            //Enquanto possuir resultados da pesquisa (cada tupla)
            while (leitor.Read())
            {
                //Instancia um objeto de Relato com os valores da tupla
                Relato relato = new Relato(leitor.GetInt32("id_relato"),
                    leitor.GetInt32("fk_id_usuario"),
                    leitor.GetString("nome_usuario"),
                    leitor.GetInt32("fk_id_tipoRelato"),
                    leitor.GetString("nome_tipoRelato"),
                    leitor.GetString("descricao_relato"),
                    leitor.GetDouble("localizacao_x_relato"),
                    leitor.GetDouble("localizacao_y_relato"),
                    leitor.GetDateTime("data_relato").ToShortDateString(),
                    leitor.GetString("horario_relato"),
                    leitor.GetInt16("anonimo_relato"));
                //Adiciona este objeto na ArrayList
                listaRelatos.Add(relato);
            }

            //Cria a resposta em JSON enviando a lista de Relatos
            Context.Response.Write(new JavaScriptSerializer().Serialize(listaRelatos));
        }
        catch (Exception e)
        {
            Context.Response.Write(new JavaScriptSerializer().Serialize("Ocorreu um erro inesperado ao conectar a Base de Dados\n"+e.Message));
        }
        finally
        {
            //Fecha a conexão com o BD
            conexaoBD.Close();
        }
    }

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void atualizaRelatos(int ID_RELATO, int FK_ID_USUARIO, int FK_ID_TIPORELATO, string DESCRICAO_RELATO, double LOCALIZACAO_X_RELATO, double LOCALIZACAO_Y_RELATO, string DATA_RELATO, string HORARIO_RELATO, int ANONIMO_RELATO)
    {
        Context.Response.ContentType = "application/json; charset=utf-8;";

        try
        {
            MySqlCommand comando = new MySqlCommand("UPDATE " + NOME_TABELA + " SET fk_id_usuario = @fk_id_usuario, fk_id_tipoRelato = @fk_id_tipoRelato, descricao_relato = @descricao_relato, localizacao_x_relato = @localizacao_x_relato, localizacao_y_relato = @localizacao_y_relato, data_relato = @data_relato, horario_relato = @horario_relato, anonimo_relato = @anonimo_relato WHERE id_relato = @id_relato;", conexaoBD);

            comando.Parameters.AddWithValue("@id_relato", ID_RELATO);
            comando.Parameters.AddWithValue("@fk_id_usuario", FK_ID_USUARIO);
            comando.Parameters.AddWithValue("@fk_id_tipoRelato", FK_ID_TIPORELATO);
            comando.Parameters.AddWithValue("@descricao_relato", DESCRICAO_RELATO);
            comando.Parameters.AddWithValue("@localizacao_x_relato", LOCALIZACAO_X_RELATO);
            comando.Parameters.AddWithValue("@localizacao_y_relato", LOCALIZACAO_Y_RELATO);
            comando.Parameters.AddWithValue("@data_relato", DATA_RELATO);
            comando.Parameters.AddWithValue("@horario_relato", HORARIO_RELATO);
            comando.Parameters.AddWithValue("@anonimo_relato", ANONIMO_RELATO);

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
    public void deletaRelato(int id_relato)
    {
        Context.Response.ContentType = "application/json; charset=utf-8";

        try
        {
            MySqlCommand comando =
                new MySqlCommand("DELETE FROM " + NOME_TABELA + " WHERE id_relato = @id_relato;", conexaoBD);
            comando.Parameters.AddWithValue("@id_relato", id_relato);

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

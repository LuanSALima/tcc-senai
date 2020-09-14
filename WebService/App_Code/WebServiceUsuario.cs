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
/// Summary description for WebServiceUsuario
/// </summary>
[WebService(Namespace = "http://tempuri.org/")]
[WebServiceBinding(ConformsTo = WsiProfiles.BasicProfile1_1)]
// To allow this Web Service to be called from script, using ASP.NET AJAX, uncomment the following line. 
// [System.Web.Script.Services.ScriptService]
public class WebServiceUsuario : System.Web.Services.WebService
{
    //Conexão com o banco de dados MySql
    private static MySqlConnection conexaoBD = new MySqlConnection(ConfigurationManager.ConnectionStrings["bancoDados"].ConnectionString);

    /*
     * String de conexão sem utilizar o Connection String do Web.config
     * static MySqlConnection conexaoBD = new MySqlConnection("Server=ESN509VMYSQL;Database=3dsa_tcc_grupoc;Uid=aluno;Pwd=Senai1234;SslMode=none");
    */

    private static string NOME_TABELA = "tb_usuarios";

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void cadastraUsuario(string nome, string email, string senha)
    {
        Context.Response.ContentType = "application/json; charset=utf-8;";

        string procuraUsuario = existeUsuario(nome, email);

        if (procuraUsuario.Contains("Nenhum"))
        {
            try
            {
                MySqlCommand comando =
                    new MySqlCommand("INSERT INTO " + NOME_TABELA + " VALUES (null,@nome_usuario,@email_usuario,@senha_usuario);", conexaoBD);

                comando.Parameters.AddWithValue("@nome_usuario", nome);
                comando.Parameters.AddWithValue("@email_usuario", email);
                comando.Parameters.AddWithValue("@senha_usuario", senha);

                conexaoBD.Open();

                if (comando.ExecuteNonQuery() != 0)
                {
                    Context.Response.Write(new JavaScriptSerializer().Serialize("Usuario Cadastrado"));
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
        else
        {
            Context.Response.Write(new JavaScriptSerializer().Serialize(procuraUsuario));
        }
    }

    public string existeUsuario(string nome, string email)
    {
        string resposta = "Sem resposta do Servidor";

        try
        {
            MySqlCommand comando = new MySqlCommand("SELECT id_usuario, nome_usuario, email_usuario, senha_usuario FROM " + NOME_TABELA + " WHERE nome_usuario = @nomeUsuario OR email_usuario = @emailUsuario;", conexaoBD);

            comando.Parameters.AddWithValue("@nomeUsuario", nome);
            comando.Parameters.AddWithValue("@emailUsuario", email);

            conexaoBD.Open();

            MySqlDataReader leitor = comando.ExecuteReader();

            if (!leitor.HasRows)
            {
                resposta = "Nenhum usuário cadastrado";
            }
            else
            {
                bool existeNome = false,
                    existeEmail = false;

                while (leitor.Read())
                {

                    if (nome.Equals(leitor.GetString("nome_usuario")))
                    {
                        existeNome = true;
                    }
                    if (email.Equals(leitor.GetString("email_usuario")))
                    {
                        existeEmail = true;
                    }
                }

                if(existeNome && existeEmail)
                {
                    resposta = "Usuário e E-mail já cadastrados";
                }
                else
                {
                    if (existeNome)
                    {
                        resposta = "Usuário já cadastrado";
                    }
                    else
                    {
                        resposta = "E-mail já cadastrado";
                    }
                }
                
            }
        }
        catch (Exception e)
        {
            resposta = "ERRO: " + e.Message;
        }
        finally
        {
            //Fecha a conexão com o BD
            conexaoBD.Close();
        }

        return resposta;
    }


    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void buscaUsuario(int idUsuario)
    {
        Context.Response.ContentType = "application/json; charset=utf-8;";

        try
        {
            //Passagem de qual comando será executado na conexão
            MySqlCommand comando = new MySqlCommand("SELECT id_usuario, nome_usuario, email_usuario, senha_usuario FROM " + NOME_TABELA + " WHERE id_usuario = @idUsuario;", conexaoBD);

            //Informando qual o valor de cada parâmetro, ou seja, de cada valor com @
            comando.Parameters.AddWithValue("@idUsuario", idUsuario);

            //Abrindo a conexão com o BD
            conexaoBD.Open();

            //Executando a pesquisa
            MySqlDataReader leitor = comando.ExecuteReader();

            //Caso a pesquisa tenha tuplas (possua um usuario com o ID procurado)
            if (leitor.HasRows)
            {
                //Direciona o ponteira para esta tupla
                leitor.Read();

                //Instancia um objeto de Usuario com os valores retornados do BD
                Usuario usuario = new Usuario
                    (leitor.GetInt32("id_usuario"),
                    leitor["nome_usuario"].ToString(),
                    leitor["email_usuario"].ToString(),
                    leitor["senha_usuario"].ToString());

                //Cria a resposta em JSON enviando o objeto de Usuario
                Context.Response.Write(new JavaScriptSerializer().Serialize(usuario));
            }
            //Caso não tenha tuplas (não exista ninguem com este ID)
            else
            {
                //Cria a resposta em JSON enviando uma mensagem informando que não existe alguem com este id
                Context.Response.Write(new JavaScriptSerializer().Serialize("Não foi possível encontrar um Usuário com este ID"));
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
    public void listaUsuarios()
    {
        Context.Response.ContentType = "application/json; charset=utf-8;";

        try
        {
            //Passagem de qual comando será executado na conexão
            MySqlCommand comando = new MySqlCommand("SELECT id_usuario, nome_usuario, email_usuario, senha_usuario FROM " + NOME_TABELA + ";", conexaoBD);

            //Abre a conexão com o BD
            conexaoBD.Open();

            //Executa a pesquisa
            MySqlDataReader leitor = comando.ExecuteReader();

            //Cria uma ArrayList para guardar todos os usuarios encontrados
            List<Usuario> listaUsuarios = new List<Usuario>();

            //Enquanto possuir resultados da pesquisa (cada tupla)
            while (leitor.Read())
            {
                //Instancia um objeto de Usuario com os valores da tupla
                Usuario usuario = new Usuario(
                    leitor.GetInt32("id_usuario"),
                    leitor["nome_usuario"].ToString(),
                    leitor["email_usuario"].ToString(),
                    leitor["senha_usuario"].ToString()
                    );

                //Adiciona este objeto na ArrayList
                listaUsuarios.Add(usuario);
            }

            //Cria a resposta em JSON enviando a lista de Usuarios
            Context.Response.Write(new JavaScriptSerializer().Serialize(listaUsuarios));
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
    public void atualizaUsuarios(int ID_USUARIO, string NOME_USUARIO, string EMAIL_USUARIO, string SENHA_USUARIO)
    {
        Context.Response.ContentType = "application/json; charset=utf-8;";

        try
        {
            MySqlCommand comando = new MySqlCommand("UPDATE " + NOME_TABELA + " SET nome_usuario = @nome_usuario, email_usuario = @email_usuario, senha_usuario = @senha_usuario WHERE id_usuario = @id_usuario;", conexaoBD);

            comando.Parameters.AddWithValue("@id_usuario", ID_USUARIO);
            comando.Parameters.AddWithValue("@nome_usuario", NOME_USUARIO);
            comando.Parameters.AddWithValue("@email_usuario", EMAIL_USUARIO);
            comando.Parameters.AddWithValue("@senha_usuario", SENHA_USUARIO);

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
    public void deletaUsuario(int id_usuario)
    {
        Context.Response.ContentType = "application/json; charset=utf-8;";

        try
        {
            MySqlCommand comando =
            new MySqlCommand("DELETE FROM " + NOME_TABELA + " WHERE id_usuario = @id_usuario;", conexaoBD);
            comando.Parameters.AddWithValue("@id_usuario", id_usuario);

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

    [WebMethod]
    [ScriptMethod(ResponseFormat = ResponseFormat.Json)]
    public void logarUsuario(string nome_email, string senha)
    {
        Context.Response.ContentType = "application/json; charset=utf-8;";

        try
        {
            MySqlCommand comando =
            new MySqlCommand("SELECT * FROM " + NOME_TABELA + " WHERE nome_usuario = @nome_email OR email_usuario = @nome_email;", conexaoBD);
            comando.Parameters.AddWithValue("@nome_email", nome_email);
            comando.Parameters.AddWithValue("@senha", senha);

            conexaoBD.Open();

            MySqlDataReader leitor = comando.ExecuteReader();

            if (leitor.HasRows)
            {
                leitor.Read();

                if (senha.Equals(leitor.GetString("senha_usuario")))
                {
                    Usuario usuario = new Usuario
                    (leitor.GetInt32("id_usuario"),
                    leitor["nome_usuario"].ToString(),
                    leitor["email_usuario"].ToString(),
                    leitor["senha_usuario"].ToString());

                    Context.Response.Write(new JavaScriptSerializer().Serialize(usuario));
                }
                else
                {
                    Context.Response.Write(new JavaScriptSerializer().Serialize("Senha Incorreta"));
                }

            }
            else
            {
                Context.Response.Write(new JavaScriptSerializer().Serialize("Usuário/E-mail inválido ou incorreto"));
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

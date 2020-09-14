using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;

namespace tcc_3dsaa_grupoc.Models
{
    public class Usuario
    {

        [Key]
        [Display(Name = "ID Usuário")]
        public int id_usuario { get; set; }

        [Display(Name = "Usuário")]
        [Required(ErrorMessage = "Informe o nome do usuário", AllowEmptyStrings = false)]
        [MaxLength(40, ErrorMessage = "Nome deve ter no máximo 40 caracteres !")]
        public string nome_usuario { get; set; }

        [Display(Name = "Email")]
        [Required(ErrorMessage = "Informe o email do usuário", AllowEmptyStrings = false)]
        [RegularExpression(@"^([0-9a-zA-Z]([\+\-_\.][0-9a-zA-Z]+)*)+@(([0-9a-zA-Z][-\w]*[0-9a-zA-Z]*\.)+[a-zA-Z0-9]{2,3})$", ErrorMessage = "Informe um email válido.")]
        [MaxLength(30, ErrorMessage = "E-mail muito longo, deve possuir no máximo 30 caracteres")]
        public string email_usuario { get; set; }

        [Display(Name = "Senha")]
        [Required(ErrorMessage = "Digite uma senha", AllowEmptyStrings = false)]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [MaxLength(20, ErrorMessage = "Senha deve ter no máximo 20 caracteres !")]
        public string senha_usuario { get; set; }

        [Display(Name = "Confirmar Senha")]
        [DataType(System.ComponentModel.DataAnnotations.DataType.Password)]
        [Compare("senha_usuario", ErrorMessage = "As senhas não coincidem")]
        public string confirmar_senha_usuario { get; set; }

        /*Banco de Dados MySQL 8.0.20*/
        private static MySqlConnection conexao = new MySqlConnection(ConfigurationManager.ConnectionStrings["BancoDeDadosMySql"].ConnectionString);

        public string CadastrarUsuario()
        {
            string resultado = "Cadastrado com sucesso !";

            try
            {

                if (!existeUsuario(this.nome_usuario, this.email_usuario))
                {
                    conexao.Open();
                    MySqlCommand comando = new MySqlCommand("INSERT INTO tb_usuarios(nome_usuario, email_usuario, senha_usuario) VALUES(@nome_usuario, @email_usuario, @senha_usuario);", conexao);

                    comando.Parameters.AddWithValue("@nome_usuario", this.nome_usuario);
                    comando.Parameters.AddWithValue("@email_usuario", this.email_usuario);
                    comando.Parameters.AddWithValue("@senha_usuario", this.senha_usuario);

                    if (comando.ExecuteNonQuery() == 0)
                    {
                        resultado = "Erro ao Cadastrar";
                    }
                }
                else
                {
                    resultado = "Nome ou E-mail ja cadastrado !";
                }
            }
            catch (Exception e)
            {
                resultado = "Erro: " + e.Message;
            }

            if (conexao.State == ConnectionState.Open)
                conexao.Close();

            return resultado;
        }

        public static Usuario BuscaUsuario(int Id_Usuario)
        {
            Usuario usuarioEncontrado = new Usuario();

            try
            {
                conexao.Open();

                MySqlCommand comando = new MySqlCommand("SELECT nome_usuario, email_usuario FROM tb_usuarios WHERE id_usuario = @idUsuario;", conexao);

                comando.Parameters.AddWithValue("@idUsuario", Id_Usuario);

                MySqlDataReader leitor = comando.ExecuteReader();

                if (leitor.Read())
                {
                    usuarioEncontrado.id_usuario = Id_Usuario;
                    usuarioEncontrado.nome_usuario = leitor.GetString("nome_usuario");
                    usuarioEncontrado.email_usuario = leitor.GetString("email_usuario");

                }
                else
                {
                    usuarioEncontrado = null;
                }

            }
            catch (Exception e)
            {
                usuarioEncontrado = null;
                System.Diagnostics.Debug.WriteLine(e.StackTrace);
            }

            if (conexao.State == ConnectionState.Open)
                conexao.Close();

            return usuarioEncontrado;
        }

        public static Usuario Logar(string usuario, string senha)
        {
            Usuario usuarioEscolhido = new Usuario();

            try
            {
                conexao.Open();

                MySqlCommand comando = new MySqlCommand("SELECT id_usuario, nome_usuario, email_usuario, senha_usuario FROM tb_usuarios WHERE nome_usuario = @nome_email OR email_usuario = @nome_email;", conexao);

                comando.Parameters.AddWithValue("@nome_email", usuario);
                comando.Parameters.AddWithValue("@senha_usuario", senha);

                MySqlDataReader leitor = comando.ExecuteReader();

                if (!leitor.HasRows)
                {
                    usuarioEscolhido = null;
                }
                else
                {
                    if (leitor.Read())
                    {

                        if (senha.Equals(leitor.GetString("senha_usuario")))
                        {
                            usuarioEscolhido.id_usuario = leitor.GetInt16("id_usuario");
                            usuarioEscolhido.nome_usuario = leitor.GetString("nome_usuario");
                            usuarioEscolhido.email_usuario = leitor.GetString("email_usuario");
                            usuarioEscolhido.senha_usuario = leitor.GetString("senha_usuario");
                        }
                        else
                        {
                            usuarioEscolhido = null;
                        }

                    }

                }

            }
            catch (Exception e)
            {
                usuarioEscolhido = null;
                System.Diagnostics.Debug.WriteLine(e.StackTrace);
            }

            if (conexao.State == ConnectionState.Open)
                conexao.Close();

            return usuarioEscolhido;
        }

        private bool existeUsuario(string nome, string email)
        {
            bool existe = false;

            try
            {
                conexao.Open();

                MySqlCommand comando = new MySqlCommand("SELECT nome_usuario, email_usuario FROM tb_usuarios WHERE nome_usuario = @nomeUsuario OR email_usuario = @emailUsuario;", conexao);

                comando.Parameters.AddWithValue("@nomeUsuario", nome);
                comando.Parameters.AddWithValue("@emailUsuario", email);

                MySqlDataReader leitor = comando.ExecuteReader();

                if (leitor.Read())
                {
                    existe = true;
                }
                else
                {
                    existe = false;
                }

            }
            catch (Exception e)
            {
                existe = true;
                System.Diagnostics.Debug.WriteLine(e.StackTrace);
            }

            if (conexao.State == ConnectionState.Open)
                conexao.Close();

            return existe;
        }

    }
}
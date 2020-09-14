using MySql.Data.MySqlClient;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;

namespace tcc_3dsaa_grupoc.Models
{
    public class Relato
    {

        [Key]
        [Display(Name = "ID Relato")]
        public int idRelato { get; set; }

        [Display(Name = "ID Usuário")]
        public int idUsuario { get; set; }

        [Display(Name = "Usuário")]
        public string nome_usuario { get; set; }

        [Display(Name = "Descrição")]
        [Required(ErrorMessage = "Informe uma breve descrição do ocorrido", AllowEmptyStrings = false)]
        [MaxLength(100, ErrorMessage = "Descrição deve ter no máximo 100 caracteres !")]
        public string descricao { get; set; }

        [Display(Name = "ID Tipo Relato")]
        public int idTipoRelato { get; set; }

        [Display(Name = "Tipo Relato")]
        public string nome_tipoRelato { get; set; }

        [Display(Name = "Latitude")]
        [Required(ErrorMessage = "Clique no Mapa para selecionar o Local que ocorreu", AllowEmptyStrings = false)]
        public double localizacaoX { get; set; }

        [Display(Name = "Longitude")]
        [Required(ErrorMessage = ".", AllowEmptyStrings = false)]
        public double localizacaoY { get; set; }

        [Display(Name = "Rua")]
        public string rua { get; set; }

        [Display(Name = "Cidade")]
        public string cidade { get; set; }

        [Display(Name = "Estado")]
        public string estado { get; set; }

        [Display(Name = "Data")]
        [Required(ErrorMessage = "Informe a data do ocorrido", AllowEmptyStrings = false)]
        [DataType(DataType.Date)]
        public string data { get; set; }

        [Display(Name = "Hora")]
        [Required(ErrorMessage = "Informe o horário do ocorrido", AllowEmptyStrings = false)]
        [DataType(DataType.Time)]
        public string hora { get; set; }

        [Display(Name = "Anônimo")]
        public bool anonimo { get; set; }

        public static int RELATO_ANONIMO_SIM = 1;
        public static int RELATO_ANONIMO_NAO = 0;

        /*Banco de Dados MySQL 8.0.20*/
        private static MySqlConnection conexao = new MySqlConnection(ConfigurationManager.ConnectionStrings["BancoDeDadosMySql"].ConnectionString);

        public string Cadastrar()
        {
            string resultado = "Cadastrado com sucesso !";

            try
            {

                conexao.Open();

                MySqlCommand comando = new MySqlCommand("INSERT INTO tb_relatos(fk_id_usuario, fk_id_tipoRelato, descricao_relato, localizacao_x_relato, localizacao_y_relato, rua_relato, cidade_relato, estado_relato, data_relato, horario_relato, anonimo_relato) VALUES (@idUsuario, @idTipoRelato, @descricao, @latitude, @longitude, @rua, @cidade, @estado, @data, @horario, @anonimo)", conexao);

                DateTime dataMySQL = DateTime.Parse(this.data + " " + this.hora); //Guardando a data e horário em uma classe que será utilizada para transformar no formato MySQL

                //Método que busca na API a rua, cidade e estado utilizando com base a latitude e longitude
                this.buscarEnderecoAPI();

                comando.Parameters.AddWithValue("@idUsuario", this.idUsuario);
                comando.Parameters.AddWithValue("@idTipoRelato", this.idTipoRelato);
                comando.Parameters.AddWithValue("@descricao", this.descricao);
                comando.Parameters.AddWithValue("@latitude", Math.Round(this.localizacaoX, 8));
                comando.Parameters.AddWithValue("@longitude", Math.Round(this.localizacaoY, 8));
                comando.Parameters.AddWithValue("@rua", this.rua);
                comando.Parameters.AddWithValue("@cidade", this.cidade);
                comando.Parameters.AddWithValue("@estado", this.estado);
                comando.Parameters.AddWithValue("@data", dataMySQL.ToString("yyyy-MM-dd"));
                comando.Parameters.AddWithValue("@horario", dataMySQL.ToString("HH:mm:ss"));

                if(this.anonimo == true)
                {
                    comando.Parameters.AddWithValue("@anonimo", RELATO_ANONIMO_SIM);
                }
                else
                {
                    comando.Parameters.AddWithValue("@anonimo", RELATO_ANONIMO_NAO);
                }

                if (comando.ExecuteNonQuery() == 0)
                {
                    resultado = "Erro ao Cadastrar";
                }

            }
            catch (Exception e)
            {
                resultado = "Erro: "+e.Message;
                System.Diagnostics.Debug.WriteLine(e.StackTrace);
            }

            if (conexao.State == ConnectionState.Open)
            {
                conexao.Close();
            }

            return resultado;
        }

        public static List<Relato> ListaRelatos()
        {
            List<Relato> listaRelatos = new List<Relato>();

            try
            {
                conexao.Open();

                MySqlCommand comando = new MySqlCommand("SELECT id_relato, fk_id_usuario, tb_usuarios.nome_usuario, fk_id_tipoRelato, tb_tipoRelatos.nome_tipoRelato, descricao_relato, rua_relato, cidade_relato, estado_relato, data_relato, horario_relato, anonimo_relato FROM tb_relatos INNER JOIN tb_usuarios ON tb_relatos.fk_id_usuario = tb_usuarios.id_usuario INNER JOIN tb_tipoRelatos ON tb_relatos.fk_id_tipoRelato = tb_tipoRelatos.id_tipoRelato;", conexao);

                MySqlDataReader leitor = comando.ExecuteReader();

                while (leitor.Read())
                {
                    Relato relatoEncontrado = new Relato();
                    relatoEncontrado.idRelato = leitor.GetInt16("id_relato");
                    relatoEncontrado.idUsuario = leitor.GetInt16("fk_id_usuario");
                    relatoEncontrado.nome_usuario = leitor.GetString("nome_usuario");
                    relatoEncontrado.idTipoRelato = leitor.GetInt16("fk_id_tipoRelato");
                    relatoEncontrado.nome_tipoRelato = leitor.GetString("nome_tipoRelato");
                    relatoEncontrado.descricao = leitor.GetString("descricao_relato");

                    //Caso esteja NULL no BCD
                    if (!string.IsNullOrEmpty(leitor["rua_relato"].ToString()))
                    {
                        relatoEncontrado.rua = leitor.GetString("rua_relato");
                    }
                    else
                    {
                        relatoEncontrado.rua = "Indisponível";
                    }
                    if (!string.IsNullOrEmpty(leitor["cidade_relato"].ToString()))
                    {
                        relatoEncontrado.cidade = leitor.GetString("cidade_relato");
                    }
                    else
                    {
                        relatoEncontrado.cidade = "Indisponível";
                    }
                    if (!string.IsNullOrEmpty(leitor["estado_relato"].ToString()))
                    {
                        relatoEncontrado.estado = leitor.GetString("estado_relato");
                    }
                    else
                    {
                        relatoEncontrado.estado = "Indisponível";
                    }
                    
                    relatoEncontrado.data = DateTime.Parse(leitor.GetString("data_relato")).ToString("dd/MM/yyyy");
                    relatoEncontrado.hora = leitor.GetString("horario_relato");
                    
                    if(leitor.GetInt16("anonimo_relato") == RELATO_ANONIMO_SIM)
                    {
                        relatoEncontrado.anonimo = true;
                    }
                    else
                    {
                        relatoEncontrado.anonimo = false;
                    }      

                    listaRelatos.Add(relatoEncontrado);
                }

            }
            catch (Exception e)
            {
                listaRelatos = new List<Relato>();
                System.Diagnostics.Debug.WriteLine(e.StackTrace);
            }

            if (conexao.State == ConnectionState.Open)
                conexao.Close();

            return listaRelatos;
        }

        public static List<Relato> ListaRelatosRua(string rua)
        {
            List<Relato> listaRelatosRua = new List<Relato>();

            try
            {
                conexao.Open();

                MySqlCommand comando = new MySqlCommand("SELECT id_relato, fk_id_usuario, tb_usuarios.nome_usuario, fk_id_tipoRelato, tb_tipoRelatos.nome_tipoRelato, descricao_relato, rua_relato, cidade_relato, estado_relato, data_relato, horario_relato, anonimo_relato FROM tb_relatos INNER JOIN tb_usuarios ON tb_relatos.fk_id_usuario = tb_usuarios.id_usuario INNER JOIN tb_tipoRelatos ON tb_relatos.fk_id_tipoRelato = tb_tipoRelatos.id_tipoRelato;", conexao);

                MySqlDataReader leitor = comando.ExecuteReader();

                while (leitor.Read())
                {
                    Relato relatoEncontrado = new Relato();
                    relatoEncontrado.idRelato = leitor.GetInt16("id_relato");
                    relatoEncontrado.idUsuario = leitor.GetInt16("fk_id_usuario");
                    relatoEncontrado.nome_usuario = leitor.GetString("nome_usuario");
                    relatoEncontrado.idTipoRelato = leitor.GetInt16("fk_id_tipoRelato");
                    relatoEncontrado.nome_tipoRelato = leitor.GetString("nome_tipoRelato");
                    relatoEncontrado.descricao = leitor.GetString("descricao_relato");

                    //Caso esteja NULL no BCD
                    if (!string.IsNullOrEmpty(leitor["rua_relato"].ToString()))
                    {
                        relatoEncontrado.rua = leitor.GetString("rua_relato");
                    }
                    else
                    {
                        relatoEncontrado.rua = "Indisponível";
                    }
                    if (!string.IsNullOrEmpty(leitor["cidade_relato"].ToString()))
                    {
                        relatoEncontrado.cidade = leitor.GetString("cidade_relato");
                    }
                    else
                    {
                        relatoEncontrado.cidade = "Indisponível";
                    }
                    if (!string.IsNullOrEmpty(leitor["estado_relato"].ToString()))
                    {
                        relatoEncontrado.estado = leitor.GetString("estado_relato");
                    }
                    else
                    {
                        relatoEncontrado.estado = "Indisponível";
                    }

                    relatoEncontrado.data = DateTime.Parse(leitor.GetString("data_relato")).ToString("dd/MM/yyyy");
                    relatoEncontrado.hora = leitor.GetString("horario_relato");

                    if (leitor.GetInt16("anonimo_relato") == RELATO_ANONIMO_SIM)
                    {
                        relatoEncontrado.anonimo = true;
                    }
                    else
                    {
                        relatoEncontrado.anonimo = false;
                    }

                    //Reparte a rua escrita em uma lista de cada palavra (separa cada " " e entre cada " " cria um item e coloca numa lista)
                    foreach (string palavraEscrita in rua.Split(' '))
                    {
                        //Se essa palavra que foi repartida do que foi escrito existir dentro do endereço que o relato foi cadastrado
                        if (relatoEncontrado.rua.ToLower().IndexOf(palavraEscrita.ToLower()) != -1)
                        {
                            //Adiciona o relato a uma lista de relatos e adiciona uma frase com o tipoRelato e a Rua
                            listaRelatosRua.Add(relatoEncontrado);
                            break;
                        }
                    }
                    
                }

            }
            catch (Exception e)
            {
                listaRelatosRua = new List<Relato>();
                System.Diagnostics.Debug.WriteLine(e.StackTrace);
            }

            if (conexao.State == ConnectionState.Open)
                conexao.Close();

            return listaRelatosRua;
        }

        public static List<Relato> ListaRelatosUsuario(int Id_Usuario, bool donoPerfil)
        {
            List<Relato> listaRelatosUsuario = new List<Relato>();

            try
            {
                conexao.Open();

                MySqlCommand comando = new MySqlCommand("SELECT id_relato, fk_id_usuario, tb_usuarios.nome_usuario, fk_id_tipoRelato, tb_tipoRelatos.nome_tipoRelato, descricao_relato, rua_relato, cidade_relato, estado_relato, data_relato, horario_relato, anonimo_relato FROM tb_relatos INNER JOIN tb_usuarios ON tb_relatos.fk_id_usuario = tb_usuarios.id_usuario INNER JOIN tb_tipoRelatos ON tb_relatos.fk_id_tipoRelato = tb_tipoRelatos.id_tipoRelato WHERE fk_id_usuario = @idUsuario;", conexao);

                comando.Parameters.AddWithValue("@idUsuario", Id_Usuario);

                MySqlDataReader leitor = comando.ExecuteReader();

                while (leitor.Read())
                {
                    Relato relatoEncontrado = new Relato();

                    if (leitor.GetInt16("anonimo_relato") == RELATO_ANONIMO_SIM)
                    {
                        if (!donoPerfil)
                        {
                            continue;
                        }
                        else
                        {
                            relatoEncontrado.anonimo = true;
                        }
                    }
                    else
                    {
                        relatoEncontrado.anonimo = false;
                    }

                    relatoEncontrado.idRelato = leitor.GetInt16("id_relato");
                    relatoEncontrado.idUsuario = leitor.GetInt16("fk_id_usuario");
                    relatoEncontrado.nome_usuario = leitor.GetString("nome_usuario");
                    relatoEncontrado.idTipoRelato = leitor.GetInt16("fk_id_tipoRelato");
                    relatoEncontrado.nome_tipoRelato = leitor.GetString("nome_tipoRelato");
                    relatoEncontrado.descricao = leitor.GetString("descricao_relato");

                    //Caso esteja NULL no BCD
                    if (!string.IsNullOrEmpty(leitor["rua_relato"].ToString()))
                    {
                        relatoEncontrado.rua = leitor.GetString("rua_relato");
                    }
                    else
                    {
                        relatoEncontrado.rua = "Indisponível";
                    }
                    if (!string.IsNullOrEmpty(leitor["cidade_relato"].ToString()))
                    {
                        relatoEncontrado.cidade = leitor.GetString("cidade_relato");
                    }
                    else
                    {
                        relatoEncontrado.cidade = "Indisponível";
                    }
                    if (!string.IsNullOrEmpty(leitor["estado_relato"].ToString()))
                    {
                        relatoEncontrado.estado = leitor.GetString("estado_relato");
                    }
                    else
                    {
                        relatoEncontrado.estado = "Indisponível";
                    }

                    relatoEncontrado.data = DateTime.Parse(leitor.GetString("data_relato")).ToString("dd/MM/yyyy");
                    relatoEncontrado.hora = leitor.GetString("horario_relato");

                    listaRelatosUsuario.Add(relatoEncontrado);
                }

            }
            catch (Exception e)
            {
                listaRelatosUsuario = new List<Relato>();
                System.Diagnostics.Debug.WriteLine(e.StackTrace);
            }

            if (conexao.State == ConnectionState.Open)
                conexao.Close();

            return listaRelatosUsuario;
        }

        public static Relato BuscaRelato(int Id_Relato)
        {
            Relato relatoEncontrado = new Relato();

            try
            {
                conexao.Open();

                MySqlCommand comando = new MySqlCommand("SELECT fk_id_usuario, fk_id_tipoRelato, descricao_relato, localizacao_x_relato, localizacao_y_relato, data_relato, horario_relato, anonimo_relato FROM tb_relatos WHERE id_relato = @idRelato;", conexao);

                comando.Parameters.AddWithValue("@idRelato", Id_Relato);

                MySqlDataReader leitor = comando.ExecuteReader();

                if(leitor.Read())
                {
                    relatoEncontrado.idRelato = Id_Relato;
                    relatoEncontrado.idUsuario = leitor.GetInt16("fk_id_usuario");
                    relatoEncontrado.idTipoRelato = leitor.GetInt16("fk_id_tipoRelato");
                    relatoEncontrado.descricao = leitor.GetString("descricao_relato");
                    relatoEncontrado.localizacaoX = leitor.GetDouble("localizacao_x_relato");
                    relatoEncontrado.localizacaoY = leitor.GetDouble("localizacao_y_relato");
                    relatoEncontrado.data = DateTime.Parse(leitor.GetString("data_relato")).ToString("yyyy-MM-dd");
                    relatoEncontrado.hora = leitor.GetString("horario_relato");

                    if (leitor.GetInt16("anonimo_relato") == RELATO_ANONIMO_SIM)
                    {
                        relatoEncontrado.anonimo = true;
                    }
                    else
                    {
                        relatoEncontrado.anonimo = false;
                    }

                }
                else
                {
                    relatoEncontrado = null;
                }

            }
            catch (Exception e)
            {
                relatoEncontrado = null;
                System.Diagnostics.Debug.WriteLine(e.StackTrace);
            }

            if (conexao.State == ConnectionState.Open)
                conexao.Close();

            return relatoEncontrado;
        }

        public bool Remover()
        {
            bool resultado = false;

            try
            {
                conexao.Open();

                MySqlCommand comando = new MySqlCommand("DELETE FROM tb_relatos WHERE id_relato = @idRelato AND fk_id_usuario = @idUsuario", conexao);
                
                comando.Parameters.AddWithValue("@idRelato", this.idRelato);
                comando.Parameters.AddWithValue("@idUsuario", this.idUsuario);

                if (comando.ExecuteNonQuery() != 0)
                {
                    resultado = true;
                }
                else
                {
                    resultado = false;
                }

            }
            catch (Exception e)
            {
                resultado = false;
                System.Diagnostics.Debug.WriteLine(e.StackTrace);
            }

            if (conexao.State == ConnectionState.Open)
                conexao.Close();

            return resultado;
        }

        public bool Editar()
        {
            bool resultado = false;

            try
            {
                conexao.Open();

                MySqlCommand comando = new MySqlCommand("UPDATE tb_relatos SET fk_id_tipoRelato = @idTipoRelato, descricao_relato = @descricaoRelato, localizacao_x_relato = @latitude, localizacao_y_relato = @longitude, rua_relato = @rua, cidade_relato = @cidade, estado_relato = @estado, data_relato = @data, horario_relato = @hora, anonimo_relato = @anonimo WHERE id_relato = @idRelato AND fk_id_usuario = @idUsuario", conexao);

                DateTime dataMySQL = DateTime.Parse(this.data + " " + this.hora);

                //Método que busca na API a rua, cidade e estado utilizando com base a latitude e longitude
                this.buscarEnderecoAPI();

                comando.Parameters.AddWithValue("@idRelato", this.idRelato);
                comando.Parameters.AddWithValue("@idUsuario", this.idUsuario);
                comando.Parameters.AddWithValue("@idTipoRelato", this.idTipoRelato);
                comando.Parameters.AddWithValue("@descricaoRelato", this.descricao);
                comando.Parameters.AddWithValue("@latitude", Math.Round(this.localizacaoX, 8));
                comando.Parameters.AddWithValue("@longitude", Math.Round(this.localizacaoY, 8));
                comando.Parameters.AddWithValue("@rua", this.rua);
                comando.Parameters.AddWithValue("@cidade", this.cidade);
                comando.Parameters.AddWithValue("@estado", this.estado);
                comando.Parameters.AddWithValue("@data", dataMySQL.ToString("yyyy-MM-dd"));
                comando.Parameters.AddWithValue("@hora", dataMySQL.ToString("HH:mm:ss"));

                if (this.anonimo == true)
                {
                    comando.Parameters.AddWithValue("@anonimo", RELATO_ANONIMO_SIM);
                }
                else
                {
                    comando.Parameters.AddWithValue("@anonimo", RELATO_ANONIMO_NAO);
                }

                if (comando.ExecuteNonQuery() != 0)
                {
                    resultado = true;
                }
                else
                {
                    resultado = false;
                }

            }
            catch (Exception e)
            {
                resultado = false;
                System.Diagnostics.Debug.WriteLine(e.StackTrace);
            }

            if (conexao.State == ConnectionState.Open)
                conexao.Close();

            return resultado;

        }

        private void buscarEnderecoAPI()
        {
            try
            {
                //https://nominatim.org/ - Open-source geocoding with OpenStreetMap data
                HttpWebRequest requisicao = (HttpWebRequest)HttpWebRequest.Create("https://nominatim.openstreetmap.org/reverse?format=json&lat=" + this.localizacaoX + "&lon=" + this.localizacaoY);
                requisicao.UserAgent = "TCC-GRUPOC"; //Se tirar esta linha a API retorna isso:  The remote server returned an error: (403) Forbidden.

                WebResponse resposta = requisicao.GetResponse();
                StreamReader leitorAPI = new StreamReader(resposta.GetResponseStream());

                string resultadoJSON = leitorAPI.ReadToEnd();


                var resultado = JsonConvert.DeserializeObject<dynamic>(resultadoJSON);

                this.rua = resultado.address.road;
                this.cidade = resultado.address.city;
                this.estado = resultado.address.state;

                if (string.IsNullOrEmpty(this.rua))
                {
                    this.rua = "Indisponível";
                }
                if (string.IsNullOrEmpty(this.cidade))
                {
                    this.cidade = "Indisponível";
                }
                if (string.IsNullOrEmpty(this.estado))
                {
                    this.estado = "Indisponível";
                }


            }
            catch (Exception e)
            {
                System.Diagnostics.Debug.WriteLine(e.Message);
                System.Diagnostics.Debug.WriteLine(e.StackTrace);

                this.rua = "Indisponível";
                this.cidade = "Indisponível";
                this.estado = "Indisponível";
            }
        }
    }
}
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
    public class TipoRelato
    {

        [Key]
        [Display(Name = "ID Tipo Relato")]
        public int idTipoRelato { get; set; }

        [Display(Name = "Nome Tipo Relato")]
        [Required(ErrorMessage = "Informe o nome do tipo relato", AllowEmptyStrings = false)]
        [MaxLength(25, ErrorMessage = "Nome do tipo relato deve ter no máximo 25 caracteres !")]
        public string nomeTipoRelato { get; set; }

        /*Banco de Dados MySQL 8.0.20*/
        private static MySqlConnection conexao = new MySqlConnection(ConfigurationManager.ConnectionStrings["BancoDeDadosMySql"].ConnectionString);

        public static List<TipoRelato> ListaTipoRelatos()
        {
            List<TipoRelato> listaTipoRelatos = new List<TipoRelato>();

            try
            {
                conexao.Open();

                MySqlCommand comando = new MySqlCommand("SELECT * FROM tb_tipoRelatos;", conexao);
                MySqlDataReader leitor = comando.ExecuteReader();

                while (leitor.Read())
                {
                    TipoRelato tipoRelatoEncontrado = new TipoRelato();
                    tipoRelatoEncontrado.idTipoRelato = leitor.GetInt16("id_tipoRelato");
                    tipoRelatoEncontrado.nomeTipoRelato = leitor.GetString("nome_tipoRelato");


                    listaTipoRelatos.Add(tipoRelatoEncontrado);
                }

            }
            catch (Exception e)
            {
                listaTipoRelatos = new List<TipoRelato>();
                System.Diagnostics.Debug.WriteLine(e.StackTrace);
            }

            if (conexao.State == ConnectionState.Open)
                conexao.Close();

            return listaTipoRelatos;
        }
    }
}
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

// Classe Usuário que possui o modelo de dados que será inserido no banco de Dados
public class Usuario
{
    public int id_usuario { get; set; }
    public string nome_usuario { get; set; }
    public string email_usuario { get; set; }
    public string senha_usuario { get; set; }


    public Usuario()
    {
    }

    public Usuario(int ID_USUARIO, string NOME_USUARIO, string EMAIL_USUARIO, string SENHA_USUARIO)
    {
        this.id_usuario = ID_USUARIO;
        this.nome_usuario = NOME_USUARIO;
        this.email_usuario = EMAIL_USUARIO;
        this.senha_usuario = SENHA_USUARIO;
    }

}

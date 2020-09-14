using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for Relato
/// </summary>
public class Relato
{
    public int id_relato { get; set; }
    public int fk_id_usuario { get; set; }
    public string nome_usuario { get; set; }
    public int fk_id_tipoRelato { get; set; }
    public string nome_tipoRelato { get; set; }
    public string descricao_relato { get; set; }
    public double localizacao_x_relato { get; set; }
    public double localizacao_y_relato { get; set; }
    public string data_relato { get; set; }
    public string horario_relato { get; set; }
    public int anonimo_relato { get; set; }

    public static int RELATO_ANONIMO_SIM = 1;
    public static int RELATO_ANONIMO_NAO = 0;

    public Relato()
    {
    }

    public Relato(int ID_RELATO, int FK_ID_USUARIO, int FK_ID_TIPORELATO, string DESCRICAO_RELATO, double LOCALIZACAO_X_RELATO, double LOCALIZACAO_Y_RELATO, string DATA_RELATO, string HORARIO_RELATO, int ANONIMO_RELATO)
    {
        this.id_relato = ID_RELATO;
        this.fk_id_usuario = FK_ID_USUARIO;
        this.fk_id_tipoRelato = FK_ID_TIPORELATO;
        this.descricao_relato = DESCRICAO_RELATO;
        this.localizacao_x_relato = LOCALIZACAO_X_RELATO;
        this.localizacao_y_relato = LOCALIZACAO_Y_RELATO;
        this.data_relato = DATA_RELATO;
        this.horario_relato = HORARIO_RELATO;
        this.anonimo_relato = ANONIMO_RELATO;
    }

    public Relato(int ID_RELATO, int FK_ID_USUARIO, string NOME_USUARIO, int FK_ID_TIPORELATO, string NOME_TIPORELATO, string DESCRICAO_RELATO, double LOCALIZACAO_X_RELATO, double LOCALIZACAO_Y_RELATO, string DATA_RELATO, string HORARIO_RELATO, int ANONIMO_RELATO)
    {
        this.id_relato = ID_RELATO;
        this.fk_id_usuario = FK_ID_USUARIO;
        this.nome_usuario = NOME_USUARIO;
        this.fk_id_tipoRelato = FK_ID_TIPORELATO;
        this.nome_tipoRelato = NOME_TIPORELATO;
        this.descricao_relato = DESCRICAO_RELATO;
        this.localizacao_x_relato = LOCALIZACAO_X_RELATO;
        this.localizacao_y_relato = LOCALIZACAO_Y_RELATO;
        this.data_relato = DATA_RELATO;
        this.horario_relato = HORARIO_RELATO;
        this.anonimo_relato = ANONIMO_RELATO;
    }
}
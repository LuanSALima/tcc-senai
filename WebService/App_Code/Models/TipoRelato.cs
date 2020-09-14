using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

/// <summary>
/// Summary description for TipoRelato
/// </summary>
public class TipoRelato
{

    public int id_tipoRelato { get; set; }
    public string nome_tipoRelato { get; set; }


    public TipoRelato()
    {
    }

    public TipoRelato(int ID_TIPORELATO, string NOME_TIPORELATO)
    {
        this.id_tipoRelato = ID_TIPORELATO;
        this.nome_tipoRelato = NOME_TIPORELATO;
    }

}
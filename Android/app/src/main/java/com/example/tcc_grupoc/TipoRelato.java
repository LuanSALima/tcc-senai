package com.example.tcc_grupoc;

public class TipoRelato {

    private int id_tipoRelato;
    private String nome_tipoRelato;

    public TipoRelato(int id_tipoRelato, String nome_tipoRelato)
    {
        this.id_tipoRelato = id_tipoRelato;
        this.nome_tipoRelato = nome_tipoRelato;
    }

    public int getId_tipoRelato() {
        return id_tipoRelato;
    }

    public String getNome_tipoRelato() {
        return nome_tipoRelato;
    }
}

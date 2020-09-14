package com.example.tcc_grupoc;

public class Relato {

    int idRelato, idUsuario, idTipoRelato, anonimo;
    String nome_usuario, nome_tipoRelato;
    String descricao, data, horario;
    double localizacao_X, localizacao_Y;

    public Relato(int idRelato, int idUsuario, String nomeUsuario, int idTipoRelato, String nomeTipoRelato, String descricao, double localizacao_X, double localizacao_Y, String data, String horario, int anonimo) {
        this.idRelato = idRelato;
        this.idUsuario = idUsuario;
        this.nome_usuario = nomeUsuario;
        this.idTipoRelato = idTipoRelato;
        this.nome_tipoRelato = nomeTipoRelato;
        this.descricao = descricao;
        this.localizacao_X = localizacao_X;
        this.localizacao_Y = localizacao_Y;
        this.data = data;
        this.horario = horario;
        this.anonimo = anonimo;
    }

    public int getIdRelato() {
        return idRelato;
    }

    public int getIdUsuario() {
        return idUsuario;
    }

    public int getIdTipoRelato() {
        return idTipoRelato;
    }

    public int getAnonimo() {
        return anonimo;
    }

    public String getNome_usuario() {
        return nome_usuario;
    }

    public String getNome_tipoRelato() {
        return nome_tipoRelato;
    }

    public String getDescricao() {
        return descricao;
    }

    public String getData() {
        return data;
    }

    public String getHorario() {
        return horario;
    }

    public double getLocalizacao_X() {
        return localizacao_X;
    }

    public double getLocalizacao_Y() {
        return localizacao_Y;
    }
}

package com.example.tcc_grupoc;

import android.location.Location;

import com.google.android.gms.maps.model.LatLng;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;

public class GrupoRelato {

    public static int numeroGruposRelato = 0;
    private int idGrupoRelato;
    private LatLng centroGrupo;
    private List<Relato> listaRelatos;

    //https://www.dataonfocus.com/k-means-clustering-java-code/

    public GrupoRelato(){
        numeroGruposRelato++;
        this.idGrupoRelato = numeroGruposRelato;
        listaRelatos = new ArrayList<>();
    }

    public boolean adicionaRelato(Relato relato){

        if(!relatoCadastrado(relato)) {
            listaRelatos.add(relato);
            relato.setGrupoRelato(this);
            calculaCentroGrupo();

            return true;
        }

        return false;
    }

    public boolean removeRelato(Relato relato){

        for(Relato relatoGrupo : this.listaRelatos){
            if(relatoGrupo.getIdTipoRelato() == relato.getIdTipoRelato()) {
                this.listaRelatos.remove(relatoGrupo);
                return true;
            }
        }

        return false;

    }

    public List<Relato> getListaRelatos(){
        return listaRelatos;
    }

    public LatLng getCentroGrupo() {
        return centroGrupo;
    }

    public int getIdGrupoRelato() {
        return idGrupoRelato;
    }

    public boolean relatoCadastrado(Relato relato){
        for (Relato ocorrencia : listaRelatos){
            if(relato.getIdRelato() == ocorrencia.getIdRelato())
                return true;
        }
        return false;
    }

    private void calculaCentroGrupo(){

        double somaLatitude = 0, somaLongitude = 0;

        for(Relato relato : this.listaRelatos){
            somaLatitude += relato.getLocalizacao_X();
            somaLongitude += relato.getLocalizacao_Y();
        }

        this.centroGrupo = new LatLng(somaLatitude/this.listaRelatos.size(), somaLongitude/this.listaRelatos.size());

    }

    public static HashMap<String, Integer> qtdTipoRelatos(GrupoRelato grupoRelato){

        HashMap<String, Integer> qtdTipoRelatos = new HashMap<>();

        for (Relato relato : grupoRelato.getListaRelatos()){

            if(qtdTipoRelatos.containsKey(relato.getNome_tipoRelato())) {
                qtdTipoRelatos.put(relato.getNome_tipoRelato(), qtdTipoRelatos.get(relato.getNome_tipoRelato())+1);
            }else{
                qtdTipoRelatos.put(relato.getNome_tipoRelato(), 1);
            }

        }

        return qtdTipoRelatos;

    }


}

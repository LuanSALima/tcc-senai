package com.example.tcc_grupoc;

import androidx.annotation.RequiresApi;
import androidx.appcompat.app.AppCompatActivity;
import androidx.core.content.ContextCompat;
import androidx.fragment.app.Fragment;

import android.Manifest;
import android.content.Intent;
import android.content.SharedPreferences;
import android.content.pm.PackageManager;
import android.os.Build;
import android.os.Bundle;
import android.view.View;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.CompoundButton;
import android.widget.EditText;
import android.widget.Spinner;
import android.widget.Switch;
import android.widget.Toast;

import com.android.volley.Request;
import com.android.volley.RequestQueue;
import com.android.volley.Response;
import com.android.volley.VolleyError;
import com.android.volley.toolbox.StringRequest;
import com.android.volley.toolbox.Volley;
import com.github.rtoshiro.util.format.SimpleMaskFormatter;
import com.github.rtoshiro.util.format.text.MaskTextWatcher;
import com.google.android.gms.common.api.GoogleApiClient;
import com.google.android.gms.maps.CameraUpdateFactory;
import com.google.android.gms.maps.GoogleMap;
import com.google.android.gms.maps.MapView;
import com.google.android.gms.maps.OnMapReadyCallback;
import com.google.android.gms.maps.model.LatLng;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Map;


public class RelatoActivity extends AppCompatActivity implements OnMapReadyCallback {
    private EditText edit_data, edit_hora, edit_descricao;
    private MapView map_relato;
    private GoogleMap map;
    private Button btn_relato;
    private Spinner spin_relato;
    private Switch sw_anonimo;
    private LatLng posicao;
    private static final String MAP_VIEW_BUNDLE_KEY = "MapViewBundleKey";

    private List<TipoRelato> listaTipoRelatos = new ArrayList<>();

    private String[] caminhosWebService =
            {"http://10.87.107.11/3DSAA_TCC_GRUPOC/",
             "http://10.87.106.29/3DSAA_TCC_GRUPOC/"};

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_relato);

        SharedPreferences logado = getSharedPreferences("login", MODE_PRIVATE);



        if(logado.getInt("id_usuario", -1) == -1){
            Toast.makeText(getApplicationContext(), "É necessário estar logado para Cadastrar Relatos", Toast.LENGTH_LONG).show();
            startActivity(new Intent(RelatoActivity.this, Tela_Login.class));
        }

        //Referencias
        edit_data = findViewById(R.id.edit_data);
        edit_hora = findViewById(R.id.edit_hora);
        edit_descricao = findViewById(R.id.edit_descricao);
        map_relato = (MapView) findViewById(R.id.map_relato);
        btn_relato = findViewById(R.id.btn_relato);
        spin_relato = findViewById(R.id.spin_relato);
        sw_anonimo = findViewById(R.id.sw_anonimo);

        populaTiposRelatos(caminhosWebService);

        SimpleMaskFormatter dataMask = new SimpleMaskFormatter("NN/NN/NNNN");
        MaskTextWatcher maskDataWatcher = new MaskTextWatcher(edit_data, dataMask);
        edit_data.addTextChangedListener(maskDataWatcher);

        SimpleMaskFormatter horarioMask = new SimpleMaskFormatter("NN:NN");
        MaskTextWatcher maskhorarioWatcher = new MaskTextWatcher(edit_hora, horarioMask);
        edit_hora.addTextChangedListener(maskhorarioWatcher);

        sw_anonimo.setOnCheckedChangeListener(new CompoundButton.OnCheckedChangeListener() {
            @Override
            public void onCheckedChanged(CompoundButton compoundButton, boolean isCheked) {
                if (isCheked == true) {
                    Toast.makeText(getBaseContext(), "Relato será anonimo", Toast.LENGTH_SHORT).show();
                }
            }
        });

        Bundle map_relato_bundle = null;
        if (savedInstanceState != null) {
            map_relato_bundle = savedInstanceState.getBundle(MAP_VIEW_BUNDLE_KEY);
        }
        map_relato.onCreate(map_relato_bundle);
        map_relato.getMapAsync((OnMapReadyCallback) this);
        map_relato.setClickable(true);

        if (ContextCompat.checkSelfPermission(RelatoActivity.this,
                Manifest.permission_group.LOCATION)
                != PackageManager.PERMISSION_GRANTED) {
            // Permission is not granted
        }

        btn_relato.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {

                final String data = formataData(edit_data.getText().toString()),
                        hora = edit_hora.getText().toString(),
                        descricao = edit_descricao.getText().toString();

                final double localizacaoX, localizacaoY;

                final int idUsuario = getSharedPreferences("login", MODE_PRIVATE).getInt("id_usuario", -1),
                        idTipoRelato = (listaTipoRelatos.get(spin_relato.getSelectedItemPosition()).getId_tipoRelato());

                if (posicao != null) {
                    localizacaoX = posicao.latitude;
                    localizacaoY = posicao.longitude;
                }else{
                    localizacaoX = 0;
                    localizacaoY = 0;
                }

                if (validaDados(descricao, data, hora, localizacaoX, localizacaoY)) {

                    for (final String caminho : caminhosWebService) {

                        final String metodoWebService = caminho + "WebServiceRelato.asmx/cadastraRelato";

                        StringRequest requisicao = new StringRequest(Request.Method.POST, metodoWebService, new Response.Listener<String>() {
                            @Override
                            public void onResponse(String response) {
                                if (response.toLowerCase().contains("cadastrado")) {
                                    Toast.makeText(RelatoActivity.this, "Relato cadastrado", Toast.LENGTH_LONG).show();
                                } else {
                                    Toast.makeText(RelatoActivity.this, response, Toast.LENGTH_LONG).show();
                                }
                            }
                        }, new Response.ErrorListener() {
                            @Override
                            public void onErrorResponse(VolleyError error) {

                                //Quando o caminho para o servidor onde esta hospedado o Webservice estiver errado cairá neste ponto do código e lançará "volley.TimeoutError" Exception
                                //Toast.makeText(Tela_Cadastro.this, "Erro ao Conectar ao Servidor", Toast.LENGTH_LONG).show();
                                error.printStackTrace();

                            }
                        }) {
                            @Override
                            protected Map<String, String> getParams() {

                                Map<String, String> parametros = new HashMap<>();
                                parametros.put("idUsuario", idUsuario+"");
                                parametros.put("idTipoRelato", idTipoRelato+"");
                                parametros.put("descricao", descricao);
                                parametros.put("localizacao_X", localizacaoX+"");
                                parametros.put("localizacao_Y", localizacaoY+"");
                                parametros.put("data", data);
                                parametros.put("horario", hora);
                                if (sw_anonimo.isChecked()) {
                                    parametros.put("anonimo", "1");
                                } else {
                                    parametros.put("anonimo", "0");
                                }
                                return parametros;
                            }
                        };
                        RequestQueue rq = Volley.newRequestQueue(RelatoActivity.this);
                        rq.add(requisicao);

                    }

                }

            }
        });

    }

    private String formataData(String data){

        if(data.isEmpty())
            return "";

        String[] data2 = data.split("/");
        int dia = Integer.parseInt(data2[0]);
        int mes = Integer.parseInt(data2[1]);
        int ano = Integer.parseInt(data2[2]);

        return (ano+"-"+mes+"-"+dia);
    }

    private boolean validaDados(String descricao, String data, String horario, double localizacaoX, double localizacaoY){

        if(descricao.isEmpty() || data.isEmpty() || horario.isEmpty()){

            if(descricao.isEmpty())
                edit_descricao.setError("Faça a descrição da ocorrencia");

            if(data.isEmpty())
                edit_data.setError("Preencha o campo data");

            if(horario.isEmpty())
                edit_hora.setError("Preencha o campo horario");

            return false;

        }else{

            if(localizacaoX == 0 || localizacaoY == 0){
                Toast.makeText(this, "Selecione no mapa o local da ocorrência", Toast.LENGTH_SHORT).show();
                return false;
            }

            String[] hora1 = horario.split(":");
            int horas = Integer.parseInt(hora1[0]);
            int minutos = Integer.parseInt(hora1[1]);

            if (horas > 23 | minutos > 59) {
                edit_hora.setError("Horario inválido");
                return false;
            } else {
                String[] data2 = data.split("-");
                int dia = Integer.parseInt(data2[2]);
                int mes = Integer.parseInt(data2[1]);
                int ano = Integer.parseInt(data2[0]);
                if (dia > 31 | mes > 12 | ano > 2020) {
                    edit_data.setError("Data inválida");
                    return false;
                }
            }

        }

        return true;
    }

    private void populaTiposRelatos(String[] caminhosWebService) {

        for (final String caminho : caminhosWebService) {

            final String metodoWebService = caminho + "WebServiceTipoRelato.asmx/listaTipoRelato";

            RequestQueue filaRequisicao = Volley.newRequestQueue(RelatoActivity.this);

            StringRequest requisicao = new StringRequest(Request.Method.POST, metodoWebService, new Response.Listener<String>() {

                @Override
                public void onResponse(String response) {

                    try {
                        JSONArray respostaWebService = new JSONArray(response);

                        for (int i = 0; i < respostaWebService.length(); i++) {
                            JSONObject itemResposta = respostaWebService.getJSONObject(i);

                            TipoRelato tipoRelatoEncontrado = new TipoRelato(itemResposta.getInt("id_tipoRelato"),
                                                                    itemResposta.getString("nome_tipoRelato"));

                            listaTipoRelatos.add(tipoRelatoEncontrado);
                        }

                    } catch (JSONException e) {
                        e.printStackTrace();
                        Toast.makeText(RelatoActivity.this, "ERRO : " + e.getMessage(), Toast.LENGTH_SHORT).show();
                    }

                }

            }, new Response.ErrorListener() {
                @Override
                public void onErrorResponse(VolleyError error) {

                    //Quando o caminho para o servidor onde esta hospedado o Webservice estiver errado cairá neste ponto do código e lançará "volley.TimeoutError" Exception
                    //Toast.makeText(Tela_Cadastro.this, "Erro ao Conectar ao Servidor", Toast.LENGTH_LONG).show();
                    error.printStackTrace();

                }
            });

            filaRequisicao.add(requisicao);

            filaRequisicao.addRequestFinishedListener(new RequestQueue.RequestFinishedListener<StringRequest>() {

                @Override
                public void onRequestFinished(Request<StringRequest> request) {

                    //POPULAR SPINNER COM OS TIPOS DE RELATOS CADASTRADOS NO WEBSERVICE

                    ArrayList<String> nomesTipoRelatos = new ArrayList<>();

                    for(TipoRelato tipoRelato : listaTipoRelatos){
                        nomesTipoRelatos.add(tipoRelato.getNome_tipoRelato());
                    }

                    ArrayAdapter<String> adapterTipoRelato = new ArrayAdapter<String>(RelatoActivity.this, android.R.layout.simple_spinner_item, nomesTipoRelatos);
                    adapterTipoRelato.setDropDownViewResource(android.R.layout.simple_spinner_dropdown_item);
                    spin_relato.setAdapter(adapterTipoRelato);

                }

            });

        }

    }

    @Override
    protected void onStart() {
        super.onStart();
        map_relato.onStart();
    }


    @Override
    protected void onStop() {
        super.onStop();
        map_relato.onStop();
    }

    @Override
    public void onDestroy() {
        super.onDestroy();
        map_relato.onDestroy();
    }

    @Override
    public void onLowMemory() {
        super.onLowMemory();
        map_relato.onLowMemory();
    }

    @Override
    public void onPause() {
        super.onPause();
        map_relato.onPause();
    }

    @Override
    public void onResume() {
        super.onResume();
        map_relato.onResume();
    }

    @Override
    protected void onSaveInstanceState(Bundle outState) {
        super.onSaveInstanceState(outState);
        Bundle map_relato_bundle = outState.getBundle(MAP_VIEW_BUNDLE_KEY);
        if (map_relato_bundle == null) {
            map_relato_bundle = new Bundle();
            outState.putBundle(MAP_VIEW_BUNDLE_KEY, map_relato_bundle);
        }
        map_relato.onSaveInstanceState(map_relato_bundle);
    }

    @RequiresApi(api = Build.VERSION_CODES.M)
    @Override
    public void onMapReady(GoogleMap googleMap) {
        map = googleMap;
        map.setMinZoomPreference(11);
        final LatLng localizacao = new LatLng(-22.907764, -47.059298);
        map.moveCamera(CameraUpdateFactory.newLatLng(localizacao));
        map.setTrafficEnabled(true);
        if (checkSelfPermission(Manifest.permission.ACCESS_FINE_LOCATION) != PackageManager.PERMISSION_GRANTED && checkSelfPermission(Manifest.permission.ACCESS_COARSE_LOCATION) != PackageManager.PERMISSION_GRANTED) {
            return;
        }
        map.setMyLocationEnabled(true);
        map.getUiSettings().setRotateGesturesEnabled(true);
        map.getUiSettings().setZoomControlsEnabled(true);

        map.setOnMapClickListener(new GoogleMap.OnMapClickListener() {
            @Override
            public void onMapClick(final LatLng latLng) {

                posicao = latLng;

            }
        });
    }




}

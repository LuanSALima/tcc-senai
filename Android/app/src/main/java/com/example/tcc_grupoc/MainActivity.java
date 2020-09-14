package com.example.tcc_grupoc;

import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.widget.Toast;

import com.android.volley.Request;
import com.android.volley.RequestQueue;
import com.android.volley.Response;
import com.android.volley.VolleyError;
import com.android.volley.toolbox.StringRequest;
import com.android.volley.toolbox.Volley;
import com.google.android.gms.maps.CameraUpdateFactory;
import com.google.android.gms.maps.GoogleMap;
import com.google.android.gms.maps.OnMapReadyCallback;
import com.google.android.gms.maps.SupportMapFragment;
import com.google.android.gms.maps.model.Circle;
import com.google.android.gms.maps.model.LatLng;
import com.google.android.gms.maps.model.MarkerOptions;
import com.google.android.material.floatingactionbutton.FloatingActionButton;

import androidx.core.view.GravityCompat;
import androidx.appcompat.app.ActionBarDrawerToggle;

import android.view.MenuItem;

import com.google.android.material.navigation.NavigationView;

import androidx.drawerlayout.widget.DrawerLayout;

import androidx.appcompat.app.AppCompatActivity;
import androidx.appcompat.widget.Toolbar;

import android.view.Menu;

import org.json.JSONArray;
import org.json.JSONException;
import org.json.JSONObject;

import java.util.ArrayList;
import java.util.List;

public class MainActivity extends AppCompatActivity
        implements NavigationView.OnNavigationItemSelectedListener, OnMapReadyCallback {
    private GoogleMap mMap;
    private List<Relato> listaRelatos = new ArrayList<>();

    private String[] caminhosWebService =
            {"http://10.87.107.11/3DSAA_TCC_GRUPOC/WebServiceRelato.asmx",
             "http://10.87.106.29/3DSAA_TCC_GRUPOC/WebServiceRelato.asmx"};

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_main2);

        SharedPreferences termos = getSharedPreferences("termos", MODE_PRIVATE);

        if(termos.getBoolean("aceitar", false) == false){
            startActivity(new Intent(getApplicationContext(), Termos.class));
        }

        // Obtain the SupportMapFragment and get notified when the map is ready to be used.

        SupportMapFragment mapFragment = (SupportMapFragment) getSupportFragmentManager()
                .findFragmentById(R.id.map);
        mapFragment.getMapAsync(this);

    /**
     * Manipulates the map once available.
     * This callback is triggered when the map is ready to be used.
     * This is where we can add markers or lines, add listeners or move the camera. In this case,
     * we just add a marker near Sydney, Australia.
     * If Google Play services is not installed on the device, the user will be prompted to install
     * it inside the SupportMapFragment. This method will only be triggered once the user has
     * installed Google Play services and returned to the app.
     */
        Toolbar toolbar = findViewById(R.id.toolbar);
        setSupportActionBar(toolbar);
        FloatingActionButton fab = findViewById(R.id.fab);

        DrawerLayout drawer = findViewById(R.id.drawer_layout);
        NavigationView navigationView = findViewById(R.id.nav_view);
        ActionBarDrawerToggle toggle = new ActionBarDrawerToggle(
                this, drawer, toolbar, R.string.navigation_drawer_open, R.string.navigation_drawer_close);
        drawer.addDrawerListener(toggle);
        toggle.syncState();
        navigationView.setNavigationItemSelectedListener(this);
    }

    @Override
    public void onMapReady(GoogleMap googleMap) {
        mMap = googleMap;

        //Popular o ArrayList de Relatos e depois para cada relato criar um circulo no mapa
        buscaListaRelatos(mMap);

        // Add a marker in Sydney and move the camera
        LatLng localizacaoAtual = new LatLng(-22.914833, -47.055931);

        mMap.moveCamera(CameraUpdateFactory.newLatLngZoom(localizacaoAtual, 16));
    
        mMap.setOnCircleClickListener(new GoogleMap.OnCircleClickListener() {
            @Override
            public void onCircleClick(Circle circle) {
                Toast.makeText(MainActivity.this, "COLOCAR EVENTO CLICK", Toast.LENGTH_SHORT).show();
            }
        });
    }

    private void buscaListaRelatos(final GoogleMap mapa){

        for (final String caminho : caminhosWebService) {

            final String metodoWebService = caminho + "/listaRelatos";

            RequestQueue filaRequisicao = Volley.newRequestQueue(MainActivity.this);


            StringRequest requisicao = new StringRequest(Request.Method.POST, metodoWebService, new Response.Listener<String>() {

                @Override
                public void onResponse(String response) {

                    try {
                        JSONArray respostaWebService = new JSONArray(response);

                        for (int i = 0; i < respostaWebService.length(); i++) {
                            JSONObject itemResposta = respostaWebService.getJSONObject(i);

                            Relato relatoEncontrado = new Relato(itemResposta.getInt("id_relato"),
                                    itemResposta.getInt("fk_id_usuario"),
                                    itemResposta.getString("nome_usuario"),
                                    itemResposta.getInt("fk_id_tipoRelato"),
                                    itemResposta.getString("nome_tipoRelato"),
                                    itemResposta.getString("descricao_relato"),
                                    itemResposta.getDouble("localizacao_x_relato"),
                                    itemResposta.getDouble("localizacao_y_relato"),
                                    itemResposta.getString("data_relato"),
                                    itemResposta.getString("horario_relato"),
                                    itemResposta.getInt("anonimo_relato"));

                            listaRelatos.add(relatoEncontrado);
                        }

                    } catch (JSONException e) {
                        e.printStackTrace();
                        Toast.makeText(MainActivity.this, "ERRO : " + e.getMessage(), Toast.LENGTH_SHORT).show();
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

                    criaMarcador(mapa);

                }

            });

        }

    }

    private void criaMarcador(GoogleMap mapa){

        for(Relato relato : listaRelatos){

            mapa.addMarker(new MarkerOptions()
                    .position(new LatLng(relato.localizacao_X, relato.localizacao_Y))
                    .title(relato.nome_tipoRelato)
            );

            /*
            mapa.addCircle(new CircleOptions()
                    .center(new LatLng(relato.getLocalizacao_X(), relato.getLocalizacao_Y()))
                    .radius(200)
                    .fillColor(Color.argb(50, 255, 0, 0))
                    .clickable(true)

            ).setTag("COLOCAR TAG AQUII");
            */
        }

    }
    public void onBackPressed() {
        DrawerLayout drawer = findViewById(R.id.drawer_layout);
        if (drawer.isDrawerOpen(GravityCompat.START)) {
            drawer.closeDrawer(GravityCompat.START);
        } else {
            super.onBackPressed();
        }

    }

    @Override
    public boolean onCreateOptionsMenu(Menu menu) {
        // Inflate the menu; this adds items to the action bar if it is present.
        getMenuInflater().inflate(R.menu.main2, menu);
        return true;
    }

    @Override
    public boolean onOptionsItemSelected(MenuItem item) {
        // Handle action bar item clicks here. The action bar will
        // automatically handle clicks on the Home/Up button, so long
        // as you specify a parent activity in AndroidManifest.xml.
        int id = item.getItemId();

        //noinspection SimplifiableIfStatement
        if (id == R.id.action_settings) {
            return true;
        }

        return super.onOptionsItemSelected(item);
    }

    @SuppressWarnings("StatementWithEmptyBody")
    @Override
    public boolean onNavigationItemSelected(MenuItem item) {
        // Handle navigation view item clicks here.
        int id = item.getItemId();

        if (id == R.id.nav_home) {
            // Handle the camera action
        } else if (id == R.id.nav_cadastro) {
            Intent intentCadastro = new Intent(this,Tela_Login_Cadastro.class);
            startActivity(intentCadastro);

        }else if (id == R.id.nav_cadRelato) {
            Intent intentRelato = new Intent(getApplicationContext(),RelatoActivity.class);
            startActivity(intentRelato);

        }else if (id == R.id.nav_logout){
            SharedPreferences.Editor deslogar = getSharedPreferences("login", MODE_PRIVATE).edit();
            deslogar.clear();
            deslogar.apply();

            Toast.makeText(this, "Deslogado com Sucesso !", Toast.LENGTH_SHORT).show();
        }

        DrawerLayout drawer = findViewById(R.id.drawer_layout);
        drawer.closeDrawer(GravityCompat.START);
        return true;
    }

    @Override
    public void onPointerCaptureChanged(boolean hasCapture) {

    }
}

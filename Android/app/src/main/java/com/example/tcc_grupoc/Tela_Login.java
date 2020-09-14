package com.example.tcc_grupoc;

import androidx.appcompat.app.AppCompatActivity;

import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.TextView;
import android.widget.Toast;

import com.android.volley.Request;
import com.android.volley.RequestQueue;
import com.android.volley.Response;
import com.android.volley.VolleyError;
import com.android.volley.toolbox.StringRequest;
import com.android.volley.toolbox.Volley;

import org.json.JSONException;
import org.json.JSONObject;

import java.util.HashMap;
import java.util.Map;

public class Tela_Login extends AppCompatActivity {

    TextView textSenha;
    EditText campoUsuario, campoSenha;
    Button btLogar, btLogarFace, btLogarGoogle;

    private String[] caminhosWebService =
            {"http://10.87.107.11/3DSAA_TCC_GRUPOC/WebServiceUsuario.asmx",
             "http://10.87.106.29/3DSAA_TCC_GRUPOC/WebServiceUsuario.asmx"};

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_tela__login);

        campoUsuario = findViewById(R.id.campoUsuario);
        campoSenha = findViewById(R.id.campoSenha);
        btLogar = findViewById(R.id.btLogar);
        btLogarFace = findViewById(R.id.btLogarFace);
        btLogarGoogle = findViewById(R.id.btLogarGoogle);
        textSenha = findViewById(R.id.txtEsqueci);

        btLogar.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                final String usuario = campoUsuario.getText().toString(),
                        senha = campoSenha.getText().toString();

                if(validadados(usuario,senha)){

                    //Aqui esta rodando um laço de repetição enchendo um RequestQueue com varias requisicoes para conectar em webservices de IPs diferentes, porém ao encontrar um ele não cancela as outras requisições
                    conectaWebService(caminhosWebService, usuario, senha);

                }
            }
        });

        View.OnClickListener desenvolvendo = new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Toast.makeText(Tela_Login.this, "Em Desenvolvimento !", Toast.LENGTH_SHORT).show();
            }
        };

        textSenha.setOnClickListener(desenvolvendo);
        btLogarGoogle.setOnClickListener(desenvolvendo);
        btLogarFace.setOnClickListener(desenvolvendo);
    }

    private boolean validadados(String usuario, String senha){
        if(usuario.isEmpty() || senha.isEmpty()){

            if(usuario.isEmpty()){

                campoUsuario.setError("Preencha o Campo usuario por favor.");
                campoUsuario.requestFocus();
            }
            if(senha.isEmpty()){

                campoSenha.setError("Preencha o Campo senha por favor.");
                campoSenha.requestFocus();
            }

            return false;
        }else {

            if (!senha.matches("^[\\w]{6,20}$")) {
                campoSenha.setError("Digite Uma Senha Válida!");

                return false;
            }

        }

        return true;
    }

    private void conectaWebService(String[] caminhosWebService, final String usuario, final String senha){

        for (final String caminho : caminhosWebService) {

            final String metodoWebService = caminho + "/logarUsuario";

            RequestQueue filaRequisicao = Volley.newRequestQueue(Tela_Login.this);

            final StringRequest requisicao = new StringRequest(Request.Method.POST, metodoWebService, new Response.Listener<String>() {

                @Override
                public void onResponse(String response) {

                    if (!response.toLowerCase().contains("incorr")) {

                        try {

                            JSONObject usuario = new JSONObject(response);

                            SharedPreferences.Editor adicionaLogin = getSharedPreferences("login", MODE_PRIVATE).edit();
                            adicionaLogin.putInt("id_usuario", usuario.getInt("id_usuario"));
                            adicionaLogin.putString("nome_usuario", usuario.getString("nome_usuario"));

                            adicionaLogin.apply();

                            startActivity(new Intent(Tela_Login.this, MainActivity.class));

                            //Toast.makeText(Tela_Login.this, "E-mail: " + usuario.getString("email_usuario") + "\nSenha: " + usuario.getString("senha_usuario"), Toast.LENGTH_SHORT).show();

                        } catch (JSONException e) {

                            Toast.makeText(Tela_Login.this, "ERRO ao tratar Resposta do Servidor " + e.getMessage(), Toast.LENGTH_SHORT).show();
                            e.printStackTrace();

                        }

                    } else {
                        Toast.makeText(Tela_Login.this, response, Toast.LENGTH_SHORT).show();
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

                    parametros.put("nome_email", usuario);
                    parametros.put("senha", senha);

                    return parametros;

                }
            };

            filaRequisicao.add(requisicao);
        }

    }

}
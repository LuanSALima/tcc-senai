package com.example.tcc_grupoc;

import androidx.appcompat.app.AppCompatActivity;

import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.EditText;
import android.widget.Toast;

import com.android.volley.Request;
import com.android.volley.RequestQueue;
import com.android.volley.Response;
import com.android.volley.VolleyError;
import com.android.volley.toolbox.StringRequest;
import com.android.volley.toolbox.Volley;

import java.util.HashMap;
import java.util.Map;

public class Tela_Cadastro extends AppCompatActivity {

    private EditText edit_usuario, edit_email, edit_senha, edit_confirmSenha;
    private Button bt_cadastrar;

    private String[] caminhosWebService =
            {"http://10.87.107.11/3DSAA_TCC_GRUPOC/WebServiceUsuario.asmx",
             "http://10.87.106.29/3DSAA_TCC_GRUPOC/WebServiceUsuario.asmx"};

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_tela_cadastro);

        edit_usuario = findViewById(R.id.edit_usuario);
        edit_email = findViewById(R.id.edit_email);
        edit_senha = findViewById(R.id.edit_senha);
        edit_confirmSenha = findViewById(R.id.edit_confirm_senha);

        bt_cadastrar = findViewById(R.id.bt_cadastrar_usuario);

        bt_cadastrar.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {

                String usuario = edit_usuario.getText().toString(),
                        email = edit_email.getText().toString(),
                        senha = edit_senha.getText().toString(),
                        confirm_senha = edit_confirmSenha.getText().toString();

                if (validaDados(usuario, email, senha, confirm_senha)) {

                    conectaWebService(caminhosWebService);

                }

            }
        });

    }

    // TERMINANDO A VALIDAÇÃO DOS DADOS
    private boolean validaDados(String usuario, String email, String senha, String confirm_senha) {

        if (usuario.isEmpty() || email.isEmpty() || senha.isEmpty() || confirm_senha.isEmpty()) {

            if(usuario.isEmpty())
                edit_usuario.setError("Preencha o Campo usuario");

            if(email.isEmpty())
                edit_email.setError("Preencha o Campo e-mail");

            if(senha.isEmpty())
                edit_senha.setError("Preencha o Campo senha");

            if(confirm_senha.isEmpty())
                edit_confirmSenha.setError("Preencha o Campo confirmar senha");

            return false;
        } else {

            if (!email.matches("^[a-z0-9.]+@[a-z0-9]+\\.[a-z]+(\\.([a-z]+))?$")) {
                //Toast.makeText(this, "E-mail Inválido !", Toast.LENGTH_SHORT).show();
                edit_email.setError("E-mail Inválido");
                return false;
            }

            if (!senha.matches("^[\\w]{6,20}$")) {
                //Toast.makeText(this, "Digite uma senha entre 6-20 caracteres !", Toast.LENGTH_SHORT).show();
                edit_senha.setError("Digite uma senha entre 6-20 caracteres");
                return false;
            }

            if (!confirm_senha.equals(senha)) {
                //Toast.makeText(this, "As senhas não coincidem !", Toast.LENGTH_SHORT).show();
                edit_senha.setError("As senhas não coincidem");
                return false;
            }

        }

        return true;
    }

    private void conectaWebService(String[] caminhosWebService) {

        for (final String caminho : caminhosWebService) {

            final String metodoWebService = caminho + "/cadastraUsuario";

            StringRequest requisicao = new StringRequest(Request.Method.POST, metodoWebService, new Response.Listener<String>() {

                @Override
                public void onResponse(String response) {

                    if (response.contains("cadastrado")) {
                        Toast.makeText(Tela_Cadastro.this, "Cadastrado com Sucesso", Toast.LENGTH_SHORT).show();
                    } else {
                        Toast.makeText(Tela_Cadastro.this, response, Toast.LENGTH_SHORT).show();
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

                    parametros.put("nome", edit_usuario.getText().toString());
                    parametros.put("email", edit_email.getText().toString());
                    parametros.put("senha", edit_senha.getText().toString());

                    return parametros;

                }
            };

            RequestQueue filaRequisicao = Volley.newRequestQueue(Tela_Cadastro.this);
            filaRequisicao.add(requisicao);
        }

    }

}

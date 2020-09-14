package com.example.tcc_grupoc;

import androidx.annotation.Nullable;
import androidx.appcompat.app.AppCompatActivity;

import android.app.Activity;
import android.content.Intent;
import android.os.Bundle;
import android.util.Log;
import android.view.View;
import android.widget.Button;
import android.widget.ImageButton;
import android.widget.Toast;

import com.google.android.gms.auth.api.signin.GoogleSignIn;
import com.google.android.gms.auth.api.signin.GoogleSignInAccount;
import com.google.android.gms.auth.api.signin.GoogleSignInClient;
import com.google.android.gms.auth.api.signin.GoogleSignInOptions;
import com.google.android.gms.common.api.ApiException;
import com.google.android.gms.tasks.Task;

public class Tela_Login_Cadastro extends AppCompatActivity {

    private Button bt_tela_cadastro, bt_tela_login;
    private ImageButton bt_login_google, bt_login_facebook;

    private GoogleSignInClient googleSignInClient;

    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_tela_login_cadastro);

        bt_tela_cadastro = findViewById(R.id.bt_tela_cadastrar);
        bt_tela_login = findViewById(R.id.bt_tela_logar);
        bt_login_google = findViewById(R.id.bt_login_google);
        bt_login_facebook = findViewById(R.id.bt_login_facebook);

        bt_tela_cadastro.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Intent tela_cadastro = new Intent(getApplicationContext(), Tela_Cadastro.class);
                startActivity(tela_cadastro);
            }
        });

        bt_tela_login.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Intent tela_login = new Intent(getApplicationContext(), Tela_Login.class);
                startActivity(tela_login);
            }
        });

        /*

                INICIO LOGIN GOOGLE

         */

        GoogleSignInOptions gso = new GoogleSignInOptions.Builder(GoogleSignInOptions.DEFAULT_SIGN_IN)
                .requestId()
                .requestEmail()
                .build();

        googleSignInClient = GoogleSignIn.getClient(this, gso);

        bt_login_google.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {

                /*
                Intent tela_google = googleSignInClient.getSignInIntent();
                startActivityForResult(tela_google, 101);
                */
                Toast.makeText(Tela_Login_Cadastro.this, "Em Desenvolvimento !", Toast.LENGTH_SHORT).show();

            }
        });

        bt_login_facebook.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                Toast.makeText(Tela_Login_Cadastro.this, "Em Desenvolvimento !", Toast.LENGTH_SHORT).show();
            }
        });

        /*

                FIM LOGIN GOOGLE

         */
    }

    @Override
    protected void onActivityResult(int requestCode, int resultCode, @Nullable Intent data) {
        super.onActivityResult(requestCode, resultCode, data);

        if(resultCode == Activity.RESULT_OK)
            switch(requestCode){
                case 101: //Caso Resultado Google
                    try{
                        Task<GoogleSignInAccount> task = GoogleSignIn.getSignedInAccountFromIntent(data);
                        GoogleSignInAccount account = task.getResult(ApiException.class);
                        Toast.makeText(this, account.getId()+" | "+account.getEmail(), Toast.LENGTH_SHORT).show();
                    } catch (ApiException e){
                        Log.w("ERRO", "signInResult:failed code=" + e.getStatusCode());

                    }
                    break;
            }
    }

}

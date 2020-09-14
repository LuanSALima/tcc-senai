package com.example.tcc_grupoc;

import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;

import androidx.appcompat.app.AppCompatActivity;

public class TermosLer extends AppCompatActivity {
    private Button li_termos;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_termos_ler);
        li_termos = findViewById(R.id.li_aceito);
        
        li_termos.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {

                SharedPreferences.Editor aceitou = getSharedPreferences("termos", MODE_PRIVATE).edit();
                aceitou.putBoolean("aceitar", true);
                aceitou.apply();

                Intent intent_termos = new Intent(getApplicationContext(), MainActivity.class);
                startActivity(intent_termos);
            }
        });
    }
}

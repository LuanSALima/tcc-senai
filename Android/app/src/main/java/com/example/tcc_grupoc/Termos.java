package com.example.tcc_grupoc;

import android.content.Intent;
import android.content.SharedPreferences;
import android.os.Bundle;
import android.view.View;
import android.widget.Button;
import android.widget.CheckBox;
import android.widget.TextView;

import androidx.appcompat.app.AppCompatActivity;

public class Termos extends AppCompatActivity {
private Button but_comecar;
private TextView txtTermos;
private CheckBox checkBox;
//CheckBox checkBox;
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_termos);

        but_comecar = findViewById(R.id.bt_comecar);
        checkBox = findViewById(R.id.checkBox_termos);
        txtTermos = findViewById(R.id.txtTermos);
        checkBox.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {
                if(checkBox.isChecked()){
                    but_comecar.setEnabled(true);

                }else{
                    but_comecar.setEnabled(false);
                }
            }
        });
        but_comecar.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View view) {

                SharedPreferences.Editor aceitou = getSharedPreferences("termos", MODE_PRIVATE).edit();
                aceitou.putBoolean("aceitar", true);
                aceitou.apply();

                Intent intent_main = new Intent(getApplicationContext(), MainActivity.class);
                startActivity(intent_main);

            }
        });
       txtTermos.setOnClickListener(new View.OnClickListener() {
           @Override
           public void onClick(View view) {
               Intent intent_ler_termos = new Intent(getApplicationContext(), TermosLer.class);
               startActivity(intent_ler_termos);
           }
       });


     }

    public void check_termos(View view) {

    }
}






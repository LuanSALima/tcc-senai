package com.example.tcc_grupoc;

import android.content.Context;
import android.os.Bundle;
import android.view.LayoutInflater;
import android.view.View;
import android.view.ViewGroup;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.ImageView;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;

import androidx.annotation.NonNull;
import androidx.annotation.Nullable;
import androidx.appcompat.app.AppCompatActivity;

public class Meus_relatos extends AppCompatActivity {
    ListView listView;
    String mTitulo [] = {"Furto","asssalto","pichaçao","tentativa de assalto","test"};
    String mDescricao [] = {"furtaram meu celular","picharam meu muro","tentaram em assaltar","furtaram meu celular","furtaram meu celular"};
    int images [] = {R.drawable.livro,R.drawable.livro,R.drawable.livro,R.drawable.livro,R.drawable.livro};
    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_meus_relatos);

      listView = findViewById(R.id.List_ViewRelatos);

        MyAdapter adapter = new MyAdapter(this,mTitulo,mDescricao,images);
        listView.setAdapter(adapter);
        listView.setOnItemClickListener(new AdapterView.OnItemClickListener() {
       @Override
       public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
           if (position == 0){
               Toast.makeText(Meus_relatos.this, "funcionou 1", Toast.LENGTH_SHORT).show();
           }
           if (position == 1){
               Toast.makeText(Meus_relatos.this, "funcionou 2", Toast.LENGTH_SHORT).show();
           }
           if (position == 2){
               Toast.makeText(Meus_relatos.this, "funcionou 3", Toast.LENGTH_SHORT).show();
           }
           if (position == 3){
               Toast.makeText(Meus_relatos.this, "funcionou 4", Toast.LENGTH_SHORT).show();
           }
           if (position == 4){
               Toast.makeText(Meus_relatos.this, "funcionou 5", Toast.LENGTH_SHORT).show();
           }
           if (position == 5){
               Toast.makeText(Meus_relatos.this, "funcionou 6", Toast.LENGTH_SHORT).show();
           }
       }
   });
    }
    class MyAdapter extends ArrayAdapter<String>{

        Context context ;
        String rTitulo [];
        String rDescricao [];
        int rImgs [];

        MyAdapter(Context c, String titulo[], String descricao[], int imgs[]){
            super(c, R.layout.row, R.id.txtViewTitulo,titulo);
            this.context = c;
            this.rTitulo = titulo;
            this.rDescricao = descricao;
            this.rImgs = imgs;

        }

        @NonNull
        @Override
        public View getView(int position, @Nullable View convertView, @NonNull ViewGroup parent) {
            LayoutInflater layoutInflater = (LayoutInflater)getApplicationContext().getSystemService(Context.LAYOUT_INFLATER_SERVICE);
            View row = layoutInflater.inflate(R.layout.row,parent,false);
            ImageView images = row.findViewById(R.id.imageRelato);
            TextView myTitulo = row.findViewById(R.id.txtViewTitulo);
            TextView myDescricao = row.findViewById(R.id.TxtViewDescricao);
            images.setImageResource(rImgs[position]);
            myTitulo.setText(rTitulo[position]);
            myDescricao.setText(rDescricao[position]);
            return row;
        }
    }
}

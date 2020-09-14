package com.example.tcc_grupoc;

import androidx.appcompat.app.AppCompatActivity;

import android.location.Geocoder;
import android.os.Bundle;
import android.view.View;
import android.widget.AdapterView;
import android.widget.ArrayAdapter;
import android.widget.Button;
import android.widget.EditText;
import android.widget.ListView;
import android.widget.TextView;
import android.widget.Toast;

import java.io.IOException;
import java.util.ArrayList;
import java.util.HashMap;
import java.util.List;
import java.util.Locale;

public class tela_buscarelatos extends AppCompatActivity {

    //Variaveis
    private List<Relato> relatosCadastrados = new ArrayList<>();

    private ListView listaOcorrencias;

    private EditText editTextRua;

    private Button botaoBuscar;


    @Override
    protected void onCreate(Bundle savedInstanceState) {
        super.onCreate(savedInstanceState);
        setContentView(R.layout.activity_tela_buscarelatos);

        //Adicionando as referencias do layout
        listaOcorrencias = findViewById(R.id.listViewRelatos);

        botaoBuscar = findViewById(R.id.btBuscar);
        editTextRua  = findViewById(R.id.editTextRua);

		//
        //Preenchendo a lista de relatos cadastrados para simular uma busca no banco de dados (fins de teste)
		//
		/*
		Modelo que usei para simular cadastros
		
        relatosCadastrados.add(
                new Relato(
                        int idRelato,
                        int idUsuario,
                        String nomeUsuario,
                        int idTipoRelato,
                        String nomeTipoRelato,
                        String descricao,
                        double localizacao_X,
                        double localizacao_Y,
                        String data,
                        String horario,
                        int anonimo));
		*/
		
		//Relato cadastrado com latitude e longitude da rua do SENAI Zerbini (Av. da Saudade, 133-101 - Pte. Preta, Campinas - SP, 13041-670) 
		relatosCadastrados.add(
                new Relato(
                        1,
                        1,
                        "Joãozinho",
                        1,
                        "Assalto",
                        "Fui assaltado na frente do SENAI",
                        -22.914995,
                        -47.056356,
                        "10/02/2019",
                        "10:40:02",
                        0));
		
		//Relato cadastrado com latitude e longitude da rua do SENAI Zerbini (Av. da Saudade, 133-101 - Pte. Preta, Campinas - SP, 13041-670)
		relatosCadastrados.add(
                new Relato(
                        2,
                        2,
                        "Pedrinho",
                        2,
                        "Homicidio",
                        "Mataram na frente do SENAI",
                        -22.914995,
                        -47.056356,
                        "15/02/2019",
                        "13:40:02",
                        1));
						
		//Relato cadastrado com latitude e longitude da rua do SENAI Zerbini (Av. da Saudade, 133-101 - Pte. Preta, Campinas - SP, 13041-670)
		relatosCadastrados.add(
                new Relato(
                        3,
                        3,
                        "Mariazinha",
                        2,
                        "Homicidio",
                        "Mataram na frente do SENAI",
                        -22.914995,
                        -47.056356,
                        "15/02/2019",
                        "13:40:02",
                        1));
						
		//Relato cadastrado com latitude e longitude da avenida Campos Sales (Av. Campos Sales, 500-598 - Centro, Campinas - SP, 13010-080) 
		relatosCadastrados.add(
                new Relato(
                        4,
                        1,
                        "Joãozinho",
                        1,
                        "Assalto",
                        "Fui assaltado na Campos Sales",
                        -22.905934,
                        -47.063336,
                        "10/02/2019",
                        "10:40:02",
                        1));
		
		//Relato cadastrado com latitude e longitude da avenida Campos Sales (Av. Campos Sales, 500-598 - Centro, Campinas - SP, 13010-080)
		relatosCadastrados.add(
                new Relato(
                        5,
                        2,
                        "Pedrinho",
                        2,
                        "Homicidio",
                        "Mataram na Campos Sales",
                        -22.905934,
                        -47.063336,
                        "15/02/2019",
                        "13:40:02",
                        0));				
		
		//Relato cadastrado com latitude e longitude da avenida Campos Sales (Av. Campos Sales, 500-598 - Centro, Campinas - SP, 13010-080)
		relatosCadastrados.add(
                new Relato(
                        6,
                        3,
                        "Mariazinha",
                        2,
                        "Homicidio",
                        "Mataram na Campos Sales",
                        -22.905934,
                        -47.063336,
                        "15/02/2019",
                        "13:40:02",
                        0));		
			
		// Latitude e Longitude usadas (Google Maps)
		//-22.914995, -47.056356  -  RUA SENAI
		//-22.905934, -47.063336  -  Av. Campos Sales
			
			
        //Ação de Clicar no Botão Buscar
        botaoBuscar.setOnClickListener(new View.OnClickListener() {
            @Override
            public void onClick(View v) {

                //Guarda o valor da rua digitada pelo usuário
                String ruaProcurada = editTextRua.getText().toString().trim();
				
				//Formatando a rua digitada para retirar espaços duplos(causam problemas para buscar rua)
				while(ruaProcurada.contains("  ")){
					ruaProcurada = ruaProcurada.replace("  ", " ");
				}

                //Cria um List<> que guardará todos os relatos que aconteceram na rua digitada
                final List<Relato> relatosEncontrados = new ArrayList<>();
                final List<String> fraseRelatoEncontrado = new ArrayList<>();

                try {
                    //Para cada relato cadastrado
                    for (Relato relato : relatosCadastrados) {

                        //Converte o valor latitude x longitude em endereço
                        android.location.Address enderecoRelato = new Geocoder(getApplicationContext(), Locale.getDefault())
                                .getFromLocation(relato.getLocalizacao_X(), relato.getLocalizacao_Y(), 1)
                                .get(0);

                        //Guarda o valor do nome da rua a partir do endereço convertido
                        String nomeRuaRelato = enderecoRelato.getAdressLine(0);

                        //Reparte a rua escrita em uma lista de cada palavra (separa cada " " e entre cada " " cria um item e coloca numa lista)


                        for (String palavraEscrita : ruaProcurada.split(" ")) {

                            //Se essa palavra que foi repartida do que foi escrito existir dentro do endereço que o relato foi cadastrado
                            if (nomeRuaRelato.toLowerCase().indexOf(palavraEscrita.toLowerCase()) != -1) {

                                //Adiciona o relato a uma lista de relatos e adiciona uma frase com o tipoRelato e a Rua
                                relatosEncontrados.add(relato);
                                fraseRelatoEncontrado.add("Ocorrencia: " + relato.getNome_tipoRelato() + "   Rua: " + nomeRuaRelato);
                                break;

                            }

                        }

                        /*      APAGAR ESTE COMENTÁRIO CASO O CÓDIGO DE CIMA FUNCIONAR, PARA QUE SEJA TESTADO A CONTAGEM DE TIPO RELATOS ENCONTRADOS

                        //Cria uma lista que guarda ID tipo Relato e a quantidade encontrada
                        HashMap<String, Integer> quantidadeTipoRelato = new HashMap<>();

                        //Para cada Relato que possui o endereço Procurado
                        for(Relato relatoEncontrado : relatosEncontrados){

                            //Guarda o valor do ID Tipo Relato
                            String nomeTipoRelato = relatoEncontrado.getNome_tipoRelato();

                            //Se já possuir um tiporelato na lista de contagem de tipo relatos
                            if( quantidadeTipoRelato.containsKey(nomeTipoRelato) ){
                                //Aumenta em 1 o valor de relatos ja encontrados
                                quantidadeTipoRelato.put(nomeTipoRelato, (quantidadeTipoRelato.get(nomeTipoRelato) + 1));

                            } else{ //Se não existir o tipo relato cadastrado
                                //Cria o primeiro tipoRelato na lista
                                quantidadeTipoRelato.put(nomeTipoRelato, 1);
                            }

                        }

                        //Limpa a lista populada anteriormente para agora ser usada para apresentar a quantidade de cada Tipo Relato encontrado
                        fraseRelatoEncontrado.clear();

                        //Para cada item na lista que guarda a quantidade de tipo relatos
                        for(String tipoRelato : quantidadeTipoRelato.keySet()){

                            fraseRelatoEncontrado.add(quantidadeTipoRelato.get(tipoRelato)+" - "+tipoRelato);

                        }

                        */

                    }

                }catch (IOException erroIO){
                    Toast.makeText(tela_buscarelatos.this, "ERRO: "+erroIO.getMessage(), Toast.LENGTH_LONG).show();
                }

                //Cria um adaptador de lista e adiciona cada item da lista na ListView
                ArrayAdapter<String> fraseRelatoAdapter = new ArrayAdapter<String>(getApplicationContext(), android.R.layout.simple_list_item_1, fraseRelatoEncontrado);
                listaOcorrencias.setAdapter(fraseRelatoAdapter);

                //Ao clicar em um item da lista, mostra a descrição do relato referente ao item da lista
                listaOcorrencias.setOnItemClickListener(new AdapterView.OnItemClickListener() {
                    @Override
                    public void onItemClick(AdapterView<?> parent, View view, int position, long id) {
                        Relato relatoClicado = relatosEncontrados.get(position);
                        Toast.makeText(tela_buscarelatos.this, relatoClicado.getDescricao(), Toast.LENGTH_LONG).show();
                    }
                });

            }
        });
    }
}

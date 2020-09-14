CREATE DATABASE 3dsa_tcc_grupoc;

use 3dsa_tcc_grupoc;

show tables;

CREATE TABLE tb_usuarios (
  id_usuario smallint(5) NOT NULL AUTO_INCREMENT,
  nome_usuario varchar(40) NOT NULL,
  email_usuario varchar(30) NOT NULL,
  senha_usuario varchar(20) NOT NULL,
  PRIMARY KEY (id_usuario)
);

CREATE TABLE tb_tipoRelatos(
	id_tipoRelato smallint(2) NOT NULL AUTO_INCREMENT,
    nome_tipoRelato varchar(25) NOT NULL,
    PRIMARY KEY (id_tipoRelato)
);

CREATE TABLE tb_relatos (
  id_relato smallint(6) NOT NULL AUTO_INCREMENT,
  fk_id_usuario smallint(5) NOT NULL,
  fk_id_tipoRelato smallint(2) NOT NULL,
  descricao_relato varchar(100) NOT NULL,
  localizacao_x_relato decimal(10, 8) NOT NULL,
  localizacao_y_relato decimal(11, 8) NOT NULL,
  rua_relato varchar(50) NULL, /*NULL porque apenas o WebSite possui a opção de resgatar rua*/
  cidade_relato varchar(40) NULL, /*NULL porque apenas o WebSite possui a opção de resgatar cidade*/
  estado_relato varchar(30) NULL, /*NULL porque apenas o WebSite possui a opção de resgatar estado*/
  data_relato date NOT NULL,
  horario_relato time NOT NULL,
  anonimo_relato smallint(1) NOT NULL,
  PRIMARY KEY (id_relato)
);

show columns from tb_usuarios;

show columns from tb_tipoRelatos;

show columns from tb_relatos;

ALTER TABLE tb_relatos ADD FOREIGN KEY (fk_id_usuario) REFERENCES tb_usuarios(id_usuario);
ALTER TABLE tb_relatos ADD FOREIGN KEY (fk_id_tipoRelato) REFERENCES tb_tipoRelatos(id_tipoRelato);

INSERT INTO tb_usuarios VALUES(null, "José Ferreira Almeida", "jose.almeida@hotmail.com", "J0$34LM31D4");
INSERT INTO tb_tipoRelatos VALUES(null, "Assalto");
INSERT INTO tb_relatos VALUES(null, 1, 1, "Fui roubado do lado do SENAI Zerbini", -22.914833, -47.055931, null, null, null, 20200220, 173000, 0);
INSERT INTO tb_relatos VALUES(null, 1, 1, "Roubaram meu tio do lado do SENAI Zerbini", -22.914833, -47.055931, "Avenida da Saudade", "Campinas", "São Paulo", 20200220, 173000, 1);

SELECT * FROM tb_usuarios;
SELECT * FROM tb_tipoRelatos;
SELECT * FROM tb_relatos;

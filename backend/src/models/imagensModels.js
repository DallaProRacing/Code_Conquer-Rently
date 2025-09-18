const pool = require("../config/db");

exports.buscarImagens = async () => {
  const result = await pool.query("SELECT * FROM imagens");
  return result.rows;
};

exports.buscarImagensPorProduto = async (idProduto) => {
  const result = await pool.query(
    'SELECT * FROM imagens WHERE "idProduto" = $1',
    [idProduto]
  );
  return result.rows;
};

exports.adicionarImagem = async (idProduto, urlImagem, ordemExibicao) => {
  const result = await pool.query(
    'INSERT INTO imagens ("idProduto", "urlImagem", "ordemExibicao") VALUES ($1, $2, $3) RETURNING *',
    [idProduto, urlImagem, ordemExibicao]
  );
  return result.rows[0];
};

exports.atualizarImagem = async (
  idImagem,
  idProduto,
  urlImagem,
  ordemExibicao
) => {
  const result = await pool.query(
    'UPDATE imagens SET "idProduto" = $1, "urlImagem" = $2, "ordemExibicao" = $3 WHERE "idImagem" = $4 RETURNING *',
    [idProduto, urlImagem, ordemExibicao, idImagem]
  );
  return result.rows[0];
};

exports.deletarImagem = async (idImagem) => {
  const result = await pool.query(
    'DELETE FROM imagens WHERE "idImagem" = $1 RETURNING *',
    [idImagem]
  );
  return result.rows[0];
};

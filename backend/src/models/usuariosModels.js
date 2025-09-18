const pool = require("../config/db");

exports.buscarUsuarios = async () => {
  const result = await pool.query("SELECT * FROM usuarios");
  return result.rows;
};

exports.adicionarUsuario = async (
  email,
  senha,
  telefone,
  cpfCNPJ,
  fotoPerfil,
  idTipoUsuario,
  idEndereco
) => {
  const result = await pool.query(
    'INSERT INTO usuarios ("emailUsuario", "senhaUsuario", "telefoneUsuario", "cpfCNPJ", "fotoPerfil", "idTipoUsuario", "idEndereco") VALUES ($1, $2, $3, $4, $5, $6, $7) RETURNING *',
    [email, senha, telefone, cpfCNPJ, fotoPerfil, idTipoUsuario, idEndereco]
  );
  return result.rows[0];
};

exports.atualizarUsuario = async (
  id,
  email,
  senha,
  telefone,
  cpfCNPJ,
  fotoPerfil,
  idTipoUsuario,
  idEndereco
) => {
  const result = await pool.query(
    'UPDATE usuarios SET "emailUsuario" = $1, "senhaUsuario" = $2, "telefoneUsuario" = $3, "cpfCNPJ" = $4, "fotoPerfil" = $5, "idTipoUsuario" = $6, "idEndereco" = $7 WHERE "idUsuario" = $8 RETURNING *',
    [email, senha, telefone, cpfCNPJ, fotoPerfil, idTipoUsuario, idEndereco, id]
  );
  return result.rows[0];
};

exports.deletarUsuario = async (id) => {
  const result = await pool.query(
    'DELETE FROM usuarios WHERE "idUsuario" = $1 RETURNING *',
    [id]
  );
  return result.rows[0];
};

exports.alterarFotoPerfil = async (id, fotoPerfil) => {
  const result = await pool.query(
    'UPDATE usuarios SET "fotoPerfil" = $1 WHERE "idUsuario" = $2 RETURNING *',
    [fotoPerfil, id]
  );
  return result.rows[0];
};

exports.buscarUsuarioPorEmail = async (email) => {
    const result = await pool.query(
        'SELECT * FROM usuarios WHERE "emailUsuario" = $1',
        [email]
    );
    return result.rows[0];
};

exports.buscarUsuarioPorId = async (id) => {
    const result = await pool.query(
        'SELECT * FROM usuarios WHERE "idUsuario" = $1',
        [id]
    );
    return result.rows[0];
}

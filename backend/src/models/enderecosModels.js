const pool = require('../config/db');

exports.buscarEnderecos = async () => {
    const result = await pool.query(
        'SELECT * FROM enderecos'
    );
    return result.rows;
};  

exports.adicionarEndereco = async (cep, rua, numero, bairro, cidade, uf, complemento) => {
    const result = await pool.query(
        'INSERT INTO enderecos ("cepEndereco", "ruaEndereco", "numeroEndereco", "bairroEndereco", "cidadeEndereco", "ufEndereco", "complementoEndereco") VALUES ($1, $2, $3, $4, $5, $6, $7) RETURNING *',
        [cep, rua, numero, bairro, cidade, uf, complemento]
    );
    return result.rows[0];
};

exports.atualizarEndereco = async (id, cep, rua, numero, bairro, cidade, uf, complemento) => {
    const result = await pool.query(
        'UPDATE enderecos SET "cepEndereco" = $1, "ruaEndereco" = $2, "numeroEndereco" = $3, "bairroEndereco" = $4, "cidadeEndereco" = $5, "ufEndereco" = $6, "complementoEndereco" = $7 WHERE "idEndereco" = $8 RETURNING *',
        [cep, rua, numero, bairro, cidade, uf, complemento, id]
    );
    return result.rows[0];
};

exports.deletarEndereco = async (id) => {
    const result = await pool.query(
        'DELETE FROM enderecos WHERE id = $1 RETURNING *',
        [id]
    );
    return result.rows[0];
};
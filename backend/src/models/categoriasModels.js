const pool = require('../config/db');

exports.buscarCategorias = async () => {
    const result = await pool.query(
        'SELECT * FROM categorias'
    );
    return result.rows;
};

exports.adicionarCategoria = async (nome) => {
    const result = await pool.query(
        'INSERT INTO categorias ("nomeCategoria") VALUES ($1) RETURNING *',
        [nome]
    );
    return result.rows[0];
};

exports.atualizarCategoria = async (id, nome) => {
    const result = await pool.query(
        'UPDATE categorias SET "nomeCategoria" = $1 WHERE "idCategoria" = $2 RETURNING *',
        [nome, id]
    );
    return result.rows[0];
};

exports.deletarCategoria = async (id) => {
    const result = await pool.query(
        'DELETE FROM categorias WHERE "idCategoria" = $1 RETURNING *',
        [id]
    );
    return result.rows[0];
};

exports.buscarCategoriaPorNome = async (nome) => {
    const result = await pool.query(
        'SELECT * FROM categorias WHERE "nomeCategoria" ILIKE $1',
        [`%${nome}%`]
    );
    return result.rows;
};
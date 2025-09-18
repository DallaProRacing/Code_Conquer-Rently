// idProduto INTEGER PRIMARY KEY,
// idUsuario INTEGER NOT NULL,
// idCategoria INTEGER NOT NULL,
// nomeProduto VARCHAR NOT NULL,
// precoProduto NUMERIC NOT NULL,
// idVendaAluguel INTEGER NOT NULL,
// idEndereco INTEGER,
// descricaoProduto VARCHAR

const pool = require('../config/db');

exports.buscarProdutos = async () => {
    const result = await pool.query(
        'SELECT * FROM produtos'
    );
    return result.rows;
};

exports.adicionarProduto = async(idUsuario, idCategoria, nome, preco, idVendaAluguel, idEndereco, descricao, unidadeDePreco) => {
    const result = await pool.query(
        `INSERT INTO produtos 
        ("idUsuario", "idCategoria", "nomeProduto", "precoProduto", "idVendaAluguel", "idEndereco", "descricaoProduto", "unidadeDePreco") VALUES
        ($1, $2, $3, $4, $5, $6, $7, $8) RETURNING *`,
        [idUsuario, idCategoria, nome, preco, idVendaAluguel, idEndereco, descricao, unidadeDePreco]
    );
    return result.rows[0];
};

exports.atualizarProduto = async(id, idUsuario, idCategoria, nome, preco, idVendaAluguel, idEndereco, descricao, unidadeDePreco) => {
    const result = await pool.query(
        `UPDATE produtos SET 
        "idUsuario" = $1,
        "idCategoria" = $2,
        "nomeProduto" = $3,
        "precoProduto" = $4,
        "idVendaAluguel" = $5,
        "idEndereco" = $6,
        "descricaoProduto" = $7,
        "unidadeDePreco" = $8
        WHERE "idProduto" = $9 RETURNING *`,
        [idUsuario, idCategoria, nome, preco, idVendaAluguel, idEndereco, descricao, unidadeDePreco, id]
    );
    return result.rows[0];
};

exports.deletarProduto = async (id) => {
    const result = await pool.query(
        'DELETE FROM produtos WHERE "idProduto" = $1 RETURNING *',
        [id]
    );
    return result.rows[0];
};
// idProduto INTEGER PRIMARY KEY,
// idUsuario INTEGER NOT NULL,
// idCategoria INTEGER NOT NULL,
// nomeProduto VARCHAR NOT NULL,
// precoProduto NUMERIC NOT NULL,
// idVendaAluguel INTEGER NOT NULL,
// idEndereco INTEGER,
// descricaoProduto VARCHAR

const categoriasModel = require('../models/produtosModels');

exports.buscarProdutos = async (req, res) => {
    try {
        const result = await categoriasModel.buscarProdutos();
        res.status(200).json(result);
    } catch (error) {
        console.error(error);
        res.status(500).json({ error: 'Erro ao buscar produtos' });
    }
};

exports.adicionarProduto = async (req, res) => {
    const { idUsuario, idCategoria, nome, preco, idVendaAluguel, idEndereco, descricao, unidadeDePreco } = req.body;
    if (!idUsuario || isNaN(idUsuario)) {
        return res.status(400).json({ error: 'ID do usuário inválido' });
    }
    if (!idCategoria || isNaN(idCategoria)) {
        return res.status(400).json({ error: 'ID da categoria inválido' });
    }
    if (!nome) {
        return res.status(400).json({ error: 'Nome do produto é obrigatório' });
    }
    if (!preco || isNaN(preco)) {
        return res.status(400).json({ error: 'Preço do produto inválido' });
    }
    if (!idVendaAluguel || isNaN(idVendaAluguel)) {
        return res.status(400).json({ error: 'ID de venda/aluguel inválido' });
    }
    if (!unidadeDePreco) {
        return res.status(400).json({ error: 'Unidade de preço do produto é obrigatório' });
    }

    try {
        const novoProduto = await categoriasModel.adicionarProduto(idUsuario, idCategoria, nome, preco, idVendaAluguel, idEndereco, descricao, unidadeDePreco);
        res.status(201).json(novoProduto);
    } catch (error) {
        console.error(error);
        res.status(500).json({ error: 'Erro ao adicionar produto' });
    }
};

exports.atualizarProduto = async (req, res) => {
    const { id } = req.params;
    const { idUsuario, idCategoria, nome, preco, idVendaAluguel, idEndereco, descricao, unidadeDePreco } = req.body;
    if (!id || isNaN(id)) {
        return res.status(400).json({ error: 'ID do produto inválido' });
    }
    if (!idUsuario || isNaN(idUsuario)) {
        return res.status(400).json({ error: 'ID do usuário inválido' });
    }
    if (!idCategoria || isNaN(idCategoria)) {
        return res.status(400).json({ error: 'ID da categoria inválido' });
    }
    if (!nome) {
        return res.status(400).json({ error: 'Nome do produto é obrigatório' });
    }
    if (!preco || isNaN(preco)) {
        return res.status(400).json({ error: 'Preço do produto inválido' });
    }
    if (!idVendaAluguel || isNaN(idVendaAluguel)) {
        return res.status(400).json({ error: 'ID de venda/aluguel inválido' });
    }
    if (!unidadeDePreco) {
        return res.status(400).json({ error: 'Unidade de preço do produto é obrigatório' });
    }

    try {
        const produtoAtualizado = await categoriasModel.atualizarProduto(id, idUsuario, idCategoria, nome, preco, idVendaAluguel, idEndereco, descricao, unidadeDePreco);
        if (!produtoAtualizado) {
            return res.status(404).json({ error: 'Produto não encontrado' });
        }
        res.status(200).json(produtoAtualizado);
    } catch (error) {
        console.error(error);
        res.status(500).json({ error: 'Erro ao atualizar produto' });
    }
};

exports.deletarProduto = async (req, res) => {
    const { id } = req.params;
    if (!id || isNaN(id)) {
        return res.status(400).json({ error: 'ID do produto inválido' });
    }
    try {
        const produtoDeletado = await categoriasModel.deletarProduto(id);
        if (!produtoDeletado) {
            return res.status(404).json({ error: 'Produto não encontrado' });
        }
        res.status(200).json(produtoDeletado);
    } catch (error) {
        console.error(error);
        res.status(500).json({ error: 'Erro ao deletar produto' });
    }
};
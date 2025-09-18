const categoriasModel = require('../models/categoriasModels');

exports.buscarCategorias = async (req, res) => {
    try {
        const result = await categoriasModel.buscarCategorias();
        res.status(200).json(result);
    } catch (error) {
        console.error(error);
        res.status(500).json({ error: 'Erro ao buscar categorias' });
    }
};

exports.adicionarCategoria = async (req, res) => {
    const { nome } = req.body;

    if (!nome) {
        return res.status(400).json({ error: 'Nome da categoria é obrigatório' });
    }

    try {
        const verificarCategoria = await categoriasModel.buscarCategoriaPorNome(nome);
        if (verificarCategoria.length > 0) {
            return res.status(409).json({ error: 'Categoria já existe' });
        }

        const novaCategoria = await categoriasModel.adicionarCategoria(nome);
        res.status(201).json(novaCategoria);

    } catch (error) {
        console.error(error);
        res.status(500).json({ error: 'Erro ao adicionar categoria' });
    }
};

exports.atualizarCategoria = async (req, res) => {
    const { id } = req.params;
    const { nome } = req.body;

    if (!id || isNaN(id)) {
            return res.status(400).json({ error: 'ID da categoria inválido' });
    }
    if (!nome) {
        return res.status(400).json({ error: 'Nome da categoria é obrigatório' });
    }

    try {
        const categoriaExistente = await categoriasModel.buscarCategoriaPorNome(nome);
        if (categoriaExistente.length > 0) {
            return res.status(409).json({ error: 'Categoria com esse nome já existe' });
        }

        const categoriaAtualizada = await categoriasModel.atualizarCategoria(id, nome);
        if (!categoriaAtualizada) {
            return res.status(404).json({ error: 'Categoria não encontrada' });
        }
        res.status(200).json(categoriaAtualizada);

    } catch (error) {
        console.error(error);
        res.status(500).json({ error: 'Erro ao atualizar categoria' });
    }
};

exports.deletarCategoria = async (req, res) => {
    const { id } = req.params;
    if (!id || isNaN(id)) {
        return res.status(400).json({ error: 'ID da categoria inválido' });
    }
    try {
        const categoriaDeletada = await categoriasModel.deletarCategoria(id);
        if (!categoriaDeletada) {
            return res.status(404).json({ error: 'Categoria não encontrada' });
        }
        res.status(200).json({ message: 'Categoria deletada com sucesso' });

    } catch (error) {
        console.error(error);
        res.status(500).json({ error: 'Erro ao deletar categoria' });
    }
};
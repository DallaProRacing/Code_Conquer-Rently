const enderecosModels = require('../models/enderecosModels');

exports.buscarEnderecos = async (req, res) => {
    try {
        const result = await enderecosModels.buscarEnderecos();
        res.status(200).json(result);
    } catch (error) {
        console.error(error);
        res.status(500).json({ error: 'Erro ao buscar endereços' });
    }
};

exports.adicionarEndereco = async (req, res) => {
    const { cep, rua, numero, bairro, cidade, uf, complemento } = req.body;
    if (!cep || !rua || !numero || !bairro || !cidade || !uf) {
        return res.status(400).json({ error: 'Campos obrigatórios não preenchidos' });
    }
    try {
        const novoEndereco = await enderecosModels.adicionarEndereco(cep, rua, numero, bairro, cidade, uf, complemento);
        res.status(201).json(novoEndereco);
    } catch (error) {
        console.error(error);
        res.status(500).json({ error: 'Erro ao adicionar endereço' });
    }
};

exports.atualizarEndereco = async (req, res) => {
    const { id } = req.params;
    const { cep, rua, numero, bairro, cidade, uf, complemento } = req.body;
    if (!id || isNaN(id)) {
        return res.status(400).json({ error: 'ID do endereço inválido' });
    }
    if (!cep || !rua || !numero || !bairro || !cidade || !uf) {
        return res.status(400).json({ error: 'Campos obrigatórios não preenchidos' });
    }
    try {
        const enderecoAtualizado = await enderecosModels.atualizarEndereco(id, cep, rua, numero, bairro, cidade, uf, complemento);
        if (!enderecoAtualizado) {
            return res.status(404).json({ error: 'Endereço não encontrado' });
        }
        res.status(200).json(enderecoAtualizado);
    } catch (error) {
        console.error(error);
        res.status(500).json({ error: 'Erro ao atualizar endereço' });
    }
};

exports.deletarEndereco = async (req, res) => {
    const { id } = req.params;
    if (!id || isNaN(id)) {
        return res.status(400).json({ error: 'ID do endereço inválido' });
    }
    try {
        const enderecoDeletado = await enderecosModels.deletarEndereco(id);
        if (!enderecoDeletado) {
            return res.status(404).json({ error: 'Endereço não encontrado' });
        }
        res.status(200).json({ message: 'Endereço deletado com sucesso' });
    } catch (error) {
        console.error(error);
        res.status(500).json({ error: 'Erro ao deletar endereço' });
    }
};
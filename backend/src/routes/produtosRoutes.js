const express = require('express');
const router = express.Router();
const produtosController = require('../controllers/produtosController');

router.get('/', produtosController.buscarProdutos);
router.post('/', produtosController.adicionarProduto);
router.put('/:id', produtosController.atualizarProduto);
router.delete('/:id', produtosController.deletarProduto);

module.exports = router;
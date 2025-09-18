const express = require('express');
const router = express.Router();
const categoriasController = require('../controllers/categoriasController');
// const token = require('../middlewares/authMiddleWare');


router.get('/', categoriasController.buscarCategorias);
router.post('/', categoriasController.adicionarCategoria);
router.put('/:id', categoriasController.atualizarCategoria);
router.delete('/:id', categoriasController.deletarCategoria);

module.exports = router;
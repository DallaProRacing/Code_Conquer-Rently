const express = require("express");
const router = express.Router();
const usuariosController = require("../controllers/usuariosController");
const multer = require("multer");
const storage = multer.memoryStorage();
const upload = multer({ storage });

router.get("/", usuariosController.buscarUsuarios);
router.post("/", usuariosController.adicionarUsuario);
router.post("/login", usuariosController.efetuarLogin);
router.get('/:email', usuariosController.verificarSeEmailCadastrado);
router.put(
  "/foto/:id",
  upload.single("fotoPerfil"),
  usuariosController.atualizarFotoPerfil
);

module.exports = router;

const express = require("express");
const upload = require("../config/multerConfig");
const ImagemController = require("../controllers/imagemController");
const cloudinary = require("../config/cloudinaryConfig");

const router = express.Router();

// Rota para teste
router.post("/upload-teste", upload.single("imagem"), (req, res) => {
  if (!req.file) {
    return res.status(400).json({ error: "Nenhum arquivo enviado" });
  }

  res.json({
    message: "Upload realizado com sucesso!",
    url: req.file.path, // URL pública no Cloudinary
    public_id: req.file.filename,
  });
});

router.post(
  "/:idProduto",
  upload.single("imagem"),
  ImagemController.upload
);

router.get("/:idProduto", ImagemController.listarPorProduto);

router.delete("/:idImagem", ImagemController.deletar);

module.exports = router;

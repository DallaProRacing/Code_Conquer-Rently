const Imagem = require("../models/imagensModels");
const cloudinary = require("../config/cloudinaryConfig");

const ImagemController = {
  async upload(req, res) {
    try {
      const { idProduto } = req.params;
      const { path, filename } = req.file; // do multer
      const ordemExibicao = req.body.ordemExibicao || "1";

      const imagem = await Imagem.adicionarImagem(
        idProduto,
        path,
        ordemExibicao
      );

      res.json({
        message: "Imagem enviada com sucesso",
        imagem,
        public_id: filename,
      });
    } catch (error) {
      console.error(error);
      res.status(500).json({ error: "Erro ao enviar imagem" });
    }
  },

  async listarPorProduto(req, res) {
    try {
      const { idProduto } = req.params;
      const imagens = await Imagem.buscarImagensPorProduto(idProduto);
      res.json(imagens);
    } catch (error) {
      console.error(error);
      res.status(500).json({ error: "Erro ao listar imagens" });
    }
  },

  async deletar(req, res) {
    try {
      const { idImagem } = req.params;
      const { public_id } = req.body; // precisa do Cloudinary

      const imagem = await Imagem.deletarImagem(idImagem);
      if (!imagem) {
        return res.status(404).json({ error: "Imagem não encontrada" });
      }

      // exclui do Cloudinary
      if (public_id) {
        await cloudinary.uploader.destroy(public_id);
      }

      res.json({ message: "Imagem deletada com sucesso" });
    } catch (error) {
      console.error(error);
      res.status(500).json({ error: "Erro ao deletar imagem" });
    }
  },
};

module.exports = ImagemController;

const express = require("express");
const cors = require("cors");

const app = express();
app.use(cors());
app.use(express.json());

// Importação das rotas
const categoriasRoutes = require("./routes/categoriasRoutes");
const enderecosRoutes = require("./routes/enderecosRoutes");
const usuariosRoutes = require("./routes/usuariosRoutes");
const produtosRoutes = require("./routes/produtosRoutes");
// const tipoUsuarioRoutes = require("./routes/tipoUsuarioRoutes");
// const produtoRoutes = require("./routes/produtoRoutes");
const imagemRoutes = require("./routes/imagemRoutes");
// const enderecoRoutes = require("./routes/enderecoRoutes");
// const imagemRoutes = require("./routes/imagemRoutes");
// const vendaAluguelRoutes = require("./routes/vendaAluguelRoutes");

app.use("/categorias", categoriasRoutes);
app.use("/enderecos", enderecosRoutes);
app.use("/usuarios", usuariosRoutes);
app.use("/produtos", produtosRoutes);
// app.use("/tipos-usuario", tipoUsuarioRoutes);
// app.use("/produtos", produtoRoutes);
app.use("/imagens", imagemRoutes);
// app.use("/enderecos", enderecoRoutes);
// app.use("/imagens", imagemRoutes);
// app.use("/venda-aluguel", vendaAluguelRoutes);*/

app.get("/health/", (req, res) => {
  res.json({
    status: "OK",
    timestamp: new Date().toISOString(),
    version: "1.0.0",
  });
});

// Middleware para rotas não encontradas
app.use("*", (req, res) => {
  res.status(404).json({
    error: "Rota não encontrada",
    path: req.originalUrl,
    method: req.method,
  });
});

// Middleware de tratamento de erros
app.use((err, req, res, next) => {
  console.error("Erro:", err.stack);
  res.status(500).json({
    error: "Erro interno do servidor",
    message:
      process.env.NODE_ENV === "development" ? err.message : "Algo deu errado!",
  });
});

module.exports = app;

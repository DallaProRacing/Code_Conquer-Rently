const { VendaAluguel } = require("../models/vendaAluguelModels");

exports.getAll = async (req, res) => {
  res.json(await VendaAluguel.findAll());
};

exports.getById = async (req, res) => {
  const dado = await VendaAluguel.findByPk(req.params.id);
  if (!dado) return res.status(404).json({ msg: "Venda/Aluguel não encontrado" });
  res.json(dado);
};

exports.create = async (req, res) => {
  res.json(await VendaAluguel.create(req.body));
};

exports.update = async (req, res) => {
  const dado = await VendaAluguel.findByPk(req.params.id);
  if (!dado) return res.status(404).json({ msg: "Venda/Aluguel não encontrado" });
  await dado.update(req.body);
  res.json(dado);
};

exports.remove = async (req, res) => {
  const dado = await VendaAluguel.findByPk(req.params.id);
  if (!dado) return res.status(404).json({ msg: "Venda/Aluguel não encontrado" });
  await dado.destroy();
  res.json({ msg: "Removido" });
};

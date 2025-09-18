const { TipoUsuario } = require("../models/tiposUsuarioModels");

exports.getAll = async (req, res) => {
  res.json(await TipoUsuario.findAll());
};

exports.getById = async (req, res) => {
  const dado = await TipoUsuario.findByPk(req.params.id);
  if (!dado) return res.status(404).json({ msg: "Tipo de usuário não encontrado" });
  res.json(dado);
};

exports.create = async (req, res) => {
  res.json(await TipoUsuario.create(req.body));
};

exports.update = async (req, res) => {
  const dado = await TipoUsuario.findByPk(req.params.id);
  if (!dado) return res.status(404).json({ msg: "Tipo de usuário não encontrado" });
  await dado.update(req.body);
  res.json(dado);
};

exports.remove = async (req, res) => {
  const dado = await TipoUsuario.findByPk(req.params.id);
  if (!dado) return res.status(404).json({ msg: "Tipo de usuário não encontrado" });
  await dado.destroy();
  res.json({ msg: "Removido" });
};

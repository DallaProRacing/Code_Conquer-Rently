const { DataTypes } = require("sequelize");
const sequelize = require("../config/db");

const TipoUsuario = sequelize.define("tiposUsuario", {
  idTipoUsuario: { type: DataTypes.INTEGER, primaryKey: true, autoIncrement: true },
  nomeTipoUsuario: DataTypes.STRING
}, { timestamps: false });

module.exports = TipoUsuario;

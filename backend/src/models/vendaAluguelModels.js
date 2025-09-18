const { DataTypes } = require("sequelize");
const sequelize = require("../config/db");

const VendaAluguel = sequelize.define("vendaAluguel", {
  idVendaAluguel: { type: DataTypes.INTEGER, primaryKey: true, autoIncrement: true },
  nomeVendaAluguel: DataTypes.STRING
}, { timestamps: false });

module.exports = VendaAluguel;

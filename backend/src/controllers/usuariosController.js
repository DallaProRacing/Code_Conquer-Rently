const usuariosModels = require("../models/usuariosModels");
const bcrypt = require("bcrypt");
const jwt = require("jsonwebtoken");
const cloudinary = require("../config/cloudinaryConfig");
const SECRET_KEY = process.env.JWT_SECRET;
const EXPIRESIN = process.env.EXPIRESIN;
const streamifier = require("streamifier");

exports.buscarUsuarios = async (req, res) => {
  try {
    const result = await usuariosModels.buscarUsuarios();
    res.status(200).json(result);
  } catch (error) {
    console.error(error);
    res.status(500).json({ error: "Erro ao buscar usuários" });
  }
};

exports.efetuarLogin = async (req, res) => {
  const { email, senha } = req.body;
  if (!email || !senha) {
    return res.status(400).json({ error: "Email e senha são obrigatórios" });
  }
  try {
    const usuarios = await usuariosModels.buscarUsuarios();
    const usuario = usuarios.find((u) => u.emailUsuario === email);
    if (!usuario) {
      return res.status(401).json({ error: "Credenciais inválidas" });
    }
    const senhaValida = await bcrypt.compare(senha, usuario.senhaUsuario);
    if (!senhaValida) {
      return res.status(401).json({ error: "Credenciais inválidas" });
    }
    const token = jwt.sign(
      { id: usuario.idUsuario, email: usuario.emailUsuario },
      SECRET_KEY,
      { expiresIn: "1h" }
    );
    res.status(200).json({ token });
  } catch (error) {
    console.error(error);
    res.status(500).json({ error: "Erro ao efetuar login" });
  }
};

exports.adicionarUsuario = async (req, res) => {
  const {
    email,
    senha,
    telefone,
    cpfCNPJ,
    fotoPerfil,
    idTipoUsuario,
    idEndereco,
  } = req.body;
  if (
    !email ||
    !senha ||
    !telefone ||
    !cpfCNPJ ||
    !idTipoUsuario ||
    !idEndereco
  ) {
    return res
      .status(400)
      .json({ error: "Todos os campos obrigatórios devem ser preenchidos" });
  }
  try {
    const usuarios = await usuariosModels.buscarUsuarios();
    const usuarioExistente = usuarios.find(u => u.emailUsuario === email);
    if (usuarioExistente) {
      return res
        .status(409)
        .json({ error: "Email já cadastrado" });
    }
    const senhaHash = await bcrypt.hash(senha, 10);
    const novoUsuario = await usuariosModels.adicionarUsuario(
      email,
      senhaHash,
      telefone,
      cpfCNPJ,
      fotoPerfil,
      idTipoUsuario,
      idEndereco
    );
    res.status(201).json(novoUsuario);
  } catch (error) {
    console.error(error);
    res.status(500).json({ error: "Erro ao adicionar usuário" });
  }
};

exports.atualizarFotoPerfil = async (req, res) => {
  const { id } = req.params;

  try {
    console.log("Arquivo recebido:", req.file);

    if (!req.file) {
      return res.status(400).json({ error: "Nenhuma imagem enviada" });
    }

    const streamUpload = (fileBuffer) => {
      return new Promise((resolve, reject) => {
        const stream = cloudinary.uploader.upload_stream(
          {
            folder: "usuarios/perfil",
            public_id: `usuario_${id}`,
            overwrite: true,
          },
          (error, result) => {
            if (result) resolve(result);
            else reject(error);
          }
        );
        streamifier.createReadStream(fileBuffer).pipe(stream);
      });
    };

    const result = await streamUpload(req.file.buffer);

    const usuarioAtualizado = await usuariosModels.alterarFotoPerfil(
      id,
      result.secure_url
    );

    if (!usuarioAtualizado) {
      return res.status(404).json({ error: "Usuário não encontrado" });
    }

    res.status(200).json({
      message: "Foto de perfil atualizada com sucesso",
      fotoPerfil: result.secure_url,
      usuario: usuarioAtualizado,
    });
  } catch (error) {
    console.error("Erro ao atualizar foto de perfil:", error);
    res.status(500).json({ error: "Erro interno ao atualizar foto de perfil" });
  }
};

exports.verificarSeEmailCadastrado = async (req, res) => {
    const { email } = req.params;
    try {
        const usuario = await usuariosModels.buscarUsuarioPorEmail(email);
        if (usuario) {
            return res.status(200).json({ cadastrado: true });
        }
        res.status(200).json({ cadastrado: false });
    } catch (error) {
        console.error(error);
        res.status(500).json({ error: 'Erro ao verificar email' });
    }   
};
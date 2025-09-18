const jwt = require('jsonwebtoken');
const { secret } = require('../config/jwt');

const verificarToken = function(req, res, next) {
    const authHeader = req.headers['authorization'];
    const token = authHeader && authHeader.split(' ')[1]; // pega só o token após "Bearer"

    if (!token) return res.status(401).json({ message: 'Token não fornecido' });

    jwt.verify(token, secret, (err, user) => {
        if (err) return res.status(401).json({ message: 'Token inválido' });

        req.user = user; // salva os dados do token no req
        next();
    });
};

module.exports = { verificarToken };
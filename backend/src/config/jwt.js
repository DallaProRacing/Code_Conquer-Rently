module.exports = {
    secret: process.env.JWT_SECRET || 'secreta',
    expiresIn: '365d' // 12 horas
};
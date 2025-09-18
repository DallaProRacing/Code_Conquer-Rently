require('dotenv').config();
const app = require('./src/app');
const PORT = process.env.PORT || 4000;

// Inicializa o servidor
app.listen(PORT, '0.0.0.0', () => {
    console.log(`🚀 Servidor rodando na porta http://localhost:${PORT}`);
    console.log(`❤️  Health check em: http://localhost:${PORT}/health`);
});

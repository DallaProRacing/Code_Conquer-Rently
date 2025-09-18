# Rently

  O **Rently** é um sistema de gerenciamento de locações. O objetivo do projeto é oferecer uma plataforma que facilite o processo de aluguel de imóveis, conectando proprietários e locatários de forma simples e eficiente.  
 
  O sistema foi projetado com uma arquitetura **frontend + backend** integrada, utilizando um banco de dados **PostgreSQL em nuvem (Neon)** para garantir **confiabilidade, desempenho e escalabilidade**.  
  
  Com isso, o Rently permite gerenciar categorias de imóveis e endereços, proporcionando uma experiência moderna e prática para os usuários.  


## 📂 Estrutura do Projeto

- **Frontend (Code_Conquer-Rently-main/)**  
  Desenvolvido em **.NET**, o frontend é responsável pela interface visual e interação com o usuário.  
  Ele consome os serviços da API e exibe as informações de forma intuitiva.

- **Backend (Code_Conquer_Rentrly_API-main/)**  
  Construído em **Node.js** com **Express**, o backend fornece uma API RESTful que centraliza as regras de negócio.  
  Ele integra-se com o **PostgreSQL (Neon Cloud)** para persistência dos dados.  

## ⚙️ Tecnologias Utilizadas

### Frontend
- .NET MAUI
- C# 

### Backend
- Node.js  
- Express  
- PostgreSQL (Neon Cloud)  

---


## 🚀 Como Executar o Projeto

### 1. Clonar este repositório
```bash
git clone <https://https.git.uricer.edu.br/CodeAndConquer/Rently.git>
cd <pasta-do-projeto>
```


### 2. Configurar o Backend
```bash
cd Code_Conquer_Rentrly_API-main
npm install
```

Criar um arquivo `.env` na raiz com as seguintes variáveis (exemplo):
```env
PORT=5000
DB_URI=<sua-string-de-conexao>
JWT_SECRET=<sua-chave-jwt>
CLOUDINARY_CLOUD_NAME=<nome>
CLOUDINARY_API_KEY=<chave>
CLOUDINARY_API_SECRET=<segredo>
```

Rodar em ambiente de desenvolvimento:
```bash
npm run dev
```

Rodar testes:
```bash
npm test
```

---

### 3. Configurar o Frontend (.NET)
```bash
cd Code_Conquer-Rently-main/Rently
dotnet restore
dotnet build
dotnet run
```

A aplicação será iniciada em:
```
http://localhost:5000 (backend)
http://localhost:port (frontend)
```

---

## 📌 Funcionalidades Principais

  
- Cadastro de usuários e itens, com possibilidade de anexar imagens.
- Sistema de busca e filtros (por nome, categoria, preço e localização).
- Reservas e solicitações de aluguel de forma integrada.
- Comunicação entre locador e locatário, garantindo clareza nas negociações.

---
## 📖 Manual do Usuário

Para entender melhor como utilizar todas as funcionalidades do Rently, acesse o manual do usuário neste link:  
[Manual do Usuário Rently](https://docs.google.com/document/d/1y7AjuNsC2uayCWBY2y5EE0HLAUV-HdzdDqRZxIDQvHA/edit?usp=sharing)

## 🤝 Contribuição

1. Faça um fork do projeto  
2. Crie uma branch (`git checkout -b feature/nova-funcionalidade`)  
3. Commit suas alterações (`git commit -m 'Adiciona nova funcionalidade'`)  
4. Envie para o repositório remoto (`git push origin feature/nova-funcionalidade`)  
5. Abra um Pull Request  

---

## 📄 Licença

Este projeto está sob a licença MIT.  

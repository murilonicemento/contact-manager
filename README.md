# Contact Manager

**Contact Manager**, é um aplicativo web desenvolvido com **ASP.NET Core MVC** para gerenciar contatos. Este projeto foi criado para praticar os conceitos aprendidos com **ASP.NET Core MVC**.


## 🛠️ Funcionalidades

- **Cadastro de Contatos**: Adicione novos contatos com informações como nome, telefone, e-mail e endereço.
- **Listagem de Contatos**: Veja todos os seus contatos cadastrados em uma tabela organizada.
- **Edição de Contatos**: Atualize as informações de um contato existente.
- **Remoção de Contatos**: Exclua contatos que não são mais necessários.
- **Busca por Contatos**: Pesquise contatos rapidamente pelo nome ou outros critérios.
- **Validação de Dados**: Garantia de inserção de dados consistentes.


## 🚀 Tecnologias Utilizadas

- **ASP.NET Core MVC**: Framework principal para o desenvolvimento da aplicação.
- **Entity Framework Core**: Para gerenciamento de banco de dados e mapeamento objeto-relacional (ORM).
- **SQL Server**: Banco de dados leve para armazenamento local.
- **Razor Pages**: Para renderização de páginas dinâmicas.


## 🔧 Configuração do Ambiente

Siga os passos abaixo para executar o projeto localmente:

### Pré-requisitos
- **.NET 8 SDK**
- **Banco de dados SQL Server**
- **IDE**: [Rider](https://www.jetbrains.com/rider/) ou [Visual Studio Code](https://code.visualstudio.com/).

### Passos
1. Clone este repositório:
   ```bash
   git clone https://github.com/murilonicemento/contact-manager
   ```

2. Configure a string de conexão no arquivo `appsettings.json`:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Data Source=contactmanager.db"
     }
   }
   ```

3. Aplique as migrações para configurar o banco de dados:
   ```bash
   dotnet ef database update
   ```

4. Execute o projeto:
   ```bash
   dotnet run
   ```

5. Acesse no navegador: `http://localhost:5265`


## 🤝 Contribuição

Contribuições são bem-vindas! Sinta-se à vontade para abrir issues ou enviar pull requests.

1. Fork o projeto
2. Crie uma branch com sua feature: `git checkout -b minha-feature`
3. Faça commit das alterações: `git commit -m 'Adicionei uma nova feature'`
4. Envie a branch: `git push origin minha-feature`
5. Abra um Pull Request


## 📝 Licença

Este projeto está licenciado sob a [MIT License](LICENSE).



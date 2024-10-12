# Feedback do Instrutor

#### 11/10/24 - Revisão Inicial - Eduardo Pires

## Pontos Positivos:

- Separação de responsabilidades.
- Controle eficiente de usuários com autorização e roles.
- Demonstrou sólido conhecimento em Identity e JWT.
- Bom controle de validação de permissão de edição de posts e validação de user Admin ou proprietário do registro.
- Bom uso de extensão do comportamento do Identity (ClaimsPrincipalExtensions) e customização da Razor Page.
- Mostrou entendimento do ecossistema de desenvolvimento em .NET
- Bom uso de extension methods para configurações.
- Uso interessante do "CalcularTempoLeituraEmMinutos"
- Bom uso do "ExceptionMiddlewareExtension"
- Bom uso de Cache, ponto importante!
- Documentou bem o repositório

## Pontos Negativos:

- Uso desnecessário de algumas camadas (Business, Identity). Poderia haver uma unica camada "Application" abraçando as responsabilidades das demais camadas Web, pois o projeto não requer tanta complexidade.
- Apesar de existir uma camada de negócios as entidades foram implementadas diretamente na camada de Dados o que não fez sentido. Isso torna a camada business desnecessária.
- Mapeou o banco utilizando dataannotations ao invés de FluentAPI.
- Duplicação da responsabilidade de ViewModels (business, API)

## Sugestões:

- Uma arquitetura mais coesa e simplificada faria mais sentido.
- Não misturar responsabilidades de domain com infra e dados.

## Problemas:

- Não consegui executar a aplicação de imediato na máquina. É necessário que o Seed esteja configurado corretamente, com uma connection string apontando para o SQLite.

  **P.S.** As migrations precisam ser geradas com uma conexão apontando para o SQLite; caso contrário, a aplicação não roda.

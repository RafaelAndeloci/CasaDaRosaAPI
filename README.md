# CasaDaRosa

## Deploy no Render com Docker

O repositório está preparado para deploy com `Dockerfile` na raiz e `render.yaml` para criação do serviço web no Render.

### Variáveis de ambiente necessárias

- `ConnectionStrings__DefaultConnection`
- `Jwt__SecretKey`
- `Jwt__Issuer`
- `Jwt__Audience`
- `Jwt__ExpirationInMinutes`

### Observações

- A aplicação lê a porta pelo `PORT`, conforme exigido pelo Render.
- O endpoint de health check está disponível em `/health`.
- As migrations não são executadas automaticamente no deploy. Rode manualmente antes de apontar a aplicação para o banco em produção.

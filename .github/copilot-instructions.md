# Copilot Instructions

## Diretrizes de projeto
- Neste projeto, utilizar FluentValidation para validações na aplicação.
- Não criar projetos automaticamente; o usuário criará os projetos manualmente.
- Não executar comandos de migrations; o usuário rodará manualmente no terminal.
- Não instalar pacotes NuGet automaticamente; apenas informar quais pacotes devem ser baixados manualmente.
- Neste repositório, evitar ao máximo Primitive Obsession. Preferir Value Objects com validações internas para evitar Primitive Obsession, deixando as entidades focadas em comportamento e regras do agregado. Priorizar Single Responsibility ao decidir onde mapear dados e comportamentos, centralizando responsabilidades no value object mais adequado. Preferir utilizar records ao invés de classes para Value Objects quando fizer sentido neste projeto. O domínio deve ser rico, com entidades, comportamentos, validações, erros e exceções bem definidos. Padronizar requests/responses da API e a tratativa de erros para o frontend.
- Considerar eventos de domínio e eventos de aplicação no projeto, com publicação e inscrição de eventos para cenários como envio de e-mail após realização de compra.
# Copilot Instructions

## Diretrizes de projeto
- Neste projeto, utilizar FluentValidation para validações na aplicação.
- Não criar projetos automaticamente; o usuário criará os projetos manualmente.
- Não executar comandos de migrations; o usuário rodará manualmente no terminal.
- Não instalar pacotes NuGet automaticamente; apenas informar quais pacotes devem ser baixados manualmente.
- Neste repositório, evitar ao máximo Primitive Obsession. Preferir Value Objects com validações internas para evitar Primitive Obsession, deixando as entidades focadas em comportamento e regras do agregado. Priorizar Single Responsibility ao decidir onde mapear dados e comportamentos, centralizando responsabilidades no value object mais adequado. Preferir utilizar records ao invés de classes para Value Objects quando fizer sentido neste projeto. O domínio deve ser rico, com entidades, comportamentos, validações, erros e exceções bem definidos. Padronizar requests/responses da API e a tratativa de erros para o frontend.
- Padronizar retorno de todos os endpoints e prever queries paginadas com possibilidade de filtros nos endpoints de listagem ('get-all').
- Considerar eventos de domínio e eventos de aplicação no projeto, com publicação e inscrição de eventos para cenários como envio de e-mail após realização de compra.
- Padrão de domínio do repositório: entidades e agregados devem expor factories estáticas Create com construtores não públicos, concentrar mudanças de estado em métodos de comportamento, usar Value Objects preferencialmente como records selados com validação interna e sem setters públicos, e manter regras/invariantes explícitas com Result para falhas de negócio e DomainValidationException para estados inválidos.
- No domínio, preferir exceções de domínio específicas para cada tipo de erro em vez de lançar diretamente DomainValidationException, ArgumentException ou similares.
- Na API, retornar enums com Id e descrição para facilitar o mapeamento no frontend.
- Garantir que endpoints e tratativas de erro sigam padrão REST, usando métodos HTTP corretos, status codes corretos e documentação Swagger alinhada ao que a API espera e retorna.

## Interação com o Copilot
- Quando o usuário disser apenas para prosseguir, continue com a implementação sem anunciar o que vai fazer.
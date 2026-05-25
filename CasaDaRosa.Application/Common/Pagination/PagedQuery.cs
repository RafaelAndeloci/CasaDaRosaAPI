namespace CasaDaRosa.Application.Common.Pagination;

public abstract record PagedQuery(int PageNumber = 1, int PageSize = 10);

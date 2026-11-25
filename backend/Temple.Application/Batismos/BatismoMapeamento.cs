using Temple.Domain.Entities;

namespace Temple.Application.Batismos;

public static class BatismoMapeamento
{
    public static BatismoResponse ParaResponse(this Batismo batismo)
    {
        return new BatismoResponse(
            batismo.Id,
            batismo.MembroId,
            batismo.DataBatismo,
            batismo.Oficiante,
            batismo.Local);
    }
}
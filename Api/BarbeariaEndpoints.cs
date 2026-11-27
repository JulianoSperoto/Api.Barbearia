using BarbeariaApi.Aplicacao;
using BarbeariaApi.Aplicacao.Portas;
using BarbeariaApi.Dominio;

namespace BarbeariaApi.Api;

public sealed class CriarAgendamentoRequest
{
    public int BarbeariaId { get; set; }
    public int ClienteId { get; set; }
    public int BarbeiroId { get; set; }
    public DateTimeOffset DataHora { get; set; }
    public int[]? ServicoIds { get; set; }
}

public sealed class AvaliacaoRequest
{
    public int Nota { get; set; }
    public string? Comentario { get; set; }
}

public static class BarbeariaEndpoints
{
    public static void MapBarbeariaEndpoints(this WebApplication app)
    {
        app.MapGet("/barbearias", (ICatalogoRepositorio catalogo) =>
            Results.Ok(catalogo.ListarBarbearias()));

        app.MapGet("/barbearias/{id:int}/barbeiros", (int id, ICatalogoRepositorio catalogo) =>
            catalogo.ListarBarbearias().Any(barbearia => barbearia.Id == id)
                ? Results.Ok(catalogo.ListarBarbeiros(id))
                : Results.NotFound(new { mensagem = "Barbearia não encontrada." }));

        app.MapGet("/barbearias/{id:int}/servicos", (int id, ICatalogoRepositorio catalogo) =>
            catalogo.ListarBarbearias().Any(barbearia => barbearia.Id == id)
                ? Results.Ok(catalogo.ListarServicos(id))
                : Results.NotFound(new { mensagem = "Barbearia não encontrada." }));

        app.MapGet("/clientes", (ICatalogoRepositorio catalogo) => Results.Ok(catalogo.ListarClientes()));

        var agenda = app.MapGroup("/agendamentos");
        agenda.MapGet("", (AgendamentoServico servico) => Results.Ok(servico.Listar()));
        agenda.MapGet("/{id:int}", (int id, AgendamentoServico servico) => Executar(() => Results.Ok(servico.Buscar(id))));

        agenda.MapPost("", (CriarAgendamentoRequest dados, AgendamentoServico servico) => Executar(() =>
        {
            var agendamento = servico.Criar(dados.BarbeariaId, dados.ClienteId, dados.BarbeiroId,
                dados.DataHora, dados.ServicoIds);
            return Results.Created($"/agendamentos/{agendamento.Id}", agendamento);
        }));

        agenda.MapPatch("/{id:int}/confirmar", (int id, AgendamentoServico servico) =>
            Executar(() => Results.Ok(servico.Confirmar(id))));
        agenda.MapPatch("/{id:int}/cancelar", (int id, AgendamentoServico servico) =>
            Executar(() => Results.Ok(servico.Cancelar(id))));
        agenda.MapPatch("/{id:int}/concluir", (int id, AgendamentoServico servico) =>
            Executar(() => Results.Ok(servico.Concluir(id))));

        agenda.MapPost("/{id:int}/avaliacao", (int id, AvaliacaoRequest dados, AgendamentoServico servico) => Executar(() =>
        {
            var agendamento = servico.Avaliar(id, dados.Nota, dados.Comentario);
            return Results.Created($"/agendamentos/{id}/avaliacao", agendamento.Avaliacao);
        }));

        agenda.MapGet("/{id:int}/avaliacao", (int id, AgendamentoServico servico) => Executar(() =>
        {
            var avaliacao = servico.Buscar(id).Avaliacao;
            return avaliacao == null
                ? Results.NotFound(new { mensagem = "Este agendamento ainda não possui avaliação." })
                : Results.Ok(avaliacao);
        }));
    }

    private static IResult Executar(Func<IResult> acao)
    {
        try
        {
            return acao();
        }
        catch (NaoEncontradoException ex)
        {
            return Results.NotFound(new { mensagem = ex.Message });
        }
        catch (ConflitoAgendamentoException ex)
        {
            return Results.Conflict(new { mensagem = ex.Message });
        }
        catch (ArgumentException ex)
        {
            return Results.BadRequest(new { mensagem = ex.Message });
        }
    }
}

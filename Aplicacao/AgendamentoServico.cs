using BarbeariaApi.Aplicacao.Portas;
using BarbeariaApi.Dominio;

namespace BarbeariaApi.Aplicacao;

public sealed class NaoEncontradoException : Exception
{
    public NaoEncontradoException(string mensagem) : base(mensagem)
    {
    }
}

public sealed class AgendamentoServico
{
    private readonly ICatalogoRepositorio catalogo;
    private readonly IAgendamentoRepositorio repositorio;

    public AgendamentoServico(ICatalogoRepositorio catalogo, IAgendamentoRepositorio repositorio)
    {
        this.catalogo = catalogo;
        this.repositorio = repositorio;
    }

    public IReadOnlyList<Agendamento> Listar() => repositorio.Listar();

    public Agendamento Buscar(int id) =>
        repositorio.Buscar(id) ?? throw new NaoEncontradoException("Agendamento não encontrado.");

    public Agendamento Criar(int barbeariaId, int clienteId, int barbeiroId,
        DateTimeOffset dataHora, IReadOnlyList<int>? servicoIds)
    {
        if (dataHora <= DateTimeOffset.UtcNow)
            throw new ArgumentException("Informe um horário futuro.");

        if (servicoIds == null || servicoIds.Count == 0)
            throw new ArgumentException("Selecione pelo menos um serviço.");

        if (servicoIds.Distinct().Count() != servicoIds.Count)
            throw new ArgumentException("Selecione cada serviço uma única vez.");

        if (!catalogo.ListarBarbearias().Any(barbearia => barbearia.Id == barbeariaId))
            throw new NaoEncontradoException("Barbearia não encontrada.");

        if (!catalogo.ListarClientes().Any(cliente => cliente.Id == clienteId))
            throw new NaoEncontradoException("Cliente não encontrado.");

        if (!catalogo.ListarBarbeiros(barbeariaId).Any(barbeiro => barbeiro.Id == barbeiroId))
            throw new NaoEncontradoException("O barbeiro não atende nesta barbearia.");

        var servicos = catalogo.ListarServicos(barbeariaId).ToDictionary(servico => servico.Id);
        var itens = new List<ItemAgendamento>();
        foreach (var servicoId in servicoIds)
        {
            if (!servicos.TryGetValue(servicoId, out var servico))
                throw new NaoEncontradoException($"Serviço {servicoId} não encontrado nesta barbearia.");

            itens.Add(new ItemAgendamento(servico.Id, servico.Nome, servico.DuracaoMinutos, servico.PrecoTabela));
        }

        if (DateTimeOffset.MaxValue - dataHora < TimeSpan.FromMinutes(itens.Sum(item => item.DuracaoMinutos)))
            throw new ArgumentException("O horário de término ultrapassa o intervalo permitido.");

        return repositorio.Reservar(new Agendamento(barbeariaId, clienteId, barbeiroId, dataHora.ToUniversalTime(), itens));
    }

    public Agendamento Confirmar(int id) => Atualizar(id, agendamento => agendamento.Confirmar());
    public Agendamento Cancelar(int id) => Atualizar(id, agendamento => agendamento.Cancelar());
    public Agendamento Concluir(int id) => Atualizar(id, agendamento => agendamento.Concluir());
    public Agendamento Avaliar(int id, int nota, string? comentario) =>
        Atualizar(id, agendamento => agendamento.Avaliar(new Avaliacao(nota, comentario)));

    private Agendamento Atualizar(int id, Func<Agendamento, Agendamento> alterar) =>
        repositorio.Atualizar(id, alterar) ?? throw new NaoEncontradoException("Agendamento não encontrado.");
}

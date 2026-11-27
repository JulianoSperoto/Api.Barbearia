namespace BarbeariaApi.Dominio;

public enum StatusAgendamento
{
    Pendente,
    Confirmado,
    Cancelado,
    Concluido
}

public sealed class ItemAgendamento
{
    public int ServicoId { get; }
    public string Nome { get; }
    public int DuracaoMinutos { get; }
    public decimal PrecoCobrado { get; }

    public ItemAgendamento(int servicoId, string nome, int duracaoMinutos, decimal precoCobrado)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);
        if (duracaoMinutos <= 0)
            throw new ArgumentOutOfRangeException(nameof(duracaoMinutos), "A duração deve ser maior que zero.");
        if (precoCobrado < 0)
            throw new ArgumentOutOfRangeException(nameof(precoCobrado), "O preço cobrado não pode ser negativo.");

        ServicoId = servicoId;
        Nome = nome.Trim();
        DuracaoMinutos = duracaoMinutos;
        PrecoCobrado = precoCobrado;
    }
}

public sealed class Avaliacao
{
    public int Nota { get; }
    public string? Comentario { get; }

    public Avaliacao(int nota, string? comentario)
    {
        if (nota is < 1 or > 5)
            throw new ArgumentOutOfRangeException(nameof(nota), "A nota deve estar entre 1 e 5.");

        Nota = nota;
        Comentario = comentario?.Trim();
    }
}

public sealed class ConflitoAgendamentoException : Exception
{
    public ConflitoAgendamentoException(string mensagem) : base(mensagem)
    {
    }
}

public sealed class Agendamento
{
    public int Id { get; private set; }
    public int BarbeariaId { get; }
    public int ClienteId { get; }
    public int BarbeiroId { get; }
    public DateTimeOffset DataHora { get; }
    public DateTimeOffset Fim { get; }
    public decimal Total { get; }
    public IReadOnlyList<ItemAgendamento> Itens { get; }
    public StatusAgendamento Status { get; private set; } = StatusAgendamento.Pendente;
    public Avaliacao? Avaliacao { get; private set; }

    public Agendamento(int barbeariaId, int clienteId, int barbeiroId,
        DateTimeOffset dataHora, IEnumerable<ItemAgendamento> itens)
    {
        ArgumentNullException.ThrowIfNull(itens);
        var listaItens = itens.ToArray();

        if (listaItens.Length == 0)
            throw new ArgumentException("O agendamento deve conter pelo menos um serviço.", nameof(itens));

        BarbeariaId = barbeariaId;
        ClienteId = clienteId;
        BarbeiroId = barbeiroId;
        DataHora = dataHora;
        Itens = Array.AsReadOnly(listaItens);
        Fim = DataHora.AddMinutes(listaItens.Sum(item => (long)item.DuracaoMinutos));
        Total = listaItens.Sum(item => item.PrecoCobrado);
    }

    private Agendamento Copiar()
    {
        return new Agendamento(BarbeariaId, ClienteId, BarbeiroId, DataHora, Itens)
        {
            Id = Id,
            Status = Status,
            Avaliacao = Avaliacao
        };
    }

    public Agendamento Identificar(int id)
    {
        var copia = Copiar();
        copia.Id = id;
        return copia;
    }

    public bool ConflitaCom(Agendamento outro)
    {
        return BarbeiroId == outro.BarbeiroId &&
               Status != StatusAgendamento.Cancelado &&
               outro.Status != StatusAgendamento.Cancelado &&
               DataHora < outro.Fim && outro.DataHora < Fim;
    }

    public Agendamento Confirmar()
    {
        if (Status != StatusAgendamento.Pendente)
            throw new ConflitoAgendamentoException("Somente reservas pendentes podem ser confirmadas.");

        var copia = Copiar();
        copia.Status = StatusAgendamento.Confirmado;
        return copia;
    }

    public Agendamento Cancelar()
    {
        if (Status != StatusAgendamento.Pendente && Status != StatusAgendamento.Confirmado)
            throw new ConflitoAgendamentoException("Somente reservas pendentes ou confirmadas podem ser canceladas.");

        var copia = Copiar();
        copia.Status = StatusAgendamento.Cancelado;
        return copia;
    }

    public Agendamento Concluir()
    {
        if (Status != StatusAgendamento.Confirmado)
            throw new ConflitoAgendamentoException("Somente reservas confirmadas podem ser concluídas.");

        var copia = Copiar();
        copia.Status = StatusAgendamento.Concluido;
        return copia;
    }

    public Agendamento Avaliar(Avaliacao avaliacao)
    {
        ArgumentNullException.ThrowIfNull(avaliacao);

        if (Status != StatusAgendamento.Concluido || Avaliacao != null)
            throw new ConflitoAgendamentoException("A avaliação só pode ser registrada uma vez, após a conclusão.");

        var copia = Copiar();
        copia.Avaliacao = avaliacao;
        return copia;
    }
}

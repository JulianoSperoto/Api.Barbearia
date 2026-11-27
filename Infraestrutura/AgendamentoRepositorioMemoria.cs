using BarbeariaApi.Aplicacao.Portas;
using BarbeariaApi.Dominio;

namespace BarbeariaApi.Infraestrutura;

public sealed class AgendamentoRepositorioMemoria : IAgendamentoRepositorio
{
    private readonly object acesso = new();
    private readonly Dictionary<int, Agendamento> agendamentos = new();
    private int proximoId = 1;

    public IReadOnlyList<Agendamento> Listar()
    {
        lock (acesso)
            return agendamentos.Values.OrderBy(agendamento => agendamento.DataHora).ToArray();
    }

    public Agendamento? Buscar(int id)
    {
        lock (acesso)
            return agendamentos.GetValueOrDefault(id);
    }

    public Agendamento Reservar(Agendamento agendamento)
    {
        lock (acesso)
        {
            if (agendamentos.Values.Any(agendamento.ConflitaCom))
                throw new ConflitoAgendamentoException("O barbeiro já tem uma reserva nesse intervalo.");

            var reserva = agendamento.Identificar(proximoId++);
            agendamentos.Add(reserva.Id, reserva);
            return reserva;
        }
    }

    public Agendamento? Atualizar(int id, Func<Agendamento, Agendamento> alterar)
    {
        lock (acesso)
        {
            if (!agendamentos.TryGetValue(id, out var atual))
                return null;

            var atualizado = alterar(atual);
            agendamentos[id] = atualizado;
            return atualizado;
        }
    }
}

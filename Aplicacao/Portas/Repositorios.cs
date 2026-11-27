using BarbeariaApi.Dominio;

namespace BarbeariaApi.Aplicacao.Portas;

public interface ICatalogoRepositorio
{
    IReadOnlyList<Barbearia> ListarBarbearias();
    IReadOnlyList<Barbeiro> ListarBarbeiros(int barbeariaId);
    IReadOnlyList<Servico> ListarServicos(int barbeariaId);
    IReadOnlyList<Cliente> ListarClientes();
}

public interface IAgendamentoRepositorio
{
    IReadOnlyList<Agendamento> Listar();
    Agendamento? Buscar(int id);
    Agendamento Reservar(Agendamento agendamento);
    Agendamento? Atualizar(int id, Func<Agendamento, Agendamento> alterar);
}

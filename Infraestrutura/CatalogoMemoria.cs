using BarbeariaApi.Aplicacao.Portas;
using BarbeariaApi.Dominio;

namespace BarbeariaApi.Infraestrutura;

public sealed class CatalogoMemoria : ICatalogoRepositorio
{
    private readonly IReadOnlyList<Barbearia> barbearias = Array.AsReadOnly(new[]
    {
        new Barbearia(1, "Barbearia Central", "(00) 0000-0000", new Endereco("Rua Central", "100", "Cidade Exemplo"))
    });

    private readonly IReadOnlyList<Barbeiro> barbeiros = Array.AsReadOnly(new[]
    {
        new Barbeiro(1, "Rafael", "(00) 0000-0001", "Corte e barba"),
        new Barbeiro(2, "Bruno", "(00) 0000-0002", "Corte e acabamento")
    });

    private readonly IReadOnlyList<Cliente> clientes = Array.AsReadOnly(new[]
    {
        new Cliente(1, "Marcos", "(00) 0000-0003"),
        new Cliente(2, "João", "(00) 0000-0004")
    });

    private readonly IReadOnlyList<Servico> servicos = Array.AsReadOnly(new[]
    {
        new Servico(1, 1, "Corte", 30, 45m),
        new Servico(2, 1, "Barba", 20, 30m),
        new Servico(3, 1, "Acabamento", 15, 20m)
    });

    public IReadOnlyList<Barbearia> ListarBarbearias() => barbearias;
    public IReadOnlyList<Cliente> ListarClientes() => clientes;
    public IReadOnlyList<Barbeiro> ListarBarbeiros(int barbeariaId) => barbeariaId == 1 ? barbeiros : Array.Empty<Barbeiro>();
    public IReadOnlyList<Servico> ListarServicos(int barbeariaId) =>
        servicos.Where(servico => servico.BarbeariaId == barbeariaId).ToArray();
}

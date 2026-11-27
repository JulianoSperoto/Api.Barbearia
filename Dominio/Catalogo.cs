namespace BarbeariaApi.Dominio;

public sealed class Endereco
{
    public string Rua { get; }
    public string Numero { get; }
    public string Cidade { get; }

    public Endereco(string rua, string numero, string cidade)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(rua);
        ArgumentException.ThrowIfNullOrWhiteSpace(numero);
        ArgumentException.ThrowIfNullOrWhiteSpace(cidade);

        Rua = rua.Trim();
        Numero = numero.Trim();
        Cidade = cidade.Trim();
    }
}

public sealed class Barbearia
{
    public int Id { get; }
    public string Nome { get; }
    public string Telefone { get; }
    public Endereco Endereco { get; }

    public Barbearia(int id, string nome, string telefone, Endereco endereco)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);
        ArgumentException.ThrowIfNullOrWhiteSpace(telefone);
        ArgumentNullException.ThrowIfNull(endereco);

        Id = id;
        Nome = nome.Trim();
        Telefone = telefone.Trim();
        Endereco = endereco;
    }
}

public sealed class Barbeiro
{
    public int Id { get; }
    public string Nome { get; }
    public string Telefone { get; }
    public string Especialidades { get; }

    public Barbeiro(int id, string nome, string telefone, string especialidades)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);
        ArgumentException.ThrowIfNullOrWhiteSpace(telefone);

        Id = id;
        Nome = nome.Trim();
        Telefone = telefone.Trim();
        Especialidades = especialidades?.Trim() ?? string.Empty;
    }
}

public sealed class Cliente
{
    public int Id { get; }
    public string Nome { get; }
    public string Telefone { get; }

    public Cliente(int id, string nome, string telefone)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);
        ArgumentException.ThrowIfNullOrWhiteSpace(telefone);

        Id = id;
        Nome = nome.Trim();
        Telefone = telefone.Trim();
    }
}

public sealed class Servico
{
    public int Id { get; }
    public int BarbeariaId { get; }
    public string Nome { get; }
    public int DuracaoMinutos { get; }
    public decimal PrecoTabela { get; }

    public Servico(int id, int barbeariaId, string nome, int duracaoMinutos, decimal precoTabela)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(nome);
        if (duracaoMinutos <= 0)
            throw new ArgumentOutOfRangeException(nameof(duracaoMinutos), "A duração deve ser maior que zero.");
        if (precoTabela < 0)
            throw new ArgumentOutOfRangeException(nameof(precoTabela), "O preço não pode ser negativo.");

        Id = id;
        BarbeariaId = barbeariaId;
        Nome = nome.Trim();
        DuracaoMinutos = duracaoMinutos;
        PrecoTabela = precoTabela;
    }
}

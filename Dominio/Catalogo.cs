namespace BarbeariaApi.Dominio;

public sealed class Endereco
{
    public string Rua { get; }
    public string Numero { get; }
    public string Cidade { get; }

    public Endereco(string rua, string numero, string cidade)
    {
        Rua = rua;
        Numero = numero;
        Cidade = cidade;
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
        Id = id;
        Nome = nome;
        Telefone = telefone;
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
        Id = id;
        Nome = nome;
        Telefone = telefone;
        Especialidades = especialidades;
    }
}

public sealed class Cliente
{
    public int Id { get; }
    public string Nome { get; }
    public string Telefone { get; }

    public Cliente(int id, string nome, string telefone)
    {
        Id = id;
        Nome = nome;
        Telefone = telefone;
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
        Id = id;
        BarbeariaId = barbeariaId;
        Nome = nome;
        DuracaoMinutos = duracaoMinutos;
        PrecoTabela = precoTabela;
    }
}

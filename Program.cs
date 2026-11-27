using System.Text.Json.Serialization;
using BarbeariaApi.Api;
using BarbeariaApi.Aplicacao;
using BarbeariaApi.Aplicacao.Portas;
using BarbeariaApi.Infraestrutura;

var builder = WebApplication.CreateBuilder(args);
builder.Services.ConfigureHttpJsonOptions(options =>
    options.SerializerOptions.Converters.Add(new JsonStringEnumConverter()));
builder.Services.AddSingleton<ICatalogoRepositorio, CatalogoMemoria>();
builder.Services.AddSingleton<IAgendamentoRepositorio, AgendamentoRepositorioMemoria>();
builder.Services.AddScoped<AgendamentoServico>();

var app = builder.Build();
app.MapBarbeariaEndpoints();
app.Run();

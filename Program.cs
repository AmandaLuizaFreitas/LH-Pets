using System.Reflection;
using LHPets.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Controllers (padrão MVC para Web API)
builder.Services.AddControllers();

// Armazenamento em memória compartilhado entre os controllers (uma única instância)
builder.Services.AddSingleton<ArmazenamentoEmMemoria>();

// Swagger / OpenAPI
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(opcoes =>
{
    // Inclui os comentários /// dos controllers e models na documentação
    var arquivoXml = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
    opcoes.IncludeXmlComments(Path.Combine(AppContext.BaseDirectory, arquivoXml));
});

var app = builder.Build();

// Swagger disponível em /swagger
app.UseSwagger();
app.UseSwaggerUI();

// Ao abrir a raiz do site, redireciona para o Swagger
app.MapGet("/", () => Results.Redirect("/swagger")).ExcludeFromDescription();

app.MapControllers();

app.Run();

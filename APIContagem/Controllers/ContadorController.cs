using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using APIContagem.Models;

namespace APIContagem.Controllers;

[ApiController]
[Route("[controller]")]
public class ContadorController : ControllerBase
{
    private static readonly Contador _CONTADOR = new Contador();
    private readonly ILogger<ContadorController> _logger;
    private readonly IConfiguration _configuration;

    public ContadorController(ILogger<ContadorController> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _configuration = configuration;
    }

    [HttpGet("fixed")]
    [EnableRateLimiting("fixed")]
    public ResultadoContador GetFixedWindow()
    {
        return GetContador("Fixed Window");
    }

    [HttpGet("concurrency")]
    [EnableRateLimiting("concurrency")]
    public ResultadoContador GetConcurrency()
    {
        return GetContador("Concurrency");
    }

    [HttpGet("fixed-queuelimit")]
    [EnableRateLimiting("fixed-queuelimit")]
    public ResultadoContador GetFixedWindowQueueLimit()
    {
        return GetContador("Fixed Window Queue Limit");
    }

    private ResultadoContador GetContador(string rateLimitMode)
    {
        int valorAtualContador;

        lock (_CONTADOR)
        {
            _CONTADOR.Incrementar();
            valorAtualContador = _CONTADOR.ValorAtual;
        }

        _logger.LogInformation($"{rateLimitMode} - Contador - Valor atual: {valorAtualContador} | " +
            $"{DateTime.Now:HH:mm:ss}");

        return new()
        {
            ValorAtual = valorAtualContador,
            Producer = _CONTADOR.Local,
            Kernel = _CONTADOR.Kernel,
            Framework = _CONTADOR.Framework,
            Mensagem = _configuration["MensagemVariavel"]
        };
    }
}
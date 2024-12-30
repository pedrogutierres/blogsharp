using Blog.Application.Helpers;
using Blog.Application.Identity;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text.Json;

namespace Blog.Application.Services
{
    public sealed class OpenAIService
    {
        private readonly IUser _user;
        private readonly ILogger<OpenAIService> _logger;
        private readonly OpenAIOptions _options;

        public OpenAIService(IUser user, ILogger<OpenAIService> logger, IOptions<OpenAIOptions> options)
        {
            _user = user;
            _logger = logger;
            _options = options.Value;
        }

        public async Task<string> GerarImagem(string contexto)
        {
            if (string.IsNullOrEmpty(_options?.ApiKey))
                throw new ArgumentNullException("ApiKey", "Chave da API OpenAI não configurada.");

            try
            {
                using var client = new HttpClient();

                client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", _options.ApiKey);

                var prompt = "Gerar uma imagem que vai ser visualizada por usuários em um blog, considerando o título da postagem a seguir: " + contexto;

                var requestContent = new
                {
                    prompt = prompt.Length > 1000 ? prompt[..1000] : prompt,
                    n = 1,
                    size = "512x512"
                };

                _logger.LogInformation($"Comunicando com OpenAI para gerar sobre: {contexto}");

                var response = await client.PostAsJsonAsync("https://api.openai.com/v1/images/generations", requestContent);

                var responseBody = await response.Content.ReadAsStringAsync();
                var responseJson = JsonDocument.Parse(responseBody);

                if (!response.IsSuccessStatusCode)
                {
                    string erro = responseJson.RootElement.GetProperty("error").GetProperty("message").GetString();
                    if (!string.IsNullOrEmpty(erro))
                        _logger.LogError($"Não foi possível gerar a imagem via OpenAI. Retorno da geração da API: {erro}");
                    else
                        _logger.LogError($"Não foi possível gerar a imagem via OpenAI. Retorno da geração da API: {responseBody}");

                    return null;
                }

                var urlImagem = responseJson.RootElement.GetProperty("data")[0].GetProperty("url").GetString();

                if (!string.IsNullOrEmpty(urlImagem))
                {
                    _logger.LogInformation($"Imagem gerada com sucesso, url: {urlImagem}");

                    return urlImagem;
                }

                _logger.LogError($"Não foi possível gerar a imagem via OpenAI. Retorno da geração da API: {responseBody}");
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"Erro ao tentar gerar a imagem via OpenAI. Erro: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro inesperado ao tentar gerar a imagem via OpenAI. Erro: {ex.Message}");
            }
            return null;
        }

        public async Task<byte[]> BaixarImagem(string urlImagem)
        {
            try
            {
                using var client = new HttpClient();

                return await client.GetByteArrayAsync(urlImagem);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"Erro ao tentar baixar a imagem. Erro: {ex.Message}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Erro inesperado ao tentar baixar a imagem. Erro: {ex.Message}");
            }
            return null;
        }
    }
}

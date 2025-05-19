using Microsoft.EntityFrameworkCore;
using RestSharp;
using System;
using System.Text.Json.Nodes;
using System.Threading.Tasks;
using Web_Scraper.Models;

namespace Web_Scraper.Services
{
    public abstract class BaseScraperService
    {
        protected readonly SanayContext _context;
        protected readonly RestClient _client;

        protected BaseScraperService(SanayContext context)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            Console.WriteLine("BaseScraperService: Initializing RestClient...");
            var options = new RestClientOptions("https://fund.fipiran.ir")
            {
                MaxTimeout = -1,
                UserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/135.0.0.0 Safari/537.36",
                RemoteCertificateValidationCallback = (sender, certificate, chain, sslPolicyErrors) => true
            };
            _client = new RestClient(options);
            Console.WriteLine("BaseScraperService: RestClient initialized.");

            // چک کردن اتصال به دیتابیس
            try
            {
                Console.WriteLine("BaseScraperService: Checking database connection...");
                _context.Database.CanConnect();
                Console.WriteLine("BaseScraperService: Database connection successful.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BaseScraperService: Database connection failed: {ex.Message}");
            }
        }

        protected async Task<JsonNode> GetApiDataAsync(string endpoint)
        {
            Console.WriteLine($"BaseScraperService: Requesting API endpoint: {endpoint}");
            var request = new RestRequest(endpoint, Method.Get);
            request.AddHeader("Accept", "application/json, text/plain, */*");
            request.AddHeader("Accept-Language", "en-US,en;q=0.9,fa-IR;q=0.8,fa;q=0.7");
            request.AddHeader("Connection", "keep-alive");
            request.AddHeader("Referer", "https://fund.fipiran.ir/mf/list");
            request.AddHeader("Sec-Fetch-Dest", "empty");
            request.AddHeader("Sec-Fetch-Mode", "cors");
            request.AddHeader("Sec-Fetch-Site", "same-origin");
            request.AddHeader("sec-ch-ua", "\"Google Chrome\";v=\"135\", \"Not-A.Brand\";v=\"8\", \"Chromium\";v=\"135\"");
            request.AddHeader("sec-ch-ua-mobile", "?0");
            request.AddHeader("sec-ch-ua-platform", "\"Windows\"");

            try
            {
                var response = await _client.ExecuteAsync(request);
                Console.WriteLine($"BaseScraperService: API response status: {response.StatusCode}");
                if (!response.IsSuccessful)
                {
                    Console.WriteLine($"BaseScraperService: API error: {response.StatusCode} - {response.ErrorMessage}");
                    return null;
                }

                var jsonNode = JsonNode.Parse(response.Content);
                Console.WriteLine($"BaseScraperService: API data parsed successfully for {endpoint}");
                return jsonNode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"BaseScraperService: Error requesting {endpoint}: {ex.Message}");
                return null;
            }
        }
    }
}
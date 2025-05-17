using final_qualifying_work.Data;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;

namespace final_qualifying_work.Services
{
    public class AiService
    {
        private readonly AppDbContext _context;
        private readonly IHttpClientFactory _httpClientFactory;

        public AiService(AppDbContext context, IHttpClientFactory httpClientFactory)
        {
            _context = context;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<string> Process(string req)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("http://host.docker.internal:5000/");

            var response = await client.PostAsJsonAsync("/process", req);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                return result;
            }

            return response.StatusCode.ToString();
        }

        public async Task<TaskResult> Task(string req)
        {
            var client = _httpClientFactory.CreateClient();
            client.BaseAddress = new Uri("http://host.docker.internal:5000/");

            var response = await client.PostAsJsonAsync("/task", req);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadAsStringAsync();
                Console.WriteLine(result);
                TaskResult taskResult = JsonConvert.DeserializeObject<TaskResult>(result)!;
                Console.WriteLine(taskResult.content);
                return taskResult;
            }

            return new TaskResult() { content = response.StatusCode.ToString() };
        }
    }

    public class TaskResult
    {
        public string time {  get; set; }
        public string content { get; set; }
        public string category { get; set; }
        public string[] skills { get; set; }
    }
}

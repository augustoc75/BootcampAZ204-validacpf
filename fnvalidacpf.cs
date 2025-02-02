using System;
using System.IO;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Text.RegularExpressions;

namespace Bootcamp_AZ204_ValidaCPF
{
    public static class fnvalidacpf
    {
        [FunctionName("fnvalidacpf")]
        public static async Task<IActionResult> Run(
            [HttpTrigger(AuthorizationLevel.Function, "post", Route = null)] HttpRequest req,
            ILogger log)
        {
            log.LogInformation("Iniciando a validação do CPF...");

            string requestBody = await new StreamReader(req.Body).ReadToEndAsync();
            dynamic data = JsonConvert.DeserializeObject(requestBody);
            if(data == null)
            { 
                return new BadRequestObjectResult("Por favor, informe um CPF.");
            }
            string cpf = data?.cpf;         
    
            if (!IsValidCpf(cpf))
            {
                return new BadRequestObjectResult("CPF inválido.");
            }
    
        return new OkObjectResult("CPF válido.");
    }

    
    public static bool IsValidCpf(string cpf)
    {
        if (string.IsNullOrWhiteSpace(cpf))
            return false;
    
        cpf = cpf.Replace(".", "").Replace("-", "");
    
        if (cpf.Length != 11 || !Regex.IsMatch(cpf, @"^\d{11}$"))
            return false;
    
        var tempCpf = cpf.Substring(0, 9);
        var sum = 0;
    
        for (var i = 0; i < 9; i++)
            sum += int.Parse(tempCpf[i].ToString()) * (10 - i);
    
        var remainder = sum % 11;
        if (remainder < 2)
            remainder = 0;
        else
            remainder = 11 - remainder;
    
        var digit = remainder.ToString();
        tempCpf = tempCpf + digit;
        sum = 0;
    
        for (var i = 0; i < 10; i++)
            sum += int.Parse(tempCpf[i].ToString()) * (11 - i);
    
        remainder = sum % 11;
        if (remainder < 2)
            remainder = 0;
        else
            remainder = 11 - remainder;
    
        digit = digit + remainder.ToString();
    
        return cpf.EndsWith(digit);
    }
    }    
    
}

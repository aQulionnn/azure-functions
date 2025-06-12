using System.Net;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Http;

namespace CurrencyConverter.Functions.Functions;

public class CurrencyConverter
{
    private readonly Dictionary<Currency, decimal> _rates = new()
    {
        { Currency.USD, 1m },
        { Currency.KZT, 511m },
        { Currency.EUR, 0.864m }
    };
    
    [Function("CurrencyConverter")]
    public async Task<HttpResponseData> Run([HttpTrigger(AuthorizationLevel.Anonymous, "post", 
        Route = "currency-converter")] HttpRequestData req)
    {
        var data = await req.ReadFromJsonAsync<ConvertRequest>();
        if (data is null) 
            return req.CreateResponse(HttpStatusCode.BadRequest);
        
        var result = Convert(data.Amount, data.From,  data.To);
        var response = req.CreateResponse(HttpStatusCode.OK);
        
        await response.WriteAsJsonAsync(new
        {
            Amount = data.Amount,
            FromCurrency = data.From.ToString(),
            ToCurrency = data.To.ToString(),
            Converted = result
        });
        
        return response;
    }
    
    private decimal Convert(decimal amount, Currency from, Currency to)
    {
        var usdAmount = amount / _rates[from];
        return usdAmount * _rates[to];
    }
}

public record ConvertRequest(decimal Amount, Currency From, Currency To);

public enum Currency
{
    USD,
    KZT,
    EUR
}

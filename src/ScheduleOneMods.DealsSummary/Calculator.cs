namespace ScheduleOneMods.DealsSummary;

public class Calculator
{
    public const string UnknownDenomination = "?";
    
    public Calculator(Dictionary<int, string> denominations)
    {
        _denominations =
            new SortedDictionary<int, string>(denominations, Comparer<int>.Create((x, y) => y.CompareTo(x)));
    }

    private readonly SortedDictionary<int, string> _denominations;

    public Dictionary<string, int> Calculate(int amount)
    {
        var sizes = new Dictionary<string, int>();
        var remaining = amount;

        foreach (var (size, name) in _denominations)
        {
            if (remaining < size || remaining <= 0)
                continue;

            var quotient = remaining / size;
            remaining -= quotient * size;

            if (quotient > 0)
                sizes.Add(name, quotient);
        }
        
        if (remaining > 0)
            sizes.Add(UnknownDenomination, remaining);
        
        return sizes;
    }
    
    public Summary[] CalculateTotals(IEnumerable<Contract> contracts)
    {
        var aggregates = new Dictionary<string, Dictionary<string, int>>();
        var totals = new Dictionary<string, int>();
        
        foreach (var c in contracts)
        {
            foreach (var e in c.Entries)
            {
                totals.TryGetValue(e.ProductId, out var total);
                totals[e.ProductId] = total + e.Quantity;
                
                var agg = Calculate(e.Quantity);
                if (aggregates.TryGetValue(e.ProductId, out var existing))
                {
                    foreach (var a in agg)
                    {
                        existing.TryGetValue(a.Key, out var value);
                        existing[a.Key] = value + a.Value;
                    }
                }
                else
                    aggregates.Add(e.ProductId, agg);
            }
        }

        return aggregates.Select(kvp => new Summary(kvp.Key, totals[kvp.Key], kvp.Value)).ToArray();
    }
}
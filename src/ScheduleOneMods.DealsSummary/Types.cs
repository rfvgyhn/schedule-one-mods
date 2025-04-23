namespace ScheduleOneMods.DealsSummary;

public record Summary(string ProductId, int Total, Dictionary<string, int> Aggregates);
public record Entry(string ProductId, int Quantity);

public class Contract
{
    public Contract(params Entry[] entries)
    {
        Entries = entries;
    }

    public Contract(Il2CppScheduleOne.Quests.Contract contract)
    {
        var entries = new Entry[contract.ProductList.entries.Count];
        for (var i = 0; i < contract.ProductList.entries.Count; i++)
        {
            var e = contract.ProductList.entries[i];
            entries[i] = new(e.ProductID, e.Quantity);
        }

        Entries = entries;
    }

    public Entry[] Entries { get; }
}
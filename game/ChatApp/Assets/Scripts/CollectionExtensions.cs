using System;
using System.Collections.Generic;
using System.Threading.Channels;
using System.Threading.Tasks;

public static class CollectionExtensions
{
    public static async Task<List<TValue>> ToListAsync<TValue>(this ChannelReader<TValue> channel)
    {
        List<TValue> values = new List<TValue>();
        while (await channel.WaitToReadAsync())
            values.Add(await channel.ReadAsync());

        return values;
    }
}
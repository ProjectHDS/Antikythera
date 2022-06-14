﻿using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Antikythera.Utils;

public static class Utils
{
    public static double Bytes2MiB(this long bytes, int round)
        => Math.Round(bytes / 1048576.0, round);

    /// <summary>Download file by url.</summary>
    /// <param name="url"></param>
    /// <param name="header"></param>
    /// <param name="timeout">Default 8000 seconds</param>
    /// <param name="limitLen">Default 2 gigabytes</param>
    /// <returns>Download result.</returns>
    public static async Task<byte[]> UrlDownload(this string url,
        Dictionary<string, string>? header = null,
        int timeout = 8000, long limitLen = ((long)2 << 30) - 1)
    {
        // Create request
        var request = new HttpClient(new HttpClientHandler { AutomaticDecompression = DecompressionMethods.All })
        {
            Timeout = new TimeSpan(0, 0, 0, timeout),
            MaxResponseContentBufferSize = limitLen
        };
        // Default useragent
        request.DefaultRequestHeaders.Add("User-Agent", new[]
        {
            "Mozilla/5.0 (Windows NT 10.0; Win64; x64)" ,
            "AppleWebKit/537.36 (KHTML, like Gecko)" ,
            "Chrome/100.0.4896.127 Safari/537.36 Edg/100.0.1185.50"
        });
        // Append request header
        if (header is not null)
            foreach (var (k, v) in header)
                request.DefaultRequestHeaders.Add(k, v);

        // Open response stream
        var response = await request.GetByteArrayAsync(url);

        // Receive the response data
        return response;
    }

    /// <summary>Get HTML meta data.</summary>
    /// <param name="html"></param>
    /// <param name="keys"></param>
    /// <returns>HTML meta data.</returns>
    public static Dictionary<string, string> GetMetaData(this string html, params string[] keys)
    {
        var metaDict = new Dictionary<string, string>();

        foreach (var i in keys)
        {
            var pattern = i + @"=""(.*?)""(.|\s)*?content=""(.*?)"".*?>";

            // Match results
            foreach (Match j in Regex.Matches(html, pattern, RegexOptions.Multiline))
            {
                metaDict.TryAdd(j.Groups[1].Value, j.Groups[3].Value);
            }
        }

        return metaDict;
    }

    /// <summary>
    /// Can I do
    /// </summary>
    /// <param name="factor">Probability scale, default 50%.</param>
    /// <returns></returns>
    public static bool CanIDo(double factor = 0.5f) => new Random().NextDouble() >= (1 - factor);

    public static double GetGaussRandom(int seed, double miu, double sigma2)
    {
        var rand = new Random(seed);
        double r1 = rand.NextDouble();
        double r2 = rand.NextDouble();
        var r = Math.Sqrt(-2 * Math.Log10(r1) * Math.Cos(r2 * Math.PI * 2));
        var result = miu + r * sigma2;
        return result;
    }
}
